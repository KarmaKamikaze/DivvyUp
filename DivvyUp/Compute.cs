using Spectre.Console;

namespace DivvyUp;

public class Compute
{
    public static void SplitCosts(List<Person> people)
    {
        Rule rule = new Rule("[red]Compute Costs[/]");
        rule.LeftJustified();
        rule.RuleStyle("silver dim");
        AnsiConsole.Write(rule);

        if (people.Count == 0)
        {
            AnsiConsole.MarkupLine("[maroon]Add people to the list first.[/]");
            return;
        }

        string inputCost = AnsiConsole.Ask<string>("[olive]Enter the total cost:[/]");
        if (!decimal.TryParse(inputCost, out decimal totalCost))
        {
            AnsiConsole.MarkupLine("[maroon]Invalid input for total cost.[/] [olive]Please enter a valid number.[/]");
            return;
        }

        decimal share = totalCost / people.Count;

        AnsiConsole.MarkupLine("[olive]Costs split evenly:[/]");

        foreach (var person in people)
        {
            person.Owes += share;
            AnsiConsole.MarkupLine($"[olive]{person.Name}[/]: [green]{share:C}[/]");
        }
    }
}
