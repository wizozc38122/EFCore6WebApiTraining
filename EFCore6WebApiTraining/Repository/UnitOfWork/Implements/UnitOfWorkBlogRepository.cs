using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Db;
using EFCore6WebApiTraining.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
{
    public class UnitOfWorkBlogRepository : IUnitOfWorkBlogRepository
    {
        private readonly BloggingContext _context;
        private readonly DbSet<Blog> _dbSet;

        public UnitOfWorkBlogRepository(BloggingContext context)
        {
            _context = context;
            _dbSet = _context.Set<Blog>();
        }

        // C
        public void Create(Blog blog)
        {
            _dbSet.Add(blog);
        }

        // R
        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(blog => blog.Posts).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _dbSet.Include(blog => blog.Posts).ToListAsync();
        }

        // U
        public void Update(Blog blog)
        {
            _dbSet.Update(blog);
        }

        // D
        public void Delete(Blog blog)
        {
            _dbSet.Remove(blog);

        }
    }
}
