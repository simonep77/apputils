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
using System.Text.RegularExpressions;

namespace AppUtils.Lib.Codici
{

    public class PasswordCheck
    {



        /// <summary>
        ///     ''' Enumeratore per tutte le Regole di Match adottate
        ///     ''' </summary>
        public enum EnumMatch : int
        {
            CaratteriAmmessi = 0,
            MinLenght = 1,
            MaxLenght = 2,
            MinLowercase = 3,
            MinUppercase = 4,
            MinNumeric = 5,
            MinSpecialChar = 6
        }




        const string STR_LOWER_CASE_ALLOWED = "a-z";
        const string STR_UPPER_CASE_ALLOWED = "A-Z";
        const string STR_NUMBERS_ALLOWED = "0-9";
        const string STR_SPECIAL_CHARS_ALLOWED = @"!#$%&()*+,-./:;<=>?@[\]^_{|}~§ìèéàòù";
        const string STR_SIMPLE_CHARS_ALLOWED = STR_LOWER_CASE_ALLOWED + STR_UPPER_CASE_ALLOWED + STR_NUMBERS_ALLOWED;
        const string STR_ALL_CHARS_ALLOWED = STR_SIMPLE_CHARS_ALLOWED + STR_SPECIAL_CHARS_ALLOWED;



        private UInt16 MinLen = UInt16.MinValue;
        /// <summary>
        ///     ''' numero MINIMO di caratteri ammessi
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public UInt16 MinLength
        {
            get
            {
                return this.MinLen;
            }
            set
            {
                this.MinLen = value;
                if (this.MinLen > this.MaxLen)
                    this.MaxLen = this.MinLen;
            }
        }

        private UInt16 MaxLen = UInt16.MaxValue;
        /// <summary>
        ///     ''' numero MASSIMO di caratteri ammessi
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public UInt16 MaxLength
        {
            get
            {
                return this.MaxLen;
            }
            set
            {
                this.MaxLen = value;
                if (this.MaxLen < this.MinLen)
                    this.MinLen = this.MaxLen;
            }
        }

        private Int16 MinLC = 0;
        // ''' <summary>
        // ''' numero MINIMO di lettere minuscole ammesse (0:nessun limite , -1:non ammesso)
        // ''' </summary>
        // ''' <returns></returns>
        public Int16 MinLowercase
        {
            get
            {
                return this.MinLC;
            }
            set
            {
                if (value < 0)
                    this.MinLC = -1;
                else
                    this.MinLC = value;
            }
        }

        private Int16 MinUC = 0;
        /// <summary>
        ///     ''' numero MINIMO di lettere maiuscole ammesse (0:nessun limite , -1:non ammesso)
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public Int16 MinUppercase
        {
            get
            {
                return this.MinUC;
            }
            set
            {
                if (value < 0)
                    this.MinUC = -1;
                else
                    this.MinUC = value;
            }
        }

        private Int16 MinNUM = 0;
        /// <summary>
        ///     ''' numero MINIMO di valori numerici ammessi (0:nessun limite , -1:non ammesso)
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public Int16 MinNumeric
        {
            get
            {
                return this.MinNUM;
            }
            set
            {
                if (value < 0)
                    this.MinNUM = -1;
                else
                    this.MinNUM = value;
            }
        }

        private Int16 MinSC = 0;
        // ''' <summary>
        // ''' numero MINIMO di caratteri speciali ammessi (0:nessun limite , -1:non ammesso)
        // ''' </summary>
        // ''' <returns></returns>
        public Int16 MinSpecialChar
        {
            get
            {
                return MinSC;
            }
            set
            {
                if (value < 0)
                    MinSC = -1;
                else
                    MinSC = value;
            }
        }



        public PasswordCheck()
        {
        }



