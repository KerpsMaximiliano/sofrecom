using System;
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

namespace Sofco.Service.Implementations.Common
{
    public class UtilsService : IUtilsService
    {
        private readonly IUserData userData;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public UtilsService(IUnitOfWork unitOfWork, IUserData userData, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.mapper = mapper;
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
        public IEnumerable<Option> GetCloseMonths()
        {
            var closeDates = unitOfWork.CloseDateRepository.GetAllBeforeNextMonth();

            for (int i = 1; i < closeDates.Count; i++)
            {
                var closeDateBefore = closeDates[i - 1];
                var closeDate = closeDates[i];

                yield return new Option { Id = closeDate.Id, Text = $"{ closeDateBefore.Day+1 } { DatesHelper.GetMonthDesc(closeDateBefore.Month) } {closeDateBefore.Year} al " +
                                                         $"{ closeDate.Day } { DatesHelper.GetMonthDesc(closeDate.Month) } {closeDateBefore.Year}" };
            }
        }

        public IEnumerable<Option> GetYears()
        {
            for (int i = 2018; i < 2099; i++)
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

        private List<SectorModel> Translate(List<Sector> data)
        {
            return mapper.Map<List<Sector>, List<SectorModel>>(data);
        }

        private List<AreaModel> Translate(List<Area> data)
        {
            return mapper.Map<List<Area>, List<AreaModel>>(data);
        }
    }
}
