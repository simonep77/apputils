
using System;
using System.Collections.Generic;


namespace AppUtils.Lib.Banca
{

    /// <summary>
    /// ''' Classe contenente funzioni generiche per gestione
    /// ''' codice IBAN
    /// ''' </summary>
    /// ''' <remarks></remarks>
    public class Iban
    {
        public string IbanCompleto => this.InfoIban.IbanCompleto;

        public string IbanPaperFormat => this.InfoIban.IbanPaperFormat;

        public string IbanCodicePaese => this.InfoIban.CodicePaese;

        public string IbanCheckDigit => this.InfoIban.CheckDigit;

        public string IbanCodiceControllo => this.InfoIban.CodiceControllo;

        public string IbanCodiceBanca => this.InfoIban.CodiceBanca;

        public string IbanCodiceSportello => this.InfoIban.CodiceSportello;

        public string IbanNumeroConto => this.InfoIban.NumeroConto;

        /// <summary>
        ///     ''' Indica se l'iban e' italiano
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public bool IsItaliano => this.InfoPaese.IsItaliano;

        /// <summary>
        ///     ''' Indica se l'iban fornito e' straniero
        ///     ''' </summary>
        ///     ''' <value></value>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public bool IsStraniero => this.InfoPaese.IsStraniero;

        /// <summary>
        ///     ''' Indica se l'iban fornito e' della lunghezza prevista
        ///     ''' </summary>
        ///     ''' <value></value>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public bool IsCompleto => this.InfoPaese.IbanLength == this.IbanCompleto.Length;

        /// <summary>
        ///     ''' Informazioni relative al paese dell'iban
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public InfoIbanNazione InfoPaese { get; private set; }

        /// <summary>
        /// Informazioni dell'iban
        /// </summary>
        public IbanInfo InfoIban { get; private set; }




        /// <summary>
        ///     ''' Valorizza Iban partendo da stringa iban completa
        ///     ''' </summary>
        ///     ''' <param name="ibanCompleto"></param>
        ///     ''' <remarks></remarks>
        public Iban(string ibanCompleto)
        {
            ibanCompleto = ibanCompleto.Trim().ToUpper();

            if (ibanCompleto.Length > 1)
            {
                // Se ci sono almeno 2 caratteri prova a determinare la nazione
                if (Iban._INFO_PAESE.TryGetValue(ibanCompleto.Substring(0, 2), out var infoPaese))
                {
                    this.InfoPaese = infoPaese;
                    // Normalizza a n caratteri
                    ibanCompleto = ibanCompleto.PadRight(this.InfoPaese.IbanLength);

                    // Decompone iban in base alle regole del paese
                    this.InfoIban = this.InfoPaese.DecomponiIban(ibanCompleto);
                }
                else
                    // Sconosciuto
                    throw new ArgumentException(string.Format("Il Codice Paese dell'IBAN fornito ('{0}') non risulta appartenere ad alcun paese conosciuto.", ibanCompleto));
            }
            else
            {
                // Ne imposta comunque uno vuoto
                this.InfoIban = new IbanInfo();

                // Imposta Info paese su Italia
                this.InfoPaese = _INFO_PAESE["IT"];
            }
        }

        /// <summary>
        ///     ''' Valorizza iban partendo da forma decomposta
        ///     ''' </summary>
        ///     ''' <param name="codicePaese"></param>
        ///     ''' <param name="checkDigit"></param>
        ///     ''' <param name="codiceControllo"></param>
        ///     ''' <param name="codiceBanca"></param>
        ///     ''' <param name="codiceSportello"></param>
        ///     ''' <param name="numeroConto"></param>
        ///     ''' <remarks></remarks>
        public Iban(string codicePaese, string checkDigit, string codiceControllo, string codiceBanca, string codiceSportello, string numeroConto) : this(string.Concat(codicePaese, checkDigit, codiceControllo, codiceBanca, codiceSportello, numeroConto))
        {
        }




        /// <summary>
        ///     ''' Esegue validazione formale in base al paese dell'iban 
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string ValidateFormalNoException()
        {
            string sRet = string.Empty;
            try
            {
                this.ValidateFormal();
            }
            catch (Exception ex)
            {
                sRet = ex.Message;
            }

            return sRet;
        }



