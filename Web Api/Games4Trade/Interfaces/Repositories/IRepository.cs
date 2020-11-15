using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Games4Trade.Interfaces.Repositories
{
    public interface IRepository <TEntity> where TEntity : class
    {
        Task<TEntity> GetASync(int id);
        Task<IEnumerable<TEntity>> GetAllASync();
        Task<IEnumerable<TEntity>> FindASync(Expression<Func<TEntity, bool>> predicate);
        Task AddASync(TEntity entity);
        Task AddRangeASync(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesASync();
    }
}
