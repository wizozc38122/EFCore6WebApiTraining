using EFCore6WebApiTraining.Repository.Interfaces;
using EFCore6WebApiTraining.Repository.Db;
using Microsoft.EntityFrameworkCore;

namespace EFCore6WebApiTraining.Repository.Implements
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly BloggingContext _context;

        public GenericRepository(BloggingContext context)
        {
            _context = context;
        }
        
        // C
        public async Task<bool> CreateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        // R
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        // U
        public async Task<bool> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        // D
        public async Task<bool> DeleteByIdAsync(int id)
        {
            var target = await GetByIdAsync(id);
            if (target == null) { return false; }

            _context.Set<TEntity>().Remove(target);

            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

    }
}
