using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using ChuanHeLib.StringProcessing;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Collections;
using ChuanHeLib.IO;

namespace PrefixMidToLegalDoc
{
    class PrefixAdder
    {
        private FileInfo[] _filesInfoArr = null;

        //int count = 0;////


        public PrefixAdder(string dirPath)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(dirPath);
            _filesInfoArr = dirInfo.GetFiles();

            EditDistanceCalculator editDistCalculator = new EditDistanceCalculator();

            //Console.WriteLine(filesInfoArr[0].Name.ToLower());
            //Console.WriteLine("Bobbie's Kitchen Bakery & Cafe".ToLower());
            //int a = editDistCalculator.EditDistanceWFA("512127 roys kitchen- bankruptcy doc.pdf".ToLower(), "Roys Kitchen".ToLower());
            //Console.WriteLine(a);
            //int b = editDistCalculator.EditDistanceWFA("512127 roys kitchen- bankruptcy doc.pdf".ToLower(), "Bobbie's Kitchen Bakery & Cafe".ToLower());
            //Console.WriteLine(b);
        }

        public void Excute()
        {

            List<string[]> allMerchants = this.GetAllPrimaryMerchants();

            EditDistanceCalculator editDistCalculator = new EditDistanceCalculator();

            foreach (FileInfo f in _filesInfoArr)
            {
                int minDist = Int32.MaxValue;
                int editDist = Int32.MaxValue;
                bool collisionHappen = true;
                string[] candidateMerchant = null;
                string currentPrefix = CheckAndSetCurrentPrefix(f);
                string fileNameDigest = RemoveConfundingInfo(f, currentPrefix).Trim();
                double minDistPct = 1.00;
                double editDistPct = 1.00;
                Console.WriteLine("fileNameDigest: " + fileNameDigest);
                if (currentPrefix != "")
                {
                    for (int i = 0; i < allMerchants.Count; i++)
                    {
                        string[] singleMerchant = allMerchants[i];
                        //Check whether currenrPrefix is a valid suffix of the Mid we fuzzy searched from DB
                        //If true, then complement the prefix to make it has full Mid as prefix;
                        //if false, then it means the currentprefix doesnt match the Mid we fuzzy searched from DB. Just dont modify it.
                        if (IsSuffix(currentPrefix, singleMerchant[1]))
                        {
                            if (candidateMerchant == null||candidateMerchant[1]==singleMerchant[1])
                            {
                                candidateMerchant = singleMerchant;        
                            }
                            else
                            {
                                collisionHappen = true;
                                break;
                            }

                        }
                    }
                    if(collisionHappen == false)
                        PrefixComplement(f, candidateMerchant[1], currentPrefix);
                }

                else
                {
                    for (int i = 0; i < allMerchants.Count; i++)
                    {

                        string[] singleMerchant = allMerchants[i];
                        string DbaName = singleMerchant[2].Trim().ToLower();

                        editDist = editDistCalculator.EditDistanceWFA(fileNameDigest.ToLower(), DbaName);
                        minDistPct = Convert.ToDouble(minDist) / Convert.ToDouble(Math.Max(fileNameDigest.Length, DbaName.Length));
                        editDistPct = Convert.ToDouble(editDist) / Convert.ToDouble(Math.Max(fileNameDigest.Length, DbaName.Length));


                        if (minDistPct > editDistPct)
                        {
                            collisionHappen = false;
                            minDist = editDist;
                            candidateMerchant = singleMerchant;
                        }
                        else if (minDistPct == editDistPct)
                        {
                            Console.WriteLine(candidateMerchant[2].ToLower() + "||dis->" + minDist);
                            Console.WriteLine(DbaName + "||dis->" + editDist);
                            collisionHappen = true;
                        }
                    }

                    if (collisionHappen == true || minDistPct > 0.43) //when collision happen or minDisPct is bigger than x%, we dont change this file name
                    {
                        Console.WriteLine("collission!!Or disPct>40%!!");
                    }

                    else
                    {
                        Console.WriteLine("candidateMerchant: " + candidateMerchant[2]);
                        Console.WriteLine("minDist: " + minDist);
                        Console.WriteLine("" + editDistCalculator.EditDistanceWFA(fileNameDigest, candidateMerchant[2].Trim().ToLower()));
                        //ProcessingFileName(f, candidateMerchant[1]);
                        AddPrefix(f, candidateMerchant[1]);

                    }
                }

            }
        }


        private List<string[]> GetAllPrimaryMerchants()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCmd = null;

