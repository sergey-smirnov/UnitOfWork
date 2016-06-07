using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using UnitOfWork.Base;

namespace UnitOfWork.Impl
{
    public class UnitOfWork<TDbContext> : IDisposable, IUnitOfWork<TDbContext> where TDbContext : DbContext, new()
    {
        #region Private Fields

        private bool _disposed;
        private readonly Dictionary<string, object> _repositories = new Dictionary<string, object>();

        #endregion

        #region Properties

        public TDbContext Context { get; }

        #endregion

        #region Public methods

        public UnitOfWork(TDbContext context)
        {
            Context = context;
        }

        public UnitOfWork()
        {
            Context = new TDbContext();
        }

        public int Save()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var entityName = typeof(TEntity).FullName;
            GenericRepository<TEntity, TDbContext> repository = null;

            if (!_repositories.ContainsKey(entityName))
            {
                repository = new GenericRepository<TEntity, TDbContext>(this);
                _repositories.Add(entityName, repository);
            }

            return repository ?? (IGenericRepository<TEntity>)_repositories[entityName];
        }

        #endregion

        #region IDisposable


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}