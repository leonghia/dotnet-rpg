using Microsoft.EntityFrameworkCore;
using dotnet_rpg.Models;

namespace dotnet_rpg.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    // Create the migration for the "Characters" table in the database
    public DbSet<Character>? Characters { get; set; }

    // Create the migration for the "Users" table in the database
    public DbSet<User>? Users { get; set; }
}
