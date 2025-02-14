using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace LendingTrackerApi.Services
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
       private readonly IConfiguration _configuration;

        public TelemetryInitializer(IConfiguration config)
        {
            _configuration = config;
        }
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = $"{_configuration["ApplicationInsights:CloudRoleName"]}";
            }
        }
    }
}
