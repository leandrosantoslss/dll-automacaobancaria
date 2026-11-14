using AutomacaoBancaria.Bancos.Cresol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AutomacaoBancaria.Bancos.Cresol.Testes
{
    public class CresolAuthenticationTests
    {
        private const string UrlToken = "https://auth-dev.governarti.com.br/auth/realms/cresol/protocol/openid-connect/token";

        [Fact]
        public async Task Password_grant_sends_guide_constants_and_bearer_token()
        {
            var config = ConfigurarPassword();
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(HttpStatusCode.OK, "{\"access_token\":\"token-de-teste\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"itens\":[]}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(config, client);
                var resposta = await integracao.TitulosListarAsync("{}");

                Assert.True(resposta.Sucesso);
                Assert.Equal(200, resposta.StatusCode);
            }

            Assert.Equal(2, handler.Requisicoes.Count);
            var tokenRequest = handler.Requisicoes[0];
            Assert.Equal("POST", tokenRequest.Metodo);
            Assert.Equal(UrlToken, tokenRequest.Uri.AbsoluteUri);
            Assert.StartsWith("application/x-www-form-urlencoded", tokenRequest.Headers["Content-Type"]);

            var form = ParseForm(tokenRequest.Corpo);
            Assert.Equal("password", form["grant_type"]);
            Assert.Equal(config.Usuario, form["username"]);
            Assert.Equal(config.Senha, form["password"]);
            Assert.Equal("cresolApi", form["client_id"]);
            Assert.Equal("cr3s0l4p1", form["client_secret"]);
            Assert.Equal("read", form["scope"]);

            var resourceRequest = handler.Requisicoes[1];
            Assert.Equal("GET", resourceRequest.Metodo);
            Assert.Equal("https://api-dev.governarti.com.br/titulos", resourceRequest.Uri.AbsoluteUri);
            Assert.Equal("Bearer token-de-teste", resourceRequest.Headers["Authorization"]);
        }

        [Theory]
        [InlineData(null, "senha-teste")]
        [InlineData("", "senha-teste")]
        [InlineData("usuario-teste", null)]
        [InlineData("usuario-teste", "")]
        public async Task Missing_password_credentials_fail_without_http(string usuario, string senha)
        {
            var config = ConfigurarPassword();
            config.Usuario = usuario;
            config.Senha = senha;
            var handler = new RecordingHttpMessageHandler();

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(config, client).TitulosListarAsync("{}");

                Assert.False(resposta.Sucesso);
                Assert.Contains("senha", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Fact]
        public async Task Password_authentication_failure_does_not_fallback_to_another_grant()
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(HttpStatusCode.Unauthorized, "{\"error\":\"invalid_grant\"}");

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosListarAsync("{}");

                Assert.False(resposta.Sucesso);
                Assert.Equal(401, resposta.StatusCode);
            }

            Assert.Single(handler.Requisicoes);
            Assert.Equal(UrlToken, handler.Requisicoes[0].Uri.AbsoluteUri);
        }

        [Fact]
        public async Task Expired_token_is_requested_again_without_refresh_grant()
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(HttpStatusCode.OK, "{\"access_token\":\"token-1\",\"expires_in\":0}");
            handler.Enfileirar(HttpStatusCode.OK, "{}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"access_token\":\"token-2\",\"expires_in\":0}");
            handler.Enfileirar(HttpStatusCode.OK, "{}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                Assert.True((await integracao.TitulosListarAsync("{}")).Sucesso);
                Assert.True((await integracao.TitulosListarAsync("{}")).Sucesso);
            }

            Assert.Equal(4, handler.Requisicoes.Count);
            Assert.All(handler.Requisicoes.Where(request => request.Uri.AbsoluteUri == UrlToken), request =>
                Assert.Equal("password", ParseForm(request.Corpo)["grant_type"]));
        }

        [Fact]
        public async Task Changing_password_invalidates_cached_token()
        {
            var config = ConfigurarPassword();
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(HttpStatusCode.OK, "{\"access_token\":\"token-1\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "{}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"access_token\":\"token-2\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "{}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(config, client);
                Assert.True((await integracao.TitulosListarAsync("{}")).Sucesso);
                config.Senha = "senha-alterada-de-teste";
                Assert.True((await integracao.TitulosListarAsync("{}")).Sucesso);
            }

            var tokenRequests = handler.Requisicoes.Where(request => request.Uri.AbsoluteUri == UrlToken).ToArray();
            Assert.Equal(2, tokenRequests.Length);
            Assert.Equal("senha-alterada-de-teste", ParseForm(tokenRequests[1].Corpo)["password"]);
        }

        [Fact]
        public async Task Token_error_body_is_not_returned_with_credentials()
        {
            var config = ConfigurarPassword();
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(HttpStatusCode.BadRequest,
                "{\"error_description\":\"senha-de-teste client-secret-de-teste\"}");

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(config, client).TitulosListarAsync("{}");

                Assert.False(resposta.Sucesso);
                Assert.DoesNotContain(config.Senha, resposta.Mensagem);
                Assert.DoesNotContain("cr3s0l4p1", resposta.Mensagem);
                Assert.Null(resposta.Dados);
            }
        }

        private static CresolConfiguracao ConfigurarPassword()
        {
            return new CresolConfiguracao
            {
                Usuario = "usuario-de-teste",
                Senha = "senha-de-teste"
            };
        }

        private static CresolIntegracao CriarIntegracao(
            CresolConfiguracao configuracao,
            HttpClient client)
        {
            return new CresolIntegracao(configuracao, client, () => DateTime.UtcNow);
        }

        private static Dictionary<string, string> ParseForm(string form)
        {
            return form.Split('&')
                .Select(part => part.Split(new[] { '=' }, 2))
                .ToDictionary(
                    part => Uri.UnescapeDataString(part[0].Replace('+', ' ')),
                    part => part.Length > 1 ? Uri.UnescapeDataString(part[1].Replace('+', ' ')) : string.Empty,
                    StringComparer.Ordinal);
        }

    }
}
