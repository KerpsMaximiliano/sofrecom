using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailStaffRepository : BaseRepository<CostDetailStaff>, ICostDetailStaffRepository
    {
        public CostDetailStaffRepository(SofcoContext context) : base(context)
        {
        }
    }
}
