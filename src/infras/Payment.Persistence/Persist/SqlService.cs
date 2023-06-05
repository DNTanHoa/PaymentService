using Microsoft.AspNetCore.Mvc;
using Payment.Application.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Persistence.Persist
{
    public class SqlService : ISqlService
    {
        public SqlParameter CreateOutputParameter(string name, SqlDbType type)
        {
            var param  = new SqlParameter(name, type);
            param.Direction = ParameterDirection.Output;
            return param;
        }

        public SqlParameter CreateOutputParameter(string name, SqlDbType type, int size)
        {
            var param = new SqlParameter(name, type, size);
            param.Direction = ParameterDirection.Output;
            return param;
        }

        public (int, string) ExecuteNonQuery(string connectionString, string sqlObjectName, params SqlParameter[] parameters)
        {
            int result = -1;
            string message = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlObjectName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 900;
                        cmd.Parameters.AddRange(parameters);
                        conn.Open();
                        result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return (result, message);
        }

        public Task<(int, string)> ExecuteNonQueryAsync(string connectionString, string sqlObjectName, params SqlParameter[] parameters)
        {
            int result = -1;
            string message = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlObjectName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 900;
                        cmd.Parameters.AddRange(parameters);
                        conn.OpenAsync();
                        result = cmd.ExecuteNonQuery();
                        conn.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Task.FromResult((result, message));
        }

        public (DataTable, string) FillDataTable(string connectionString, string sqlObjectName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string message = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlObjectName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 900;
                        cmd.Parameters.AddRange(parameters);
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        conn.Open();
                        adapter.Fill(dt);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return (dt, message);
        }

        public Task<(DataTable, string)> FillDataTableAsync(string connectionString, string sqlObjectName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string message = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlObjectName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 900;
                        cmd.Parameters.AddRange(parameters);
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        conn.OpenAsync();
                        adapter.Fill(dt);
                        conn.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }


            return Task.FromResult((dt, message));
        }
    }
}
