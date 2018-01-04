using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AnalyticValidationHelper
    {
        public static Analytic Find(Response response, IAnalyticRepository analyticRepository, int id)
        {
            var analytic = analyticRepository.GetSingle(x => x.Id == id);

            if (analytic == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.NotFound, MessageType.Error));
            }

            return analytic;
        }

        public static void Exist(Response response, IAnalyticRepository analyticRepository, int id)
        {
            var exist = analyticRepository.Exist(id);

            if (!exist)
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
            }
        }

        public static void CheckIfTitleIsNumber(Response response, Analytic analytic)
        {
            var titleId = 0;
            var titleSplitted = analytic.Title.Split('-')[1];
            var title = titleSplitted.Substring(1, titleSplitted.Length - 1);

            var result = int.TryParse(title, out titleId);

            if (result)
            {
                analytic.TitleId = titleId;
            }
            else
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleIsNotANumber);
            }
        }

        public static void CheckNameAndDescription(Response response, Analytic analytic)
        {
            if (string.IsNullOrWhiteSpace(analytic.Name))
            {
                response.AddError(Resources.AllocationManagement.Analytic.NameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(analytic.Description))
            {
                response.AddError(Resources.AllocationManagement.Analytic.DescriptionIsRequired);
            }
        }

        public static void CheckDirector(Response response, Analytic analytic)
        {
            if (analytic.DirectorId <= 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.DirectorIsRequired);
            }
        }

        public static void CheckCurrency(Response response, Analytic analytic)
        {
            if (!analytic.CurrencyId.HasValue || analytic.CurrencyId <= 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.CurrencyIsRequired);
            }
        }

        public static void CheckIfTitleExist(Response<Analytic> response, Analytic analytic, IAnalyticRepository analyticRepository)
        {
            if (analyticRepository.ExistTitle(analytic.Title))
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleAlreadyExist);
            }
        }
    }
}
