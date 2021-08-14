using HeffayPresentsAchievements.Models;
using Microsoft.EntityFrameworkCore;

namespace HeffayPresentsAchievements.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Achievement>? Achievements { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<User>? Users { get; set; }
    }
}
