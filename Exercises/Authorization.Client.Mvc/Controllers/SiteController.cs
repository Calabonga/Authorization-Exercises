using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Authorization.Client.Mvc.ViewModels;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Authorization.Client.Mvc.Controllers
{
    [Route("[controller]")]
    public class SiteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SiteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> Secret()
        {
            var model = new ClaimManager(HttpContext, User);

            try
            {
                ViewBag.Message = await GetSecretAsync(model);
                return View(model);
            }
            catch (Exception exception)
            {
                await RefreshToken(model.RefreshToken);
                var model2 = new ClaimManager(HttpContext, User);
                ViewBag.Message = await GetSecretAsync(model2);
            }
            return View(model);
        }

        private async Task<string> GetSecretAsync(ClaimManager model)
        {
            var client = _httpClientFactory.CreateClient();
            client.SetBearerToken(model.AccessToken);
            return await client.GetStringAsync("https://localhost:5001/site/secret");
        }

        private async Task RefreshToken(string refreshToken)
        {
            var refreshClient = _httpClientFactory.CreateClient();
            var resultRefreshTokenAsync = await refreshClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = "https://localhost:10001/connect/token",
                ClientId = "client_id_mvc",
                ClientSecret = "client_secret_mvc",
                RefreshToken = refreshToken,
                Scope = "openid ordersAPI offline_access"
            });

            //var parameters = new Dictionary<string, string>
            //{
            //    ["refresh_token"] = refreshToken,
            //    ["grant_type"] = "refresh_token",
            //    ["client_id"] = "client_id_mvc",
            //    ["client_secret"] = "client_secret_mvc"
            //};
            //var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:10001/connect/token")
            //{
            //    Content = new FormUrlEncodedContent(parameters)
            //};
            //var basics = "client_id_mvc:client_secret_mvc";
            //var encodedData = Encoding.UTF8.GetBytes(basics);
            //var encodeData4Base = Convert.ToBase64String(encodedData);
            //request.Headers.Add("Authorization", $"Bearer {encodeData4Base}");
            //var response = await refreshClient.SendAsync(request);

            //var tokenData = await response.Content.ReadAsStringAsync();
            //var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenData);
            //var accessTokenNew = tokenResponse.GetValueOrDefault("access_token");
            //var refreshTokenNew = tokenResponse.GetValueOrDefault("refresh_token");

            await UpdateAuthContextAsync(resultRefreshTokenAsync.AccessToken, resultRefreshTokenAsync.RefreshToken);
        }

        private async Task UpdateAuthContextAsync(string accessTokenNew, string refreshTokenNew)
        {
            var authenticate = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            authenticate.Properties.UpdateTokenValue("access_token", accessTokenNew);
            authenticate.Properties.UpdateTokenValue("refresh_token", refreshTokenNew);

            await HttpContext.SignInAsync(authenticate.Principal, authenticate.Properties);
        }

        [Authorize(Policy = "HasDateOfBirth")]
        [Route("[action]")]
        public IActionResult Secret1()
        {
            var model = new ClaimManager(HttpContext, User);

            return View("Secret", model);
        }

        [Authorize(Policy = "OlderThan")]
        [Route("[action]")]
        public IActionResult Secret2()
        {
            var model = new ClaimManager(HttpContext, User);

            return View("Secret", model);
        }
    }
}
