namespace Sofco.Core.Validations.Workflow
{
    public interface IOnTransitionSuccessFactory
    {
        IOnTransitionSuccessState GetInstance(string code);
    }
}
