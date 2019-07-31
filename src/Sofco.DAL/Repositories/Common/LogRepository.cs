using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        public LogRepository(SofcoContext context) : base(context)
        {
        }
    }
}
