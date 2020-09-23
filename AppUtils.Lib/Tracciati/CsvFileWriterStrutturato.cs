using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using AppUtils.Lib.Common;

/// <summary>
///  Classe per scrittura file CSV con base tabellare eventualmente persistente in memoria.
///  Supporto alle transazioni
///  </summary>
///  <remarks></remarks>
public class CsvFileWriterStrutturato : IDisposable
{
    private bool disposedValue = false;        // Per rilevare chiamate ridondanti
    private char mSeparatore;
    private string mNomeFile;
    private Encoding mEncoding;
    private TextWriter mTextWriter;

    private DataTable mMainTable = new DataTable("csv");

    private List<DataRow> mCurrentRowList = new List<DataRow>();
    private DataRow mCurrentRow;

    private bool mHeaderWritten = false;

    private bool mIsInTransaction;






    public bool KeepTableInMemory { get; set; } = false;

    public bool WriteHeader { get; set; } = true;

    /// <summary>
    /// Indica se e' attiva la gestione transazionale
    /// </summary>
    /// <returns></returns>
    public bool IsInTransaction
    {
        get
        {
            return this.mIsInTransaction;
        }
    }




    public CsvFileWriterStrutturato(string filePath) : this(filePath, ',', Encoding.UTF8)
    {
    }

    public CsvFileWriterStrutturato(string filePath, char separatore) : this(filePath, separatore, Encoding.UTF8)
    {
    }

    public CsvFileWriterStrutturato(string filePath, char separatore, Encoding enc)
    {
        this.mSeparatore = separatore;
        this.mNomeFile = filePath;
        this.mEncoding = enc;
        this.mTextWriter = new StreamWriter(filePath, true, enc);
    }


    public void AddColumn(DataColumn col)
    {
        this.mMainTable.Columns.Add(col);
    }

    public void AddColumn(string colName)
    {
        this.AddColumn(colName, -1);
    }

    public void AddColumn(string colName, int maxLength)
    {
        DataColumn oCol = new DataColumn(colName, typeof(string));
        oCol.DefaultValue = string.Empty;

        if (maxLength > 0)
            oCol.MaxLength = maxLength;

        this.mMainTable.Columns.Add(oCol);
    }


    public void AddColumns(IEnumerable<string> cols)
    {
        foreach (string colName in cols)
            this.AddColumn(colName);
    }

    public void AddColumns(IEnumerable<DataColumn> cols)
    {
        foreach (DataColumn col in cols)
            this.AddColumn(col);
    }


    /// <summary>
    /// Avvia transazione
    /// </summary>
    public void BeginTransaction()
    {
        this.transCheck(false);

        this.mIsInTransaction = true;
    }

    /// <summary>
    /// Commit transazione (scrive dati)
    /// </summary>
    public void CommitTransaction()
    {
        this.transCheck(true);

        this.writeOnDisk();

        this.transReset();
    }

    /// <summary>
    /// Rollback transazione (scarta dati)
    /// </summary>
    public void RollbackTransaction()
    {
        this.transCheck(true);

        // Svuota lista rows
        this.mCurrentRowList.Clear();

        this.transReset();
    }

    /// <summary>
    /// Verifica se transazione attiva
    /// </summary>
    private void transCheck(bool state)
    {
        if (this.IsInTransaction != state)
            throw new ApplicationException("Transazione non attiva");
    }

    /// <summary>
    /// Resetta transazione
    /// </summary>
    private void transReset()
    {
        // Disattiva transazione
        this.mIsInTransaction = false;
        // Ricrea row corrente
        this.buildRow();
    }

    /// <summary>
    /// Assicura la valorizzazione della riga corrente
    /// </summary>
    private void ensureRow()
    {
        if (this.mCurrentRow == null)
            this.buildRow();
    }

    private void buildRow()
    {
        this.mCurrentRow = this.mMainTable.NewRow();
    }


    private void writeOnDisk()
    {
        int iLastRow = this.mMainTable.Rows.Count - 1;
        int iLastCol = this.mMainTable.Columns.Count - 1;

        // Scrive Header
        if (this.WriteHeader && !this.mHeaderWritten)
        {
            // Imnposta flag di scrittura header
            this.mHeaderWritten = true;

            foreach (DataColumn item in this.mMainTable.Columns)
            {
                this.mTextWriter.Write(item.ColumnName);

                if (item.Ordinal < iLastCol)
                    this.mTextWriter.Write(this.mSeparatore);
            }
            this.mTextWriter.WriteLine();
        }

        // NEW: carica le row inserite e non consolidate
        foreach (DataRow row in this.mCurrentRowList)
        {
            foreach (DataColumn item in this.mMainTable.Columns)
            {
                this.mTextWriter.Write(row[item]);

                if (item.Ordinal < iLastCol)
                    this.mTextWriter.Write(this.mSeparatore);
            }

            this.mTextWriter.WriteLine();

            // Carica su tabella se impostato che deve essere persistente
            if (this.KeepTableInMemory)
                this.mMainTable.Rows.Add(row);
        }

        // Svuota lista rows
        this.mCurrentRowList.Clear();
    }







    /// <summary>
    /// Se ci sono campi nel buffer scrive il record ed azzera il buffer dei campi
    /// </summary>
    /// <remarks></remarks>
    public void WriteRow()
    {
        this.ensureRow();

        // Aggiunge ad elenco volatili
        this.mCurrentRowList.Add(this.mCurrentRow);

        if (!this.IsInTransaction)
            this.writeOnDisk();

        // Ricrea la row
        this.mCurrentRow = this.mMainTable.NewRow();
    }


    /// <summary>
    /// Scrive una riga vuota senza toccare il contenuto del buffer dei campi
    /// </summary>
    /// <remarks></remarks>
    public void WriteEmptyRow()
    {
        // Salva vecchia current
        var oldRow = this.mCurrentRow;
        // Ricrea la current row 
        this.mCurrentRow = this.mMainTable.NewRow();

        this.WriteRow();

        // Reimposta la vecchia row
        this.mCurrentRow = oldRow;
    }



    /// <summary>
    /// Aggiunge campo verificando la lunghezza
    /// </summary>
    /// <param name="colonna"></param>
    /// <param name="valore"></param>
    /// <remarks></remarks>
    public void SetValue(string colonna, string valore)
    {
        this.ensureRow();

        valore = valore.Trim().Replace(this.mSeparatore, '-'); // Pulisce valore in input

        DataColumn oCol = this.mMainTable.Columns[colonna];

        if (oCol.MaxLength > 0 && valore.Length > oCol.MaxLength)
            valore = valore.Truncate(oCol.MaxLength);

        this.mCurrentRow[oCol] = valore;
    }


    /// <summary>
    /// Ritorna struttura tabellare rappresentante il file
    /// </summary>
    /// <returns></returns>
    public DataView GetDataView()
    {
        return this.mMainTable.DefaultView;
    }

    /// <summary>
    /// Ritorna record corrente
    /// </summary>
    /// <returns></returns>
    public DataRow GetCurrentRowView()
    {
        this.ensureRow();

        return this.mCurrentRow;
    }


    /// <summary>
    /// Esegue Finalizza() per scrivere dati non ancora salvati poi libera memoria
    /// </summary>
    /// <remarks></remarks>
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
                    if (this.IsInTransaction)
                        this.RollbackTransaction();

                    this.writeOnDisk();

                    this.mMainTable.Dispose();
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
