using System.Collections.Generic;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Managers
{
    public interface IRoleManager
    {
        List<Role> GetRoles();

        bool HasFullAccess();

        bool IsDirector();

        bool IsCdg();

        bool IsManager();

        bool HasAccessForRefund();

        bool HasAdvancementAccess();

        bool IsDafOrGaf();

        bool IsPmo();

        bool IsRrhh();

        bool IsCompliance();

        bool CanViewSensibleData();
    }
}