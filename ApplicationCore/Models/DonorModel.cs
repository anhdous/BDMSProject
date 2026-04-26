namespace ApplicationCore.Models
{
  public class DonorModel
  {
    public int DonorID { get; set; }
    public string FullName { get; set; }
    public DateTime DOB { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string BloodType { get; set; }
    public string RhFactor { get; set; }
    public DateTime? LastDonationDate { get; set; }
  }
}