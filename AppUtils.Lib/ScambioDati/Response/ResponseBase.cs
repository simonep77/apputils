using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Response Base
    /// </summary>
    public abstract class ResponseBase
    {
        /// <summary>
        ///  Lista messaggi 
        /// </summary>
        public List<ResponseMsg> Messages { get; set; } = new List<ResponseMsg>();
 
        /// <summary>
        /// Indica se presente almeno un errore
        /// </summary>
        public bool  HasErrors
        {
            get
            {
                return this.Messages.Any(m => m.Level > ResponseMsg.LEVEL_WARNING);
            }
            set { }
        }

        /// <summary>
        /// Indica se presente almeno un warning
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                return this.Messages.Any(m => m.Level == ResponseMsg.LEVEL_WARNING);

            }
            set { }
        }

        /// <summary>
        /// Informazioni di esecuzione
        /// </summary>
        public ResponseExecInfo ExecInfo { get; set; } = new ResponseExecInfo();





        public void AddError(string text) {
            this.Messages.Add(new ResponseMsg(1 ,text, string.Empty, ResponseMsg.LEVEL_ERROR, string.Empty));
        }

        public void AddError(int code, string text)
        {
            this.Messages.Add(new ResponseMsg(code, text, string.Empty, ResponseMsg.LEVEL_ERROR, string.Empty));
        }

        public void AddInfo(string text)
        {
            this.Messages.Add(new ResponseMsg(1, text, string.Empty, ResponseMsg.LEVEL_INFO, string.Empty));
        }

        public void AddInfo(int code, string text)
        {
            this.Messages.Add(new ResponseMsg(code, text, string.Empty, ResponseMsg.LEVEL_INFO, string.Empty));
        }

        public void AddWarning(string text)
        {
            this.Messages.Add(new ResponseMsg(1, text, string.Empty, ResponseMsg.LEVEL_WARNING, string.Empty));
        }

        public void AddWarning(int code, string text)
        {
            this.Messages.Add(new ResponseMsg(code, text, string.Empty, ResponseMsg.LEVEL_WARNING, string.Empty));
        }



    }



}