using System;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.Helpers
{
    public static class RolesInitializer
    {
        private static readonly string[] Roles = Enum.GetNames(typeof(UserRole));

        public static async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (!dbContext.UserRoles.Any())
                {
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    foreach (var role in Roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
                }
            }
        }
    }
}
