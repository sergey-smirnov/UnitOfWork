using System.Threading.Tasks;

namespace UnitOfWork.Base
{
    public interface IUnitOfWork<out TDbContext>
    {
        TDbContext Context { get; }

        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        int Save();
        Task<int> SaveAsync();
    }
}