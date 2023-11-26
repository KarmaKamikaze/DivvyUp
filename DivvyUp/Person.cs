namespace DivvyUp;

public class Person(string name, decimal paid)
{
    public Person(string name, decimal paid, decimal owes) : this(name, paid)
    {
        Owes = owes;
    }

    public Person(int id, string name, decimal paid, decimal owes) : this(name, paid, owes)
    {
        Id = id;
    }

    public int Id { get; set; }
    public string Name { get; } = name;
    public decimal Paid { get; set; } = paid;
    public decimal Owes { get; set; }
}
