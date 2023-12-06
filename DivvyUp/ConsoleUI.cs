using Spectre.Console;

namespace DivvyUp;

public class ConsoleUI(List<Person>? people, DBUtility dbUtility)
{
    private bool _running;

    private readonly string[] _menuOptions =
    {
        "1. Overview",
        "2. Add Person",
        "3. Remove Person",
        "4. Reset",
        "5. Exit",
    };

    public void RunUI()
    {
        Console.WriteLine("\n\n\n");
        FigletFont font = FigletFont.Load("./Data/alligator2.flf");
        AnsiConsole.Write(
            new FigletText(font, "DivvyUp")
                .Centered()
                .Color(Color.Lime));
        Console.WriteLine("\n\n");

        _running = true;
        while (_running)
        {
            Console.WriteLine("\n");
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
                people = DeletePerson();
                break;
            case var _ when string.Equals(choice, _menuOptions[3]):
                people = DeleteAllPeople();
                break;
            case var _ when string.Equals(choice, _menuOptions[4]):
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
        decimal paidAmount = AnsiConsole.Ask<decimal>("[olive]Enter the [green]amount paid[/] by the person:[/]");
        dbUtility.AddPerson(new Person(name, paidAmount));
        AnsiConsole.MarkupLine($"[olive]{name} added to the list.[/]");
        SplitCosts();
        return dbUtility.GetPeopleFromDatabase();
    }

    private void SplitCosts()
    {
        List<Person> owingPeople = dbUtility.GetPeopleFromDatabase()!;
        if (owingPeople?.Count == 0)
        {
            AnsiConsole.MarkupLine("[maroon]Add people to DivvyUp first.[/]");
            return;
        }

        decimal totalPaid = 0;

        foreach (Person person in owingPeople!)
        {
            totalPaid += person.Paid;
        }

        decimal share = totalPaid / owingPeople.Count;

        foreach (Person person in owingPeople)
        {
            person.Owes = share - person.Paid; // Adjust Owes based on the difference between share and paid
        }

        dbUtility.UpdatePeopleOwes(owingPeople);
    }

    private List<String> SimplifyDebts()
    {
        List<Person> peopleFromDatabase = dbUtility.GetPeopleFromDatabase()!;
        if (peopleFromDatabase?.Count == 0)
        {
            throw new ApplicationException("No people were added to DivvyUp.");
        }

        List<Person> positiveBalances = peopleFromDatabase!.Where(p => p.Owes > 0).OrderByDescending(p => p.Owes).ToList();
        List<Person> negativeBalances = peopleFromDatabase!.Where(p => p.Owes < 0).OrderBy(p => p.Owes).ToList();
        List<String> simplifiedDebtMessages = new List<string>();

        foreach (Person debtor in negativeBalances)
        {
            for (int j = 0; j < positiveBalances.Count; j++)
            {
                Person creditor = positiveBalances[j];

                decimal amountToTransfer = Math.Min(Math.Abs(debtor.Owes), creditor.Owes);

                debtor.Owes += amountToTransfer;
                creditor.Owes -= amountToTransfer;

                simplifiedDebtMessages.Add($"[olive][red]{creditor.Name}[/] pays [green]{amountToTransfer:C}[/] to [teal]{debtor.Name}[/][/]");

                if (creditor.Owes == 0)
                {
                    positiveBalances.RemoveAt(j);
                    j--;
                }

                if (debtor.Owes == 0)
                    break;
            }
        }

        return simplifiedDebtMessages;
    }

    private List<Person> DeleteAllPeople()
    {
        dbUtility.DeleteAllPeople();
        AnsiConsole.MarkupLine("[olive]All people have been [red]removed[/] and DivvyUp has been [lime]reset[/].[/]");
        return new List<Person>();
    }

    private List<Person>? DeletePerson()
    {
        if (people?.Count == 0)
        {
            AnsiConsole.MarkupLine("[maroon]Add people to DivvyUp first.[/]");
            return null;
        }

        Rule rule = new Rule("[red]Remove Person[/]");
        rule.LeftJustified();
        rule.RuleStyle("silver dim");
        AnsiConsole.Write(rule);

        Table table = new Table()
            .Title("Added People")
            .HeavyEdgeBorder();

        table.AddColumn(new TableColumn("ID"));
        table.AddColumn(new TableColumn("Name"));

        foreach (TableColumn column in table.Columns)
        {
            column.Padding(3, 3).Centered();
        }

        foreach (Person person in people!)
        {
            table.AddRow(new Markup($"[green]{person.Id}[/]"), new Markup($"[olive]{person.Name}[/]"));
        }

        Console.WriteLine("\n");
        AnsiConsole.Write(table);

        Console.WriteLine("\n");
        int personIdToDelete =
            AnsiConsole.Ask<int>("[olive]Please choose the [green]ID[/] of the person you wish to delete:[/]");
        Person personToDelete = people.FirstOrDefault(p => p.Id == personIdToDelete)!;
        if (AnsiConsole.Confirm(
                $"[olive]Are you sure you want to remove {personToDelete.Name} with ID [[[green]{personToDelete.Id}[/]]]?[/]",
                false))
        {
            dbUtility.DeletePerson(personIdToDelete);
            people.Remove(personToDelete);
            if (people.Count != 0)
            {
                decimal share = personToDelete.Owes / people.Count;
                foreach (Person person in people)
                {
                    person.Owes += share;
                }

                dbUtility.UpdatePeopleOwes(people);
            }
        }

        return dbUtility.GetPeopleFromDatabase();
    }

    private void DisplayOverview()
    {
        if (people?.Count == 0)
        {
            AnsiConsole.MarkupLine("[maroon]Add people to DivvyUp first.[/]");
            return;
        }

        Table outerTable = new Table()
            .Title("DivvyUp Overview")
            .HeavyEdgeBorder()
            .HideHeaders()
            .Centered();

        outerTable.AddColumn(new TableColumn("Header"));

        Table debtTable = new Table()
            .Centered();

        debtTable.AddColumn(new TableColumn("Name"));
        debtTable.AddColumn(new TableColumn("Paid"));
        debtTable.AddColumn(new TableColumn("Owes"));

        foreach (TableColumn column in debtTable.Columns)
        {
            column.Padding(3, 3).Centered();
        }

        IOrderedEnumerable<Person> sortedPeople = people!.OrderBy(p => p.Name);
        foreach (Person person in sortedPeople)
        {
            string owedColor = person.Owes < 0 ? "teal" : person.Owes > 0 ? "maroon" : "green";

            debtTable.AddRow(new Markup($"[olive]{person.Name}[/]"), new Markup($"[green]{person.Paid:C}[/]"),
                new Markup($"[{owedColor}]{person.Owes:C}[/]"));
        }

        outerTable.AddRow(debtTable);

        Table paymentTable = new Table()
            .Centered();

        paymentTable.AddColumn(new TableColumn("DivvyUp").Padding(3, 3).Centered());

        foreach (string message in SimplifyDebts())
        {
            paymentTable.AddRow(new Markup(message));
        }

        outerTable.AddRow(paymentTable);

        AnsiConsole.Write(outerTable);
    }
}
