namespace DivvyUp;

public class Person(string name)
{
    public Person(string name, decimal owes) : this(name)
    {
        Owes = owes;
    }

    public Person(int id, string name, decimal owes) : this(name, owes)
    {
        Id = id;
    }

    public int Id { get; set; }
    public string Name { get; } = name;
    public decimal Owes { get; set; }
}
