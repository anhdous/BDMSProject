namespace ApplicationCore.Models;

public class UsageReportFilterModel
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public string ComponentType { get; set; }
  public string BloodType { get; set; }
  public string RhFactor { get; set; }  
  public List<UsageReportDetailModel> Results { get; set; }
  public bool HasSearched { get; set; }
}