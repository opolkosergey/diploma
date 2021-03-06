﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<ApplicationUser> GetUser(ClaimsPrincipal claimsPrincipal) => await _userManager.GetUserAsync(claimsPrincipal);        

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.Users
                .Include(i => i.Organization)
                .Include(i => i.Roles)                
                .Include(i => i.UserFolders)
                .ThenInclude(x => x.DocumentFolders)
                .Include(i => i.UserKeys)
                .FirstOrDefaultAsync(x => x.Email == email);            
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            return await _userManager.Users
                .Include(i => i.Organization)
                .Include(i => i.Roles)
                .Include(i => i.UserFolders)                
                .Include(i => i.UserKeys)
                .ToListAsync();
        }

        public IQueryable<IdentityRole> GetRoles() =>_roleManager.Roles;       

        public async Task UpdateUserByAdmin(string email, string role, Organization organization)
        {
            var user = await GetUserByEmail(email);

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

            await _userManager.UpdateAsync(user);
        }
    }
}
