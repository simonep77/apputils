using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Dati di paginazione
    /// </summary>
    public class ResponsePager
    {
        public int Page { get; set; }
        public int Offset { get; set; }
        public int TotPages { get; set; }
        public int TotRecords { get; set; }


        public ResponsePager() { }

    }
}
