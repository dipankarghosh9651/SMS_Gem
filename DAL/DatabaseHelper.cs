using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SMS.DAL
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString;

        public static SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string procedureName, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static (int ReturnCode, string ReturnMessage) ExecuteNonQueryWithReturn(string procedureName, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = 0
                });
                cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = string.Empty
                });
                conn.Open();

                SqlCommandBuilder.DeriveParameters(cmd);

                foreach (SqlParameter procedureParameter in cmd.Parameters)
                {
                    if (procedureParameter.Direction == ParameterDirection.ReturnValue)
                    {
                        continue;
                    }

                    SqlParameter suppliedParameter = null;
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (string.Equals(parameter.ParameterName, procedureParameter.ParameterName,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                suppliedParameter = parameter;
                                break;
                            }
                        }
                    }

                    if (suppliedParameter != null && suppliedParameter.Value != null)
                    {
                        procedureParameter.Value = ConvertParameterValue(
                            suppliedParameter.Value, procedureParameter.SqlDbType);
                    }
                    else if (procedureParameter.Direction == ParameterDirection.Input)
                    {
                        procedureParameter.Value = DBNull.Value;
                    }
                }

                cmd.ExecuteNonQuery();

                SqlParameter pOutCode = FindParameter(cmd, "@ReturnCode");
                SqlParameter pOutMsg = FindParameter(cmd, "@ReturnMessage");

                int code = pOutCode != null && pOutCode.Value != DBNull.Value
                    ? Convert.ToInt32(pOutCode.Value)
                    : 0;
                string msg = pOutMsg != null && pOutMsg.Value != DBNull.Value
                    ? pOutMsg.Value.ToString()
                    : string.Empty;

                return (code, msg);
            }
        }

        private static SqlParameter FindParameter(SqlCommand command, string parameterName)
        {
            foreach (SqlParameter parameter in command.Parameters)
            {
                if (string.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter;
                }
            }

            return null;
        }

        private static object ConvertParameterValue(object value, SqlDbType targetType)
        {
            if (targetType == SqlDbType.Binary ||
                targetType == SqlDbType.VarBinary ||
                targetType == SqlDbType.Image)
            {
                if (value is string text)
                {
                    int commaIndex = text.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        text = text.Substring(commaIndex + 1);
                    }

                    return Convert.FromBase64String(text);
                }
            }

            return value;
        }
    }
}