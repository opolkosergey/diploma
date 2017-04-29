using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Services.Abstracts
{
    public interface IUserService
    {
        ApplicationUser GetUserByEmail(string email);

        Task UpdateUserByAdmin(string email, string role, string organizationName);
    }
}
