using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Models.TimeManagement;
using Sofco.Model.Utils;
using Sofco.Model.Enums;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AnalyticValidationHelper
    {
        public static Analytic Find(Response response, IAnalyticRepository analyticRepository, int id)
        {
            var analytic = analyticRepository.GetSingle(x => x.Id == id);

            if (analytic == null)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Analytic.NotFound, MessageType.Error));
            }

            return analytic;
        }

        public static void Exist(Response response, IAnalyticRepository analyticRepository, int id)
        {
            var exist = analyticRepository.Exist(id);

            if (!exist)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Analytic.NotFound, MessageType.Error));
            }
        }
    }
}
