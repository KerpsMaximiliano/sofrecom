using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.Workflow;
using Sofco.Service.Implementations.Admin;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowStateService : IWorkflowStateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CategoryService> logger;

        public WorkflowStateService(IUnitOfWork unitOfWork, ILogMailer<CategoryService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public IList<WorkflowState> GetAll()
        {
            var list = unitOfWork.WorkflowStateRepository.GetAll();

            return list;
        }
    }
}
