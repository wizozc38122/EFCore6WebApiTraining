using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Db;
using EFCore6WebApiTraining.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
{
    public class UnitOfWorkPostRepository : IUnitOfWorkPostRepository
    {
        private readonly BloggingContext _context;
        private readonly DbSet<Post> _dbSet;

        public UnitOfWorkPostRepository(BloggingContext context)
        {
            _context = context;
            _dbSet = _context.Set<Post>();
        }

        // C
        public void Create(Post post)
        {
            _dbSet.Add(post);
        }

        // R
        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // U
        public void Update(Post post)
        {
            _dbSet.Update(post);
        }

        // D
        public void Delete(Post post)
        {
            _dbSet.Remove(post);

        }
    }
}
