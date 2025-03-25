using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;

namespace PetHaven.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.Migrate();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Immunization> Immunizations { get; set; }
    public DbSet<Checkup> Checkups { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{

    //}

}