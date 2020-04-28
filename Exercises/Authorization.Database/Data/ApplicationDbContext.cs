﻿using System;
using Authorization.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Database.Data
{
    /// <summary>
    /// // Calabonga: update summary (2020-04-28 04:56 ApplicationDbContext)
    /// </summary>
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

    }
}
