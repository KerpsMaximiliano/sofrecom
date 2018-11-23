namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowConditionStateFactory
    {
        IWorkflowConditionState GetInstance(string code);
    }
}
