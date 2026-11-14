using AutomacaoBancaria.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AutomacaoBancaria
{
    /// <summary>
    /// Cliente de acesso aos recursos da Api do boleto simples
    /// </summary>
    public class AutomacaoBancariaClient : IDisposable
    {
        public readonly ClientConnection Connection;

        /// <summary>
        /// Api que prove informações do usuário pelo token de acesso
        /// </summary>
        public readonly AuthApi Auth;

        private readonly HttpClient _client;

        /// <summary>
        /// Client default com dados da conexão de externos ao seu arquivo de configuração
        /// </summary>
        /// <param name="clientConnection">Dados de conexão com a api</param>
        public AutomacaoBancariaClient(ClientConnection clientConnection) : this(new HttpClient(), clientConnection)
        { }

        /// <summary>
        /// Client http customizado e dados da conexão de externos ao seu arquivo de configuração
        /// </summary>
        /// <param name="client">Sua versão do HttpClient</param>
        /// <param name="clientConnection">Dados de conexão com a api</param>
        public AutomacaoBancariaClient(HttpClient client, ClientConnection clientConnection)
        {
            _client = client;
            _client.BaseAddress = clientConnection.GetBaseUri();
            Connection = clientConnection;

            Auth = new AuthApi(this);
        }

        internal async Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request, default(CancellationToken));
            request.Dispose();

            return new ApiResponse<T>(response);
        }

        internal async Task<PagedApiResponse<T>> SendPagedAsync<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request, default(CancellationToken));
            request.Dispose();

            return new PagedApiResponse<T>(response);
        }

        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request, default(CancellationToken));
            request.Dispose();

            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
