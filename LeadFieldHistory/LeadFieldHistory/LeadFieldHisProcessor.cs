using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace LeadFieldHistory
{
    class LeadFieldHisProcesser
    {
        private string dirPath;
        private string fieldName;
        private DirectoryInfo dirInfo;

        //Constructor
        public LeadFieldHisProcesser(string fieldName)
        {
            this.dirPath = ConfigurationManager.AppSettings["dirPath"];
            this.fieldName = fieldName;
            this.dirInfo = Directory.CreateDirectory(dirPath);
        }

        public void Processing()
        {
            FileInfo[] fileInfoArr = dirInfo.GetFiles();
            foreach (FileInfo f in fileInfoArr)
            {
                Console.WriteLine(f.FullName);
            }
        }

    }
}
