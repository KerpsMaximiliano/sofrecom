using System.Collections.Generic;
using Sofco.Model.Utils;

namespace Sofco.Core.Data.Billing
{
    public interface ISectorData
    {
        IList<Sector> GetAll();

        void ClearKeys();
    }
}