using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.Workflow.Conditions
{
    public class PendingApproveManagerToRrhhCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;

        public PendingApproveManagerToRrhhCondition(IUnitOfWork unitOfWork, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        // TODO: cambiar al parametro de la db cuando se haga la historia
        private const decimal ValueA = 1000;

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var currentUser = userData.GetCurrentUser();

            var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

            var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

            var advancement = (Advancement)entity;

            if (advancement.Ammount <= ValueA)
            {
                return true;
            }
            else
            {
                if (sectors.Any(x => x.ResponsableUserId == entity.UserApplicantId || x.ResponsableUserId == currentUser.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
