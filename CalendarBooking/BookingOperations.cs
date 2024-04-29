using CalendarBooking.Repository;
using System.Text.RegularExpressions;

namespace CalendarBooking
{
    public class BookingOperations
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingOperations(IBookingRepository bookingRepository, int userInput = 0)
        {
            _bookingRepository = bookingRepository;
        }

        public void CalendarBookingFunctions(int userInput)
        {
            DateTime parsedDate;
            var dateTimeValue = string.Empty;
            List<DateTime> availableTimeSlots = new List<DateTime>();
            var keepTimeSlot = string.Empty;
            string pattern = string.Empty;
            bool isMatch = false;

            switch (userInput)
            {
                case 1:
                    Console.Write("Please enter in the format \"ADD DD/MM hh:mm\" to add an appointment:");

                    var inputAddAppointment = Console.ReadLine();
                    pattern = @"^(?i)ADD \d{2}/\d{2} \d{2}:\d{2}$";
                    isMatch = Regex.IsMatch(inputAddAppointment, pattern);

                    if (isMatch)
                    {
                        dateTimeValue = inputAddAppointment?.ToLower().Trim().Substring(4);
                        
                        if (DateTime.TryParseExact(dateTimeValue, ["dd/MM HH:mm", "dd/MM hh:mm", "dd/MM h:mm"], null, System.Globalization.DateTimeStyles.AssumeLocal, out parsedDate))
                        {
                            _bookingRepository.AddBooking(parsedDate);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid Date Value.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Input string does not match the format.");
                        Console.ResetColor();
                    }
                    break;

                case 2:
                    Console.Write("Please enter in the format \"DELETE DD/MM hh:mm\" to remove an appointment:");

                    var inputDeleteAppointment = Console.ReadLine();
                    pattern = @"^(?i)DELETE \d{2}/\d{2} \d{2}:\d{2}$";
                    isMatch = Regex.IsMatch(inputDeleteAppointment, pattern);

                    if (isMatch)
                    {
                        dateTimeValue = inputDeleteAppointment?.Trim().Substring(7);

                        if (DateTime.TryParseExact(dateTimeValue, ["dd/MM HH:mm", "dd/MM hh:mm", "dd/MM h:mm"], null, System.Globalization.DateTimeStyles.AssumeLocal, out parsedDate))
                        {
                            _bookingRepository.DeleteBooking(parsedDate);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid Date Value.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Input string does not match the format.");
                        Console.ResetColor();
                    }
                    break;

                case 3:
                    Console.Write("Please enter in the format \"FIND DD/MM\" to find a free timeslot for the day:");

                    var inputFindTimeslot = Console.ReadLine();
                    pattern = @"^(?i)FIND \d{2}/\d{2}$";
                    isMatch = Regex.IsMatch(inputFindTimeslot, pattern);

                    if (isMatch)
                    {
                        dateTimeValue = inputFindTimeslot?.Trim().Substring(5);
                        if (DateTime.TryParseExact(dateTimeValue, "dd/MM", null, System.Globalization.DateTimeStyles.AssumeLocal, out parsedDate))
                        {
                            availableTimeSlots = _bookingRepository.FindBooking(parsedDate);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Following time slots are available on date {DateOnly.FromDateTime(parsedDate)}");
                            foreach (var item in availableTimeSlots)
                            {
                                Console.WriteLine(item);
                            }
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid Date Value.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Input string does not match the format.");
                        Console.ResetColor();
                    }
                    break;

                case 4:
                    Console.Write("Please enter in the format \"KEEP hh:mm\" to keep a timeslot for any day:");

                    var keepTimeSlotInput = Console.ReadLine();
                    pattern = @"^(?i)KEEP \d{2}:\d{2}$";
                    isMatch = Regex.IsMatch(keepTimeSlotInput, pattern);

                    if (isMatch)
                    {
                        dateTimeValue = keepTimeSlotInput?.Trim().Substring(5);
                        
                        if (dateTimeValue != null)
                        {
                            var parsedTimeSlot = TimeSpan.Parse(dateTimeValue);
                            _bookingRepository.KeepBooking(parsedTimeSlot);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid Date Value.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Input string does not match the format.");
                        Console.ResetColor();
                    }
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid input. Please enter numbers between 1 and {AppointmentBooking.optionsList.Count}");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
