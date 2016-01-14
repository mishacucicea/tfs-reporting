using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using TfsConnector;
using TfsViewer.Helpers;

namespace TfsViewer.Reports
{
    public partial class TaskListing : ViewPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rvTasks.LocalReport.ReportPath = Server.MapPath(@"~\App_Data\Tasks.rdlc");

                Stopwatch sw = new Stopwatch();
                
                sw.Start();
                var tasks =  GetTasks(Request.QueryString["queryId"]);
                sw.Stop();
                long time = sw.ElapsedMilliseconds;
                
                var tasksRds = new ReportDataSource("Tasks", tasks);

                rvTasks.LocalReport.DataSources.Add(tasksRds);
                rvTasks.LocalReport.Refresh();
            }
        }

        private IEnumerable<TfsItem> GetTasks(string queryId)
        {
            IEnumerable<TfsItem> tasks = null;

            if (!string.IsNullOrWhiteSpace(queryId))
            {
                tasks = QueryHelper.Instance.GetTasksByQueryId(queryId);
            }
            
            return tasks;
        }
    }
}