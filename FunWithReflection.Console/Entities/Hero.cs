namespace FunWithReflection.Console.Entities;

public class Hero : IEntity<int>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}
