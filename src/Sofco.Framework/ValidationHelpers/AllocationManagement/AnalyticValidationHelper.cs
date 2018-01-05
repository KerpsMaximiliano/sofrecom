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

        public static void CheckTitle(Response response, Analytic analytic, ICostCenterRepository costCenterRepository)
        {
            if (string.IsNullOrWhiteSpace(analytic.Title))
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleIsRequired);
                return;
            }

            if (!analytic.Title.Contains("-"))
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleInvalid);
                return;
            }

            if(analytic.CostCenterId == 0) return;

            var costCenter = costCenterRepository.GetSingle(x => x.Id == analytic.CostCenterId);

            var titleId = 0;
            var titleSplitted = analytic.Title.Split('-');

            var letter = titleSplitted[1].Substring(0, 1);
            var code = titleSplitted[0];

            if (!costCenter.Code.ToString().Equals(code) || !costCenter.Letter.Equals(letter))
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleInvalid);
                return;
            }

            var title = titleSplitted[1].Substring(1, titleSplitted[1].Length - 1);

            var result = int.TryParse(title, out titleId);

            if (result)
            {
                analytic.TitleId = titleId;
            }
            else
            {
                response.AddError(Resources.AllocationManagement.Analytic.TitleInvalid);
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

        public static void CheckDates(Response response, Analytic analytic)
        {
            if (analytic.EndDateContract < analytic.StartDateContract)
            {
                response.AddError(Resources.AllocationManagement.Analytic.WrongDates);
            }
        }
    }
}
