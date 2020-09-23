using System;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
///     ''' Logger asincrono
///     ''' </summary>
public class AsyncFileLogger : IDisposable
{
    private long mStarted;
    private object mSync = new object();
    private string mFileFormatPath;
    private StringBuilder mLogBuffer;
    private int mWriteEverySec = 15;
    private Thread mThread;
    private ManualResetEvent mThreadWait = new ManualResetEvent(true);




    /// <summary>
    ///     ''' Registra un log da gestire
    ///     ''' </summary>
    ///     ''' <param name="fileFormatPath"></param>
    public AsyncFileLogger(string fileFormatPath, int writeEverySec)
    {
        this.mWriteEverySec = writeEverySec;
        // Controlla path
        if (string.IsNullOrEmpty(fileFormatPath))
            throw new ArgumentException("File di log non fornito.");

        // Crea directory
        Directory.CreateDirectory(Path.GetDirectoryName(fileFormatPath));

        // Aggiunge
        this.mFileFormatPath = fileFormatPath;
        this.renewBuffer();
    }

    /// <summary>
    ///     ''' Inizia scrittura esclusiva
    ///     ''' </summary>
    public void BeginWrite()
    {
        Monitor.Enter(this.mSync);
    }

    /// <summary>
    ///     ''' Termina scrittura esclusiva
    ///     ''' </summary>
    public void EndWrite()
    {
        Monitor.Exit(this.mSync);
    }


    /// <summary>
    ///     ''' Accoda record log
    ///     ''' </summary>
    ///     ''' <param name="msgFmt"></param>
    ///     ''' <param name="args"></param>
    public void WriteMessage(string msgFmt, params object[] args)
    {
        // Verifica flag avviato
        if (Interlocked.Read(ref this.mStarted) == 0)
            return;

        string sMsgBase = string.Concat(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " - T", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("D4"), " - ", string.Format(msgFmt, args));

        lock (this.mSync)
            this.mLogBuffer.AppendLine(sMsgBase);
    }

    /// <summary>
    ///     ''' Scrive eccezione
    ///     ''' </summary>
    ///     ''' <param name="e"></param>
    public void WriteException(Exception e)
    {
        // Verifica flag avviato
        if (Interlocked.Read(ref this.mStarted) == 0)
            return;

        // Crea il blocco da scrivere
        StringBuilder sb = new StringBuilder(2000);
        // Scrive
        Exception oException;
        int iIndentEx = 0;
        int iInnerCount = 0;
        string sSep = string.Empty.PadRight(210, '=');
        string sMsgBase = string.Concat(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " - T", System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("D4"), " - ");

        sb.Append(sMsgBase);
        sb.AppendLine(sSep);

        oException = e;

        while (oException != null)
        {
            string sIndent = string.Empty.PadRight(iIndentEx);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            if (iIndentEx == 0)
                sb.AppendLine("ECCEZIONE!");
            else
            {
                sb.Append(iInnerCount.ToString("D2"));
                sb.AppendLine(") Inner");
            }

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Tipo     : ");
            sb.AppendLine(oException.GetType().Name);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Messaggio: ");
            sb.AppendLine(oException.Message);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Source   : ");
            sb.AppendLine(oException.Source);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Classe   : ");
            sb.AppendLine(oException.TargetSite.DeclaringType.Name);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Metodo   : ");
            sb.AppendLine(oException.TargetSite.Name);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Namespace: ");
            sb.AppendLine(oException.TargetSite.DeclaringType.Namespace);

            sb.Append(sMsgBase);
            sb.Append(sIndent);
            sb.Append("  * Stack    : ");
            sb.AppendLine(oException.StackTrace);

            // Successiva
            iInnerCount += 1;
            oException = oException.InnerException;
            iIndentEx += 4;
        }

        sb.Append(sMsgBase);
        sb.AppendLine(sSep);

        // Il lock avviene solo sull'append globale
        lock (this.mSync)
            this.mLogBuffer.Append(sb.ToString());
    }


    /// <summary>
    ///     ''' Avvia logging asincrono
    ///     ''' </summary>
    public void Start()
    {
        // check avviato
        if (Interlocked.Read(ref this.mStarted) > 0)
            return;

        // Imposta flag on
        Interlocked.Exchange(ref this.mStarted, 1);

        // Imposta blocco termine
        this.mThreadWait.Reset();

        // avvia thread di gestione
        this.mThread = new Thread(new ThreadStart(this.handleLogs));
        this.mThread.Start();
    }

    /// <summary>
    ///     ''' Chiude il log
    ///     ''' </summary>
    public void Stop()
    {
        // check avviato
        if (Interlocked.Read(ref this.mStarted) == 0)
            return;

        // Imposta flag off
        Interlocked.Exchange(ref this.mStarted, 0);

        // Consente al thread di gestione di concludere
        this.mThreadWait.Set();
        this.mThread.Join();
        this.mThread = null;
    }



    /// <summary>
    ///     ''' Crea un nuovo buffer per il log
    ///     ''' </summary>
    private void renewBuffer()
    {
        this.mLogBuffer = new StringBuilder(2000);
    }

    /// <summary>
    ///     ''' Loop principale di gestione dei log
    ///     ''' </summary>
    private void handleLogs()
    {
        while (Interlocked.Read(ref this.mStarted) > 0)
        {
            // Attende
            this.mThreadWait.WaitOne(this.mWriteEverySec * 1000);

            // Flush logs
            this.flushLogs();
        }
    }

    /// <summary>
    ///     ''' Scrive log su file
    ///     ''' </summary>
    private void flushLogs()
    {
        StringBuilder sb;
        // Esegue lock per liberare il buffer
        lock (this.mSync)
        {
            // Se vuoto esce
            if (this.mLogBuffer.Length == 0)
                return;

            // Copia il riferimento al buffer
            sb = this.mLogBuffer;
            this.renewBuffer();
        }

        // Ok, scrive (se il nome del file fornito comprende paramtri data allora procede alla sostituzione)
        DateTime dtNow = DateTime.Now;
        int iRetry = 3;

        string sFileName = string.Format(this.mFileFormatPath, dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);

        // Crea directory se non esiste (utile in caso di formati nomefile di log comprensivi di info dinamiche directory)
        Directory.CreateDirectory(Path.GetDirectoryName(sFileName));

        // Scrive
        File.AppendAllText(sFileName, sb.ToString());

        // Svuota buffer
        sb.Length = 0;
        sb.Capacity = 0;
    }




    // Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
    public void Dispose()
    {
        this.Stop();
    }
}
