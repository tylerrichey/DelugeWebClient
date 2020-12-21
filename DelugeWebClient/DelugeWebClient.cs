using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Deluge.Model;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DelugeWebClient.Test")]

namespace Deluge
{
    public class DelugeWebClient : IDisposable
    {
        private HttpClientHandler _httpClientHandler;
        private HttpClient _httpClient;
        private int _RequestId;
        public string Url { get; }

        public DelugeWebClient(string url)
        {
            _httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            _httpClient = new HttpClient(_httpClientHandler, true);
            _RequestId = 1;

            Url = url;
        }

        public async Task LoginAsync(string password)
        {
            var result = await SendRequestAsync<bool>("auth.login", password);
            if (!result) throw new AuthenticationException("Failed to login.");
        }

        public Task<bool> AuthCheckSessionAsync() => SendRequestAsync<bool>("auth.check_session");

        public async Task LogoutAsync()
        {
            var result = await SendRequestAsync<bool>("auth.delete_session");
            if (!result) throw new DelugeWebClientException("Failed to delete session.", 0);
        }

        public Task<string> AddTorrentMagnetAsync(string uri, TorrentOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentException(nameof(uri));
            var req = CreateRequest("core.add_torrent_magnet", uri, options);
            req.NullValueHandling = NullValueHandling.Ignore;
            return SendRequestAsync<string>(req);
        }

        public Task<bool> RemoveTorrentAsync(string torrentId, bool removeData = false) => SendRequestAsync<bool>("core.remove_torrent", torrentId, removeData);

        public async Task<List<TorrentStatus>> GetTorrentsStatusAsync()
        {
            var emptyFilterDict = new Dictionary<string, string>();
            var keys = Utils.GetAllJsonPropertyFromType(typeof(TorrentStatus));
            var result = await SendRequestAsync<Dictionary<string, TorrentStatus>>("core.get_torrents_status", emptyFilterDict, keys);
            return result.Values.ToList();
        }

        public async Task<SessionStatus> GetSessionStatusAsync()
        {
            var keys = Utils.GetAllJsonPropertyFromType(typeof(SessionStatus));
            return await SendRequestAsync<SessionStatus>("core.get_session_status", keys);
        }

        public Task<DelugeConfig> GetConfigAsync() => SendRequestAsync<DelugeConfig>("core.get_config");

        private Task<T> SendRequestAsync<T>(string method, params object[] parameters) => SendRequestAsync<T>(CreateRequest(method, parameters));

        private async Task<T> SendRequestAsync<T>(WebRequestMessage webRequest) 
        {
            var requestJson = JsonConvert.SerializeObject(webRequest, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = webRequest.NullValueHandling
            });

            var responseJson = await PostJson(requestJson);
            var webResponse = JsonConvert.DeserializeObject<WebResponseMessage<T>>(responseJson);

            if(webResponse.Error != null) throw new DelugeWebClientException(webResponse.Error.Message, webResponse.Error.Code);
            if (webResponse.ResponseId != webRequest.RequestId) throw new DelugeWebClientException("Desync.", 0);

            return webResponse.Result;
        }

        private async Task<string> PostJson(string json)
        {
            StringContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var responseMessage = await _httpClient.PostAsync(Url, content);
            responseMessage.EnsureSuccessStatusCode();

            return await responseMessage.Content.ReadAsStringAsync();
        }

        private WebRequestMessage CreateRequest(string method, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException(nameof(method));
            return new WebRequestMessage(_RequestId++, method, parameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }
        public void Dispose() => Dispose(true);
    }
}
