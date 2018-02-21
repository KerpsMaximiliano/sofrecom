using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class FileRepository : BaseRepository<File>, IFileRepository
    {
        public FileRepository(SofcoContext context) : base(context)
        {
        }
    }
}
