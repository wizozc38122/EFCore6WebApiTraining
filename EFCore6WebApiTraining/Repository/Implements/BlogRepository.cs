using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using EFCore6WebApiTraining.Repository.Db;

namespace EFCore6WebApiTraining.Repository.Implements
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BloggingContext _context;

        public BlogRepository(BloggingContext context)
        {
            _context = context;
        }
        
        // C
        public async Task<bool> CreateAsync(Blog blog)
        {
            _context.Set<Blog>().Add(blog);
            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        // R
        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Set<Blog>().FindAsync(id);
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Set<Blog>().ToListAsync();
        }

        // U
        public async Task<bool> UpdateAsync(Blog blog)
        {
            _context.Set<Blog>().Update(blog);
            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        // D
        public async Task<bool> DeleteByIdAsync(int id)
        {
            var target = await GetByIdAsync(id);
            if (target == null) { return false; }

            _context.Set<Blog>().Remove(target);

            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        //public async Task<Blog?> FindAsync(Expression<Func<Blog,bool>> match)
        //{
        //    return await _context.Set<Blog>().SingleOrDefaultAsync(match);
        //}
    }
}
