using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Repositories
{
    public class SignatureWarrantRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task CreateSignatureWarrant(SignatureWarrant signatureWarrant)
        {
            ctx.SignatureWarrants.Add(signatureWarrant);

            await ctx.SaveChangesAsync();
        }

        public IEnumerable<SignatureWarrant> GetUserSignatureWarrants(string userEmail)
        {
            return ctx.SignatureWarrants.Where(x => x.ToUser == userEmail).ToList();
        }
    }
}
