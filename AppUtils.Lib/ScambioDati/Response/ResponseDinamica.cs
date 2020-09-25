using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Response con dati dinamici
    /// </summary>
    public class ResponseDinamica : ResponseBase
         
    {
        private dynamic _Data = new ExpandoObject();

        /// <summary>
        /// Contenitore Generico di dati
        /// </summary>
        public dynamic Data
        {
            get
            {
                return this._Data;
            }
            set
            {
                this._Data = value;
            }
        }


    }
}
