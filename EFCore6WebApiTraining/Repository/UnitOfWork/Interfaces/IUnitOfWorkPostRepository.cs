using EFCore6WebApiTraining.Repository.Entities;

namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkPostRepository : IUnitOfWorkGenericRepository<Post>
    {
    }
}
