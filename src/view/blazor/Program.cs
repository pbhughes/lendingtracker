using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;


namespace LendingView
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.AdditionalScopesToConsent.Add("profile");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://needthatback.onmicrosoft.com/lender/lender");
            });

            // Add Microsoft Graph client
            builder.Services.AddScoped(sp =>
            {
                var accessTokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
                return new GraphServiceClient(new DelegateAuthenticationProvider(async request =>
                {
                    var result = await accessTokenProvider.RequestAccessToken();
                    if (result.TryGetToken(out var token))
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
                    }
                }));
            });

            await builder.Build().RunAsync();
        }
    }
}
