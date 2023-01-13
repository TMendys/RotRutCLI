using System.Text.Json.Serialization;
using static System.Environment;

namespace rotrut.Models;

public record struct Payment(
    [property: JsonPropertyName("fakturanummer")] string InvoiceNumber,
    [property: JsonPropertyName("godkantBelopp")] int ApprovedAmount)
{
    public override string ToString() =>
        $"{InvoiceNumber},{ApprovedAmount}{NewLine}";
}