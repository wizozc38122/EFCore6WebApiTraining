namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IUnitOfWorkBlogRepository Blogs { get; }
        public IUnitOfWorkPostRepository Posts { get;  }
        Task<int> SaveChangeAsync();
    }
}
