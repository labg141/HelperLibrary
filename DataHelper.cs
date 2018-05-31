using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Helpers
{
    public static class DataHelper
    {
        /// <summary>
        /// Converts a DataRow into the desired model
        /// </summary>
        /// <param name="model">Model the DataRow is going to be converted to.</param>
        /// <param name="row">Datarow needed to convert.</param>
        /// <returns></returns>
        public static object DataRowToModel(Type objectType, DataRow row)
        {
            object model = Activator.CreateInstance(objectType);
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    property.SetValue(model, row[property.Name],null);                
            }
            return model;
        }

        /// <summary>
        /// Converts a DataTable into a list of the desired model
        /// </summary>
        /// <param name="model">Model the list is going to be.</param>
        /// <param name="data">DataTable needed to convert.</param>
        /// <returns></returns>
        public static List<object> DataTableToObjectList(Type objectType, DataTable data)
        {
            List<object> list = new List<object>();
            foreach (DataRow dr in data.Rows)
            {                
                object item = DataRowToModel(objectType, dr);
                list.Add(item);
            }
            return list;
        }



        public static DataTable ByteToDataTable( byte[] byteArrayData )
        {            
            DataTable dt;            
            using (MemoryStream stream = new MemoryStream(byteArrayData))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                dt = (DataTable)bformatter.Deserialize(stream);
                return dt;               
            }
        }
    }
}