using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
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
        public const string TWITCH_HELIX_VIDEOS_ENDPOINT_URL = "https://api.twitch.tv/helix/videos";
        public const string TWITCH_HELIX_USERS_ENDPOINT_URL = "https://api.twitch.tv/helix/users";

        /// <summary>
        /// WARNING!!! Do not use the TWITCH GQL API if you are signed in!!!
        /// </summary>
        public const string TWITCH_GQL_API_URL = "https://gql.twitch.tv/gql";

        public const string UNKNOWN_GAME_URL = "https://static-cdn.jtvnw.net/ttv-boxart/404_boxart.png";

        public const string TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE = "https://<server_id>.cloudfront.net/<stream_id>/chunked/index-dvr.m3u8";
        public const string TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE = "https://<server_id>.cloudfront.net/<stream_id>/chunked/highlight-<video_id>.m3u8";

        public static readonly string[] TwitchFileServerIds = new string[] { "dgeft87wbj63p", "d2nvs31859zcd8", "d1m7jfoe9zdc1j" };

        private static Dictionary<string, bool> _primeChannels = new Dictionary<string, bool>();

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

        /// <summary>
        /// WARNING!!! Do not use this body if you are signed in!!!
        /// </summary>
        public static JArray GenerateVodPlaybackTokenRequestBody(string vodId)
        {
            const string hashValue = "0828119ded1c13477966434e15800ff57ddacf13ba1911c129dc2200705b0712";
            JObject jPersistedQuery = new JObject();
            jPersistedQuery["version"] = 1;
            jPersistedQuery["sha256Hash"] = hashValue;

            JObject jExtensions = new JObject();
            jExtensions.Add(new JProperty("persistedQuery", jPersistedQuery));

            JObject jVariables = new JObject();
            jVariables["isLive"] = false;
            jVariables["login"] = string.Empty;
            jVariables["isVod"] = true;
            jVariables["vodID"] = vodId;
            jVariables["playerType"] = "embed";

            JObject json = new JObject();
            json["operationName"] = "PlaybackAccessToken";
            json.Add(new JProperty("extensions", jExtensions));
            json.Add(new JProperty("variables", jVariables));

            return new JArray(){ json };
        }

        /// <summary>
        /// WARNING!!! Do not use this body if you are signed in!!!
        /// </summary>
        public static JObject GenerateVodGameInfoRequestBody(string vodId)
        {
            const string hashValue = "38bbbbd9ae2e0150f335e208b05cf09978e542b464a78c2d4952673cd02ea42b";
            JObject jPersistedQuery = new JObject();
            jPersistedQuery["version"] = 1;
            jPersistedQuery["sha256Hash"] = hashValue;

            JObject jExtensions = new JObject();
            jExtensions.Add(new JProperty("persistedQuery", jPersistedQuery));

            JObject jVariables = new JObject();
            jVariables["videoID"] = vodId;
            jVariables["hasVideoID"] = true;

            JObject json = new JObject();
            json["operationName"] = "WatchTrackQuery";
            json.Add(new JProperty("variables", jVariables));
            json.Add(new JProperty("extensions", jExtensions));

            return json;
        }

        /// <summary>
        /// WARNING!!! Do not use this body if you are signed in!!!
        /// </summary>
        public static JArray GenerateVodMutedSegmentsInfoRequestBody(string vodId)
        {
            const string hashValue = "c36e7400657815f4704e6063d265dff766ed8fc1590361c6d71e4368805e0b49";
            JObject jPersistedQuery = new JObject();
            jPersistedQuery["version"] = 1;
            jPersistedQuery["sha256Hash"] = hashValue;

            JObject jExtensions = new JObject();
            jExtensions.Add(new JProperty("persistedQuery", jPersistedQuery));

            JObject jVariables = new JObject();
            jVariables["vodID"] = vodId;
            
            JObject json = new JObject();
            json["operationName"] = "VideoPlayer_MutedSegmentsAlertOverlay";
            json.Add(new JProperty("variables", jVariables));
            json.Add(new JProperty("extensions", jExtensions));


            return new JArray() { json };
        }

        /// <summary>
        /// WARNING!!! Do not use this body if you are signed in!!!
        /// </summary>
        public static JArray GenerateVodInfoRequestBody(string vodId, string channelLogin)
        {
            const string hashValue = "cb3b1eb2f2d2b2f65b8389ba446ec521d76c3aa44f5424a1b1d235fe21eb4806";
            JObject jPersistedQuery = new JObject();
            jPersistedQuery["version"] = 1;
            jPersistedQuery["sha256Hash"] = hashValue;

            JObject jExtensions = new JObject();
            jExtensions.Add(new JProperty("persistedQuery", jPersistedQuery));

            JObject jVariables = new JObject();
            jVariables["channelLogin"] = channelLogin;
            jVariables["videoID"] = vodId;
            
            JObject json = new JObject();
            json["operationName"] = "VideoMetadata";
            json.Add(new JProperty("variables", jVariables));
            json.Add(new JProperty("extensions", jExtensions));

            JArray jArray = new JArray() { json };
            return jArray;
        }

        public static string GenerateVodPlaylistManifestUrl(string vodId, JObject vodPlaybackToken)
        {
            string tokenValue = vodPlaybackToken.Value<string>("value");
            string tokenSignature = vodPlaybackToken.Value<string>("signature");

            string usherUrl = $"https://usher.ttvnw.net/vod/{vodId}.m3u8?";
            int randomNumber = random.Next(999999);

            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("player", "twitchweb");
            query.Add("allow_audio_only", "true");
            query.Add("allow_source", "true");
            query.Add("type", "any");
            query.Add("nauth", tokenValue);
            query.Add("nauthsig", tokenSignature);
            query.Add("p", randomNumber.ToString());

            string url = usherUrl + query.ToString();
            return url;
        }

        public string GetChannelVideosRequestUrl_Helix(string channelId, int videosPerPage, string pageToken)
        {
            string url = $"{TWITCH_HELIX_VIDEOS_ENDPOINT_URL}?user_id={channelId}&first={videosPerPage}";
            if (!string.IsNullOrEmpty(pageToken) && !string.IsNullOrWhiteSpace(pageToken))
            {
                url += $"&{pageToken}";
            }
            return url;
        }

        public static string GetUserInfoRequestUrl_Helix(string userLogin)
        {
            string url = $"{TWITCH_HELIX_USERS_ENDPOINT_URL}?login={userLogin}";
            return url;
        }

        public string GenerateVodInfoRequestUrl_Helix(string vodId)
        {
            string url = $"{TWITCH_HELIX_VIDEOS_ENDPOINT_URL}?id={vodId}";
            return url;
        }

        public int GetUserInfo_Helix(string channelName,
                             TwitchUserInfo userInfo, out string errorMessage)
        {
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
                    userInfo.Login = j.Value<string>("login");
                    userInfo.DisplayName = j.Value<string>("display_name");
                    userInfo.Id = j.Value<string>("id");
                    userInfo.Description = j.Value<string>("description");
                    userInfo.ViewCount = j.Value<int>("view_count");
                    userInfo.DateCreated = TwitchTimeToDateTime(j.Value<string>("created_at"), true);
                    string broadcasterType = j.Value<string>("broadcaster_type");
                    if (broadcasterType == "partner")
                    {
                        userInfo.BroadcasterType = TwitchBroadcasterType.Partner;
                    }
                    else if (broadcasterType == "affiliate")
                    {
                        userInfo.BroadcasterType = TwitchBroadcasterType.Affiliate;
                    }
                    else
                    {
                        userInfo.BroadcasterType = TwitchBroadcasterType.None;
                    }
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

        /// <summary>
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        public TwitchGameInfoResult GetVodGameInfo_GQL(string vodId)
        {
            if (!string.IsNullOrEmpty(vodId) && !string.IsNullOrWhiteSpace(vodId))
            {
                JObject body = GenerateVodGameInfoRequestBody(vodId);

                int errorCode = HttpsPost(TWITCH_GQL_API_URL, body.ToString(), out string response);
                if (errorCode == 200)
                {
                    JObject json = JObject.Parse(response);
                    JObject jGame = json.Value<JObject>("data").Value<JObject>("video").Value<JObject>("game");
                    if (jGame != null)
                    {
                        TwitchGameInfo gameInfo = new TwitchGameInfo();
                        JToken jt = jGame.Value<JToken>("name");
                        if (jt != null)
                        {
                            gameInfo.Title = jt.Value<string>();
                        }
                        return new TwitchGameInfoResult(gameInfo, errorCode);
                    }
                }
            }
            return new TwitchGameInfoResult(null, 400);
        }

        /// <summary>
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        public TwitchGameInfoResult GetVodGameInfo_GQL(string vodId, string channelLogin)
        {
            if (!string.IsNullOrEmpty(vodId) && !string.IsNullOrWhiteSpace(vodId))
            {
                JArray body = GenerateVodInfoRequestBody(vodId, channelLogin);

                int errorCode = HttpsPost(TWITCH_GQL_API_URL, body.ToString(), out string response);
                if (errorCode == 200)
                {
                    JArray jArray = JArray.Parse(response);
                    JObject json = jArray.Value<JObject>(0);
                    JObject jData = json.Value<JObject>("data");
                    if (jData != null)
                    {
                        JObject jVideo = jData.Value<JObject>("video");
                        if (jVideo != null)
                        {
                            JObject jGame = jVideo.Value<JObject>("game");
                            if (jGame != null)
                            {
                                TwitchGameInfo gameInfo = new TwitchGameInfo();
                                gameInfo.Title = jGame.Value<string>("name");
                                JToken jt = jGame.Value<JToken>("boxArtURL");
                                if (jt != null)
                                {
                                    gameInfo.ImagePreviewTemplateUrl = jt.Value<string>();
                                }
                                return new TwitchGameInfoResult(gameInfo, errorCode);
                            }
                        }
                    }
                }
            }

            return new TwitchGameInfoResult(null, 400);
        }

        public int GetVodInfo(string vodId, out string infoString)
        {
            string url = GenerateVodInfoRequestUrl_Helix(vodId);
            int errorCode = HttpsGet_Helix(url, out infoString);
            return errorCode;
        }

        /// <summary>
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        public TwitchVod ParseVodInfo(JObject vodInfo)
        {
            TwitchVod vod = new TwitchVod();
            vod.Title = vodInfo.Value<string>("title");
            vod.VideoId = vodInfo.Value<string>("id");
            vod.VideoUrl = vodInfo.Value<string>("url");
            vod.UserInfo.Id = vodInfo.Value<string>("user_id");
            vod.UserInfo.Login = vodInfo.Value<string>("user_login");
            vod.ImagePreviewTemplateUrl = vodInfo.Value<string>("thumbnail_url");
            vod.StreamId = ExtractStreamIDFromImageURL(vod.ImagePreviewTemplateUrl);
            vod.ViewCount = vodInfo.Value<int>("view_count");
            vod.Type = vodInfo.Value<string>("type");
            vod.DateCreationString = vodInfo.Value<string>("created_at");
            vod.DateCreation = TwitchTimeToDateTime(vod.DateCreationString, config.UseLocalVodDate);
            TimeSpan vodLifeTime = TimeSpan.FromDays(vod.UserInfo.BroadcasterType == TwitchBroadcasterType.Partner ? 60.0 : 14.0);
            vod.DateDeletion = new DateTime(vod.DateCreation.Ticks + vodLifeTime.Ticks);
            vod.InfoStringJson = vodInfo.ToString();
            if (IsChannelPrime(vod.UserInfo.Login, out bool prime) == 200)
            {
                vod.IsPrime = prime;
            }
            if (GetUserInfo_Helix(vod.UserInfo.Login, vod.UserInfo, out _) == 200)
            {
                VideoMetadataResult videoMetadata = GetVodMetadata(vod.VideoId, vod.UserInfo.Login);
                if (videoMetadata.ErrorCode == 200)
                {
                    if (videoMetadata.Data != null && videoMetadata.Data.Count > 0 && videoMetadata.Data[0] != null)
                    {
                        JObject jData = videoMetadata.Data[0].Value<JObject>("data");
                        if (jData != null)
                        {
                            JObject jVideo = jData.Value<JObject>("video");
                            if (jVideo != null)
                            {
                                int seconds = jVideo.Value<int>("lengthSeconds");
                                vod.Length = TimeSpan.FromSeconds(seconds);
                            }
                        }
                    }
                }
            }

            TwitchGameInfoResult gameInfo = GetVodGameInfo_GQL(vod.VideoId, vod.VideoId);
            if (gameInfo.ErrorCode == 200)
            {
                vod.GameInfo.Title = gameInfo.GameInfo.Title;
                vod.GameInfo.ImagePreviewTemplateUrl = gameInfo.GameInfo.ImagePreviewTemplateUrl;
            }
            else
            {
                vod.GameInfo.ImagePreviewTemplateUrl = UNKNOWN_GAME_URL;
            }

            TwitchVodMutedSegmentsResult mutedSegmentsResult = GetVodMutedSegments(vod.VideoId);
            if (mutedSegmentsResult.ErrorCode == 200)
            {
                ParseMutedSegments(mutedSegmentsResult.MutedSegments, vod.MutedSegments);
            }

            return vod;
        }

        /// <summary>
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        public TwitchVodMutedSegmentsResult GetVodMutedSegments(string vodId)
        {
            //Official Helix API does not working properly. So, we need to use the GQL API.
            JArray body = GenerateVodMutedSegmentsInfoRequestBody(vodId);
            int errorCode = HttpsPost(TWITCH_GQL_API_URL, body.ToString(), out string response);
            if (errorCode == 200)
            {
                try
                {
                    JArray jArray = JArray.Parse(response);
                    JObject jVideo = jArray.Value<JObject>(0).Value<JObject>("data").Value<JObject>("video");
                    JObject jMutedSegmentConnection = jVideo.Value<JObject>("muteInfo").Value<JObject>("mutedSegmentConnection");
                    JArray segments = jMutedSegmentConnection != null ? jMutedSegmentConnection.Value<JArray>("nodes") : null;
                    if (segments == null)
                    {
                        errorCode = 404;
                    }
                    return new TwitchVodMutedSegmentsResult(segments, errorCode);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    return new TwitchVodMutedSegmentsResult(null, 404);
                }
            }
            return new TwitchVodMutedSegmentsResult(null, errorCode);
        }

        public static void ParseMutedSegments(JArray jArray, TwitchVodMutedSegments mutedChunks)
        {
            mutedChunks.Clear();
            if (jArray != null && jArray.Count > 0)
            {
                for (int i = 0; i < jArray.Count; i++)
                {
                    int offset = jArray[i].Value<int>("offset");
                    int dur = jArray[i].Value<int>("duration");
                    mutedChunks.Segments.Add(new TwitchVodMutedSegment(offset, dur));
                    mutedChunks.TotalLength = new TimeSpan(mutedChunks.TotalLength.Ticks + TimeSpan.FromSeconds(dur).Ticks);
                    DateTime start = DateTime.MinValue + TimeSpan.FromSeconds(offset);
                    DateTime end = DateTime.MinValue + TimeSpan.FromSeconds(offset + dur);
                    mutedChunks.SegmentList.Add($"{start:HH:mm:ss} - {end:HH:mm:ss}");
                }
            }
        }
        
        /// <summary>
        /// WARNING!!! Do not use this method if you are signed in!!!
        /// </summary>
        public VideoMetadataResult GetVodMetadata(string vodId, string channelLogin)
        {
            JArray body = GenerateVodInfoRequestBody(vodId, channelLogin);
            int errorCode = HttpsPost(TWITCH_GQL_API_URL, body.ToString(), out string response);
            JArray res = errorCode == 200 ? JArray.Parse(response) : null;
            return new VideoMetadataResult(res, errorCode);
        }

        public static int GetVodAccessToken(string vodId, out JObject token)
        {
            token = null;
            try
            {
                JArray body = GenerateVodPlaybackTokenRequestBody(vodId);
                int errorCode = HttpsPost(TWITCH_GQL_API_URL, body.ToString(), out string response);
                if (errorCode == 200)
                {
                    JArray jsonArr = JArray.Parse(response);
                    if (jsonArr != null && jsonArr.Count > 0)
                    {
                        JObject jData = (jsonArr[0] as JObject).Value<JObject>("data");
                        if (jData != null)
                        {
                            token = jData.Value<JObject>("videoPlaybackAccessToken");
                            return token != null ? 200 : 400;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return 400;
        }

        public static int GetVodPlaylistUrl(TwitchVod vod, out string playlistUrl)
        {
            playlistUrl = null;
            int errorCode = GetVodAccessToken(vod.VideoId, out JObject token);
            if (errorCode == 200)
            {
                playlistUrl = GenerateVodPlaylistManifestUrl(vod.VideoId, token);
                errorCode = DownloadString(playlistUrl, out string manifest);
                if (errorCode == 200)
                {
                    TwitchPlaylistManifestParser parser = new TwitchPlaylistManifestParser(manifest);
                    playlistUrl = parser.FindPlaylistUrl("chunked");
                    return !string.IsNullOrEmpty(playlistUrl) && !string.IsNullOrWhiteSpace(playlistUrl) &&
                        MultiThreadedDownloader.GetUrlContentLength(playlistUrl, out _) == 200 ? 200 : 400;
                }
            }
            if (!string.IsNullOrEmpty(vod.ImagePreviewTemplateUrl) && !string.IsNullOrWhiteSpace(vod.ImagePreviewTemplateUrl))
            {
                errorCode = 404;
                for (int i = 0; i < TwitchFileServerIds.Length && errorCode != 200; i++)
                {
                    if (vod.IsHighlight())
                    {
                        playlistUrl = TWITCH_PLAYLIST_HIGHLIGHT_URL_TEMPLATE.Replace("<server_id>", TwitchFileServerIds[i])
                            .Replace("<stream_id>", vod.StreamId).Replace("<video_id>", vod.VideoId);
                    }
                    else
                    {
                        playlistUrl = TWITCH_PLAYLIST_ARCHIVE_URL_TEMPLATE.Replace("<server_id>", TwitchFileServerIds[i])
                            .Replace("<stream_id>", vod.StreamId);
                    }
                    errorCode = MultiThreadedDownloader.GetUrlContentLength(playlistUrl, out _);
                }
                return errorCode;
            }

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

            if (_primeChannels.ContainsKey(channelName))
            {
                result = _primeChannels[channelName];
                return 200;
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
                _primeChannels[channelName] = result;
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
    }


    public sealed class TwitchVodMutedSegment
    {
        public int Offset { get; private set; }
        public int Duration { get; private set; }

        public TwitchVodMutedSegment(int offset, int duration)
        {
            Offset = offset;
            Duration = duration;
        }
    }

    public sealed class TwitchVodMutedSegments
    {
        public List<TwitchVodMutedSegment> Segments { get; private set; } = new List<TwitchVodMutedSegment>();
        public List<string> SegmentList { get; private set; } = new List<string>();
        public TimeSpan TotalLength { get; set; } = new TimeSpan(0L);

        public void Clear()
        {
            Segments.Clear();
            SegmentList.Clear();
            TotalLength = new TimeSpan(0L);
        }
    }

    public sealed class TwitchVodMutedSegmentsResult
    {
        public JArray MutedSegments { get; private set; }
        public int ErrorCode { get; private set; }

        public TwitchVodMutedSegmentsResult(JArray mutedSegments, int errorCode)
        {
            MutedSegments = mutedSegments;
            ErrorCode = errorCode;
        }
    }

    public enum TwitchVodChunkState { NotMuted, Muted, Unmuted };

    public sealed class TwitchVodChunk
    {
        public string FileName { get; private set; }

        public TwitchVodChunk(string fileName)
        {
            FileName = fileName;
        }

        public string GetName()
        {
            return FileName.Substring(0, FileName.IndexOf(
                GetState() == TwitchVodChunkState.NotMuted ? ".ts" : "-"));
        }

        public TwitchVodChunkState GetState()
        {
            if (FileName.EndsWith("-muted.ts"))
            {
                return TwitchVodChunkState.Muted;
            }
            else if (FileName.EndsWith("-unmuted.ts"))
            {
                return TwitchVodChunkState.Unmuted;
            }
            return TwitchVodChunkState.NotMuted;
        }

        public void SetState(TwitchVodChunkState state)
        {
            switch (state)
            {
                case TwitchVodChunkState.Muted:
                    FileName = GetName() + "-muted.ts";
                    break;

                case TwitchVodChunkState.Unmuted:
                    FileName = GetName() + "-unmuted.ts";
                    break;

                case TwitchVodChunkState.NotMuted:
                    FileName = GetName() + ".ts";
                    break;
            }
        }

        public TwitchVodChunkState NextState()
        {
            switch (GetState())
            {
                case TwitchVodChunkState.Muted:
                    SetState(TwitchVodChunkState.NotMuted);
                    return TwitchVodChunkState.NotMuted;

                case TwitchVodChunkState.NotMuted:
                    SetState(TwitchVodChunkState.Unmuted);
                    return TwitchVodChunkState.Unmuted;

                case TwitchVodChunkState.Unmuted:
                    SetState(TwitchVodChunkState.Muted);
                    return TwitchVodChunkState.Muted;
            }
            return TwitchVodChunkState.NotMuted;
        }
    }

    public sealed class TwitchGameInfo
    {
        public string Title { get; set; }
        public string ImagePreviewTemplateUrl { get; set; }
        public Stream ImageData { get; set; } = new MemoryStream();

        ~TwitchGameInfo()
        {
            ImageData?.Dispose();
        }
    }

    public sealed class TwitchGameInfoResult
    {
        public TwitchGameInfo GameInfo { get; private set; }
        public int ErrorCode { get; private set; }

        public TwitchGameInfoResult(TwitchGameInfo gameInfo, int errorCode)
        {
            GameInfo = gameInfo;
            ErrorCode = errorCode;
        }
    }

    public sealed class TwitchUserInfo
    {
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public TwitchBroadcasterType BroadcasterType { get; set; }
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public enum TwitchBroadcasterType { Partner, Affiliate, None }

    public sealed class TwitchVod
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public string StreamId { get; set; }
        public string VideoUrl { get; set; }
        public TimeSpan Length { get; set; }
        public int ViewCount { get; set; }
        public string Type { get; set; }
        public TwitchVodMutedSegments MutedSegments { get; private set; } = new TwitchVodMutedSegments();
        public string DateCreationString { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateDeletion { get; set; }
        public string ImagePreviewTemplateUrl { get; set; }
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
        public const string TWITCH_HELIX_OAUTH_TOKEN_URL_TEMPLATE =
            "https://id.twitch.tv/oauth2/token?client_id={0}&client_secret={1}&grant_type=client_credentials";
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

    public class TwitchVodResult
    {
        public int ErrorCode { get; private set; }
        public TwitchVod TwitchVod { get; private set; }

        public TwitchVodResult(TwitchVod vod, int errorCode)
        {
            TwitchVod = vod;
            ErrorCode = errorCode;
        }
    }

    public class VideoMetadataResult
    {
        public int ErrorCode { get; private set; }
        public JArray Data { get; private set; }

        public VideoMetadataResult(JArray data, int errorCode)
        {
            Data = data;
            ErrorCode = errorCode;
        }
    }
}
