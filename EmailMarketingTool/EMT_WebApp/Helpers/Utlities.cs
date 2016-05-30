using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Helpers
{
    public class Utlities
    {
        public static string GetURL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string url = request.Url.ToString();
            return url;
        }
        /// <summary>
        /// New user is been alloted with a starter plan 
        /// </summary>
        public void registerPlan() {

        }

    }
}