using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsConnector
{
    public class TfsTask : TfsItem
    {
        public double CompletedWork { get; set; }
        public double RemainingWork { get; set; }
        public double OriginalEstimate { get; set; }
    }
}
