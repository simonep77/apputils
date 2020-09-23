using System;
using System.IO;
using System.Text;
using AppUtils.Lib.Common;

/// <summary>
///  Classe per scrittura file CSV
///  </summary>
///  <remarks></remarks>
public class CsvFileWriter : IDisposable
{
    private bool disposedValue = false;        // Per rilevare chiamate ridondanti
    private char mSeparatore;
    private string mNomeFile;
    private Encoding mEncoding;
    private TextWriter mTextWriter;
    private const char C_MENO = '-';

    private StringBuilder mBufferSb = new StringBuilder(2000);
    private bool mIsInTransaction;


    /// <summary>
    ///  Indica se e' attiva la gestione transazionale
    ///  </summary>
    ///  <returns></returns>
    public bool IsInTransaction
    {
        get
        {
            return this.mIsInTransaction;
        }
    }




    public CsvFileWriter(string filePath) : this(filePath, ',', Encoding.UTF8)
    {
    }

    public CsvFileWriter(string filePath, char separatore) : this(filePath, separatore, Encoding.UTF8)
    {
    }

    public CsvFileWriter(string filePath, char separatore, Encoding enc)
    {
        if (separatore == C_MENO)
            throw new ArgumentException(string.Format("Il carattere {0} non e' utilizzabile in quanto riservato per usi interni", C_MENO));

        this.mSeparatore = separatore;
        this.mNomeFile = filePath;
        this.mEncoding = enc;
        mTextWriter = new StreamWriter(filePath, true, enc);
    }


    /// <summary>
    ///  Avvia transazione
    ///  </summary>
    public void BeginTransaction()
    {
        this.mIsInTransaction = true;
    }

    /// <summary>
    ///  Commit transazione (scrive dati)
    ///  </summary>
    public void CommitTransaction()
    {
        this.transCheck();

        this.writeBuffer();

        this.transReset();
    }

    /// <summary>
    ///  Rollback transazione (scarta dati)
    ///  </summary>
    public void RollbackTransaction()
    {
        this.transCheck();

        this.transReset();
    }

    /// <summary>
    ///  Verifica se transazione attiva
    ///  </summary>
    private void transCheck()
    {
        if (!this.IsInTransaction)
            throw new ApplicationException("Transazione non attiva");
    }

    /// <summary>
    ///  Resetta transazione
    ///  </summary>
    private void transReset()
    {
        // Disattiva transazione
        this.mIsInTransaction = false;
        // Resetta dati in cache
        this.mBufferSb.Length = 0;
    }

    /// <summary>
    ///  Scrive il buffer
    ///  </summary>
    private void writeBuffer()
    {
        if (this.mBufferSb.Length == 0)
            return;

        this.mTextWriter.Write(this.mBufferSb.ToString());
        this.mBufferSb.Length = 0;
    }






    /// <summary>
    ///  Se ci sono campi nel buffer scrive il record ed azzera il buffer dei campi
    ///  </summary>
    ///  <remarks></remarks>
    public void ScriviRecord()
    {

        // Elimina eventualmente la virgola finale
        if (this.mBufferSb.Length > 0 && this.mBufferSb[this.mBufferSb.Length - 1] == this.mSeparatore)
            this.mBufferSb.Remove(this.mBufferSb.Length - 1, 1);

        // Scrive la entry
        this.ScriviRecordVuoto();
    }


    /// <summary>
    ///  Scrive una riga vuota senza toccare il contenuto del buffer dei campi
    ///  </summary>
    ///  <remarks></remarks>
    public void ScriviRecordVuoto()
    {
        this.mBufferSb.AppendLine();

        if (!this.IsInTransaction)
            this.writeBuffer();
    }


    /// <summary>
    ///  Aggiunge campo senza verificare la lunghezza
    ///  </summary>
    ///  <param name="valore"></param>
    ///  <remarks></remarks>
    public void AggiungiCampo(string valore)
    {
        this.AggiungiCampo(valore, -1);
    }


    /// <summary>
    ///  Aggiunge campo verificando la lunghezza
    ///  </summary>
    ///  <param name="valore"></param>
    ///  <param name="maxLen"></param>
    ///  <remarks></remarks>
    public void AggiungiCampo(string valore, int maxLen)
    {
        valore = valore.Trim().Replace(this.mSeparatore, C_MENO); // Pulisce valore in input
        if (maxLen != -1 && valore.Length > maxLen)
            valore = valore.Truncate(maxLen);

        this.mBufferSb.Append(valore);
        this.mBufferSb.Append(this.mSeparatore);
    }



    /// <summary>
    ///  Esegue Finalizza() per scrivere dati non ancora salvati poi libera memoria
    ///  </summary>
    ///  <remarks></remarks>
    public void Dispose()
    {
        
        // Non modificare questo codice. Inserire il codice di pulitura in Dispose(ByVal disposing As Boolean).
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Scrive Ultima parte
                try
                {
                    this.writeBuffer();
                }
                finally
                {
                    this.mTextWriter.Dispose();
                }
            }
        }
        this.disposedValue = true;
    }
}