        /// <summary>
        ///     ''' Esegue validazione formale in base al paese dell'iban
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        public void ValidateFormal()
        {

            // Controllo lunghezze
            if (this.InfoPaese.IbanLength != this.IbanCompleto.Trim().Length)
                throw new ArgumentException($"L'iban fornito '{this.IbanCompleto.Trim()}' ha lunghezza errata (attesa {this.InfoPaese.IbanLength})");

            if (this.InfoPaese.CodiceControlloLength != this.IbanCodiceControllo.Trim().Length)
                throw new ArgumentException($"Il codice di controllo '{this.IbanCodiceControllo.Trim()}' ha lunghezza errata (attesa {this.InfoPaese.CodiceControlloLength})");

            if (this.InfoPaese.CodiceBancaLength != this.IbanCodiceBanca.Trim().Length)
                throw new ArgumentException($"Il codice banca (ABI) '{this.IbanCodiceBanca.Trim()}' ha lunghezza errata (attesa {this.InfoPaese.CodiceBancaLength})");

            if (this.InfoPaese.CodiceSportelloLength != this.IbanCodiceSportello.Trim().Length)
                throw new ArgumentException($"Il codice sportello (CAB) '{this.IbanCodiceSportello.Trim()}' ha lunghezza errata (attesa {this.InfoPaese.CodiceSportelloLength})");

            if (this.InfoPaese.NumeroContoLength != this.IbanNumeroConto.Trim().Length)
                throw new ArgumentException($"Il numero conto '{this.IbanNumeroConto.Trim()}' ha lunghezza errata (attesa {this.InfoPaese.NumeroContoLength})");

            // Se italia verifica CIN
            if (!this.IsStraniero)
            {
                string sCinCalc = calcolaCinIT(string.Concat(this.IbanCodiceBanca, this.IbanCodiceSportello, this.IbanNumeroConto));
                if (this.IbanCodiceControllo != sCinCalc)
                    throw new ArgumentException($"Il codice di controllo (CIN) '{this.IbanCodiceControllo}' non coincide con quello atteso {sCinCalc}");
            }

            // Valida check digit
            if (!this.CheckIbanCheckDigit(this.InfoIban.IbanCompleto))
                throw new ArgumentException($"Il check digit '{this.IbanCheckDigit}' non coincide con quello atteso");
        }

        /// <summary>
        ///     ''' Ritorna l'iban specificato e, se incompleto, riempito alla lunghezza preivista con il carattere fornito
        ///     ''' </summary>
        ///     ''' <param name="c"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public string GetIbanFilledWith(char c)
        {
            return this.InfoIban.IbanCompleto.PadRight(this.InfoPaese.IbanLength, c);
        }




        /// <summary>
        ///     ''' Verifica la validita' del check digit
        ///     ''' </summary>
        ///     ''' <param name="Iban"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private bool CheckIbanCheckDigit(string Iban)
        {

            // Si convertono le lettere in numeri in base alle regole dettate dagli organi di competenza
            // Es.: 15052967397012120000033409182959 Le lettere F,C,C,I e T sono state sostituite (F=15;C=12;I=18;T=29)
            Iban = ConvertiIBAN(Iban);

            // Calcolo del Modulo 97-10 (Modulo 97 in base 10) dell'IBAN
            return (Modulo97(Iban) == 1);
        }




        /// <summary>
        ///     ''' Contiene le informazioni per gli iban dei vari paesi
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private static Dictionary<string, InfoIbanNazione> _INFO_PAESE = new Dictionary<string, InfoIbanNazione>(30);

        /// <summary>
        ///     ''' Costruttore statico che inizializza le informazioni sui paesi
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        static Iban()
        {
            Iban.CaricaCodiciPaese();
        }

