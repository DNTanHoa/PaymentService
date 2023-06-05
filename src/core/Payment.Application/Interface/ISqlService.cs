using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Interface
{
    public interface ISqlService
    {
        SqlParameter CreateOutputParameter(string name, SqlDbType type);
        SqlParameter CreateOutputParameter(string name, SqlDbType type, int size);

        (int, string) ExecuteNonQuery(string connectionString, string sqlObjectName,
            params SqlParameter[] parameters);
        Task<(int, string)> ExecuteNonQueryAsync(string connectionString, string sqlObjectName,
            params SqlParameter[] parameters);
        (DataTable, string) FillDataTable(string connectionString, string sqlObjectName,
            params SqlParameter[] parameters);
        Task<(DataTable, string)> FillDataTableAsync(string connectionString, string sqlObjectName,
            params SqlParameter[] parameters);
    }
}
