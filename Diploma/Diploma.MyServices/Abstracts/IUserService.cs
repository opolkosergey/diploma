using System;
using System.Collections.Generic;
using System.Text;
using Diploma.Core.Models;

namespace Diploma.Services.Abstracts
{
    public interface IUserService
    {
        ApplicationUser GetUserByEmail(string email);
    }
}
