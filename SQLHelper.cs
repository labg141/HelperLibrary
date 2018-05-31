using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace Helpers
{
    public static class SQLHelper
    {
        /// <summary>
        /// Gets a DataTable from specified stored procedure.
        /// </summary>
        /// <param name="SP">Stored Procedure name.</param>
        /// <param name="model">Model where the data is extracted.</param>
        /// <param name="connectionString">Connection string needed to connect to the databse.</param>
        /// <param name="Parameters">Parameter array (optional), used for custom parameters.</param>
        /// <returns></returns>
        public static DataTable GetDataDT(string SP, object model, string connectionString, SqlParameter[] Parameters = null)
        {
            Mis_ConfieEntities db = new Mis_ConfieEntities();            
            DataTable DT = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (Parameters == null)
                    Parameters = GetParameterArray(model);
                DT = ExecuteSPDT(SP, Parameters, connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return DT;
        }

        /// <summary>
        /// Gets a DataSet from specified stored procedure.
        /// </summary>
        /// <param name="SP">Stored Procedure name.</param>
        /// <param name="model">Model where the data is extracted.</param>
        /// <param name="connectionString">Connection string needed to connect to the databse.</param>
        /// <param name="Parameters">Parameter array (optional), used for custom parameters.</param>
        /// <returns></returns>
        public static DataSet GetDataDS(string SP, object model, string connectionString, SqlParameter[] Parameters = null)
        {
            DataSet DS = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (Parameters == null)
                    Parameters = GetParameterArray(model);                                    
                DS = ExecuteSPDS(SP, Parameters, connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DS;
        }

        public static void ExecuteSP (string SP, object model, string connectionString, SqlParameter[] Parameters = null)
        {
            Mis_ConfieEntities db = new Mis_ConfieEntities();            
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (Parameters == null)
                    Parameters = GetParameterArray(model);
                ExecuteSPDT(SP, Parameters, connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        
        

        /// <summary>
        ///Gets a Custom parameter array needed for a stored procedure. Used when the model does not fit with specified Stored Procedure.
        /// </summary>
        /// <param name="model">Model where the data gets extracted.</param>
        /// <param name="database">Database where the stored procedure is located.</param>
        /// <param name="SP">Name of the stored procedure that is going to be used.</param>
        /// <param name="schema">Schema of the stored procedure</param>
        /// <param name="connectionString">Connection string needed to connecto to the database.</param>
        /// <returns></returns>
        public static SqlParameter[] GetCustomParameterArray(object model, string database, string SP, string schema, string connectionString)
        {
            DataTable ParamNames = GetDataDT("[dbo].[Get_SP_Params]", new { database = database, SP = SP, schema = schema }, connectionString);
            List<SqlParameter> Parameters = new List<SqlParameter>();            
            foreach (DataRow row in ParamNames.Rows)
            { 
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    if (row["Parameter"].ToString() == "@" + property.Name)
                    {
                        if (property.GetValue(model, null) != null)
                        {
                            SqlDbType dbType = GetDBType(property.PropertyType);
                            SqlParameter param = new SqlParameter("@" + property.Name, dbType);
                            param.Value = property.GetValue(model, null);
                            Parameters.Add(param);
                        }
                    }                    
                }                
            }
            return Parameters.ToArray<SqlParameter>();
        }

        /// <summary>
        /// Sets a SQL Parameter array from specified model
        /// </summary>
        /// <param name="model">Model where the data is going to be extracted.</param>
        /// <returns></returns>
        private static SqlParameter[] GetParameterArray(object model)
        {
            int propertyCount = model.GetType().GetProperties().Select(x => x.GetValue(model, null)).Count(x => x != null);
            SqlParameter[] Parameters = new SqlParameter[propertyCount];
            int i = 0;
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (property.GetValue(model, null) != null)
                {
                    SqlDbType dbType = GetDBType(property.PropertyType);
                    Parameters[i] = new SqlParameter("@" + property.Name, dbType);
                    Parameters[i].Value = property.GetValue(model, null);
                    i++;
                }
            }
            return Parameters;
        }

        private static SqlDbType GetDBType(Type varType)
        {
            if (varType == typeof(string))
                return SqlDbType.VarChar;
            else if (varType == typeof(int) || varType == typeof(int?))
                return SqlDbType.Int;
            else if (varType == typeof(DateTime) || varType == typeof(DateTime?))
                return SqlDbType.DateTime;
            else if (varType == typeof(double) || varType == typeof(double?))
                return SqlDbType.Float;
            else if (varType == typeof(bool) || varType == typeof(bool?))
                return SqlDbType.Bit;
            else if (varType == typeof(byte[]))
                return SqlDbType.VarBinary;
            else if (varType == typeof(DataTable))
                return SqlDbType.Structured;
            else
                return SqlDbType.VarChar;
            
        }

        private static DataTable ExecuteSPDT(string SP, SqlParameter[] Parameters, SqlConnection connection)
        {
            using (SqlConnection sqlConnection = connection)
            { 
                using (SqlDataAdapter sqlDA = new SqlDataAdapter())
                {
                    using (SqlCommand sqlCmd = new SqlCommand())
                    {
                        DataTable DT = new DataTable();
                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = SP;
                        sqlCmd.Parameters.AddRange(Parameters);

                        sqlDA.SelectCommand = sqlCmd;
                        if(sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlDA.Fill(DT);
                        if (sqlConnection.State == ConnectionState.Open)
                            sqlConnection.Close();
                        return DT;
                    }
                }
            }
        }
        private static DataSet ExecuteSPDS(string SP, SqlParameter[] Parameters, SqlConnection connection)
        {
            using (SqlConnection sqlConnection = connection)
            {
                using (SqlDataAdapter sqlDA = new SqlDataAdapter())
                {
                    using (SqlCommand sqlCmd = new SqlCommand())
                    {
                        DataSet DS = new DataSet();
                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandTimeout = 0;
                        sqlCmd.CommandText = SP;
                        sqlCmd.Parameters.AddRange(Parameters);

                        sqlDA.SelectCommand = sqlCmd;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlDA.Fill(DS);
                        if (sqlConnection.State == ConnectionState.Open)
                            sqlConnection.Close();
                        return DS;
                    }
                }
            }
        }


    }
}