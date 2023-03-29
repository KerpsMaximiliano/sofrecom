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
using Sofco.Core.Models.Providers;
using Sofco.Core.Services.Providers;
using Sofco.Domain.Models.Providers;

namespace Sofco.Service.Implementations.providers
{
    public class ProvidersService: IprovidersService
    {
        private readonly IProvidersRepository providersRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ProvidersService> logger;

        public ProvidersService(IProvidersRepository providers, IUnitOfWork unitOfWork, ILogMailer<ProvidersService> logger)
        {
            providersRepository = providers;
            this.unitOfWork = unitOfWork;
            this.logger = logger;



        }
        public Response<IList<ProvidersModelGet>> GetAll()
        {
            var response = new Response<IList<ProvidersModelGet>>();

            response.Data = providersRepository.GetAll().Select(x => new ProvidersModelGet(x)).ToList();

            return response;
        }

        public Response<ProvidersModelGet> GetById(int providersid)
        {

            var response = new Response<ProvidersModelGet>();
            var provider = providersRepository.GetById(providersid);
            if (provider != null)
            {
                response.Data = new ProvidersModelGet(provider);
            }

            return response;
        }

        public Response Put(int id, ProvidersModel provider)
        {
            //var providers = new Domain.Models.Providers.Providers();
            var providers = unitOfWork.ProvidersRepository.GetById(id);
            var response = new Response<ProvidersModel>();

            providers.Name = provider.Name;
            providers.ProviderAreaId = provider.ProviderAreaId;
            providers.UserApplicantId = provider.UserApplicantId;
            providers.Score = provider.Score;
            providers.StartDate = (DateTime)provider.StartDate;
            providers.EndDate = provider.EndDate;
            providers.Active = provider.Active;
            providers.CUIT = provider.CUIT;
            providers.IngresosBrutos = provider.IngresosBrutos;
            providers.CondicionIVA = provider.CondicionIVA;
            providers.Address = provider.Address;
            providers.City = provider.City;
            providers.ZIPCode = provider.ZIPCode;
            providers.Province = provider.Province;
            providers.Country = provider.Country;
            providers.ContactName = provider.ContactName;   
            providers.Phone = provider.Phone;
            providers.Email = provider.Email;
            providers.WebSite = provider.WebSite;
            providers.Comments = provider.Comments;
            providers.Id = id;

            providers.ProvidersAreaProviders.RemoveAll(p => !provider.ProvidersAreaProviders.Any(pa => pa.Id == p.Id));
            if (provider.ProvidersAreaProviders != null)
            {
                foreach (ProvidersAreaProvidersModel providerAreaProvider in provider.ProvidersAreaProviders)
                {
                    var providerAreaProviderExiste = providers.ProvidersAreaProviders.FirstOrDefault(p => p.Id == providerAreaProvider.Id);
                    if (providerAreaProviderExiste != null)
                        providerAreaProviderExiste.ProviderAreaId = providerAreaProvider.ProviderAreaId;
                    else
                    {
                        providerAreaProviderExiste = new ProvidersAreaProviders();
                        providerAreaProviderExiste.ProviderAreaId = providerAreaProvider.ProviderAreaId;
                        providerAreaProviderExiste.Id = providerAreaProvider.Id;

                        providers.ProvidersAreaProviders.Add(providerAreaProviderExiste);
                    }

                } 
            }
            //unitOfWork.ProvidersRepository.Update(providers);
            unitOfWork.Save();

            response.Data = provider;
            return response;
        }

        public Response<ProvidersModel> Post(ProvidersModel provider)
        {
            var response = new Response<ProvidersModel>();
            var providers = new Domain.Models.Providers.Providers();


            try
            {
                providers.Name = provider.Name;
                providers.ProviderAreaId = provider.ProviderAreaId;
                providers.UserApplicantId = provider.UserApplicantId;
                providers.Score = provider.Score;
                provider.StartDate = DateTime.Now;
                providers.StartDate = provider.StartDate.Value;
                providers.EndDate = provider.EndDate;
                provider.Active = true;
                providers.Active = true;
                providers.CUIT = provider.CUIT;
                providers.IngresosBrutos = provider.IngresosBrutos;
                providers.CondicionIVA = provider.CondicionIVA;
                providers.Address = provider.Address;
                providers.City = provider.City;
                providers.ZIPCode = provider.ZIPCode;
                providers.Province = provider.Province;
                providers.Country = provider.Country;
                providers.ContactName = provider.ContactName;
                providers.Phone = provider.Phone;
                providers.Email = provider.Email;
                providers.WebSite = provider.WebSite;
                providers.Comments = provider.Comments;
                providers.Id = provider.Id;

                providers.ProvidersAreaProviders = new List<ProvidersAreaProviders>();
                if (provider.ProvidersAreaProviders != null)
                {
                    foreach (ProvidersAreaProvidersModel providerAreaProvider in provider.ProvidersAreaProviders)
                    {
                        ProvidersAreaProviders providersAreaProviders = new ProvidersAreaProviders();
                        providersAreaProviders.ProviderAreaId = providerAreaProvider.ProviderAreaId;

                        providers.ProvidersAreaProviders.Add(providersAreaProviders);
                    }
                }
                unitOfWork.ProvidersRepository.Add(providers);

                unitOfWork.Save();

                response.Data = provider;

            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
