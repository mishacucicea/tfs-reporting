using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TfsConnector
{
    public class TfsSessionContext : ITfsContext
    {
        private ISession session;

        public TfsSessionContext(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        public string Uri
        {
            get
            {
                return session.GetString("Uri");
            }
            set
            {
                session.SetString("Uri", value);
            }
        }

        public string Username
        {
            get
            {
                return session.GetString("Username");
            }
            set
            {
                session.SetString("Username", value);
            }
        }

        public string Password
        {
            get
            {
                return session.GetString("Password");
            }
            set
            {
                session.SetString("Password", value);
            }
        }

        public string ProjectName
        {
            get
            {
                return session.GetString("ProjectName");
            }
            set
            {
                session.SetString("ProjectName", value);
            }
        }
    }
}
