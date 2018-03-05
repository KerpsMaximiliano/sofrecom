using System.Collections.Generic;

namespace Sofco.Core.Data.Billing
{
    public interface ISolfacDelegateData
    {
        List<string> GetUserDelegateByUserName(string userName);
    }
}
