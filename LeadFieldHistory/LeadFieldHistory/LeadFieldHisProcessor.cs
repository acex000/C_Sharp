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
        private Excel.Workbook excelBook = null;
        private Database db = null;
        private DbCommand dbCmd = null;

        //Constructor
        public LeadFieldHisProcesser(string fieldName)
        {
            this.dirPath = ConfigurationManager.AppSettings["dirPath"];
            this.fieldName = fieldName;
            this.dirInfo = Directory.CreateDirectory(dirPath);
            this.excelApp = new Excel.Application();

            this.db = DatabaseFactory.CreateDatabase();
            try
            {
                this.dbCmd = db.GetStoredProcCommand("lead_owner_change_log_insert");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }

        //Get files and process every file
        public void Processing()
        {
            FileInfo[] fileInfoArr = dirInfo.GetFiles();

            foreach (FileInfo f in fileInfoArr)
            {
                Console.WriteLine("processing: "+f.Name);
                this.filterFile(f);
                Console.WriteLine("finished: " + f.Name);
            }
            //filterFile(fileInfoArr[0]);

            this.excelApp.Quit();
            if (excelApp != null)
            {
                Marshal.ReleaseComObject(excelApp);
            }
            this.excelApp = null;
            GC.Collect();
            
        }


        //Get valid rows in the chosen file
        private void filterFile(FileInfo file)
        {
            
            try
            {
                this.excelBook = excelApp.Workbooks.Open(file.FullName, Type.Missing, false, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception e)
            {
                throw e;
            }

            Excel.Worksheet currentSheet = (Excel.Worksheet)excelBook.Worksheets.get_Item(1);
            
            //this value can denote how many valid rows in total in this current sheet
            int lastUsedRow = currentSheet.UsedRange.Row + currentSheet.UsedRange.Rows.Count - 1;
            //Console.WriteLine(lastUsedRow);

            //<--Use these three collection variable to get a row in sheet
            Excel.Range rowRange = null;
            System.Array cellsInRow_SysArr = null;
            string[] cellsInRow_strArr = new string[6];
            //-->

            //ArrayList rowsList = new ArrayList(); //--

            for (int i = 2; i <= lastUsedRow; i++)
            {
                rowRange = currentSheet.get_Range("A" + i.ToString(), "F" + i.ToString());            
                cellsInRow_SysArr = (System.Array)rowRange.Cells.Value;
                
                //string[] strArray = cellsInRow_SysArr.OfType<object>().Select(o => o.ToString()).ToArray(); //this method will ignore the blank cells, which will lead mistakes
                
                //In this way, when a cell is blank, it wont be ignored, we will get a "" string. This systemarray is a 2 dimention array, the first parameter must be 1 here, and the second is 
                //from 1 to 6, mapping to A-F column range
                cellsInRow_strArr[0] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 1));
                cellsInRow_strArr[1] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 2));
                cellsInRow_strArr[2] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 3));
                cellsInRow_strArr[3] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 4));
                cellsInRow_strArr[4] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 5));
                cellsInRow_strArr[5] = Convert.ToString(cellsInRow_SysArr.GetValue(1, 6));



                if (cellsInRow_strArr[1] == this.fieldName)
                {
                    //rowsList.Add(cellsInRow_strArr); //--
                    saveToDB(cellsInRow_strArr[0], cellsInRow_strArr[3], cellsInRow_strArr[4], cellsInRow_strArr[2]);
                    //Console.WriteLine(file.FullName + "---recordCount:" + i);
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

        public void saveToDB(string leadIdStr, string ownerName, string modifiedBy, string dateModified)
        {
            int leadId = string.IsNullOrEmpty(leadIdStr) ? 0 : Convert.ToInt32(leadIdStr);
            ownerName = ownerName != null ? ownerName : "";
            modifiedBy = modifiedBy != null ? modifiedBy : "";
            dateModified = dateModified != null ? dateModified : "1900-01-01 00:00:00:000";

            try
            {               
                db.AddInParameter(dbCmd, "LeadId", System.Data.DbType.Int32, leadId);
                db.AddInParameter(dbCmd, "OwnerName", System.Data.DbType.String, ownerName);
                db.AddInParameter(dbCmd, "ModifiedBy", DbType.String, modifiedBy);
                db.AddInParameter(dbCmd, "DateModified", DbType.String, dateModified);
                db.ExecuteNonQuery(dbCmd);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public void exceptionCloseApp()
        {
            this.excelBook.Close(true, Type.Missing, Type.Missing);
            this.excelApp.Workbooks.Close();
            if (excelBook != null)
            {
                Marshal.ReleaseComObject(excelBook);
            }
            excelBook = null;

            excelApp.Quit();
            if (excelApp != null)
            {
                Marshal.ReleaseComObject(excelApp);
            }
            excelApp = null;
            GC.Collect();
        }
    }
}
