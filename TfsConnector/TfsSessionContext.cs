using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace TfsConnector
{
    public class TfsSessionContext : ITfsContext
    {
        private HttpSessionState session = HttpContext.Current.Session;

        public string Uri
        {
            get
            {
                return (string)session["Uri"];
            }
            set
            {
                session["Uri"] = value;
            }
        }

        public string Username
        {
            get
            {
                return (string)session["Username"];
            }
            set
            {
                session["Username"] = value;
            }
        }

        public string Password
        {
            get
            {
                return (string)session["Password"];
            }
            set
            {
                session["Password"] = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return (string)session["ProjectName"];
            }
            set
            {
                session["ProjectName"] = value;
            }
        }
    }
}
