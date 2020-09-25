using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace AppUtils.Lib.Web
{
    /// <summary>
    /// Web client con proprieta' estese esposte
    /// </summary>
    public class WebClientEx : System.Net.WebClient
    {
        public int TimeoutMSec { get; set; } = 60 * 1000;
        public bool UsaCookie { get; set; }


        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest oWebReq = base.GetWebRequest(address);

            oWebReq.Timeout = this.TimeoutMSec;

            // 'Imposta cookie handler
            HttpWebRequest oHttp = oWebReq as HttpWebRequest;

            if (oHttp != null)
            {
                if (UsaCookie && oHttp.CookieContainer == null)
                    oHttp.CookieContainer = new CookieContainer();
            }

            return oWebReq;
        }
    }

}
