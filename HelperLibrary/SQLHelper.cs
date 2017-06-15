using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;

namespace HelperLibrary
{
    static class SQLHelper
    {
        public static DataTable get_DataDT(string SP, object model, string connectionString, SqlParameter[] Parametros = null)
        {
            SQLConn SQLconn = new SQLConn();
            DataTable DT = new DataTable();
            SqlConnection conexion = new SqlConnection(connectionString);
            try
            {
                if (Parametros == null)
                    Parametros = get_ParameterArray(model);
                DT = SQLconn.ExecuteSPDT(SP, Parametros, conexion);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return DT;
        }

        public static DataSet get_DataDS(string SP, object model, string connectionString, SqlParameter[] Parametros = null)
        {
            SQLConn SQLconn = new SQLConn();
            DataSet DS = new DataSet();
            SqlConnection conexion = new SqlConnection(connectionString);
            try
            {
                if (Parametros == null)
                    Parametros = get_ParameterArray(model);
                DS = SQLconn.ExecuteSPDS(SP, Parametros, conexion);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return DS;
        }

        private static SqlParameter[] get_ParameterArray(object model)
        {
            int propertyCount = model.GetType().GetProperties().Select(x => x.GetValue(model, null)).Count(x => x != null);
            SqlParameter[] Parametros = new SqlParameter[propertyCount];
            int i = 0;
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (property.GetValue(model, null) != null)
                {
                    SqlDbType dbType = get_dbType(property.PropertyType);
                    Parametros[i] = new SqlParameter("@" + property.Name, dbType);
                    Parametros[i].Value = property.GetValue(model, null);                    
                    i++;
                }
            }

            return Parametros;
        }

        public static DataTable get_TableFields(string connectionString, string SP = null, string database = null, string schema = null, string table = null)
        {
            DataTable fields;
            if (SP != null)
                fields = get_DataDT(SP, new { }, connectionString);
            else
                fields = get_DataDT("[dbo].[get_TableFields]", new { database = database, schema = schema, table = table }, connectionString);
            return fields;
        }

        public static DataTable setXML(string SP, StringBuilder xml, string connectionString)
        {
            SQLConn SQLconn = new SQLConn();
            SqlConnection conexion = new SqlConnection(connectionString);
            SqlParameter[] Parametros = new SqlParameter[1];
            Parametros[0] = new SqlParameter("@XML",SqlDbType.Xml);            
            Parametros[0].Value = xml.ToString();
            return SQLconn.ExecuteSPDT(SP, Parametros, conexion);
        }

        public static SqlParameter[] get_CustomParameterArray(object model, string database,string sp, string schema, string connectionString)
        {
            DataTable paramNames = get_DataDT("[dbo].[get_SP_Params]", new { database = database, sp = sp, schema = schema }, connectionString);
            List<SqlParameter> Parametros = new List<SqlParameter>();

            foreach (DataRow row in paramNames.Rows)
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    if (row["Parametro"].ToString() == "@" + property.Name)
                    {
                        if (property.GetValue(model, null) != null)
                        {
                            SqlDbType dbType = get_dbType(property.PropertyType);
                            SqlParameter param = new SqlParameter("@" + property.Name, dbType);
                            param.Value = property.GetValue(model, null);
                            Parametros.Add(param);
                        }                            
                        break;
                    }

                }
            }
            return Parametros.ToArray<SqlParameter>();
        }


        private static SqlDbType get_dbType(Type varType)
        {
            if (varType.Name == "String")
            {
                return SqlDbType.NVarChar;
            }
            else if (varType.Name == "Int32")
            {
                return SqlDbType.Int;
            }
            else if (varType.Name == "DateTime")
            {
                return SqlDbType.DateTime;
            }
            else if (varType.Name == "Double")
            {
                return SqlDbType.Float;
            }
            else if (varType.Name == "Boolean")
            {
                return SqlDbType.Bit;
            }
            else if (varType == typeof(int?))
            {
                return SqlDbType.Int;
            }
            else if (varType == typeof(bool?))
            {
                return SqlDbType.Bit;
            }
            else if (varType == typeof(DateTime?))
            {
                return SqlDbType.DateTime;
            }
            else if (varType == typeof(DataTable))
            {
                return SqlDbType.Structured ;
            }
            else
            {
                return SqlDbType.NVarChar;
            }
        }
    }
}
