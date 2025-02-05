﻿using System.Collections.Generic;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowService
    {
        void DoTransition<TEntity, THistory>(WorkflowChangeStatusParameters parameters, Response<TransitionSuccessModel> response) 
            where TEntity : WorkflowEntity where THistory : WorkflowHistory;

        Response<IList<TransitionItemModel>> GetPossibleTransitions<T>(TransitionParameters parameters) where T : WorkflowEntity;

        Response<IList<WorkflowListItemModel>> GetAll();

        Response<WorkflowDetailModel> GetById(int workflowId);

        Response<WorkflowListItemModel> Post(WorkflowAddModel model);

        Response<IList<Option>> GetTypes();

        Response<IList<Option>> GetStates();

        Response Put(int id, WorkflowAddModel model);

        void DoTransitionWithoutFlow<TEntity, THistory>(WorkflowChangeStatusMasiveParameters parameter, Response<TransitionSuccessModel> response)
            where TEntity : WorkflowEntity where THistory : WorkflowHistory;
    }
}
