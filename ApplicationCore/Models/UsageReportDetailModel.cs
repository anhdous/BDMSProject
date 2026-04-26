namespace ApplicationCore.Models;

public class UsageReportDetailModel
{
  public string UnitID { get; set; }  
  public string ComponentType { get; set; }
  public string BloodType { get; set; }
  public string RhFactor { get; set; }
  public string Volume { get; set; }
  public DateTime RequestDate { get; set; }
  public string Quantity { get; set; }
  public string RequestHospital { get; set; }
    
}