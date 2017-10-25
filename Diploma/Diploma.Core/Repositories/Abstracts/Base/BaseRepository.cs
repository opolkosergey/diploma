using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Repositories.Abstracts.Base
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext ctx;

        protected BaseRepository(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
        }

        public virtual async Task Add(T entity)
        {
            ctx.Set<T>().Add(entity);

            await ctx.SaveChangesAsync();
        }

        public virtual async Task Update(T entity)
        {
            ctx.Entry(entity).State = EntityState.Modified;
            ctx.Set<T>().Update(entity);

            await ctx.SaveChangesAsync();
        }

        public virtual IEnumerable<T> FindBy(Func<T, bool> func)
        {
            return ctx.Set<T>().Where(func).ToList();
        }

        public abstract Task<T> Get(string id);

        public abstract Task<IEnumerable<T>> GetAll();

        public abstract Task Remove(string id);
    }
}
