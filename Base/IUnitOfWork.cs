using System.Threading.Tasks;

namespace UnitOfWork.Base
{
    public interface IUnitOfWork<out TDbContext>
    {
        TDbContext Context { get; }

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        void RegisterRepository<TEntity>(IGenericRepository<TEntity> repository) where TEntity : class;

        int Save();
        Task<int> SaveAsync();
    }
}