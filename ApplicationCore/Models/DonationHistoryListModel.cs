namespace ApplicationCore.Models;

public class DonationHistoryListModel
  {
    public DateTime DonationDate { get; set; }
    public string Location { get; set; }
    public int CollectedVolume { get; set; }
    public string StaffName { get; set; }
  }
