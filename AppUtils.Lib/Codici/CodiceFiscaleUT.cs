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
    private static readonly string[] _ArrCharMesi = new[] { "A", "B", "C", "D", "E", "H", "L", "M", "P", "R", "S", "T" };
    private static readonly int[] _ArrNumPari = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    private static readonly int[] _ArrNumDispari = new[] { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };
    private const int I_CHAR_A = 65;

    public class Comune
    {
        public string Nome;
        public string SiglaProvincia;
    }

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
            ControllaCheck(codFiscIn);
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
    ///     ''' Esegue la validazione di un codice fiscale con i rispettivi dati in input
    ///     ''' </summary>
    ///     ''' <param name="cfIn"></param>
    ///     ''' <param name="nomeIn"></param>
    ///     ''' <param name="cognomeIn"></param>
    ///     ''' <param name="dataNascitaIn"></param>
    ///     ''' <param name="sessoIn"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static EsitoControllo Controlla(string cfIn, string cognomeIn, string nomeIn, DateTime dataNascitaIn, char sessoIn)
    {
        EsitoControllo oEsito = new EsitoControllo();
        try
        {
            var icheck = checkCodiceFiscaleNoComuni(cfIn, cognomeIn, nomeIn, dataNascitaIn.Year.ToString(), dataNascitaIn.Month.ToString().PadLeft(2, '0'), dataNascitaIn.Day.ToString().PadLeft(2, '0'), sessoIn.ToString());

            switch (icheck)
            {
                case 0:
                    {
                        break;
                    }

                case -1:
                    {
                        throw new ApplicationException("Dati non validi (numero caratteri<>16)");
                    }

                case -2:
                    {
                        throw new ApplicationException("CHECK DIGIT non corrisponde");
                    }

                case -3:
                    {
                        throw new ApplicationException("parte COGNOME non corrisponde");
                    }

                case -4:
                    {
                        throw new ApplicationException("parte NOME non corrisponde");
                    }

                case -5:
                    {
                        throw new ApplicationException("parte DATA NASCITA E SESSO non corrisponde");
                    }

                default:
                    {
                        throw new ApplicationException("errore nel calcolo");
                    }
            }
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
    ///     ''' Esegue la validazione di un codice fiscale lanciando eccezione in caso di errore
    ///     ''' </summary>
    ///     ''' <param name="codFiscIn"></param>
    ///     ''' <remarks></remarks>
    public static void ControllaCheck(string codFiscIn)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(codFiscIn, "^[a-zA-Z0-9]{16}$", System.Text.RegularExpressions.RegexOptions.Compiled))
            throw new ArgumentException("La partita IVA deve essere di 11 caratteri alfanumerici [a-z, A-Z, 0-9]");

        codFiscIn = codFiscIn.ToUpper();

        string cCtrlCalc = CalcolaCarattereControllo(codFiscIn);
        string cCtrlPres = codFiscIn[15].ToString();
        if (cCtrlCalc != cCtrlPres)
            throw new ArgumentException(string.Format("Il carattere di controllo atteso del codice fiscale '{0}' non coincide con quello presente '{1}'", cCtrlCalc, cCtrlPres));
    }

    /// <summary>
    ///     ''' Calcola il carattere di controllo di una partita iva
    ///     ''' </summary>
    ///     ''' <param name="value"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static string CalcolaCarattereControllo(string value)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-zA-Z0-9]{15,}$", System.Text.RegularExpressions.RegexOptions.Compiled))
            throw new ArgumentException("Il calcolo del carattere di controllo delcodice fiscale richiede almeno 15 caratteri alfanumerici [a-z, A-Z, 0-9]");

        int iTmp;

        value = value.Substring(0, 15).ToUpper();

        int iCtrl = 0;

        for (int index = 0; index <= value.Length - 1; index++)
        {
            char ccChar = value[index];

            // Indicizza carattere
            if (char.IsNumber(ccChar))
                iTmp = Convert.ToInt32(ccChar.ToString());
            else
                iTmp = Convert.ToInt32(ccChar) - I_CHAR_A;

            if (((index + 1) % 2) == 0)
                // Paro
                iTmp = _ArrNumPari[iTmp];
            else
                // Disparo
                iTmp = _ArrNumDispari[iTmp];

            // Somma
            iCtrl += iTmp;
        }

        iCtrl = (iCtrl % 26);

        // Ritorna il valore ASCII
        return Convert.ToChar(I_CHAR_A + iCtrl).ToString();
    }


    private static string decodeChar(string s, params int[] vPos)
    {
        // decodifica dei caratteri per la gestione delle omocodie...
        int iPos;
        decodeChar = s;
        for (iPos = 0; iPos <= Information.UBound(vPos); iPos++)
        {
            if (!Information.IsNumeric(Strings.Mid(s, vPos[iPos], 1)))
            {
                switch (Strings.LCase(Strings.Mid(s, vPos[iPos], 1)))
                {
                    case "l":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "0" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "m":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "1" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "n":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "2" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "p":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "3" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "q":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "4" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "r":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "5" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "s":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "6" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "t":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "7" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "u":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "8" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }

                    case "v":
                        {
                            decodeChar = Strings.Left(s, vPos[iPos] - 1) + "9" + Strings.Right(s, Strings.Len(s) - vPos[iPos]);
                            break;
                        }
                }
            }
        }
    }


    private static int checkCodiceFiscaleNoComuni(string codice_c_f, string cognome, string nome, string anno, string mese, string giorno, string sesso)
    {

        // *******************************************************************************
        // *                                                                             *
        // *                         Descrizione del algoritmo                           *
        // *                                                                             *
        // *******************************************************************************
        // *                                                                             *
        // * Il codice fiscale e formato dalle parti: cognome di 3 bytes, nome di        *
        // * 3 bytes, data di nascita e sesso di 5 bytes, comune di nascita di 4 bytes,  *
        // * check digit di 1 byte.                                                      *
        // * La parte "cognome" e determinata dalle prime tre consonanti del cognome,    *
        // * o dalle prime due consonanti  e dalla prima vocale, o dalla prima consonan- *
        // * te e dalle prime due vocali, o dalla prima consonante, dalla prima vocale   *
        // * e da "x".                                                                   *
        // * La parte "nome" e determinata dalla prima, dalle terza e dalle quarta con-  *
        // * sonante del nome, o dalle prime tre consonanti, o dalle prime due consonan- *
        // * ti  e dalla prima vocale, o dalla prima consonante e dalle prime due        *
        // * vocali, o dalla prima consonante, dalla prima vocale e da "x".              *
        // * La parte "data di nascita e sesso" e determinata dall'anno di nascita       *
        // * senza millesimo che deve essere maggiore di  zero, dalla decodifica del     *
        // * mese che deve essere compreso tra 1 e 12,  e  dal giorno che deve essere    *
        // * maggiore di zero. Solo per le donne viene sommato 40 al giorno di nascita.  *
        // * La  parte "comune di nascita"  e determinata dalla decodifica del  comune.  *
        // *                                                                             *
        // * Per calcolare il check digit si utilizzano due tabelle che associano un va- *
        // * lore numerico ai caratteri, una per i caratteri in posizione pari e una per *
        // * i caratteri in posizione dispari.                                           *
        // * Si sommano tutti i valori trovati e si divide per 26. AI risultato cosi     *
        // * ottenuto viene sommato 1. La lettera dell'alfabeto inglese corrispondente   *
        // * e' il check digit.                                                          *
        // *                                                                             *
        // *******************************************************************************
        // * ESEMPIO :      PNS MNC 67C45 H501                                           *
        // *             P   S    N    6    C   5    5    1                              *
        // *      PARI = 3 + 12 + 20 + 15 + 5 + 13 + 13 + 0 = 81                         *
        // *             N    M    C   7   4   H   0                                     *
        // *   DISPARI = 13 + 12 + 2 + 7 + 4 + 7 + 0 = 45                                *
        // *                                                                             *
        // *   N = 81+45 = 126                                                           *
        // *                                                                             *
        // *   126 = 26*4 + 22  = 22   W e' check                                        *
        // *                                                                             *
        // *******************************************************************************

        // *******************************************************************************
        // *                           CODICI DEGLI ERRORI                               *
        // *******************************************************************************
        // *              -1  - errore GENERICO nella compilazione dei dati              *
        // *                    (numero caratteri<>16)                                   *
        // *              -2  - CHECK DIGIT non corrisponde                              *
        // *              -3  - parte COGNOME non corrisponde                            *
        // *              -4  - parte NOME non corrisponde                               *
        // *              -5  - parte DATA NASCITA E SESSO non corrisponde               *
        // *         !!!!!-6  - parte PROVINCIA DI NASCITA non corrisponde               *
        // *                    (la sigla della provincia passata come parametro non     *
        // *                     corrisponde al codice del comune risultante dal         *
        // *                     codice fiscale)                                         *
        // *         !!!!!-7  - parte COMUNE DI NASCITA non corrisponde                  *
        // *                    (il comune di nascita passato come parametro non         *
        // *                    corrisponde al codice del comune risultante dal codice   *
        // *                    fiscale()                                                *
        // *         !!!!!-10 - il codice fiscale passato come argomento contiene un     *
        // *                    codice comune non esistente nel database                 *
        // *******************************************************************************

        string[] c_f = new string[16];
        int errore;
        int pari;
        int dispari;
        int risultato;
        string codice_cognome;
        int sommacognome;
        int i;
        int j;
        codice_c_f = Strings.LCase(codice_c_f.Trim());

        // REM si devono eliminari gli apici e gli spazi
        cognome = Strings.Replace(Strings.LCase(cognome), "'", "");
        cognome = Strings.Replace(Strings.LCase(cognome), " ", "");
        nome = Strings.Replace(Strings.LCase(nome), "'", "");
        nome = Strings.Replace(Strings.LCase(nome), " ", "");

        sesso = Strings.LCase(sesso);

        string[] cod = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        string[] decod_pari = new[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" };
        string[] decod_dispari = new[] { "01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "02", "04", "18", "20", "11", "03", "06", "08", "12", "14", "16", "10", "22", "25", "24", "23" };
        string[] cod_digit = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        string[] decod_digit = new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26" };
        string[] vocali = new[] { "a", "e", "i", "o", "u" };
        string[] mesi = new[] { "a", "b", "c", "d", "e", "h", "l", "m", "p", "r", "s", "t" };

        errore = 0;
        pari = 0;
        dispari = 0;

        // ---------------------------Generazione della parte COGNOME---------------------
        string[] cognome_array = null;
        int[] cognome_bin = null;
        string cod_cognome = "";
        codice_cognome = "";
        sommacognome = 0;

        cognome_bin = new int[Strings.Len(cognome) + 1];
        cognome_array = new string[Strings.Len(cognome) + 1];

        for (i = 1; i <= Strings.Len(cognome); i++)
        {
            cognome_array[i] = Strings.Mid(cognome, i, 1);
            cognome_bin[i] = 1;
            for (j = 0; j <= 4; j++)
            {
                if (cognome_array[i] == vocali[j])
                    cognome_bin[i] = 0;
            }
            sommacognome = sommacognome + cognome_bin[i];
        }

        for (i = 1; i <= Strings.Len(cognome); i++)
        {
            if (cognome_bin[i] == 1)
                cod_cognome = cod_cognome + cognome_array[i];
        }

        for (i = 0; i <= Strings.Len(cognome); i++)
        {
            if (cognome_bin[i] == 0)
                cod_cognome = cod_cognome + cognome_array[i];
        }

        if (Strings.Len(cognome) == 2)
            cod_cognome = Strings.Left(cod_cognome, 2) + "x";
        else if (Strings.Len(cognome) == 1)
            cod_cognome = Strings.Left(cod_cognome, 2) + "xx";
        else
            cod_cognome = Strings.Left(cod_cognome, 3);

        // --------------------fine   Generazione della parte COGNOME---------------------


        // ---------------------------Generazione della parte NOME------------------------
        string[] nome_array = null;
        int[] nome_bin = null;
        string codice_nome = "";
        string cod_nome = "";
        int sommanome = 0;
        codice_nome = "";
        sommanome = 0;

        nome_bin = new int[Strings.Len(nome) + 1];
        nome_array = new string[Strings.Len(nome) + 1];

        for (i = 1; i <= Strings.Len(nome); i++)
        {
            nome_array[i] = Strings.Mid(nome, i, 1);
            nome_bin[i] = 1;
            for (j = 0; j <= 4; j++)
            {
                if (nome_array[i] == vocali[j])
                    nome_bin[i] = 0;
            }
            sommanome = sommanome + nome_bin[i];
        }

        for (i = 1; i <= Strings.Len(nome); i++)
        {
            if (nome_bin[i] == 1)
                cod_nome = cod_nome + nome_array[i];
        }

        for (i = 0; i <= nome.Length; i++)
        {
            if (nome_bin[i] == 0)
                cod_nome = cod_nome + nome_array[i];
        }

        if (Strings.Len(nome) == 2)
            cod_nome = Strings.Left(cod_nome, 2) + "x";
        else if (Strings.Len(nome) == 1)
            cod_nome = Strings.Left(cod_nome, 2) + "xx";
        else if (sommanome >= 4)
            cod_nome = Strings.Left(cod_nome, 1) + Strings.Mid(cod_nome, 3, 2);
        else
            cod_nome = Strings.Left(cod_nome, 3);
        string cod_anno;
        string cod_mese;

        // --------------------fine   Generazione della parte NOME------------------------



        // ------------ Generazione della parte DATA DI NASCITA - SESSO-------------------
        string cod_giorno;
        string cod_data_sesso;
        cod_anno = Strings.Right(anno, 2);
        cod_mese = mesi[mese - 1];

        if (sesso == "f")
            cod_giorno = giorno + 40;
        else
            // Per i giorni compresi tra 1 e 9 deve essere inserito lo '0'
            cod_giorno = giorno.PadLeft(2, "0");

        cod_data_sesso = cod_anno + cod_mese + cod_giorno;
        // -----------fine Generazione della parte DATA DI NASCITA - SESSO----------------



        // '------------ Generazione della parte COMUNE PROVINCIA-------------------

        // REM il codice del comune di nascita risultante dal codice fiscale passato
        // REM come argomento
        // Dim cod_comune_nascita As String
        // 'Dim strComuneInDB As String
        // 'Dim strSiglaProvInDB As String

        // ' +++++++++++++ Inizio modifica ++++++++++++++++++++++++++++
        string codice_c_f_mod;
        codice_c_f_mod = decodeChar(codice_c_f, 7, 8, 10, 11, 13, 14, 15);
        // ' ++++++++++++ Fine modifica ++++++++++++++++++++++++++++++


        // '-------------------------------- CONTROLLI ------------------------------------
        errore = 0;

        try
        {
            if (Strings.Len(codice_c_f) != 16)
            {
                errore = -1;
                throw new ApplicationException();
            }

            if (Strings.Left(codice_c_f, 3) != cod_cognome)
            {
                errore = -3;
                throw new ApplicationException();
            }

            if (Strings.Mid(codice_c_f, 4, 3) != cod_nome)
            {
                errore = -4;
                throw new ApplicationException();
            }

            // ++++++++++++ Inizio modifica +++++++++++++++++++++++++++++
            if (Strings.Mid(codice_c_f_mod, 7, 5) != cod_data_sesso)
            {
                errore = -5;
                throw new ApplicationException();
            }

            // controllo di corrispondenza fra sigla provincia risultante dal codice fiscale passato
            // e quella passata come parametro a sé stante
            // For Each comuneFisc As ComuneFiscale In elencoComuni.localElenco
            // If comuneFisc.Provincia.IdProvincia = Provincia Then
            // If comuneFisc.Descrizione.Trim.ToUpper = comune.Trim.ToUpper Then
            // 'imposto l'errore a 0
            // errore = 0
            // Exit For
            // Else
            // errore = -7
            // End If
            // Else
            // errore = -6
            // End If

            // Next

            // verifico se ha passato i controlli di corrispondenza provincia - comune
            if (errore != 0)
                throw new ApplicationException();

            // ### controllo del check digit ###
            for (i = 1; i <= 15; i++)
                c_f[i] = Strings.Mid(codice_c_f, i, 1);

            // controllo del check digit - calcolo delle cifre pari
            for (i = 2; i <= 14; i += 2)
            {
                for (j = 0; j <= 35; j++)
                {
                    if (c_f[i] == cod[j])
                        pari = pari + decod_pari[j];
                }
            }

            // controllo del check digit - calcolo delle cifre dispari
            for (i = 1; i <= 15; i += 2)
            {
                for (j = 0; j <= 35; j++)
                {
                    if (c_f[i] == cod[j])
                        dispari = dispari + decod_dispari[j];
                }
            }

            // controllo del check digit - risultato della parte progressivo
            risultato = pari + dispari;
            risultato = risultato - Conversion.Int(risultato / (double)26) * 26 + 1;

            if (cod_digit[risultato - 1] != Strings.Right(codice_c_f, 1))
                errore = -2;
        }
        catch (ApplicationException ex)
        {
        }


        return errore;
    }


    /// <summary>
    ///     ''' Dato un codice fiscale ritorna il sesso (M o F)
    ///     ''' </summary>
    ///     ''' <param name="cfIn"></param>
    ///     ''' <returns></returns>
    public static char DecodeSessoFromCF(string cfIn)
    {
        if (cfIn.Trim().Length != 16)
            throw new ArgumentException("Codice fiscale non valido");

        int iGn = Convert.ToInt32(cfIn.Substring(9, 2));

        if (iGn > 40)
            return "F";

        return "M";
    }


    /// <summary>
    ///     ''' Decodifica una data di nascita da codice fiscale attenzione per gli ultacentenari non funziona: li tratta come bambini!!!
    ///     ''' es. anno 1916, su cf 16, viene trattato come 2016
    ///     ''' </summary>
    ///     ''' <param name="cfIn"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public static DateTime DecodeDataNascitaFromCF(string cfIn)
    {
        const var sMesi = "ABCDEHLMPRST";

        if (cfIn.Trim().Length != 16)
            throw new ArgumentException("Codice fiscale non valido");

        cfIn = cfIn.ToUpper();
        var iAnno = Convert.ToInt32(cfIn.Substring(6, 2));
        var iAnnoCurr = Convert.ToInt32(DateTime.Today.Year.ToString().Substring(2, 2));

        if (iAnno > iAnnoCurr)
            iAnno += 1900;
        else
            iAnno += 2000;

        int iMese = sMesi.IndexOf(cfIn.Substring(8, 1)) + 1;
        int iSesso = DecodeSessoFromCF(cfIn) == "M" ? 1 : 2;
        var iGiorno = Convert.ToInt32(cfIn.Substring(9, 2)) - ((iSesso - 1) * 40);

        return new DateTime(iAnno, iMese, iGiorno);
    }

    public static Comune DecodeComuneProvinciaFromCF(string CFin, Common.DbConnection cn)
    {
        if (CFin.Trim().Length != 16)
            throw new ArgumentException("Codice fiscale non valido");

        AdoContainer mAdoContainer = new AdoContainer(cn, null);
        string CodiceCatastale = CFin.Substring(11, 4);

        using (Common.DbCommand oCmd = mAdoContainer.CreateCommand())
        {
            mAdoContainer.OpenConn();

            try
            {
                System.Text.StringBuilder sbSql = new System.Text.StringBuilder("SELECT anag_comuni.Nome, anag_comuni.CodProvincia ");
                sbSql.Append("FROM anag_comuni ");
                sbSql.AppendFormat("WHERE anag_comuni.CodiceCatastale='{0}' ", CodiceCatastale.Trim());

                oCmd.CommandText = sbSql.ToString();

                // Esegue
                using (Common.DbDataReader rd = oCmd.ExecuteReader())
                {
                    // Se non trovato ritorna null
                    if (rd.FieldCount == 0)
                        return null;

                    rd.Read();
                    Comune comune = new Comune();
                    comune.Nome = rd.GetValue(0);
                    comune.SiglaProvincia = rd.GetValue(1);

                    // Ritorna Comune
                    return comune;
                }
            }
            finally
            {
                mAdoContainer.CloseConn(false, false);
            }
        }
    }


    public static string CalcolaCodiceFiscaleNoComuni(string cognome, string nome, DateTime dataNascita, string sesso, string codComune)
    {
        string[] c_f = new string[16];
        int pari;
        int dispari;
        string codice_cognome;
        int sommacognome;
        int i;
        int j;
        System.Text.StringBuilder cf = new System.Text.StringBuilder(20);

        // REM si devono eliminari gli apici e gli spazi
        cognome = cognome.Replace("'", "").Replace(" ", "").ToLower();
        nome = nome.Replace("'", "").Replace(" ", "").ToLower();
        sesso = sesso.ToLower();

        string[] cod = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        string[] decod_pari = new[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" };
        string[] decod_dispari = new[] { "01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "02", "04", "18", "20", "11", "03", "06", "08", "12", "14", "16", "10", "22", "25", "24", "23" };
        string[] cod_digit = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        string[] decod_digit = new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26" };
        string[] vocali = new[] { "a", "e", "i", "o", "u" };
        string[] mesi = new[] { "a", "b", "c", "d", "e", "h", "l", "m", "p", "r", "s", "t" };

        pari = 0;
        dispari = 0;

        // ---------------------------Generazione della parte COGNOME---------------------
        string[] cognome_array = null;
        int[] cognome_bin = null;
        string cod_cognome = "";
        codice_cognome = "";
        sommacognome = 0;

        cognome_bin = new int[Strings.Len(cognome) + 1];
        cognome_array = new string[Strings.Len(cognome) + 1];

        for (i = 1; i <= Strings.Len(cognome); i++)
        {
            cognome_array[i] = Strings.Mid(cognome, i, 1);
            cognome_bin[i] = 1;
            for (j = 0; j <= 4; j++)
            {
                if (cognome_array[i] == vocali[j])
                    cognome_bin[i] = 0;
            }
            sommacognome = sommacognome + cognome_bin[i];
        }

        for (i = 1; i <= Strings.Len(cognome); i++)
        {
            if (cognome_bin[i] == 1)
                cod_cognome = cod_cognome + cognome_array[i];
        }

        for (i = 0; i <= Strings.Len(cognome); i++)
        {
            if (cognome_bin[i] == 0)
                cod_cognome = cod_cognome + cognome_array[i];
        }

        if (Strings.Len(cognome) == 2)
            cod_cognome = Strings.Left(cod_cognome, 2) + "x";
        else if (Strings.Len(cognome) == 1)
            cod_cognome = Strings.Left(cod_cognome, 2) + "xx";
        else
            cod_cognome = Strings.Left(cod_cognome, 3);

        cf.Append(cod_cognome);
        // --------------------fine   Generazione della parte COGNOME---------------------


        // ---------------------------Generazione della parte NOME------------------------
        string[] nome_array = null;
        int[] nome_bin = null;
        string codice_nome = "";
        string cod_nome = "";
        int sommanome = 0;
        codice_nome = "";
        sommanome = 0;

        nome_bin = new int[Strings.Len(nome) + 1];
        nome_array = new string[Strings.Len(nome) + 1];

        for (i = 1; i <= Strings.Len(nome); i++)
        {
            nome_array[i] = Strings.Mid(nome, i, 1);
            nome_bin[i] = 1;
            for (j = 0; j <= 4; j++)
            {
                if (nome_array[i] == vocali[j])
                    nome_bin[i] = 0;
            }
            sommanome = sommanome + nome_bin[i];
        }

        for (i = 1; i <= Strings.Len(nome); i++)
        {
            if (nome_bin[i] == 1)
                cod_nome = cod_nome + nome_array[i];
        }

        for (i = 0; i <= nome.Length; i++)
        {
            if (nome_bin[i] == 0)
                cod_nome = cod_nome + nome_array[i];
        }

        if (Strings.Len(nome) == 2)
            cod_nome = Strings.Left(cod_nome, 2) + "x";
        else if (Strings.Len(nome) == 1)
            cod_nome = Strings.Left(cod_nome, 2) + "xx";
        else if (sommanome >= 4)
            cod_nome = Strings.Left(cod_nome, 1) + Strings.Mid(cod_nome, 3, 2);
        else
            cod_nome = Strings.Left(cod_nome, 3);

        cf.Append(cod_nome);
        string cod_anno;
        string cod_mese;
        // --------------------fine   Generazione della parte NOME------------------------



        // ------------ Generazione della parte DATA DI NASCITA - SESSO-------------------
        string cod_giorno;
        cod_anno = Strings.Right(dataNascita.Year.ToString(), 2);
        cod_mese = mesi[dataNascita.Month - 1];

        if (sesso == "f")
            cod_giorno = dataNascita.Day + 40;
        else
            // Per i giorni compresi tra 1 e 9 deve essere inserito lo '0'
            cod_giorno = dataNascita.Day.ToString().PadLeft(2, "0");


        cf.Append(cod_anno);
        cf.Append(cod_mese);
        cf.Append(cod_giorno);
        // -----------fine Generazione della parte DATA DI NASCITA - SESSO----------------



        // '------------ Generazione della parte COMUNE PROVINCIA-------------------
        cf.Append(codComune.ToUpper());


        cf.Append(CalcolaCarattereControllo(cf.ToString()));

        return cf.ToString().ToUpper();
    }
}
