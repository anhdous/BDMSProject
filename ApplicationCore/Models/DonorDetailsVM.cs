using ApplicationCore.Models;

public class DonorDetailsVM
{
    public DonorModel Donor { get; set; }
    public List<DonationHistoryListModel> Donations { get; set; }
}