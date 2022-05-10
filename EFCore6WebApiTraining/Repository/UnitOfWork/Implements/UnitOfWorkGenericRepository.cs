using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Db;
using Microsoft.EntityFrameworkCore;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
{
    public class UnitOfWorkGenericRepository<TEntity> : IUnitOfWorkGenericRepository<TEntity> where TEntity : class
    {
        private readonly BloggingContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public UnitOfWorkGenericRepository(BloggingContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // C
        public void Create(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        // R
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // U
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        // D
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);

        }
    }
}
