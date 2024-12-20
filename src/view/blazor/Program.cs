using System;
using System.Net.Http;
using System.Threading.Tasks;
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

            builder.Services.AddScoped<HttpClientInterceptor>();
            // Configure HttpClient with the interceptor
            builder.Services.AddScoped(sp =>
            {
                var interceptor = sp.GetRequiredService<HttpClientInterceptor>();

                // Assign the default HttpClientHandler as the inner handler
                interceptor.InnerHandler = new HttpClientHandler();

                return new HttpClient(interceptor)
                {
                    BaseAddress = new Uri("https://localhost:5002"),
                    Timeout = TimeSpan.FromSeconds(6)
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
