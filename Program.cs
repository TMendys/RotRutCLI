using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using RotRut;

// TODO: Add logging and exceptionhandling

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
    })
{ IsRequired = true };

fileOption.AddAlias("--file");
fileOption.AddAlias("-f");

var directoryOption = new Option<DirectoryInfo?>(
    name: "--output",
    description: "Var den nya filen ska sparas",
    isDefault: true,
    parseArgument: result =>
    {
        if (result.Tokens.Count == 0)
        {
            return new DirectoryInfo(Environment.CurrentDirectory);
        }

        var directoryPath = result.Tokens.Single().Value;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return new DirectoryInfo(directoryPath);
    }
);

directoryOption.AddAlias("-o");

var rootCommand = new RootCommand("Läser skatteverkets beslutsfil och spara ner informationen i en cvs fil för att användas av en robot på Visma Eekonomi.");
rootCommand.AddOption(fileOption);
rootCommand.AddOption(directoryOption);

rootCommand.SetHandler((FileInfo? file, DirectoryInfo? directory) =>
{
    Parse parser = new(directory);
    var payments = parser.ParseFile(file);
    parser.CreateCsvFile(payments);
}, fileOption, directoryOption);

var commandLineBuilder = new CommandLineBuilder(rootCommand)
    .UseDefaults();

var parser = commandLineBuilder.Build();

return await parser.InvokeAsync(args);