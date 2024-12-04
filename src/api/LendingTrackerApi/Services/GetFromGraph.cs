
using Azure.Identity;
using Microsoft.Graph;

namespace LendingTrackerApi.Services
{
    public class GetFromGraph : IGetFromGraph
    {
        public async Task<string?> GetPhoneNumber(string email)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var clientId = "806b17b5-8f69-46d7-b9f8-26fff192a38f";
            var tenantId = "5d3c1990-1973-4668-81a3-d858bbbec640";
            var clientSecret = "fFu8Q~z8.psMoTBzxjlV49h~JUAEHl46Wxy2jcF7";

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            var graphUser = await graphClient.Users["262159c5-1ec1-4aa1-83d8-d14720120a18@needthatback.onmicrosoft.com"].GetAsync();

            return graphUser?.MobilePhone ?? "2707049633";
        }
    }
}
