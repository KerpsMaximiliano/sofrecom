using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class AllocationCleanJobService : IAllocationCleanJobService
    {
        private readonly IUnitOfWork unitOfWork;

        public AllocationCleanJobService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Clean()
        {
            unitOfWork.AllocationRepository.Clean();
        }
    }
}
