using System.Collections.Generic;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.DAL.Rrhh
{
    public interface IRrhhRepository
    {
        IList<SocialCharge> GetSocialCharges(int year, int month);
        void Add(List<SocialCharge> listToAdd);
        void Update(List<SocialCharge> listToUpdate);
        bool ExistData(int yearId, int monthId);
    }
}
