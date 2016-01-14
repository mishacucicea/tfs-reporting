using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsConnector;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Collections.Specialized;

namespace TfsConnectorTestClient
{
    class Program
    {
        const string ProjectName = "TfsReportingTest";

        static void Main(string[] args)
        {
            //ListProjects();
            // ListTfsItems();
            // ListIteration();
            //ListQueries();

            QueryAccessToken("2c1f591e-37d2-422d-e703-729d06e9bcff");
        }

        private static void ListQueries()
        {
            TfsProjects ttp = new TfsProjects(new TfsConfigurationContext());
            var queries = ttp.Project.QueryHierarchy;
            ListQuery(queries);
        }

        private static void ListQuery(IEnumerable<QueryItem> queryItems, int depth = 0)
        {
            foreach (var item in queryItems)
            {
                Debug.Write(String.Concat(Enumerable.Repeat("\t", depth)));
                Debug.WriteLine(string.Format("[{0}]{1}", item.Id, item.Path));
                if (item is QueryFolder)
                {
                    ListQuery((QueryFolder)item, depth + 1);
                }
            }
        }

        private static void ListIteration()
        {
            TfsProjects ttp = new TfsProjects(new TfsConfigurationContext());
            var x = ttp.GetIteration(ProjectName+@"\Sprint 3");
        }

        private static void ListProjects()
        {
            // URI of the team project collection
            string _myUri = @"https://bupa-international.visualstudio.com";

            TfsConfigurationServer configurationServer =
                            TfsConfigurationServerFactory.GetConfigurationServer(new Uri(_myUri));

            CatalogNode catalogNode = configurationServer.CatalogNode;

            ReadOnlyCollection<CatalogNode> tpcNodes = catalogNode.QueryChildren(
                            new Guid[] { CatalogResourceTypes.ProjectCollection },
                            false, CatalogQueryOptions.None);

            // tpc = Team Project Collection
            foreach (CatalogNode tpcNode in tpcNodes)
            {
                Guid tpcId = new Guid(tpcNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection tpc = configurationServer.GetTeamProjectCollection(tpcId);

                // Get catalog of tp = 'Team Projects' for the tpc = 'Team Project Collection'
                var tpNodes = tpcNode.QueryChildren(
                          new Guid[] { CatalogResourceTypes.TeamProject },
                          false, CatalogQueryOptions.None);

                foreach (var p in tpNodes)
                {
                    Debug.Write(Environment.NewLine + " Team Project : " + p.Resource.DisplayName + " - " + p.Resource.Description + Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Lists the TFS items.
        /// </summary>
        public static void ListTfsItems()
        {
            TfsProjects ttp = new TfsProjects(new TfsConfigurationContext());

            TfsQueries queries = new TfsQueries(ttp);
            var items = queries.GetTfsTasks(ProjectName+@"\Sprint 5");


            foreach (var workItem in items)
            {

                Console.WriteLine(string.Format("{0}; {1}; {2}; {3}",
                    workItem.AssignedTo,
                    workItem.Title,
                    workItem.OriginalEstimate,
                    workItem.CompletedWork));
            }
        }

        public static string QueryAccessToken(string authorizationCode)
        {
            string accessToken;
            var data = new NameValueCollection();
            data["client_id"] = "000000004C10CE7D";
            data["redirect_uri"] = "http://misha.localtest.me";
            data["client_secret"] = "MT8UnIEiBoUsNB8EZvg1xk1nFUusSEAU";
            data["code"] = authorizationCode;
            data["grant_type"] = "authorization_code";

            string url = "https://login.live.com/oauth20_token.srf";
            string qs = CreateQueryString(data);
            using (var wb = new WebClient())
            {
                var response = wb.UploadValues(url, "POST", data);
               var stringResponse = Encoding.UTF8.GetString(response);
            }

            return null;
        }

        private static string CreateQueryString(Dictionary<string, string> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dic)
            {
                sb.AppendFormat("{0}={1}&", item.Key, System.Uri.EscapeUriString(item.Value));
            }
            return sb.ToString(0, sb.Length - 1);
        }

        private static string CreateQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in nvc.AllKeys)
            {
                sb.AppendFormat("{0}={1}&", key, System.Uri.EscapeUriString(nvc[key]));
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// The json helper.
        /// </summary>
        internal static class JsonHelper
        {
            /// <summary>
            /// The deserialize.
            /// </summary>
            /// <param name="stream">
            /// The stream.
            /// </param>
            /// <typeparam name="T">The type of the value to deserialize.</typeparam>
            /// <returns>
            /// The deserialized value.
            /// </returns>
            public static T Deserialize<T>(Stream stream)
            where T : class
            {
                return (T)(new DataContractJsonSerializer(typeof(T))).ReadObject(stream);
            }
        }
    }
}