        /// <summary>
        ///     ''' verifica di tutte le regola di match per la validazione della password
        ///     ''' </summary>
        ///     ''' <param name="StringaToMatchIn">password da verificare</param>
        ///     ''' <returns></returns>
        public EsitoControlloLista CheckMatch(string StringaToMatchIn)
        {
            var returnCheckMatch = new EsitoControlloLista();
            MatchCollection resultMatches;
            string patternRegex = "";
            string strMsg = "";
            bool isValid = true;

            patternRegex = string.Format("[{0}]", STR_ALL_CHARS_ALLOWED) + "{1,1}";
            resultMatches = Regex.Matches(StringaToMatchIn, patternRegex);
            isValid = resultMatches.Count == StringaToMatchIn.Length;
            returnCheckMatch.Add(isValid, (int)EnumMatch.CaratteriAmmessi, isValid ? string.Empty : "La password contiene dei caratteri non ammessi");

            isValid = (this.MinLength == 0 || this.MinLength <= StringaToMatchIn.Length);
            returnCheckMatch.Add(isValid, (int)EnumMatch.MinLenght, isValid ? string.Empty : string.Format("La password deve essere lunga almeno {0} caratter{1}", this.MinLength, MinLength == 1 ? "e" : "i")
    );

            isValid = this.MaxLength == 0 || this.MaxLength >= StringaToMatchIn.Length;
            returnCheckMatch.Add(isValid, (int)EnumMatch.MaxLenght, isValid ? string.Empty : string.Format("La password deve essere lunga al massimo {0} caratter{1}", this.MaxLength, this.MaxLength == 1 ? "e" : "i")
    );

            // LETTERE MINUSCOLE
            patternRegex = string.Format("[{0}]", STR_LOWER_CASE_ALLOWED) + "{1,1}";
            resultMatches = Regex.Matches(StringaToMatchIn, patternRegex);
            isValid = (this.MinLowercase == 0 || (this.MinLowercase == -1 && resultMatches.Count == 0) || (this.MinLowercase <= resultMatches.Count));
            if (isValid)
                strMsg = "";
            else if (this.MinLowercase == -1)
                strMsg = "La password non deve contenere lettere minuscole";
            else if (this.MinLowercase > 0)
                strMsg = isValid ? string.Empty : string.Format("La password deve contenere almeno {0} letter{1} minuscol{1}", this.MinLowercase, this.MinLowercase == 1 ? "a" : "e");
            returnCheckMatch.Add(isValid, (int)EnumMatch.MinLowercase, strMsg);

            // LETTERE MAIUSCOLE
            patternRegex = string.Format("[{0}]", STR_UPPER_CASE_ALLOWED) + "{1,1}";
            resultMatches = Regex.Matches(StringaToMatchIn, patternRegex);
            isValid = (this.MinUppercase == 0 || (this.MinUppercase == -1 && resultMatches.Count == 0) || (this.MinUppercase <= resultMatches.Count));
            if (isValid)
                strMsg = "";
            else if (this.MinUppercase == -1)
                strMsg = "La password non deve contenere lettere maiuscole";
            else if (this.MinUppercase > 0)
                strMsg = isValid ? string.Empty : string.Format("La password deve contenere almeno {0} letter{1} maiuscol{1}", this.MinUppercase, this.MinUppercase == 1 ? "a" : "e");
            returnCheckMatch.Add(isValid, (int)EnumMatch.MinUppercase, strMsg);
            // '''''''''''''''''

            // NUMERI
            patternRegex = string.Format("[{0}]", STR_NUMBERS_ALLOWED) + "{1,1}";
            resultMatches = Regex.Matches(StringaToMatchIn, patternRegex);
            isValid = (this.MinNumeric == 0 || (this.MinNumeric == -1 && resultMatches.Count == 0) || (this.MinNumeric <= resultMatches.Count));
            if (isValid)
                strMsg = "";
            else if (this.MinNumeric == -1)
                strMsg = "La password non deve contenere numeri";
            else if (this.MinNumeric > 0)
                strMsg = isValid ? string.Empty : string.Format("La password deve contenere almeno {0} numer{1}", this.MinNumeric, this.MinNumeric == 1 ? "o" : "i");
            returnCheckMatch.Add(isValid, (int)EnumMatch.MinNumeric, strMsg);
            // '''''''''''''''''

            // CARATTERI SPECIALI
            patternRegex = string.Format("[{0}]", STR_SPECIAL_CHARS_ALLOWED) + "{1,1}";
            resultMatches = Regex.Matches(StringaToMatchIn, patternRegex);
            isValid = (this.MinSpecialChar == 0 || (this.MinSpecialChar == -1 && resultMatches.Count == 0) || (this.MinSpecialChar <= resultMatches.Count));
            if (isValid)
                strMsg = "";
            else if (this.MinSpecialChar == -1)
                strMsg = "La password non deve contenere caratteri speciali";
            else if (this.MinSpecialChar > 0)
                strMsg = isValid ? string.Empty : string.Format("La password deve contenere almeno {0} caratteri{1} special{1}", this.MinSpecialChar, this.MinSpecialChar == 1 ? "e" : "i");
            returnCheckMatch.Add(isValid, (int)EnumMatch.MinSpecialChar, strMsg);
            // '''''''''''''''''

            return returnCheckMatch;
        }



