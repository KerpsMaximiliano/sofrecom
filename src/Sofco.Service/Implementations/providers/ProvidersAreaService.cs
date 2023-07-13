using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;
using Sofco.Framework.Workflow.Notifications;
using Sofco.Core.Services;
using Sofco.Core.DAL.Provider;

namespace Sofco.Service.Implementations.providers
{
    public class ProvidersAreaService : IProvidersAreaService
    {
        private readonly IProviderAreaRepository providersAreaRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ProvidersAreaService> logger;

        public ProvidersAreaService(IProviderAreaRepository providers, IUnitOfWork unitOfWork,ILogMailer<ProvidersAreaService> logger)
        {
            providersAreaRepository = providers;
            this.unitOfWork = unitOfWork;
            this.logger = logger;


        }
        public Response<IList<ProvidersAreaModel>> GetAll()
        {
            var response = new Response<IList<ProvidersAreaModel>>();

            response.Data = providersAreaRepository.GetAll().Select(x => new ProvidersAreaModel(x)).ToList();

            return response;
        }

        public Response<ProvidersAreaModel> GetById(int providersAreaid)
        {

            var response = new Response<ProvidersAreaModel>();
            var provider = providersAreaRepository.GetById(providersAreaid);
            if(provider != null) 
            {
                response.Data = new ProvidersAreaModel(provider);
            }

            return response;
        }

        public Response<ProvidersAreaModel> Post(ProvidersAreaModel provider)
        {
            var response = new Response<ProvidersAreaModel>();
            var providerArea = new Domain.Models.Providers.ProvidersArea();


            try
            {
                providerArea.Active = provider.Active;
                providerArea.Critical = provider.Critical;
                providerArea.Description = provider.Description;
                providerArea.EndDate = provider.EndDate;
                providerArea.StartDate = provider.StartDate;
                providerArea.Id = provider.Id;

                unitOfWork.ProviderAreaRepository.Add(providerArea);

                unitOfWork.Save();

                response.Data = new ProvidersAreaModel(providerArea);
               
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Put(int id, ProvidersAreaModel provider)
        {
            var providerArea = new Domain.Models.Providers.ProvidersArea();
            var response = new Response<ProvidersAreaModel>();

            providerArea.Active = provider.Active;
            providerArea.Critical = provider.Critical;
            providerArea.Description = provider.Description;
            providerArea.EndDate = provider.EndDate;
            providerArea.StartDate = provider.StartDate;
            providerArea.Id = provider.Id;
            providersAreaRepository.Update(providerArea);

            response.Data = provider;

            return response;
        }

        public Response Disable(int id)
        {
            var providerArea = unitOfWork.ProviderAreaRepository.GetById(id);
            var response = new Response<int>();

            providerArea.Active = false;
            providerArea.EndDate = DateTime.Now;

            unitOfWork.Save();
            response.Data = id;
            return response;
        }

        public Response Enable(int id)
        {
            var providerArea = unitOfWork.ProviderAreaRepository.GetById(id);
            var response = new Response<int>();

            providerArea.Active = true;
            providerArea.EndDate = null;
            providerArea.StartDate = DateTime.Now;

            unitOfWork.Save();
            response.Data = id;
            return response;
        }
    }

}
