using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;

namespace PetHaven.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    
}