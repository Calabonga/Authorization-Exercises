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
                var client = _httpClientFactory.CreateClient();
                client.SetBearerToken(model.AccessToken);

                try
                {
                    var stringAsync = await client.GetAsync("https://localhost:5001/site/secret");

                    if (stringAsync.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshToken(model.RefreshToken);
                        var stringRepeatAsync = await client.GetStringAsync("https://localhost:5001/site/secret");
                        var contentSecret = stringRepeatAsync;
                        ViewBag.Message = contentSecret;
                        return View(model);
                    }
                    ViewBag.Message = await stringAsync.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {

                    throw;
                }



            }
            catch (Exception exception)
            {

                ViewBag.Message = exception.Message;
            }
            return View(model);
        }

        private async Task RefreshToken(string refreshToken)
        {
            var refreshClient = _httpClientFactory.CreateClient();

            var parameters = new Dictionary<string, string>
            {
                ["refresh_token"] = refreshToken,
                ["grant_type"] = "refresh_token",
                ["client_id"] = "client_id_mvc",
                ["client_secret"] = "client_secret_mvc"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:10001/connect/token")
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            var basics = "client_id_mvc:client_secret_mvc";
            var encodedData = Encoding.UTF8.GetBytes(basics);
            var encodeData4Base = Convert.ToBase64String(encodedData);
            request.Headers.Add("Authorization", $"Bearer {encodeData4Base}");
            var response = await refreshClient.SendAsync(request);

            var tokenData = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenData);
            var accessTokenNew = tokenResponse.GetValueOrDefault("access_token");
            var refreshTokenNew = tokenResponse.GetValueOrDefault("refresh_token");

            await UpdateAuthContextAsync(accessTokenNew, refreshTokenNew);
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
