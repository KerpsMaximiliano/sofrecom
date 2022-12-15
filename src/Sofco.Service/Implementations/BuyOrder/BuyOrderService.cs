using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.BuyOrder
{
    public class BuyOrderService : IBuyOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<BuyOrderService> logger;
        private readonly IUserData userData;
        private readonly FileConfig fileConfig;
        private readonly AppSetting settings;
        private readonly IWorkflowStateRepository workflowStateRepository;
        public BuyOrderService(IUnitOfWork unitOfWork, ILogMailer<BuyOrderService> logger, IUserData userData,
            IOptions<FileConfig> fileOptions, IWorkflowStateRepository workflowStateRepository, IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            fileConfig = fileOptions.Value;
            settings = settingOptions.Value;
        }

        public IList<BuyOrderGridModel> GetAll(BuyOrderGridFilters filters)
        {
            var user = userData.GetCurrentUser();
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "NOPE");
            return this.unitOfWork.BuyOrderRepository.GetAll(filters)
                    .Select(n => new BuyOrderGridModel(n, permisos, user.Id, settings))
                    .Where(n => n.HasEditPermissions || n.HasReadPermissions).ToList();
        }

        public Response<BuyOrderModel> GetById(int id)
        {
            var response = new Response<BuyOrderModel>();

            var order = this.unitOfWork.BuyOrderRepository.GetById(id);
            if (order == null)
            {
                response.AddError(Resources.RequestNote.BuyOrder.NotFound);
                return response;
            }
            var user = userData.GetCurrentUser();
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "NOPE");
            var datos = new BuyOrderModel(order, permisos, user.Id, settings);
            if (!datos.HasEditPermissions && !datos.HasReadPermissions)
            {
                response.AddError(Resources.RequestNote.BuyOrder.NotAllowed);
                return response;
            }
            response.Data = datos;
            
            /* Ver qué datos extra hay que mostrar en cada estado
            if (new List<int>() {
                settings.WorkflowStatusNPPendienteAprobacionGerente,
                settings.WorkflowStatusNPPendienteAprobacionCompras,
                settings.WorkflowStatusNPPendienteAprobacionSAP
                //otros?
                }
            .Contains(order.StatusId))
            {
                response.Data.Analytics = response.Data.Analytics.Where(a => a.ManagerId == user.Id).ToList();
            }
            if (new List<int>() {
                settings.WorkflowStatusNPPendienteAprobacionDAF,
                settings.WorkflowStatusNPPendienteRecepcionMerc,
                settings.WorkflowStatusNPRecepcionParcial,
                settings.WorkflowStatusNPCerrado,
                //otros?
                }
            .Contains(order.StatusId))
            {
                response.Data.Providers = response.Data.Providers.Where(p => p.ProviderId == response.Data.ProviderSelectedId).ToList();
            }
            */
            return response;
        }
        public Response<IList<Option>> GetStates()
        {
            var states = workflowStateRepository.GetStateByWorkflowTypeCode(settings.BuyOrderWorkflowTypeCode);

            var result = states.Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
/*
            if (result.All(x => x.Id != settings.Buy))
            {
                var finalizeState = workflowStateRepository.Get(settings.WorkflowStatusNPCerrado);

                result.Add(new Option { Id = finalizeState.Id, Text = finalizeState.Name });
            }*/
            /*
            var draft = result.SingleOrDefault(x => x.Id == settings.WorkflowStatusNPBorrador);

            if (draft != null) result.Remove(draft);
            */
            return new Response<IList<Option>>
            {
                Data = result
            };
        }

        public Response<string> Add(BuyOrderModel model)
        {
            var response = new Response<string>();
            //TODO: agregar validaciones
            //validation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.StatusId = settings.WorkflowStatusBOPendienteAprobacionDAF;
                domain.InWorkflowProcess = true;

                var workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.BuyOrderWorkflowId);

                domain.WorkflowId = workflow.Id;

                unitOfWork.BuyOrderRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.RequestNote.BuyOrder.AddSuccess);

                response.Data = domain.Id.ToString();
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }
    }
}
