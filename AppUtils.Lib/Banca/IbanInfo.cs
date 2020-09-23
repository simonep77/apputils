/// <summary>
/// Classe che rappresenta i dati di un Iban
/// </summary>
public class IbanInfo
{
    public string IbanCompleto { get; set; } = string.Empty;
    public string IbanPaperFormat { get; set; } = string.Empty;
    public string CodicePaese { get; set; } = string.Empty;
    public string CheckDigit { get; set; } = string.Empty;
    public string CodiceControllo { get; set; } = string.Empty;
    public string CodiceBanca { get; set; } = string.Empty;
    public string CodiceSportello { get; set; } = string.Empty;
    public string NumeroConto { get; set; } = string.Empty;
}
