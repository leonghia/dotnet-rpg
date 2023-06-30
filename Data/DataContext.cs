using Microsoft.EntityFrameworkCore;
using dotnet_rpg.Models;

namespace dotnet_rpg.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    // Seeding the Skills table
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "Rabona", Damage = 10 },
            new Skill { Id = 2, Name = "Step Over", Damage = 20},
            new Skill { Id = 3, Name = "Nutmeg", Damage = 15}
        );
    }

    // Create the migration for the "Characters" table in the database
    public DbSet<Character>? Characters { get; set; }

    // Create the migration for the "Users" table in the database
    public DbSet<User>? Users { get; set; }
    
    // So on...
    public DbSet<Weapon> Weapons { get; set; }
    public DbSet<Skill> Skills { get; set; }
}
