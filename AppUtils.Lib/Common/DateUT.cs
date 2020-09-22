using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppUtils.Lib.Common
{
    /// <summary>
    /// Funzioni di utilita' sulle date
    /// </summary>
    public class DateUT
    {

        private static CultureInfo _Culture_IT = new CultureInfo("it-IT");
        private static DateTime[] _ArrFestivita = new DateTime[] { 
            new DateTime(1,1,1),
            new DateTime(1,1,6),
            new DateTime(1,4,25),
            new DateTime(1,5,1),
            new DateTime(1,6,2),
            new DateTime(1,8,15),
            new DateTime(1,11,1),
            new DateTime(1,12,8),
            new DateTime(1,12,25),
            new DateTime(1,12,26)
        };
        private static DateTime[] _ArrFestivitaRM = new DateTime[] {
            new DateTime(1,6,29),
        };
        private static DateTime[] _ArrFestivitaMI = new DateTime[] {
            new DateTime(1,12,7),
        };
        private static ConcurrentDictionary<int, DateTime> _Pasquette = new System.Collections.Concurrent.ConcurrentDictionary<int, DateTime>();

        #region PUBLIC METHODS

        #region PARSING e FORMATTING

        /// <summary>
        /// Converte una stringa in data secondo il formato specificato: se conversione in errore torna Date.MinValue
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <param name="sFormatoIn"></param>
        /// <returns></returns>
        public static DateTime ParseDataNull(string sDataIn, string sFormatoIn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sFormatoIn))
                    sFormatoIn = @"dd/MM/yyyy";

                return DateTime.ParseExact(sDataIn, sFormatoIn, _Culture_IT);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Converte una stringa in data secondo il formato dd/MM/yyyy: se conversione in errore torna Date.MinValue
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <returns></returns>
        public static DateTime ParseDataNull(string sDataIn)
        {
            return ParseDataNull(sDataIn, null);
        }


        /// <summary>
        /// Converte una stringa in data secondo il formato specificato e con nome campo personalizzabile
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <param name="nomeCampo"></param>
        /// <param name="sFormatoIn"></param>
        /// <returns></returns>
        public static DateTime ParseData(string sDataIn, string nomeCampo, string sFormatoIn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nomeCampo))
                    sFormatoIn = @"data";

                if (string.IsNullOrWhiteSpace(sFormatoIn))
                    sFormatoIn = @"dd/MM/yyyy";

                return DateTime.ParseExact(sDataIn, sFormatoIn, _Culture_IT);

            }
            catch (Exception)
            {
                throw new ArgumentException($"La {nomeCampo} fornita ({sDataIn}) non e' valida. Deve rispettare la formattazione attesa ({sFormatoIn})");
            }
        }

        /// <summary>
        /// Converte una stringa in data secondo il formato dd/MM/yyyy con nome campo fornito
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <param name="nomeCampo"></param>
        /// <returns></returns>
        public static DateTime ParseData(string sDataIn, string nomeCampo)
        {
            return ParseData(sDataIn, nomeCampo, null);
        }

        /// <summary>
        /// Converte una stringa in data secondo il formato dd/MM/yyyy con nome campo generico 'data'
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <returns></returns>
        public static DateTime ParseData(string sDataIn)
        {
            return ParseData(sDataIn, null, null);
        }


        /// <summary>
        /// Ritorna data formattata come dd/MM/yyyy
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToDateStringIT(DateTime dataIn)
        {
            return dataIn.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Ritorna data formattata come dd/MM/yyyy, se data=minvalue allora torna stringa vuota
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToDateStringEmptyIT(DateTime dataIn)
        {
            if (dataIn == DateTime.MinValue)
                return string.Empty;

            return dataIn.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Ritorna data/ora formattata come dd/MM/yyyy HH:mm
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToDateTimeStringIT(DateTime dataIn)
        {
            return dataIn.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Ritorna data/ora formattata come dd/MM/yyyy HH:mm, se data = minvalue ritorna stringa vuota
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToDateTimeStringEmptyIT(DateTime dataIn)
        {
            if (dataIn == DateTime.MinValue)
                return string.Empty;

            return dataIn.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Ritorna ora formattata come HH:ss
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToTimeStringIT(DateTime dataIn)
        {
            return dataIn.ToString("HH:mm");
        }


        /// <summary>
        /// Ritorna la data in formato testuale per essere utilizzata nelle lettere.
        /// Il formato è: gg mese-parlante aaaa
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static string ToDataEstesaStringIT(DateTime dataIn)
        {
            try
            {
                return dataIn.ToString("dd MMMM yyyy", _Culture_IT);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Errore in 'ToDataEstesaStringIT': {0}", ex.Message);
            }
        }


        #endregion

        #region FACILITY

        /// <summary>
        /// Ritorna la data di fine mese rispetto alla data fornita
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static DateTime GetDataFineMese(DateTime dataIn)
        {
            return new DateTime(dataIn.Year, dataIn.Month, DateTime.DaysInMonth(dataIn.Year, dataIn.Month));
        }

        /// <summary>
        ///  Ritorna data inizio mese
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static DateTime GetDataInizioMese(DateTime dataIn)
        {
            return new DateTime(dataIn.Year, dataIn.Month, 1);
        }

        /// <summary>
        /// Verifica se una data e' compresa in un intervallo
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="dataDa"></param>
        /// <param name="dataA"></param>
        /// <returns></returns>
        public static bool IsBetween(DateTime dataIn, DateTime dataDa, DateTime dataA)
        {
            return (dataIn >= dataDa && dataIn <= dataA);
        }

        /// <summary>
        /// Indica se due intervalli data sono sovrapposti (estremi compresi)
        /// </summary>
        /// <param name="data1Inizio"></param>
        /// <param name="Data1Fine"></param>
        /// <param name="data2Inizio"></param>
        /// <param name="data2Fine"></param>
        /// <returns></returns>
        public static bool IsSovrapposte(DateTime data1Inizio,DateTime Data1Fine, DateTime data2Inizio, DateTime data2Fine)
        {
            return (data1Inizio <= data2Fine && Data1Fine >= data2Inizio);
        }

        /// <summary>
        /// Indica se due intervalli di date sono contigui (fine di uno = inizio dell'altro)
        /// </summary>
        /// <param name="data1Inizio"></param>
        /// <param name="Data1Fine"></param>
        /// <param name="data2Inizio"></param>
        /// <param name="data2Fine"></param>
        /// <returns></returns>
        public static bool IsContigue(DateTime data1Inizio, DateTime Data1Fine, DateTime data2Inizio, DateTime data2Fine)
        {
            return (Data1Fine == data2Inizio && data1Inizio != data2Fine) ||
                    (data1Inizio == data2Fine && Data1Fine != data2Inizio);
        }


        /// <summary>
        /// Ritorna sovrapposizione tra 2 intervalli data
        /// </summary>
        /// <param name="data1Inizio"></param>
        /// <param name="Data1Fine"></param>
        /// <param name="data2Inizio"></param>
        /// <param name="data2Fine"></param>
        /// <returns></returns>
        public static TimeSpan GetSovrapposizione(DateTime data1Inizio, DateTime Data1Fine, DateTime data2Inizio, DateTime data2Fine)
        {
            return MinData(Data1Fine, data2Fine).Subtract(MaxData(data1Inizio, data2Inizio));
        }


        /// <summary>
        /// Ritorna la data minima tra quelle presenti in elenco
        /// </summary>
        /// <param name="dates"></param>
        /// <returns>
        /// Se input null o vuoto ritorna MinValue
        /// </returns>
        public static DateTime MinData(params DateTime[] dates)
        {
            var dtRet = DateTime.MaxValue;

            if (dates == null || dates.Length == 0)
                return DateTime.MinValue;

            for (int i = 0; i < dates.Length -1 ; i++)
            {
                if (dates[i] < dtRet)
                    dtRet = dates[i];
            }

            return dtRet;
        }

        /// <summary>
        ///  Ritorna data massima tra quelle presenti in elenco
        /// </summary>
        /// <param name="dates"></param>
        /// <returns>
        /// Se input null o vuoto ritorna MinValue
        /// </returns>
        public static DateTime MaxData(params DateTime[] dates)
        {
            var dtRet = DateTime.MinValue;

            if (dates == null || dates.Length == 0)
                return DateTime.MinValue;

            for (int i = 0; i < dates.Length - 1; i++)
            {
                if (dates[i] > dtRet)
                    dtRet = dates[i];
            }

            return dtRet;
        }



        #endregion

        #region FESTIVITA'

        /// <summary>
        /// Calcola la pasqua dell'anno in input
        /// </summary>
        /// <param name="annoIn"></param>
        /// <returns></returns>
        public static DateTime CalcolaPasqua(int annoIn)
        {
            var y = annoIn;
            var d = (((255 - 11 * (y % 19)) -21) % 30) + 21;
            var x = d > 48 ? 1 : 0;
            var dtEaster = new DateTime(y, 3, 1);
            dtEaster = dtEaster.AddDays(d + x + 6 - ((y + y / 4 + d + x + 1) % 7));

            return dtEaster;
        }

        /// <summary>
        /// Ritorna la data dela pasquetta per l'anno fornito.
        /// Il dato e' cached in memoria
        /// </summary>
        /// <param name="annoIn"></param>
        /// <returns></returns>
        public static DateTime GetPasquetta(int annoIn)
        {
            DateTime dtPasq;

            if (!_Pasquette.TryGetValue(annoIn, out dtPasq))
            {
                dtPasq = CalcolaPasqua(annoIn).AddDays(1);
                _Pasquette[annoIn] = dtPasq;
            }

            return dtPasq;
        }


        /// <summary>
        /// Calcola una data aggiungendo giorni tenendo conto di Domeniche e delle feste standard. Il sabato può essere considerato festivo (true) o lavorativo (false)
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="numGiorni"></param>
        /// <param name="sabatoFestivoIn"></param>
        /// <returns></returns>
        public static DateTime AddGiorniLavorativi(DateTime dataIn, int numGiorni, bool sabatoFestivoIn)
        {
            int iStepGiorni = +1;
            DateTime dtPasQuetta = GetPasquetta(dataIn.Year);
            dataIn = dataIn.Date;

            // Se numeroGiorni < 0 lo step è al contrario
            if (numGiorni < 0)
                iStepGiorni = -1;

            while (numGiorni != 0)
            {
                // Aggiunge giorno
                dataIn = dataIn.AddDays(iStepGiorni);

                if (IsFestivo(dataIn, sabatoFestivoIn))
                    continue;

                // Decrementa giorni
                numGiorni -= iStepGiorni;
            }

            // Ritorna data calcolata
            return dataIn;
        }


        /// <summary>
        /// Aggiunge un numero di giorni lavorativi alla data specificata. Il sabato e' inteso come giorno NON lavorativo
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="numGiorni"></param>
        /// <returns></returns>
        public static DateTime AddGiorniLavorativi(DateTime dataIn, int numGiorni)
        {
            return AddGiorniLavorativi(dataIn, numGiorni, true);
        }

        /// <summary>
        /// Verifica se una data risulta festiva. Il sabato e'inteso come festivo (NON LAVORATIVO).
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        public static bool IsFestivo(DateTime dataIn)
        {
            //Se ieri + 1 gg lavorativo e' uguale ad oggi allora significa che oggi non e' festivo
            return IsFestivo(dataIn, true);
        }

        /// <summary>
        /// Verifica se una data risulta festiva. Opzionalmente si puo' specificare come considerare il sabato
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="sabatoFestivoIn"></param>
        /// <returns></returns>
        public static bool IsFestivo(DateTime dataIn, bool sabatoFestivoIn)
        {
            if (dataIn.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (sabatoFestivoIn && dataIn.DayOfWeek == DayOfWeek.Saturday)
                return true;

            // Festivita' Naz
            for (int i = 0; i <= _ArrFestivita.Length - 1; i++)
            {
                if (dataIn.Month == _ArrFestivita[i].Month && dataIn.Day == _ArrFestivita[i].Day)
                    return true;
            }

            // Festivita' Locali

            // Se siamo prima di giugno verifichiamo pasquetta
            if (dataIn.Month < 6 && dataIn.Month > 2 && dataIn.DayOfWeek == DayOfWeek.Monday)
            {
                if (dataIn.Equals(GetPasquetta(dataIn.Year)))
                    return true;
            }

            return false;

        }

        /// <summary>
        /// Calcola primo giorno lavorativo alla data di riferimento. Se la data di riferimento è Lavoraiva restituisce la stessa data. Il sabato può essere considerato festivo (true) o lavorativo (false)
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="sabatoFestivoIn"></param>
        /// <returns></returns>
        public static DateTime CalcolaPrimoGiornoLavorativo(DateTime dataIn, bool sabatoFestivoIn)
        {
            return AddGiorniLavorativi(dataIn.AddDays(-1), 1, sabatoFestivoIn);
        }


        /// <summary>
        /// calcola il numero di giorni lavorativi tra due date
        //  il parametro sabatoNonLavorativo = True indica che la settimana consta di 5 giorni lavorativi
        //  l'intervallo mimimo è 0 giorni: un evento che inizia e finisce lo stesso giorno lavorativo restituisce 0
        /// </summary>
        /// <param name="dataInizioIn"></param>
        /// <param name="dataFineIn"></param>
        /// <param name="sabatoFestivoIn"></param>
        /// <returns></returns>
        public static int CalcolaGiorniLavorativi(DateTime dataInizioIn, DateTime dataFineIn, bool sabatoFestivoIn)
        {
            var differenzaDate = Convert.ToInt32((dataInizioIn.Date - dataFineIn.Date).TotalDays);

            if (differenzaDate <= 0)
                return 0;

            var festivi = 0;
            var iterDate = dataInizioIn;

            while(iterDate <= dataFineIn)
            {
                if (IsFestivo(iterDate, sabatoFestivoIn))
                    festivi++;

                iterDate = iterDate.AddDays(1);
            }

            return Math.Max(0, differenzaDate - festivi);
        }


        /// <summary>
        /// Ritorna timespan di sovrapposizione tra due intervalli di date senza considerare le festività
        /// </summary>
        /// <param name="data1Inizio"></param>
        /// <param name="Data1Fine"></param>
        /// <param name="data2Inizio"></param>
        /// <param name="data2Fine"></param>
        /// <returns></returns>
        public static TimeSpan GetSovrapposizioneFeriali(DateTime data1Inizio, DateTime Data1Fine, DateTime data2Inizio, DateTime data2Fine)
        {
            var dtInizio = MaxData(data1Inizio, data2Inizio);
            var dtFine = MinData(Data1Fine, data2Fine);
            var tsElaps = dtFine.Subtract(dtInizio);
            var iDays = Convert.ToInt32(tsElaps.TotalDays);

            for (int i = 0; i < iDays - 1; i++)
            {
                if (IsFestivo(dtInizio))
                    tsElaps = tsElaps.Subtract(TimeSpan.FromDays(1));

                dtInizio = dtInizio.AddDays(1);
            }

            return tsElaps;
        }

               

        #endregion

        #region CALCOLI VARI

        public static int CalcolaEta(DateTime dataNascita, DateTime dataCalcolo)
        {
            //Se la data da cui calcolare l'età è minore o uguale alla data di nascita torna 0
            if (dataCalcolo < dataNascita)
                return 0;

            //Data per comparazione temporale
            var dataComparazioneCalcolo = new DateTime(2020, dataCalcolo.Month, dataCalcolo.Day);
            var dataComparazioneNascita = new DateTime(2020, dataNascita.Month, dataNascita.Day);

            //Calcola anni di differenza
            var anni = dataCalcolo.Year - dataNascita.Year;

            //Se la data di dataComparazioneCalcolo è minore di quella di dataComparazioneNascita significa che ancora non abbiamo compiuto gli anni
            if (dataComparazioneCalcolo < dataComparazioneNascita)
                anni--;

            return anni;
        }

        #endregion

        #endregion
    }
}

//''' <summary>
//''' Funzioni generiche per Date
//''' </summary>
//''' <remarks></remarks>
//Public Class DateUT


//    ''' <summary>
//    ''' Include festività specifiche provinciali (al momento solo RM e MI)
//    ''' Operazione da effettuare una volta a caricamento applicazione.
//    ''' Chiamate successive non provocano alcun risultato
//    ''' </summary>
//    ''' <param name="codProvincia"></param>
//    ''' <remarks></remarks>
//    Public Shared Sub AttivaFestivitaProv(codProvincia As String)
//        Dim iArrLen As Integer = _ArrFestivita.Length
//        Dim ArrDates() As Date = Nothing

//        Select Case codProvincia.ToUpper()
//            Case "RM"
//                'Imposta array da caricare e deimposta
//                ArrDates = _ArrFestivitaRM
//                _ArrFestivitaRM = Nothing
//            Case "MI"
//                'Imposta array da caricare e deimposta
//                ArrDates = _ArrFestivitaMI
//                _ArrFestivitaMI = Nothing
//            Case Else
//                Throw New ArgumentException(String.Concat("Nessuna festivita' prevista per il codice '", codProvincia, "'"))
//        End Select

//        'Se array gia' caricato in precedenza esce
//        If ArrDates Is Nothing Then
//            Return
//        End If

//        'Copia elementi su array festivita'
//        Array.Resize(_ArrFestivita, iArrLen + ArrDates.Length)
//        Array.Copy(ArrDates, 0, _ArrFestivita, iArrLen, ArrDates.Length)
//    End Sub








//End Class
