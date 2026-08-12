using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using gmvTM.Domain;

namespace gmvTM.Server.Classes.Tools
{
    //developer convenience only: opens a separate styled console listing the local endpoints
    public static class DeveloperConsoleTools
    {
        public static void Open(IEnumerable<string> addresses, string? authUsername)
        {
            if (!OperatingSystem.IsWindows())
                return;

            //integration tests boot the server in Development through an in-memory TestServer;
            //those report no explicit port and run under the testhost process, so both guards skip them
            if (Process.GetCurrentProcess().ProcessName.Contains("testhost", StringComparison.OrdinalIgnoreCase))
                return;

            List<string> urls = addresses.Where(a => !string.IsNullOrWhiteSpace(a)).Select(NormalizeAddress).Where(HasExplicitPort).ToList();
            if (!urls.Any())
                return;

            string baseUrl = urls.FirstOrDefault(u => u.StartsWith("https", StringComparison.OrdinalIgnoreCase)) ?? urls.First();

            try
            {
                string scriptPath = Path.Combine(Path.GetTempPath(), "gmvtm-developer-endpoints.ps1");
                File.WriteAllText(scriptPath, BuildScript(baseUrl, urls, authUsername), Encoding.UTF8);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -NoExit -File \"{scriptPath}\"",
                    UseShellExecute = true
                });
            }
            catch
            {
                //never let a convenience window break startup
            }
        }

        private static string NormalizeAddress(string address) =>
            address.Replace("0.0.0.0", "localhost").Replace("[::]", "localhost").TrimEnd('/');

        private static bool HasExplicitPort(string address) =>
            Uri.TryCreate(address, UriKind.Absolute, out Uri? uri) && !uri.IsDefaultPort;

        private static string BuildScript(string baseUrl, IReadOnlyList<string> urls, string? authUsername)
        {
            string hubPath = gmvDomain.AppConstants.VehiclePositionHubPath;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"$host.UI.RawUI.WindowTitle = '{Escape(gmvDomain.Messages.AppTitle)} - Developer Endpoints'");
            sb.AppendLine("Clear-Host");
            sb.AppendLine("Write-Host ''");
            sb.AppendLine("Write-Host '  ===========================================================' -ForegroundColor DarkMagenta");
            sb.AppendLine($"Write-Host '   {Escape(gmvDomain.Messages.AppTitle)} - Developer Endpoints' -ForegroundColor Magenta");
            sb.AppendLine("Write-Host '  ===========================================================' -ForegroundColor DarkMagenta");
            sb.AppendLine("Write-Host ''");

            AppendRow(sb, "Listening on", string.Join(", ", urls), "White");
            sb.AppendLine("Write-Host ''");

            AppendRow(sb, "Swagger UI", $"{baseUrl}/swagger", "Green");
            AppendRow(sb, "API (v1)", $"{baseUrl}/api/v1", "Cyan");
            AppendSubRow(sb, $"{baseUrl}/api/v1/routes");
            AppendSubRow(sb, $"{baseUrl}/api/v1/routes/F/stops");
            AppendSubRow(sb, $"{baseUrl}/api/v1/vehicles");
            AppendSubRow(sb, $"{baseUrl}/api/v1/admin/database/reseed  (POST)");
            AppendRow(sb, "OData", $"{baseUrl}/odata", "Cyan");
            AppendSubRow(sb, $"{baseUrl}/odata/$metadata");
            AppendSubRow(sb, "RouteItems, StopItems, VehicleItems, TripItems, StopPlanItems, StopTripItems");
            AppendRow(sb, "Prometheus", $"{baseUrl}/metrics", "Cyan");
            AppendSubRow(sb, "simulation counters, active simulations gauge, HTTP metrics");
            AppendRow(sb, "SignalR hub", $"{baseUrl}{hubPath}", "Cyan");
            sb.AppendLine("Write-Host ''");

            if (!string.IsNullOrWhiteSpace(authUsername))
                AppendRow(sb, "Basic auth", $"username '{authUsername}' (password in appsettings)", "Yellow");

            sb.AppendLine("Write-Host ''");
            sb.AppendLine("Write-Host '  API and OData require the Basic auth header. Swagger has an Authorize button.' -ForegroundColor DarkGray");
            sb.AppendLine("Write-Host ''");

            return sb.ToString();
        }

        private static void AppendRow(StringBuilder sb, string label, string value, string color)
        {
            sb.AppendLine($"Write-Host '  {Escape(label),-13}: ' -NoNewline -ForegroundColor Gray; Write-Host '{Escape(value)}' -ForegroundColor {color}");
        }

        private static void AppendSubRow(StringBuilder sb, string value)
        {
            sb.AppendLine($"Write-Host '                 {Escape(value)}' -ForegroundColor DarkCyan");
        }

        private static string Escape(string value) => value.Replace("'", "''");
    }
}
