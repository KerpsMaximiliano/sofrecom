using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AnalyticValidationHelper
    {
        public static Analytic Find(Response response, IUnitOfWork unitOfWork, int id)
        {
            var analytic = unitOfWork.AnalyticRepository.GetSingle(x => x.Id == id);

            if (analytic == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.NotFound, MessageType.Error));
            }
            else
            {
                var projects = unitOfWork.ProjectRepository.GetAllActives(analytic.ServiceId);

                var opportunities = projects.Select(x => $"{x.OpportunityNumber} {x.OpportunityName}");

                analytic.Proposal = string.Join(";", opportunities);
                analytic.Refunds = unitOfWork.RefundRepository.GetRefundsByAnalytics(id);
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

        public static void CheckName(Response response, Analytic analytic)
        {
            if (string.IsNullOrWhiteSpace(analytic.Name))
            {
                response.AddError(Resources.AllocationManagement.Analytic.NameIsRequired);
            }
        }

        public static void CheckDirector(Response response, Analytic analytic)
        {
            if (analytic.SectorId <= 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.DirectorIsRequired);
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

        public static void CheckService(Response<Analytic> response, Analytic analytic, IAnalyticRepository analyticRepository)
        {
            if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
            {
                var domain = analyticRepository.GetByService(analytic.ServiceId);

                if (domain != null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.ServiceAlreadyRelated);
                }
            }
        }
    }
}
