using Sofco.Core.DAL.Admin;

namespace Sofco.DAL
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository();
    }
}
