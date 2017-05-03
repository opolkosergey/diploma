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
    public class DocumentRepository //: IDocumentRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await ctx.Documents
                .Include(i => i.DocumentAccesses)
                .ToListAsync();
        }

        public async Task Save(UserFolder documentFolder, ApplicationUser user)
        {           
            ctx.UserFolders.Update(documentFolder);
            ctx.Entry(user).State = EntityState.Unchanged;
            ctx.Entry(user.Roles.First()).State = EntityState.Unchanged;

            //user.OrganizationId = 1;
            //user.Organization = ctx.Organizations.First();
            //ctx.Entry(user).State = EntityState.Modified;
            //ctx.Users.Update(user);

            await ctx.SaveChangesAsync();
        }

        public async Task SaveFolder(ApplicationUser user)
        {
            ctx.Entry(user).State = EntityState.Unchanged;
            ctx.Entry(user.Roles.First()).State = EntityState.Unchanged;

            await ctx.SaveChangesAsync();
        }

        public async Task Update(Document document)
        {
            ctx.Entry(document).State = EntityState.Modified;
            ctx.Documents.Update(document);

            await ctx.SaveChangesAsync();
        }

        public async Task<Document> Get(int documentId)
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

        public IEnumerable<Document> FindBy(Func<Document, bool> func)
        {
            return ctx.Documents
                .Include(i => i.DocumentAccesses)
                .Where(func)
                .ToList();
        }
    }
}
