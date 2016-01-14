using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Reporting.WebForms;

namespace TfsViewer
{
    public static class HtmlExtensions
    { 
        public static IHtmlString RenderControl(this HtmlHelper html, Control control)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter textwriter = new StringWriter(sb))
            {
                HtmlTextWriter htmlWriter = new HtmlTextWriter(textwriter);
                control.RenderControl(htmlWriter);
            }

           return html.Raw(sb);
        }
    }
}