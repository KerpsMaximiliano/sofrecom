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
        private readonly AppSetting appSetting;

        public WorkflowNotificationFactory(IMailSender mailSender,
            IOptions<EmailConfig> emailConfig,
            IOptions<AppSetting> appSetting,
            IUnitOfWork unitOfWork)
        {
            this.mailSender = mailSender;
            this.emailConfig = emailConfig.Value;
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting.Value;
        }

        public WorkflowNotification GetInstance(string code)
        {
            switch (code)
            {
                case "ADVANCEMENT-DEFAULT": return new WorkflowAdvancementNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork);
                case "REFUND-DEFAULT": return new WorkflowRefundNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork);
                case "REFUND-REJECT": return new WorkflowRefundNotificationReject(mailSender, emailConfig, appSetting, unitOfWork);
                case "REQUEST-NOTE-DEFAULT": return new WorkflowRequestNoteNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork);
                case "REQUEST-NOTE-REJECT": return new WorkflowRequestNoteNotificationReject(mailSender, emailConfig, appSetting, unitOfWork);
                case "BUY-ORDER-DEFAULT": return new WorkflowBuyOrderNotificationDefault(mailSender, emailConfig, appSetting, unitOfWork);
                case "BUY-ORDER-REJECT": return new WorkflowBuyOrderNotificationReject(mailSender, emailConfig, appSetting, unitOfWork);

                default: return null;
            }
        }
    }
}
