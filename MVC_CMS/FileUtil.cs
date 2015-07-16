using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Collections;
//using java.util.zip;
//using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zip;

namespace MVC_CMS.Utilities
{
    public class FileUtil
    {
        /// <summary>
        /// Read text from *.txt file
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <returns>Content text</returns>
        private string[] ReadLineTxtFile(string fileName)
        {
            TextReader tr = new StreamReader(fileName, Encoding.UTF8);
            string[] output = null;
            string lineText = null;
            int i = 0;
            while ((lineText = tr.ReadLine()) != null)
            {
                if (lineText.Trim().Length != 0)
                {
                    Array.Resize(ref output, i + 1);
                    output[i] = lineText;
                    i++;
                }
            }
            tr.Close();
            return output;
        }

        /// <summary>
        /// Write Text to *.txt file
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="input">Content text</param>
        private void WriteTxtFile(string fileName, string input)
        {
            TextWriter tw = new StreamWriter(fileName, false, Encoding.UTF8);
            tw.Write(input);
            tw.Close();
        }

        /// <summary>
        /// Reader all text in txt file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ReadFile(string fileName)
        {
            TextReader tr = new StreamReader(fileName, Encoding.UTF8);
            return tr.ReadToEnd();
        }

        /// <summary>
        /// Read content from text file (*.doc, *.docx, *.txt, *.rtf, *.vi, *.en, *.ucn)
        /// </summary>
        /// <returns></returns>
        public string[] ReadParagraphsInFile(string fileName)
        {
            string[] textResult = null;
            textResult = ReadLineTxtFile(fileName);
            return textResult;
        }

        /// <summary>
        /// Write content to text file (*.doc, *.docx, *.txt, *.rtf, *.vi, *.en, *.ucn)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="input"></param>
        public void WriteFile(string fileName, string input)
        {
            WriteTxtFile(fileName, input);
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                if (fileName != string.Empty)
                    File.Delete(fileName);
            }
            catch (Exception)
            {
                //throw;
            }            
        }

        public void DeleteFolder(string folderName)
        {
            if (folderName != string.Empty)
            {
                Directory.Delete(folderName,true);


                //bool result = false;

                //string[] files = Directory.GetFiles(target_dir);
                //string[] dirs = Directory.GetDirectories(target_dir);

                //foreach (string file in files)
                //{
                //    File.SetAttributes(file, FileAttributes.Normal);
                //    File.Delete(file);
                //}

                //foreach (string dir in dirs)
                //{
                //    DeleteFolder(dir);
                //}

                //Directory.Delete(target_dir, false);

                //return result;



            }
        }

        public void CreateFile(string fileName)
        {
            if (fileName != string.Empty)
            {
                FileStream fs = File.Create(fileName);
                fs.Close();
            }
        }

        public void RenameFile(string oldFileName, string newFileName)
        {
            File.Move(oldFileName, newFileName);
        }

        public void CopyFile(string oldFileName, string newFileName)
        {
            File.Copy(oldFileName, newFileName, true);
        }

