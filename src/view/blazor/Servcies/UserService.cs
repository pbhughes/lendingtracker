using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LendingView.Servcies
{
    public class UserService : IUserService
    {
        private AuthenticationStateProvider _auth;
        private IAccessTokenProvider _tp;
        private NavigationManager _nav;
        private string _token;
        private string _error;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        public event Action? AuthenticationStateChanged;
        private bool _isAuthenticated;

        public UserService(AuthenticationStateProvider auth, IAccessTokenProvider tp, NavigationManager nav)
        {
            _auth = auth;
            _tp = tp;
            _nav = nav;
            _tokenHandler = new JwtSecurityTokenHandler();
        }


        public async Task GetAccessToken(bool triggerLogin = false)
        {
            
            var tokenResult = await _tp.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[] { "https://needthatback.onmicrosoft.com/lender/lender" }
                
            });

            if (tokenResult.TryGetToken(out var token))
            {
                _token = token.Value;
            }
            else
            {
                if (!triggerLogin)
                {
                    _token = string.Empty;
                }
                else
                {
                    _ = tokenResult.Status switch
                    {
                        AccessTokenResultStatus.RequiresRedirect => "Redirect required. Navigating...",
                        AccessTokenResultStatus.Success => null,
                        _ => "An unknown error occurred."
                    };

                    if (tokenResult.Status == AccessTokenResultStatus.RequiresRedirect)
                    {
                        _nav.NavigateTo(tokenResult.InteractiveRequestUrl!);
                    }
                }
            }
        }

        public async Task<bool> IsAdmin()
        {

            // await GetAccessToken();
            var authState = await _auth.GetAuthenticationStateAsync();

            if (! authState.User.Identity.IsAuthenticated)
            {
                return false;
            }
            await GetAccessToken(triggerLogin: false);

            var token = _tokenHandler.ReadJwtToken(_token);
            var user = new ClaimsPrincipal(new ClaimsIdentity(token.Claims));

            return user?.HasClaim("extension_userRole", "admin") ?? false;

        }

        public async Task TriggerLoginNotification()
        {
            if (AuthenticationStateChanged != null)
                AuthenticationStateChanged();
        }
    }
}
