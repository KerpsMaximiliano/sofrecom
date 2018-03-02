using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacDelegateRepository
    {
        List<SolfacDelegate> GetByServiceIds(List<string> serviceIds);

        List<SolfacDelegate> GetByUserId(int userId);

        SolfacDelegate Save(SolfacDelegate solfacDelegate);

        void Delete(int solfacDelegateId);

        bool HasSolfacDelegate(string userName);
    }
}