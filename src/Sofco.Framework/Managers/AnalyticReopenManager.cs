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
    public class AnalyticReopenManager : IAnalyticReopenManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmServiceService crmServiceService;

        private readonly ILogMailer<AnalyticCloseManager> logger;

        private readonly EmailConfig emailConfig;

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;
        private readonly IRoleManager roleManager;
        private readonly IUserData userData;

        public AnalyticReopenManager(IUnitOfWork unitOfWork, ICrmServiceService crmServiceService, IUserData userData,
            ILogMailer<AnalyticCloseManager> logger, IOptions<EmailConfig> emailOptions, IMailSender mailSender, IMailBuilder mailBuilder,IRoleManager roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.crmServiceService = crmServiceService;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            emailConfig = emailOptions.Value;
            this.userData = userData;
        }

        public Response Reopen(int analyticId, AnalyticStatus status)
        {
            var response = new Response();

            if(roleManager.IsCdg())
            {
                var analytic = unitOfWork.AnalyticRepository.GetById(analyticId);

                analytic.ClosedAt = null;
                analytic.Status = AnalyticStatus.Open;
                unitOfWork.Save();
            }

            return response;
        }
    }
}
