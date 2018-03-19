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
	a001.legaj, 
	a001.nomb, 
	a001.fenac, 
	a001.feiem, 
	a001.febaj, 
	tb058.dtitu, 
	tb061.didio, 
	tb040.dgrup, 
	a001.calle,
	a001.nro,
	a001.piso,
	a001.depto,
	a001.loca,
	tb021.dprov,
	a001.obsoc,
	tb031.dobso,
	a001.ospla,
	tb032.dospl,
    tb012.domif as officeaddress,
    LOWER(a071.demai) as email
FROM
	a001 
	LEFT JOIN a051 ON a001.empre = a051.empre AND a001.legaj = a051.legaj 
  	LEFT JOIN tb058 ON a051.titul = tb058.titul 
	LEFT JOIN tb040 ON a001.empre = tb040.empre AND a051.grupe = tb040.grupe
	LEFT JOIN tb061 ON a051.idio2 = tb061.idiom
    LEFT JOIN tb021 ON a001.prov = tb021.prov
	LEFT JOIN tb031 ON a001.obsoc = tb031.obsoc
	LEFT JOIN tb032 ON a001.ospla = tb032.ospla AND a001.obsoc = tb032.obsoc
	LEFT JOIN tb012 ON a001.empre = tb012.empre AND a001.flial = tb012.flial
	LEFT JOIN a071 ON a001.empre = a071.empre AND a001.legaj = a071.legaj
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
    }
}
