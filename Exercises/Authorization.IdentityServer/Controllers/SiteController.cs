using Microsoft.AspNetCore.Mvc;

namespace Authorization.IdentityServer.Controllers
{
    public class SiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
