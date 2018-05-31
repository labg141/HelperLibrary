using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Globalization;
using System.Threading;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Helpers
{
    public static class ExcelHelper
    {
        public static DataTable ExcelToDataTable( bool hasHeader,string path, byte[] fileArray = null)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {                
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                FileStream file = null;
                MemoryStream memStream = new MemoryStream();

                if (fileArray == null)
                {
                    file = File.OpenRead(path);
                }
                else
                {                    
                    memStream.Write(fileArray, 0, fileArray.Length);                   
                }

                if (file == null)
                    pck.Load(memStream);
                else
                    pck.Load(file);
                
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? textInfo.ToTitleCase(firstRowCell.Text).Replace(" ","") : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

        //Converts DataTable to Excel.
        public static ExcelPackage DataTableToExcel( DataTable dt, bool hasHeader, string sheetName )
        {
            ExcelPackage pck = new ExcelPackage();                        
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
            ws.Cells["A1"].LoadFromDataTable(dt, hasHeader, TableStyles.Medium2);
            ws.Cells[ws.Dimension.Address].AutoFitColumns();            
            
            return pck;
        }
    }
}