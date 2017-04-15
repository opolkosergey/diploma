using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diploma.Core.Models;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            var user = _userManager.Users
                .Include(i => i.Roles)
                .Include(i => i.UserFolders)
                .First(x => x.Email == email);

            return user;
        }
    }
}
