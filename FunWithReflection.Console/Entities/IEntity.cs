namespace FunWithReflection.Console.Entities;

public interface IEntity<T>
{
    public T Id { get; set; }
}