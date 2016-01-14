using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using TfsConnector;

namespace TfsViewer.Helpers
{
    public class ProjectHelper
    {
        private static HttpSessionState session = HttpContext.Current.Session;

        public static ITfsContext TfsContext
        {
            get
            {
                return session["TfsContext"] != null ? (ITfsContext)session["TfsContext"] : new TfsConfigurationContext();
            }
            set
            {
                session["TfsContext"] = value;
                QueryHelper.ResetInstance();
            }
        }

        public static string ConnectionType
        {
            get
            {
                if ((ProjectHelper.TfsContext == null) || (ProjectHelper.TfsContext is TfsConfigurationContext))
                {
                    return "Default";
                }
                else
                {
                    return "Custom";
                }
            }
        }

        private static float GetTotalNumberOfStories(DateTime date)
        {
            IEnumerable<TfsStory> tfsStories = QueryHelper.Instance.GetStories();

            int count = (from story in tfsStories
                             where (story.ClosedDate > DateTime.Now  || story.ClosedDate == null)
                             && story.State != "Removed"
                         select story).Count();
           
            return count;
        }

        private static float GetTotalOfUnestimatedStories(DateTime date)
        {
            IEnumerable<TfsStory> tfsStories = QueryHelper.Instance.GetStories();

            int count = (from story in tfsStories
                         where (story.FirstEstimationDate > date || story.FirstEstimationDate == null)
                                && story.CreatedDate <= date && story.State != "Removed"
                         select story).Count();
            return count;
        }

        public static string PercentageOfUnestimateTasks(DateTime date)
        {
            float result = ((GetTotalOfUnestimatedStories(date) / GetTotalNumberOfStories(date)) * 100);
            return result.ToString("0.00");
        }
    
    }
}