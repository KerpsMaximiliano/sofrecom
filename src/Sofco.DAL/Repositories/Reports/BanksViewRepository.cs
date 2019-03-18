using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Views;
using Sofco.Domain.Models.Reports;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.Reports
{
    public class BanksViewRepository : IBanksViewRepository
    {
        private readonly DbSet<BankView> banksViewRepository;

        public BanksViewRepository(ReportContext context)
        {
            banksViewRepository = context.Set<BankView>();
        }

        public List<BankView> GetBanks()
        {
            IQueryable<BankView> query = banksViewRepository;

            var result = query.ToList();

            return result;
        }

    }
}
