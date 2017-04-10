using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await ctx.Documents
                .Include(i => i.DocumentAccesses)
                .ToListAsync();
        }

        public async Task Save(Document document, ApplicationUser user)
        {
            user.Documents.Add(document);
            ctx.Users.Attach(user);
            await ctx.SaveChangesAsync();
        }

        public async Task<Document> Get(ApplicationUser user, int documentId)
        {
            var document = await ctx.Documents
                .Include(i => i.DocumentAccesses)
                .FirstOrDefaultAsync(d => d.Id == documentId);            

            if (document == null)
            {
                return null;
            }

            return document;
        }
    }
}
