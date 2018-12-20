using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class RefundRepository : BaseRepository<Refund>, IRefundRepository
    {
        public RefundRepository(SofcoContext context) : base(context)
        {
        }

        public void InsertFile(RefundFile refundFile)
        {
            context.RefundFiles.Add(refundFile);
        }

        public Refund GetFullById(int id)
        {
            return context.Refunds
                .Include(x => x.Currency)
                .Include(x => x.Analytic)
                .Include(x => x.UserApplicant)
                .Include(x => x.Status)
                .Include(x => x.Details)
                .Include(x => x.AdvancementRefunds)
                    .ThenInclude(x => x.Advancement)
                .Include(x => x.Attachments)
                    .ThenInclude(x => x.File)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
