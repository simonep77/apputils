using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using AppUtils.Lib.Common;
using AppUtils.Lib.Web;

namespace AppUtils.Lib.Arch.FileSystem
{

        /// <summary>
        ///     ''' Classe Wrapper per la gestione di file remoti
        ///     ''' su Http File Server
        ///     ''' </summary>
        public class FileSystemHFS : FileSystemBase
        {

            /// <summary>
            ///  Costanti
            ///  </summary>
            private sealed class FS
            {
                public const string HEADER_STATUS_CODE = "HFS-STATUS-CODE";
                public const string HEADER_STATUS_MSG = "HFS-STATUS-MSG";

                public const string ACTION_EXIST = "exist";
                public const string ACTION_READ = "read";
                public const string ACTION_WRITE = "write";
                public const string ACTION_APPEND = "append";
                public const string ACTION_MOVE = "move";
                public const string ACTION_COPY = "copy";
                public const string ACTION_DELETE = "delete";
                public const string ACTION_INFO = "info";
                public const string ACTION_IMAGEINFO = "imageinfo";
                public const string ACTION_EMAIL = "email";
                public const string ACTION_TOUCH = "touch";
                public const string ACTION_SETATTR = "setattr";
                public const string ACTION_HASHSHA1 = "hashsha1";
                public const string ACTION_GETLINK = "getlink";
                public const string ACTION_MKDIR = "mkdir";
                public const string ACTION_RMDIR = "rmdir";
                public const string ACTION_CPDIR = "cpdir";
                public const string ACTION_MVDIR = "mvdir";
                public const string ACTION_QUOTADIR = "quotadir";
                public const string ACTION_LISTF = "listf";
                public const string ACTION_LISTD = "listd";
                public const string ACTION_EXISTDIR = "existdir";
                public const string ACTION_LISTVFS = "listvfs";
                public const string ACTION_RELOADVFS = "reloadvfs";
                public const string ACTION_RESTART = "restart";
                public const string ACTION_CONVERT_TYPE_SWF = "swf";
                public const string ACTION_CONVERT_TYPE_PDF = "pdf";

                public const string PARAM_ACTION = "hfs-action";
                public const string PARAM_USER = "hfs-user";
                public const string PARAM_PASS = "hfs-pass";
                public const string PARAM_VPATH = "hfs-vpath";
                public const string PARAM_VPATHDEST = "hfs-vpathdest";
                public const string PARAM_ATTR = "hfs-attr";
                public const string PARAM_PATTERN = "hfs-pattern";
                public const string PARAM_LINK = "hfs-link";
                public const string PARAM_MAILTO = "hfs-mailto";
                public const string PARAM_MAILFROM = "hfs-mailfrom";
                public const string PARAM_MAILSUBJ = "hfs-mailsubj";
                public const string PARAM_MAILBODY = "hfs-mailbody";

                public const string VPATH_TEMP = "/temp";

                public const char VPATH_DIR_SEP = '/';
                public const char VPATH_DIR_SEP_WIN = '\\';

                public const int MAX_SINGLE_FILE_SIZE = 2097152;
            }

            /// <summary>
            ///  Elenco parametri per chiamate http
            ///  </summary>
            private class ParamList : Dictionary<string, string>
            {
            }


            private string mLastRespMsg;
            private int mLastRespCode;
            private ParamList mParams;
            private string mPass;



            /// <summary>
            ///  Ultimo Codice Risposta
            ///  </summary>
            public override int LastRespCode
            {
                get
                {
                    return this.mLastRespCode;
                }
            }

            /// <summary>
            ///  Ultimo Messaggio Risposta
            ///  </summary>
            public override string LastRespMsg
            {
                get
                {
                    return this.mLastRespMsg;
                }
            }

            /// <summary>
            ///  Directory corrente
            ///  </summary>
            ///  <value></value>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public override string CurrentDirectory
            {
                get
                {
                    return base.CurrentDirectory;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                        base.CurrentDirectory = string.Empty;
                    else
                        base.CurrentDirectory = string.Concat(FS.VPATH_DIR_SEP, value.Replace(FS.VPATH_DIR_SEP_WIN, FS.VPATH_DIR_SEP).Trim(FS.VPATH_DIR_SEP));
                }
            }




