using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsConnector
{
    public interface ITfsContext
    {
        string Uri { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string ProjectName { get; set; }
    }
}
