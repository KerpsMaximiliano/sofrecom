namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowNotificationFactory
    {
        IWorkflowNotification GetInstance(string code);
    }
}
