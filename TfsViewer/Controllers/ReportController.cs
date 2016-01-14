using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TfsConnector;
using TfsViewer.Helpers;
using TfsViewer.Models;

namespace TfsViewer.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            string currentIteration;
            var iterations = QueryHelper.Instance.GetIterations(out currentIteration);
            IEnumerable<TfsTask> tasks = QueryHelper.Instance.GetTasks(currentIteration);

            ViewBag.IterationList = new SelectList(iterations, "Name", "Path", currentIteration);

            return View(tasks);
        }

        [HttpPost]
        public ActionResult Index(string iterationPath)
        {
            string currentIteration;
            var iterations = QueryHelper.Instance.GetIterations(out currentIteration);
            IEnumerable<TfsTask> tasks = QueryHelper.Instance.GetTasks(iterationPath);

            ViewBag.IterationList = new SelectList(iterations, "Name", "Path", iterationPath);

            return View(tasks);
        }

        public ActionResult UnplannedTasks(string iterationPath)
        {
            string currentIteration;
            var iterations = QueryHelper.Instance.GetIterations(out currentIteration);

            iterationPath = iterationPath ?? currentIteration;

            TfsProjects ttp = new TfsProjects(ProjectHelper.TfsContext);

            var iteration = ttp.GetIteration(iterationPath);

            // Get the tasks that were added after the start of the sprint.
            IEnumerable<TfsItem> tasks = QueryHelper.Instance.GetTasks(iterationPath).
                Where(t => t.CreatedDate.AddDays(-1) > iteration.StartDate && t.CreatedDate < iteration.FinishDate);

            ViewBag.IterationList = new SelectList(iterations, "Name", "Path", iterationPath);

            return View(tasks);
        }

        public ActionResult ExportTasks()
        {
            var treeView = QueryHelper.Instance.GetQueries();
            return View(treeView);
        }

        /// <summary>
        /// Action for getting all the stories from the project
        /// </summary>
        /// <returns> the list of tfsStories</returns>
        public ActionResult Stories()
        {
            IEnumerable<TfsStory> tfsStories = QueryHelper.Instance.GetStories();
            return View(tfsStories);
        }

        /// <summary>
        /// Action for getting all the iterations from the project
        /// </summary>
        /// <returns>a list of tfsIterations</returns>
        public ActionResult Iterations()
        {
            
           string currentIteration;
           IEnumerable<TfsIteration> tfsIterations = QueryHelper.Instance.GetIterations(out currentIteration);

            return View(tfsIterations);
        }
       

        /// <summary>
        /// Action for generating the project report.
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectReport()
        {
            ProjectReportModel model = new ProjectReportModel();

            model.Iterations = QueryHelper.Instance.GetIterations();
            model.UserStories = QueryHelper.Instance.GetStories();
            model.InitCollections();

            if (model.Iterations.Any())
            {
                model.ProjectStartDate = model.Iterations.First().StartDate;
                model.ProjectReleaseDate = model.Iterations.Last().FinishDate;
            }

            ViewBag.Labels = (new JavaScriptSerializer()).Serialize(model.getIterationDates());
            ViewBag.CompletedSPValues = (new JavaScriptSerializer()).Serialize(model.getIterationSPCompletedValues());
            ViewBag.TotalSPValues = (new JavaScriptSerializer()).Serialize(model.getIterationSPTotalValues());
            ViewBag.ValuesPercentChart = (new JavaScriptSerializer()).Serialize(model.getPercentChartValues());
            ViewBag.DatesPercentChart = (new JavaScriptSerializer()).Serialize(model.getPercentChartDates());

            return View(model);
        }
    }
}
