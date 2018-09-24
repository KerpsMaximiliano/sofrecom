using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Framework.Managers.UserApprovers
{
    public class LicenseApproverEmployeeManager : ILicenseApproverEmployeeManager
    {
        private readonly IUserApproverEmployeeRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserApproverManager approverManager;

        private readonly IUserData userData;

        private readonly UserApproverType Type = UserApproverType.LicenseAuthorizer;

        public LicenseApproverEmployeeManager(IUserApproverEmployeeRepository repository, IUnitOfWork unitOfWork, IUserData userData, IUserApproverManager approverManager)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.approverManager = approverManager;
        }

        public List<UserApproverEmployee> Get(UserApproverQuery query)
        {
            var result = new List<UserApproverEmployee>();

            if (approverManager.IsDirector())
            {
                result = GetByCurrentSectors();
            }

            if (!approverManager.IsManager()) return result;

            result.AddRange(approverManager.GetByCurrentManager(query, Type));

            return result;
        }

        private List<UserApproverEmployee> GetByCurrentSectors()
        {
            var currentUser = userData.GetCurrentUser();

            var sectorIds = unitOfWork.SectorRepository.GetByUserId(currentUser.Id)
                .Select(s => s.Id)
                .ToList();

            return approverManager.ResolveData(repository
                .GetByAllocationManagersBySectors(sectorIds, Type));
        }
    }
}
