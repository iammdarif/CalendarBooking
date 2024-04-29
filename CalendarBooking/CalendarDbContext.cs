using CalendarBooking.Entities;
using Microsoft.EntityFrameworkCore;


namespace CalendarBooking
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
