using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TfsViewer
{
    public class TreeViewLocation
    {
        public TreeViewLocation()
        {
            ChildLocations = new List<TreeViewLocation>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TreeViewLocation> ChildLocations { get; set; }
    }
}