            try
            {
                dbCmd = db.GetStoredProcCommand("usp_getAllPrimaryMerchantDbaNameAndMid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            List<string[]> primaryMerchants = new List<string[]>();

            try
            {
                using (IDataReader reader = db.ExecuteReader(dbCmd))
                {
                    while (reader.Read())
                    {
                        string[] singleMerchant = new string[3];
                        singleMerchant[0] = reader["IntMid"].ToString().Trim();
                        singleMerchant[1] = reader["Mid"].ToString().Trim();
                        singleMerchant[2] = reader["BizDbaName"].ToString().Trim();
                        primaryMerchants.Add(singleMerchant);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return primaryMerchants;
        }


        private string RemoveConfundingInfo(FileInfo fileInfo, string currentPrefix)
        {
            string fileNameDigest = Path.GetFileNameWithoutExtension(fileInfo.Name);
            //All the noise strings must be checked to make sure none of them exist in Dba names in production DB
            string[] noise = { "bankruptcy", "judgement", "settlement", "agreement", "summons", "complaint", "affidivate", "schedule", "interrogatories" };



            for (int i = 0; i < noise.Length; i++)
            {
                if (fileNameDigest.Contains(noise[i]))
                {
                    int index = fileNameDigest.IndexOf(noise[i]);
                    fileNameDigest = fileNameDigest.Remove(index, noise[i].Length);
                }
            }

            //
            if (currentPrefix != "")
            {
                int index = fileNameDigest.IndexOf(currentPrefix);
                fileNameDigest = fileNameDigest.Remove(index, currentPrefix.Length);
            }

            //Console.WriteLine("fileNameDigest: " + fileNameDigest);
            return fileNameDigest;
        }


        private string CheckAndSetCurrentPrefix(FileInfo fileInfo)
        {
            StringBuilder sb = new StringBuilder();
            string currentPrefix = string.Empty;
            //<<---Check if original filename has a prefix with continuous number more than or equal 6 digits
            //-----If this kind of prefix exists, then that means it is a substring of Mid added by someone
            for (int i = 0; i < fileInfo.Name.Length; i++)
            {
                char ch = fileInfo.Name[i];
                if (!(char.IsDigit(ch) && ch != '.'))
                {
                    if (i < 6)
                        break;
                    else
                    {
                        currentPrefix = sb.ToString();
                        break;
                    }
                }
                else
                    sb.Append(ch);
            }
            //--->>
            Console.WriteLine(currentPrefix+"prefix");
            return currentPrefix;
        }

        //private void ProcessingFileName(FileInfo fileInfo, string Mid)
        //{

        //    if (_currentPrefix != "")
        //    {
        //        //Check whether currenrPrefix is a valid suffix of the Mid we fuzzy searched from DB
        //        //If true, then complement the prefix to make it has full Mid as prefix;
        //        //if false, then it means the currentprefix doesnt match the Mid we fuzzy searched from DB. Just dont modify it.
        //        if (IsSuffix(_currentPrefix, Mid))
        //            PrefixComplement(fileInfo, Mid, _currentPrefix);
        //    }
        //    else
        //        AddPrefix(fileInfo, Mid);
        //}


        private bool IsSuffix(string currentPrefix, string Mid)
        {
            int pl = currentPrefix.Length;
            int ml = Mid.Length;
            if (pl >= ml)
                return false;

            for (; pl > 0; pl--, ml--)
                if (currentPrefix[pl - 1] != Mid[ml - 1])
                    return false;
            return true;
        }

        private void PrefixComplement(FileInfo fileInfo, string Mid, string currentPrefix)
        {
            string newFileName = Mid + "_" + fileInfo.Name.Substring(currentPrefix.Length);
            string newFileFullName = fileInfo.DirectoryName + @"\" + newFileName;
            FileProcessor fp = new FileProcessor();
            if(File.Exists(newFileFullName))
                newFileFullName = fp.AddSuffixForDuplicateFile(newFileFullName);
            File.Move(fileInfo.FullName, newFileFullName);
        }

        private void AddPrefix(FileInfo fileInfo, string Mid)
        {
            string newFileFullName = fileInfo.DirectoryName + @"\" + Mid + "_" + fileInfo.Name;
            FileProcessor fp = new FileProcessor();
            if (File.Exists(newFileFullName))
                newFileFullName = fp.AddSuffixForDuplicateFile(newFileFullName);
            File.Move(fileInfo.FullName, newFileFullName);
        }
    }
}
