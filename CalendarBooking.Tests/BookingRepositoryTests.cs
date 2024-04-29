using CalendarBooking.Entities;
using CalendarBooking.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace CalendarBooking.Tests
{
    [TestFixture]
    public class BookingRepositoryTests
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly CalendarDbContext _context;
        private StringWriter _stringWriter;
        private TextWriter _originalConsoleOut;

        public BookingRepositoryTests()
        {
            _context = GetInMemoryContext();
            _bookingRepository = new BookingRepository( _context );
        }

        [SetUp]
        public void SetUp()
        {
            _stringWriter = new StringWriter();
            _originalConsoleOut = Console.Out;
            Console.SetOut(_stringWriter); 
        }

        [Test]
        public void BookingRepository_AddBooking_ShouldAddBooking() //valid appointment booking
        {
            DateTime _datetime = new DateTime(2024, 04, 29, 09, 00,00);
            string expectedMessage = $"Your appointment booking was successful. Appointment booked for : {_datetime.ToString()}";

            Console.SetOut(_stringWriter);
            _bookingRepository.AddBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));            
        }

        [Test]
        public void BookingRepository_AddBooking_ShouldNotAddDuplicateBooking() //Invalid/duplicate appointment booking
        {           

            DateTime _datetime = new DateTime(2024, 04, 28, 09, 00, 00);
            string expectedMessage = "Error booking the appointment : An Appointment already exists for this slot." ;
            _bookingRepository.AddBooking(_datetime);

            Console.SetOut(_stringWriter);
            _bookingRepository.AddBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }

        [Test]
        public void BookingRepository_AddBooking_ShouldOnlyAddAfter9AM() //Appointments can only be between 9 AM - 5PM
        {
            DateTime _datetime = new DateTime(2024, 04, 28, 08, 30, 00);
            string expectedMessage = "Error booking the appointment : Appointments can only booked between 9:00 AM and 5:00 PM for any day.";
            
            Console.SetOut(_stringWriter);
            _bookingRepository.AddBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }
        
        [Test]
        public void BookingRepository_AddBooking_ShouldOnlyAddBefore5PM() //Appointments can only be between 9 AM - 5PM
        {
            DateTime _datetime = new DateTime(2024, 04, 28, 17, 30, 00);
            string expectedMessage = "Error booking the appointment : Appointments can only booked between 9:00 AM and 5:00 PM for any day.";
            
            Console.SetOut(_stringWriter);
            _bookingRepository.AddBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }  

        [Test]
        public void BookingRepository_AddBooking_ShouldNotBookBetween4PMAnd5PMOnSecondDayOfThirdWeek() //Except from 4 PM to 5 PM on each second day of the third week of any month - this must be reserved and unavailable
        {
            DateTime _datetime = new DateTime(2024, 04, 16, 16, 30, 00);
            string expectedMessage = "Error booking the appointment : 4 PM to 5 PM on each second day of the third week of each month is reserved and unavailable.";
            
            Console.SetOut(_stringWriter);
            _bookingRepository.AddBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }

        [Test]
        public void BookingRepository_DeleteBooking_ShouldDeleteBooking() //Delete appointment from calendar
        {
            DateTime _datetime = new DateTime(2024, 04, 29, 09, 00, 00);
            string expectedMessage = $"Your appointment booking for {_datetime.ToString()} was deleted successfully.";

            _bookingRepository.AddBooking(_datetime); //appointment added to DB

            Console.SetOut(_stringWriter);
            _bookingRepository.DeleteBooking(_datetime);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }

        [Test]
        public void BookingRepository_DeleteBooking_ShouldNotDeleteInvalidBooking() //Delete non-existent appointment from calendar
        {
            DateTime _datetime = new DateTime(2024, 04, 29, 09, 00, 00);
            string expectedMessage = $"Error : Appointment {_datetime.ToString()} does not exist in the database.";

            Console.SetOut(_stringWriter);
            _bookingRepository.DeleteBooking(_datetime); //appointment does not exist in the DB

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        } 
        
        [Test]
        public void BookingRepository_FindBooking_ShouldReturnFreeTimeSlotsForTheDay() //Find available timeslots for any day
        {
            DateTime _datetime = new DateTime(2024, 04, 29, 00, 00, 00); 

            foreach (var item in GetAppointments())
            {
                _bookingRepository.AddBooking(item.AppointmenStartDateTime);
            }

            var bookingsForTheDay = _bookingRepository.FindBooking(_datetime);

            bool areTotallyDifferent = AreListsTotallyDifferent(_bookingRepository.GetAllAppointments(), bookingsForTheDay);

            Assert.IsTrue(areTotallyDifferent);
        }

        [Test]
        public void BookingRepository_KeepBooking_ShouldKeepBookingForTimeSlot() //Add Booking for the time slot for current day
        {
            TimeSpan timespan = TimeSpan.FromHours(11);
            DateTime today = DateTime.Today;
            DateTime timeSlot = today.Add(timespan);

            string expectedMessage = $"The time slot {timeSlot} has been reserved.";

            _bookingRepository.AddBooking(DateTime.Today.AddHours(9));
            _bookingRepository.AddBooking(DateTime.Today.AddHours(10));          

            Console.SetOut(_stringWriter);
            _bookingRepository.KeepBooking(timespan);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }

        [Test]
        public void BookingRepository_KeepBooking_ShouldNotKeepBookingForExistingTimeSlot() //Should not Add Booking for the existing time slot for current day
        {
            _bookingRepository.AddBooking(DateTime.Today.AddHours(9));
            _bookingRepository.AddBooking(DateTime.Today.AddHours(10));

            TimeSpan timespan = TimeSpan.FromHours(9);
            DateTime today = DateTime.Today;
            DateTime timeSlot = today.Add(timespan);

            string expectedMessage = $"The time slot {timeSlot} has already been reserved in the past. Please select a new time slot.";

            Console.SetOut(_stringWriter);
            _bookingRepository.KeepBooking(timespan);

            string consoleOutput = _stringWriter.ToString().Trim();
            Assert.IsTrue(consoleOutput.Contains(expectedMessage));
        }



        private CalendarDbContext GetInMemoryContext()
        { 
            var builder = new DbContextOptionsBuilder<CalendarDbContext>();
            builder.UseInMemoryDatabase("TestDatabase");

            return new CalendarDbContext(builder.Options);
        }

        private List<Appointment> GetAppointments()
        {
            return new List<Appointment>() 
            { 
                new Appointment { Id = 1, AppointmenStartDateTime = new DateTime(2024, 04, 29, 09, 00, 00), AppointmenEndDateTime = new DateTime(2024, 04, 29, 09, 30, 00) }, 
                new Appointment { Id = 2, AppointmenStartDateTime = new DateTime(2024, 04, 29, 09, 30, 00), AppointmenEndDateTime = new DateTime(2024, 04, 29, 10, 00, 00) }, 
                new Appointment { Id = 3, AppointmenStartDateTime = new DateTime(2024, 04, 29, 12, 00, 00), AppointmenEndDateTime = new DateTime(2024, 04, 29, 12, 30, 00) }, 
                new Appointment { Id = 4, AppointmenStartDateTime = new DateTime(2024, 04, 29, 12, 30, 00), AppointmenEndDateTime = new DateTime(2024, 04, 29, 13, 00, 00) } 
            };
        }

        private bool AreListsTotallyDifferent(List<DateTime> list1, List<DateTime> list2)
        {
            foreach (DateTime item1 in list1)
            {
                foreach (DateTime item2 in list2)
                {
                    if (EqualityComparer<DateTime>.Default.Equals(item1, item2))
                    {
                        return false; // Items are equal, lists are not totally different
                    }
                }
            }
            return true; // No items are equal, lists are totally different
        }

        [OneTimeTearDown]       
        public void DisposeContext()
        {
            _context.Dispose();
        } 
        
        
        [TearDown]
        public void DisposeStringWriter()
        {
            _stringWriter.Dispose();
            Console.SetOut(_originalConsoleOut);
        }
    }
}