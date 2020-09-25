using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using AppUtils.Lib.Response;

namespace AppUtils.Lib.Request

{
    /// <summary>
    /// request base per ricerche paginate
    /// </summary>
    public class RequestPagedBase : RequestBase
    {

        public const int SORT_ASC = 0;
        public const int SORT_DESC = 1;

        public int Rows {get; set;}

        public int Page { get; set; }

        public string SortField { get; set; } = string.Empty;

        public int SortOrder { get; set; }


    }




}
