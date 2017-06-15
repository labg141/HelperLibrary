using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.IO;

namespace HelperLibrary.Helpers
{
    public static class DataHelper
    {
        /// <summary>
        /// Metodó que convierte un DataTable a una lista del modelo especificado.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<object> DataTableToObjectList(object model, DataTable data)
        {
            List<object> list = new List<object>();
            foreach (DataRow dr in data.Rows)
            {
                object item = Activator.CreateInstance(model.GetType());
                item = DataRowToModel(item, dr);
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Metodo que convierte un DataRow a un objeto del modelo especificado.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static object DataRowToModel(object model, DataRow row)
        {
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                try
                {
                    if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                        property.SetValue(model, row[property.Name],null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            return model;
        }

        /// <summary>
        /// Metodó que asigna los valores de un DataRow al modelo estatico especificado.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void DataRowToStaticModel(Type model, DataRow row)
        {
            foreach (PropertyInfo property in model.GetProperties())
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    property.SetValue(model, row[property.Name],null);
            }
        }

        public static Dictionary<string, object> DataRowToDictonary(DataRow row)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict = row.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => row[c]);
            return dict;
        }

        public static StringBuilder FieldsToXML(object model, DataTable fields)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<Params>");
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                foreach (DataRow row in fields.Rows)
                {                    
                    if (row["COLUMN_NAME"].ToString() == property.Name)
                    {
                        xml.Append("<" + property.Name + ">");
                        object value = property.GetValue(model, null);
                        if ( value != null)
                        {
                            if (value.GetType() == typeof(string))                            
                                xml.Append("'" + value.ToString() + "'");                            
                            else if(value.GetType() == typeof(bool))
                            {
                                if ((bool)value == true)
                                    xml.Append("1");
                                else
                                    xml.Append("0");
                            }
                            else
                                xml.Append(value.ToString());
                        }
                        else
                            xml.Append("NULL");
                        xml.Append("</" + property.Name + ">");
                    }
                }
            }
            xml.Append("</Params>");

            return xml;
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
    }
}
