using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace TfsViewer
{
    public class LogErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs. Log exception and execute base.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            ILog log = LogManager.GetLogger(typeof(LogErrorAttribute));

            log.Error(filterContext.Exception.ToString());

            // forward the exception context to the base
            base.OnException(filterContext);
        }
    }
}