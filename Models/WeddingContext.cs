    using Microsoft.EntityFrameworkCore; 
    namespace WeddingPlanner.Models
    {
        public class WeddingContext : DbContext
        {
            public WeddingContext(DbContextOptions<WeddingContext> options) : base(options) { }
            
            // "users" table is represented by this DbSet "Users"
            public DbSet<Users> Users {get;set;}

            public DbSet<Wedding> Wedding {get; set;}

            public DbSet<Guest> Guest {get; set;}
        }
    }
    