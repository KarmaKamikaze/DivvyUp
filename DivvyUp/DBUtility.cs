using System.Configuration;
using System.Data.SQLite;
using Spectre.Console;

namespace DivvyUp;

public class DBUtility
{
    private readonly string? _dbPath = ConfigurationManager.AppSettings["DBPath"];

    private readonly string? _connectionString =
        ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;

    public void InitializeDatabase()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        try
        {
            connection.Open();

            // Create the Person table if it does not exist
            const string createTableQuery = """
                                            CREATE TABLE IF NOT EXISTS Person (
                                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Name TEXT,
                                                Paid DECIMAL,
                                                Owes DECIMAL
                                            )
                                            """;

            using SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection);
            createTableCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public void AddPerson(Person person)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        try
        {
            connection.Open();

            // Insert the new person into the Person table
            const string insertQuery = "INSERT INTO Person (Name, Paid, Owes) VALUES (@Name, @Paid, @Owes)";

            using SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@Name", person.Name);
            insertCommand.Parameters.AddWithValue("@Paid", person.Paid);
            insertCommand.Parameters.AddWithValue("@Owes", person.Owes);

            insertCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public List<Person>? GetPeopleFromDatabase()
    {
        List<Person> people = new List<Person>();

        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return null;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        try
        {
            connection.Open();

            // Retrieve all people from the Person table
            const string selectQuery = "SELECT ID, Name, Paid, Owes FROM Person";

            using SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection);
            using SQLiteDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string name = reader["Name"].ToString()!;
                decimal paid = Convert.ToDecimal(reader["Paid"]);
                decimal owes = Convert.ToDecimal(reader["Owes"]);

                Person person = new Person(id, name, paid, owes);

                people.Add(person);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return people;
    }

    public void DeletePerson(int personId)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        try
        {
            connection.Open();

            // Delete the person from the Person table
            const string deleteQuery = "DELETE FROM Person WHERE ID = @ID";

            using SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@ID", personId);
            deleteCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public void DeleteAllPeople()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        try
        {
            connection.Open();

            // Delete all people from the Person table
            const string deleteAllQuery = "DELETE FROM Person";

            using SQLiteCommand deleteAllCommand = new SQLiteCommand(deleteAllQuery, connection);
            deleteAllCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public void UpdatePeopleOwes(List<Person> people)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            AnsiConsole.MarkupLine("[red]SQLite connection string not found in [olive]App.config.[/][/]");
            return;
        }

        using SQLiteConnection connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using SQLiteTransaction transaction = connection.BeginTransaction();
        try
        {
            foreach (Person updatedPerson in people)
            {
                // Update the Owes attribute for each person in the Person table
                const string updateQuery = "UPDATE Person SET Paid = @Paid, Owes = @Owes WHERE ID = @ID";

                using SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection, transaction);
                updateCommand.Parameters.AddWithValue("@Paid", updatedPerson.Paid);
                updateCommand.Parameters.AddWithValue("@Owes", updatedPerson.Owes);
                updateCommand.Parameters.AddWithValue("@ID", updatedPerson.Id);

                updateCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            AnsiConsole.WriteException(ex);
        }
    }

    public void CreateDB()
    {
        Directory.CreateDirectory("./Data");
        if (!Path.Exists(_dbPath))
        {
            SQLiteConnection.CreateFile(_dbPath);
        }
    }
}
