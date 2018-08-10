using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Data.Billing
{
    public interface ISectorData
    {
        IList<Sector> GetAll();

        void ClearKeys();

        List<int> GetIdByCurrent();
    }
}