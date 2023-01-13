using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using rotrut;

var fileOption = new Option<FileInfo?>(
    name: "--fil",
    description: "Filen som ska läsas.",
    isDefault: true,
    parseArgument: result =>
    {
        if (result.Tokens.Count == 0)
        {
            result.ErrorMessage = "Du måste välja en fil.";
            return null;
        }

        var filePath = result.Tokens.Single().Value;
        if (!File.Exists(filePath))
        {
            result.ErrorMessage = "Filen finns inte.";
            return null;
        }

        return new FileInfo(filePath);
    });

fileOption.AddAlias("--file");
fileOption.AddAlias("-f");

var rootCommand = new RootCommand("Läser skatteverkets beslutsfil och spara ner informationen i en cvs fil för att användas av en robot på Visma Eekonomi.");
rootCommand.AddGlobalOption(fileOption);

var createCommand = new Command("skapa", "Skapa ny csv-fil.");
createCommand.AddAlias("create");

rootCommand.Add(createCommand);

rootCommand.SetHandler((FileInfo? file) =>
{
    Create.ParseFile(file);
}, fileOption);

createCommand.SetHandler((FileInfo? file) =>
{
    Create.ParseFile(file);
}, fileOption);

var commandLineBuilder = new CommandLineBuilder(rootCommand)
    .UseDefaults();

var parser = commandLineBuilder.Build();

return await parser.InvokeAsync(args);