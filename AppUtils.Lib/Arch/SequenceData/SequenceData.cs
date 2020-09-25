using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace AppUtils.Lib.Arch.SequenceData
{

/// <summary>
///  Classe per la gestione centralizzata di generatori di numeri sequenziali
///  con accesso esclusivo.
///  
///  Attenzione i numeri generati non sono regredibili in base alle transazioni (Bruciati)
///  </summary>
///  <remarks></remarks>
public class SequenceData : IDisposable
    {
        private const string STR_CONN_STRING = "SequeceGeneratorConnection";
        private const string STR_SP_DROP = "sp_SEQ_Drop";
        private const string STR_SP_RESET = "sp_SEQ_Reset";
        private const string STR_SP_GETNEXT = "sp_SEQ_GetNextValue";
        private const string STR_SP_PARAM_NAME = "@seqName";

        private DbConnection mConn;
        private string mDbName = string.Empty;


        /// <summary>
        ///  In input una connessione db (aperta o chiusa) da cui viene generata una nuova connessione avulsa da essa (no eventuale transazione)
        ///  </summary>
        ///  <param name="dbConnToClone"></param>
        ///  <remarks></remarks>
        public SequenceData(DbConnection dbConnToClone) : this(dbConnToClone, string.Empty)
        {
        }

        /// <summary>
        ///  In input una connessione db (aperta o chiusa) da cui viene generata una nuova connessione avulsa da essa (no eventuale transazione) ed il nome db da utilizzare (in caso di app muli database)
        ///  </summary>
        ///  <param name="dbConnToClone"></param>
        ///  <remarks></remarks>
        public SequenceData(DbConnection dbConnToClone, string dbName)
        {
            try
            {
                if (!string.IsNullOrEmpty(dbName))
                    this.mDbName = string.Concat(dbName.TrimEnd('.'), ".");
                this.mConn = (DbConnection)((ICloneable)dbConnToClone).Clone();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"SequenceData - Impossibile clonare la connessione fornita in input. Exception: {ex.Message}");
            }
        }


        /// <summary>
        ///  Esegue controlli sulla corretta configurazione dell'ambiente (esegue tutti i metodi con una sequenza temporanea che viene poi eliminata).
        ///  Torna 'OK' in caso di ambiente configurato e test superato, altrimenti il testo dell'errore
        ///  </summary>
        ///  <remarks></remarks>
        public string CheckSequenceEnv()
        {
            // 
            System.Text.StringBuilder sbRet = new System.Text.StringBuilder();
            Random rnd = new Random();
            string testSeq = string.Concat("CHK_", rnd.Next(1, 100000).ToString(), "_", Guid.NewGuid().ToString("N"));

            try
            {
                this.GetNextValue(testSeq);
            }
            catch (Exception ex)
            {
                sbRet.Append("Errore test 'GetNextValue': ");
                sbRet.AppendLine(ex.Message);
            }

            try
            {
                this.ResetSequence(testSeq);
            }
            catch (Exception ex)
            {
                sbRet.Append("Errore test 'ResetSequence': ");
                sbRet.AppendLine(ex.Message);
            }

            try
            {
                this.DropSequence(testSeq);
            }
            catch (Exception ex)
            {
                sbRet.Append("Errore test 'DropSequence': ");
                sbRet.AppendLine(ex.Message);
            }

            if (sbRet.Length == 0)
                sbRet.Append("OK");

            return sbRet.ToString();
        }


        /// <summary>
        ///  Elimina definitivamente una sequenza (DELETE)
        ///  </summary>
        ///  <param name="sequenceName"></param>
        ///  <remarks></remarks>
        public void DropSequence(string sequenceName)
        {
            using (DbCommand oCmd = this.mConn.CreateCommand())
            {
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = string.Concat(this.mDbName, STR_SP_DROP);
                DbParameter oParam = oCmd.CreateParameter();
                oParam.ParameterName = STR_SP_PARAM_NAME;
                oParam.Value = sequenceName;
                oCmd.Parameters.Add(oParam);

                // Esegue
                this.mConn.Open();
                try
                {
                    oCmd.ExecuteNonQuery();
                }
                finally
                {
                    this.mConn.Close();
                }
            }
        }

        /// <summary>
        ///  Resetta una sequenza a Zero. Se non esiste la crea e la imposta a zero
        ///  </summary>
        ///  <param name="sequenceName"></param>
        ///  <remarks></remarks>
        public long ResetSequence(string sequenceName)
        {
            using (DbCommand oCmd = this.mConn.CreateCommand())
            {
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = string.Concat(this.mDbName, STR_SP_RESET);
                DbParameter oParam = oCmd.CreateParameter();
                oParam.ParameterName = STR_SP_PARAM_NAME;
                oParam.Value = sequenceName;
                oCmd.Parameters.Add(oParam);

                // Esegue
                this.mConn.Open();
                try
                {
                    return Convert.ToInt64(oCmd.ExecuteScalar());
                }
                finally
                {
                    this.mConn.Close();
                }
            }
        }


        /// <summary>
        ///  Dato un nome di sequenza ritorna il valore incrementato di 1
        ///  </summary>
        ///  <param name="sequenceName"></param>
        ///  <returns></returns>
        ///  <remarks></remarks>
        public long GetNextValue(string sequenceName)
        {
            using (DbCommand oCmd = this.mConn.CreateCommand())
            {
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = string.Concat(this.mDbName, STR_SP_GETNEXT);
                DbParameter oParam = oCmd.CreateParameter();
                oParam.ParameterName = STR_SP_PARAM_NAME;
                oParam.Value = sequenceName;
                oCmd.Parameters.Add(oParam);

                // Esegue
                this.mConn.Open();
                try
                {
                    return Convert.ToInt64(oCmd.ExecuteScalar());
                }
                finally
                {
                    this.mConn.Close();
                }
            }
        }








        private bool disposedValue; // Per rilevare chiamate ridondanti

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                    // TODO: eliminare stato gestito (oggetti gestiti).
                    this.mConn.Dispose();
            }
            this.disposedValue = true;
        }

        // TODO: eseguire l'override di Finalize() solo se Dispose(ByVal disposing As Boolean) dispone del codice per liberare risorse non gestite.
        // Protected Overrides Sub Finalize()
        // ' Non modificare questo codice. Inserire il codice di pulizia in Dispose(ByVal disposing As Boolean).
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(ByVal disposing As Boolean).
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


}
