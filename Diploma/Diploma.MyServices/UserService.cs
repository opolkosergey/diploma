using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            var user = _userManager.Users
                .Include(i => i.Roles)                
                .Include(i => i.UserFolders)
                .ThenInclude(x => x.Documents)
                .Include(i => i.UserKeys)
                .First(x => x.Email == email);

            return user;
        }

        public async Task UpdateUserByAdmin(string email, string role, string organizationName)
        {
            var user = GetUserByEmail(email);

            var newRole = _roleManager.Roles.Single(x => x.Name == role);
            //var newOrganization = 

            user.Roles.Clear();
            user.Roles.Add(new IdentityUserRole<string>
            {
                RoleId = newRole.Id
            });

            await _userManager.UpdateAsync(user);
        }
    }
}
