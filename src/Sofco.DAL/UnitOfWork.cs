using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Admin;

namespace Sofco.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SofcoContext context;

        private IUserRepository userRepository;

        public UnitOfWork(SofcoContext context)
        {
            this.context = context;
        }

        public IUserRepository UserRepository()
        {
            return userRepository ?? (userRepository = new UserRepository(context));
        }
    }
}
