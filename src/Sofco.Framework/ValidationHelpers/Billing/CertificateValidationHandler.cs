using Sofco.Core.DAL;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public class CertificateValidationHandler
    {
        public static void ValidateName(Response<Certificate> response, Certificate domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Name))
            {
                response.AddError(Resources.Billing.Certificate.NameIsRequired);
            }
        }

        public static void ValidateClient(Response response, Certificate domain)
        {
            if (string.IsNullOrWhiteSpace(domain.ClientExternalId))
            {
                response.AddError(Resources.Billing.Certificate.ClientIsRequired);
            }
        }

        public static void ValidateYear(Response response, Certificate domain)
        {
            if (domain.Year < 2015 || domain.Year > 2099)
            {
                response.AddError(Resources.Billing.Certificate.YearIsRequired);
            }
        }

        public static Certificate Find(int certificateId, Response response, IUnitOfWork unitOfWork)
        {
            var certificate = unitOfWork.CertificateRepository.GetById(certificateId);

            if (certificate == null)
            {
                response.AddError(Resources.Billing.Certificate.NotFound);
            }

            return certificate;
        }

        public static void Exist(Response response, int certificateId, IUnitOfWork unitOfWork)
        {
            var exist = unitOfWork.CertificateRepository.Exist(certificateId);

            if (!exist)
            {
                response.AddError(Resources.Billing.Certificate.NotFound);
            }
        }
    }
}
