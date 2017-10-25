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
    public class SignatureWarrantRepository : BaseRepository<SignatureWarrant>
    {
        public SignatureWarrantRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override IEnumerable<SignatureWarrant> FindBy(Func<SignatureWarrant, bool> func)
        {
            return ctx.SignatureWarrants
                .Include(x => x.ApplicationUser)
                .Where(func)
                .ToList();
        }

        public override async Task<SignatureWarrant> Get(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<SignatureWarrant>> GetAll()
        {
            throw new NotImplementedException();
        }

        public override async Task Remove(string id)
        {
            throw new NotImplementedException();
        }
    }
}
