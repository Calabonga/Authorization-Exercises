using System.ComponentModel.DataAnnotations;

namespace Authorization.Database.Controllers
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
}