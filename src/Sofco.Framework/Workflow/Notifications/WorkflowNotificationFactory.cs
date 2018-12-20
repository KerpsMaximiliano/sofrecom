using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Mail;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowNotificationFactory : IWorkflowNotificationFactory
    {
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWorkflowRepository workflowRepository;
        private readonly AppSetting appSetting;

        public WorkflowNotificationFactory(IMailSender mailSender,
            IOptions<EmailConfig> emailConfig,
            IOptions<AppSetting> appSetting,
            IUnitOfWork unitOfWork,
            IWorkflowRepository workflowRepository)
        {
            this.mailSender = mailSender;
            this.emailConfig = emailConfig.Value;
            this.unitOfWork = unitOfWork;
            this.workflowRepository = workflowRepository;
            this.appSetting = appSetting.Value;
        }

        public WorkflowNotification GetInstance(string code)
        {
            switch (code)
            {
                case "ADVANCEMENT-DEFAULT": return new WorkflowAdvancementNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork, workflowRepository);
                case "REFUND-DEFAULT": return new WorkflowRefundNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork, workflowRepository);
                default: return null;
            }
        }
    }
}
