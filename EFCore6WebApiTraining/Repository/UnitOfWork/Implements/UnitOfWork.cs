using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Db;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposedValue = false;
        private readonly BloggingContext _context;
        public IUnitOfWorkBlogRepository Blogs { get; private set; }
        public IUnitOfWorkPostRepository Posts { get; private set; }


        public UnitOfWork(BloggingContext context)
        {
            _context = context;

            Blogs = new UnitOfWorkBlogRepository(_context);
            Posts = new UnitOfWorkPostRepository(_context);
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }


        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
