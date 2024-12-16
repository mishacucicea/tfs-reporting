using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TfsConnector
{
    public class TfsConfigurationContext : ITfsContext
    {
        private readonly IConfiguration _configuration;

        public TfsConfigurationContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Uri
        {
            get
            {
                return _configuration["TfsUri"];
            }
            set { throw new InvalidOperationException("Uri configured in appsettings"); }
        }

        public string Username
        {
            get
            {
                return _configuration["TfsUsername"];
            }
            set { throw new InvalidOperationException("Username configured in appsettings"); }
        }

        public string Password
        {
            get
            {
                return _configuration["TfsPassword"];
            }
            set { throw new InvalidOperationException("Password configured in appsettings"); }
        }

        public string ProjectName
        {
            get
            {
                return _configuration["TfsProjectName"];
            }
            set { throw new InvalidOperationException("ProjectName configured in appsettings"); }
        }
    }
}
