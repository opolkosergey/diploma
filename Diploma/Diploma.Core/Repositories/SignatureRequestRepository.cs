using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Repositories
{
    public class SignatureRequestRepository : BaseRepository<IncomingSignatureRequest>
    {
        public SignatureRequestRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task Remove(string id)
        {
            var sw = await ctx.IncomingSignatureRequests.SingleAsync(x => x.Id == int.Parse(id));

            ctx.Remove(sw);

            await ctx.SaveChangesAsync();
        }

        public override async Task<IncomingSignatureRequest> Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<IEnumerable<IncomingSignatureRequest>> GetAll()
        {
            return await ctx.IncomingSignatureRequests
                .Include(x => x.ApplicationUser)
                .ThenInclude(x => x.UserKeys)
                .ToListAsync();
        }
    }
}
