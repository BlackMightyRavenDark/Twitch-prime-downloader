using System;
using System.IO;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public class TwitchApi
    {
        public static string TWITCH_CLIENT_ID = "gs7pui3law5lsi69yzi9qzyaqvlcsy";

        /// <summary>
        /// Used for hidden / GQL API requests.
        /// </summary>
        public static string TWITCH_CLIENT_ID_PRIVATE = "kimne78kx3ncx6brgo4mv6wki5h1ko";
        public static string TWITCH_CLIENT_SECRET = "srr2yi260t15ir6w0wq5blir22i9pq";
        public static string TWITCH_ACCEPT_V5_STRING = "application/vnd.twitchtv.v5+json";
        public static string TWITCH_HELIX_OAUTH_TOKEN = "https://id.twitch.tv/oauth2/token?client_id={0}&" +
                      "client_secret={1}&grant_type=client_credentials";
        public static string TWITCH_HELIX_USERS_LOGIN_URL_TEMPLATE = "https://api.twitch.tv/helix/users?login={0}";
        public static string TWITCH_USERS_URL = "https://api.twitch.tv/kraken/users?login={0}";
        public static string TWITCH_CHANNEL_VIDEOS_URL = "https://api.twitch.tv/kraken/channels/{0}/videos";
        public static string TWITCH_VIDEO_INFO_URL = "https://api.twitch.tv/kraken/videos/{0}";
        public static string TWITCH_GAME_QUERY_URL = "https://api.twitch.tv/kraken/search/games?query={0}";
        public static string TWITCH_ACCESS_TOKEN_VOD_URL = "https://api.twitch.tv/api/vods/{0}/access_token?client_id={1}";
        public static string TWITCH_USHER_VOD_URL = "https://usher.ttvnw.net/vod/{0}.m3u8?" +
                     "player=twitchweb&nauth={1}&nauthsig={2}&" +
                     "$allow_audio_only=true&allow_source=true&type=any&p={3}";

        /// <summary>
        /// WARNING!!! Do not use the TWITCH GQL API if you are signed in!!!
        /// </summary>
        public const string TWITCH_GQL_API_URL = "https://gql.twitch.tv/gql";

        public static string UNKNOWN_GAME_URL = "https://static-cdn.jtvnw.net/ttv-boxart/404_boxart.png";

        public static string TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE = "<server>/<stream_id>/chunked/index-dvr.m3u8";
        public static string TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE = "<server>/<stream_id>/chunked/highlight-<video_id>.m3u8";


        public static int GetHelixOauthToken(out string resToken)
        {
            int res = 200;
            if (twitchHelixOauthToken.expireDate <= DateTime.Now ||
                string.IsNullOrEmpty(twitchHelixOauthToken.accessToken))
            {
                string req = string.Format(TWITCH_HELIX_OAUTH_TOKEN, TWITCH_CLIENT_ID, TWITCH_CLIENT_SECRET);
                res = HttpsPost(req, out string buf);
                if (res == 200)
                {
                    JObject j = JObject.Parse(buf);
                    twitchHelixOauthToken.accessToken = j.Value<string>("access_token");
                    twitchHelixOauthToken.expireDate =
                        DateTime.Now.Add(TimeSpan.FromSeconds(j.Value<long>("expires_in")));
                }
            }
            resToken = twitchHelixOauthToken.accessToken;
            return res;
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

        public static string GetChannelVideosUrl_Kraken(string channelId, int maxVideos, int offset)
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

        public static int GetTwitchUserInfo_Helix(string channelName,
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
                    userInfo.displayName = j.Value<string>("display_name");
                    userInfo.id = j.Value<string>("id");
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

        public static int GetTwitchGameInfo(string gameName, out string resJson)
        {
            if (!string.IsNullOrEmpty(gameName) && !string.IsNullOrWhiteSpace(gameName))
            {
                int n = HttpsGet_Kraken(string.Format(TWITCH_GAME_QUERY_URL, gameName), out resJson);
                return n;
            }
            else
            {
                resJson = string.Empty;
                return 404;
            }
        }

        public static int GetTwitchVodInfo_Kraken(string videoId, out string resJson)
        {
            string url = string.Format(TWITCH_VIDEO_INFO_URL, videoId);
            return HttpsGet_Kraken(url, out resJson);
        }

        public static int GetStreamPlaylistUrl(TwitchStreamInfo aStream, out string playlistUrl)
        {
            if (!string.IsNullOrEmpty(aStream.imageAnimatedPreviewUrl) && !string.IsNullOrWhiteSpace(aStream.imageAnimatedPreviewUrl))
            {
                int n = aStream.imageAnimatedPreviewUrl.IndexOf(".net/");
                if (n > 0)
                {
                    string server = aStream.imageAnimatedPreviewUrl.Substring(0, n + 4);
                    if (aStream.vodType == "highlight")
                    {
                        playlistUrl = TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE.Replace("<server>", server)
                            .Replace("<stream_id>", aStream.streamId).Replace("<video_id>", aStream.videoId);
                    }
                    else
                    {
                        playlistUrl = TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE.Replace("<server>", server)
                            .Replace("<stream_id>", aStream.streamId);
                    }
                    int res = MultiThreadedDownloader.GetUrlContentLength(playlistUrl, out _);
                    return res;
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
            string body = GenerateChannelTokenRequestBody(channelName);
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
    }

    public class TwitchGameInfo
    {
        public string title;
        public string ImagePreviewSmallURL;
        public string ImagePreviewMediumURL;
        public string ImagePreviewLargeURL;
        public Stream imageData = new MemoryStream();

        ~TwitchGameInfo()
        {
            imageData?.Dispose();
        }
    }

    public class TwitchUserInfo
    {
        public string displayName;
        public string id;
    }

    public class TwitchStreamInfo
    {
        public string title;
        public string videoId;
        public string streamId;
        public string videoUrl;
        public DateTime length;
        public int viewCount;
        public string vodType;
        public TwitchVodMutedChunks mutedChunks = new TwitchVodMutedChunks();
        public DateTime dateCreation;
        public DateTime dateDeletion;
        public string imagePreviewTemplateUrl;
        public string imageAnimatedPreviewUrl;
        public Stream imageData = new MemoryStream();
        public bool isPrime;
        public TwitchUserInfo userInfo = new TwitchUserInfo();
        public TwitchGameInfo gameInfo = new TwitchGameInfo();
        public string infoStringJson;

        public bool IsHighlight()
        {
            return vodType == "highlight";
        }

        ~TwitchStreamInfo()
        {
            imageData?.Dispose();
        }
    }

    public class TwitchHelixOauthToken
    {
        public string accessToken = string.Empty;
        public string tokenType = string.Empty;
        public DateTime expireDate = DateTime.MinValue;

        public void Reset()
        {
            accessToken = string.Empty;
            tokenType = string.Empty;
            expireDate = DateTime.MinValue;
        }
    }
}
