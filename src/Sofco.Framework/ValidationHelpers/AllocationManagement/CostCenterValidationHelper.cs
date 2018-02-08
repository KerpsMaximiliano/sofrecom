using System.Text.RegularExpressions;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public class CostCenterValidationHelper
    {
        public static void ValidateCode(Response response, CostCenter domain, ICostCenterRepository costCenterRepository)
        {
            if (domain.Code == 0)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.CodeIsRequired, MessageType.Error));
            }
            else
            {
                if (domain.Code.ToString().Length != 3)
                {
                    response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.CodeWrongLength, MessageType.Error));
                }
                else
                {
                    if (costCenterRepository.ExistCode(domain.Code))
                    {
                        response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.CodeAlreadyExist, MessageType.Error));
                    }
                }
            }
        }

        public static void ValidateLetter(Response response, CostCenter domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Letter))
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.LetterIsRequired, MessageType.Error));
            }
            else
            {
                Match match = Regex.Match(domain.Letter, @"(?i)^[a-zA-Z0-9]+");

                if (domain.Letter.Length != 1 || !match.Success)
                {
                    response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.LetterWrong, MessageType.Error));
                }
            }
        }

        public static void ValidateDescription(Response response, CostCenter domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Description))
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.DescriptionRequired, MessageType.Error));
            }
        }

        public static CostCenter Exist(Response response, int id, IUnitOfWork unitOfWork)
        {
            var costCenter = unitOfWork.CostCenterRepository.GetSingle(x => x.Id == id);

            if (costCenter == null)
            {
                response.AddError(Resources.AllocationManagement.CostCenter.NotFound);
                return null;
            }

            return costCenter;
        }
    }
}

