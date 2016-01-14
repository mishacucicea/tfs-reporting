using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client.Catalog.Objects;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System.Security.Permissions;

namespace TfsConnector
{
    /// <summary>
    /// Facilitates accessing information related to a project collection. 
    /// </summary>
    public class TfsProjects
    {
        public TfsTeamProjectCollection ProjectCollection { get; set; }
        public WorkItemStore Store { get; set; }
        public Project Project
        {
            get
            {
                return Store.Projects[ProjectName];
            }
        }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="TfsProjects"/> class from being created.
        /// This might change if the project name can be optional.
        /// </summary>
        public TfsProjects(string tfsUri, string username, string password, string projectName = null)
        {
            NetworkCredential netCred = new NetworkCredential(username, password);

            BasicAuthCredential basicCred = new BasicAuthCredential(netCred);
            TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred);
            tfsCred.AllowInteractive = false;

            ProjectCollection = new TfsTeamProjectCollection(new Uri(tfsUri), tfsCred);
            ProjectCollection.Authenticate();

            Store = new WorkItemStore(ProjectCollection);
            ProjectName = projectName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsProjects"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="projectName">Name of the project.</param>
        public TfsProjects(ITfsContext context)
        {
            NetworkCredential netCred = new NetworkCredential(context.Username, context.Password);

            BasicAuthCredential basicCred = new BasicAuthCredential(netCred);
            TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred);
            tfsCred.AllowInteractive = false;

            ProjectCollection = new TfsTeamProjectCollection(new Uri(context.Uri), tfsCred);
            ProjectCollection.Authenticate();

            Store = new WorkItemStore(ProjectCollection);
            ProjectName = context.ProjectName;
        }

        /// <summary>
        /// Gets the team settings using the default team for the project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        public TeamSettings GetTeamSettings()
        {
            //TODO: need to see what happens when there are multiple teams on the same project
            var configSvc = ProjectCollection.GetService<TeamSettingsConfigurationService>();
            var configs = configSvc.GetTeamConfigurationsForUser(new[] { Project.Uri.AbsoluteUri });
            var teamConfig = configs.FirstOrDefault(c => c.IsDefaultTeam);

            return teamConfig.TeamSettings;
        }

        /// <summary>
        /// Gets the iteration details.
        /// </summary>
        /// <param name="iterationPath">The iteration path (including the project name, e.g. "TfsReporting\Sprint 3").</param>
        /// <returns></returns>
        public TfsIteration GetIteration(string iterationPath)
        {
            // you need to add the “Iteration” word after the team project name.
            // //Project Name\\Iteration\Iteration 1
            var projectNameIndex = iterationPath.IndexOf("\\", 2);
            var fullPath = iterationPath.Insert(projectNameIndex, "\\Iteration");

            var css = ProjectCollection.GetService<ICommonStructureService4>();
            var node = css.GetNodeFromPath(fullPath);
            var iteration = new TfsIteration()
            {
                StartDate = node.StartDate,
                FinishDate = node.FinishDate,
                Name = node.Name,
                Path = node.Path
            };

            return iteration;
        }

        /// <summary>
        /// Gets the iteration paths.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="currentIteration">The current iteration.</param>
        /// <returns></returns>
        public IEnumerable<TfsIteration> GetIterationPaths(out string currentIteration)
        {
            var settings = GetTeamSettings();
            currentIteration = settings.CurrentIterationPath;

            var iterationList = from iteration in settings.IterationPaths
                                orderby iteration
                                select new TfsIteration
                                {
                                    Name = iteration,
                                    Path = iteration
                                };

            return iterationList;
        }

        /// <summary>
        /// Gets the current iteration path.
        /// </summary>
        /// <value>
        /// The current iteration path.
        /// </value>
        public string CurrentIterationPath
        {
            get
            {
                var settings = GetTeamSettings();
                return settings.CurrentIterationPath;
            }
        }
    }
}
