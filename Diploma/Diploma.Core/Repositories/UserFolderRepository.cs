using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Repositories
{
    public class UserFolderRepository : BaseRepository<UserFolder>
    {
        public UserFolderRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task<UserFolder> Get(string id)
        {
            return await ctx.UserFolders
                .Include(x => x.ApplicationUser)
                .Include(x => x.DocumentFolders)
                .ThenInclude(x => x.Document)
                .FirstOrDefaultAsync(x => x.Id == int.Parse(id));
        }

        public override async Task<IEnumerable<UserFolder>> GetAll()
        {
            throw new NotImplementedException();
        }

        public override async Task Remove(string id)
        {
            throw new NotImplementedException();
        }
    }
}
