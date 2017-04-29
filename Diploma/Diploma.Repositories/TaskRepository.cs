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
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext ctx = new ApplicationDbContext();

        public async Task CreateTask(UserTask userTask)
        {
            ctx.UserTasks.Add(userTask);

            await ctx.SaveChangesAsync();
        }

        public async Task UpdateTask(UserTask userTask)
        {
            ctx.Entry(userTask).State = EntityState.Modified;
            ctx.Update(userTask);

            await ctx.SaveChangesAsync();
        }

        public IEnumerable<UserTask> GetUserTasks(string userEmail)
        {
            return ctx.UserTasks.Where(x => x.AssignedTo == userEmail).ToList();
        }

        public IEnumerable<UserTask> FindBy(Func<UserTask, bool> func)
        {
            return ctx.UserTasks.Where(func).ToList();
        }
    }
}
