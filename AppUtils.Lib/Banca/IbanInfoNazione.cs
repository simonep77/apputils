/// <summary>
/// ''' Contiene informazioni sulla tipologia utilizzata in un paese
/// ''' </summary>
/// ''' <remarks></remarks>
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

namespace AppUtils.Lib.Banca
{

    public class InfoIbanNazione
    {

        private static Dictionary<string, string> _Nazioni;

        public string IbanCodifica { get; private set; }
        public string CodiceValuta { get; private set; }
        public bool IsAreaSepa { get; private set; }

        public string CodiceIsoNazione { get; private set; }

        public string DescNazione { get; private set; }

        public int IbanLength => this.IbanCodifica.Length;

        public int CodiceControlloLength { get; private set; }

        public int CodiceBancaLength { get; private set; }

        public int CodiceSportelloLength { get; private set; }

        public int NumeroContoLength { get; private set; }

        public bool IsItaliano => this.CodiceIsoNazione == "IT";

        public bool IsStraniero => !this.IsItaliano;



        /// <summary>
        ///     ''' Imposta informazioni iban paese.
        ///     ''' La codifica attesa è:
        ///     ''' IT00KBBBBBSSSSSCCCCCCCCCCCC dove
        ///     ''' - IT è il cod paese
        ///     ''' - 00 indica il checkdigit IBAN
        ///     ''' - K è un codice di controllo (italia CIN)
        ///     ''' - B è il codice della banca (italia ABI)
        ///     ''' - S è il codice dello sportello (italia CAB)
        ///     ''' - C è il numero di conto
        ///     ''' </summary>
        ///     ''' <param name="isoNazione"></param>
        ///     ''' <param name="descNazione"></param>
        ///     ''' <param name="codificaIban"></param>
        public InfoIbanNazione(string isoNazione, string descNazione, string codificaIban, bool areaSepa, string codvaluta)
        {
            this.IbanCodifica = codificaIban.ToUpper().Trim().PadRight(4);

            // Imposta codice paese
            this.CodiceIsoNazione = isoNazione;
            this.DescNazione = descNazione;
            this.IsAreaSepa = areaSepa;
            this.CodiceValuta = codvaluta;

            // Esegue conteggio
            for (int index = 4; index <= codificaIban.Length - 1; index++)
            {
                switch (codificaIban[index])
                {
                    case 'B':
                        {
                            this.CodiceBancaLength++;
                            break;
                        }

                    case 'S':
                        {
                            this.CodiceSportelloLength++;
                            break;
                        }

                    case 'K':
                        {
                            this.CodiceControlloLength++;
                            break;
                        }

                    case 'C':
                        {
                            this.NumeroContoLength++;
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }


        /// <summary>
        ///     ''' Dato un Iban ritorna una decomposizione
        ///     ''' </summary>
        ///     ''' <param name="iban"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public IbanInfo DecomponiIban(string iban)
        {
            IbanInfo oIbanDec = new IbanInfo();

            System.Text.StringBuilder sbBanca = new System.Text.StringBuilder();
            System.Text.StringBuilder sbSportello = new System.Text.StringBuilder();
            System.Text.StringBuilder sbNumeroCC = new System.Text.StringBuilder();
            System.Text.StringBuilder sbCodiceControllo = new System.Text.StringBuilder();
            int iLenMaxIban = Math.Min(iban.Length, this.IbanLength);

            // Esegue decomposizione
            for (int index = 4; index <= iLenMaxIban - 1; index++)
            {
                switch (this.IbanCodifica[index])
                {
                    case 'B':
                        {
                            sbBanca.Append(iban[index]);
                            break;
                        }

                    case 'S':
                        {
                            sbSportello.Append(iban[index]);
                            break;
                        }

                    case 'K':
                        {
                            sbCodiceControllo.Append(iban[index]);
                            break;
                        }

                    case 'C':
                        {
                            sbNumeroCC.Append(iban[index]);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            // Imposta campi
            oIbanDec.IbanCompleto = iban;
            oIbanDec.CodicePaese = this.CodiceIsoNazione;
            oIbanDec.CheckDigit = iban.Substring(2, 2);
            oIbanDec.CodiceControllo = sbCodiceControllo.ToString();
            oIbanDec.CodiceBanca = sbBanca.ToString();
            oIbanDec.CodiceSportello = sbSportello.ToString();
            oIbanDec.NumeroConto = sbNumeroCC.ToString();
            oIbanDec.IbanPaperFormat = Iban.ToPaperFormat(iban);

            return oIbanDec;
        }


        /// <summary>
        ///     ''' Dati alcuni valori standard che compongono un iban ritorna le informazioni iban 
        ///     ''' secondo la codifica del paese
        ///     ''' </summary>
        ///     ''' <param name="checkDigit"></param>
        ///     ''' <param name="codiceControllo"></param>
        ///     ''' <param name="codiceBanca"></param>
        ///     ''' <param name="codiceSportello"></param>
        ///     ''' <param name="numeroConto"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public IbanInfo RicomponiIban(string checkDigit, string codiceControllo, string codiceBanca, string codiceSportello, string numeroConto)
        {
            IbanInfo oIbanInfo = new IbanInfo();
            System.Text.StringBuilder sbIban = new System.Text.StringBuilder(this.CodiceIsoNazione, this.IbanLength);
            char cCurrChar = ' ';
            int iCurrIndex = 0;

            // Imposta dati base
            checkDigit = checkDigit.PadRight(2);
            codiceControllo = codiceControllo.PadRight(this.CodiceControlloLength);
            codiceBanca = codiceBanca.PadRight(this.CodiceBancaLength);
            codiceSportello = codiceSportello.PadRight(this.CodiceSportelloLength);
            numeroConto = numeroConto.PadRight(this.NumeroContoLength);

            // Esegue decomposizione
            for (int index = 2; index <= this.IbanCodifica.Length - 1; index++)
            {
                if (cCurrChar != this.IbanCodifica[index])
                {
                    iCurrIndex = 0;
                    cCurrChar = this.IbanCodifica[index];
                }

                switch (cCurrChar)
                {
                    case '0':
                        {
                            sbIban.Append(checkDigit[iCurrIndex]);
                            break;
                        }

                    case 'B':
                        {
                            sbIban.Append(codiceBanca[iCurrIndex]);
                            break;
                        }

                    case 'S':
                        {
                            sbIban.Append(codiceSportello[iCurrIndex]);
                            break;
                        }

                    case 'K':
                        {
                            sbIban.Append(codiceControllo[iCurrIndex]);
                            break;
                        }

                    case 'C':
                        {
                            sbIban.Append(numeroConto[iCurrIndex]);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                // Incrementa indice interno
                iCurrIndex += 1;
            }

            oIbanInfo.CodicePaese = this.CodiceIsoNazione;
            oIbanInfo.CheckDigit = checkDigit;
            oIbanInfo.CodiceControllo = codiceControllo;
            oIbanInfo.CodiceBanca = codiceBanca;
            oIbanInfo.CodiceSportello = codiceSportello;
            oIbanInfo.NumeroConto = numeroConto;
            oIbanInfo.IbanCompleto = sbIban.ToString();
            oIbanInfo.IbanPaperFormat = Iban.ToPaperFormat(oIbanInfo.IbanCompleto);

            return oIbanInfo;
        }
    }

}