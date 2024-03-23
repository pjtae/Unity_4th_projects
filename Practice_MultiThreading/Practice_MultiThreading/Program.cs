namespace Practice_MultiThreading;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    static Barista HireBarista(string nickname)
    {
        return new Barista(nickname);
    }
}

public class Barista
{
    public Barista(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public Barista GoToWork()
    {
        Console.WriteLine($"바리스타 {Name} 이 출근했습니다...");
        return this;
    }
}