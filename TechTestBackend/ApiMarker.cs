using Newtonsoft.Json;
using TechTestBackend.Interface;
using TechTestBackend.Models;

namespace TechTestBackend
{
    public class ApiMarker(HttpClient httpClient) : IApiMarker
    {
        public async Task<SpotifySong> GetTrack(string id, CancellationToken cancellationToken)
        {
            SpotifySong track = await httpClient.GetFromJsonAsync<SpotifySong>($"v1/tracks/{id}", cancellationToken)
                ?? throw new HttpRequestException("Couldn't get track from Spotify");

            return track;
        }

        public async Task<List<SpotifySong>> GetTracks(string artist, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"v1/search?q={artist}&type=track", cancellationToken);

            var result = JsonConvert.DeserializeObject<SpotifyTracksResponseModel>(await response.Content.ReadAsStringAsync(cancellationToken))
                ?? throw new HttpRequestException("Couldn't get tracks from Spotify"); ;

            return result.Tracks.Items;
        }
    }

    
}
