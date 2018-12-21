using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class RefundRepository : BaseRepository<Refund>, IRefundRepository
    {
        public RefundRepository(SofcoContext context) : base(context)
        {
        }

        public List<Refund> GetByParameters(RefundListParameterModel parameter)
        {
            throw new System.NotImplementedException();
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

        public RefundFile GetFile(int id, int fileId)
        {
            return context.RefundFiles.Include(x => x.File).SingleOrDefault(x => x.RefundId == id && x.FileId == fileId);
        }

        public void DeleteFile(RefundFile file)
        {
            context.RefundFiles.Remove(file);
        }

        public IList<RefundHistory> GetHistories(int id)
        {
            return context.RefundHistories
                .Include(x => x.StatusFrom)
                .Include(x => x.StatusTo)
                .Where(x => x.RefundId == id).ToList();
        }

        public bool Exist(int id)
        {
            return context.Refunds.Any(x => x.Id == id);
        }

        public Refund GetById(int id)
        {
            return context.Refunds.Include(x => x.Details).Include(x => x.AdvancementRefunds).SingleOrDefault(x => x.Id == id);
        }
    }
}
