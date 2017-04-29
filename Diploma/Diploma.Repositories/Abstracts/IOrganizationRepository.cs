using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Repositories.Abstracts
{
    public interface IOrganizationRepository
    {
        Task CreateOrganization(Organization organization);

        Task<IEnumerable<Organization>> GetAll();

        Task<Organization> GetOrganizationByName(string name);
    }
}
