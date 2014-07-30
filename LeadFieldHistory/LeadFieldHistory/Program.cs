using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Data.Common;
using System.Data;


namespace LeadFieldHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            LeadFieldHisProcesser processor = new LeadFieldHisProcesser("Owner");
            processor.Processing();
            //processor.saveToDB();
        }
    }
}
