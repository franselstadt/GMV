using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Workers
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
                    $"SELECT 1 FROM {gmvDomain.Tables.Routes} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT {gmvDomain.Columns.StopCode} FROM {gmvDomain.Tables.Stops} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT SpecialAlert FROM {gmvDomain.Tables.Stops} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {gmvDomain.Tables.StopPlans} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {gmvDomain.Tables.StopTrips} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT 1 FROM {gmvDomain.Tables.Vehicles} LIMIT 1",
                    cancellationToken);

                await this.Context.Database.ExecuteSqlRawAsync(
                    $"SELECT StartStopID FROM {gmvDomain.Tables.Trips} LIMIT 1",
                    cancellationToken);

                int legacyColumnCount = await this.Context.Database
                    .SqlQueryRaw<int>($"SELECT COUNT(*) AS \"Value\" FROM pragma_table_info('{gmvDomain.Tables.Trips}') WHERE name = 'ScheduleRunIndex'")
                    .SingleAsync(cancellationToken);

                if (legacyColumnCount > 0)
                    await this.Context.Database.EnsureDeletedAsync(cancellationToken);
            }
            catch
            {
                await this.Context.Database.EnsureDeletedAsync(cancellationToken);
            }

            await this.Context.Database.EnsureCreatedAsync(cancellationToken);

            if (await this.Context.Routes.AnyAsync(cancellationToken))
            {
                this.logger.LogInformation(gmvDomain.Messages.LogDatabaseAlreadySeeded);
                return;
            }

            string seedPath = ResolveSeedFilePath();
            await using FileStream stream = File.OpenRead(seedPath);


            RouteSeedDocument seed = await JsonSerializer.DeserializeAsync<RouteSeedDocument>(stream,JsonOptions,cancellationToken)
                ?? throw new InvalidOperationException(string.Format(gmvDomain.Messages.SeedFileDeserializeFailed, seedPath));

            RouteItem route = RouteFactory.CreateItem(seed.Route.ShortName, seed.Route.LongName, seed.Route.Color, seed.Route.EncodedPolyline);

            await this.Context.Routes.AddAsync(route, cancellationToken);
            await this.Context.SaveChangesAsync(cancellationToken);

            VehicleItem vehicle = VehicleFactory.CreateItem(
                gmvDomain.Resources.SampleFleetCode,
                gmvDomain.Resources.SampleVehicleMake,
                gmvDomain.Resources.SampleVehicleModel,
                gmvDomain.Resources.SampleLicensePlate,
                gmvDomain.Resources.SampleVehicleCapacity,
                gmvDomain.Resources.SampleVehicleModelYear,
                wheelchairAccessible: true);


            await this.Context.Vehicles.AddAsync(vehicle, cancellationToken);
            await this.Context.SaveChangesAsync(cancellationToken);

            Dictionary<string, StopItem> stopsByCode = new Dictionary<string, StopItem>(StringComparer.OrdinalIgnoreCase);
            
            foreach (SeedStop seedStop in seed.Stops.OrderBy(s => s.Sequence))
            {
                StopItem stop = StopFactory.CreateItem(
                    route.ID,
                    seedStop.StopCode,
                    seedStop.Name,
                    seedStop.Latitude,
                    seedStop.Longitude,
                    seedStop.Sequence,
                    seedStop.SpecialAlert);
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

            foreach (SeedStopPlan row in seed.StopPlans.OrderBy(s => s.Sequence))
            {
                if (!seededSequences.Add(row.Sequence))
                    continue;

                if (!stopsByCode.TryGetValue(row.StopCode, out StopItem? catalogStop))
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.SeedUnknownStopCode, row.StopCode));

                if (!arrivalSecondsBySequence.TryGetValue(row.Sequence, out int arrivalSeconds))
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.NoBaselineArrivalSeconds, row.Sequence));

                await this.Context.StopPlans.AddAsync(
                    StopPlanFactory.CreateItem(catalogStop.ID, row.Sequence, arrivalSeconds),
                    cancellationToken);

                planCount++;
            }

            await this.Context.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation(
                gmvDomain.Messages.LogSeededRoute,
                route.ShortName,
                vehicle.FleetCode,
                seed.Stops.Count,
                planCount,
                gmvDomain.AppConstants.DefaultAverageMph,
                gmvDomain.AppConstants.DefaultAverageDwellSeconds);
        }

        private Dictionary<int, int> BuildArrivalSecondsBySequence(string encodedPolyline, IReadOnlyList<StopItem> stopsInOrder)
        {
            if (stopsInOrder.Count < 2)
                throw new InvalidOperationException(gmvDomain.Messages.RouteNeedsTwoStopsForArrivalSeconds);

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

                bySequence[stopsInOrder[i].Sequence] = gmvDomain.AppConstants.ArrivalSecondsFromPrevious(
                    meters,
                    gmvDomain.AppConstants.DefaultAverageMph,
                    gmvDomain.AppConstants.DefaultAverageDwellSeconds);
            }

            return bySequence;
        }


        private static string ResolveSeedFilePath()
        {
            string[] candidates =
            [
                Path.Combine(AppContext.BaseDirectory, gmvDomain.Resources.SeedOutputFolder, gmvDomain.Resources.SeedFileName),
                Path.Combine(AppContext.BaseDirectory, gmvDomain.Resources.SeedFileName),
                Path.Combine(Directory.GetCurrentDirectory(), gmvDomain.Resources.SeedOutputFolder, gmvDomain.Resources.SeedFileName),
                Path.Combine(Directory.GetCurrentDirectory(), "gmvTM.Domain","Infrastructure",gmvDomain.Resources.SeedOutputFolder, gmvDomain.Resources.SeedFileName)
            ];

            foreach (string path in candidates)
            {
                if (File.Exists(path))
                    return path;
            }

            throw new FileNotFoundException(
                string.Format(gmvDomain.Messages.SeedFileNotFound, gmvDomain.Resources.SeedFileName, gmvDomain.Resources.SeedOutputFolder));
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

            public string? SpecialAlert { get; set; }
        }

        private sealed class SeedStopPlan
        {
            public string StopCode { get; set; } = null!;

            public int Sequence { get; set; }
        }

        #endregion
    }
}
