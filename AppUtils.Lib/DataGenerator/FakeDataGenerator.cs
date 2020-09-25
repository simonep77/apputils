using System;
using System.Linq;
using System.Text;

namespace AppUtils.Lib.DataGenerator
{



    /// <summary>

    /// ''' Classe per generare dati fake

    /// ''' </summary>
    public class FakeDataGenerator
    {
        private Random mRand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     ''' Genera i dati di una persona
        ///     ''' </summary>
        ///     ''' <param name="maschio"></param>
        ///     ''' <param name="etaMin"></param>
        ///     ''' <param name="etaMax"></param>
        ///     ''' <param name="codComuneCat"></param>
        ///     ''' <returns></returns>
        public FakePersona GeneraPersona(bool maschio, int etaMin, int etaMax, string codComuneCat)
        {
            FakePersona oPers = new FakePersona();

            try
            {
                oPers.Cognome = FakeData_Static.List_Cognomi[this.mRand.Next(0, FakeData_Static.List_Cognomi.Length)];

                if (maschio)
                {
                    oPers.Sesso = 'M';
                    oPers.Nome = FakeData_Static.List_NomiM[this.mRand.Next(0, FakeData_Static.List_NomiM.Length)];
                }
                else
                {
                    oPers.Sesso = 'F';
                    oPers.Nome = FakeData_Static.List_NomiF[this.mRand.Next(0, FakeData_Static.List_NomiF.Length)];
                }

                // oPers.Cognome = oPers.Cognome
                // oPers.Nome = oPers.Nome
                oPers.Email = string.Concat(oPers.Nome.Replace(" ", "").Replace("'", ""), ".", oPers.Cognome.Replace(" ", "").Replace("'", ""), "@", FakeData_Static.List_DominiEmail[this.mRand.Next(0, FakeData_Static.List_DominiEmail.Length)]).ToLower();

                // Calcola data nascita fake
                DateTime dtMin = new DateTime(DateTime.Today.Year - etaMax, 1, 1);
                DateTime dtMax = new DateTime(DateTime.Today.Year - etaMin, 12, 31);

                int days = (int)Math.Floor(dtMax.Subtract(dtMin).TotalDays + 1);
                var newdays = this.mRand.Next(0, days);

                oPers.DataNascita = dtMin.AddDays(newdays);
                if (string.IsNullOrEmpty(codComuneCat) || codComuneCat.Length != 4)
                    codComuneCat = FakeData_Static.List_Comuni_Cat[this.mRand.Next(0, FakeData_Static.List_Comuni_Cat.Length - 1)];

                oPers.CF = CodiceFiscaleUT.CalcolaCodiceFiscale(oPers.Nome, oPers.Cognome,oPers.DataNascita, oPers.Sesso, codComuneCat);

                return oPers;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Errore nella generazione di {oPers.Cognome} { oPers.Nome} - {oPers.Sesso} - {oPers.DataNascita:dd/MM/yyyy}: {ex.Message}");
            }
        }


        /// <summary>
        ///     ''' Genera i dati di una persona con CF generato automaticamente
        ///     ''' </summary>
        ///     ''' <param name="maschio"></param>
        ///     ''' <param name="etaMin"></param>
        ///     ''' <param name="etaMax"></param>
        ///     ''' <returns></returns>
        public FakePersona GeneraPersona(bool maschio, int etaMin, int etaMax)
        {
            return this.GeneraPersona(maschio, etaMin, etaMax, string.Empty);
        }


        /// <summary>
        ///     ''' Normalizza testo ascii:
        ///     ''' - converte i caratteri di servizio (0-31) in spazi
        ///     ''' - converte accentati in standard
        ///     ''' - elimina il resto
        ///     ''' </summary>
        ///     ''' <param name="input"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public static string NormalizeTextCF(string input)
        {
            if (input == null)
                input = string.Empty;

            StringBuilder sb = new StringBuilder(input.Length);

            for (int index = 0; index <= input.Length - 1; index++)
            {
                int iChar = Convert.ToInt32(input[index]);
                switch (iChar)
                {
                    case object _ when 0 <= iChar && iChar <= 31:
                        {
                            sb.Append(AppUtils.Lib.Common.CharCodes.SPACE);
                            break;
                        }

                    case object _ when 65 <= iChar && iChar <= 90:
                        {
                            sb.Append(input[index]);
                            break;
                        }

                    case object _ when 96 <= iChar && iChar <= 122:
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



    }

}