        public bool CheckFileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public bool CheckDirectoryExists(string directoryName)
        {
            if (directoryName != string.Empty &&
                Directory.Exists(directoryName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDirectoryExistsByFileName(string fileName)
        {
            if (fileName != string.Empty && fileName.IndexOf('\\') != -1 &&
                Directory.Exists(fileName.Substring(0, fileName.LastIndexOf('\\'))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateDirectory(string directoryName)
        {
            if (directoryName != string.Empty)
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        public void CopyDirectory(string SourcePath, string DestinationPath)
        {
            SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
            DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";
            if (Directory.Exists(SourcePath))
            {
                if (Directory.Exists(DestinationPath) == false)
                    Directory.CreateDirectory(DestinationPath);

                foreach (string fls in Directory.GetFiles(SourcePath))
                {
                    FileInfo flinfo = new FileInfo(fls);                    
                    flinfo.CopyTo(DestinationPath + flinfo.Name);
                }
                foreach (string drs in Directory.GetDirectories(SourcePath))
                {
                    DirectoryInfo drinfo = new DirectoryInfo(drs);
                    CopyDirectory(drs, DestinationPath + drinfo.Name);
                }
            }
        }

        #region For Writeline

        TextWriter writerForWriteLine;
        private void OpenTxtFileForWriteLine(string fileName)
        {
            writerForWriteLine = new StreamWriter(fileName, true, Encoding.UTF8);
        }

        private void CloseTxtFileForWriteLine()
        {
            writerForWriteLine.Close();
        }

        /// <summary>
        /// Write Text to *.txt file
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="input">Content text</param>
        private void WriteLineTxtFile(string fileName, string input)
        {
            writerForWriteLine.WriteLine(input);
        }

        /// <summary>
        /// Open file for writeline
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenFileForWriteLine(string fileName)
        {
            OpenTxtFileForWriteLine(fileName);
        }

        /// <summary>
        /// Close file for writeline
        /// </summary>
        /// <param name="fileName"></param>
        public void CloseFileForWriteLine()
        {
            CloseTxtFileForWriteLine();
        }

        /// <summary>
        /// You must call OpenTxtFileForWriteLine(fileName) before.
        /// Write content to text file (*.doc, *.docx, *.txt, *.rtf, *.vi, *.en, *.ucn).
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="input"></param>
        public void WriteLineFile(string fileName, string input)
        {
            WriteLineTxtFile(fileName, input);
        }
        #endregion

        #region Zip file J#
        /*
        public static void ZipFiles(string inputFolderPath, string outputFileName, string password, bool deleteInputFolder)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            TrimLength += 1;
            FileStream ostream;
            byte[] obuffer;
            string outPath = inputFolderPath + outputFileName;

            java.io.FileOutputStream outStream = new java.io.FileOutputStream(outPath);
            ZipOutputStream oZipStream = new ZipOutputStream(outStream);

            if (password != null && password != String.Empty)
            {
                //oZipStream. = password;
            }


            oZipStream.setLevel(9);
            ZipEntry oZipEntry;
            foreach (string Fil in ar)
            {
                oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                oZipStream.putNextEntry(oZipEntry);

                if (!Fil.EndsWith(@"/"))
                {
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[ostream.Length];
                    ostream.Read(obuffer, 0, obuffer.Length);

                    sbyte[] s = new sbyte[obuffer.Length];
                    Buffer.BlockCopy(obuffer, 0, s, 0, obuffer.Length);
                    oZipStream.write(s, 0, s.Length);
                    ostream.Dispose();
                }
            }
            oZipStream.finish();
            outStream.close();
            oZipStream.closeEntry();
        }


        public static void Extract(string zipFileName, string destinationPath)
        {
            ZipFile zipfile = new ZipFile(zipFileName);
            List<ZipEntry> zipFiles = GetZipFiles(zipfile);

            foreach (ZipEntry zipFile in zipFiles)
            {
                if (!zipFile.isDirectory())
                {
                    java.io.InputStream s = zipfile.getInputStream(zipFile);
                    try
                    {
                        Directory.CreateDirectory(destinationPath + "\\" +
                          Path.GetDirectoryName(zipFile.getName()));
                        java.io.FileOutputStream dest = new java.io.FileOutputStream(Path.Combine
                          (destinationPath + "\\" + Path.GetDirectoryName(zipFile.getName()),
                          Path.GetFileName(zipFile.getName())));
                        try
                        {
                            int len = 0;
                            sbyte[] buffer = new sbyte[7168];
                            while ((len = s.read(buffer)) >= 0)
                            {
                                dest.write(buffer, 0, len);
                            }
                        }
                        finally
                        {
                            dest.close();
                        }
                    }
                    finally
                    {
                        s.close();
                    }
                }
            }
        }

        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir))
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                if (Directory.GetDirectories(Dir).Length == 0)
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils;
        }

        private static List<ZipEntry> GetZipFiles(ZipFile zipfil)
        {
            List<ZipEntry> lstZip = new List<ZipEntry>();
            java.util.Enumeration zipEnum = zipfil.entries();
            while (zipEnum.hasMoreElements())
            {
                ZipEntry zip = (ZipEntry)zipEnum.nextElement();
                lstZip.Add(zip);
            }
            return lstZip;
        }
        */
        #endregion

        #region SharpZipLib
        /*
        /// <summary>
        /// Zip a file
        /// </summary>
        /// <param name="SrcFile">source file path</param>
        /// <param name="DstFile">zipped file path</param>
        /// <param name="BufferSize">buffer to use</param>
        public static void ZipFile(string SrcFile, string DstFile,string password)
        {            
            FileStream fileStreamIn = new FileStream(SrcFile, FileMode.Open, FileAccess.Read);
            FileStream fileStreamOut = new FileStream(DstFile, FileMode.Create, FileAccess.Write);
            ZipOutputStream zipOutStream = new ZipOutputStream(fileStreamOut);

            if (password != null && password != String.Empty)
            {
                zipOutStream.Password = password;                
            }


            byte[] buffer = new byte[4096];

            ZipEntry entry = new ZipEntry(Path.GetFileName(SrcFile));
            zipOutStream.PutNextEntry(entry);

            int size;
            do
            {
                size = fileStreamIn.Read(buffer, 0, buffer.Length);
                zipOutStream.Write(buffer, 0, size);
            } while (size > 0);

            zipOutStream.Close();
            fileStreamOut.Close();
            fileStreamIn.Close();
        }

        public static void ZipFolder(string inputFolderPath, string outputPathAndFile, string password)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            string outPath = outputPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream

            if (password != null && password != String.Empty)
                oZipStream.Password = password;
            
            oZipStream.SetLevel(9); // maximum compression
            ZipEntry oZipEntry;
            foreach (string Fil in ar) // for each file, generate a zipentry
            {
                oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                oZipStream.PutNextEntry(oZipEntry);

                if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                {                    
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[4096];
                    ostream.Read(obuffer, 0, obuffer.Length);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                    ostream.Close();
                    ostream.Dispose();
                }
            }
            oZipStream.Finish();
            oZipStream.Close();
        }

        /// <summary>
        /// UnZip a file
        /// </summary>
        /// <param name="SrcFile">source file path</param>
        /// <param name="DstFile">unzipped file path</param>
        /// <param name="BufferSize">buffer to use</param>
        /// 
        public static void Extract(string SrcFile, string DstFile)
        {
            FileStream fileStreamIn = new FileStream(SrcFile, FileMode.Open, FileAccess.Read);
            ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
            ZipEntry entry = zipInStream.GetNextEntry();
            FileStream fileStreamOut = new FileStream(DstFile + @"\" + entry.Name, FileMode.Create, FileAccess.Write);

            int size;
            byte[] buffer = new byte[4096];
            do
            {
                size = zipInStream.Read(buffer, 0, buffer.Length);
                fileStreamOut.Write(buffer, 0, size);
            } while (size > 0);

            zipInStream.Close();
            fileStreamOut.Close();
            fileStreamIn.Close();
        }

        public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipPathAndFile));
            if (password != null && password != String.Empty)
                s.Password = password;
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();
            if (deleteZipFile)
                File.Delete(zipPathAndFile);
        }
        
        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                if (Directory.GetDirectories(Dir).Length == 0)
                // if directory is completely empty, add it
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils; // return file list
        }
         */
 
        #endregion 

        #region DotNetZip

        //public static bool ZipFiles(string outputFile, string password, List<string> inputFiles)
        public static bool ZipFiles(string outputFile, string password, string directoryToZip)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        zip.Password = password.Replace(@"\", "").Trim();
                    }
                    //zip.AddFiles(inputFiles);

                    zip.AddItem(directoryToZip);
                    zip.Save(outputFile);
                }
            }
            catch (Exception ex)
            {
                string xxx = ex.ToString();
                return false;
            }
            return true;
        }

        public static bool UnZipFiles(string outputDirectory, string password, string inputFile)
        {
            try
            {
                using (ZipFile zip = ZipFile.Read(inputFile))
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        foreach (ZipEntry entry in zip.Entries)
                        {
                            entry.ExtractWithPassword(outputDirectory, password);
                        }
                    }
                    else
                    {
                        zip.ExtractAll(outputDirectory);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}