        /// <summary>
        ///     ''' Esegue caricamento codici paese
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private static void CaricaCodiciPaese()
        {
            // Se già caricati esce
            if (_INFO_PAESE.Count > 0)
                return;

            // === CARICA ===

            _INFO_PAESE.Add("AL", new InfoIbanNazione("AL", "ALBANIA", "AL00BBBBBBBBCCCCCCCCCCCCCCCC", true, "ALL"));
            _INFO_PAESE.Add("AD", new InfoIbanNazione("AD", "ANDORRA", "AD00BBBBSSSSCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("SA", new InfoIbanNazione("SA", "ARABIA SAUDITA", "SA00BBCCCCCCCCCCCCCCCCCC", false, "SAR"));
            _INFO_PAESE.Add("AT", new InfoIbanNazione("AT", "AUSTRIA", "AT00BBBBBCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("AZ", new InfoIbanNazione("AZ", "AZERBAIGIAN", "AZ00BBBBCCCCCCCCCCCCCCCCCCCC", false, "AZN"));
            _INFO_PAESE.Add("BH", new InfoIbanNazione("BH", "BAHREIN", "BH00BBBBCCCCCCCCCCCCCC", false, "BHD"));
            _INFO_PAESE.Add("BE", new InfoIbanNazione("BE", "BELGIO", "BE00BBBCCCCCCCKK", true, "EUR"));
            _INFO_PAESE.Add("BA", new InfoIbanNazione("BA", "BOSNIA ERZEGOVINA", "BA00BBBSSSCCCCCCCoKK", false, "BAM"));
            _INFO_PAESE.Add("BG", new InfoIbanNazione("BG", "BULGARIA", "BG00BBBBSSSSCCCCCCCCCC",true, "BGN"));
            _INFO_PAESE.Add("CR", new InfoIbanNazione("CR", "COSTARICA", "CR00KBBBCCCCCCCCCCCCCC", false, "CRC"));
            _INFO_PAESE.Add("HR", new InfoIbanNazione("HR", "CROAZIA", "HR00BBBBBBBCCCCCCCCCC", true, "HRK"));
            _INFO_PAESE.Add("CY", new InfoIbanNazione("CY", "CIPRO", "CY00BBBSSSSSCCCCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("CZ", new InfoIbanNazione("CZ", "REPUBBLICA CECA", "CZ00BBBBSSSSSSCCCCCCCCCC", true, "CZK"));
            _INFO_PAESE.Add("DK", new InfoIbanNazione("DK", "DANIMARCA", "DK00BBBBCCCCCCCCCC", true, "DKK"));
            _INFO_PAESE.Add("EE", new InfoIbanNazione("EE", "ESTONIA", "EE00BBSSCCCCCCCCCCCK", true, "EUR"));
            _INFO_PAESE.Add("FO", new InfoIbanNazione("FO", "ISOLE FAROE", "FO00CCCCCCCCCCCCCC", false, "DKK"));
            _INFO_PAESE.Add("FI", new InfoIbanNazione("FI", "FINLANDIA", "FI00BBBBBBCCCCCCCK", true, "EUR"));
            _INFO_PAESE.Add("FR", new InfoIbanNazione("FR", "FRANCIA", "FR00BBBBBSSSSSCCCCCCCCCCCKK", true, "EUR"));
            _INFO_PAESE.Add("DE", new InfoIbanNazione("DE", "GERMANIA", "DE00BBBBBBBBCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("GE", new InfoIbanNazione("GE", "GEORGIA", "GE00BBCCCCCCCCCCCCCCCC", false, "GEL"));
            _INFO_PAESE.Add("GI", new InfoIbanNazione("GI", "GIBILTERRA", "GI00BBBBCCCCCCCCCCCCCCC", true, "GIP"));
            _INFO_PAESE.Add("GR", new InfoIbanNazione("GR", "GRECIA", "GR00BBBBBBBCCCCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("GL", new InfoIbanNazione("GL", "GROENLANDIA", "GL00BBBBCCCCCCCCCC", false, "DKK"));
            _INFO_PAESE.Add("HU", new InfoIbanNazione("HU", "UNGHERIA", "HU00BBBBBBBCCCCCCCCCCCCCCCCC", true, "HUF"));
            _INFO_PAESE.Add("IS", new InfoIbanNazione("IS", "ISLANDA", "IS00BBBBSSCCCCCCCCCCCCCCCC", true, "ISK"));
            _INFO_PAESE.Add("IE", new InfoIbanNazione("IE", "IRLANDA", "IE00BBBBSSSSSSCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("IL", new InfoIbanNazione("IL", "ISRAELE", "IL00BBBSSSCCCCCCCCCCCCC", false, "ILS"));
            _INFO_PAESE.Add("IT", new InfoIbanNazione("IT", "ITALIA", "IT00KBBBBBSSSSSCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("LV", new InfoIbanNazione("LV", "LETTONIA", "LV00BBBBCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("LB", new InfoIbanNazione("LB", "LIBANO", "LB00BBBBCCCCCCCCCCCCCCCCCCCC", false, "LBP"));
            _INFO_PAESE.Add("LI", new InfoIbanNazione("LI", "LIECHTENSTEIN", "LI00BBBBBCCCCCCCCCCCC", true, "CHF"));
            _INFO_PAESE.Add("LT", new InfoIbanNazione("LT", "LITUANIA", "LT00BBBBBCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("LU", new InfoIbanNazione("LU", "LUSSEMBURGO", "LU00BBBCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("KZ", new InfoIbanNazione("KZ", "KAZAKISTAN", "KZ00BBBCCCCCCCCCCCCC", false, "KZT"));
            _INFO_PAESE.Add("KW", new InfoIbanNazione("KW", "KUWAIT", "KW00BBBBCCCCCCCCCCCCCCCCCCCCCC", false, "KWD"));
            _INFO_PAESE.Add("XK", new InfoIbanNazione("XK", "KOSOVO", "XK00BBSSCCCCCCCCCCKK", false, "EUR"));
            _INFO_PAESE.Add("MK", new InfoIbanNazione("MK", "MACEDONIA", "MK00BBBCCCCCCCCCCKK", false, "MKD"));
            _INFO_PAESE.Add("MT", new InfoIbanNazione("MT", "MALTA", "MT00BBBBSSSSSCCCCCCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("MR", new InfoIbanNazione("MR", "MAURITANIA", "MR00BBBBBSSSSSCCCCCCCCCCCKK", false, "MRO"));
            _INFO_PAESE.Add("MU", new InfoIbanNazione("MU", "MAURITIUS", "MU00BBBBBBSSCCCCCCCCCCCCCCCCCC", false, "MUR"));
            _INFO_PAESE.Add("MD", new InfoIbanNazione("MD", "MOLDAVIA", "MD00BBCCCCCCCCCCCCCCCCCC", false, "MDL"));
            _INFO_PAESE.Add("MC", new InfoIbanNazione("MC", "MONACO", "MC00BBBBBSSSSSCCCCCCCCCCCKK", true, "EUR"));
            _INFO_PAESE.Add("ME", new InfoIbanNazione("ME", "MONTENEGRO", "ME00BBBCCCCCCCCCCCCCKK", false, "EUR"));
            _INFO_PAESE.Add("NL", new InfoIbanNazione("NL", "OLANDA", "NL00BBBBCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("NO", new InfoIbanNazione("NO", "NORVEGIA", "NO00BBBBCCCCCCK", true, "NOK"));
            _INFO_PAESE.Add("PL", new InfoIbanNazione("PL", "POLONIA", "PL00BBBSSSSKCCCCCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("PT", new InfoIbanNazione("PT", "PORTOGALLO", "PT00BBBBBBBBCCCCCCCCCCCKK", true, "EUR"));
            _INFO_PAESE.Add("DO", new InfoIbanNazione("DO", "REPUBBLICA DOMINICANA", "DO00BBBBCCCCCCCCCCCCCCCCCCCC", false, "DOP"));
            _INFO_PAESE.Add("RO", new InfoIbanNazione("RO", "ROMANIA", "RO00BBBBCCCCCCCCCCCCCCCC", true, "RON"));
            _INFO_PAESE.Add("SM", new InfoIbanNazione("SM", "SAN MARINO", "SM00KBBBBBSSSSSCCCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("RS", new InfoIbanNazione("RS", "SERBIA", "RS00BBBCCCCCCCCCCCCCKK", false, "RSD"));
            _INFO_PAESE.Add("SK", new InfoIbanNazione("SK", "SLOVACCHIA", "SK00BBBBSSSSSSCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("SK", new InfoIbanNazione("SI", "SLOVENIA", "SI00BBSSSCCCCCCCCKK", true, "EUR"));
            _INFO_PAESE.Add("ES", new InfoIbanNazione("ES", "SPAGNA", "ES00BBBBSSSSKKCCCCCCCCCC", true, "EUR"));
            _INFO_PAESE.Add("SE", new InfoIbanNazione("SE", "SVEZIA", "SE00BBBBCCCCCCCCCCCCCCCC", true, "SEK"));
            _INFO_PAESE.Add("CH", new InfoIbanNazione("CH", "SVIZZERA", "CH00BBBBBCCCCCCCCCCCC", true, "CHF"));
            _INFO_PAESE.Add("TR", new InfoIbanNazione("TR", "TURCHIA", "TR00BBBBBKCCCCCCCCCCCCCCCC", false, "TRY"));
            _INFO_PAESE.Add("TN", new InfoIbanNazione("TN", "TUNISIA", "TN00BBBBBCCCCCCCCCCCCCCC", false, "TND"));
            _INFO_PAESE.Add("GB", new InfoIbanNazione("GB", "REGNO UNITO", "GB00BBBBSSSSSSCCCCCCCC", true, "GBP"));
        }

        /// <summary>
        ///     ''' Dato un iban completo (es IT01A0123405678000000000001)
        ///     ''' ritorna la corrispondente versione a quartetti (es IT01 A012 3405 6780 0000 0000 001)
        ///     ''' </summary>
        ///     ''' <param name="ibanCompleto"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static string ToPaperFormat(string ibanCompleto)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 1; i <= ibanCompleto.Length; i++)
            {
                sb.Append(ibanCompleto[i - 1]);

                // Se raggiunti quattro caratteri imposta spazio
                if ((i % 4) == 0)
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        ///     ''' Dato un iban in formato Paper, ritorna il corrispondente formato compatto
        ///     ''' (es IT01 A012 3405 6780 0000 0000 001 => IT01A0123405678000000000001)
        ///     ''' </summary>
        ///     ''' <param name="ibanPaperFormat"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static string FromPaperFormat(string ibanPaperFormat)
        {
            return ibanPaperFormat.Replace(" ", string.Empty);
        }


        /// <summary>
        ///     ''' Calcola il cin in base all'algoritmo italiano
        ///     ''' </summary>
        ///     ''' <param name="abi_cab_cc"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static string calcolaCinIT(string abi_cab_cc)
        {
            // costanti e variabili per calcolo pesi
            const string numeri = "0123456789";
            const string lettere = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-. ";
            const int DIVISORE = 26;
            int[] listaPari = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 };
            int[] listaDispari = new[] { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23, 27, 28, 26 };

            // codice normalizzato
            string codice = abi_cab_cc;

            // calcolo valori caratteri
            int somma = 0;
            char[] c = codice.ToUpper().ToCharArray();
            for (int k = 0; k <= codice.Length - 1; k++)
            {
                int i = numeri.IndexOf(c[k]);
                if (i < 0)
                    i = lettere.IndexOf(c[k]);

                // se ci sono caratteri errati usciamo con un valore
                // impossibile da trovare sul cin
                if (i < 0)
                    return Environment.NewLine;

                if ((k % 2) == 0)
                    // valore dispari
                    somma += listaDispari[i];
                else
                    // valore pari
                    somma += listaPari[i];
            }

            return lettere.Substring(somma % DIVISORE, 1);
        }

        /// <summary>
        ///     ''' Dato un IBAN converte le Letttere in numeri in base alla tabella di conversione secondo cui: A=10; B=11; C=12; D=13 .....
        ///     ''' </summary>
        ///     ''' <param name="IBAN"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static string ConvertiIBAN(string IBAN)
        {
            const string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Si spostano i primi 4 caratteri dell'IBAN alla fine (es.: F0529673970CC0000033409IT59)
            IBAN = string.Format("{0}{1}", IBAN.Substring(4), IBAN.Substring(0, 4));

            System.Text.StringBuilder ibanConvertito = new System.Text.StringBuilder();
            string appo;

            foreach (char c in IBAN)
            {
                if (alfabeto.IndexOf(c) != -1)
                    appo = (alfabeto.IndexOf(c) + 10).ToString();
                else
                    appo = c.ToString();
                ibanConvertito.Append(appo);
            }
            return ibanConvertito.ToString();
        }

        /// <summary>
        ///     ''' Restituisce il MOD 97-10 (Modulo 97 in base 10) di un numero passato come stringa
        ///     ''' </summary>
        ///     ''' <param name="numero"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static int Modulo97(string numero)
        {
            // Divisione Modulo 97 ciclica di interi con massimo 18 cifre 
            // ESEMPIO:15052967397012120000033409182959 relativo al codice IBAN IT59 F052 9673 970C C000 0033 409
            const int LUNGHEZZA_BLOCCHI_CALCOLO = 16;
            const int MOD_DIVISORE = 97;
            // Si prendono le prime 16 cifre dell'IBAN e si fa il modulo 97: 1505296739701212 MOD 97 = 21
            // Il 21 si 'concatena' all'inizio delle successive cifre dell'IBAN in modo che al massimo le cifre siano 18  
            // Visto che il Resto non può superare 96 (Modulo 97) che ha 2 cifre, si prendono al massimo le successive 16 cifre e si fa il modulo 97:  210000033409182959 MOD 97 = 1
            Int64 dividendo = 0;
            int resto = 0;
            int i = 0;
            while (i < numero.Length)
            {
                // Per concatenare il resto prima delle successive cifre dell'IBAN si segue la seguente regola:
                // esempio: devo mettere il numero 37 prima del numero 237 e costruire il numero 37237: (37*10^3)+237 = 37237
                // L'esponente della base 10 è pari al numero di cifre del numero 237

                if (numero.Length >= i + LUNGHEZZA_BLOCCHI_CALCOLO)
                    dividendo = (resto * Convert.ToInt64(Math.Pow(10, LUNGHEZZA_BLOCCHI_CALCOLO))) + Convert.ToInt64(numero.Substring(i, LUNGHEZZA_BLOCCHI_CALCOLO));
                else
                    dividendo = (resto * Convert.ToInt64(Math.Pow(10, (numero.Length - i)))) + Convert.ToInt64(numero.Substring(i));
                i += LUNGHEZZA_BLOCCHI_CALCOLO;

                resto = (int)(dividendo % MOD_DIVISORE);
            }

            return resto;
        }
    }

}