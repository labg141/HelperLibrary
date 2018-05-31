using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Helpers
{
    public static class EmailHelper
    {
        public static string PartialViewToString( ControllerContext context, string PartialViewName, object model)
        {
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, PartialViewName);
                ViewDataDictionary data = new ViewDataDictionary();
                data.Model = model;        
                TempDataDictionary tdata = new TempDataDictionary();
                var viewContext = new ViewContext(context, viewResult.View, data, tdata, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}