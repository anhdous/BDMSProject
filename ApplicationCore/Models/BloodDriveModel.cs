namespace ApplicationCore.Models
{
public class BloodDriveModel
  {
      public int BDID { get; set; }
      public string Title { get; set; }
      public string Location { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public string Organizer { get; set; }
  }
}