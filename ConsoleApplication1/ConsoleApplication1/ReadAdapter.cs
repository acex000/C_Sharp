using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Runtime.InteropServices;

public class ReadAdapter
{
    String exlPathName = ConfigurationManager.AppSettings["exlPathName"];
    Excel.Application excelApp = new Excel.Application();
    Excel.Workbook excelBook = null;


    //Constuctor
    public ReadAdapter()
    {
        excelApp.Visible = false;
    }
    //Open and read file
    public Excel.Workbook ReadFileToBook()
    {
        try
        {
            excelBook = excelApp.Workbooks.Open(exlPathName, Type.Missing, false, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing);
        }
        catch (Exception e)
        {
            throw e;
        }
        return excelBook;
    }
    //Close file After reading
    public void CloseFile()
    {
        excelBook.Close(true, Type.Missing, Type.Missing);
        excelApp.Workbooks.Close();
        excelApp.Quit();

        if (excelBook != null)
        {
            Marshal.ReleaseComObject(excelBook);
        }
        if (excelApp != null)
        {
            Marshal.ReleaseComObject(excelApp);
        }

        excelBook = null;
        excelApp = null;
        GC.Collect();
    }
}