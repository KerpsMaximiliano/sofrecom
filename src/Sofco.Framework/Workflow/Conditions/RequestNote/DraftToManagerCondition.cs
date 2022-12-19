using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;
using System.Linq;

namespace Sofco.Framework.Workflow.Conditions.RequestNote
{
    public class DraftToManagerCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;

        public DraftToManagerCondition(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            //Validar que exista al menos un registro en la grilla de producto/ servicio y que la
            //suma total de las cantidades de todos los registros sea mayor a 0.
            //Validar que exista al menos un registro en la grilla de analíticas y que la
            //suma total de los % de asignación sea 100.
            //Las analíticas seleccionadas deberán asociarse con el estado “Pendiente de Aprobación”. 
            
            var req = unitOfWork.RequestNoteRepository.Get(entity.Id);

            if (req == null) return false;
            var hasProductServices = req.ProductsServices.Any() && req.ProductsServices.All(p => p.Quantity > 0);
            var hasAnalytics = req.Analytics.Any() && req.Analytics.Sum(a => a.Percentage) == 100;
            return hasProductServices && hasAnalytics;
        }
    }
}
