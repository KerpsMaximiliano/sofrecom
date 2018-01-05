using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ICollection<Analytic> GetAll()
        {
            return unitOfWork.AnalyticRepository.GetAllReadOnly();
        }

        public AnalyticOptions GetOptions()
        {
            var options = new AnalyticOptions();

            options.Activities = unitOfWork.UtilsRepository.GetImputationNumbers().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Directors = unitOfWork.UserRepository.GetDirectors().Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
            options.Managers = unitOfWork.UserRepository.GetManagers().Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
            options.Currencies = unitOfWork.UtilsRepository.GetCurrencies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Solutions = unitOfWork.UtilsRepository.GetSolutions().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Technologies = unitOfWork.UtilsRepository.GetTechnologies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Products = unitOfWork.UtilsRepository.GetProducts().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.ClientGroups = unitOfWork.UtilsRepository.GetClientGroups().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.PurchaseOrders = unitOfWork.UtilsRepository.GetPurchaseOrders().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.SoftwareLaws = unitOfWork.UtilsRepository.GetSoftwareLaws().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.ServiceTypes = unitOfWork.UtilsRepository.GetServiceTypes().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();

            return options;
        }

        public Response<Analytic> GetById(int id)
        {
            var response = new Response<Analytic>();

            response.Data = AnalyticValidationHelper.Find(response, unitOfWork.AnalyticRepository, id);

            return response;
        }

        public Response<IList<Allocation>> GetResources(int id)
        {
            var response = new Response<IList<Allocation>>();

            var resources = unitOfWork.AnalyticRepository.GetResources(id);

            if (!resources.Any())
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }

            response.Data = resources;

            return response;
        }

        public Response<Analytic> Add(Analytic analytic)
        {
            var response = new Response<Analytic>();

            AnalyticValidationHelper.CheckTitle(response, analytic, unitOfWork.CostCenterRepository);
            AnalyticValidationHelper.CheckIfTitleExist(response, analytic, unitOfWork.AnalyticRepository);
            AnalyticValidationHelper.CheckNameAndDescription(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckCurrency(response, analytic);
            AnalyticValidationHelper.CheckDates(response, analytic);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.AnalyticRepository.Insert(analytic);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Analytic.SaveSuccess);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
            }
    

            return response;
        }

        public Response<string> GetNewTitle(int costCenterId)
        {
            var response = new Response<string>();

            if (costCenterId == 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.CostCenterIsRequired);
                return response;
            }

            var analytic = unitOfWork.AnalyticRepository.GetLastAnalytic(costCenterId);

            if (analytic == null)
            {
                var costCenter = unitOfWork.CostCenterRepository.GetSingle(x => x.Id == costCenterId);
                response.Data = $"{costCenter.Code}-{costCenter.Letter}0001";
            }
            else
            {
                analytic.TitleId++;
                response.Data = $"{analytic.CostCenter.Code}-{analytic.CostCenter.Letter}{analytic.TitleId.ToString().PadLeft(4, '0')}";
            }

            return response;
        }
    }
}
