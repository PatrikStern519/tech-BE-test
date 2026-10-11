using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace TechTestBackend
{
    public class SpotifyAuthHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private const string CacheKey = "SpotifyAccessToken";

        public SpotifyAuthHandler(IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string accessToken = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                var tokenResponse = await FetchNewTokenAsync();

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    throw new InvalidOperationException("Couldn't get token for authorization from Spotify API.");
                }

                //Token valid for 3600 seconds/1 hour according to Spotify Docs, so only refreshing after 60min - 1 minute for margin, otherwise get from cache
                int expiresIn = tokenResponse.expires_in > 60 ? tokenResponse.expires_in - 60 : tokenResponse.expires_in;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresIn);

                return tokenResponse.access_token;
            }) ?? throw new InvalidOperationException("Token is missing in the cache.");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<SpotifyTokenResponse> FetchNewTokenAsync()
        {
            // Separate client for auth request
            var authClient = _httpClientFactory.CreateClient();

            string clientId = _configuration["Spotify:ClientId"]!;
            string clientSecret = _configuration["Spotify:ClientSecret"]!;
            var bytes = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
            var base64 = Convert.ToBase64String(bytes);

            var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            authRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
            authRequest.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

            var response = await authClient.SendAsync(authRequest);
            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SpotifyTokenResponse>(contentString)!;
        }
    }

    public class SpotifyTokenResponse
    {
        public required string access_token { get; set; }
        public int expires_in { get; set; }
    }
}

