using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppUtils.Lib.Common
{
    /// <summary>
    /// variabili di utilizzo comune
    /// </summary>
    public static class LibContext
    {

        public static CultureInfo Culture_IT { get; } = new CultureInfo("it-IT");
        public static CultureInfo Culture_US { get; } = new CultureInfo("en-US");


    }
}
