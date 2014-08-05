using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Configuration;

namespace ImportCSVintoDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPath = ConfigurationManager.AppSettings["dirPath"];
            CSVImporter importer = new CSVImporter();
            importer.ImportMultipleFiles(dirPath);
        }
    }
}
