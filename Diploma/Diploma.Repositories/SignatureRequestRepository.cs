using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Repositories
{
    public class SignatureRequestRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task Create(IncomingSignatureRequest signatureRequest)
        {
            ctx.IncomingSignatureRequests.Add(signatureRequest);

            await ctx.SaveChangesAsync();
        }

        public async Task DeleteSignatureRequest(int signatureWarrantId)
        {
            var sw = await ctx.IncomingSignatureRequests.SingleAsync(x => x.Id == signatureWarrantId);

            ctx.Remove(sw);

            await ctx.SaveChangesAsync();
        }

        public IEnumerable<IncomingSignatureRequest> GetAll()
        {
            return ctx.IncomingSignatureRequests.ToList();
        }
    }
}
