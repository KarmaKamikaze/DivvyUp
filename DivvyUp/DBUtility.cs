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
            const string insertQuery = "INSERT INTO Person (Name, Owes) VALUES (@Name, @Owes)";

            using SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@Name", person.Name);
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
            const string selectQuery = "SELECT ID, Name, Owes FROM Person";

            using SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection);
            using SQLiteDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string name = reader["Name"].ToString()!;
                decimal owes = Convert.ToDecimal(reader["Owes"]);

                Person person = new Person(id, name, owes);

                people.Add(person);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return people;
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
