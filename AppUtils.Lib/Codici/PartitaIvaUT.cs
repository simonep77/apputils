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

namespace AppUtils.Lib.Codici
{

    public class PartitaIvaUT
    {


        /// <summary>
        ///  Esegue la validazione di una partita IVA ritornando un oggetto esito
        ///  </summary>
        ///  <param name="partitaIvaIn"></param>
        ///  <remarks></remarks>
        public static EsitoControllo Controlla(string partitaIvaIn)
        {
            EsitoControllo oEsito = new EsitoControllo();
            try
            {
                ControllaCheck(partitaIvaIn);
            }
            catch (Exception ex)
            {
                oEsito.Positivo = false;
                oEsito.EsitoTesto = ex.Message;
                oEsito.EsitoCodice = 1;
            }

            return oEsito;
        }


        /// <summary>
        ///  Esegue la validazione di una partita IVA lanciando eccezione in caso di errore
        ///  </summary>
        ///  <param name="value"></param>
        ///  <remarks></remarks>
        public static void ControllaCheck(string value)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\d]{11}$", System.Text.RegularExpressions.RegexOptions.Compiled))
                throw new ArgumentException("La partita IVA deve essere di 11 caratteri numerici");

            if (value == "00000000000")
                throw new ArgumentException("Partita IVA non valida");

            string cCtrlCalc = CalcolaCarattereControllo(value);
            string cCtrlPres = value[10].ToString();
            if (cCtrlCalc != cCtrlPres)
                throw new ArgumentException($"Il carattere di controllo atteso della partita IVA '{cCtrlCalc}' non coincide con quello presente '{cCtrlPres}'");
        }


        /// <summary>
        ///  Calcola il carattere di controllo di una partita iva
        ///  </summary>
        ///  <param name="value"></param>
        ///  <returns></returns>
        ///  <remarks></remarks>
        public static string CalcolaCarattereControllo(string value)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\d]{10,}$", System.Text.RegularExpressions.RegexOptions.Compiled))
                throw new ArgumentException("Il calcolo del carattere di controllo della partita IVA richiede almeno 10 caratteri numerici (i restanti verranno ignorati)");

            int iTmp;

            value = value.Substring(0, 10);
            int iCtrl = 0;

            for (int index = 0; index <= value.Length - 1; index++)
            {
                iTmp = Convert.ToInt32(value[index].ToString());

                if (((index + 1) % 2) == 0)
                {
                    iTmp *= 2;

                    if (iTmp > 9)
                        iTmp -= 9;
                }
                iCtrl += iTmp;
            }

            iCtrl = (iCtrl % 10);


            if (iCtrl == 0)
                return 0.ToString();
            else
                return (10 - iCtrl).ToString();
        }
    }

}