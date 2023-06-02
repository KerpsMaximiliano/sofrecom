using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Helpers;
using Sofco.Service.Implementations.Entities;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeWorkTimesAddJobService : EmployeeWorkTimesAddJobServiceBase, IEmployeeWorkTimesAddJobService
    {
        

        #region private properties
      
        #endregion

        public EmployeeWorkTimesAddJobService(IUnitOfWork unitOfWork, ILogMailer<EmployeeWorkTimesAddJobService> logger) : base (unitOfWork, logger) { }
        

        protected override bool MustExecute()
        {
            return true;
        }


      
    }
}
