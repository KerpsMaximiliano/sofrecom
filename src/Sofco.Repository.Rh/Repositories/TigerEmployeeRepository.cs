using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            telef as phoneNumber,
            dbanc as bank,
            ayude,
            nuset,
            nuse1,
            activ,
            dsemi,
            denac
        FROM
	        View_A001
        ";

        private DbSet<TigerEmployee> TigerEmployeeSet { get; }

        private readonly TigerContext Context;

        public TigerEmployeeRepository(TigerContext context)
        {
            TigerEmployeeSet = context.Set<TigerEmployee>();
            Context = context;
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

        public IList<EmployeeSocialCharges> GetSocialCharges(int year, int month)
        {
            var list = new List<EmployeeSocialCharges>();

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "proc_Consulta01";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("Anioq", year));
                command.Parameters.Add(new SqlParameter("meslq", month));

                Context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(MapRow(reader));
                        }
                    }
                }

                Context.Database.CloseConnection();
            }

            return list;
        }

        private EmployeeSocialCharges MapRow(DbDataReader reader)
        {
            var employee =new EmployeeSocialCharges();

            if (!reader.IsDBNull(reader.GetOrdinal("legaj")))
            {
                employee.EmployeeNumber = reader.GetInt32(reader.GetOrdinal("legaj")).ToString();
            }

            if (!reader.IsDBNull(reader.GetOrdinal("nroc")))
            {
                employee.AccountNumber = reader.GetInt32(reader.GetOrdinal("nroc"));
            }

            if (!reader.IsDBNull(reader.GetOrdinal("cuenta")))
            {
                employee.AccountName = reader.GetString(reader.GetOrdinal("cuenta"));
            }

            if (!reader.IsDBNull(reader.GetOrdinal("neto")))
            {
                employee.Value = reader.GetDecimal(reader.GetOrdinal("neto"));
            }

            return employee;
        }
    }
}
