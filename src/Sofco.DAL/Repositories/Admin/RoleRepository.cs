﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return context.Roles.Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Role> GetAllActivesReadOnly()
        {
            return context.Set<Role>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public Role GetDetail(int id)
        {
            return context.Set<Role>()
                    .Include(x => x.Groups)
                    .Include(x => x.RoleFunctionality)
                        .ThenInclude(x => x.Functionality)
                            .ThenInclude(x => x.Module)
                   .SingleOrDefault(x => x.Id == id);
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return context.Groups
                    .Include(x => x.Role)
                    .Where(x => groupIds.Contains(x.Id) && x.Role != null)
                    .Select(x => x.Role)
                    .ToList();
        }
    }
}
