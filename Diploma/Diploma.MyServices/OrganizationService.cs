using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Repositories;
using Diploma.Repositories.Abstracts;
using Diploma.Services.Abstracts;

namespace Diploma.Services
{
    public class OrganizationService : IOrganizationsService
    {
        private readonly IOrganizationRepository _organizationRepository = new OrganizationRepository();

        public async Task<IEnumerable<Organization>> GetAll()
        {
            return await _organizationRepository.GetAll();
        }

        public async Task CreateOrganization(Organization organization)
        {
            await _organizationRepository.CreateOrganization(organization);
        }

        public async Task<Organization> GetOrganizationByName(string name)
        {
            return await _organizationRepository.GetOrganizationByName(name);
        }
    }
}
