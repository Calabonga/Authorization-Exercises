using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Authorization.IdentityServer.ViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.IdentityServer.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(
            IIdentityServerInteractionService interactionService,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Route("[action]")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutResult = await _interactionService.GetLogoutContextAsync(logoutId);
            if (string.IsNullOrEmpty(logoutResult.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Site");
            }

            return Redirect(logoutResult.PostLogoutRedirectUri);
        }

        [Route("[action]")]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                Password = "123qwe", 
                ReturnUrl = returnUrl,
                UserName = "User"
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", "User not found");
                return View(model);
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (signinResult.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            ModelState.AddModelError("UserName", "Something went wrong");
            return View(model);
        }
    }
}
