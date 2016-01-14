using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TfsViewer.Helpers;

namespace TfsViewer.Controllers
{
    public class QueryController : Controller
    {
        //
        // GET: /Query/

        public ActionResult Index()
        {
            var treeView = QueryHelper.Instance.GetQueries();
            return View(treeView);
        }

        public ActionResult HistoricalData()
        {
            IEnumerable<TfsConnector.TfsItem> treeView = QueryHelper.Instance.GetHistoricalItemData();
            return View(treeView);
        }
    }
}
