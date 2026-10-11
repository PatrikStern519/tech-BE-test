using TechTestBackend.Models;

namespace TechTestBackend.Interface;

public interface IApiMarker
{
    public Task<List<SpotifySong>> GetTracks(string artist);

    public Task<SpotifySong> GetTrack(string id);
}