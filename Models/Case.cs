using System.Text.Json.Serialization;

namespace rotrut.Models;

// public class Case
// {
//     [JsonPropertyName("namn")]
//     public string Name { get; set; }

//     [JsonPropertyName("arenden")]
//     public List<Payment> Payments { get; set; }
// }

public record struct Case(
    [property: JsonPropertyName("namn")] string Name,
    [property: JsonPropertyName("arenden")] IEnumerable<Payment> Payments);