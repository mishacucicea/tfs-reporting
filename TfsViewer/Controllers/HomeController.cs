using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.TeamFoundation.Common.Internal;
using TfsConnector;

namespace TfsViewer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ThrowException()
        {
            throw new Exception("Simulated Exception.");
        }

        public ActionResult ErrorPage()
        {
            return View("Error");
        }

        
    }
}
