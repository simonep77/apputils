using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Classe base per risposte di ricerca
    /// </summary>
    public abstract class ResponseBasePaged : ResponseBase
    {
        public ResponsePager Pager { get; set; } = new ResponsePager();

    }
}
