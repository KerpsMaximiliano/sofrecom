using System;
using Sofco.Core.DAL;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public class HitoValidatorHelper
    {
        public static void ValidateAmmounts(HitoParameters hito, Response response)
        {
            if (!hito.Ammount.HasValue || hito.Ammount.GetValueOrDefault() <= 0 || hito.Ammount.GetValueOrDefault() > 99999999)
            {
                response.AddError(Resources.Billing.Project.HitoAmmoutRequired);
            }
        }

        public static void ValidateOpportunity(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.OpportunityId) || hito.OpportunityId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.AddError(Resources.Billing.Project.OpportunityRequired);
            }
        }

        public static void ValidateName(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.Name))
            {
                response.AddError(Resources.Billing.Project.NameRequired);
            }
        }

        public static void ValidateMonth(HitoParameters hito, Response response)
        {
            if (!hito.Month.HasValue || hito.Month <= 0 || hito.Month > 36)
            {
                response.AddError(Resources.Billing.Project.MonthRequired);
            }
        }

        public static void ValidateDate(HitoParameters hito, Response response, IUnitOfWork unitOfWork)
        {
            if (!hito.StartDate.HasValue)
            {
                response.AddError(Resources.Billing.Project.DateRequired);
            }
            else if(!string.IsNullOrWhiteSpace(hito.ProjectId) && !hito.ProjectId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                var analytic = unitOfWork.AnalyticRepository.GetByProyectId(hito.ProjectId);

                if (analytic != null)
                {
                    if (hito.StartDate.GetValueOrDefault().Date < analytic.StartDateContract.Date ||
                        hito.StartDate.GetValueOrDefault().Date > analytic.EndDateContract.Date)
                    {
                        response.AddError(Resources.Billing.Project.HitoDatesOutRange);
                    }
                }
            }
        }

        public static void ValidateCurrency(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.MoneyId))
            {
                response.AddError(Resources.Common.CurrencyRequired);
            }
        }

        public static void ValidateProject(HitoParameters hito, Response response)
        {
            if (string.IsNullOrWhiteSpace(hito.ProjectId) || hito.ProjectId.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.AddError(Resources.Billing.Project.Required);
            }
        }
    }
}
