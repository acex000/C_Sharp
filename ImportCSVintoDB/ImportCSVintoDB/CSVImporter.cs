using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
//using Microsoft.SqlServer.Management.Smo;


namespace ImportCSVintoDB
{
    class CSVImporter
    {
        private string dirPath = string.Empty;
        private DirectoryInfo dirInfo = null;
        private Database db = null;


        //Constructor
        /// <summary>
        /// Pre stored procedure name is necessary as parameter
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="procedureName"></param>
        public CSVImporter()
        {           
            this.db = DatabaseFactory.CreateDatabase();
        }

        /// <summary>
        /// A directory path of files are necessary. All file under this directory will be scanned
        /// </summary>
        /// <param name="Path"></param>
        public void ImportMultipleFiles(string Path)
        {
            this.dirPath = Path;
            this.dirInfo = Directory.CreateDirectory(dirPath);
            FileInfo[] filesInfo = dirInfo.GetFiles();
            foreach (FileInfo f in filesInfo)
            {
                ImportSingleFile(f);
                Console.WriteLine("finished file :"+ f.Name);
            }
            //ImportSingleFile(filesInfo[0]);
        }

        /// <summary>
        /// FileInfo of the target file is necessary as parameter
        /// </summary>
        /// <param name="file"></param>
        public void ImportSingleFile(FileInfo file)
        {
            StreamReader myStreamRder = new StreamReader(file.FullName);
            DataTable dataTable = new DataTable();

            string[] columnNames = null;
            string[] rowValues = null;
            string rowInStream = string.Empty;
            bool isFirstRow = true;
            int c = 0;
            while(!myStreamRder.EndOfStream)
            {
                c++;
                rowInStream = myStreamRder.ReadLine().Trim();
                
                if (isFirstRow == true)
                {
                    isFirstRow = false;
                    columnNames = rowInStream.Split(',');

                    this.SetupColumn(dataTable, columnNames);                   
                }
                else
                {
                    rowValues = rowInStream.Split(',');
                    DataRow dataRow = dataTable.NewRow();

                    //if (rowValues.Length != 12)
                    //{
                    //    Console.WriteLine(c+"RR"+rowValues.Length);
                    //    Console.ReadLine();
                    //}

                    for (int i =0 ; i<columnNames.Length ; i++ )
                    {
                        if (i < rowValues.Length)
                        {
                            //Console.WriteLine(c + " " + dataTable.Columns[i].DataType + ">" );
                            //Console.WriteLine(dataRow[columnNames[i]]);
                            if (i == 0 )
                            {
                                dataRow[columnNames[i]] = string.IsNullOrEmpty(rowValues[i]) ? 0 : Convert.ToInt32(rowValues[i]);
                            }
                            else
                            {
                                dataRow[columnNames[i]] = rowValues[i] == null ? string.Empty : rowValues[i].ToString();
                            }                          
                        }
                        else 
                        {
                            //Console.WriteLine(c + " " + dataTable.Columns[i].DataType + ">" );
                            //Console.WriteLine(dataRow[columnNames[i]]);
                            if (i == 0 )
                            {
                                dataRow[columnNames[i]] = 0;
                            }
                            else
                            {
                                dataRow[columnNames[i]] = string.Empty;
                            };
                        }


                        //Console.WriteLine(c + " " + dataRow[columnNames[i]]);
                    }
                    dataTable.Rows.Add(dataRow);
                    //if (c == 2)
                    //    Console.ReadLine();
                }

                //if (c == 4715)
                //    break;

            }
            myStreamRder.Close();
            myStreamRder.Dispose();
            //int tt = 1;
            //foreach (DataRow dr in dataTable.Rows)
            //{
            //    for (int j = 0; j < columnNames.Length; j++)
            //    {
            //        if (j == 7)
            //        {
            //            Console.WriteLine("row # is: " + tt);
            //            Console.WriteLine(dr[columnNames[j]].ToString());
            //            DateTime dt = Convert.ToDateTime(dr[columnNames[j]].ToString());
            //            Console.WriteLine(dt);





            //        }

            //    }
            //    tt++;
            //    //Console.WriteLine(dr[columnNames[j]].ToString());
            //}

           

            string cstr = db.ConnectionString;
            SqlBulkCopy sqlBulkcp = new SqlBulkCopy(cstr);
            //Console.WriteLine(cstr);
            sqlBulkcp.DestinationTableName = "[Test].[dbo].[LeadStatusHistory]";
            try
            {
                sqlBulkcp.WriteToServer(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
 
        }


        public void SetupColumn(DataTable dataTable, string[] columnNames)
        {
            DataColumn[] dataColumns = new DataColumn[columnNames.Length];
            //for (int i =0 ; i<columnNames.Length ; i++)
            //{
               
            //}
            dataColumns[0] = new DataColumn(columnNames[0].ToUpper(), typeof(int));
            dataColumns[1] = new DataColumn(columnNames[1].ToUpper(), typeof(string));
            dataColumns[2] = new DataColumn(columnNames[2].ToUpper(), typeof(string));
            dataColumns[3] = new DataColumn(columnNames[3].ToUpper(), typeof(string));
            dataColumns[4] = new DataColumn(columnNames[4].ToUpper(), typeof(string));
            dataColumns[5] = new DataColumn(columnNames[5].ToUpper(), typeof(string));
            dataColumns[6] = new DataColumn(columnNames[6].ToUpper(), typeof(string));
            dataColumns[7] = new DataColumn(columnNames[7].ToUpper(), typeof(string));
            dataColumns[8] = new DataColumn(columnNames[8].ToUpper(), typeof(string));
            dataColumns[9] = new DataColumn(columnNames[9].ToUpper(), typeof(string));
            dataColumns[10] = new DataColumn(columnNames[10].ToUpper(), typeof(string));
            dataColumns[11] = new DataColumn(columnNames[11].ToUpper(), typeof(string));

            for (int i = 0; i < columnNames.Length; i++)
            {
                dataTable.Columns.Add(dataColumns[i]);
            }

            //dataColumns[0].DataType = DataType.Int;
            //dataColumns[1].DataType = DataType.VarChar(50);
            
        }

    }
}
