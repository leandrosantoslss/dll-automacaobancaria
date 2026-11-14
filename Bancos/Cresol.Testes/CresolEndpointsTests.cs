using AutomacaoBancaria.Bancos.Cresol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AutomacaoBancaria.Bancos.Cresol.Testes
{
    public class CresolEndpointsTests
    {
        private const string TokenUrl = "https://auth-dev.governarti.com.br/auth/realms/cresol/protocol/openid-connect/token";
        private const string JsonTitulo = "[{\"docPagador\":\"12345678901\",\"dtDocumento\":\"2026-11-04\",\"dtVencimento\":\"2026-12-04\",\"idEspecie\":1,\"numeroDocumento\":\"DOC-1\",\"pagadorBairro\":\"Centro\",\"pagadorCep\":12345000,\"pagadorCidade\":\"Curitiba\",\"pagadorEndereco\":\"Rua A\",\"pagadorNome\":\"Teste\",\"pagadorUf\":\"PR\",\"valorNominal\":10.25}]";
        private const string JsonPagador = "{\"docPagador\":\"12345678901\",\"pagadorBairro\":\"Centro\",\"pagadorCep\":12345000,\"pagadorCidade\":\"Curitiba\",\"pagadorEndereco\":\"Rua A\",\"pagadorNome\":\"Teste\",\"pagadorUf\":\"PR\"}";

        [Fact]
        public async Task Title_routes_use_published_verbs_paths_headers_bodies_and_preserve_203()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"pagina\":0}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"id\":41}");
            handler.Enfileirar(HttpStatusCode.NonAuthoritativeInformation, "{\"id\":42,\"status\":\"EM_PROCESSAMENTO\"}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"alterado\":true}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"baixa\":\"solicitada\"}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                Assert.True((await integracao.TitulosListarAsync("{\"page\":2,\"status\":\"EM_ABERTO\",\"nrDocumento\":\"DOC 1\"}")).Sucesso);
                Assert.True((await integracao.TituloConsultarAsync("41")).Sucesso);
                var inclusao = await integracao.TitulosIncluirAsync(JsonTitulo);
                Assert.True(inclusao.Sucesso);
                Assert.Equal(203, inclusao.StatusCode);
                Assert.Equal("EM_PROCESSAMENTO", ((Newtonsoft.Json.Linq.JObject)inclusao.Dados)["status"].ToString());
                Assert.True((await integracao.TituloAlterarAsync("43", "{\"dtVencimento\":\"2026-12-31\"}")).Sucesso);
                Assert.True((await integracao.TituloBaixarAsync("44")).Sucesso);
            }

            Assert.Equal(6, handler.Requisicoes.Count);
            Assert.Equal(TokenUrl, handler.Requisicoes[0].Uri.AbsoluteUri);
            var requests = handler.Requisicoes.Skip(1).ToArray();
            Assert.Equal(new[] { "GET", "GET", "POST", "PUT", "PUT" }, requests.Select(request => request.Metodo));
            Assert.Equal(new[]
            {
                "https://api-dev.governarti.com.br/titulos",
                "https://api-dev.governarti.com.br/titulos/41",
                "https://api-dev.governarti.com.br/titulos",
                "https://api-dev.governarti.com.br/titulos/43",
                "https://api-dev.governarti.com.br/titulos/44/operacao/baixar"
            }, requests.Select(request => request.Uri.GetLeftPart(UriPartial.Path)));

            var filtros = ParseQuery(requests[0].Uri);
            Assert.Equal("2", filtros["page"]);
            Assert.Equal("EM_ABERTO", filtros["status"]);
            Assert.Equal("DOC 1", filtros["nrDocumento"]);
            Assert.All(requests, request =>
            {
                Assert.Equal("Bearer token", request.Headers["Authorization"]);
                Assert.Equal("conta-homologacao-1", request.Headers["X-Account-Id"]);
                Assert.False(request.Headers.ContainsKey("X-Cooperativa"));
            });
            Assert.Equal(JsonTitulo, requests[2].Corpo);
            Assert.StartsWith("application/json", requests[2].Headers["Content-Type"]);
            Assert.Equal("{\"dtVencimento\":\"2026-12-31\"}", requests[3].Corpo);
            Assert.Null(requests[4].Corpo);
        }

        [Fact]
        public async Task Pagador_routes_use_published_verbs_paths_query_and_raw_json()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "[]");
            handler.Enfileirar(HttpStatusCode.OK, JsonPagador);
            handler.Enfileirar(HttpStatusCode.OK, "{\"id\":51}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"alterado\":true}");
            handler.Enfileirar(HttpStatusCode.OK, "{\"excluido\":true}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                Assert.True((await integracao.PagadoresListarAsync("{\"page\":1,\"size\":20,\"query\":\"João Silva\",\"nomePagador\":\"João\",\"ativo\":true}")).Sucesso);
                Assert.True((await integracao.PagadorConsultarAsync("50")).Sucesso);
                Assert.True((await integracao.PagadorIncluirAsync(JsonPagador)).Sucesso);
                Assert.True((await integracao.PagadorAlterarAsync("51", JsonPagador)).Sucesso);
                Assert.True((await integracao.PagadorExcluirAsync("52")).Sucesso);
            }

            Assert.Equal(6, handler.Requisicoes.Count);
            var requests = handler.Requisicoes.Skip(1).ToArray();
            Assert.Equal(new[] { "GET", "GET", "POST", "PUT", "DELETE" }, requests.Select(request => request.Metodo));
            Assert.Equal(new[]
            {
                "https://api-dev.governarti.com.br/pagadores",
                "https://api-dev.governarti.com.br/pagadores/50",
                "https://api-dev.governarti.com.br/pagadores",
                "https://api-dev.governarti.com.br/pagadores/51",
                "https://api-dev.governarti.com.br/pagadores/52"
            }, requests.Select(request => request.Uri.GetLeftPart(UriPartial.Path)));
            var filtros = ParseQuery(requests[0].Uri);
            Assert.Equal("1", filtros["page"]);
            Assert.Equal("20", filtros["size"]);
            Assert.Equal("João Silva", filtros["query"]);
            Assert.Equal("João", filtros["nomePagador"]);
            Assert.Equal("true", filtros["ativo"]);
            Assert.All(requests, request => Assert.Equal("conta-homologacao-1", request.Headers["X-Account-Id"]));
            Assert.Equal(JsonPagador, requests[2].Corpo);
            Assert.Equal(JsonPagador, requests[3].Corpo);
        }

        [Fact]
        public async Task Occurrence_routes_use_date_query_and_sequencial_path()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "[]");
            handler.Enfileirar(HttpStatusCode.OK, "{\"sequencial\":999}");

            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                Assert.True((await integracao.OcorrenciasSequenciaisAsync("2026-11-01", "2026-11-04")).Sucesso);
                Assert.True((await integracao.OcorrenciaConsultarAsync("999")).Sucesso);
            }

            Assert.Equal(3, handler.Requisicoes.Count);
            var listRequest = handler.Requisicoes[1];
            Assert.Equal("GET", listRequest.Metodo);
            Assert.Equal("https://api-dev.governarti.com.br/ocorrencias/sequenciais", listRequest.Uri.GetLeftPart(UriPartial.Path));
            var filtros = ParseQuery(listRequest.Uri);
            Assert.Equal("2026-11-01", filtros["dateStart"]);
            Assert.Equal("2026-11-04", filtros["dateEnd"]);
            Assert.Equal("https://api-dev.governarti.com.br/ocorrencias/999", handler.Requisicoes[2].Uri.AbsoluteUri);
        }

        [Theory]
        [InlineData("{\"status\":\"EM_ABERTO\",\"method\":\"DELETE\"}")]
        [InlineData("{\"naoPublicado\":\"x\"}")]
        public async Task Title_list_rejects_filters_outside_openapi_allowlist(string filtrosJson)
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosListarAsync(filtrosJson);
                Assert.False(resposta.Sucesso);
                Assert.Contains("filtro", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Theory]
        [InlineData("dtLiquidacaoInicial", "2026-11-01")]
        [InlineData("dtLiquidacaoFinal", "2026-11-04")]
        public async Task Title_list_allows_either_optional_liquidation_date_filter_alone(string nomeFiltro, string data)
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK, "[]");
            var filtrosJson = "{\"" + nomeFiltro + "\":\"" + data + "\"}";

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosListarAsync(filtrosJson);

                Assert.True(resposta.Sucesso);
            }

            Assert.Equal(2, handler.Requisicoes.Count);
            Assert.Equal(data, ParseQuery(handler.Requisicoes[1].Uri)[nomeFiltro]);
        }

        [Fact]
        public async Task Pagador_list_rejects_filters_outside_openapi_allowlist()
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).PagadoresListarAsync("{\"cpf\":\"123\"}");
                Assert.False(resposta.Sucesso);
                Assert.Contains("filtro", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Theory]
        [InlineData("TituloConsultar", "abc")]
        [InlineData("TituloConsultar", "0")]
        [InlineData("PagadorExcluir", "-1")]
        [InlineData("OcorrenciaConsultar", "0")]
        public async Task Resource_ids_must_be_positive_int64(string operacao, string id)
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                var resposta = operacao == "TituloConsultar"
                    ? await integracao.TituloConsultarAsync(id)
                    : operacao == "PagadorExcluir"
                        ? await integracao.PagadorExcluirAsync(id)
                        : await integracao.OcorrenciaConsultarAsync(id);

                Assert.False(resposta.Sucesso);
                Assert.Contains("ID", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Theory]
        [InlineData("\"nao-array\"")]
        [InlineData("{}")]
        public async Task Title_creation_requires_array_json(string json)
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosIncluirAsync(json);
                Assert.False(resposta.Sucesso);
                Assert.Contains("array", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Theory]
        [InlineData("{\"campo\":1}")]
        public async Task Title_update_requires_dtVencimento(string json)
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TituloAlterarAsync("43", json);
                Assert.False(resposta.Sucesso);
                Assert.Contains("dtVencimento", resposta.Mensagem);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Fact]
        public async Task Title_update_requires_a_json_object()
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TituloAlterarAsync("43", "[]");
                Assert.False(resposta.Sucesso);
                Assert.Contains("objeto JSON", resposta.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Fact]
        public async Task Pagador_mutations_require_json_objects()
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var integracao = CriarIntegracao(ConfigurarPassword(), client);
                var incluir = await integracao.PagadorIncluirAsync("[]");
                var alterar = await integracao.PagadorAlterarAsync("52", "null");
                Assert.False(incluir.Sucesso);
                Assert.False(alterar.Sucesso);
                Assert.Contains("objeto JSON", incluir.Mensagem, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("objeto JSON", alterar.Mensagem, StringComparison.OrdinalIgnoreCase);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Theory]
        [InlineData(null, "2026-11-04")]
        [InlineData("2026-11-01", null)]
        [InlineData("2026-02-30", "2026-11-04")]
        public async Task Occurrence_list_requires_both_valid_iso_dates(string dataInicial, string dataFinal)
        {
            var handler = new RecordingHttpMessageHandler();
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client)
                    .OcorrenciasSequenciaisAsync(dataInicial, dataFinal);
                Assert.False(resposta.Sucesso);
                Assert.Contains("dateStart/dateEnd", resposta.Mensagem);
            }

            Assert.Empty(handler.Requisicoes);
        }

        [Fact]
        public async Task Api_error_response_is_returned_sanitized_and_mutation_is_not_retried()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token-secreto-de-teste\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.BadRequest, "{\"erro\":\"token-secreto-de-teste\"}");
            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosIncluirAsync(JsonTitulo);
                Assert.False(resposta.Sucesso);
                Assert.Equal(400, resposta.StatusCode);
                Assert.DoesNotContain("token-secreto-de-teste", resposta.Mensagem);
                Assert.DoesNotContain("token-secreto-de-teste", resposta.Dados?.ToString());
            }

            Assert.Equal(2, handler.Requisicoes.Count);
            Assert.Equal("POST", handler.Requisicoes[1].Metodo);
        }

        [Fact]
        public async Task Api_response_redacts_sensitive_fields_even_when_values_are_not_current_credentials()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token-da-requisicao\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.OK,
                "{\"access_token\":\"outro-token\",\"refresh_token\":\"refresh-secreto\",\"nested\":{\"clientSecret\":\"secret-inesperado\",\"password\":\"senha-inesperada\"}}");

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(ConfigurarPassword(), client).TitulosListarAsync("{}");

                Assert.True(resposta.Sucesso);
                var dados = ((Newtonsoft.Json.Linq.JObject)resposta.Dados).ToString(Newtonsoft.Json.Formatting.None);
                Assert.DoesNotContain("outro-token", dados);
                Assert.DoesNotContain("refresh-secreto", dados);
                Assert.DoesNotContain("secret-inesperado", dados);
                Assert.DoesNotContain("senha-inesperada", dados);
                Assert.Equal("[REDACTED]", ((Newtonsoft.Json.Linq.JObject)resposta.Dados)["access_token"].ToString());
            }
        }

        [Fact]
        public async Task Api_error_redacts_configured_secret_after_json_unescaping()
        {
            var configuracao = ConfigurarPassword();
            configuracao.Senha = "senha\"com\\barra";
            var corpoErro = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                detail = "Credencial refletida: " + configuracao.Senha
            });
            var handler = CriarHandler(HttpStatusCode.OK, "{\"access_token\":\"token\",\"expires_in\":3600}");
            handler.Enfileirar(HttpStatusCode.BadRequest, corpoErro);

            using (var client = new HttpClient(handler))
            {
                var resposta = await CriarIntegracao(configuracao, client).TitulosListarAsync("{}");

                Assert.False(resposta.Sucesso);
                Assert.DoesNotContain(Newtonsoft.Json.JsonConvert.ToString(configuracao.Senha).Trim('"'), resposta.Mensagem);
                Assert.Contains("[REDACTED]", resposta.Mensagem);
            }
        }

        private static RecordingHttpMessageHandler CriarHandler(HttpStatusCode status, string body)
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(status, body);
            return handler;
        }

        private static CresolConfiguracao ConfigurarPassword()
        {
            return new CresolConfiguracao
            {
                Usuario = "usuario-teste",
                Senha = "senha-teste",
                AccountId = "conta-homologacao-1",
                Cooperativa = "nao-enviar-no-password"
            };
        }

        private static CresolIntegracao CriarIntegracao(
            CresolConfiguracao configuracao,
            HttpClient client)
        {
            return new CresolIntegracao(configuracao, client, () => DateTime.UtcNow);
        }

        private static Dictionary<string, string> ParseQuery(Uri uri)
        {
            return uri.Query.TrimStart('?')
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(par => par.Split(new[] { '=' }, 2))
                .ToDictionary(
                    par => Uri.UnescapeDataString(par[0].Replace('+', ' ')),
                    par => par.Length > 1 ? Uri.UnescapeDataString(par[1].Replace('+', ' ')) : string.Empty,
                    StringComparer.Ordinal);
        }

    }
}
