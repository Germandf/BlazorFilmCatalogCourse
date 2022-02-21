using BlazorFilmCatalogCourse.Client.Helpers;
using BlazorFilmCatalogCourse.Client.Services.Data;
using BlazorFilmCatalogCourse.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Client.Auth
{
    public class JWTAuthProvider : AuthenticationStateProvider, ILoginService
    {
        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";
        private IJSRuntime _js;
        private HttpClient _httpClient;
        private IRepository _repository;
        private AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JWTAuthProvider(IJSRuntime js, HttpClient httpClient, IRepository repository)
        {
            _js = js;
            _httpClient = httpClient;
            _repository = repository;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.GetFromLocalStorage(TOKENKEY);
            if (string.IsNullOrEmpty(token))
            {
                return _anonymous;
            }
            var expirationTimeString = await _js.GetFromLocalStorage(EXPIRATIONTOKENKEY);
            DateTime expirationTime;
            if(DateTime.TryParse(expirationTimeString, out expirationTime))
            {
                if (TokenIsExpired(expirationTime))
                {
                    await ClearToken();
                    return _anonymous;
                }
                if (HasToRenewToken(expirationTime))
                {
                    token = await RenewToken(token);
                }
            }
            return BuildAuthenticationState(token);
        }

        private bool TokenIsExpired(DateTime expirationTime)
        {
            return expirationTime <= DateTime.UtcNow;
        }

        private bool HasToRenewToken(DateTime expirationTime)
        {
            return expirationTime.Subtract(DateTime.UtcNow) < TimeSpan.FromMinutes(5);
        }

        private async Task<string> RenewToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var httpResponse = await _repository.Get<UserTokenDTO>("api/cuentas/renovarToken");
            var newToken = httpResponse.Response;
            await _js.SetInLocalStorage(TOKENKEY, newToken.Token);
            await _js.SetInLocalStorage(EXPIRATIONTOKENKEY, newToken.Expiration.ToString());
            return newToken.Token;
        }

        public async Task ManageTokenRenew()
        {
            var expirationTimeString = await _js.GetFromLocalStorage(EXPIRATIONTOKENKEY);
            DateTime expirationTime;
            if (DateTime.TryParse(expirationTimeString, out expirationTime))
            {
                if (TokenIsExpired(expirationTime))
                {
                    await Logout();
                }
                if (HasToRenewToken(expirationTime))
                {
                    var token = await _js.GetFromLocalStorage(TOKENKEY);
                    var newToken = await RenewToken(token);
                    var authState = BuildAuthenticationState(newToken);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
            if(roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                    foreach(var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async Task Login(UserTokenDTO userTokenDTO)
        {
            await _js.SetInLocalStorage(TOKENKEY, userTokenDTO.Token);
            await _js.SetInLocalStorage(EXPIRATIONTOKENKEY, userTokenDTO.Expiration.ToString());
            var authState = BuildAuthenticationState(userTokenDTO.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await ClearToken();
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        private async Task ClearToken()
        {
            await _js.RemoveItem(TOKENKEY);
            await _js.RemoveItem(EXPIRATIONTOKENKEY);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
