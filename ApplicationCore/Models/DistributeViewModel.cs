using ApplicationCore.Models;

public class DistributeViewModel
{
    public BloodRequestModel Request { get; set; }
    public List<BloodUnitModel> AvailableUnits { get; set; }

    public int AssignedCount { get; set; }
    public int Remaining { get; set; }
}