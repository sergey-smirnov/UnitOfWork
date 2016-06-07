using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitOfWork.Base;

namespace UnitOfWork.Impl
{
    public class GenericRepository<TEntity, TDbContext> : IGenericRepository<TEntity> where TEntity : class
        where TDbContext : DbContext
    {
        #region Private members

        private readonly IUnitOfWork<TDbContext> _unitOfWork;
        private readonly DbSet<TEntity> _dbSet;

        #endregion

        public GenericRepository(IUnitOfWork<TDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbSet = _unitOfWork.Context.Set<TEntity>();
        }

        #region IGenericRepository

        #region GetById

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        #endregion

        #region Query

        public virtual IQueryable<TEntity> Query<TField>(Expression<Func<TEntity, TField>> orderBy, Expression<Func<TEntity, bool>> predicate = null, bool descending = false)
        {
            var query = Query(predicate);

            query = @descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query;
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet.AsQueryable();
        }

        #endregion

        #region Find

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate = null)
        {
            return Query(predicate).FirstOrDefault();
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await Query().FirstOrDefaultAsync();
            }
            return await Query(predicate).FirstOrDefaultAsync();
        }

        public virtual TEntityDto Find<TEntityDto>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntityDto>> mapper) where TEntityDto : class
        {
            var entity = _dbSet.FirstOrDefault(predicate);
            if (entity != null)
            {
                var method = mapper.Compile();
                return method.Invoke(entity);
            }

            return null;
        }

        #endregion

        #region FindAll

        public virtual List<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate != null ? Query(predicate).ToList() : _dbSet.ToList();
        }

        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return await Query(predicate).ToListAsync();
            }

            return await _dbSet.ToListAsync();
        }

        #endregion

        #region Insert

        public virtual TEntity Insert(TEntity entity, bool submitChanges = true)
        {
            var result = _dbSet.Add(entity);

            if (submitChanges)
            {
                SaveChanges();
            }

            return result;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, bool submitChanges = true)
        {
            var result = _dbSet.Add(entity);

            if (submitChanges)
            {
                await SaveChangesAsync();
            }

            return result;
        }

        #endregion

        #region Delete

        public virtual void Delete(object id, bool submitChanges = true)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);

            if (submitChanges)
            {
                SaveChanges();
            }
        }

        public virtual void Delete(TEntity entityToDelete, bool submitChanges = true)
        {
            if (_unitOfWork.Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);

            if (submitChanges)
            {
                _unitOfWork.Context.SaveChanges();
            }
        }

        public virtual async Task DeleteAsync(object id, bool submitChanges = true)
        {
            var entityToDelete = await _dbSet.FindAsync(id);

            await DeleteAsync(entityToDelete);

            if (submitChanges)
            {
                await SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete, bool submitChanges = true)
        {
            if (_unitOfWork.Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);

            if (submitChanges)
            {
                await SaveChangesAsync();
            }
        }

        #endregion

        #region Update

        public virtual int Update(TEntity entityToUpdate, bool submitChanges = true)
        {
            _dbSet.Attach(entityToUpdate);
            _unitOfWork.Context.Entry(entityToUpdate).State = EntityState.Modified;

            return submitChanges ? SaveChanges() : 0;
        }

        public virtual async Task<int> UpdateAsync(TEntity entityToUpdate, bool submitChanges = true)
        {
            _dbSet.Attach(entityToUpdate);
            _unitOfWork.Context.Entry(entityToUpdate).State = EntityState.Modified;

            return await (submitChanges ? SaveChangesAsync() : Task.FromResult(0));
        }

        #endregion

        #region SaveChanges

        public int SaveChanges()
        {
            return _unitOfWork.Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _unitOfWork.Context.SaveChangesAsync();
        }

        #endregion

        #endregion
    }
}