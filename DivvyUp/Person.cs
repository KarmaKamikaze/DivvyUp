namespace DivvyUp;

public class Person(string name)
{
    public string Name { get; } = name;
    public decimal Owes { get; set; } = 0;
}
