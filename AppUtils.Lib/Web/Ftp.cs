using System;
using System.Collections.Generic;
using System.Text;

namespace AppUtils.Lib.Web
{
    using System;
    using System.Net;

    /// <summary>

    /// ''' Funzioni per l'utilizzo del protocollo FTP

    /// ''' </summary>

    /// ''' <remarks></remarks>
    public class Ftp
    {



        /// <summary>
        ///     ''' Crea la richiesta Ftp
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private static FtpWebRequest CreateFtpRequest(string url, string user, string password, bool passive)
        {
            // Imposta Url
            FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(url);

            // Imposta passivo/attivo
            reqFtp.UsePassive = passive;

            // Imposta User, Pass
            if (!string.IsNullOrEmpty(user))
                reqFtp.Credentials = new NetworkCredential(user, password);

            // 'Ritorna richiesta
            return reqFtp;
        }


        /// <summary>
        ///     ''' Crea una directory su un server FTP
        ///     ''' </summary>
        ///     ''' <param name="url">
        ///     ''' url del tipo ftp://host:port/path
        ///     ''' </param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void CreateFolder(string url, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
            reqFtp.UseBinary = true;
            FtpWebResponse respFtp = (FtpWebResponse)reqFtp.GetResponse();

            respFtp.Close();
        }


        /// <summary>
        ///     ''' Elimina una cartella VUOTA
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void DeleteEmptyFolder(string url, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.RemoveDirectory;
            reqFtp.UseBinary = true;
            FtpWebResponse respFtp = (FtpWebResponse)reqFtp.GetResponse();

            respFtp.Close();
        }


        /// <summary>
        ///     ''' Elimina file singolo
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void DeleteFile(string url, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;
            reqFtp.UseBinary = true;
            FtpWebResponse respFtp = (FtpWebResponse)reqFtp.GetResponse();

            respFtp.Close();
        }


        /// <summary>
        ///     ''' Elimina la cartella e tutto il suo contenuto (sottocartelle comprese)
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void DeleteFolderTree(string url, string user, string pass)
        {
            string[] listaFtp = Ftp.GetList(url, user, pass);
            string newFtpUrl;

            foreach (string sEntry in listaFtp)
            {
                // Calcola nuovo url (sia file che dir)
                newFtpUrl = string.Format("{0}/{1}", url, System.IO.Path.GetFileName(sEntry));

                if (sEntry.Contains("."))
                    // Elimia File
                    Ftp.DeleteFile(newFtpUrl, user, pass);
                else
                    // Elimina Directory Tree
                    Ftp.DeleteFolderTree(newFtpUrl, user, pass);
            }

            // Elimina cartella vuota
            Ftp.DeleteEmptyFolder(url, user, pass);
        }


        /// <summary>
        ///     ''' Esegue Upload File
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="localFile"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void UploadFile(string url, string localFile, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            using (System.IO.Stream ftpStream = reqFtp.GetRequestStream())
            {
                byte[] buff = System.IO.File.ReadAllBytes(localFile);
                ftpStream.Write(buff, 0, buff.Length);
            }
        }


        /// <summary>
        ///     ''' Scarica file da server ftp
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="localFile"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static void DownloadFile(string url, string localFile, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFtp.UseBinary = true;
            FtpWebResponse respFtp = (FtpWebResponse)reqFtp.GetResponse();
            using (System.IO.Stream respFtpStream = respFtp.GetResponseStream())
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(localFile, System.IO.FileMode.Create))
                {
                    byte[] buffer = new byte[Int16.MaxValue + 1];
                    int rBytes = respFtpStream.Read(buffer, 0, Int16.MaxValue);
                    while (rBytes > 0)
                    {
                        // Scrive su file
                        fs.Write(buffer, 0, rBytes);

                        // Legge
                        rBytes = respFtpStream.Read(buffer, 0, Int16.MaxValue);
                    }
                }
            }
        }


        /// <summary>
        ///     ''' Elenco contenuto directory
        ///     ''' </summary>
        ///     ''' <param name="url"></param>
        ///     ''' <param name="user"></param>
        ///     ''' <param name="pass"></param>
        ///     ''' <remarks></remarks>
        public static string[] GetList(string url, string user, string pass)
        {
            FtpWebRequest reqFtp = Ftp.CreateFtpRequest(url, user, pass, false);
            reqFtp.Method = WebRequestMethods.Ftp.ListDirectory;
            reqFtp.UseBinary = true;

            FtpWebResponse respFtp = (FtpWebResponse)reqFtp.GetResponse();
            string retStr = string.Empty;

            using (System.IO.StreamReader ftpStremReader = new System.IO.StreamReader(respFtp.GetResponseStream()))
            {
                retStr = ftpStremReader.ReadToEnd();
            }

            return retStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

}
