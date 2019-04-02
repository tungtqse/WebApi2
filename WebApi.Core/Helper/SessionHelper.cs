using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Core.Helper
{
    public static class SessionHelper
    {
        public static object GetSessionObject(string name)
        {
            try
            {
                return HttpContext.Current.Session[name];
            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException("Failed to get the session. Exception Message: " + ex.Message);
            }
        }
    }
}
