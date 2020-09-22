/// <summary>
/// ''' Rappresenta l'esito di una validazione effettuata
/// ''' </summary>
/// ''' <remarks></remarks>
using System;
using System.Collections.Generic;



/// <summary>
/// Rappresenta l'esito di una verifica
/// </summary>
public class EsitoControllo
{

    /// <summary>
    ///     ''' Indica se la validazione ha avuto successo
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public bool Positivo { get; set; } = true;

    /// <summary>
    ///     ''' Codice numerico addizionale dell'errore
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public int EsitoCodice { get; set; }

    /// <summary>
    ///     ''' Testo relativo alla condizione (di errore o info)
    ///     ''' </summary>
    ///     ''' <value></value>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public string EsitoTesto { get; set; } = string.Empty;


    /// <summary>
    ///     ''' Codice identificativo del controllo effettuato
    ///     ''' </summary>
    ///     ''' <returns></returns>
    public uint TipoControllo { get; set; }


    /// <summary>
    ///     ''' Chiave identificativa dell'entita' relativa al controllo effettuato
    ///     ''' </summary>
    ///     ''' <returns></returns>
    public string ChiaveEntita { get; set; } = string.Empty;

    /// <summary>
    ///     ''' Attributi aggiuntivi legati al controllo
    ///     ''' </summary>
    ///     ''' <returns></returns>
    public Dictionary<string, string> AttributiCustom { get; } = new Dictionary<string, string>();
    
}
