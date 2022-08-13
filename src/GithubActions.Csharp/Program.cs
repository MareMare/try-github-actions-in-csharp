// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.Binding;
using System.Text;
using Octokit;
using Octokit.Internal;

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
    async person => await DoRootCommandAsync(person),
    new PersonBinder(firstNameOption, lastNameOption));

await rootCommand.InvokeAsync(args);

static async Task DoRootCommandAsync(Person aPerson)
{
    await UpdateReadmeAsync().ConfigureAwait(false);
    Console.WriteLine($"::set-output name=summary-details::{aPerson.FirstName} {aPerson.LastName}");
}

static async Task UpdateReadmeAsync()
{
    var token = Environment.GetEnvironmentVariable("GH_TOKEN");
    var tokenAuth = new Credentials(token);
    var github = new GitHubClient(
        new ProductHeaderValue("my-cool-app"),
        new InMemoryCredentialStore(tokenAuth));

    var owner = "MareMare";
    var repoName = "try-github-actions-in-csharp";
    //var branchName = "main";

    //var repository = await client.Repository.Get(owner, repoName).ConfigureAwait(false);
    //var branch = await client.Repository.Branch.Get(owner, repoName, branchName).ConfigureAwait(false);
    var contents = await github.Repository.Content.GetAllContents(owner, repoName).ConfigureAwait(false);
    var readmeContent = contents?.FirstOrDefault(
        content => content.Type == ContentType.File &&
                   string.Equals(content.Name, "README.md", StringComparison.OrdinalIgnoreCase));
    var readmeRawBuffer = await github.Repository.Content.GetRawContent(owner, repoName, readmeContent?.Path).ConfigureAwait(false);

    //var readmeRaw = Convert.ToBase64String(readmeRawBuffer ?? Array.Empty<byte>());
    using (var stream = new MemoryStream(readmeRawBuffer))
    using (var reader = new StreamReader(stream))
    {
        var readmeContentText = await reader.ReadToEndAsync().ConfigureAwait(false);

        var dtLocal = DateTimeOffset.Now.ToLocalTime();
        var updatedContentText = new StringBuilder(readmeContentText)
            .AppendLine()
            .AppendLine($"* updated! {dtLocal:yyyy/MM/dd HH:mm:ss}(UTC)")
            .ToString();

        // Update.
        var updateFileRequest = new UpdateFileRequest(
            message: $"[skip ci] Updated by C#! at {dtLocal:yyyy/MM/dd HH:mm:ss}",
            content: updatedContentText,
            sha: readmeContent?.Sha);
        var aa = await github.Repository.Content.UpdateFile(owner, repoName, readmeContent?.Path, updateFileRequest)
            .ConfigureAwait(false);
    }
    
    // Search.
    var commitRequest = new CommitRequest { Path = readmeContent?.Path, };
    var apiOptions = new ApiOptions
    {
        StartPage = 1,
        PageSize = 1000,
        PageCount = 1,
    };
    var commits = await github.Repository.Commit.GetAll(owner, repoName, commitRequest, apiOptions).ConfigureAwait(false);
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