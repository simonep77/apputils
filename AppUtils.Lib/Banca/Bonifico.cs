
using System;

namespace AppUtils.Lib.Banca
{
    /// <summary>
    /// ''' Identifica le informazioni relative ad un bonifico
    /// ''' </summary>
    /// ''' <remarks></remarks>
    public class Bonifico
    {
        public string Nominativo { get; set; } = string.Empty;

        public string CodiceBeneficiario { get; set; } = string.Empty;

        public string DescrizioneOperazione { get; set; } = string.Empty;

        public DateTime DataValuta { get; set; }

        public decimal Importo { get; set; }

        public Iban Iban { get; set; }

        public string Bic { get; set; } = string.Empty;

        public RecapitoPostale Recapito { get; set; } = new RecapitoPostale();

        public string CodiceFiscaleBeneficiario { get; set; } = string.Empty;

        public string IdentificativoOperazione { get; set; } = string.Empty;
    }
}
