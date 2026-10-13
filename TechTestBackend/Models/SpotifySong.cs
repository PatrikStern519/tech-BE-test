
using Newtonsoft.Json;

namespace TechTestBackend.Models;

public class SpotifySong
{
    [JsonProperty("id")]
    public required string Id { get; set; }
    [JsonProperty("name")]
    public required string Name { get; set; }
}