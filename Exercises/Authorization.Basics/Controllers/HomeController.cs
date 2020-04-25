using Microsoft.AspNetCore.Mvc;

namespace Authorization.Basics.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
