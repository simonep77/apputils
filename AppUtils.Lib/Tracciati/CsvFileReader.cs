
using System.IO;
using System.Linq;
using System.Data;


/// <summary>

/// ''' Classe per la lettura di file CSV

/// ''' </summary>

/// ''' <remarks></remarks>
public class CsvFileReader
{
    private string mFileName = string.Empty;
    private bool mFirstRowHeader = true;
    private bool mSkipEmptyLines = true;
    private char mSeparatorChar = ';';


    public string FileName
    {
        get
        {
            return this.mFileName;
        }
        set
        {
            this.mFileName = value;
        }
    }

    public bool FirstRowHeader
    {
        get
        {
            return this.mFirstRowHeader;
        }
        set
        {
            this.mFirstRowHeader = value;
        }
    }

    public bool SkipEmptyLines
    {
        get
        {
            return this.mSkipEmptyLines;
        }
        set
        {
            this.mSkipEmptyLines = value;
        }
    }

    public char Separator
    {
        get
        {
            return this.mSeparatorChar;
        }
        set
        {
            this.mSeparatorChar = value;
        }
    }




    /// <summary>
    ///     ''' Legge il contenuto del file e ritorna un datatable che lo rappresenta
    ///     ''' </summary>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    public DataTable Read()
    {
        string line;
        string[] fields;

        using (StreamReader fs = File.OpenText(this.mFileName))
        {
            // Crea tabella
            DataTable tab = new DataTable("TABLE");
            // Legge la prima riga per determinare la struttura della tabella
            if (!fs.EndOfStream)
            {
                // legge
                line = fs.ReadLine();

                // Splitta
                fields = line.Split(this.mSeparatorChar);

                // Crea le colonne
                this.createColumnsFromFields(tab, fields);

                // Se non è header aggiunge la riga
                if (!this.mFirstRowHeader)
                    this.addFieldsToTable(tab, fields);
            }

            // Legge il resto
            while (!fs.EndOfStream)
            {
                // legge
                line = fs.ReadLine();

                // Controlla se da saltare
                if (string.IsNullOrEmpty(line.Trim()) && this.mSkipEmptyLines)
                    continue;

                // Splitta
                fields = line.Split(this.mSeparatorChar);

                // Scrive record
                this.addFieldsToTable(tab, fields);
            }

            // Ritorna tabella creata
            tab.AcceptChanges();

            return tab;
        }
    }




    /// <summary>
    ///     ''' Dato un record, crea la struttura di colonne
    ///     ''' </summary>
    ///     ''' <param name="tab"></param>
    ///     ''' <param name="fields"></param>
    ///     ''' <remarks></remarks>
    private void createColumnsFromFields(DataTable tab, string[] fields)
    {
        DataColumn col;

        for (int i = 0; i <= fields.Length - 1; i++)
        {
            col = new DataColumn();
            col.DataType = typeof(string);
            col.AllowDBNull = false;
            col.DefaultValue = string.Empty;

            // Imposta il nome in base a valori oppure default
            if (this.mFirstRowHeader)
                col.ColumnName = fields[i];
            else
                col.ColumnName = string.Format("Col_{0}", i);

            // aggiunge a tabella
            tab.Columns.Add(col);
        }
    }

    /// <summary>
    ///     ''' Aggiunge un nuovo record alla tabella a partire dai campi forniti
    ///     ''' </summary>
    ///     ''' <param name="tab"></param>
    ///     ''' <param name="fields"></param>
    ///     ''' <remarks></remarks>
    private void addFieldsToTable(DataTable tab, string[] fields)
    {
        DataRow aRow = tab.NewRow();
        int maxFields = tab.Columns.Count < fields.Length ? tab.Columns.Count : fields.Length;

        for (int i = 0; i <= maxFields - 1; i++)
            aRow[i] = fields[i];

        tab.Rows.Add(aRow);
    }
}
