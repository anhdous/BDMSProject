
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models
{
    public class ScheduleAppointmentModel
    {
        public int AppointmentID { get; set; }
        public int BDID { get; set; }

        public int DonorID { get; set; }
        public string Location { get; set; }

        [Required, DataType(DataType.Date)]
        public DateOnly? Date { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeOnly? Time { get; set; }
    }
}