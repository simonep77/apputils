using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUtils.Lib.Response
{

    /// <summary>
    /// Classe per la gestione di messaggi
    /// </summary>
    public class ResponseMsg
    {
        public const int LEVEL_DEBUG = 0;
        public const int LEVEL_INFO = 1;
        public const int LEVEL_WARNING = 2;
        public const int LEVEL_ERROR = 3;


        public int Code { get; set; }
        public string Text { get; set; }
        public string Stack { get; set; }
        public int Level { get; set; }

        public ResponseMsg()
        { }

        public ResponseMsg(int CodeIn, string TextIn, string StackIn, int LevelIn, string UiFieldIn)
        {
            this.Code = CodeIn;
            this.Text = TextIn;
            this.Stack = StackIn;
            this.Level = LevelIn;
        }




    }
}
