using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsConnector
{
    /// <summary>
    /// Represents an Iteration entity.
    /// </summary>
    public class TfsIteration
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}
