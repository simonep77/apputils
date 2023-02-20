using System;
using System.IO;
using System.Text;
using AppUtils.Lib.Common;
using static System.Net.Mime.MediaTypeNames;

namespace AppUtils.Lib.Tracciati
{

    /// <summary>
    ///  Classe per scrittura file con posizioni fisse
    ///  </summary>
    ///  <remarks></remarks>
    public class FixedFileWriter : IDisposable
    {
        public string NomeFile { get; private set; }

        public TextWriter Writer { get; private set; }

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




        public FixedFileWriter(string filePath) : this(filePath, Encoding.UTF8)
        {
        }

        public FixedFileWriter(string filePath, Encoding enc)
        {
            this.NomeFile = filePath;
            this.Writer = new StreamWriter(this.NomeFile, false, enc);
        }


        /// <summary>
        ///  Avvia transazione
        ///  </summary>
        public void BeginTransaction()
        {
            if (this.IsInTransaction)
                throw new ApplicationException("Transazione già attiva");

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
            this.mBufferSb.Clear();
        }

        /// <summary>
        ///  Scrive il buffer
        ///  </summary>
        private void writeBuffer()
        {
            if (this.mBufferSb.Length == 0)
                return;

            this.Writer.Write(this.mBufferSb.ToString());
            this.mBufferSb.Length = 0;
        }


        /// <summary>
        ///  Se ci sono campi nel buffer scrive il record ed azzera il buffer dei campi
        ///  </summary>
        ///  <remarks></remarks>
        public void ScriviRecord()
        {
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
        /// Aggiunge valore alla posizione specificata e per la lunghezza definita
        /// </summary>
        /// <param name="valore"></param>
        /// <param name="pos"></param>
        /// <param name="len"></param>
        /// <param name="padchar"></param>
        /// <param name="alignRight"></param>
        public void AggiungiCampo(int pos, int len, string valore, char padchar = ' ', bool alignRight = false)
        {
            //Verifica se necessario estendere il record
            if (this.mBufferSb.Length < pos + len)
            {
                var ext = (pos + len) - this.mBufferSb.Length;
                this.mBufferSb.Append(" ".PadRight(ext, ' '));
            }
            
            //Prepara lo spazio per inserire il nuovo valore
            this.mBufferSb.Remove(pos, len);
            if (!alignRight)
                this.mBufferSb.Insert(pos, valore.PadRight(len, padchar).Substring(0, len));
            else
                this.mBufferSb.Insert(pos, valore.PadLeft(len, padchar).Substring(0, len));

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

        private bool disposedValue = false;        // Per rilevare chiamate ridondanti

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
                        this.Writer.Dispose();
                    }
                }
            }
            this.disposedValue = true;
        }
    }

}