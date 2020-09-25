using System.Text.RegularExpressions;

namespace AppUtils.Lib.Web
{

    /// <summary>

    /// ''' Utilità per indirizzi IP

    /// ''' </summary>

    /// ''' <remarks></remarks>
    public class Ip
    {

        /// <summary>
        ///     ''' Indica se l'ipAddressIn fa parte della famiglia di ipAddressMask
        ///     ''' </summary>
        ///     ''' <param name="ipAddressIn"></param>
        ///     ''' <param name="ipAddressMask"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static bool Check(string ipAddressIn, string ipAddressMask)
        {
            string strExpression;
            // Espressione regolare per un indirizzo Ip valido con numeri da 0-255
            // \b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b

            strExpression = ipAddressMask.Replace(".", @"\.");
            strExpression = strExpression.Replace("*", "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
            strExpression = string.Format("^{0}$", strExpression);

            // test effettivo della validità dell'indirizzo
            Regex regEx = new Regex(strExpression, RegexOptions.IgnoreCase);
            return regEx.IsMatch(ipAddressIn);
        }
    }

}
