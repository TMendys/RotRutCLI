using System.Text;
using System.Text.Json;
using rotrut.Models;

namespace rotrut;
public static class Create
{
    static readonly string directory =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    static readonly string path = Path.Combine(directory, ".nuget", "packages", "bookrotandrut", "1.0.1", "content", "Serialization.csv");

    public static void ParseFile(FileInfo file)
    {
        string jsonString = File.ReadAllText(file.FullName);
        using var document = JsonDocument.Parse(jsonString);

        var element = document.RootElement.GetProperty("beslut").EnumerateArray();

        var cases = element.Select(c => c.Deserialize<Case>());

        File.Delete(path);
        Console.WriteLine("Dessa beslut är sparade:");
        foreach (var @case in cases)
        {
            var merged = MergeDoubleInvoiceNumbers(@case.Payments);
            CreateCsvFile(merged);
            Console.WriteLine($"{@case.Name}");
        }
    }

    public static IEnumerable<Payment> MergeDoubleInvoiceNumbers(IEnumerable<Payment> payments)
    {
        var withoutDuplicates = payments.GroupBy(p => p.InvoiceNumber)
            .Where(g => g.Count() > 1)
            .Select(x => new Payment
            {
                InvoiceNumber = x.Key,
                ApprovedAmount = x.Sum(p => p.ApprovedAmount)
            }).UnionBy(payments, x => x.InvoiceNumber);

        return withoutDuplicates;
    }

    static void CreateCsvFile(IEnumerable<Payment> payments)
    {
        string csv = payments.Aggregate(
            new StringBuilder(),
            (sb, s) => sb.Append(s),
            sb => sb.ToString());

        File.AppendAllText(path, csv);
    }
}
