using System;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Framework.StatusHandlers.Analytic;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.Managers
{
    public class AnalyticCloseManager : IAnalyticCloseManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmServiceService crmServiceService;

        private readonly ILogMailer<AnalyticCloseManager> logger;

        private readonly EmailConfig emailConfig;

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;

        private readonly IUserData userData;

        public AnalyticCloseManager(IUnitOfWork unitOfWork, ICrmServiceService crmServiceService, IUserData userData,
            ILogMailer<AnalyticCloseManager> logger, IOptions<EmailConfig> emailOptions, IMailSender mailSender, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.crmServiceService = crmServiceService;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            emailConfig = emailOptions.Value;
            this.userData = userData;
        }

        public Response Close(int analyticId, AnalyticStatus status)
        {
            var response = new Response();
            var analytic = AnalyticValidationHelper.Find(response, unitOfWork, analyticId);

            if (response.HasErrors()) return response;

            if (!string.IsNullOrWhiteSpace(analytic.ServiceId) && status == AnalyticStatus.Close)
            {
                var result = crmServiceService.DeactivateService(new Guid(analytic.ServiceId));

                if (result.HasErrors)
                {
                    response.AddError(Resources.Common.CrmGeneralError);
                    return response;
                }
            }

            try
            {
                if (status == AnalyticStatus.Close && !string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var service = unitOfWork.ServiceRepository.GetByIdCrm(analytic.ServiceId);

                    if (service != null)
                    {
                        service.Active = false;
                        unitOfWork.ServiceRepository.UpdateActive(service);
                    }
                }

                AnalyticStatusClose.Save(analytic, unitOfWork, response, status, userData.GetCurrentUser().UserName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);

                if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var result = crmServiceService.ActivateService(new Guid(analytic.ServiceId));
                    if (result.HasErrors)
                    {
                        response.AddError(Resources.Common.CrmGeneralError);
                    }
                }

                return response;
            }

            try
            {
                AnalyticStatusClose.SendMail(response, analytic, emailConfig, mailSender, unitOfWork, mailBuilder);
            }
            catch (Exception ex)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
                logger.LogError(ex);
            }

            return response;
        }
    }
}
