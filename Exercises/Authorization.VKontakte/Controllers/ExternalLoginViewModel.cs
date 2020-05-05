using System.ComponentModel.DataAnnotations;

namespace Authorization.VKontakte.Controllers
{
    public class ExternalLoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string ReturnUrl { get; set; }
    }
}