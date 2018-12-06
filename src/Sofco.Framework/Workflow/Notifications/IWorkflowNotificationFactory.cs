namespace Sofco.Framework.Workflow.Notifications
{
    public interface IWorkflowNotificationFactory
    {
        WorkflowNotification GetInstance(string code);
    }
}
