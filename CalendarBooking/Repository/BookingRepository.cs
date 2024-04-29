using CalendarBooking.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly CalendarDbContext _context;

        public BookingRepository(CalendarDbContext context)
        {
            _context = context;
        }

        public void AddBooking(DateTime appointmentDateTime)
        {
            if (appointmentDateTime.TimeOfDay < TimeSpan.FromHours(9) || appointmentDateTime.TimeOfDay >= TimeSpan.FromHours(17))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error booking the appointment : Appointments can only booked between 9:00 AM and 5:00 PM for any day.");
                Console.ResetColor();
                return;
            }
            else
            {
                if (appointmentDateTime.DayOfWeek == DayOfWeek.Tuesday && 
                    appointmentDateTime.Day >= 15 && 
                    appointmentDateTime.Day <= 21 && 
                    appointmentDateTime.TimeOfDay >= TimeSpan.FromHours(16) &&
                    appointmentDateTime.TimeOfDay < TimeSpan.FromHours(17))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error booking the appointment : 4 PM to 5 PM on each second day of the third week of each month is reserved and unavailable.");
                    Console.ResetColor();
                    return;
                }

                var findAppointmentExists = _context.Appointments.FirstOrDefault(x =>
                                    x.AppointmenStartDateTime == appointmentDateTime);

                if (findAppointmentExists == null)
                {
                    var appointmentToAdd = new Appointment { AppointmenStartDateTime = appointmentDateTime, AppointmenEndDateTime = appointmentDateTime.AddMinutes(30) };
                    _context.Appointments.Add(appointmentToAdd);
                    _context.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Your appointment booking was successful. Appointment booked for : {appointmentToAdd.AppointmenStartDateTime.ToString()}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error booking the appointment : An Appointment already exists for this slot.");
                    Console.ResetColor();
                    return;
                }                
            }                    
        }
        
        public void DeleteBooking(DateTime appointmentDateTime)
        {
            var appointmentToDelete = _context.Appointments.FirstOrDefault(x => 
                x.AppointmenStartDateTime == appointmentDateTime);

            if (appointmentToDelete == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error : Appointment {appointmentDateTime.ToString()} does not exist in the database.");
                Console.ResetColor();
                return;
            }

            _context.Appointments.Remove(appointmentToDelete);
            _context.SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Your appointment booking for {appointmentDateTime.ToString()} was deleted successfully.");
            Console.ResetColor();
        }
        
        public List<DateTime> FindBooking(DateTime appointmentDate)
        {
            List<DateTime> allTimeSlotsList = new List<DateTime>();          

            DateTime startTime = appointmentDate.Date.AddHours(9);
            DateTime endTime = appointmentDate.Date.AddHours(17);

            for (DateTime current = startTime; current < endTime; current = current.AddMinutes(30))
            {                
                allTimeSlotsList.Add(current);
            }

            if (appointmentDate.DayOfWeek == DayOfWeek.Tuesday && appointmentDate.Day >= 15 && appointmentDate.Day <= 21)
            {
                for (DateTime current = appointmentDate.Date.AddHours(16); current < appointmentDate.Date.AddHours(17); current = current.AddMinutes(30))
                {
                    allTimeSlotsList.Remove(current);
                }
            }

            var bookedTimeSlots = _context.Appointments
                                    .Where(x => x.AppointmenStartDateTime.Date == appointmentDate.Date)
                                    .Select(x => x.AppointmenStartDateTime)
                                    .ToList();

            var freeTimeSlots = allTimeSlotsList.Except(bookedTimeSlots).ToList();

            return freeTimeSlots;                 
        }

        public void KeepBooking(TimeSpan time)
        {
            DateTime today = DateTime.Today;
            DateTime timeSlot = today.Add(time);

            if (timeSlot.TimeOfDay < TimeSpan.FromHours(9) || timeSlot.TimeOfDay >= TimeSpan.FromHours(17))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Appointment booking timeslot must be between 9:00 AM and 5:00 PM");
                Console.ResetColor();
                return;
            }
            else
            {
                if (timeSlot.DayOfWeek == DayOfWeek.Tuesday &&
                    timeSlot.Day >= 15 &&
                    timeSlot.Day <= 21 &&
                    timeSlot.TimeOfDay >= TimeSpan.FromHours(16) &&
                    timeSlot.TimeOfDay < TimeSpan.FromHours(17))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error booking the appointment : 4 PM to 5 PM on each second day of the third week of each month is reserved and unavailable.");
                    Console.ResetColor();
                    return;
                }

                var findAppointmentExists = _context.Appointments.FirstOrDefault(x =>
                                    x.AppointmenStartDateTime == timeSlot);

                if (findAppointmentExists == null)
                {
                    _context.Appointments.Add(new Appointment { AppointmenStartDateTime = timeSlot, AppointmenEndDateTime = timeSlot.AddMinutes(30) });
                    _context.SaveChanges();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"The time slot {timeSlot} has been reserved.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The time slot {timeSlot} has already been reserved in the past. Please select a new time slot.");
                    Console.ResetColor();
                }                
            }            
        }

        public List<DateTime> GetAllAppointments()
        { 
            return _context.Appointments.Select(x => x.AppointmenStartDateTime).ToList();
        }
    }
}
