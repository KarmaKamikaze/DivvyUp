using Spectre.Console;

namespace DivvyUp;

public class ConsoleUI(List<Person>? people, DBUtility dbUtility)
{
    private bool _running;

    private readonly string[] _menuOptions =
    {
        "1. Add Person",
        "2. Split Costs",
        "3. Exit",
    };

    public void RunUI()
    {
        Console.WriteLine("\n\n\n");
        FigletFont font = FigletFont.Load("./Data/alligator2.flf");
        AnsiConsole.Write(
            new FigletText(font, "DivvyUp")
                .Centered()
                .Color(Color.Lime));
        Console.WriteLine("\n\n\n");

        _running = true;
        while (_running)
        {
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
                people = AddPerson();
                break;
            case var _ when string.Equals(choice, _menuOptions[1]):
                Compute.SplitCosts(people);
                break;
            case var _ when string.Equals(choice, _menuOptions[2]):
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
}
