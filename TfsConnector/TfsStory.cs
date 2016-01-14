using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsConnector

{

    /// <summary>
    /// A TFS User Story
    /// </summary>
    public class TfsStory : TfsItem
    {
        public double StoryPoints { get; set; }
        public string IterationPath { get; set; }
        public IEnumerable<TfsTask> Tasks { get; set; } 
        public DateTime? ClosedDate { get; set; }
        public DateTime? FirstEstimationDate { get; set; }
    }
}
