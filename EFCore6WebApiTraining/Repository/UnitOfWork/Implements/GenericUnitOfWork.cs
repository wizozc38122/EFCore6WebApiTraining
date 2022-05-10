using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Db;
using System.Collections;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
{
    public class GenericUnitOfWork : IGenericUnitOfWork
    {
        private bool disposedValue = false;
        private readonly BloggingContext _context;
        private Hashtable? _repositories;


        public GenericUnitOfWork(BloggingContext context)
        {
            _context = context;
        }

        public IUnitOfWorkGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new UnitOfWorkGenericRepository<TEntity>(_context);
            }

            return (IUnitOfWorkGenericRepository<TEntity>)_repositories[type];
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
