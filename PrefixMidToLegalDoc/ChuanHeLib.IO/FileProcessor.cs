using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChuanHeLib.IO
{
    public class FileProcessor
    {

        /// <summary>
        /// Add suffix to original file to avoid name duplication
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public string AddSuffixForDuplicateFile(string fileFullName)
        {
            string suffixedName = string.Empty;

            string directory = Path.GetDirectoryName(fileFullName);
            string filename = Path.GetFileNameWithoutExtension(fileFullName);
            string extension = Path.GetExtension(fileFullName);
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
