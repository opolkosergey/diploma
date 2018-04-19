using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>
    {
        public OrganizationRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task<Organization> Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<IEnumerable<Organization>> GetAll()
        {
            return await ctx.Organizations.Include(i => i.Employees).ToListAsync();
        }

        public override async Task Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<Organization> FindBy(Func<Organization, bool> func)
        {
            return ctx.Organizations
                .Include(i => i.Employees)
                .Where(func);
        }
    }
}
