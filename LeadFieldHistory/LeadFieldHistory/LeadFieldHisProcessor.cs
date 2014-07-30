using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Collections;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;

namespace LeadFieldHistory
{
    class LeadFieldHisProcesser
    {
        private string dirPath = string.Empty;
        private string fieldName = string.Empty;
        private DirectoryInfo dirInfo = null;
        private Excel.Application excelApp = null;

        //Constructor
        public LeadFieldHisProcesser(string fieldName)
        {
            this.dirPath = ConfigurationManager.AppSettings["dirPath"];
            this.fieldName = fieldName;
            this.dirInfo = Directory.CreateDirectory(dirPath);
            this.excelApp = new Excel.Application();

        }

        public void Processing()
        {
            FileInfo[] fileInfoArr = dirInfo.GetFiles();
           
            foreach (FileInfo f in fileInfoArr)
            {
                Console.WriteLine(f.FullName);
                this.filterFile(f);                
            }

            excelApp.Quit();
            if (excelApp != null)
            {
                Marshal.ReleaseComObject(excelApp);
            }
            excelApp = null;
            GC.Collect();
            
        }

        private void filterFile(FileInfo file)
        {
            //FileInfo file = fileInfoArr[0];
            Excel.Workbook excelBook = null;
            try
            {
                excelBook = excelApp.Workbooks.Open(file.FullName, Type.Missing, false, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception e)
            {
                throw e;
            }

            Excel.Worksheet currentSheet = (Excel.Worksheet)excelBook.Worksheets.get_Item(1);
            //Console.WriteLine(f.FullName);

            int lastUsedRow = currentSheet.UsedRange.Row + currentSheet.UsedRange.Rows.Count - 1;
            Console.WriteLine(lastUsedRow - 1);


            ArrayList arrList = new ArrayList(); //--

            for (int i = 2; i <= lastUsedRow; i++)
            {
                Excel.Range range = currentSheet.get_Range("A" + i.ToString(), "F" + i.ToString());
                System.Array myvalues = (System.Array)range.Cells.Value;
                string[] strArray = myvalues.OfType<object>().Select(o => o.ToString()).ToArray();
                //Console.WriteLine(strArray[2]);
                if (strArray[1] == this.fieldName)
                {
                    arrList.Add(strArray); //--
                    //saveToDB(Convert.ToInt32(strArray[0]), strArray[3], strArray[4], strArray[2]);
                    Console.WriteLine(file.FullName + "---recordCount:" + i);
                }
                
            }
            //Console.WriteLine(arrList.Count);
            excelBook.Close(true, Type.Missing, Type.Missing);
            excelApp.Workbooks.Close();
            if (excelBook != null)
            {
                Marshal.ReleaseComObject(excelBook);
            }
            excelBook = null;
        }


        public void saveToDB(int leadId, string ownerName, string modifiedBy, string dateModified)
        {
            leadId = (leadId != null && leadId > 0 )? leadId : 0;
            ownerName = ownerName != null ? ownerName : "";
            modifiedBy = modifiedBy != null ? modifiedBy : "";
            dateModified = dateModified != null ? dateModified : "1900-01-01 00:00:00:000";

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCmd = null;
            try
            {
                dbCmd = db.GetStoredProcCommand("lead_owner_change_log_insert");
                db.AddInParameter(dbCmd, "LeadId", System.Data.DbType.Int32, leadId);
                db.AddInParameter(dbCmd, "OwnerName", System.Data.DbType.String, ownerName);
                db.AddInParameter(dbCmd, "ModifiedBy", DbType.String, modifiedBy);
                db.AddInParameter(dbCmd, "DateModified", DbType.String, dateModified);
                db.ExecuteNonQuery(dbCmd);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