        /// <summary>
        ///     ''' Stringa contenente tutti i caratteriSpeciali ammessi
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string GetSpecialCharsAllowed()
        {
            if (this.MinSpecialChar < 0)
                return string.Empty;
            return STR_SPECIAL_CHARS_ALLOWED.Replace(@"\", "");
        }

        /// <summary>
        ///     ''' Testo informativo per la regola di validazione password relativo alla lunghezza ammessa
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string getInfoPswLength()
        {
            var carText = (this.MinLength, MinLength == 1 ? "e" : "i");
            if (this.MinLength == this.MaxLength)
                return $"Deve essere composta da {this.MinLength} caratter{carText}";
            else
                return $"Deve essere composta da minimo {this.MinLength} caratter{carText} e massimo da {this.MaxLength}";
        }

        /// <summary>
        ///     ''' Testo informativo per la regola di validazione password relativo alle lettere minuscole
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string getInfoPswLowerCase()
        {
            var strMsg = "Sono ammesse lettere minuscole";
            if (this.MinLowercase < 0)
                return string.Format("Non {0}", strMsg.ToString().ToLower());
            else if (this.MinLowercase == 0)
                return strMsg;
            else
                return $"{strMsg}, almeno {this.MinLowercase}";
        }

        /// <summary>
        ///     ''' Testo informativo per la regola di validazione password relativo alle lettere maiuscole
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string getInfoPswUpperCase()
        {
            var strMsg = "Sono ammesse lettere maiuscole";
            if (this.MinUppercase < 0)
                return string.Format("Non {0}", strMsg.ToString().ToLower());
            else if (this.MinUppercase == 0)
                return strMsg;
            else
                return $"{strMsg}, almeno {this.MinUppercase}";
        }

        /// <summary>
        ///     ''' Testo informativo per la regola di validazione password relativo ai valori numerici
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string getInfoPswNumeri()
        {
            var strMsg = "Sono ammessi numeri";
            if (this.MinNumeric < 0)
                return string.Format("Non {0}", strMsg.ToString().ToLower());
            else if (this.MinNumeric == 0)
                return strMsg;
            else
                return $"{strMsg}, almeno {this.MinNumeric}";
        }

        /// <summary>
        ///     ''' Testo informativo per la regola di validazione password relativo ai caratteri speciali
        ///     ''' </summary>
        ///     ''' <returns></returns>
        public string getInfoPswSpecialChars()
        {
            var strMsg = "Sono ammessi i seguenti caratteri speciali";
            if (this.MinSpecialChar < 0)
                return string.Format("Non {0}", strMsg.ToString().ToLower());
            else if (this.MinSpecialChar == 0)
                return strMsg;
            else
                return $"{strMsg}, almeno {this.MinSpecialChar}";
        }
    }

}