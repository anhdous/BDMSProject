namespace ApplicationCore.Models;

public class BloodRequestModel
{
  public int RequestID { get; set; }
  public DateTime? RequestDate { get; set; }
  public string BloodType { get; set; }
  public string RhFactor { get; set; }
  public string ComponentType { get; set; }
  public int Quantity { get; set; }
  public string? UrgencyLevel { get; set; }
  public string? Status { get; set; }
  public int HospitalID { get; set; }
  public int AvailableUnits { get; set; }
}