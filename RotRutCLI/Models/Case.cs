using System.Text.Json.Serialization;

namespace RotRut.Models;

public record struct Case(
    [property: JsonPropertyName("namn")] string Name,
    [property: JsonPropertyName("arenden")] IEnumerable<Payment> Payments);