using CalendarBooking.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CalendarBooking
{
    public class Program
    {       

        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    // Configure logging to only log warnings and above
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<CalendarDbContext>(options =>
                    {
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                    });
                    services.AddTransient<IBookingRepository, BookingRepository>();
                })
                .Build();

            using (var serviceScope = host.Services.CreateScope())
            {             

                var services = serviceScope.ServiceProvider;
                var userService = services.GetRequiredService<IBookingRepository>();

                string userChoice = string.Empty;

                do
                {
                    AppointmentBooking.InitialOptions();
                    Console.WriteLine(Environment.NewLine);

                    Console.Write($"Enter your choice from one of the above {AppointmentBooking.optionsList.Count} options: ");

                    Int16.TryParse(Console.ReadLine(), out short userInput);

                    BookingOperations bookingApp = new BookingOperations(userService, userInput);
                    bookingApp.CalendarBookingFunctions(userInput);

                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("Would you like to continue. Y/N");

                    userChoice = Console.ReadLine();


                    if (userChoice.ToLower() != "y" && userChoice.ToLower() != "n")
                    {
                        do
                        {
                            Console.WriteLine("Please select either \"Y\" or \"N\".");
                            Console.WriteLine("Would you like to continue. Y/N");
                            userChoice = Console.ReadLine();

                        } while (userChoice.ToLower() != "y" && userChoice.ToLower() != "n");
                    }

                } while (userChoice.ToLower() == "y");               
            }
        }

    }
}
