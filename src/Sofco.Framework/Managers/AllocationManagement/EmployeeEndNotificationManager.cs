using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Managers.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Framework.Managers.AllocationManagement
{
    public class EmployeeEndNotificationManager : IEmployeeEndNotificationManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public EmployeeEndNotificationManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public void Save(EmployeeEndNotificationModel model)
        {
            unitOfWork.EmployeeEndNotificationRepository.Save(Translate(model));
        }

        private EmployeeEndNotification Translate(EmployeeEndNotificationModel model)
        {
            return mapper.Map<EmployeeEndNotificationModel, EmployeeEndNotification>(model);
        }
    }
}
