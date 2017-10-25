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
    public class DocumentRepository : BaseRepository<Document>
    {
        public DocumentRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task Remove(string id)
        {
            var document = await ctx.IncomingSignatureRequests.SingleAsync(x => x.Id == int.Parse(id));

            ctx.Remove(document);

            await ctx.SaveChangesAsync();
        }

        public override async Task<Document> Get(string id)
        {
            var document = await ctx.Documents
                .Include(i => i.DocumentAccesses)
                .FirstOrDefaultAsync(d => d.Id == int.Parse(id));

            return document;
        }

        public override IEnumerable<Document> FindBy(Func<Document, bool> func)
        {
            return ctx.Documents
                .Include(i => i.DocumentAccesses)
                .Where(func)
                .ToList();
        }

        public override async Task<IEnumerable<Document>> GetAll()
        {
            return await ctx.Documents
                .Include(i => i.DocumentAccesses)
                .ToListAsync();
        }
    }
}
