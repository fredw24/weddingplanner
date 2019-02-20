using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
    namespace WeddingPlanner.Models
    {
        public class WeddingContext : DbContext
        {
            public WeddingContext(DbContextOptions<WeddingContext> options) : base(options) { }
            public DbSet<Users> Users {get;set;}
            public DbSet<Wedding> Wedding {get; set;}
            public DbSet<Guest> Guest {get; set;}

            public void createUser(HttpContext context, Users user)
            {
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                user.Password = Hasher.HashPassword(user, user.Password);
                Add(user);
                SaveChanges();
                context.Session.SetInt32("id", user.UserId);
            }
        }
    }
    