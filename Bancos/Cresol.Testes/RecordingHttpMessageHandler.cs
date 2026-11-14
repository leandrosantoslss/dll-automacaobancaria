using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Cresol.Testes
{
    internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> respostas = new Queue<HttpResponseMessage>();

        public List<RecordedRequest> Requisicoes { get; } = new List<RecordedRequest>();

        public void Enfileirar(HttpStatusCode statusCode, string corpo)
        {
            respostas.Enqueue(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(corpo ?? string.Empty)
            });
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers.ToDictionary(
                item => item.Key,
                item => string.Join(",", item.Value),
                StringComparer.OrdinalIgnoreCase);

            if (request.Content != null)
            {
                foreach (var item in request.Content.Headers)
                    headers[item.Key] = string.Join(",", item.Value);
            }

            var corpo = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync().ConfigureAwait(false);

            Requisicoes.Add(new RecordedRequest
            {
                Metodo = request.Method.Method,
                Uri = request.RequestUri,
                Headers = headers,
                Corpo = corpo
            });

            if (respostas.Count == 0)
                throw new InvalidOperationException("Nenhuma resposta HTTP de teste foi enfileirada.");

            return respostas.Dequeue();
        }
    }

    internal sealed class RecordedRequest
    {
        public string Metodo { get; set; }
        public Uri Uri { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Corpo { get; set; }
    }
}
