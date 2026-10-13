using TechTestBackend.Models;

namespace TechTestBackend.Interface;

public interface IApiMarker
{
    public Task<List<SpotifySong>> GetTracks(string artist, CancellationToken cancellationToken);

    public Task<SpotifySong> GetTrack(string id, CancellationToken cancellationToken);
}