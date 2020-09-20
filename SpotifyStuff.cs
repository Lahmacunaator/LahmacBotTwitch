using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LahmacBot_Twitch
{
    public class SpotifyStuff
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ConsoleLogger logger = new ConsoleLogger();
        private static JsonSerializer json = new JsonSerializer();
        private static AccessToken sr = new AccessToken();
        private static RefreshToken rf = new RefreshToken();
        private static Root root = new Root();




        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static async Task<Root> DisplaySong()
        {
            if (sr.access_token == null)
            {
                await GetToken().ConfigureAwait(false);

            }
            await GetSong().ConfigureAwait(false);

            return root;
        }

        public static async Task GetToken()
        {
            if (File.Exists("AccessToken.txt") == false)
            {
                var uri = "https://accounts.spotify.com/api/token";

                var encoded = Base64Encode($"{SpotifyInfo.ClientID}:{SpotifyInfo.ClientSecret}");

                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Basic", encoded);


                var values = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", SpotifyInfo.AuthToken},
                    {"redirect_uri", "https://bariscansoy.azureedge.net"} /*,
                { "client_id", SpotifyInfo.ClientID },
                { "client_secret", SpotifyInfo.ClientSecret }*/
                };

                var content = new FormUrlEncodedContent(values);

                var result = await client.PostAsync(uri, content).ConfigureAwait(false);

                var responseString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                sr = JsonConvert.DeserializeObject<AccessToken>(responseString);
                rf = JsonConvert.DeserializeObject<RefreshToken>(responseString);

                logger.Log(sr.access_token);
                File.WriteAllText("AccessToken.txt",responseString);
                logger.Log(responseString);

                logger.Log("-------------------------------------------------");
            }
            else
            {
                sr = JsonConvert.DeserializeObject<AccessToken>(File.ReadAllText("AccessToken.txt"));
            }
        }

        public static async Task GetSong()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sr.access_token);

            var result = await httpClient.GetAsync("https://api.spotify.com/v1/me/player/currently-playing").ConfigureAwait(false);

            var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            root = JsonConvert.DeserializeObject<Root>(response);

            logger.Log(root.item.name);
        }

        public static async Task RefreshToken()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Base64Encode($"{SpotifyInfo.ClientID}:{SpotifyInfo.ClientSecret}"));

            var uri = "https://accounts.spotify.com/api/token";

            var values = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", SpotifyInfo.AuthToken}
            };

            rf = JsonConvert.DeserializeObject<RefreshToken>(File.ReadAllText("AccessToken.txt"));

            var content = new FormUrlEncodedContent(values);

            var result = await client.PostAsync(uri, content).ConfigureAwait(false);

            var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            sr = JsonConvert.DeserializeObject<AccessToken>(response);
        }

    }





    public class AccessToken
    {
        public string access_token { get; set; }
    }

    public class RefreshToken
    {
        public string refresh_token { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ExternalUrls
    {
        public string spotify { get; set; }
    }

    public class Artist
    {
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalUrls2
    {
        public string spotify { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Album
    {
        public string album_type { get; set; }
        public List<Artist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls2 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalUrls3
    {
        public string spotify { get; set; }
    }

    public class Artist2
    {
        public ExternalUrls3 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalIds
    {
        public string isrc { get; set; }
    }

    public class ExternalUrls4
    {
        public string spotify { get; set; }
    }

    public class Item
    {
        public Album album { get; set; }
        public List<Artist2> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool isExplicit { get; set; }
        public ExternalIds external_ids { get; set; }
        public ExternalUrls4 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Disallows
    {
        public bool resuming { get; set; }
    }

    public class Actions
    {
        public Disallows disallows { get; set; }
    }

    public class Root
    {
        public long timestamp { get; set; }
        public object context { get; set; }
        public int progress_ms { get; set; }
        public Item item { get; set; }
        public string currently_playing_type { get; set; }
        public Actions actions { get; set; }
        public bool is_playing { get; set; }
    }



}