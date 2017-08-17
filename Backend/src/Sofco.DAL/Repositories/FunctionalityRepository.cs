﻿using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories
{
    public class FunctionalityRepository : BaseRepository<Functionality>, IFunctionalityRepository
    {
        public FunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<Functionality>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Functionality> GetAllActivesReadOnly()
        {
            return _context.Set<Functionality>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Functionality> GetAllFullReadOnly()
        {
            return _context.Set<Functionality>()
                .Include(x => x.RoleModuleFunctionality)
                    .ThenInclude(s => s.Role)
                .ToList();
        }

        public IList<Functionality> GetFunctionalitiesByModuleAndRole(int moduleId, int roleId)
        {
            return _context.RoleModuleFunctionality
                .Include(x => x.Functionality)
                .Where(x => x.ModuleId == moduleId && x.RoleId == roleId && x.Functionality != null)
                .Select(x => x.Functionality)
                .Distinct()
                .ToList();
        }

        public IList<RoleModuleFunctionality> GetModuleAndFuntionalitiesByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleModuleFunctionality
                    .Include(x => x.Module)
                    .Include(x => x.Functionality)
                    .Where(x => roleIds.Contains(x.RoleId) && x.Functionality != null && x.Module != null)
                    .Distinct()
                    .ToList();
        }

        public Functionality GetSingleWithRoles(Expression<Func<Functionality, bool>> predicate)
        {
            return _context.Set<Functionality>()
                .Include(x => x.RoleModuleFunctionality)
                    .ThenInclude(s => s.Role)
                .SingleOrDefault(predicate);
        }
    }
}
