using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    // Declare the "Characters" table in the database
    public DbSet<Character>? Characters { get; set; }
}
