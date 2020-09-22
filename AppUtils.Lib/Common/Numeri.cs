using AppUtils.Lib.Common;
using System;

/// <summary>
/// Funzioni relative ai numeri
/// </summary>
public static class Numeri
{
    /// <summary>
    /// Routine privata di parsing
    /// </summary>
    /// <param name="decimalStr"></param>
    /// <param name="culture"></param>
    /// <param name="decimalSep"></param>
    /// <param name="thousandSep"></param>
    /// <returns></returns>
    private static decimal decimalFromString(string decimalStr, System.Globalization.CultureInfo culture, string decimalSep, string thousandSep)
    {
        decimal decimalOut = 0M;
        culture.NumberFormat.CurrencyDecimalSeparator = decimalSep;
        culture.NumberFormat.CurrencyGroupSeparator = thousandSep;
        if (!decimal.TryParse(decimalStr, System.Globalization.NumberStyles.Currency, culture, out decimalOut))
            throw new ApplicationException($"La cifra fornita '{decimalStr}' non e' valida. Non rispetta la formattazione attesa ( tipo nnn,nnn ).");
        return decimalOut;
    }


    /// <summary>
    ///  Converte una stringa con formato n[,n] in numero decimale
    ///  </summary>
    ///  <param name="decimalStr"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static decimal DecimalFromStringIT(string decimalStr)
    {
        return Numeri.decimalFromString(decimalStr, LibContext.Culture_IT, ",", ".");
    }

    /// <summary>
    ///  Ritorna decimale partendo da una formattazione di tipo universale
    ///  es. 1,000.00
    ///  </summary>
    ///  <param name="decimalStr"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static decimal DecimalFromStringUNIV(string decimalStr)
    {
        return Numeri.decimalFromString(decimalStr, LibContext.Culture_US, ".", ",");
    }

    /// <summary>
    ///  Ritorna una stringa italiana interpretazione numeri decimali
    ///  es. 1.000,00 => 1000,00
    ///  </summary>
    ///  <param name="value"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static string DecimalToStringIT(decimal value)
    {
        return value.ToString("F", LibContext.Culture_IT);
    }

    /// <summary>
    ///  Ritorna una stringa universale di interpretazione numeri decimali
    ///  es. 1.000,00 => 1000.00
    ///  </summary>
    ///  <param name="value"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static string DecimalToStringUNIV(decimal value)
    {
        return value.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
    }
}
