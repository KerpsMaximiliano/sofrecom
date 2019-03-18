using Sofco.Domain.Models.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.Views
{
   public interface IBanksViewRepository
    {
        List<BankView> GetBanks();
    }
}
