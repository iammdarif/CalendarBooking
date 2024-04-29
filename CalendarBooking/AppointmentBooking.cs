
namespace CalendarBooking
{
    public static class AppointmentBooking
    {
        public static List<string> optionsList = new List<string>
            {
                "1. ADD DD/MM hh:mm to add an appointment",
                "2. DELETE DD/MM hh:mm to remove an appointment",
                "3. FIND DD/MM to find a free timeslot for the day",
                "4. KEEP hh:mm to keep a timeslot for any day."
            };

        public static void InitialOptions()
        {
            Console.WriteLine("Please enter your choice from the below options:" + Environment.NewLine);

            foreach (var item in optionsList)
            {
                Console.WriteLine(item);
            }
        }
    }
}
