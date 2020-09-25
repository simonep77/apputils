using System.Net;
using System.Text;

namespace AppUtils.Lib.Web
{


    /// <summary>

    /// ''' Funzioni comuni protocollo HTTP

    /// ''' </summary>

    /// ''' <remarks></remarks>
    public class Http
    {

        /// <summary>
        ///     ''' Esegue il download di un file sul path specificato
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="path"></param>
        ///     ''' <remarks></remarks>
        public static void DownloadFile(string url, string path)
        {
            using (WebClient webCli = new WebClient())
            {
                webCli.DownloadFile(url, path);
            }
        }


        /// <summary>
        ///     ''' Esegue l'upload del file specificato sull'url
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="path"></param>
        ///     ''' <remarks></remarks>
        public static void UploadFile(string url, string path)
        {
            using (WebClient webCli = new WebClient())
            {
                webCli.UploadFile(url, path);
            }
        }


        /// <summary>
        ///     ''' Esegue chiamata a WS tramite WebClient Post [Timeout default= 5 ore]
        ///     ''' </summary>
        ///     ''' <param name="WsUrl"></param>
        ///     ''' <param name="WsMethod"></param>
        ///     ''' <param name="WsXmlns"></param>
        ///     ''' <returns></returns>
        public static string WsCall(string WsUrl, string WsMethod, string WsXmlns, long timeOut = 18000000)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            sb.Append("  <soap:Body>");
            sb.AppendFormat("   <{0} xmlns=\"{1}\" />", WsMethod, WsXmlns);
            sb.Append(" </soap:Body>");
            sb.Append("</soap:Envelope>");

            using (WebClientEx WsCli = new WebClientEx())
            {
                WsCli.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                WsCli.TimeoutMSec = (int)timeOut;
                return Encoding.UTF8.GetString(WsCli.UploadData(WsUrl, "POST", Encoding.UTF8.GetBytes(sb.ToString())));
            }
        }
    }

}
