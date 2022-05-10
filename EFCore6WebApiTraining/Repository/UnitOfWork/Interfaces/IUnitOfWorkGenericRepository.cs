namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkGenericRepository<TEntity> where TEntity : class
    {
        // C
        void Create(TEntity entity);

        // R
        Task<TEntity?> GetByIdAsync(int Id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        // U
        void Update(TEntity entity);

        // Delete
        void Delete(TEntity entity);
    }
}
