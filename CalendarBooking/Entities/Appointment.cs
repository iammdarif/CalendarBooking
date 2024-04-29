using System.ComponentModel.DataAnnotations;

namespace CalendarBooking.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AppointmenStartDateTime { get; set; }

        public DateTime AppointmenEndDateTime { get; set; }

    }
}
