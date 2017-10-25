using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Services
{
    public class UserService
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
                .Include(i => i.Organization)
                .Include(i => i.Roles)                
                .Include(i => i.UserFolders)
                .ThenInclude(x => x.DocumentFolders)
                .Include(i => i.UserKeys)
                .FirstOrDefault(x => x.Email == email);

            return user;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _userManager.Users
                .Include(i => i.Organization)
                .Include(i => i.Roles)
                .Include(i => i.UserFolders)
                //.ThenInclude(x => x.Documents)
                .Include(i => i.UserKeys)
                .ToList();
        }

        public async Task UpdateUserByAdmin(string email, string role, Organization organization)
        {
            var user = GetUserByEmail(email);

            var newRole = _roleManager.Roles.Single(x => x.Name == role);

            if (!user.Roles.Any(x => x.RoleId == newRole.Id))
            {
                user.Roles.Clear();
                user.Roles.Add(new IdentityUserRole<string>
                {
                    RoleId = newRole.Id
                });
            }            
           
            user.OrganizationId = organization.Id;
            //user.Organization = organization;            

            await _userManager.UpdateAsync(user);
        }
    }
}
