using BlazorApplicationInsights;
using BlazorApplicationInsights.Models;
using Blazored.LocalStorage;
using LendingView.Servcies;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;


namespace LendingView
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");


            // Build the configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Default settings
                .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: false, reloadOnChange: true) // Environment-specific
                .Build();
            // Add the configuration to the services
            builder.Services.AddSingleton<IConfiguration>(config);

            //support authentication
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.AdditionalScopesToConsent.Add("profile");
                options.ProviderOptions.AdditionalScopesToConsent.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://needthatback.onmicrosoft.com/lender/lender");
                options.ProviderOptions.LoginMode = "redirect";
            });

            //add observability
            builder.Services.AddBlazorApplicationInsights(x =>
            {
                x.ConnectionString = "InstrumentationKey=34e61473-fb29-43a9-90c6-b5324284ff96;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/;ApplicationId=a1daa383-6400-4e5b-828b-c5ebd3060666";
            },
            async applicationInsights =>
            {
            var telemetryItem = new TelemetryItem()
            {
                Tags = new Dictionary<string, object?>()
                {
                    { "ai.cloud.role", "Lending View SPA" },
                }
                };

                await applicationInsights.AddTelemetryInitializer(telemetryItem);

            });
            //support inital page load
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //support authorization
            builder.Services.AddAuthorizationCore(options =>
            {
                 options.AddPolicy("admin", policy =>
                    policy.RequireClaim("extension_userRole", "admin")); // Ensure the token has the required scope
            });
            // authorization handler
            builder.Services.AddTransient<ApiAuthorizationMessageHandler>();


            //inject services
            
            //http service
            builder.Services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri(config["HostAPI:BaseURL"]);
            }).AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

            // Set the named HttpClient as the default
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

            //add local storage support
            builder.Services.AddBlazoredLocalStorage();

            //add ui support mudblazor
            builder.Services.AddMudServices();
            
            //user service
            builder.Services.AddScoped<IUserService, UserService>();

            //injec the api data service
            builder.Services.AddScoped<LendingTrackerService>();
                    
            //run the app
           await builder.Build().RunAsync();
        }
    }
}
