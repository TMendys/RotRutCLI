using System.Text;
using System.Text.Json;
using RotRut.Models;

namespace RotRut;

public class Parse
{
    private readonly string directory;
    private readonly string path;

    public Parse(DirectoryInfo? outputDirectory)
    {
        if (outputDirectory is null)
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }

        directory = outputDirectory.FullName;
        path = Path.Combine(directory, "Serialization.csv");
    }

    public List<Payment> ParseFile(FileInfo? file)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file),
            message: "Du måste välja en fil.");
        }

        try
        {
            string jsonString = File.ReadAllText(file.FullName);
            using var document = JsonDocument.Parse(jsonString);
            var element = document.RootElement.GetProperty("beslut").EnumerateArray();
            var cases = element.Select(c => c.Deserialize<Case>());
            Console.WriteLine("Dessa beslut är sparade:");
            return ListAllPayments(cases);
        }
        // TODO: Catch JsonException and KeyNotFoundException
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Directory.Delete(directory);
            throw;
        }
    }

    private static List<Payment> ListAllPayments(IEnumerable<Case> cases)
    {
        List<Payment> payments = new();
        foreach (var @case in cases)
        {
            Console.WriteLine($"{@case.Name}");

            payments.AddRange(@case.Payments
                .GroupBy(x => x.InvoiceNumber)
                .Select(x => new Payment
                {
                    InvoiceNumber = x.Key,
                    ApprovedAmount = x.Sum(x => x.ApprovedAmount)
                }));
        }
        return payments;
    }

    public void CreateCsvFile(List<Payment> payments)
    {
        File.Delete(path);
        string csv = payments.Aggregate(
            new StringBuilder(),
            (sb, s) => sb.Append(s),
            sb => sb.ToString());

        File.AppendAllText(path, csv);
    }
}
