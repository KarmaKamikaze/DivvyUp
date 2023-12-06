using DivvyUp;

namespace DivvyUpTests;

public class PersonTests
{
    [Fact]
    public void ConstructorWithNameAndPaid_ShouldSetProperties()
    {
        // Arrange
        string name = "John";
        decimal paid = 100.0m;

        // Act
        Person person = new Person(name, paid);

        // Assert
        Assert.Equal(name, person.Name);
        Assert.Equal(paid, person.Paid);
        Assert.Equal(0.0m, person.Owes);
    }

    [Fact]
    public void ConstructorWithIdNamePaidAndOwes_ShouldSetProperties()
    {
        // Arrange
        int id = 1;
        string name = "Alice";
        decimal paid = 50.0m;
        decimal owes = 20.0m;

        // Act
        Person person = new Person(id, name, paid, owes);

        // Assert
        Assert.Equal(id, person.Id);
        Assert.Equal(name, person.Name);
        Assert.Equal(paid, person.Paid);
        Assert.Equal(owes, person.Owes);
    }

    [Fact]
    public void SetPaid_ShouldUpdatePaid()
    {
        // Arrange
        string name = "Bob";
        decimal initialPaid = 50.0m;
        Person person = new Person(name, initialPaid);

        // Act
        decimal newPaid = 75.0m;
        person.Paid = newPaid;

        // Assert
        Assert.Equal(newPaid, person.Paid);
    }
}
