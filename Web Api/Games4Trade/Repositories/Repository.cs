using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Games4Trade.Data;
using Microsoft.EntityFrameworkCore;
using Games4Trade.Interfaces.Repositories;

namespace Games4Trade.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationContext Context;

        public Repository(ApplicationContext context)
        {
            Context = context;
        }

        public async Task AddASync(TEntity entity) => await Context.Set<TEntity>().AddAsync(entity);

        public async Task AddRangeASync(IEnumerable<TEntity> entities) => await Context.Set<TEntity>().AddRangeAsync(entities);
        
        public async Task<IEnumerable<TEntity>> FindASync(Expression<Func<TEntity, bool>> predicate) =>
            await Context.Set<TEntity>().Where(predicate).ToListAsync();

        public async Task<TEntity> GetASync(int id) => await Context.Set<TEntity>().FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllASync() => await Context.Set<TEntity>().ToListAsync();
        
        public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().RemoveRange(entities);
       
        public virtual async Task<int> SaveChangesASync() => await Context.SaveChangesAsync();
    }
}
