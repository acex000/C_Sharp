using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace FileTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = ConfigurationManager.AppSettings["sourcePath"];
            string destPath = ConfigurationManager.AppSettings["destinationPath"];

            DirectoryContentCopier copier = new DirectoryContentCopier();
            copier.MoveAllContent(sourcePath,destPath);
        }
    }
}
