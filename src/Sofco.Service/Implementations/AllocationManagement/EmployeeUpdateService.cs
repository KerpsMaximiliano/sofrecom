using System;
using Sofco.Core.Logger;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeUpdateService : IEmployeeUpdateService
    {
        private readonly ILogMailer<EmployeeUpdateService> logger;

        private readonly IEmployeeSyncProfileJobService syncProfileJobService;

        private readonly IEmployeeSyncActionJobService syncActionJobService;

        public EmployeeUpdateService(ILogMailer<EmployeeUpdateService> logger, IEmployeeSyncProfileJobService syncProfileJobService, IEmployeeSyncActionJobService syncActionJobService)
        {
            this.logger = logger;
            this.syncProfileJobService = syncProfileJobService;
            this.syncActionJobService = syncActionJobService;
        }

        public Response<string> UpdateEmployees()
        {
            var response = new Response<string>();

            try
            {
                syncProfileJobService.Sync();

                syncActionJobService.SyncEndEmployees();

                syncActionJobService.SyncNewEmployees();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
