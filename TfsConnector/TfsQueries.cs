using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Client.Catalog.Objects;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections;
using System.Diagnostics;

namespace TfsConnector
{
    public class TfsQueries
    {
        public TfsProjects TeamProject { get; set; }

        private IList<TfsStory> _stories = null;
        private IList<TfsTask> _tasks = null;

        public TfsQueries(TfsProjects teamProject)
        {
            TeamProject = teamProject;
        }

        /// <summary>
        /// Gets the TFS tasks.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <returns></returns>
        public IEnumerable<TfsTask> GetTfsTasks(string iterationPath)
        {
            if (_tasks == null)
            {
                _tasks = new List<TfsTask>();
                string wiql = @"Select [State], [Title] 
                            From [WorkItems]
                            Where [Work Item Type] = 'Task'
                            And [Team Project] = '{0}'
                            And [Iteration Path] = '{1}'";

                WorkItemCollection queryResults = TeamProject.Store.Query(
                    string.Format(wiql, TeamProject.ProjectName, iterationPath));

                foreach (WorkItem item in queryResults)
                {
                    TfsTask task = null;

                    task = item.ToTfsTask();
                    task.Link = string.Format("{0}/{1}/_workitems/edit/{2}", TeamProject.Store.TeamProjectCollection.Uri.AbsoluteUri,
                        TeamProject.ProjectName, item.Id);

                    _tasks.Add(task);
                }
            }
            return _tasks;
        }

        public IEnumerable<TfsStory> GetTfsStories()
        {
            if (_stories == null)
            {
                _stories = new List<TfsStory>();
                string wiql = @"Select [Id], [Title], [Assigned To], [Created Date], [Iteration Path], [State]  
                            From [WorkItems]
                            Where ([Work Item Type] = 'User Story' or [Work Item Type] = 'Product Backlog Item')
                            and [Team Project] = '{0}'";

                WorkItemCollection queryResults = TeamProject.Store.Query(
                   string.Format(wiql, TeamProject.ProjectName));

                foreach (WorkItem item in queryResults)
                {
                    TfsStory story = null;

                    story = item.ToTfsStory();
                    story.Link = string.Format("{0}/{1}/_workitems/edit/{2}", TeamProject.Store.TeamProjectCollection.Uri.AbsoluteUri,
                        TeamProject.ProjectName, item.Id);

                    _stories.Add(story);
                }
            }

            return _stories;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryId">The query identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public IEnumerable<TfsItem> ExecuteQuery(Guid queryId)
        {
            var query = TeamProject.Store.GetQueryDefinition(queryId);
            if (query == null)
            {
                throw new Exception(string.Format("Query with Id = [{0}] not found.", queryId));
            }

            return ExecuteQuery(query);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryDef">The query definition.</param>
        /// <returns></returns>
        public IEnumerable<TfsItem> ExecuteQuery(QueryDefinition queryDef)
        {
            IList<TfsItem> tasks = new List<TfsItem>();

            IEnumerable<WorkItem> workItems = null;

            Query query = new Query(TeamProject.Store, queryDef.QueryText,GetParamsDictionary());

            if (queryDef.QueryType == QueryType.List)
            {    
                
                workItems = query.RunQuery().Cast<WorkItem>();
            }
            else
            {
                workItems = FlatLinkQuery(query);
            }

            // transform the workItems into TfsItem
            foreach (WorkItem item in workItems)
            {
                tasks.Add(item.ToTfsTask());
            }

            return tasks;
        }

        private IDictionary GetParamsDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "project", TeamProject.ProjectName }
            };
        }

        /// <summary>
        /// Flats the link query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private IEnumerable<WorkItem> FlatLinkQuery(Query query)
        {
            var workItemLinks = query.RunLinkQuery();

            // Build the list of work items for which we want to retrieve more information
            int[] ids = (from WorkItemLinkInfo info in workItemLinks
                         select info.TargetId).ToArray();


            // Next we want to create a new query that will retrieve all the column values from the original query, for
            // each of the work item IDs returned by the original query.
            var detailsWiql = new StringBuilder();
            detailsWiql.AppendLine("SELECT");
            bool first = true;

            foreach (FieldDefinition field in query.DisplayFieldList)
            {
                detailsWiql.Append("    ");
                if (!first)
                    detailsWiql.Append(",");
                detailsWiql.AppendLine("[" + field.ReferenceName + "]");
                first = false;
            }
            detailsWiql.AppendLine("FROM WorkItems");

            // Get the work item details
            var flatQuery = new Query(TeamProject.Store, detailsWiql.ToString(), ids);
            WorkItemCollection details = flatQuery.RunQuery();

            return details.Cast<WorkItem>();
        }


        
    }
}