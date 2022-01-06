using System;
using System.IO;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public sealed class TwitchApi
    {
        public static string TWITCH_CLIENT_ID = "gs7pui3law5lsi69yzi9qzyaqvlcsy";

        /// <summary>
        /// Used for hidden / GQL API requests.
        /// </summary>
        public const string TWITCH_CLIENT_ID_PRIVATE = "kimne78kx3ncx6brgo4mv6wki5h1ko";

        public const string TWITCH_ACCEPT_V5_STRING = "application/vnd.twitchtv.v5+json";
        public const string TWITCH_HELIX_USERS_LOGIN_URL_TEMPLATE = "https://api.twitch.tv/helix/users?login={0}";
        public const string TWITCH_KRAKEN_VIDEO_INFO_URL_TEMPLATE = "https://api.twitch.tv/kraken/videos/{0}";
        public const string TWITCH_KRAKEN_GAME_QUERY_URL_TEMPLATE = "https://api.twitch.tv/kraken/search/games?query={0}";
        /*public const string TWITCH_USERS_URL_TEMPLATE = "https://api.twitch.tv/kraken/users?login={0}";
        public const string TWITCH_CHANNEL_VIDEOS_URL_TEMPLATE = "https://api.twitch.tv/kraken/channels/{0}/videos";
        public const string TWITCH_ACCESS_TOKEN_VOD_URL_TEMPLATE = "https://api.twitch.tv/api/vods/{0}/access_token?client_id={1}";
        public const string TWITCH_USHER_VOD_URL_TEMPLATE = "https://usher.ttvnw.net/vod/{0}.m3u8?" +
                     "player=twitchweb&nauth={1}&nauthsig={2}&" +
                     "$allow_audio_only=true&allow_source=true&type=any&p={3}";*/

        /// <summary>
        /// WARNING!!! Do not use the TWITCH GQL API if you are signed in!!!
        /// </summary>
        public const string TWITCH_GQL_API_URL = "https://gql.twitch.tv/gql";

        public const string UNKNOWN_GAME_URL = "https://static-cdn.jtvnw.net/ttv-boxart/404_boxart.png";

        public const string TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE = "<server>/<stream_id>/chunked/index-dvr.m3u8";
        public const string TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE = "<server>/<stream_id>/chunked/highlight-<video_id>.m3u8";

        public TwitchHelixOauthToken HelixOauthToken { get; private set; } = new TwitchHelixOauthToken();

        public int GetHelixOauthToken(out string resToken)
        {
            int errorCode = HelixOauthToken.Update(TWITCH_CLIENT_ID);
            resToken = HelixOauthToken.AccessToken;
            return errorCode;
        }

        /// <summary>
        /// WARNING!!! Do not use this body if you are signed in!!!
        /// </summary>
        public static string GenerateChannelTokenRequestBody(string userLogin)
        {
            const string hashValue = "0828119ded1c13477966434e15800ff57ddacf13ba1911c129dc2200705b0712";
            JObject jPersistedQuery = new JObject();
            jPersistedQuery["version"] = 1;
            jPersistedQuery["sha256Hash"] = hashValue;

            JObject jExtensions = new JObject();
            jExtensions.Add(new JProperty("persistedQuery", jPersistedQuery));

            JObject jVariables = new JObject();
            jVariables["isLive"] = true;
            jVariables["login"] = userLogin;
            jVariables["isVod"] = false;
            jVariables["vodID"] = "";
            jVariables["playerType"] = "embed";

            JObject json = new JObject();
            json["operationName"] = "PlaybackAccessToken";
            json.Add(new JProperty("extensions", jExtensions));
            json.Add(new JProperty("variables", jVariables));

            return json.ToString();
        }

        public string GetChannelVideosUrl_Kraken(string channelId, int maxVideos, int offset)
        {
            string urlTemplate = "https://api.twitch.tv/kraken/channels/{0}/videos?broadcast_type=all&limit={1}&offset={2}";
            string req = string.Format(urlTemplate, channelId, maxVideos.ToString(), offset.ToString());
            return req;
        }

        public static string GetUserInfoRequestUrl_Helix(string userLogin)
        {
            string req = string.Format(TWITCH_HELIX_USERS_LOGIN_URL_TEMPLATE, userLogin);
            return req;
        }

        public int GetUserInfo_Helix(string channelName,
                             out TwitchUserInfo userInfo, out string errorMessage)
        {
            userInfo = null;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                errorMessage = "Empty channel name";
                return 400;
            }

            string req = GetUserInfoRequestUrl_Helix(channelName.ToLower());
            int res = HttpsGet_Helix(req, out string buf);
            if (res == 200)
            {
                JObject json = JObject.Parse(buf);
                JArray ja = json.Value<JArray>("data");
                if (ja != null && ja.Count > 0)
                {
                    JObject j = ja[0].Value<JObject>();
                    userInfo = new TwitchUserInfo();
                    userInfo.DisplayName = j.Value<string>("display_name");
                    userInfo.Id = j.Value<string>("id");
                    errorMessage = null;
                }
                else
                {
                    errorMessage = buf;
                    return 404;
                }
            }
            else
            {
                errorMessage = buf;
            }
            return res;
        }

        public int GetGameInfo_Kraken(string gameName, out string resJson)
        {
            if (!string.IsNullOrEmpty(gameName) && !string.IsNullOrWhiteSpace(gameName))
            {
                string req = string.Format(TWITCH_KRAKEN_GAME_QUERY_URL_TEMPLATE, Uri.EscapeDataString(gameName.ToLower()));
                return HttpsGet_Kraken(req, out resJson);
            }
            resJson = null;
            return 400;
        }

        public int GetVodInfo_Kraken(string videoId, out string resJson)
        {
            string url = string.Format(TWITCH_KRAKEN_VIDEO_INFO_URL_TEMPLATE, videoId);
            return HttpsGet_Kraken(url, out resJson);
        }

        public static int GetVodPlaylistUrl(TwitchVod vod, out string playlistUrl)
        {
            if (!string.IsNullOrEmpty(vod.ImageAnimatedPreviewUrl) && !string.IsNullOrWhiteSpace(vod.ImageAnimatedPreviewUrl))
            {
                int n = vod.ImageAnimatedPreviewUrl.IndexOf(".net/");
                if (n > 0)
                {
                    string server = vod.ImageAnimatedPreviewUrl.Substring(0, n + 4);
                    if (vod.Type == "highlight")
                    {
                        playlistUrl = TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE.Replace("<server>", server)
                            .Replace("<stream_id>", vod.StreamId).Replace("<video_id>", vod.VideoId);
                    }
                    else
                    {
                        playlistUrl = TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE.Replace("<server>", server)
                            .Replace("<stream_id>", vod.StreamId);
                    }
                    int errorCode = MultiThreadedDownloader.GetUrlContentLength(playlistUrl, out _);
                    return errorCode;
                }
            }

            playlistUrl = null;
            return 400;
        }

        /// <summary>
        /// Checks channel is prime or not.
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        /// <param name="channelName">Channel name or user login</param>
        /// <returns>HTTP error code</returns>
        public static int IsChannelPrime(string channelName, out bool result)
        {
            result = false;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                return 400;
            }
            string body = GenerateChannelTokenRequestBody(channelName.ToLower());
            int errorCode = HttpsPost(TWITCH_GQL_API_URL, body, out string token);
            if (errorCode == 200)
            {
                JObject jsonToken = JObject.Parse(token);
                string t = jsonToken.Value<JObject>("data").Value<JObject>("streamPlaybackAccessToken").Value<string>("value");
                JObject j = JObject.Parse(t);
                JToken jt = j.Value<JObject>("chansub").Value<JToken>("restricted_bitrates");
                result = jt != null && (jt as JArray).Count > 0;
            }
            return errorCode;
        }

        public int HttpsGet_Helix(string url, out string recvText)
        {
            int errorCode = GetHelixOauthToken(out string token);
            if (errorCode == 200)
            {
                FileDownloader d = new FileDownloader();
                d.Url = url;
                d.Headers.Add("Client-ID", TWITCH_CLIENT_ID);
                d.Headers.Add("Authorization", "Bearer " + token);
                errorCode = d.DownloadString(out recvText);
                return errorCode;
            }
            recvText = null;
            return errorCode;
        }

        public int HttpsGet_Kraken(string url, out string recvText)
        {
            FileDownloader d = new FileDownloader();
            d.Url = url;
            d.Headers.Add("Client-ID", TWITCH_CLIENT_ID);
            d.Accept = TWITCH_ACCEPT_V5_STRING;
            int errorCode = d.DownloadString(out recvText);
            return errorCode;
        }
    }

    public sealed class TwitchGameInfo
    {
        public string Title { get; set; }
        public string ImagePreviewSmallUrl { get; set; }
        public string ImagePreviewMediumUrl { get; set; }
        public string ImagePreviewLargeUrl { get; set; }
        public Stream ImageData { get; set; } = new MemoryStream();

        ~TwitchGameInfo()
        {
            ImageData?.Dispose();
        }
    }

    public sealed class TwitchUserInfo
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
    }

    public sealed class TwitchVod
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public string StreamId { get; set; }
        public string VideoUrl { get; set; }
        public TimeSpan Length { get; set; }
        public int ViewCount { get; set; }
        public string Type { get; set; }
        public TwitchVodMutedChunks MutedChunks { get; private set; } = new TwitchVodMutedChunks();
        public DateTime DateCreation { get; set; }
        public DateTime DateDeletion { get; set; }
        public string ImagePreviewTemplateUrl { get; set; }
        public string ImageAnimatedPreviewUrl { get; set; }
        public Stream ImageData { get; set; } = new MemoryStream();
        public bool IsPrime { get; set; }
        public TwitchUserInfo UserInfo { get; private set; } = new TwitchUserInfo();
        public TwitchGameInfo GameInfo { get; private set; } = new TwitchGameInfo();
        public string InfoStringJson { get; set; }

        public bool IsHighlight()
        {
            return Type == "highlight";
        }

        ~TwitchVod()
        {
            ImageData?.Dispose();
        }
    }

    public sealed class TwitchHelixOauthToken
    {
        public const string TWITCH_HELIX_OAUTH_TOKEN_URL_TEMPLATE = "https://id.twitch.tv/oauth2/token?client_id={0}&" +
                      "client_secret={1}&grant_type=client_credentials";
        public const string TWITCH_CLIENT_SECRET = "srr2yi260t15ir6w0wq5blir22i9pq";

        public string AccessToken { get; private set; }
        public string TokenType { get; private set; }
        public DateTime ExpireDate { get; private set; } = DateTime.MinValue;

        public void Reset()
        {
            AccessToken = null;
            TokenType = null;
            ExpireDate = DateTime.MinValue;
        }

        public int Update(string clientId)
        {
            int errorCode = 200;
            if (ExpireDate <= DateTime.Now || string.IsNullOrEmpty(AccessToken))
            {
                string req = string.Format(TWITCH_HELIX_OAUTH_TOKEN_URL_TEMPLATE, clientId, TWITCH_CLIENT_SECRET);
                errorCode = HttpsPost(req, out string buf);
                if (errorCode == 200)
                {
                    JObject j = JObject.Parse(buf);
                    if (j == null)
                    {
                        return 400;
                    }
                    AccessToken = j.Value<string>("access_token");
                    long expiresIn = j.Value<long>("expires_in");
                    ExpireDate = DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn));
                }
            }
            return errorCode;
        }
    }
}
