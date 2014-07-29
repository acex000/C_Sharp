using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel=Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Readfile
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        public static void Test()
        {
            ReadAdapter adapter = new ReadAdapter();
         
            Excel.Workbook book = adapter.ReadFileToBook();
            Excel.Sheets allSheets = book.Worksheets;
            Excel.Worksheet currentSheet = (Excel.Worksheet)allSheets.get_Item(1);
            String value = currentSheet.Cells[3, 3].Value.ToString();
            Console.WriteLine(value);
            adapter.CloseFile();
        }
    }
}
