using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace LendingView.Servcies
{
    public class ApiAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;

        public ApiAuthorizationMessageHandler(IAccessTokenProvider tokenProvider, NavigationManager navigation)
        {
            _tokenProvider = tokenProvider;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenResult = await _tokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
            else
            {
                // Redirect to login if token is missing
                _navigation.NavigateTo("authentication/login");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
