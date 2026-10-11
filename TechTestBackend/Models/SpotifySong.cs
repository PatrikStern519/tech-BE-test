
using System.Text.Json.Serialization;

namespace TechTestBackend.Models;

public class SpotifySong
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}