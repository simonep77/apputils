using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AppUtils.Lib.Common
{
    /// <summary>
    /// Utilita' per stringhe (extension methods)
    /// </summary>
    public static class StringUT
    {

        /// <summary>
        /// Tronca una stringa
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Truncate(this string input, int len)
        {
            if (input == null)
                return string.Empty;

            if (len == 0 || input.Length <= len)
                return input;

            return input.Substring(0, len);
        }

        /// <summary>
        /// Decompone stringa in parole.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> GetWords(this string input)
        {
            var ret = new List<string>();

            if (input == null)
                return ret;

            var sb = new StringBuilder(50);

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0)
                    {
                        ret.Add(sb.ToString());
                        sb.Length = 0;
                    }
                }
                else
                    sb.Append(c);
            }

            return ret;
        }

        /// <summary>
        /// Spezza eventuali parole piu' lunghe di wordLen con uno spazio
        /// </summary>
        /// <param name="input"></param>
        /// <param name="wordLen"></param>
        /// <returns></returns>
        public static string AddWordSpace(this string input, int wordLen)
        {

            if (input == null)
                return input;

            var iwc = 0;
            var sb = new StringBuilder(input.Length + 30);

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    iwc = 0;
                }
                else
                {
                    iwc++;

                    if (iwc > wordLen)
                    {
                        sb.Append(c);
                        iwc = 0;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }



        /// <summary>
        /// Ritorna una stringa vuota 
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetEmptyWithLen(int len)
        {
            return string.Empty.PadRight(len, ' ');
        }

        /// <summary>
        /// Ritorna una stringa di lunghezza len con eventuali caratteri eccedenti filler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        public static string FillFixedLeft(this string input , int len, char filler)
        {
            if (filler == '\0')
                throw new ArgumentException();

            if (len <= input.Length)
                return input;

            return input.PadLeft(len - input.Length, filler);
        }

        /// <summary>
        /// Ritorna una stringa di lunghezza len con eventuali caratteri eccedenti ' '
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillFixedLeft(this string input, int len)
        {
            return input.FillFixedLeft(len, ' ');
        }

        /// <summary>
        ///  Ritorna una stringa di lunghezza len con eventuali caratteri eccedenti filler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        public static string FillFixedRight(this string input, int len, char filler)
        {
            if (filler == '\0')
                throw new ArgumentException();

            if (len <= input.Length)
                return input;

            return input.PadRight(len - input.Length, filler);
        }

        /// <summary>
        /// Ritorna una stringa di lunghezza len con eventuali caratteri eccedenti ' '
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FillFixedRight(this string input, int len)
        {
            return input.FillFixedRight(len, ' ');
        }


        /// <summary>
        /// Ritorna stringa invertita
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Reverse(this string input)
        {
            var cs = input.ToCharArray();
            Array.Reverse(cs);
            return new string(cs);
        }



        /// <summary>
        ///  Normalizza testo ascii:
        ///  - converte i caratteri di servizio (0-31) in spazi
        ///  - converte accentati in standard
        ///  - elimina il resto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NormalizeAscii(this string input)
        {
            if (input == null)
                input = string.Empty;

            StringBuilder sb = new StringBuilder(input.Length);

            for (int index = 0; index <= input.Length - 1; index++)
            {
                int iChar = Convert.ToInt32(input[index]);
                switch (iChar)
                {
                    case 96:
                        {
                            sb.Append("'");
                            break;
                        }

                    case object _ when 0 <= iChar && iChar <= 31:
                        {
                            sb.Append(CharCodes.SPACE);
                            break;
                        }

                    case object _ when 32 <= iChar && iChar <= 127:
                        {
                            sb.Append(input[index]);
                            break;
                        }

                    case object _ when 224 <= iChar && iChar <= 230:
                        {
                            sb.Append("a");
                            break;
                        }

                    case object _ when 232 <= iChar && iChar <= 235:
                        {
                            sb.Append("e");
                            break;
                        }

                    case object _ when 236 <= iChar && iChar <= 239:
                        {
                            sb.Append("i");
                            break;
                        }

                    case object _ when 242 <= iChar && iChar <= 246:
                        {
                            sb.Append("o");
                            break;
                        }

                    case object _ when 249 <= iChar && iChar <= 252:
                        {
                            sb.Append("u");
                            break;
                        }

                    case object _ when 192 <= iChar && iChar <= 198:
                        {
                            sb.Append("A");
                            break;
                        }

                    case object _ when 200 <= iChar && iChar <= 203:
                        {
                            sb.Append("E");
                            break;
                        }

                    case object _ when 204 <= iChar && iChar <= 207:
                        {
                            sb.Append("I");
                            break;
                        }

                    case object _ when 210 <= iChar && iChar <= 214:
                        {
                            sb.Append("O");
                            break;
                        }

                    case object _ when 217 <= iChar && iChar <= 220:
                        {
                            sb.Append("U");
                            break;
                        }
                }
            }

            return sb.ToString();
        }

        #region CONVERSION

        /// <summary>
        /// Normalizza i caratteri per essere compatibili con i flussi SEPA/CBI
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NormalizeCBI(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            const string PATTERNREGEX = @"[A-Za-z\d\+\-\*\.\;\:\,_\@\%\&\(\)\'\$\""<=>?\[\]\/\s]";
            const string PATTERNCHARTOSUSBSTITUTE = "[ÁÂÀÄČĆĎĐÆÈÉÊÍÎÏĹĽŇÓÔÖŐŔŠŤÙÚÛÜŰÝŸŽßàáâäçćčďđèéêëìíîïĺľňñóôöőŕšťùúûüűÿýžæŒœ]";

            Regex charAmmesso = new Regex(PATTERNREGEX);
            Regex charToSubstitute = new Regex(PATTERNCHARTOSUSBSTITUTE);
            StringBuilder sb = new StringBuilder(input.Length);
            int iChar;

            Encoding iso = Encoding.GetEncoding("ISO-8859-8");
            Encoding utf8 = Encoding.UTF8;

            for (int index = 0; index <= input.Length - 1; index++)
            {
                iChar = Convert.ToInt32(input[index]);

                // check preliminare per caratteri da scartare, si accoda uno spazio
                if (iChar >= 0 & iChar < 32)
                    sb.Append(CharCodes.SPACE);
                else
                    // se carattere fa parte del set ammesso , viene accodato alla string risultante
                    if (charAmmesso.IsMatch(input[index].ToString()))
                    sb.Append(input[index]);
                else
                        // se carattere fa parte di quelli da sostituire, viene accodato il carattere 
                        // derivante dalla conversione seguente *
                        if (charToSubstitute.IsMatch(input[index].ToString()))
                {
                    // caso particolare per il carattere ß
                    if (input[index].ToString() == "ß")
                        sb.Append("s");
                    else
                    {
                        // * istruzioni per la conversione del carattere nel set ISO-8859-8
                        byte[] utfBytes = utf8.GetBytes(input[index].ToString());
                        byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);

                        sb.Append(iso.GetString(isoBytes));
                    }
                }
            }

            return sb.ToString();
        }


        #endregion

    }
}
