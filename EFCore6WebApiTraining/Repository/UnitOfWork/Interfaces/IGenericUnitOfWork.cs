namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
{
    public interface IGenericUnitOfWork : IDisposable 
    {
        IUnitOfWorkGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task<int> SaveChangeAsync();
    }
}
