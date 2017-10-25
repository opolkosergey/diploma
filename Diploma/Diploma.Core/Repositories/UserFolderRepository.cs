using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;

namespace Diploma.Core.Repositories
{
    public class UserFolderRepository : BaseRepository<UserFolder>
    {
        public UserFolderRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task<UserFolder> Get(string id)
        {
            throw new NotImplementedException();
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
