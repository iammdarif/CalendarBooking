using CalendarBooking.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Repository
{
    public interface IBookingRepository
    {
        void AddBooking(DateTime appointmentDateTime);
        void DeleteBooking(DateTime appointmentDateTime);
        List<DateTime> FindBooking(DateTime appointmentDate);
        void KeepBooking(TimeSpan time);

        List<DateTime> GetAllAppointments();
    }
}
