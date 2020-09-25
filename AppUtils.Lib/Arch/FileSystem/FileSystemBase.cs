using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AppUtils.Lib.Arch.FileSystem.Interfacce;

namespace AppUtils.Lib.Arch.FileSystem
{


        /// <summary>
        ///     ''' Classe base
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        public abstract class FileSystemBase : IFileSystem
        {
            protected string mUrl;
            protected string mUser;
            public virtual string CurrentDirectory { get; set; } = string.Empty;



            // Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
            public abstract void Dispose();




            /// <summary>
            ///  Eventuale timeout per le operazioni (default 10 Minuti)
            ///  </summary>
            ///  <returns></returns>
            public int TimeoutMSec { get; set; } = (10 * 60 * 1000);


            /// <summary>
            ///  Uri contenente tutte le informazioni di configurazione del file system
            ///  (host, porta, user, pass, parametri in querystring)
            ///  </summary>
            ///  <value></value>
            ///  <returns></returns>
            ///  <remarks></remarks>
            public string Url
            {
                get
                {
                    return this.mUrl;
                }
            }

            public string User
            {
                get
                {
                    return this.mUser;
                }
            }




            public abstract int LastRespCode { get; }


            public abstract string LastRespMsg { get; }


            public abstract void DirectoryCopy(string vpath, string vpathdest);


            public abstract void DirectoryCreate(string vpath);


            public abstract void DirectoryDelete(string vpath);


            public abstract void DirectoryMove(string vpath, string vpathdest);


            public abstract bool DirectoryExist(string vpath);


            public abstract void FileAppendFromBuffer(string vpath, byte[] buffer);


            public abstract void FileCopy(string vpath, string vpathdest);


            public abstract void FileDelete(string vpath);


            public abstract void FileEmail(string vpath, string to, string from, string subj, string body);


            public abstract bool FileExist(string vpath);


            public abstract string FileGetLink(string vpath);


            public abstract FSFileInfo FileGetInfo(string vpath);

            public abstract string FileGetImageInfo(string vpath);

            public abstract void FileMove(string vpath, string vpathdest);

            public abstract byte[] FileReadToBuffer(string vpath);

            public abstract byte[] FileReadToBuffer(string vpath, string attr);


            public abstract void FileReadToDisk(string vpath, string localFile);


            public abstract void FileReadToStream(string vpath, System.IO.Stream stream);


            public abstract void FileTouch(string vpath);


            public abstract void FileWriteFromBuffer(string vpath, byte[] buffer);


            public abstract void FileWriteFromDisk(string vpath, string localFile);


            public abstract void FileWriteFromStream(string vpath, System.IO.Stream stream);


            public abstract string FileHashSHA1(string vpath);


            public abstract string FileLocalHashSHA1(string localfile);


            public abstract FSDirInfo[] DirectoryListSubDirFilter(string vpath, string pattern, bool recursive);


            public abstract FSFileInfo[] DirectoryListFilesFilter(string vpath, string pattern, bool recursive);


            public abstract FSFileInfo[] DirectoryListFiles(string vpath);


            public abstract FSDirInfo[] DirectoryListSubDir(string vpath);


            public abstract FSDirInfo[] ListRootEntries();
        }
    }

