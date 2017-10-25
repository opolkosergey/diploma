using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories;
using Diploma.Core.Repositories.Abstracts;
using Diploma.Core.Repositories.Abstracts.Base;

namespace Diploma.Core.Services
{
    public class OrganizationService
    {
        private readonly BaseRepository<Organization> _organizationRepository;

        public OrganizationService(BaseRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public async Task<IEnumerable<Organization>> GetAll()
        {
            return await _organizationRepository.GetAll();
        }

        public async Task CreateOrganization(Organization organization)
        {
            await _organizationRepository.Add(organization);
        }

        public Organization GetOrganizationByName(string name)
        {
            return _organizationRepository.FindBy(org => org.Name == name).Single();
        }
    }
}
