using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TfsConnector;
using TfsViewer.Helpers;

namespace TfsViewer.Models
{
    /// <summary>
    /// The model for the project report
    /// </summary>
    public class ProjectReportModel
    {
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectReleaseDate { get; set; }

        public IEnumerable<TfsIteration> Iterations { get; set; }
        public IEnumerable<TfsStory> UserStories { get; set; }

        //private member data
        private Dictionary<string, double> iterationSPCompletedValuesDic;
        private Dictionary<string, double> iterationSPTotalValuesDic;
        private List<string> datesForPercentChart;
        private List<double> valuesForPercentChart;

        //SP for all the user stories that were not allocated to an iteration
        private double unassignedSP = 0;

        //cumulative completed SP
        private double completedSP = 0;

        //cumulative total SP
        private double totalSP = 0;

        //list of ordered iterations
        private List<TfsIteration> orderedIterations;


        /// <summary>
        /// Initializes the collections.
        /// </summary>
        public void InitCollections()
        {
            //create the collections
            iterationSPCompletedValuesDic = new Dictionary<string, double>();
            iterationSPTotalValuesDic = new Dictionary<string, double>();
            datesForPercentChart = new List<string>();
            valuesForPercentChart = new List<double>();

            //sort the iterations by date
            orderedIterations = Iterations.OrderBy(iteration => iteration.StartDate).ToList();

            //populate the iterations paths in the related dictionaries
            foreach (TfsIteration i in orderedIterations)
            {
                if (i.StartDate <= DateTime.UtcNow)
                {
                    iterationSPCompletedValuesDic.Add(i.Path, 0);
                    iterationSPTotalValuesDic.Add(i.Path, 0);
                }
            }

            //compute partial values for each iteration taking in account the userstories
            foreach (TfsStory s in UserStories)
            {
                if (iterationSPCompletedValuesDic.ContainsKey(s.IterationPath) && s.State != "Removed")
                {
                    if (s.State == "Closed")
                    {
                        iterationSPCompletedValuesDic[s.IterationPath] += s.StoryPoints;
                    }
                    iterationSPTotalValuesDic[s.IterationPath] += s.StoryPoints;
                }
                else
                    if(s.State != "Removed") unassignedSP += s.StoryPoints;
            }

            //compute the cumulative data taking in account the unassignedSP
            //by this step each item from the above dictionaries contains the completed SP values and total SP values
            //for each iteration. This step computes a cumulative value meaning that each iteration will take in account
            //the previous data.
            foreach (TfsIteration i in orderedIterations)
            {
                if (iterationSPCompletedValuesDic.ContainsKey(i.Path))
                {
                    //increase the completedSP with the previously calculated value (SP on this particular iterations)
                    completedSP += iterationSPCompletedValuesDic[i.Path];

                    //set the value taking in account the historical value
                    iterationSPCompletedValuesDic[i.Path] = completedSP;
                }

                if (iterationSPTotalValuesDic.ContainsKey(i.Path))
                {
                    totalSP += iterationSPTotalValuesDic[i.Path];
                    iterationSPTotalValuesDic[i.Path] = totalSP + unassignedSP;
                }
            }

            //initialize the values used for computing the percentage of Unestimated US / Total US at a given point in time
            DateTime start = ProjectStartDate.HasValue ? ProjectStartDate.Value : 
                                orderedIterations[0].StartDate.HasValue ? orderedIterations[0].StartDate.Value : DateTime.Now;

            DateTime end = DateTime.Now;
            DateTime newDate = start;
            
            //compute the values with a 1 day step
            do
            {
                string val = ProjectHelper.PercentageOfUnestimateTasks(newDate);
                valuesForPercentChart.Add(Double.Parse(val, System.Globalization.NumberStyles.Float));
                datesForPercentChart.Add(newDate.ToString("MM/dd/yyyy"));
                newDate = newDate.AddDays(1);
            }
            while (newDate <= end);
        }

        /// <summary>
        /// Gets the iteration dates.
        /// </summary>
        /// <returns></returns>
        public List<string> getIterationDates()
        {
            if (orderedIterations == null) return null;

            List<string> dates = new List<string>();

            foreach (TfsIteration i in orderedIterations)
            {
                dates.Add(i.StartDate.HasValue ? i.StartDate.Value.ToString("MM/dd/yyyy") : ProjectStartDate.Value.ToString("MM/dd/yyyy"));
            }

            return dates;
        }

        /// <summary>
        /// Gets the iteration total SP values.
        /// </summary>
        /// <returns></returns>
        public List<double> getIterationSPTotalValues()
        {
            return iterationSPTotalValuesDic.Values.ToList();
        }

        /// <summary>
        /// Gets the iteration completed SP values.
        /// </summary>
        /// <returns></returns>
        public List<double> getIterationSPCompletedValues()
        {
            return iterationSPCompletedValuesDic.Values.ToList();
        }

        /// <summary>
        /// Gets the percentage chart dates.
        /// </summary>
        /// <returns></returns>
        public List<string> getPercentChartDates()
        {
            return datesForPercentChart;
        }

        /// <summary>
        /// Gets the percentage chart values.
        /// </summary>
        /// <returns></returns>
        public List<double> getPercentChartValues()
        {
            return valuesForPercentChart;
        }
    }
}