using System;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Database.Entities
{
    /// <summary>
    /// // Calabonga: update summary (2020-04-28 04:54 ApplicationUser)
    /// </summary>
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}
