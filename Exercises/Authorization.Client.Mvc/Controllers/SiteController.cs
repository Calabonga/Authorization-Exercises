using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Authorization.Client.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", model.AccessToken);
                var stringAsync = await client.GetStringAsync("https://localhost:5001/site/secret");
                ViewBag.Message = stringAsync;
            }
            catch (Exception exception)
            {

                ViewBag.Message = exception.Message;
            }


            return View(model);
        }

        [Authorize(Policy = "HasDateOfBirth")]
        [Route("[action]")]
        public async Task<IActionResult> Secret1()
        {
            var model = new ClaimManager(HttpContext, User);
            
            return View(model);
        }

        [Authorize(Policy = "OlderThan")]
        [Route("[action]")]
        public async Task<IActionResult> Secret2()
        {
            var model = new ClaimManager(HttpContext, User);
            
            return View(model);
        }
    }
}
