namespace ApplicationCore.Models;

public class BloodInventoryFilterModel
{
    public string ComponentType { get; set; }
    public string BloodType { get; set; }
    public string RhFactor { get; set; }
    public string Status { get; set; }

    public List<BloodInventoryRecord> Results { get; set; }
}