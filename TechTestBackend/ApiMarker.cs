using Newtonsoft.Json;
using TechTestBackend.Interface;
using TechTestBackend.Models;

namespace TechTestBackend
{
    public class ApiMarker : IApiMarker
    {
        private readonly HttpClient _httpClient;
 
        public ApiMarker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SpotifySong> GetTrack(string id)
        {
            SpotifySong track = await _httpClient.GetFromJsonAsync<SpotifySong>($"v1/tracks/{id}")
                ?? throw new HttpRequestException("Couldn't get track from Spotify");

            return track;
        }

        public async Task<List<SpotifySong>> GetTracks(string artist)
        {
            var response = await _httpClient.GetAsync($"v1/search?q={artist}&type=track");

            var result = JsonConvert.DeserializeObject<SpotifyTracksResponseModel>(await response.Content.ReadAsStringAsync())
                ?? throw new HttpRequestException("Couldn't get tracks from Spotify"); ;

            return result.TracksResponseModel.Tracks;
        }
    }

    
}
