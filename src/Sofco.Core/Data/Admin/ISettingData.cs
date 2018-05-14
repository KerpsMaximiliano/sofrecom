using Sofco.Model.Models.Admin;

namespace Sofco.Core.Data.Admin
{
    public interface ISettingData
    {
        Setting GetByKey(string key);

        void ClearKeys();
    }
}