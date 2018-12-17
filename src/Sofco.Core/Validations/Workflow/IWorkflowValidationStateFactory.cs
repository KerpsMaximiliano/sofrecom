namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowValidationStateFactory
    {
        IWorkflowValidationState GetInstance(string code);
    }
}
