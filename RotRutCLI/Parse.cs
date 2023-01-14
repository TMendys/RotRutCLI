using System.Text;
using System.Text.Json;
using RotRut.Models;

namespace RotRut;

public class Parse
{

    private readonly string directory;

    public Parse(DirectoryInfo? outputDirectory)
    {
        if (outputDirectory is null)
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }

        directory = outputDirectory.FullName;
    }

    private string Path { get => System.IO.Path.Combine(directory, "Serialization.csv"); }

    public void ParseFile(FileInfo? file)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file),
            message: "Du måste välja en fil.");
        }

        string jsonString = File.ReadAllText(file.FullName);
        using var document = JsonDocument.Parse(jsonString);

        var element = document.RootElement.GetProperty("beslut").EnumerateArray();

        var cases = element.Select(c => c.Deserialize<Case>());

        File.Delete(Path);
        Console.WriteLine("Dessa beslut är sparade:");
        foreach (var @case in cases)
        {
            var payments = @case.Payments.ToList();
            if (payments.ContainsDoubleInvoiceNumbers())
            {
                payments.MergeDoubleInvoiceNumbers();
            }
            CreateCsvFile(payments);
            Console.WriteLine($"{@case.Name}");
        }
    }

    private void CreateCsvFile(IEnumerable<Payment> payments)
    {
        string csv = payments.Aggregate(
            new StringBuilder(),
            (sb, s) => sb.Append(s),
            sb => sb.ToString());

        File.AppendAllText(Path, csv);
    }
}
