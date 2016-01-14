using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TfsConnector;
using TfsViewer.Helpers;

namespace TfsViewer
{
    public class QueryHelper
    {
        private TfsProjects _tfsProject = null;
        private TfsQueries _query = null;
        private static QueryHelper _instance = null;

        private QueryHelper()
        {
            _tfsProject = new TfsProjects(ProjectHelper.TfsContext);
            _query = new TfsQueries(_tfsProject);
        }

        public static void ResetInstance()
        {
            _instance = null;
        }

        public static QueryHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QueryHelper();
                }

                return _instance;
            }
        }

        public IEnumerable<TfsTask> GetTasks( string iteration)
        {
            
            return _query.GetTfsTasks(iterationPath: iteration).OrderBy(q => q.AssignedTo);
        }

        public IEnumerable<TfsItem> GetTasksByQueryId(string queryId)
        {
            TfsProjects tp = new TfsProjects(ProjectHelper.TfsContext);
            TfsQueries _query = new TfsQueries(tp);

            return _query.ExecuteQuery(new Guid(queryId));
        }

        public IEnumerable<TfsStory> GetStories()
        {
            IEnumerable<TfsStory> stories = _query.GetTfsStories();
            return stories;
        }

        public IEnumerable<TfsIteration> GetIterations(out string currentIteration)
        {
            var settings = _tfsProject.GetTeamSettings();
            currentIteration = settings.CurrentIterationPath;

            var iterationList = from iteration in settings.IterationPaths
                                orderby iteration
                                select new TfsIteration
                                {
                                    Name = iteration,
                                    Path = iteration,
                                    StartDate = _tfsProject.GetIteration(iteration).StartDate,
                                    FinishDate = _tfsProject.GetIteration(iteration).FinishDate
                                };

            return iterationList;
        }

        public IEnumerable<TfsIteration> GetIterations()
        {
            var settings = _tfsProject.GetTeamSettings();
            
            var iterationList = from iteration in settings.IterationPaths
                                orderby iteration
                                select new TfsIteration
                                {
                                    Name = iteration,
                                    Path = iteration,
                                    StartDate = _tfsProject.GetIteration(iteration).StartDate,
                                    FinishDate = _tfsProject.GetIteration(iteration).FinishDate

                                };
            return iterationList;
        }

        public IEnumerable<TreeViewLocation> GetQueries()
        {
            var queries = _tfsProject.Project.QueryHierarchy;
            var treeViewLocation = ToTreeViewLocation(queries);

            return treeViewLocation;
        }

        private IEnumerable<TreeViewLocation> ToTreeViewLocation(IEnumerable<QueryItem> queryItems)
        {
            List<TreeViewLocation> list = new List<TreeViewLocation>();

            foreach (var item in queryItems)
            {
                var treeView = new TreeViewLocation()
                {
                    Name = item.Path,
                    Id = item.Id.ToString()
                };

                if (item is QueryFolder)
                {
                    treeView.ChildLocations = ToTreeViewLocation((QueryFolder)item);
                }

                list.Add(treeView);
            }

            return list;
        }

        public IEnumerable<TfsStory> GetHistoricalItemData()
        {
            return _query.GetTfsStories().OrderBy(q => q.FirstEstimationDate);
        }    
    }
}