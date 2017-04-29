using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task CreateOrganization(Organization organization)
        {
            ctx.Organizations.Add(organization);

            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Organization>> GetAll()
        {
            return  await ctx.Organizations.Include(i => i.Employees).ToListAsync();
        }

        public async Task<Organization> GetOrganizationByName(string name)
        {
            return await ctx.Organizations
                .Include(i => i.Employees)
                .SingleAsync(x => x.Name == name);
        }
    }
}
