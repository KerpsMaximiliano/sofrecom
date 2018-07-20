using System.Collections.Generic;
using Sofco.Model.Utils;

namespace Sofco.Core.Data.Billing
{
    public interface IAreaData
    {
        IList<Area> GetAll();

        void ClearKeys();
    }
}