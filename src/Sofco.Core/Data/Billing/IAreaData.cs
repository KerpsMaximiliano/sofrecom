using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Data.Billing
{
    public interface IAreaData
    {
        IList<Area> GetAll();

        void ClearKeys();
    }
}