using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ExperianData
{
    public DateTime AlertDate { get; set; }
    public int Bin { get; set; }
    public string BestBusinessName { get; set; }
    public string customerBusinessName { get; set; }
    public int MasterSubcode { get; set; }
    public int CustomerSubcode { get; set; }
    public string PortfolioName { get; set; }
    public long UserTrackingID { get; set; }
    public int TriggerCode { get; set; }
    public string TriggerGroupCode { get; set; }
    public string TriggerDescription { get; set; }
    public int AlertID { get; set; }
    public int TriggerPriority { get; set; }
    public DateTime EventDate { get; set; }
    public string TypeOfBusiness { get; set; }
}