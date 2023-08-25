using Sofco.Core.DAL;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AnalyticsRenovationsValidationHelper
    {
        public static AnalyticsRenovation Find(Response response, IUnitOfWork unitOfWork, int id)
        {
            var renovation = unitOfWork.AnalyticsRenovationRepository.GetSingle(x => x.Id == id);

            if (renovation == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.AnalyticsRenovation.NotFound, MessageType.Error));
            }
            
            return renovation;
        }

        public static void Exist(Response response, IUnitOfWork unitOfWork, AnalyticsRenovation renovation)
        {
            var exist = unitOfWork.AnalyticsRenovationRepository.Exist(renovation);
            if (!exist)
            {
                response.AddError(Resources.AllocationManagement.AnalyticsRenovation.NotFound);                
            }
        }

        public static void CheckDates(Response response, AnalyticsRenovationModel renovation)
        {
            if (renovation.EndDate < renovation.StartDate)
            {
                response.AddError(Resources.AllocationManagement.AnalyticsRenovation.WrongDates);
            }
        }
    }
}
