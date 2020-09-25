using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Response paginata con dati dinamici
    /// </summary>
    public class ResponseDinamicaPaged : ResponseBasePaged
         
    {
        /// <summary>
        /// Contenitore Generico di dati
        /// </summary>
        public dynamic Data { get; set; } = new ExpandoObject();





    }
}
