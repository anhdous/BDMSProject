namespace ApplicationCore.Models;

public class BloodInventoryRecord
{
    public int UnitID { get; set; }
    public string ComponentType { get; set; }
    public string BloodType { get; set; }
    public string RhFactor { get; set; }
    public int Volume { get; set; }
    public DateTime CollectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string StorageInfo { get; set; }
     public string Status { get; set; }
    
}