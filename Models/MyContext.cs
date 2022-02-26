using Microsoft.EntityFrameworkCore;
namespace belt.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Hangout> Hangouts {get;set;}
        public DbSet<Attendance> Attendances {get;set;}
    }
}