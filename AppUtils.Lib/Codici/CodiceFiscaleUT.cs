using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

public class CodiceFiscaleUT
{


    /// <summary>
    ///     ''' Esegue la validazione di un codice fiscale ritornando un oggetto esito
    ///     ''' </summary>
    ///     ''' <param name="codFiscIn"></param>
    ///     ''' <remarks></remarks>
    public static EsitoControllo Controlla(string codFiscIn)
    {
        EsitoControllo oEsito = new EsitoControllo();
        try
        {
            
        }
        catch (Exception ex)
        {
            oEsito.Positivo = false;
            oEsito.EsitoTesto = ex.Message;
            oEsito.EsitoCodice = 1;
        }

        return oEsito;
    }


}
