using System;
using System.Collections.Generic;
using System.IO;

namespace AppUtils.Lib.Arch.FileSystem
{


    public class FileSystemLocal : FileSystemBase
    {
        private string mBaseDir;
        private char[] ARR_C_SEP = new[] { '\\' };
        private DirectoryInfo mDirInfo;

        public FileSystemLocal(string baseDir)
        {
            this.mBaseDir = baseDir;
            this.mDirInfo = new DirectoryInfo(this.mBaseDir);
        }

        public override void DirectoryCopy(string vpath, string vpathdest)
        {
        }

        public override void DirectoryCreate(string vpath)
        {
          
        }

        public override void DirectoryDelete(string vpath)
        {
            this.mDirInfo.Delete(true);
        }

        public override bool DirectoryExist(string vpath)
        {
            return this.mDirInfo.Exists;
        }

        public override FSFileInfo[] DirectoryListFiles(string vpath)
        {
        }

        public override FSFileInfo[] DirectoryListFilesFilter(string vpath, string pattern, bool recursive)
        {
        }

        public override FSDirInfo[] DirectoryListSubDir(string vpath)
        {
        }

        public override FSDirInfo[] DirectoryListSubDirFilter(string vpath, string pattern, bool recursive)
        {
        }

        public override void DirectoryMove(string vpath, string vpathdest)
        {
  
        }

        public override void Dispose()
        {
        }

        public override void FileAppendFromBuffer(string vpath, byte[] buffer)
        {
        }

        public override void FileCopy(string vpath, string vpathdest)
        {
        }

        public override void FileDelete(string vpath)
        {
        }

        public override void FileEmail(string vpath, string to, string from, string subj, string body)
        {
        }

        public override bool FileExist(string vpath)
        {
        }

        public override FSFileInfo FileGetInfo(string vpath)
        {
        }

        public override string FileGetLink(string vpath)
        {
        }

        public override void FileMove(string vpath, string vpathdest)
        {
        }

        public override byte[] FileReadToBuffer(string vpath)
        {
        }

        public override void FileReadToDisk(string vpath, string localFile)
        {
        }

        public override void FileReadToStream(string vpath, System.IO.Stream stream)
        {
        }

        public override void FileTouch(string vpath)
        {
            using (FileStream fs = File.Create(vpath))
            {
            }
        }

        public override void FileWriteFromBuffer(string vpath, byte[] buffer)
        {
            File.WriteAllBytes(vpath, buffer);
        }

        public override void FileWriteFromDisk(string vpath, string localFile)
        {
            using (FileStream fs = File.OpenRead(localFile))
            {
                FileWriteFromStream(vpath, fs);
            }
        }

        public override void FileWriteFromStream(string vpath, System.IO.Stream stream)
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] buff = Array.CreateInstance(typeof(byte[]), Int16.MaxValue);
            int iRead;
            using (FileStream fs = File.Create(vpath))
            {
                iRead = stream.Read(buff, 0, buff.Length);
                while (iRead > 0)
                {
                    fs.Write(buff, 0, iRead);
                    iRead = stream.Read(buff, 0, buff.Length);
                }
            }
        }

        public override int LastRespCode
        {
            get
            {
                return 0;
            }
        }

        public override string LastRespMsg
        {
            get
            {
                return string.Empty;
            }
        }

        public override FSDirInfo[] ListRootEntries()
        {
            System.IO.DriveInfo[] oDrives = System.IO.DriveInfo.GetDrives();
            List<FSDirInfo> oRetLista = new List<FSDirInfo>(3);
            foreach (System.IO.DriveInfo oDrive in oDrives)
            {
                switch (oDrive.DriveType)
                {
                    case System.IO.DriveType.Fixed:
                    case System.IO.DriveType.Removable:
                        {
                            oRetLista.Add(new FSDirInfo(oDrive.Name, DateTime.MinValue, new FSPermissionHFS("LRWD")));
                            break;
                        }
                }
            }

            return oRetLista.ToArray();
        }

        public override string FileGetImageInfo(string vpath)
        {
            throw new NotImplementedException();
        }

        public override byte[] FileReadToBuffer(string vpath, string attr)
        {
            throw new NotImplementedException();
        }

        public override string FileHashSHA1(string vpath)
        {
            throw new NotImplementedException();
        }

        public override string FileLocalHashSHA1(string localfile)
        {
            throw new NotImplementedException();
        }
    }
}

