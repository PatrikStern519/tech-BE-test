using Microsoft.AspNetCore.Mvc;
using TechTestBackend.Database;
using TechTestBackend.Interface;
using TechTestBackend.Models;

namespace TechTestBackend.Controllers;

[ApiController]
[Route("api/spotify")]
public class SpotifyController : ControllerBase
{
    //constructors are obsolete should not be used anymore

    //Never heard anything about constructors being obsolete, they seem pretty necessary :)

    private readonly SongStorageDbContext _dbContext;
    private readonly ILogger<SpotifyController> _logger;
    private IApiMarker _apiMarker;

    public SpotifyController(SongStorageDbContext dbContext, ILogger<SpotifyController> logger, IApiMarker apiMarker)
    {
        _dbContext = dbContext;
        _logger = logger;
        _apiMarker = apiMarker;
    }

    [HttpGet]
    [Route("searchTracks")]

    public async Task<IActionResult> SearchTracks(string artist)
    {
        List<SpotifySong> tracks = new();

        try
        {
            tracks = await _apiMarker.GetTracks(artist);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        return Ok(tracks);
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> Like(string id)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);

        if (!SpotifyId(id))
        {
            return BadRequest();
        }

        try
        {
            var track = await _apiMarker.GetTrack(id); //check if track exists 

            if (track is null)
            {
                return NotFound("The song isn't available in Spotify catalogue");
            }

            //crashes sometimes for some reason
            // we   have to look into this

            //Earlier implementation could of crashed because of multiple reasons.

            //Before the try phrase: 
            // If the track was for any reason returned as null from the SpotifyHelper as a result of deserialization failure or failure in the request the application would throw an NullReferenceException when trying to check for track.id or using track.Name to populate the earlier created instance of SpotifySong.  

            //During the try phrase:
            // If the requested service of in-memory storage failed adding song will fail.
            // Loading dbContext out of the HttpContext is not a good practice. We'll possibly loose connection to the initiated request context given response time of threads. Since SpotifyHelper uses async requests, very badly handled as well without awaits or thread safety the possibility of losing our request scoped instance of dbContext is likely. We'll use the recommended approach instead of DP Injection

            _dbContext.Songs.Add(track); //Only adding in-memory before SaveAsync. Requirement dosn't mention I/O need for ID check up before creation in DB.

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }

        return Ok();
    }

    [HttpPost]
    [Route("removeLike")]
    public async Task<IActionResult> RemoveLike(string id)
    {
        ArgumentNullException.ThrowIfNull(_dbContext);

        if (!SpotifyId(id))
        {
            return BadRequest();
        }

        try
        {
            var track = await _apiMarker.GetTrack(id); //check if track exists 

            if (track is null)
            {
                return NotFound("The song isn't available on Spotify");
            }

            //Here was same issue as before without thread safety handling in async methods which can result in us loosing our requested scoped instance of dbContext during the call, so when trying to save, the dbContext is not there anymore.

            _dbContext.Songs.Remove(track); // this is not working every time
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        return Ok();
    }

    [HttpGet]
    [Route("listLiked")]
    public async Task<IActionResult> ListLiked()
    {
        ArgumentNullException.ThrowIfNull(_dbContext);

        var savedTracks = _dbContext.Songs.ToList();
        List<SpotifySong> response = new();

        foreach (var savedTrack in savedTracks)
        {
            try
            {
                var track = await _apiMarker.GetTrack(savedTrack.Id);
                if (track is null)
                {
                    _dbContext.Remove(savedTrack);
                }
                else if (!response.Any(s => s.Id == track.Id || s.Name == track.Name))
                {
                    response.Add(track);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        await _dbContext.SaveChangesAsync();

        return Ok(response);
    }

    private static bool SpotifyId(string id)
    {
        return id.Length == 22;
    }
}