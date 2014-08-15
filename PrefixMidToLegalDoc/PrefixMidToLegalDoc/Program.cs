using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace PrefixMidToLegalDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPath = ConfigurationManager.AppSettings["dirPath"];
            PrefixAdder adder = new PrefixAdder(dirPath);
            adder.Excute();
            //string st = "123   456 7 8  9 10";
            //string[] sa = st.Split(' ');
            //foreach (string s in sa)
            //    Console.WriteLine(s+"||");
            //int a=4,b=3,c=7;
            //double r = 1.00;
            //r = Convert.ToDouble(a)/ Convert.ToDouble(Math.Max(b, c));
            //Console.WriteLine(r);
        }
    }
}
