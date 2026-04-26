namespace ApplicationCore.Models;

public class BloodUnitModel
{
    public int UnitID { get; set; }

    public string ComponentType { get; set; } = string.Empty;

    public string BloodType { get; set; } = string.Empty;

    public string RhFactor { get; set; } = string.Empty;

    public int Volume { get; set; }

    public DateTime CollectionDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string StorageInfo { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int DonationID { get; set; }

    public int? RequestID { get; set; }
}