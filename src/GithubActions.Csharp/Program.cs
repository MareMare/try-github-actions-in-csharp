// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.Binding;

Console.WriteLine("Hello, World!");

var firstNameOption = new Option<string>(
    name: "--first-name",
    description: "Person.FirstName");

var lastNameOption = new Option<string>(
    name: "--last-name",
    description: "Person.LastName");

var rootCommand = new RootCommand
{
    firstNameOption,
    lastNameOption,
};

rootCommand.SetHandler(
    DoRootCommand,
    new PersonBinder(firstNameOption, lastNameOption));

await rootCommand.InvokeAsync(args);

static void DoRootCommand(Person aPerson)
{
    Console.WriteLine($"::set-output name=summary-details::{aPerson.FirstName} {aPerson.LastName}");
}

internal class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

internal class PersonBinder: BinderBase<Person>
{
    private readonly Option<string> _firstNameOption;
    private readonly Option<string> _lastNameOption;

    public PersonBinder(Option<string> firstNameOption, Option<string> lastNameOption)
    {
        _firstNameOption = firstNameOption;
        _lastNameOption = lastNameOption;
    }

    protected override Person GetBoundValue(BindingContext bindingContext) =>
        new Person
        {
            FirstName = bindingContext.ParseResult.GetValueForOption(_firstNameOption),
            LastName = bindingContext.ParseResult.GetValueForOption(_lastNameOption),
        };
}