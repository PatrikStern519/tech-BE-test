using System.Text.Json.Serialization;

namespace TechTestBackend.Models
{
    public class SpotifyTracksResponseModel
    {
        public required SpotifyTracksModel TracksResponseModel { get; set; }
    }
    public class SpotifyTracksModel
    {
        [JsonPropertyName("tracks")]
        public required List<SpotifySong> Tracks { get; set; }
    }
}
