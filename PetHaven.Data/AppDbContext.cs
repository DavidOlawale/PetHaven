using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;

namespace PetHaven.Data.Model;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.Migrate();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Immunization> Immunizations { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<ForumThread> ForumThreads { get; set; }
    public DbSet<ForumComment> ForumComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var passwordHasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Id = 1,
            Email = "pethaven_superadmin@gmail.com",
            FirstName = "David",
            LastName = "Olaniran",
            Role = UserRoles.Administrator,
            ZipCode = "10027"
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "123ABCdef#");

        // Seed admin user
        modelBuilder.Entity<User>().HasData(adminUser);
    }

}