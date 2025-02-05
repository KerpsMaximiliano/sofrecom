﻿using System;
using Sofco.Core.Services.Common;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.Billing;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Domain.Models.Reports;
using Sofco.Core.DAL.Views;
using Sofco.Core.Models.Admin;

namespace Sofco.Service.Implementations.Common
{
    public class UtilsService : IUtilsService
    {
        private readonly IUserData userData;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly IBanksViewRepository banksViewRepository;

        public UtilsService(IUnitOfWork unitOfWork, IUserData userData, IMapper mapper, IBanksViewRepository banksViewRepository)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.mapper = mapper;
            this.banksViewRepository = banksViewRepository;
        }

        public IList<Currency> GetCurrencies() 
        {
            return unitOfWork.UtilsRepository.GetCurrencies();
        }

        public IList<PaymentTerm> GetPaymentTerms()
        {
            return unitOfWork.UtilsRepository.GetPaymentTerms();
        }

        public IList<SectorModel> GetSectors()
        {
            var data = unitOfWork.UtilsRepository.GetSectors().ToList();

            return Translate(data).OrderBy(x => x.Text).ToList();
        }

        public Response<List<SectorModel>> GetSectorsByCurrentUser()
        {
            var currentuser = userData.GetCurrentUser();

            var data = unitOfWork.UtilsRepository.GetSectors()
                .Where(s => s.ResponsableUserId == currentuser.Id)
                .ToList();
            return new Response<List<SectorModel>>
            {
                Data = Translate(data).OrderBy(x => x.Text).ToList()
            };
        }

        public IList<EmployeeEndReason> GetEmployeeTypeEndReasons()
        {
            return unitOfWork.UtilsRepository.GetEmployeeTypeEndReasons();
        }

        public IEnumerable<Option> GetMonths()
        {
            yield return new Option { Id = 1, Text = Resources.Months.January };
            yield return new Option { Id = 2, Text = Resources.Months.February };
            yield return new Option { Id = 3, Text = Resources.Months.March };
            yield return new Option { Id = 4, Text = Resources.Months.April };
            yield return new Option { Id = 5, Text = Resources.Months.May };
            yield return new Option { Id = 6, Text = Resources.Months.June };
            yield return new Option { Id = 7, Text = Resources.Months.July };
            yield return new Option { Id = 8, Text = Resources.Months.August };
            yield return new Option { Id = 9, Text = Resources.Months.September };
            yield return new Option { Id = 10, Text = Resources.Months.October };
            yield return new Option { Id = 11, Text = Resources.Months.November };
            yield return new Option { Id = 12, Text = Resources.Months.December };
        }
        
        public IEnumerable<MonthsReturn> GetCloseMonths()
        {
            var closeDates = unitOfWork.CloseDateRepository.GetAllBeforeNextMonth();

            for (int i = 1; i < closeDates.Count; i++)
            {
                var closeDateBefore = closeDates[i - 1];
                var closeDate = closeDates[i];

                yield return new MonthsReturn
                {
                    Id = closeDateBefore.Id,
                    Text = $"{ closeDate.Day + 1 } { DatesHelper.GetMonthDesc(closeDate.Month) } {closeDate.Year} al " +
                                                         $"{ closeDateBefore.Day } { DatesHelper.GetMonthDesc(closeDateBefore.Month) } {closeDateBefore.Year}",
                    Month = closeDate.Month
                };
            }
        }

        public IList<MonthsReturn> GetMonthsReturn()
        {
            return unitOfWork.UtilsRepository.GetMonthsReturn();
        }

        public IList<Option> GetCreditCards()
        {
            return unitOfWork.UtilsRepository.GetCreditCards().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
        }

        public IEnumerable<Option> GetYears()
        {
            var today = DateTime.UtcNow;

            for (int i = today.AddYears(-2).Year; i < today.AddYears(5).Year; i++)
            {
                yield return new Option { Id = i, Text = i.ToString() };
            }
        }

        public IList<AreaModel> GetAreas()
        {
            var data = unitOfWork.UtilsRepository.GetAreas().ToList();

            return Translate(data).OrderBy(x => x.Text).ToList();
        }

        public Response<List<AreaModel>> GetAreasByCurrentUser()
        {
            var currentuser = userData.GetCurrentUser();

            var areas = unitOfWork.UtilsRepository.GetAreas()
                .Where(s => s.ResponsableUserId == currentuser.Id)
                .ToList();

            return new Response<List<AreaModel>>
            {
                Data = Translate(areas).OrderBy(x => x.Text).ToList()
            };
        }

        public List<object> GetUserDelegateType()
        {
            var enumVals = new List<object>();

            foreach (var item in Enum.GetValues(typeof(UserDelegateType)))
            {
                enumVals.Add(new { Id = (int)item, Text = item.ToString() });
            }

            return enumVals;
        }

        public IList<DocumentType> GetDocumentTypes()
        {
            return unitOfWork.UtilsRepository.GetDocumentTypes();
        }

        public IList<ImputationNumber> GetImputationNumbers()
        {
            return unitOfWork.UtilsRepository.GetImputationNumbers();
        }

        public IList<Province> GetProvinces()
        {
            return unitOfWork.UtilsRepository.GetProvinces();
        }

        public IList<BankModel> GetBanks()
        {
            var data = banksViewRepository.GetBanks().ToList();

            return Translate(data).OrderBy(x => x.Name).ToList();
        }

        public IList<EmployeeProfileModel> GetEmployeeProfiles()
        {
            var data = unitOfWork.UtilsRepository.GetEmployeeProfiles().ToList();

            return Translate(data).ToList();
        }

        private List<SectorModel> Translate(List<Sector> data)
        {
            return mapper.Map<List<Sector>, List<SectorModel>>(data);
        }

        private List<AreaModel> Translate(List<Area> data)
        {
            return mapper.Map<List<Area>, List<AreaModel>>(data);
        }

        public List<BankModel> Translate(List<BankView> data)
        {
            List<BankModel> banks = new List<BankModel>();
            foreach (var item in data)
            {
                BankModel bank = new BankModel();
                bank.Id = item.Id;
                bank.Name = item.Name;

                banks.Add(bank);
            }

            return banks;
        }

        private List<EmployeeProfileModel> Translate(List<EmployeeProfile> data)
        {
            return mapper.Map<List<EmployeeProfile>, List<EmployeeProfileModel>>(data);
        }

        public IList<Prepaid> GetPrepaids()
        {
            return unitOfWork.UtilsRepository.GetPrepaids();
        }
    }
}
