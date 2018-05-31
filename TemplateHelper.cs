using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;

namespace Helpers
{
    public static class TemplateHelper
    {
        public static List<SelectListItem> GetSelectList(DataTable data, object selectedValue = null, string TextColumn = null, string ValueColumn = null)
        {            
            TextColumn = string.IsNullOrEmpty(TextColumn) || string.IsNullOrWhiteSpace(TextColumn) ? "Text" : TextColumn;
            ValueColumn = string.IsNullOrEmpty(ValueColumn) || string.IsNullOrWhiteSpace(ValueColumn) ? "Text" : ValueColumn;

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in data.Rows) {
                bool selected = false;
                if (selectedValue != null)
                {
                    if (row[ValueColumn].ToString() == selectedValue.ToString())
                    {
                        selected = true;
                    }
                }                
                list.Add(new SelectListItem { Text = row[TextColumn].ToString(), Value = row[ValueColumn].ToString(), Selected = selected });
            }

            return list;
        }


        public static StringBuilder DrawTable(DataTable table, bool hiddenId, bool foot)
        {
            StringBuilder text = new StringBuilder();
            text.Append("");
            text.Append("<thead>");
            text.Append(GetColumns(table,hiddenId));
            text.Append("</thead>");
            text.Append(GetBody(table,hiddenId));
            if (foot)
            {
                text.Append("<tfoot>");
                text.Append(GetColumns(table, hiddenId));    
                text.Append("</tfoot>");
            }            
            return text;
        }

        private static StringBuilder GetColumns(DataTable table, bool hiddenId)
        {
            StringBuilder text = new StringBuilder();
            text.Append("<tr>");
            foreach(DataColumn col in table.Columns)
            {
                if (col.ColumnName.ToUpper().Contains("ID") && hiddenId)
                {
                    text.Append(InsertCol(col, true));
                    continue;                    
                }
                text.Append(InsertCol(col));                    
            }
            text.Append("</tr>");
            return text;
        }

        private static StringBuilder InsertCol(DataColumn col, bool hiddenCol = false)
        {
            StringBuilder colString = new StringBuilder();
            string noExport = col.Caption == " " ? "class = 'noExport'" : "";            
            if (hiddenCol)
                colString.Append("<th style='display:none;' "+noExport+">" + col.Caption + "</th>");
            else
                colString.Append("<th "+noExport+">" + col.Caption + "</th>");
            return colString;
        }

        private static StringBuilder GetBody( DataTable table, bool hiddenId )
        {
            StringBuilder text = new StringBuilder();
            text.Append("<tbody>");
            foreach (DataRow row in table.Rows) 
            {
                text.Append(GetRow(row, hiddenId));
            }
            text.Append("</tbody>");
            return text;
        }

        private static StringBuilder GetRow( DataRow row, bool hiddenId )
        {
            StringBuilder rowString = new StringBuilder();
            Dictionary<string, string> items = row.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => row[c].ToString()); 
            rowString.Append("<tr>");
            foreach (string key in items.Keys)
            {
                if (key.ToUpper().Contains("ID") && hiddenId)
                {
                    rowString.Append("<td style='display:none;'>" + items[key] + "</td>");
                    continue;
                }
                rowString.Append("<td>" + items[key] + "</td>");
            }
            rowString.Append("</tr>");
            return rowString;
        }
    }
}