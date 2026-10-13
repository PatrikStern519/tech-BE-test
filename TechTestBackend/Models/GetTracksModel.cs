using Newtonsoft.Json;

namespace TechTestBackend.Models
{
    public class SpotifyTracksResponseModel
    {
        [JsonProperty("tracks")]
        public required SpotifyTracksModel Tracks { get; set; }
    }

    public class SpotifyTracksModel
    {
        [JsonProperty("items")]
        public required List<SpotifySong> Items { get; set; }
    }
}
