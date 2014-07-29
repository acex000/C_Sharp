using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Data.Common;
using System.Data;

namespace DBpractice
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p1 = new Program();
            p1.ReadDBTest(2);
            p1.InsertDBTest("new_Custom", 500);
            p1.UpdateDBTest(250, 3);
        }

        public void ReadDBTest(int customID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCmd = null;
            try
            {
                dbCmd = db.GetStoredProcCommand("select_all");
                db.AddInParameter(dbCmd, "Custom_ID", System.Data.DbType.Int32, customID);
                using (IDataReader reader = db.ExecuteReader(dbCmd))
                {
                    while (reader.Read())
                    {
                        try
                        {
                            Console.WriteLine(reader["Custom_Name"].ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (dbCmd != null)
                    dbCmd.Dispose();
                dbCmd = null;
                db = null;
            }
        }

        public void InsertDBTest(string newName, int newLoan)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCmd = null;
            try
            {
                dbCmd = db.GetStoredProcCommand("insert_new_record");
                db.AddInParameter(dbCmd, "new_name", System.Data.DbType.String, newName);
                db.AddInParameter(dbCmd, "new_loan", DbType.Int32, newLoan);
                db.ExecuteNonQuery(dbCmd);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if (dbCmd != null)
                    dbCmd.Dispose();
                dbCmd = null;
                db = null;
            }
        }

        public void UpdateDBTest(int updatedLoan, int customID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCmd = null;
            try
            {
                dbCmd = db.GetStoredProcCommand("update_loan");
                db.AddInParameter(dbCmd, "Updated_loan", DbType.Int32, updatedLoan);
                db.AddInParameter(dbCmd, "C_ID", DbType.Int32, customID);
                db.ExecuteNonQuery(dbCmd);
            }  
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if (dbCmd != null)
                    dbCmd.Dispose();
                dbCmd = null;
                db = null;
            }
        }
    }
}
