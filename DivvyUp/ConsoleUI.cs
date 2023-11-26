using Spectre.Console;

namespace DivvyUp;

public class ConsoleUI(List<Person>? people, DBUtility dbUtility)
{
    private bool _running;

    private readonly string[] _menuOptions =
    {
        "1. Overview",
        "2. Add Person",
        "3. Split Costs",
        "4. Exit",
    };

    public void RunUI()
    {
        Console.WriteLine("\n\n\n");
        FigletFont font = FigletFont.Load("./Data/alligator2.flf");
        AnsiConsole.Write(
            new FigletText(font, "DivvyUp")
                .Centered()
                .Color(Color.Lime));

        _running = true;
        while (_running)
        {
            Console.WriteLine("\n\n\n");
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[olive]Please [green]choose[/] what to do next.[/]")
                    .AddChoices(_menuOptions)
            );

            HandleUserInput(choice);
        }
    }

    private void HandleUserInput(string choice)
    {
        switch (choice)
        {
            case var _ when string.Equals(choice, _menuOptions[0]):
                DisplayOverview();
                break;
            case var _ when string.Equals(choice, _menuOptions[1]):
                people = AddPerson();
                break;
            case var _ when string.Equals(choice, _menuOptions[2]):
                Compute.SplitCosts(people, dbUtility);
                break;
            case var _ when string.Equals(choice, _menuOptions[3]):
                if (AnsiConsole.Confirm("[olive]Are you sure you want to quit?[/]", false))
                {
                    _running = false;
                    Environment.Exit(0);
                }

                break;
            default:
                AnsiConsole.MarkupLine("[maroon]Invalid choice.[/] [olive]Please try again.[/]");
                break;
        }
    }

    private List<Person>? AddPerson()
    {
        Rule rule = new Rule("[red]Add New Person[/]");
        rule.LeftJustified();
        rule.RuleStyle("silver dim");
        AnsiConsole.Write(rule);
        string name = AnsiConsole.Ask<string>("[olive]Enter the [green]name[/] of the person:[/]");
        dbUtility.AddPerson(new Person(name));
        AnsiConsole.MarkupLine($"[olive]{name} added to the list.[/]");
        return dbUtility.GetPeopleFromDatabase();
    }

    private void DisplayOverview()
    {
        if (people?.Count == 0)
        {
            AnsiConsole.MarkupLine("[maroon]Add people to DivvyUp first.[/]");
            return;
        }

        Table table = new Table()
            .Title("DivvyUp Overview")
            .HeavyEdgeBorder();

        table.AddColumn(new TableColumn("Name"));
        table.AddColumn(new TableColumn("Paid")); // TODO: Implement different paid amounts
        table.AddColumn(new TableColumn("Owes"));

        foreach (TableColumn column in table.Columns)
        {
            column.Padding(3, 3).Centered();
        }

        foreach (Person person in people!)
        {
            table.AddRow(new Markup($"[olive]{person.Name}[/]"), new Markup("[green]paid[/]"),
                new Markup($"[maroon]{person.Owes:C}[/]"));
        }

        AnsiConsole.Write(table);
    }
}
