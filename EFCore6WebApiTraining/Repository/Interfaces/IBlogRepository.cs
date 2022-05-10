using EFCore6WebApiTraining.Repository.Entities;

namespace EFCore6WebApiTraining.Repository.Interfaces
{
    public interface IBlogRepository
    {
        // C
        Task<bool> CreateAsync(Blog blog);

        // R
        Task<Blog?> GetByIdAsync(int Id);

        Task<IEnumerable<Blog>> GetAllAsync();

        // U
        Task<bool> UpdateAsync(Blog blog);


        // Delete
        Task<bool> DeleteByIdAsync(int id);

    }
}

