using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Repository.Rh.Repositories
{
    public class TigerEmployeeRepository : ITigerEmployeeRepository
    {
        private const string TigerEmployeeSql = @"
SELECT
	legaj, 
	nomb, 
	fenac, 
	feiem, 
	febaj, 
	dtitu, 
	didio, 
	dgrup, 
	calle,
	nro,
	piso,
	depto,
	loca,
	dprov,
	obsoc,
	dobso,
	ospla,
	dospl,
    domif as officeaddress,
    LOWER(demai) as email,
    tidoc as documentNumberType,
    nudoc as documentNumber,
    nusin as cuil,
    CAST(teddi as INTEGER) as phoneCountryCode,
    CAST(teddn as INTEGER) as phoneAreaCode,
    telef as phoneNumber
FROM
	View_A001
";

        private DbSet<TigerEmployee> TigerEmployeeSet { get; }

        public TigerEmployeeRepository(TigerContext context)
        {
            TigerEmployeeSet = context.Set<TigerEmployee>();
        }

        public IList<TigerEmployee> GetWithStartDate(DateTime startDate)
        {
            var sql = string.Format("{0} WHERE {1}", 
                TigerEmployeeSql, 
                @" feiem >= @startDate ");

            var startDateSqlParameter = new SqlParameter("@startDate", startDate);

            return TigerEmployeeSet
                .FromSql(sql, startDateSqlParameter)
                .ToList();
        }

        public IList<TigerEmployee> GetWithEndDate(DateTime endDate)
        {
            var sql = string.Format("{0} WHERE {1}",
                TigerEmployeeSql,
                @" febaj IS NOT NULL AND febaj >= @endDate ");

            var endDateSqlParameter = new SqlParameter("@endDate", endDate);

            return TigerEmployeeSet
                .FromSql(sql, endDateSqlParameter)
                .ToList();
        }

        public List<TigerEmployee> GetByLegajs(int[] legajs)
        {
            var sql = string.Format("{0} WHERE {1}",
                TigerEmployeeSql,
                @" legaj IN (@legajs) ");

            var sqlParameter = new SqlParameter("@legajs", string.Join(",", legajs));

            return TigerEmployeeSet
                .FromSql(sql, sqlParameter)
                .ToList();
        }

        public List<TigerEmployee> GetActive()
        {
            var sql = string.Format("{0} WHERE {1}", TigerEmployeeSql, " febaj IS NOT NULL");

            return TigerEmployeeSet.FromSql(sql).ToList();
        }
    }
}
