using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DirectoryContentCopier
{
    public class DirectoryContentCopier
    {
        //subdirectories in the first level which isjust under current directory
        string[] subDirectories = null;
        //files in the first level which is just under current directory
        string[] filesUnderSourceDir = null;

        string[] filesUnderDestDir = null;


        private void Initialize(string sourcePath, string destPath)
        {
            try
            {
                //Get all subdirectories in current source directory
                subDirectories = Directory.GetDirectories(sourcePath);
                //Get all files in current source directory
                filesUnderSourceDir = Directory.GetFiles(sourcePath);
                //Get all files in current destination directory
                filesUnderDestDir = Directory.GetFiles(destPath);
                ////Just use this to check if the destPath exist, if not, an exception will happen
                //Directory.GetDirectories(destPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        /// <summary>
        /// Copy All files and Directories from source dir to destination. If source subdirectory and destination subdirectory have same name, they will
        /// be merged automatically. If there already exists a file that has the same name with the operatated file, then sufix the file name with (i).
        /// eg. "test.txt" will be renamed as "test (1).txt"
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public void CopyAllContent(string sourcePath, string destPath)
        {
            CopyAllContent(sourcePath, destPath, false);
        }
        
        /// <summary>
        /// Copy All files and Directories from source dir to destination. If the "overWrite" parameter is true, then overwrite duplicate file with the same name.
        /// If the "overWrite" parameter is false, then rename the duplicate file by adding a suffix (i) to keep both files.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="overWrite"></param>
        public void CopyAllContent(string sourcePath, string destPath, bool overWrite)
        {

            Initialize(sourcePath, destPath);

            //Copy files those are just under sourcePath directory to the destination directory
            if (filesUnderSourceDir != null && Directory.Exists(destPath))
            {
                //here USD is short for UnderSourceDir, and UDD is short for UnderDestDir      
                FileInfo fileUSDInfo = null;
                string destFileFullName = string.Empty;

                //If "overWrite" is false, then make suffixed name for the new duplicate file
                if (!overWrite)
                {
                    string suffixedName = string.Empty;
                    foreach (string fileUSD in filesUnderSourceDir)
                    {
                        fileUSDInfo = new FileInfo(fileUSD);
                        destFileFullName = destPath + fileUSDInfo.Name;
                        if (File.Exists(destFileFullName))
                        {
                            suffixedName = AddSuffixForDuplicateFile(destFileFullName);
                            File.Copy(fileUSD, suffixedName);
                        }
                        else
                            File.Copy(fileUSD, destFileFullName);
                    }
                }
                //if "OverWrite" is true, then just overwrite duplicate file
                else
                {
                    foreach (string fileUSD in filesUnderSourceDir)
                    {
                        fileUSDInfo = new FileInfo(fileUSD);
                        File.Copy(fileUSD, destPath + fileUSDInfo.Name, true);
                    }
                }
            }

            //Copy directories those are just under sourcePath directory to the destination directory
            if (subDirectories != null && Directory.Exists(destPath))
            {
                if (!overWrite)
                {
                    //Traverse every subdirectory in the source directory
                    foreach (string subDir in subDirectories)
                    {
                        DirectoryInfo sourceSubDirPathInfo = new DirectoryInfo(subDir);
                        DirectoryInfo destSubDirPathInfo = new DirectoryInfo(destPath + sourceSubDirPathInfo.Name + @"\");
                        //Call function to copy subdirectory and all of its inclusive subdirectories and files recursively
                        CopyDirsSuffixDuplicate(sourceSubDirPathInfo, destSubDirPathInfo);
                    }
                }
                else
                {
                    //Traverse every subdirectory in the source directory
                    foreach (string subDir in subDirectories)
                    {
                        DirectoryInfo sourceSubDirPathInfo = new DirectoryInfo(subDir);
                        DirectoryInfo destSubDirPathInfo = new DirectoryInfo(destPath + sourceSubDirPathInfo.Name + @"\");
                        //Call function to copy subdirectory and all of its inclusive subdirectories and files recursively
                        CopyDirsOverWriteDuplicate(sourceSubDirPathInfo, destSubDirPathInfo);
                    }
                }
            }


        }

        private void CopyDirsOverWriteDuplicate(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirsOverWriteDuplicate(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void CopyDirsSuffixDuplicate(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            string destFileFullName = string.Empty;
            string suffixedName = string.Empty;
            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                destFileFullName = Path.Combine(target.ToString(), fi.Name);
                if (File.Exists(destFileFullName))
                {
                    suffixedName = AddSuffixForDuplicateFile(destFileFullName);
                    Console.WriteLine(@"Copying {0}\{1}", suffixedName, fi.Name);
                    fi.CopyTo(suffixedName);
                }
                else
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(destFileFullName);
                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirsSuffixDuplicate(diSourceSubDir, nextTargetSubDir);
            }
        }

        /// <summary>
        /// Move All files and Directories from source dir to destination. If source subdirectory and destination subdirectory have same name, they will
        /// be merged automatically. If there already exists a file that has the same name with the operatated file, then sufix the file name with (i).
        /// eg. "test.txt" will be renamed as "test (1).txt"
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public void MoveAllContent(string sourcePath, string destPath)
        {
            MoveAllContent(sourcePath, destPath, false);
        }

        /// <summary>
        /// Move All files and Directories from source dir to destination. If the "overWrite" parameter is true, then overwrite duplicate file with the same name.
        /// If the "overWrite" parameter is false, then rename the duplicate file by adding a suffix (i) to keep both files.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="overWrite"></param>
        public void MoveAllContent(string sourcePath, string destPath, bool overWrite)
        {
            //Initialize(sourcePath, destPath);

            //if (!overWrite)
            //{
            CopyAllContent(sourcePath, destPath, overWrite);
            foreach (string fileName in filesUnderSourceDir)
            {
                File.Delete(fileName);
            }
            foreach (string subDir in subDirectories)
            {
                Directory.Delete(subDir, true);
            }
            //}
            //else
            //{

            //}

            ////Move files those are just under sourcePath directory to the destination directory
            //if (filesUnderSourceDir != null && Directory.Exists(destPath))
            //{
            //    FileInfo fileUSDInfo = null;
            //    foreach (string fileUSD in filesUnderSourceDir)
            //    {
            //        fileUSDInfo = new FileInfo(fileUSD);
            //        if(File.Exists(destPath+fileUSDInfo.Name))
            //        {

            //            File.Delete(destPath+fileUSDInfo.Name);
            //        }
            //        else
            //            File.Move(fileUSD, destPath + fileUSDInfo.Name);
            //    }
            //}

            //Move directories those are just under sourcePath directory to the destination directory
            //if (subDirectories != null && Directory.Exists(destPath))
            //{
            //    //Traverse every subdirectory in the source directory
            //    foreach (string subDir in subDirectories)
            //    {
            //        DirectoryInfo sourceSubDirPathInfo = new DirectoryInfo(subDir);
            //        DirectoryInfo destSubDirPathInfo = new DirectoryInfo(destPath + sourceSubDirPathInfo.Name + @"\");
            //        //Call function to copy subdirectory and all of its inclusive subdirectories and files recursively
            //        MoveDirs(sourceSubDirPathInfo, destSubDirPathInfo, true);
            //    }
            //}

        }

        //private void MoveDirs(DirectoryInfo source, DirectoryInfo target, bool overWrite)
        //{
        //    CopyDirsOverWriteDuplicate(source, target);
        //    Directory.Delete(source.FullName, true);
        //}

        ///<summary>
        ///Add suffix " (i)" for dupilcate new coming file. eg. file named "test.txt" will become "test (i).txt"
        ///</summary>
        private string AddSuffixForDuplicateFile(string filePathName)
        {
            string suffixedName = string.Empty;

            string directory = Path.GetDirectoryName(filePathName);
            string filename = Path.GetFileNameWithoutExtension(filePathName);
            string extension = Path.GetExtension(filePathName);
            int counter = 1;

            do
            {
                string newFilename = string.Format("{0} ({1}){2}", filename, counter, extension);
                suffixedName = Path.Combine(directory, newFilename);
                counter++;
            } while (File.Exists(suffixedName));

            return suffixedName;
        }
    }
}
