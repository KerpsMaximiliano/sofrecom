﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.Common
{
    public class UtilsRepository : IUtilsRepository
    {
        protected readonly SofcoContext Context;

        public UtilsRepository(SofcoContext context)
        {
            Context = context;
        }

        public IList<Currency> GetCurrencies()
        {
            return Context.Currencies.ToList().AsReadOnly();
        }

        public IList<PaymentTerm> GetPaymentTerms()
        {
            return Context.PaymentTerms.ToList().AsReadOnly();
        }

        public IList<Solution> GetSolutions()
        {
            return Context.Solutions.ToList().AsReadOnly();
        }

        public IList<Technology> GetTechnologies()
        {
            return Context.Technologies.ToList().AsReadOnly();
        }

        public IList<Product> GetProducts()
        {
            return Context.Products.ToList().AsReadOnly();
        }

        public IList<ClientGroup> GetClientGroups()
        {
            return Context.ClientGroups.ToList().AsReadOnly();
        }

        public IList<PurchaseOrderOptions> GetPurchaseOrderOptions()
        {
            return Context.PurchaseOrderOptions.ToList().AsReadOnly();
        }

        public IList<ServiceType> GetServiceTypes()
        {
            return Context.ServiceTypes.ToList().AsReadOnly();
        }

        public IList<Sector> GetSectors()
        {
            return Context.Sectors
                .Include(x => x.ResponsableUser)
                .Where(x => x.Active)
                .ToList()
                .AsReadOnly();
        }

        public IList<EmployeeEndReason> GetEmployeeTypeEndReasons()
        {
            return Context.EmployeeEndReason.ToList().AsReadOnly();
        }

        public IList<Area> GetAreas()
        {
            return Context.Areas
                .Include(x => x.ResponsableUser)
                .Where(x => x.Active)
                .ToList()
                .AsReadOnly();
        }

        public IList<SoftwareLaw> GetSoftwareLaws()
        {
            return Context.SoftwareLaws.ToList().AsReadOnly();
        }

        public IList<DocumentType> GetDocumentTypes()
        {
            return Context.DocumentTypes.ToList().AsReadOnly();
        }

        public IList<ImputationNumber> GetImputationNumbers()
        {
            return Context.ImputationNumbers.ToList().AsReadOnly();
        }

        public IList<Province> GetProvinces()
        {
            return Context.Provinces.Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<MonthsReturn> GetMonthsReturn()
        {
            return Context.MonthsReturns.ToList().AsReadOnly();
        }

        public bool ExistMonthReturn(int id)
        {
            return Context.MonthsReturns.Any(x => x.Id == id);
        }

        public bool ExistCurrency(int id)
        {
            return Context.Currencies.Any(x => x.Id == id);
        }

        public bool ExistCreditCard(int creditCardId)
        {
            return Context.CreditCards.Any(x => x.Id == creditCardId);
        }

        public IList<CreditCard> GetCreditCards()
        {
            return Context.CreditCards.Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<EmployeeProfile> GetEmployeeProfiles()
        {
            return Context.EmployeeProfile
                    .OrderBy(p => p.Text)
                    .ToList();
        }

        public IList<Prepaid> GetPrepaids()
        {
            return Context.Prepaids.ToList().AsReadOnly();
        }

        public Prepaid GetPrepaid(int prepaidId)
        {
            return Context.Prepaids.SingleOrDefault(x => x.Id == prepaidId);
        }

        public bool ExistProfile(int id)
        {
            return Context.Profiles.Any(x => x.Id == id);
        }

        public bool ExistSeniority(int id)
        {
            return Context.Seniorities.Any(x => x.Id == id);
        }
    }
}
