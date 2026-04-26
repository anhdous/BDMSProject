using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models
{
    public class AppointmentListModel
    {
        public int AppointmentID { get; set; }
        public string Location { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }
        public int? BDID { get; set; }

    }
}