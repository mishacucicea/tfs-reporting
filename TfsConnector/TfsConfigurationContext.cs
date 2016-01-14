using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsConnector
{
    public class TfsConfigurationContext : ITfsContext
    {
        public string Uri
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsUri"];
            }
            set { throw new InvalidOperationException("Uri configured in appsettings"); }
        }

        public string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsUsername"];
            }
            set { throw new InvalidOperationException("Username configured in appsettings"); }
        }

        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsPassword"];
            }
            set { throw new InvalidOperationException("Password configured in appsettings"); }
        }

        public string ProjectName
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsProjectName"];
            }
            set { throw new InvalidOperationException("ProjectName configured in appsettings"); }
        }
    }
}
