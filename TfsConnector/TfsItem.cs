using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsConnector
{
    /// <summary>
    /// 
    /// </summary>
    public class TfsItem
    {
        public string AssignedTo { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; }
        public string State { get; set; }
        public int Parent { get; set; }
        public string Tags { get; set; }
    }
}
