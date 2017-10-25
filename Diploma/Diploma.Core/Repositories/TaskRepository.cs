using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts;
using Diploma.Core.Repositories.Abstracts.Base;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Repositories
{
    public class TaskRepository : BaseRepository<UserTask>
    {
        public TaskRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public override async Task<UserTask> Get(string id)
        {
            return await ctx.UserTasks.FirstOrDefaultAsync(x => x.Id == int.Parse(id));
        }

        public override async Task<IEnumerable<UserTask>> GetAll()
        {
            return await ctx.UserTasks.ToListAsync();
        }

        public override async Task Remove(string id)
        {
            var userTask = await ctx.IncomingSignatureRequests.SingleAsync(x => x.Id == int.Parse(id));

            ctx.Remove(userTask);

            await ctx.SaveChangesAsync();
        }
    }
}
