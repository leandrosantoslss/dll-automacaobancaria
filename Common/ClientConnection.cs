using System;

namespace AutomacaoBancaria.Common
{
    /// <summary>
    /// Representa os parametros de conexão utilizados pelo client
    /// </summary>
    public class ClientConnection
    {
        public readonly string UserAgent;
        public readonly string ApiToken;
        public readonly string ApiVersion;
        public readonly string ApiUrl;
        public readonly string ApiReturnUrl;
        public readonly string ClientId;
        public readonly string ClientSecret;

        public ClientConnection(string apiUrl, string apiVersion, string apiToken, string userAgent, string apiReturnUrl, string clientId, string clientSecret)
        {
            ApiUrl = apiUrl;
            ApiVersion = apiVersion;
            ApiToken = apiToken;
            UserAgent = userAgent;
            ApiReturnUrl = apiReturnUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public bool IsOAuthConnection() => !(string.IsNullOrEmpty(ClientId) && string.IsNullOrEmpty(ClientSecret));
        public Uri GetBaseUri() => new Uri($"{ApiUrl}/{ApiVersion}");
    }
}
