using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using LendingView.Servcies;
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
            builder.RootComponents.Add<App>("app");

 
            // Build the configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Default settings
                .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: false, reloadOnChange: true) // Environment-specific
                .Build();

            var apiHostUrl = config["HostAPI:BaseURL"];

            // Add the configuration to the services
            builder.Services.AddSingleton<IConfiguration>(config);

            //add local storage support
            builder.Services.AddBlazoredLocalStorage();


            builder.Services.AddScoped<HttpClientInterceptor>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Configure HttpClient with the interceptor
            builder.Services.AddScoped(sp =>
            {
                var interceptor = sp.GetRequiredService<HttpClientInterceptor>();

                // Assign the default HttpClientHandler as the inner handler
                interceptor.InnerHandler = new HttpClientHandler();

                double leadTime = 60;
                if(builder.HostEnvironment.Environment.ToLower() == "production")
                {
                    leadTime = 30;

                }
                return new HttpClient(interceptor)
                {
                    BaseAddress = new Uri($"{apiHostUrl}"),
                    Timeout = TimeSpan.FromSeconds(leadTime)
                };
            });

                     
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.AdditionalScopesToConsent.Add("profile"); 
                options.ProviderOptions.AdditionalScopesToConsent.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://needthatback.onmicrosoft.com/lender/lender");
            });



            builder.Services.AddMudServices();
           
           
            await builder.Build().RunAsync();
        }
    }
}
