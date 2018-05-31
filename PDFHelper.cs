using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Globalization;
using System.IO;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;

namespace Helpers
{
    public class PDFHelper
    {        
        public static MemoryStream createDocument()
        {            
            PdfDocument document = new PdfDocument();
            /*this.Time = document.Info.CreationDate;
            document.Info.Title = "PDFsharp Clock Demo";
            document.Info.Author = "Stefan Lange";
            document.Info.Subject = "Server time: " +
              this.Time.ToString("F", CultureInfo.InvariantCulture);

            // Create new page
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromMillimeter(200);
            page.Height = XUnit.FromMillimeter(200);

            // Create graphics object and draw clock
            XGraphics gfx = XGraphics.FromPdfPage(page);
            RenderClock(gfx);

            // Send PDF to browser
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);*/
            MemoryStream stream = new MemoryStream();
            return stream;
        }       
        



    }

    public static class PdfSharpExtensions
    {
        public static IEnumerable<string> ExtractText( this PdfPage page )
        {            
            var content = ContentReader.ReadContent(page);            
            var text = content.ExtractText();
            return text;
        }

        public static IEnumerable<string> ExtractText( this CObject cObject )
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }
    }
}