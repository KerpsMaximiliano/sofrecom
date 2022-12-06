using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.BuyOrder
{
    public class BuyOrderService : IBuyOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<BuyOrderService> logger;
        private readonly IUserData userData;
        private readonly FileConfig fileConfig;

        public BuyOrderService(IUnitOfWork unitOfWork, ILogMailer<BuyOrderService> logger, IUserData userData,
            IOptions<FileConfig> fileOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            fileConfig = fileOptions.Value;
        }

        public IList<BuyOrderGridModel> GetAll(BuyOrderGridFilters filters)
        {
            var user = userData.GetCurrentUser();
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "NOPE");
            return this.unitOfWork.BuyOrderRepository.GetAll(filters)
                    .Select(n => new BuyOrderGridModel(n, permisos, user.Id))
                    .Where(n => n.HasEditPermissions || n.HasReadPermissions).ToList();
        }
    }
}