            /// <summary>
            ///  Costruttore con specifica di credenziali
            ///  </summary>
            ///  <param name="url"></param>
            ///  <param name="user"></param>
            ///  <param name="pass"></param>
            public FileSystemHFS(string url, string user, string pass)
            {
                this.mUrl = url;
                this.mParams = new ParamList();
                if (!string.IsNullOrEmpty(user))
                {
                    this.mUser = user;
                    this.mPass = pass;
                }
            }




            /// <summary>
            ///  Controlla esistenza file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            public override bool FileExist(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_EXIST);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                string sRet = Encoding.ASCII.GetString(this.sendRequest(this.mParams));
                // ritorna valore
                return sRet.Equals("1");
            }

            /// <summary>
            ///  Ritorna informazioni su file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            public override FSFileInfo FileGetInfo(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_INFO);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                string sRet = Encoding.UTF8.GetString(this.sendRequest(this.mParams));
                // Controlla risposta
                // Carica dati da xml
                using (DataSet ds = new DataSet())
                {
                    using (StringReader sr = new StringReader(sRet))
                    {
                        ds.ReadXml(sr);
                    }
                    using (DataTable tab = ds.Tables["file"])
                    {
                        return this.getFileInfoFromRow(tab.Rows[0]);
                    }
                }
            }

            /// <summary>
            ///  Ritorna informazioni su file Immagine remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public override string FileGetImageInfo(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_IMAGEINFO);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                string sRet = Encoding.UTF8.GetString(this.sendRequest(this.mParams));
                return sRet;
            }





            public override string FileGetLink(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_GETLINK);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                // ritorna valore
                return Encoding.ASCII.GetString(this.sendRequest(this.mParams));
            }

            /// <summary>
            ///  Elimina file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileDelete(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_DELETE);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.sendRequest(this.mParams);
            }

            /// <summary>
            ///  Invia file remoto per email
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileEmail(string vpath, string to, string from, string subj, string body)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_EMAIL);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_MAILTO, to);
                this.mParams.Add(FS.PARAM_MAILFROM, from);
                this.mParams.Add(FS.PARAM_MAILSUBJ, subj);
                this.mParams.Add(FS.PARAM_MAILBODY, body);
                this.sendRequest(this.mParams);
            }


            /// <summary>
            ///  Elimina file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="attr"></param>
            ///  <returns></returns>
            public override byte[] FileReadToBuffer(string vpath, string attr)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_READ);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_ATTR, attr);

                byte[] oBuffer = this.sendRequest(this.mParams);
                // Ritorna buffer
                return oBuffer;
            }

            /// <summary>
            ///  legge file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override byte[] FileReadToBuffer(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_READ);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                byte[] oBuffer = this.sendRequest(this.mParams);
                // Ritorna buffer
                return oBuffer;
            }

            /// <summary>
            ///  legge file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileReadToDisk(string vpath, string localFile)
            {
                this.checkInputFile(localFile, false);
                // Scrive file
                File.WriteAllBytes(localFile, this.FileReadToBuffer(vpath));
            }

            /// <summary>
            ///  Legge file su stream fornito in input
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="stream"></param>
            public override void FileReadToStream(string vpath, Stream stream)
            {
                this.checkStream(stream, false, true, false);
                byte[] oBuffer = this.FileReadToBuffer(vpath);
                // Scrive stream
                stream.Write(oBuffer, 0, oBuffer.Length);
            }


            /// <summary>
            ///  Scrive file con dati da buffer
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="buffer"></param>
            public override void FileWriteFromBuffer(string vpath, byte[] buffer)
            {
                using (var ms = new MemoryStream(buffer))
                {
                    ms.Position = 0;
                    this.FileWriteFromStream(vpath, ms);
                }
                
            }


            /// <summary>
            ///  Scrive file partendo da file fisico
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="localFile"></param>
            public override void FileWriteFromDisk(string vpath, string localFile)
            {
                // Esegue invio a blocchi
                using (FileStream oFs = File.OpenRead(localFile))
                {
                    this.FileWriteFromStream(vpath, oFs);
                }
        }


            /// <summary>
            ///  Scrive file a partire da uno stream
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="stream">
            ///  stream con posibilità di read e seek
            ///  </param>
            public override void FileWriteFromStream(string vpath, Stream stream)
            {
                this.checkStream(stream, true, false, true);
                byte[] buffer;

                if (stream.Length <= FS.MAX_SINGLE_FILE_SIZE)
                {
                    // Si sposta all'inizio
                    stream.Seek(0, SeekOrigin.Begin);
                    // Legge su buffer
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    // Scrive buffer
                    this.FileWriteFromBuffer(vpath, buffer);
                    // Annulla puntatore
                    buffer = null;
                }
                else
                {
                    buffer = new byte[FS.MAX_SINGLE_FILE_SIZE];
                    // Esegue invio a blocchi
                    string sTempName = string.Concat(FS.VPATH_TEMP, "/", Environment.MachineName, "_", Guid.NewGuid().ToString(), Path.GetExtension(vpath));
                    int iDataLen = (int)stream.Length;
                    int iTotRead = 0;
                    int iRead = 0;
                    // Crea temporaneo
                    this.FileTouch(sTempName);

                    while (iDataLen > iTotRead)
                    {
                        // Legge file
                        iRead = stream.Read(buffer, 0, Math.Min(iDataLen, FS.MAX_SINGLE_FILE_SIZE));
                        iTotRead += iRead;
                        // Appende blocco al file temporaneo
                        this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_APPEND);
                        this.mParams.Add(FS.PARAM_VPATH, sTempName);
                        this.sendRequest(this.mParams, buffer, 0, iRead);
                    }


                    // Cancella eventuale file gia' presente
                    this.FileDelete(vpath);
                    // Sposta temporaneo su file finale
                    this.FileMove(sTempName, vpath);
                }
            }


            /// <summary>
            ///  Append file con dati da buffer
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="buffer"></param>
            public override void FileAppendFromBuffer(string vpath, byte[] buffer)
            {
                this.checkBuffer(buffer);
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_APPEND);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.sendRequest(this.mParams, buffer, 0, buffer.Length);
            }


            /// <summary>
            ///  Copia file remoto su altro remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileCopy(string vpath, string vpathdest)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_COPY);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_VPATHDEST, vpathdest);
                this.sendRequest(this.mParams);
            }

            /// <summary>
            ///  Sposta file remoto su altro remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileMove(string vpath, string vpathdest)
            {
                // Controllo path
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_MOVE);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_VPATHDEST, vpathdest);
                this.sendRequest(this.mParams);
            }


            /// <summary>
            ///  Crea o azzera file
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void FileTouch(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_TOUCH);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.sendRequest(this.mParams);
            }


            /// <summary>
            ///  Elenca ROOT directory a cui l'utente ha accessoe
            ///  </summary>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public override FSDirInfo[] ListRootEntries()
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_LISTVFS);
                string sRet = Encoding.UTF8.GetString(this.sendRequest(this.mParams));
                // Carica dati da xml
                using (DataSet ds = new DataSet())
                {
                    using (StringReader sr = new StringReader(sRet))
                    {
                        ds.ReadXml(sr);
                    }
                    DataTable tab = ds.Tables["path"];
                    int iCount = (tab != null) ? tab.Rows.Count : 0;

                    FSDirInfo[] oList = new FSDirInfo[iCount - 1 + 1];
                    using (tab)
                    {
                        DataRow oRow;
                        for (int i = 0; i <= iCount - 1; i++)
                        {
                            oRow = tab.Rows[i];
                            oList[i] = new FSDirInfo(oRow["name"].ToString(), DateTime.MinValue, new FSPermissionHFS(oRow["access"].ToString()));
                        }
                    }
                    // Ritorna lista
                    return oList;
                }
            }


            /// <summary>
            ///  Calcola Hash SHA1 di un file remoto
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public override string FileHashSHA1(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_HASHSHA1);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                return Encoding.ASCII.GetString(this.sendRequest(this.mParams));
            }


            /// <summary>
            ///  Calcola Hash SHA1 di un file locale
            ///  </summary>
            ///  <param name="localfile"></param>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public override string FileLocalHashSHA1(string localfile)
            {
                using (FileStream oFs = File.OpenRead(localfile))
                {
                    using (System.Security.Cryptography.SHA1 oSha1 = new System.Security.Cryptography.SHA1Managed())
                    {
                        return Convert.ToBase64String(oSha1.ComputeHash(oFs));
                    }
                }
            }


            /// <summary>
            ///  Esegue una richiesta HFS con parametri liberi
            ///  </summary>
            ///  <param name="args"></param>
            ///  <returns></returns>
            public byte[] SendParamRequest(params string[] args)
            {
                if (args == null || args.Length == 0 || ((args.Length % 2) != 0))
                    throw new ArgumentException("Il metodo accetta solo coppie di tipo chiave/valore");

                ParamList oParams = new ParamList();

                string sKey = string.Empty;

                foreach (string arg in args)
                {
                    if (string.IsNullOrEmpty(sKey))
                        sKey = arg;
                    else
                    {
                        oParams.Add(sKey, arg);
                        sKey = string.Empty;
                    }
                }

                // Imposta user e pass se non presenti
                if (!string.IsNullOrEmpty(this.mUser))
                {
                    if (!oParams.ContainsKey(FS.PARAM_USER))
                        oParams[FS.PARAM_USER] = this.mUser;
                    if (!oParams.ContainsKey(FS.PARAM_PASS))
                        oParams[FS.PARAM_PASS] = this.mPass;
                }

                return this.sendRequest(oParams);
            }




            /// <summary>
            ///  Crea directory
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void DirectoryCreate(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_MKDIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.sendRequest(this.mParams);
            }


            public override bool DirectoryExist(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_EXISTDIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                string sRet = Encoding.ASCII.GetString(this.sendRequest(this.mParams));
                // ritorna valore
                return sRet.Equals("1");
            }


            /// <summary>
            ///  Elimina directory e tutto il suo contenuto
            ///  </summary>
            ///  <param name="vpath"></param>
            public override void DirectoryDelete(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_RMDIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.sendRequest(this.mParams);
            }

            /// <summary>
            ///  Sposta/Rinomina Directory
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="vpathdest"></param>
            public override void DirectoryMove(string vpath, string vpathdest)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_MVDIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_VPATHDEST, vpathdest);
                this.sendRequest(this.mParams);
            }

            /// <summary>
            ///  Copia Directory su altra
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <param name="vpathdest"></param>
            public override void DirectoryCopy(string vpath, string vpathdest)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_CPDIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_VPATHDEST, vpathdest);
                this.sendRequest(this.mParams);
            }

            /// <summary>
            ///  Ritorna la dimensione del contenuto di una directory e di tutte le sue sottodirectory
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public long DirectoryQuota(string vpath)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_QUOTADIR);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                return Convert.ToInt64(Encoding.ASCII.GetString(this.sendRequest(this.mParams)));
            }

            public override FSFileInfo[] DirectoryListFiles(string vpath)
            {
                return this.DirectoryListFilesFilter(vpath, string.Empty, false);
            }

            /// <summary>
            ///  Elenca files presenti in directory remota
            ///  </summary>
            ///  <param name="vpath"></param>
            public override FSFileInfo[] DirectoryListFilesFilter(string vpath, string pattern, bool recursive)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_LISTF);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_PATTERN, pattern);
                if (recursive)
                    this.mParams.Add(FS.PARAM_ATTR, "R");
                string sRet = Encoding.UTF8.GetString(this.sendRequest(this.mParams));
                // Carica dati da xml
                using (DataSet ds = new DataSet())
                {
                    using (StringReader sr = new StringReader(sRet))
                    {
                        ds.ReadXml(sr);
                    }
                    DataTable tab = ds.Tables["file"];
                    int iFiles = (tab != null) ? tab.Rows.Count : 0;
                    FSFileInfo[] oList = new FSFileInfo[iFiles - 1 + 1];
                    using (tab)
                        for (int i = 0; i <= iFiles - 1; i++)
                            oList[i] = this.getFileInfoFromRow(tab.Rows[i]);
                    // Ritorna lista
                    return oList;
                }
            }


            public override FSDirInfo[] DirectoryListSubDir(string vpath)
            {
                return this.DirectoryListSubDirFilter(vpath, string.Empty, false);
            }

            /// <summary>
            ///  Elenca files presenti in directory remota
            ///  </summary>
            ///  <param name="vpath"></param>
            public override FSDirInfo[] DirectoryListSubDirFilter(string vpath, string pattern, bool recursive)
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_LISTD);
                this.mParams.Add(FS.PARAM_VPATH, vpath);
                this.mParams.Add(FS.PARAM_PATTERN, pattern);
                if (recursive)
                    this.mParams.Add(FS.PARAM_ATTR, "R");
                string sRet = Encoding.UTF8.GetString(this.sendRequest(this.mParams));

                // Carica dati da xml
                using (DataSet ds = new DataSet())
                {
                    using (StringReader sr = new StringReader(sRet))
                    {
                        ds.ReadXml(sr);
                    }
                    DataTable tab = ds.Tables["dir"];
                    int iDirs = (tab != null) ? tab.Rows.Count : 0;
                    FSDirInfo[] oList = new FSDirInfo[iDirs - 1 + 1];
                    using (tab)
                        for (int i = 0; i <= iDirs - 1; i++)
                            oList[i] = this.getDirInfoFromRow(tab.Rows[i]);
                    // Ritorna lista
                    return oList;
                }
            }


            /// <summary>
            ///  Ricarica database VFS (Virtual File System)
            ///  </summary>
            ///  <remarks></remarks>
            public void ReloadVFS()
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_RELOADVFS);
                this.sendRequest(this.mParams);
            }


            /// <summary>
            ///  Riavvia applicativo server
            ///  </summary>
            ///  <remarks></remarks>
            public void RestartServer()
            {
                this.mParams.Add(FS.PARAM_ACTION, FS.ACTION_RESTART);
                this.sendRequest(this.mParams);
            }








            /// <summary>
            ///  Invia richiesta
            ///  </summary>
            ///  <param name="param"></param>
            ///  <returns></returns>
            private byte[] sendRequest(ParamList param)
            {
                return this.sendRequest(param, null, 0, 0);
            }


            /// <summary>
            ///  Invia richiesta con elenco parametri e buffer
            ///  </summary>
            ///  <param name="param"></param>
            ///  <param name="buffer"></param>
            ///  <returns></returns>
            private byte[] sendRequest(ParamList param, byte[] buffer, int offset, int length)
            {
                this.setResponse(0, string.Empty);
                try
                {
                    using (WebClientEx oWebCli = new WebClientEx())
                    {
                        oWebCli.TimeoutMSec = this.TimeoutMSec;
                        oWebCli.Headers.Clear();
                        oWebCli.Headers.Add("User-Agent", "HFS Client.NET");
                        // Imposta user e pass se non presenti
                        if (!string.IsNullOrEmpty(this.mUser))
                        {
                            this.mParams.Add(FS.PARAM_USER, this.mUser);
                            this.mParams.Add(FS.PARAM_PASS, this.mPass);
                        }

                        // Reimposta valori path se necessario
                        string sVPath = "";
                        if (param.TryGetValue(FS.PARAM_VPATH, out sVPath))
                        {
                            try
                            {
                                param[FS.PARAM_VPATH] = this.getFullVPath(sVPath);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Il path virtuale fornito non e' valorizzato");
                            }
                        }
                        if (param.TryGetValue(FS.PARAM_VPATHDEST, out sVPath))
                        {
                            try
                            {
                                param[FS.PARAM_VPATHDEST] = this.getFullVPath(sVPath);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Il path virtuale di destinazione fornito non e' valorizzato");
                            }
                        }


                        foreach (KeyValuePair<string, string> oPair in param)
                            oWebCli.Headers.Add(oPair.Key, oPair.Value);

                        // Esegue
                        byte[] buf = null;
                        if (buffer == null)
                            buf = oWebCli.DownloadData(this.mUrl);
                        else
                            using (Stream oStream = oWebCli.OpenWrite(this.mUrl, "POST"))
                            {
                                oStream.Write(buffer, offset, length);
                                oStream.Flush();
                                oStream.Close();
                            }

                        // Testa esito
                        this.checkResponse(oWebCli);

                        return buf;
                    }
                }

                // Ritorna buffer output
                finally
                {
                    this.mParams.Clear();
                }
            }



            /// <summary>
            ///  Invia richiesta con elenco parametri e buffer
            ///  </summary>
            ///  <param name="param"></param>
            ///  <param name="buffer"></param>
            ///  <returns></returns>
            private byte[] sendRequestQS(ParamList param, byte[] buffer, int offset, int length)
            {
                this.setResponse(0, string.Empty);
                try
                {
                    using (WebClientEx oWebCli = new WebClientEx())
                    {
                        oWebCli.TimeoutMSec = this.TimeoutMSec;

                        // Imposta user e pass se non presenti
                        if (!string.IsNullOrEmpty(this.mUser))
                        {
                            this.mParams.Add(FS.PARAM_USER, this.mUser);
                            this.mParams.Add(FS.PARAM_PASS, this.mPass);
                        }

                        // Reimposta valori path se necessario
                        string sVPath = "";
                        if (param.TryGetValue(FS.PARAM_VPATH, out sVPath))
                        {
                            try
                            {
                                param[FS.PARAM_VPATH] = this.getFullVPath(sVPath);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Il path virtuale fornito non e' valorizzato");
                            }
                        }
                        if (param.TryGetValue(FS.PARAM_VPATHDEST, out sVPath))
                        {
                            try
                            {
                                param[FS.PARAM_VPATHDEST] = this.getFullVPath(sVPath);
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("Il path virtuale di destinazione fornito non e' valorizzato");
                            }
                        }

                        char cDelim = '?';
                        StringBuilder sbReq = new StringBuilder();
                        sbReq.Append(this.mUrl);
                        foreach (KeyValuePair<string, string> oPair in param)
                        {
                            sbReq.Append(cDelim);
                            sbReq.Append(oPair.Key);
                            sbReq.Append("=");
                            sbReq.Append(oPair.Value);
                            cDelim = '&';
                        }

                        // Esegue
                        byte[] buf = null;
                        if (buffer == null)
                            buf = oWebCli.DownloadData(sbReq.ToString());
                        else
                            using (Stream oStream = oWebCli.OpenWrite(sbReq.ToString(), "POST"))
                            {
                                oStream.Write(buffer, offset, length);
                                oStream.Flush();
                                oStream.Close();
                            }

                        // Testa esito
                        this.checkResponse(oWebCli);

                        // Ritorna buffer output
                        return buf;
                    }
                }
                finally
                {
                    this.mParams.Clear();
                }
            }




            /// <summary>
            ///  Ritorna path unificato con Current Directory
            ///  </summary>
            ///  <param name="vpath"></param>
            ///  <returns></returns>
            ///  <remarks></remarks>
            private string getFullVPath(string vpath)
            {
                vpath = vpath.Replace(FS.VPATH_DIR_SEP_WIN, FS.VPATH_DIR_SEP);
                if (string.IsNullOrEmpty(this.CurrentDirectory) || vpath.StartsWith(FS.VPATH_DIR_SEP.ToString(), StringComparison.Ordinal))

                    return vpath;

                vpath = string.Concat(this.CurrentDirectory, "/", vpath);

                if (vpath.Trim(FS.VPATH_DIR_SEP).Length == 0)
                    throw new ArgumentException();

                return vpath;
            }

            /// <summary>
            ///  Imposta dati risposta
            ///  </summary>
            ///  <param name="code"></param>
            ///  <param name="msg"></param>
            private void setResponse(int code, string msg)
            {
                this.mLastRespCode = code;
                this.mLastRespMsg = msg;
            }


            /// <summary>
            ///  Controlla esito chiamata
            ///  </summary>
            private void checkResponse(WebClient http)
            {
                string sCode = http.ResponseHeaders[FS.HEADER_STATUS_CODE];
                string sMsg = http.ResponseHeaders[FS.HEADER_STATUS_MSG];

                // Presenza headers
                if (string.IsNullOrEmpty(sCode) || sMsg == null)
                {
                    this.setResponse(100, "Risposta dal server non riconosciuta (mancano headers)");
                    throw new ApplicationException(this.mLastRespMsg);
                }

                // Imposta valori
                int iCode = 0;
                if (Int32.TryParse(sCode, out iCode))
                    this.setResponse(iCode, System.Web.HttpUtility.UrlDecode(sMsg));
                else
                    this.setResponse(100, "Risposta dal server non riconosciuta (codice stato non numerico)");

                // Se code <> 0: ERRORE
                if (this.mLastRespCode != 0)
                    throw new ApplicationException(string.Concat("Server: ", this.mLastRespMsg, " (rc=", this.mLastRespCode.ToString(), ")"));
            }



            /// <summary>
            ///  Controllo file locale
            ///  </summary>
            ///  <param name="localFile"></param>
            ///  <param name="checkExist"></param>
            private void checkInputFile(string localFile, bool checkExist)
            {
                // Controllo file
                if (localFile == null || localFile.Trim() == "")
                    this.setResponse(100, "E' necessario fornire un nome file locale valido");
                // Esistenza
                if (checkExist && !File.Exists(localFile))
                    this.setResponse(100, string.Format("Il file fornito in input '{0}' non esiste", localFile));

                if (this.mLastRespCode > 0)
                    throw new ArgumentException(this.mLastRespMsg);
            }



            /// <summary>
            ///  Controllo buffer
            ///  </summary>
            ///  <param name="buffer"></param>
            private void checkBuffer(byte[] buffer)
            {
                // Controllo null
                if (buffer == null)
                    this.setResponse(100, "E' necessario fornire un buffer non nullo");
                // Controllo lunghezza
                if (buffer.Length == 0)
                    this.setResponse(100, "Il buffer fornito non contiene dati. Se si vuole creare un file a 0 byte utilizzare il metodo FileTouch");

                if (this.mLastRespCode > 0)
                    throw new ArgumentException(this.mLastRespMsg);
            }


            /// <summary>
            ///  Controlla stream
            ///  </summary>
            ///  <param name="stream"></param>
            private void checkStream(Stream stream, bool checkRead, bool checkWrite, bool checkSeek)
            {
                // Controllo stream
                if (stream == null)
                    this.setResponse(100, "E' necessario fornire uno stream in input");
                // Read
                if (checkRead && !stream.CanRead)
                    this.setResponse(100, "Lo stream fornito in input non consente la lettura");
                // Read
                if (checkWrite && !stream.CanWrite)
                    this.setResponse(100, "Lo stream fornito in input non consente la scrittura");
                // Seek
                if (checkSeek && !stream.CanSeek)
                    this.setResponse(100, "Lo stream fornito in input non consente il posizionamento");

                if (this.mLastRespCode > 0)
                    throw new ArgumentException(this.mLastRespMsg);
            }



            /// <summary>
            ///  Carica FileInfo da datarow
            ///  </summary>
            ///  <param name="row"></param>
            ///  <returns></returns>
            private FSFileInfo getFileInfoFromRow(DataRow row)
            {
                return new FSFileInfo((string)row["name"], Convert.ToInt64(row["len"]), (FileAttributes)Convert.ToInt32(row["attr"].ToString()), DateUT.ParseData(row["ctime"].ToString(), "", "yyyyMMddHHmmss"), DateUT.ParseData(row["atime"].ToString(), "", "yyyyMMddHHmmss"), DateUT.ParseData(row["wtime"].ToString(), "", "yyyyMMddHHmmss"), new FSPermissionHFS(row["access"].ToString()));
            }

            /// <summary>
            ///  Carica FSDirInfo da datarow
            ///  </summary>
            ///  <param name="row"></param>
            ///  <returns></returns>
            private FSDirInfo getDirInfoFromRow(DataRow row)
            {
                return new FSDirInfo(row["name"].ToString(), DateUT.ParseData(row["ctime"].ToString(), "", "yyyyMMddHHmmss"), new FSPermissionHFS(row["access"].ToString()));
            }





            public override void Dispose()
            {
                this.mParams.Clear();
            }
        }
    }

