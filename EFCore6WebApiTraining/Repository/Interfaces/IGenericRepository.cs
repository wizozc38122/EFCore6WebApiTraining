namespace EFCore6WebApiTraining.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // C
        Task<bool> CreateAsync(TEntity entity);

        // R
        Task<TEntity?> GetByIdAsync(int Id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        // U
        Task<bool> UpdateAsync(TEntity entity);

        // Delete
        Task<bool> DeleteByIdAsync(int id);
    }
}
