using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary
{
    class SQLConn
    {
        public static void set_ConnectionStrings()
        {            
        }


        public DataTable ExecuteSPDT(string storedProcedure, SqlParameter[] sqlParameters, SqlConnection conexion)
        {
            using (SqlConnection sqlConnection = conexion)
            {
                using (SqlDataAdapter sqlDa = new SqlDataAdapter())
                {
                    using (SqlCommand sqlCmd = new SqlCommand())
                    {
                        DataTable dt = new DataTable();

                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = storedProcedure;
                        sqlCmd.Parameters.AddRange(sqlParameters);

                        sqlDa.SelectCommand = sqlCmd;

                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();

                        sqlDa.Fill(dt);
                        if (sqlConnection.State == ConnectionState.Open)
                            sqlConnection.Close();

                        return dt;
                    }
                }
            }
        }

        public DataSet ExecuteSPDS(string storedProcedure, SqlParameter[] sqlParameters, SqlConnection conexion)
        {
            using (SqlConnection sqlConnection = conexion)
            {
                using (SqlDataAdapter sqlDa = new SqlDataAdapter())
                {
                    using (SqlCommand sqlCmd = new SqlCommand())
                    {
                        DataSet dt = new DataSet();

                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = storedProcedure;
                        sqlCmd.Parameters.AddRange(sqlParameters);

                        sqlDa.SelectCommand = sqlCmd;

                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();

                        sqlDa.Fill(dt);
                        if (sqlConnection.State == ConnectionState.Open)
                            sqlConnection.Close();

                        return dt;
                    }
                }
            }
        }
    }
}
