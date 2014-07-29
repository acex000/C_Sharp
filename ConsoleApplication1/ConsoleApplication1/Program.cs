using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Collections;

namespace TxtToDB
{
    class Program
    {

        public int readExl(ref int counter)
        {
            ReadAdapter adapter = new ReadAdapter();

            Excel.Workbook book = adapter.ReadFileToBook();
            Excel.Sheets allSheets = book.Worksheets;
            Excel.Worksheet currentSheet = (Excel.Worksheet)allSheets.get_Item(2);
            int value=0;
            if (currentSheet.Cells[counter, 4].Value == null)
            {
                counter++;
                value = (int)currentSheet.Cells[counter, 4].Value;
                Console.WriteLine(currentSheet.Cells[counter, 4].Value.ToString());
            }
            
            adapter.CloseFile();
            return value;
        }

        public List<ExperianData> txtToDB()
        {
            int counter = 0;
            string txtPathName = string.Empty;
            string line = string.Empty;
            StreamReader file = null;
            List<ExperianData> expDataList = null;
            try
            {
                txtPathName = ConfigurationManager.AppSettings["txtPathName"];
                file = new StreamReader(txtPathName);
                expDataList = new List<ExperianData>();
                // Read the file and display it line by line.
                while ((line = file.ReadLine()) != null)
                {
                    expDataList.Add(handleData(line, ref counter));
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if(file!=null)
                    file.Close();
            }
            return expDataList;
        }

        public ExperianData handleData(string line, ref int counter)
        {
            ExperianData expData = new ExperianData();
            try
            {
                try
                {
                    string temp = line.Substring(0, 8);

                    temp = temp.Insert(temp.Length - 4, "/");
                    temp = temp.Insert(temp.Length - 7, "/");
                    expData.AlertDate = DateTime.Parse(temp);
                }
                catch (Exception e)
                {
                    throw e;
                }
                try
                {
                    expData.Bin = int.Parse(line.Substring(8, 9));
                }
                catch (Exception e)
                {
                    throw e;
                }
                expData.BestBusinessName = line.Substring(17, 40);
                expData.customerBusinessName = line.Substring(57, 40);
                try
                {
                    expData.MasterSubcode = int.Parse(line.Substring(97, 7));
                }
                catch (Exception e)
                {
                    throw e;
                }
                try
                {
                    expData.CustomerSubcode = int.Parse(line.Substring(104, 7));
                }
                catch (Exception e)
                {
                    throw e;
                }
                expData.PortfolioName = line.Substring(112, 20);
                try
                {
                    expData.UserTrackingID = long.Parse(line.Substring(131, 40));
                }
                catch (Exception e)
                {
                    throw e;
                }
                try
                {
                    expData.TriggerCode = int.Parse(line.Substring(171, 5));
                }
                catch (Exception e)
                {
                    throw e;
                }
                expData.TriggerGroupCode = line.Substring(176, 3);
                expData.TriggerDescription = line.Substring(179, 60);
                try
                {
                    expData.AlertID = int.Parse(line.Substring(239, 10));
                }
                catch (Exception e)
                {
                    throw e;
                }
                try
                {
                    expData.TriggerPriority = int.Parse(line.Substring(249, 3));
                }
                catch (Exception e)
                {
                    throw e;
                }
                try
                {
                    string half = line.Substring(252);
                    half = half.Replace("     ", "^");
                    string[] halfSplit = half.Split(new Char[] { '^' });
                    List<string> strList = new List<string>();
                    foreach (string s in halfSplit)
                    {
                        string temp = s.Trim(new char[] { '^', ' ' });
                        temp = temp.TrimStart(new char[] { '0' });
                        if (temp != string.Empty)
                        {
                            strList.Add(temp);
                        }
                    }

                    strList[0] = strList.ElementAt(0).Insert(strList.ElementAt(0).Length - 4, "/");
                    strList[0] = strList.ElementAt(0).Insert(strList.ElementAt(0).Length - 7, "/");
                    Console.WriteLine(strList.ElementAt(0));
                    expData.EventDate = DateTime.Parse(strList.ElementAt(0));
                    try
                    {
                        expData.TypeOfBusiness = strList.ElementAt(1);
                    }
                    catch (Exception e)
                    {
                        expData.TypeOfBusiness = string.Empty;
                    }
                    Console.WriteLine(expData.EventDate);
                    Console.WriteLine(expData.TypeOfBusiness);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return expData;
        }

    


        static void Main(string[] args)
        {
            Program obj = new Program();
            List<ExperianData> expData =  obj.txtToDB();
        }


    }
}
