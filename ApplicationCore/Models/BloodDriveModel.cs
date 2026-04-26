using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models
{
public class BloodDriveModel
  {
      public int BDID { get; set; }
      [Required(ErrorMessage ="Event Name is required.")]
      public string Title { get; set; }
      [Required]
      public string Location { get; set; }
      [Required(ErrorMessage = "Start Date is required.")]
      public DateTime? StartDate { get; set; }
      [Required(ErrorMessage = "End Date is required.")]
      public DateTime? EndDate { get; set; }
      [Required]
      public string Organizer { get; set; }
  }
}