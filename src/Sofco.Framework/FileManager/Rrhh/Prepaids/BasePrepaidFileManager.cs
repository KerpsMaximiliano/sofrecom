using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Framework.FileManager.Rrhh.Prepaids
{
    public class BasePrepaidFileManager
    {
        protected string FileName;

        public void CheckDifferencies(PrepaidImportedData itemToAdd)
        {
            if (itemToAdd.TigerBeneficiaries != itemToAdd.PrepaidBeneficiaries)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInBeneficiaires);
            }

            if (itemToAdd.TigerCost != itemToAdd.PrepaidCost)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInCosts);
            }

            if (itemToAdd.Date.Date != itemToAdd.Period.Date)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInPeriod);
            }
        }

        public Response<PrepaidDashboard> CheckFileName(IFormFile file)
        {
            var response = new Response<PrepaidDashboard>();

            if (file.FileName.Trim().ToLowerInvariant() != FileName)
            {
                response.AddError(Resources.Rrhh.Prepaid.FileWrong);
            }

            return response;
        }

        protected void CreateResponse(Response<PrepaidDashboard> response, IList<PrepaidImportedData> list)
        {
            list = list.ToList();

            response.Data = new PrepaidDashboard
            {
                CountSuccess = list.Count(x => x.Status == PrepaidImportedDataStatus.Success),
                CountError = list.Count(x => x.Status == PrepaidImportedDataStatus.Error),
            };
        }
    }
}
