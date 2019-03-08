using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.Admin;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowStateService : IWorkflowStateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CategoryService> logger;
        private readonly IUserData userData;

        public WorkflowStateService(IUnitOfWork unitOfWork, ILogMailer<CategoryService> logger, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
        }

        public IList<WorkflowState> GetAll()
        {
            var list = unitOfWork.WorkflowStateRepository.GetAll();

            return list;
        }

        public IList<WorkflowStateTypeListItemModel> GetTypes()
        {
            var list = Enum.GetValues(typeof(WorkflowStateType))
                                     .Cast<WorkflowStateType>()
                                     .ToList();

            var typesList = list.Select(t => new WorkflowStateTypeListItemModel { Id = (int)t, Text = t.ToString() })
                               .OrderBy(x => x.Text).ToList();

            return typesList;
        }

        public Response<WorkflowState> Active(int id, bool active)
        {
            var response = new Response<WorkflowState>();
            var entity = unitOfWork.WorkflowStateRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                unitOfWork.WorkflowStateRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.AddSuccess(active ? Resources.Workflow.Workflow.StateEnabled : Resources.Workflow.Workflow.StateDisabled);
                return response;
            }

            response.AddError(Resources.Workflow.Workflow.StateNotFound);
            return response;
        }

        public Response<WorkflowStateModel> GetById(int id)
        {
            var response = new Response<WorkflowStateModel>();
            var state = unitOfWork.WorkflowStateRepository.Get(id);

            if (state != null)
            {
                response.Data = MapState(state);
                return response;
            }

            response.AddError(Resources.Workflow.Workflow.StateNotFound);
            return response;
        }

        public Response Update(WorkflowStateModel model)
        {
            var response = new Response();
            var entity = unitOfWork.WorkflowStateRepository.GetSingle(x => x.Id == model.Id);

            if (entity == null)
            {
                response.AddError(Resources.Workflow.Workflow.StateNotFound);
                return response;
            }

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                entity.Name = model.Name;
                entity.ActionName = model.ActionName;
                entity.Type = (WorkflowStateType)model.IdType;
                entity.ModifiedById = currentUser.Id;
                entity.ModifiedAt = DateTime.UtcNow;

                unitOfWork.WorkflowStateRepository.Update(entity);
                unitOfWork.Save();
                response.AddSuccess(Resources.Workflow.Workflow.StateUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Add(WorkflowStateModel model)
        {
            var response = new Response();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                var WorkflowState = new WorkflowState
                {
                    Name = model.Name,
                    ActionName = model.ActionName,
                    Type = (WorkflowStateType)model.IdType,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = currentUser.Id,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedById = currentUser.Id
                };

                unitOfWork.WorkflowStateRepository.Insert(WorkflowState);
                unitOfWork.Save();

                response.AddSuccess(Resources.Workflow.Workflow.StateCreated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private WorkflowStateModel MapState(WorkflowState state)
        {
            var model = new WorkflowStateModel
            {
                Id = state.Id,
                Name = state.Name,
                ActionName = state.ActionName,
                IdType = (int)state.Type
            };

            return model;
        }

        private void Validate(WorkflowStateModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                response.AddError(Resources.Workflow.Workflow.StateNameRequired);

            if (string.IsNullOrWhiteSpace(model.ActionName))
                response.AddError(Resources.Workflow.Workflow.StateTextButtonRequired);

            if (model.IdType <= 0)
                response.AddError(Resources.Workflow.Workflow.StateTypeRequired);
        }
    }
}
