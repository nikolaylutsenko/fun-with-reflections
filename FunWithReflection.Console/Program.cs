using FunWithReflection.Console;
using FunWithReflection.Console.Context;
using FunWithReflection.Console.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

class Program
{
    public static async Task Main(params string[] attr)
    {
        var _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
        var DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();

        await DbContext.AddData();

        var repository = new Repository<Hero>(DbContext);
        var asT0 = await repository.GetAsync<Hero>(x => x.Id == 1);
        var asT1 = await repository.GetAsync<List<Hero>>(x => x.Name.Contains("man"));
    }
}
