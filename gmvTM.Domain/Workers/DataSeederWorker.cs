using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Base;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{
    public sealed class DataSeederWorker : BaseWorker, IDataSeederWorker
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        private readonly ILogger<DataSeederWorker> logger;
        private readonly IPolylineDecoderWorker polylineDecoder;
        private readonly IRoutePathBuilderWorker pathBuilder;

        public DataSeederWorker(DatabaseContext context, ILogger<DataSeederWorker> logger, IPolylineDecoderWorker polylineDecoder, IRoutePathBuilderWorker pathBuilder): base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.polylineDecoder = polylineDecoder
                ?? throw new ArgumentNullException(nameof(polylineDecoder));

            this.pathBuilder = pathBuilder
                ?? throw new ArgumentNullException(nameof(pathBuilder));
        }

        public void Seed()
        {
            this.SeedAsync().GetAwaiter().GetResult();
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {Tables.Routes} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT {Columns.StopCode} FROM {Tables.Stops} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {Tables.StopPlans} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {Tables.StopTrips} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {Tables.Vehicles} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT StartStopID FROM {Tables.Trips} LIMIT 1",
                    cancellationToken);
            }
            catch
            {
                await this.Context.Database.EnsureDeletedAsync(cancellationToken);
            }

            await this.Context.Database.EnsureCreatedAsync(cancellationToken);

            if (await this.Context.Routes.AnyAsync(cancellationToken))
            {
                this.logger.LogInformation("Database already seeded");
                return;
            }

            string seedPath = ResolveSeedFilePath();
            await using FileStream stream = File.OpenRead(seedPath);


            RouteSeedDocument seed = await JsonSerializer.DeserializeAsync<RouteSeedDocument>(stream,JsonOptions,cancellationToken)
                ?? throw new InvalidOperationException($"Failed to deserialize seed file '{seedPath}'.");

            RouteItem route = new RouteItem
            {
                ShortName = seed.Route.ShortName,
                LongName = seed.Route.LongName,
                Color = seed.Route.Color,
                EncodedPolyline = seed.Route.EncodedPolyline
            };

            await this.Context.Routes.AddAsync(route, cancellationToken);
            await this.Context.SaveChangesAsync(cancellationToken);

            VehicleItem vehicle = new VehicleItem
            {
                FleetCode = Resources.SampleFleetCode,
                Make = Resources.SampleVehicleMake,
                Model = Resources.SampleVehicleModel,
                LicensePlate = Resources.SampleLicensePlate,
                Capacity = Resources.SampleVehicleCapacity,
                ModelYear = Resources.SampleVehicleModelYear,
                WheelchairAccessible = true
            };


            await this.Context.Vehicles.AddAsync(vehicle, cancellationToken);
            await this.Context.SaveChangesAsync(cancellationToken);

            Dictionary<string, StopItem> stopsByCode = new Dictionary<string, StopItem>(StringComparer.OrdinalIgnoreCase);
            
            foreach (SeedStop seedStop in seed.Stops.OrderBy(s => s.Sequence))
            {
                StopItem stop = new StopItem
                {
                    RouteID = route.ID,
                    StopCode = seedStop.StopCode,
                    Name = seedStop.Name,
                    Latitude = seedStop.Latitude,
                    Longitude = seedStop.Longitude,
                    Sequence = seedStop.Sequence
                };
                await this.Context.Stops.AddAsync(stop, cancellationToken);
            }

            await this.Context.SaveChangesAsync(cancellationToken);

            foreach (StopItem stop in await this.Context.Stops.AsNoTracking().ToListAsync(cancellationToken))
            {
                stopsByCode[stop.StopCode] = stop;
            }

            List<StopItem> stopsInOrder = stopsByCode.Values.OrderBy(s => s.Sequence).ToList();

            Dictionary<int, int> arrivalSecondsBySequence = BuildArrivalSecondsBySequence(
                route.EncodedPolyline,
                stopsInOrder);

            HashSet<int> seededSequences = new HashSet<int>();
            int planCount = 0;

            foreach (SeedStopPlan row in seed.StopPlans.OrderBy(s => s.Sequence).ThenBy(s => s.RunIndex))
            {
                if (!seededSequences.Add(row.Sequence))
                    continue;

                if (!stopsByCode.TryGetValue(row.StopCode, out StopItem? catalogStop))
                    throw new InvalidOperationException(
                        $"Scheduled stop references unknown stop code '{row.StopCode}'.");

                if (!arrivalSecondsBySequence.TryGetValue(row.Sequence, out int arrivalSeconds))
                    throw new InvalidOperationException(
                        $"No baseline arrival seconds for sequence {row.Sequence}.");

                await this.Context.StopPlans.AddAsync(
                    new StopPlanItem
                    {
                        StopID = catalogStop.ID,
                        Sequence = row.Sequence,
                        ArrivalSeconds = arrivalSeconds
                    },
                    cancellationToken);

                planCount++;
            }

            await this.Context.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation(
                "Seeded route {ShortName} with vehicle {FleetCode}, {StopCount} stops, and {ScheduleCount} scheduled stops (arrival seconds at {Mph} mph + {Dwell}s dwell).",
                route.ShortName,
                vehicle.FleetCode,
                seed.Stops.Count,
                planCount,
                AppConstants.DefaultAverageMph,
                AppConstants.DefaultAverageDwellSeconds);
        }

        private Dictionary<int, int> BuildArrivalSecondsBySequence(string encodedPolyline, IReadOnlyList<StopItem> stopsInOrder)
        {
            if (stopsInOrder.Count < 2)
                throw new InvalidOperationException("Route needs at least two stops to build arrival seconds.");

            RoutePathViewItem path = this.pathBuilder.Build(this.polylineDecoder.Decode(encodedPolyline));
            double[] alongPath = new double[stopsInOrder.Count];

            for (int i = 0; i < stopsInOrder.Count; i++)
            {
                StopItem stop = stopsInOrder[i];
                alongPath[i] = path.NearestDistanceMeters(
                    new CoordinatesViewItem(stop.Latitude, stop.Longitude));

                if (i > 0 && alongPath[i] < alongPath[i - 1])
                    alongPath[i] = alongPath[i - 1];
            }

            Dictionary<int, int> bySequence = new Dictionary<int, int>();

            for (int i = 0; i < stopsInOrder.Count; i++)
            {
                int previous = i == 0 ? stopsInOrder.Count - 1 : i - 1;
                double meters;


                if (i == 0)
                {
                    meters = RoutePathViewItem.HaversineMeters(
                        new CoordinatesViewItem(stopsInOrder[previous].Latitude, stopsInOrder[previous].Longitude),
                        new CoordinatesViewItem(stopsInOrder[i].Latitude, stopsInOrder[i].Longitude));
                }
                else
                    meters = Math.Max(0, alongPath[i] - alongPath[previous]);

                bySequence[stopsInOrder[i].Sequence] = AppConstants.ArrivalSecondsFromPrevious(
                    meters,
                    AppConstants.DefaultAverageMph,
                    AppConstants.DefaultAverageDwellSeconds);
            }

            return bySequence;
        }


        //copied from claude
        private static string ResolveSeedFilePath()
        {
            string[] candidates =
            [
                Path.Combine(AppContext.BaseDirectory, Resources.SeedOutputFolder, Resources.SeedFileName),
                Path.Combine(AppContext.BaseDirectory, Resources.SeedFileName),
                Path.Combine(Directory.GetCurrentDirectory(), Resources.SeedOutputFolder, Resources.SeedFileName),
                Path.Combine(Directory.GetCurrentDirectory(), "gmvTM.Domain","Infrastructure",Resources.SeedOutputFolder, Resources.SeedFileName)
            ];

            foreach (string path in candidates)
            {
                if (File.Exists(path))
                    return path;
            }

            throw new FileNotFoundException(
                $"culd not find {Resources.SeedFileName}. Expected under {Resources.SeedOutputFolder}/ next to the this assembly.");
        }

        #region objects for deserializing the seed file
        private sealed class RouteSeedDocument
        {
            public SeedRoute Route { get; set; } = null!;
            public List<SeedStop> Stops { get; set; } = [];
            public List<SeedStopPlan> StopPlans { get; set; } = [];
        }

        private sealed class SeedRoute
        {
            public string ShortName { get; set; } = null!;
            public string LongName { get; set; } = null!;
            public string? Color { get; set; }
            public string EncodedPolyline { get; set; } = null!;
        }

        private sealed class SeedStop
        {
            public string StopCode { get; set; } = null!;
            public string Name { get; set; } = null!;

            [System.Text.Json.Serialization.JsonPropertyName("lat")]
            public double Latitude { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("lon")]
            public double Longitude { get; set; }

            public int Sequence { get; set; }
        }

        private sealed class SeedStopPlan
        {
            public string StopCode { get; set; } = null!;

            public int Sequence { get; set; }

            public int RunIndex { get; set; }

            public string RunLabel { get; set; } = null!;
        }

        #endregion
    }
}
