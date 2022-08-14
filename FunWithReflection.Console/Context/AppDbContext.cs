using FunWithReflection.Console.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FunWithReflection.Console.Context;

public class AppDbContext : DbContext
{
    public DbSet<Hero>? Heroes { get; set; }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
        keepAliveConnection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(keepAliveConnection).Options;
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hero>().HasKey(k => k.Id);
        modelBuilder.Entity<Hero>().Property(p => p.Name).IsRequired();
        modelBuilder.Entity<Hero>().Property(p => p.Age).IsRequired();
    }

    public async Task AddData()
    {
        var listOfHeroes = new List<Hero>
        {
            new Hero { Id = 1, Name = "Spider-man", Age = 18 },
            new Hero { Id = 2, Name = "Iron Man", Age = 50 },
            new Hero { Id = 3, Name = "Robocop", Age = 30 },
            new Hero { Id = 4, Name = "Gordon Freeman", Age = 27 },
            new Hero { Id = 5, Name = "Alyx Wance", Age = 25 }
        };

        await this.Set<Hero>().AddRangeAsync(listOfHeroes);
    }
}