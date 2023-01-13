using System.Text;
using System.Text.Json;
using rotrut.Models;

namespace rotrut;
public static class Create
{
    static readonly string directory =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    static readonly string path = Path.Combine(directory, ".nuget", "packages", "bookrotandrut", "1.0.1", "content", "Serialization.csv");

    public static void ParseFile(FileInfo? file)
    {
        if (file is null) throw new ArgumentNullException(nameof(file),
            message: "Du måste välja en fil.");

        string jsonString = File.ReadAllText(file.FullName);
        using var document = JsonDocument.Parse(jsonString);

        var element = document.RootElement.GetProperty("beslut").EnumerateArray();

        var cases = element.Select(c => c.Deserialize<Case>());

        File.Delete(path);
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

    public static IEnumerable<Payment> MergeDoubleInvoiceNumbers(this IEnumerable<Payment> payments) =>
        payments.GroupBy(p => p.InvoiceNumber)
            .Select(x => new Payment
            {
                InvoiceNumber = x.Key,
                ApprovedAmount = x.Sum(p => p.ApprovedAmount)
            });

    static bool ContainsDoubleInvoiceNumbers(this IEnumerable<Payment> payments)
    {
        HashSet<Payment> knownKeys = new();
        return payments.Any(item => !knownKeys.Add(item));
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