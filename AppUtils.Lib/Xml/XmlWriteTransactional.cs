using System;
using System.IO;
using System.Text;
using System.Xml;

/// <summary>
///  Classe utility per scrivere xml con un solo oggetto in transazione
///  </summary>
///  <remarks></remarks>
public class XmlWriteTransactional : IDisposable
{
    private XmlTextWriter _xmlWriter = new XmlTextWriter(new StringWriter());

    private XmlTextWriter _xmlWriterTrans;

    private XmlTextWriter _xmlCurrentWriter;

    private int _totNumCommit = 0;
    private int _totNumRollback = 0;
    private bool _isInTransaction = false;

    public bool Indent { get; set; } = false;

    /// <summary>
    ///  Indica se e' attiva la gestione transazionale
    ///  </summary>
    ///  <returns></returns>
    public bool IsInTransaction
    {
        get
        {
            return _isInTransaction;
        }
    }

    public int TotNumCommit
    {
        get
        {
            return _totNumCommit;
        }
    }

    public int TotNumRollback
    {
        get
        {
            return _totNumRollback;
        }
    }



    public XmlWriteTransactional()
    {
        this.transReset();

    }



    /// <summary>
    ///  Avvia transazione
    ///  </summary>
    public void BeginTransaction()
    {
        if (this._isInTransaction)
            throw new ApplicationException("Transazione già attiva");

        this._isInTransaction = true;
        this._xmlWriterTrans = new XmlTextWriter(new StringWriter());
        this._xmlCurrentWriter = this._xmlWriterTrans;
    }

    /// <summary>
    ///  Commit transazione (scrive dati)
    ///  </summary>
    public void CommitTransaction()
    {
        //Verifica in transazione
        this.transCheck();

        //Imposta il writer base con il nuovo transazionale
        this._xmlWriter = this._xmlWriterTrans;

        //reimposta transazione
        this.transReset();

        this._totNumCommit += 1;
    }

    /// <summary>
    ///  Rollback transazione (scarta dati)
    ///  </summary>
    public void RollbackTransaction()
    {
        //Verifica in transazione
        this.transCheck();

        //Chiude writer transazione
        this._xmlWriterTrans.Close();

        //Ripristina stato
        this.transReset();

        //Aggiorna counter
        this._totNumRollback += 1;
    }

    /// <summary>
    ///  Verifica se transazione attiva
    ///  </summary>
    private void transCheck()
    {
        if (!IsInTransaction)
            throw new ApplicationException("Transazione non attiva");
    }

    /// <summary>
    ///  Resetta transazione
    ///  </summary>
    private void transReset()
    {
        // Disattiva transazione
        _isInTransaction = false;

        //Reimposta copia corrente su base
        this._xmlCurrentWriter = this._xmlWriter;

        // Resetta dati in cache transazione
        _xmlWriterTrans = null; ;
    }


    public void WriteStartElement(string nomeElemento)
    {
        this._xmlCurrentWriter.WriteStartElement(nomeElemento);
    }

    public void WriteEndElement()
    {
        this._xmlCurrentWriter.WriteEndElement();
    }

    public void WriteEmptyElement(string nomeElemento)
    {
        this._xmlCurrentWriter.WriteElementString(nomeElemento, string.Empty);
       
    }

    public virtual void WriteElementString(string nomeElemento, string valore)
    {
        this._xmlCurrentWriter.WriteElementString(nomeElemento, valore);
    }

    public virtual void WriteElementStringCDATA(string nomeElemento, string valore)
    {
        this._xmlCurrentWriter.WriteStartElement(nomeElemento);
        this._xmlCurrentWriter.WriteCData(valore);
        this._xmlCurrentWriter.WriteEndElement();
        
    }

    public void WriteRaw(string rawXml)
    {
        this._xmlCurrentWriter.WriteRaw(rawXml);
    }

    public void WriteAttributeString(string nome, string valore)
    {
        this._xmlCurrentWriter.WriteAttributeString(nome, valore);
    }

    public void WriteBase64(byte[] buffer)
    {
        this._xmlCurrentWriter.WriteBase64(buffer, 0, buffer.Length);
    }

    public void WriteCData(string valore)
    {
        this._xmlCurrentWriter.WriteCData(valore);
    }

    public void WriteComment(string valore)
    {
        this._xmlCurrentWriter.WriteComment(valore);
    }

    public void WriteValue(object value)
    {
        this._xmlCurrentWriter.WriteValue(value);
    }


    /// <summary>
    ///  Cancella le strutture interne, il writer NON è più utilizzabile e va creata una nuova istanza
    ///  </summary>
    public void Dispose()
    {
        if (this.IsInTransaction)
            this.RollbackTransaction();

        this._xmlWriter.Close();
        
    }


    /// <summary>
    /// Ritorna testo XML
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        this._xmlCurrentWriter.Flush();
        return this._xmlCurrentWriter.BaseStream.ToString();
    }


}
