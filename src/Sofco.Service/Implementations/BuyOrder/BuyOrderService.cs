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
using Sofco.Domain.Models.RequestNote;
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
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "ORCOM");
            var todas = this.unitOfWork.BuyOrderRepository.GetAll(filters);
            var todasModel = todas.Select(n => new BuyOrderGridModel(n, permisos, user.Id, settings));
            return todasModel.Where(n => n.HasEditPermissions || n.HasReadPermissions).ToList();
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
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "ORCOM");
            var datos = new BuyOrderModel(order, permisos, user.Id, settings);
            if (!datos.HasEditPermissions && !datos.HasReadPermissions)
            {
                response.AddError(Resources.RequestNote.BuyOrder.NotAllowed);
                return response;
            }
            var productsNoteRequest = order.RequestNote.ProductsServices;
            var productsOrders = this.unitOfWork.BuyOrderRepository.GetAll(new BuyOrderGridFilters() { RequestNoteId = order.RequestNoteId })
                .Where(a => a.StatusId == settings.WorkflowStatusBOPendienteRecepcionFact || a.StatusId == settings.WorkflowStatusFinalizedId)
                .SelectMany(a=> a.ProductsServices);
            foreach (var item in datos.Items) //Calcularle las cantidades totales y pendientes (para la NP)
            {
                item.RequestedQuantity = productsNoteRequest.Where(a => a.Id == item.RequestNoteProductServiceId).Sum(a => a.Quantity);
                item.PendingQuantity = productsOrders.Where(a => a.Id == item.RequestNoteProductServiceId).Sum(a => a.Quantity);
            }
            response.Data = datos;

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
                var domain = model.CreateDomain(); //llena campos
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
        public void SaveChanges(BuyOrderModel model, int nextStatus)
        {
            Domain.Models.RequestNote.BuyOrder order = this.unitOfWork.BuyOrderRepository.GetById(model.Id);
            var user = userData.GetCurrentUser();
            /*if (order.StatusId == settings.WorkflowStatusBOPendienteAprobacionDAF)
            { //Acá al final no guarda nada
                if (nextStatus == settings.WorkflowStatusBOPendienteRecepcionMerc)
                {
                    //Guardar las cantidades/precios de cada ítem en BuyOrderProductService
                    foreach (var item in order.ProductsServices)
                    {
                        var prod = model.Items.SingleOrDefault(p => p.RequestNoteProductServiceId == item.RequestNoteProductServiceId);
                        if (prod != null)
                        {
                            item.Quantity = prod.Quantity;
                            item.Price = prod.Amount;
                        }
                    }
                }
            }
            else*/ if (order.StatusId == settings.WorkflowStatusBOPendienteRecepcionMerc)
            {
                if (nextStatus == settings.WorkflowStatusBOPendienteRecepcionFact)
                {
                    //Guardar las cantidades recibidas de cada ítem en BuyOrderProductService
                    foreach (var item in order.ProductsServices)
                    {
                        var prod = model.Items.SingleOrDefault(p => p.RequestNoteProductServiceId == item.RequestNoteProductServiceId);
                        if (prod != null)
                        {
                            item.DeliveredQuantity = prod.DeliveredQuantity;
                        }
                    }
                }
            }
            else if (order.StatusId == settings.WorkflowStatusBOPendienteRecepcionFact)
            {
                if (nextStatus == settings.WorkflowStatusFinalizedId)
                {
                    if(model.Invoice != null) //Si esto no viene, debería validarlo antes de pasar de estado
                    {
                        if(order.Invoices!= null && order.Invoices.Any()) //Hay una factura... no debería, pero puede pasar! La piso
                        {
                            var inv = order.Invoices.First();
                            inv = model.Invoice.CreateDomain();
                        }
                        else
                        {
                            var inv = model.Invoice.CreateDomain();
                            order.Invoices.Add(inv);
                        }
                    }
                }
            }
            this.unitOfWork.BuyOrderRepository.UpdateBuyOrder(order);
            this.unitOfWork.BuyOrderRepository.Save();
        }
    }
}
