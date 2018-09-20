using System;
using System.IO;
using System.Linq;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Rrhh
{
    public class RrhhService : IRrhhService
    {
        private readonly IWorktimeData worktimeData;
        private readonly ILogMailer<RrhhService> logger;

        public RrhhService(IWorktimeData worktimeData, ILogMailer<RrhhService> logger)
        {
            this.worktimeData = worktimeData;
            this.logger = logger;
        }

        public Response<byte[]> GenerateTigerTxt()
        {
            var response = new Response<byte[]>();

            var items = worktimeData.GetAll();

            if (!items.Any())
            {
                response.AddError(Resources.Rrhh.TigerReport.NotFound);
                return response;
            }

            var memoryStream = new MemoryStream();
            var text = new StreamWriter(memoryStream);

            try
            {
                foreach (var tigerReportItem in items)
                {
                    text.WriteLine(tigerReportItem.GetLine());
                    text.Flush();
                }

                response.Data = memoryStream.GetBuffer();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
