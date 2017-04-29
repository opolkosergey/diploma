using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Services.Abstracts
{
    public interface IOrganizationsService
    {
        Task<IEnumerable<Organization>> GetAll();

        Task CreateOrganization(Organization organization);

        Task<Organization> GetOrganizationByName(string name);
    }
}
