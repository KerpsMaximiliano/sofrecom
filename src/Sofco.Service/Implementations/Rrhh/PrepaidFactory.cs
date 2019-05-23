using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Services.Rrhh;
using Sofco.Framework.FileManager.Rrhh.Prepaids;

namespace Sofco.Service.Implementations.Rrhh
{
    public class PrepaidFactory : IPrepaidFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public PrepaidFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IPrepaidFileManager GetInstance(string code)
        {
            switch (code)
            {
                case "SWISS": return new SwissPrepaidFileManager(unitOfWork);
                default: return null;
            }
        }
    }
}
