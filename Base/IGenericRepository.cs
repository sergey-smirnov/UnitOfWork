using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UnitOfWork.Base
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete, bool submitChanges = true);
        void Delete(object id, bool submitChanges = true);
        Task DeleteAsync(TEntity entityToDelete, bool submitChanges = true);
        Task DeleteAsync(object id, bool submitChanges = true);

        TEntity Find(Expression<Func<TEntity, bool>> predicate = null);
        TEntityDto Find<TEntityDto>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntityDto>> mapper) where TEntityDto : class;
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate = null);

        List<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null);
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate = null);

        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);

        TEntity Insert(TEntity entity, bool submitChanges = true);
        Task<TEntity> InsertAsync(TEntity entity, bool submitChanges = true);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null);
        IQueryable<TEntity> Query<TField>(Expression<Func<TEntity, TField>> orderBy, Expression<Func<TEntity, bool>> predicate = null, bool descending = false);

        int SaveChanges();
        Task<int> SaveChangesAsync();

        int Update(TEntity entityToUpdate, bool submitChanges = true);
        Task<int> UpdateAsync(TEntity entityToUpdate, bool submitChanges = true);
    }
}