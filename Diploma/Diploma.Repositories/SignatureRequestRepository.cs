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

        public async Task Create(SignatureWarrant signatureWarrant)
        {
            ctx.SignatureWarrants.Add(signatureWarrant);

            await ctx.SaveChangesAsync();
        }

        public async Task DeleteSignatureWarrant(int signatureWarrantId)
        {
            var sw = await ctx.SignatureWarrants.SingleAsync(x => x.Id == signatureWarrantId);

            ctx.Remove(sw);

            await ctx.SaveChangesAsync();
        }

        public IEnumerable<SignatureRequest> GetAll()
        {
            return ctx.SignatureRequests.ToList();
        }
    }
}
