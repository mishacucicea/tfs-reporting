using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Mvc;
using TfsConnector;
using TfsViewer.Helpers;

namespace TfsViewer.Controllers
{
    public class ConfigurationController : Controller
    {
        //
        // GET: /Configuration/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Connect(bool changeConfiguration = false)
        {
            ViewBag.ChangeConfiguration = changeConfiguration;
            return View();
        }

        [HttpPost]
        public ActionResult Connect(string username, string password, string uri, string projectName)
        {
            ProjectHelper.TfsContext = new TfsSessionContext();
            ProjectHelper.TfsContext.Password = password;
            ProjectHelper.TfsContext.Username = username;
            ProjectHelper.TfsContext.Uri = uri;
            ProjectHelper.TfsContext.ProjectName = projectName;

            return RedirectToAction("Index", "Report");
        }

        public ActionResult Reset()
        {
            ProjectHelper.TfsContext = new TfsConfigurationContext();

            return RedirectToAction("Index", "Report");
        }
    }
}
