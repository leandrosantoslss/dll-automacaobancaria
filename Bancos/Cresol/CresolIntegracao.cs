using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomacaoBancaria.Bancos.Cresol
{
    [ComVisible(false)]
    public sealed class CresolIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly CresolConfiguracao _configuracao;
        private readonly HttpClient _httpClientInjetado;
        private readonly Func<DateTime> _agoraUtc;
        private string _accessToken;
        private DateTime _tokenExpiraEmUtc;
        private string _fingerprintToken;

        private static readonly HashSet<string> FiltrosTitulos = new HashSet<string>(StringComparer.Ordinal)
        {
            "page", "size", "status", "dtVencimentoInicial", "dtVencimentoFinal",
            "dtProcessamentoInicial", "dtProcessamentoFinal", "dtLiquidacaoInicial", "dtLiquidacaoFinal",
            "nrDocumento", "nn", "idPagador", "idConta", "idPessoa", "protesto", "tdStatus",
            "idContaCorrente", "idTitulo", "idBoleto", "boletoCresol", "dt_vencimento_ini",
            "dt_vencimento_fim", "dt_processamento_ini", "dt_processamento_fim"
        };

        private static readonly HashSet<string> FiltrosPagadores = new HashSet<string>(StringComparer.Ordinal)
        {
            "page", "size", "query", "nomePagador", "ativo"
        };

        private static readonly HashSet<string> FiltrosDataTitulos = new HashSet<string>(StringComparer.Ordinal)
        {
            "dtVencimentoInicial", "dtVencimentoFinal", "dtProcessamentoInicial", "dtProcessamentoFinal",
            "dtLiquidacaoInicial", "dtLiquidacaoFinal", "dt_vencimento_ini", "dt_vencimento_fim",
            "dt_processamento_ini", "dt_processamento_fim"
        };

        private static readonly string[] CamposObrigatoriosTitulo =
        {
            "docPagador", "dtDocumento", "dtVencimento", "idEspecie", "numeroDocumento",
            "pagadorBairro", "pagadorCep", "pagadorCidade", "pagadorEndereco", "pagadorNome",
            "pagadorUf", "valorNominal"
        };

        private static readonly string[] CamposObrigatoriosPagador =
        {
            "docPagador", "pagadorBairro", "pagadorCep", "pagadorCidade", "pagadorEndereco",
            "pagadorNome", "pagadorUf"
        };

        public CresolIntegracao(CresolConfiguracao configuracao)
            : this(configuracao, null, null)
        {
        }

        public CresolIntegracao(
            CresolConfiguracao configuracao,
            HttpClient httpClient,
            Func<DateTime> agoraUtc)
        {
            _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            _httpClientInjetado = httpClient;
            _agoraUtc = agoraUtc ?? (() => DateTime.UtcNow);
        }

        public override int CodigoBanco => 133;
        public override string Nome => "CRESOL";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto;

        public Task<BancoResposta> TitulosListarAsync(string filtrosJson)
        {
            if (!TentarLerFiltros(filtrosJson, FiltrosTitulos, "títulos", out var filtros, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "titulos", filtros, null);
        }

        public Task<BancoResposta> TituloConsultarAsync(string id)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do título deve ser um inteiro positivo."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "titulos/" + Uri.EscapeDataString(idNormalizado), null, null);
        }

        public Task<BancoResposta> TitulosIncluirAsync(string json)
        {
            if (!TentarLerArrayTitulos(json, out var titulos, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            return ExecutarRequisicaoApiAsync(HttpMethod.Post, "titulos", null, json);
        }

        public Task<BancoResposta> TituloAlterarAsync(string id, string json)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do título deve ser um inteiro positivo."));
            if (!TentarLerObjeto(json, out var titulo, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            if (!titulo.TryGetValue("dtVencimento", StringComparison.Ordinal, out var vencimento)
                || vencimento.Type != JTokenType.String
                || !DataIsoValida(vencimento.Value<string>()))
                return Task.FromResult(BancoResposta.Falha("dtVencimento é obrigatório e deve usar o formato yyyy-MM-dd."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Put, "titulos/" + Uri.EscapeDataString(idNormalizado), null, json);
        }

        public Task<BancoResposta> TituloBaixarAsync(string id)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do título deve ser um inteiro positivo."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Put, "titulos/" + Uri.EscapeDataString(idNormalizado) + "/operacao/baixar", null, null);
        }

        public Task<BancoResposta> PagadoresListarAsync(string filtrosJson)
        {
            if (!TentarLerFiltros(filtrosJson, FiltrosPagadores, "pagadores", out var filtros, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "pagadores", filtros, null);
        }

        public Task<BancoResposta> PagadorConsultarAsync(string id)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do pagador deve ser um inteiro positivo."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "pagadores/" + Uri.EscapeDataString(idNormalizado), null, null);
        }

        public Task<BancoResposta> PagadorIncluirAsync(string json)
        {
            if (!TentarLerObjeto(json, out var pagador, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            if (!ValidarCamposObrigatorios(pagador, CamposObrigatoriosPagador, out erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            return ExecutarRequisicaoApiAsync(HttpMethod.Post, "pagadores", null, json);
        }

        public Task<BancoResposta> PagadorAlterarAsync(string id, string json)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do pagador deve ser um inteiro positivo."));
            if (!TentarLerObjeto(json, out var pagador, out var erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            if (!ValidarCamposObrigatorios(pagador, CamposObrigatoriosPagador, out erro))
                return Task.FromResult(BancoResposta.Falha(erro));
            return ExecutarRequisicaoApiAsync(HttpMethod.Put, "pagadores/" + Uri.EscapeDataString(idNormalizado), null, json);
        }

        public Task<BancoResposta> PagadorExcluirAsync(string id)
        {
            if (!TentarNormalizarId(id, out var idNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID do pagador deve ser um inteiro positivo."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Delete, "pagadores/" + Uri.EscapeDataString(idNormalizado), null, null);
        }

        public Task<BancoResposta> OcorrenciasSequenciaisAsync(string dateStart, string dateEnd)
        {
            if (!DataIsoValida(dateStart) || !DataIsoValida(dateEnd))
                return Task.FromResult(BancoResposta.Falha("dateStart/dateEnd são obrigatórias e devem usar o formato yyyy-MM-dd."));
            var filtros = new JObject
            {
                ["dateStart"] = dateStart,
                ["dateEnd"] = dateEnd
            };
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "ocorrencias/sequenciais", filtros, null);
        }

        public Task<BancoResposta> OcorrenciaConsultarAsync(string sequencial)
        {
            if (!TentarNormalizarId(sequencial, out var sequencialNormalizado))
                return Task.FromResult(BancoResposta.Falha("ID sequencial da ocorrência deve ser um inteiro positivo."));
            return ExecutarRequisicaoApiAsync(HttpMethod.Get, "ocorrencias/" + Uri.EscapeDataString(sequencialNormalizado), null, null);
        }

        private async Task<BancoResposta> ExecutarRequisicaoApiAsync(
            HttpMethod metodo,
            string caminho,
            JObject filtros,
            string corpoJson)
        {
            var erroCredenciais = ValidarCredenciais();
            if (erroCredenciais != null)
                return BancoResposta.Falha(erroCredenciais);

            var token = await ObterAccessTokenAsync().ConfigureAwait(false);
            if (!token.Sucesso)
                return BancoResposta.Falha(token.Mensagem, token.StatusCode);

            var uri = ConstruirUriApi(caminho, filtros ?? new JObject());
            using (var request = new HttpRequestMessage(metodo, uri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrWhiteSpace(_configuracao.AccountId))
                    request.Headers.TryAddWithoutValidation("X-Account-Id", _configuracao.AccountId);
                if (corpoJson != null)
                    request.Content = new StringContent(corpoJson, Encoding.UTF8, "application/json");

                try
                {
                    using (var response = await EnviarAsync(request).ConfigureAwait(false))
                    {
                        var corpo = response.Content == null
                            ? string.Empty
                            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return CriarRespostaApi(response.StatusCode, corpo);
                    }
                }
                catch
                {
                    return BancoResposta.Falha("Falha na comunicação com a API CRESOL.");
                }
            }
        }

        private static bool TentarLerFiltros(string json, HashSet<string> permitidos, string nome, out JObject filtros, out string erro)
        {
            filtros = new JObject();
            erro = null;
            if (string.IsNullOrWhiteSpace(json))
                return true;

            try
            {
                filtros = JObject.Parse(json);
            }
            catch (JsonException)
            {
                erro = "Os filtros de " + nome + " devem ser um objeto JSON válido.";
                return false;
            }

            foreach (var propriedade in filtros.Properties())
            {
                if (!permitidos.Contains(propriedade.Name))
                {
                    erro = "Filtro não publicado para " + nome + ": " + propriedade.Name + ".";
                    return false;
                }
                if (!(propriedade.Value is JValue valor) || valor.Type == JTokenType.Null)
                {
                    erro = "Os valores dos filtros devem ser escalares.";
                    return false;
                }

                if (propriedade.Name == "page" || propriedade.Name == "size"
                    || propriedade.Name == "idPagador" || propriedade.Name == "idConta"
                    || propriedade.Name == "idPessoa" || propriedade.Name == "idContaCorrente"
                    || propriedade.Name == "idTitulo" || propriedade.Name == "idBoleto")
                {
                    if (valor.Type != JTokenType.Integer)
                    {
                        erro = "O filtro " + propriedade.Name + " deve ser inteiro.";
                        return false;
                    }
                }
                else if (propriedade.Name == "ativo")
                {
                    if (valor.Type != JTokenType.Boolean)
                    {
                        erro = "O filtro ativo deve ser booleano.";
                        return false;
                    }
                }
                else if (valor.Type != JTokenType.String)
                {
                    erro = "O filtro " + propriedade.Name + " deve ser texto.";
                    return false;
                }

                if (FiltrosDataTitulos.Contains(propriedade.Name) && !DataIsoValida(valor.Value<string>()))
                {
                    erro = "O filtro " + propriedade.Name + " deve usar o formato yyyy-MM-dd.";
                    return false;
                }
            }

            return true;
        }

        private static bool TentarLerArrayTitulos(string json, out JArray titulos, out string erro)
        {
            titulos = null;
            erro = null;
            try
            {
                titulos = JArray.Parse(json ?? string.Empty);
            }
            catch (JsonException)
            {
                erro = "A inclusão de títulos exige um array JSON CRESOL válido.";
                return false;
            }

            foreach (var item in titulos)
            {
                if (!(item is JObject titulo))
                {
                    erro = "Cada título do array deve ser um objeto JSON.";
                    return false;
                }
                if (!ValidarCamposObrigatorios(titulo, CamposObrigatoriosTitulo, out erro))
                    return false;
                if (!DataIsoValida(titulo["dtDocumento"].Value<string>())
                    || !DataIsoValida(titulo["dtVencimento"].Value<string>()))
                {
                    erro = "dtDocumento e dtVencimento devem usar o formato yyyy-MM-dd.";
                    return false;
                }
            }

            return true;
        }

        private static bool TentarLerObjeto(string json, out JObject objeto, out string erro)
        {
            objeto = null;
            erro = null;
            try
            {
                objeto = JObject.Parse(json ?? string.Empty);
                return true;
            }
            catch (JsonException)
            {
                erro = "A operação exige um objeto JSON válido.";
                return false;
            }
        }

        private static bool ValidarCamposObrigatorios(JObject objeto, IEnumerable<string> campos, out string erro)
        {
            foreach (var campo in campos)
            {
                var valor = objeto[campo];
                if (valor == null || valor.Type == JTokenType.Null)
                {
                    erro = "Campo obrigatório ausente no JSON CRESOL: " + campo + ".";
                    return false;
                }

                var tipoValido = campo == "pagadorCep" || campo == "idEspecie"
                    ? valor.Type == JTokenType.Integer
                    : campo == "valorNominal"
                        ? valor.Type == JTokenType.Integer || valor.Type == JTokenType.Float
                        : valor.Type == JTokenType.String;
                if (!tipoValido)
                {
                    erro = "Tipo inválido no campo obrigatório do JSON CRESOL: " + campo + ".";
                    return false;
                }
            }

            erro = null;
            return true;
        }

        private static bool TentarNormalizarId(string id, out string normalizado)
        {
            normalizado = null;
            if (!long.TryParse(id?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var valor) || valor <= 0)
                return false;
            normalizado = valor.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        private static bool DataIsoValida(string valor)
            => DateTime.TryParseExact(
                valor,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);

        private async Task<ResultadoToken> ObterAccessTokenAsync()
        {
            var fingerprint = CriarFingerprintAutenticacao();
            var agora = _agoraUtc();
            if (!string.IsNullOrWhiteSpace(_accessToken)
                && string.Equals(_fingerprintToken, fingerprint, StringComparison.Ordinal)
                && agora.AddSeconds(30) < _tokenExpiraEmUtc)
            {
                return ResultadoToken.Ok(_accessToken);
            }

            var campos = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", CresolConfiguracao.PasswordGrantType),
                new KeyValuePair<string, string>("client_id", CresolConfiguracao.PasswordClientId),
                new KeyValuePair<string, string>("username", _configuracao.Usuario),
                new KeyValuePair<string, string>("password", _configuracao.Senha),
                new KeyValuePair<string, string>("client_secret", CresolConfiguracao.PasswordClientSecret),
                new KeyValuePair<string, string>("scope", CresolConfiguracao.PasswordScope)
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, CresolConfiguracao.TokenUrlPasswordHomologacao))
            {
                request.Content = new FormUrlEncodedContent(campos);
                try
                {
                    using (var response = await EnviarAsync(request).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                            return ResultadoToken.Falha("Falha na autenticação CRESOL.", (int)response.StatusCode);

                        var corpo = response.Content == null
                            ? string.Empty
                            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        JObject payload;
                        try
                        {
                            payload = JObject.Parse(corpo);
                        }
                        catch (JsonException)
                        {
                            return ResultadoToken.Falha("Resposta de autenticação CRESOL inválida.", (int)response.StatusCode);
                        }

                        var accessToken = payload["access_token"]?.Value<string>();
                        if (string.IsNullOrWhiteSpace(accessToken))
                            return ResultadoToken.Falha("Resposta de autenticação CRESOL sem access_token.", (int)response.StatusCode);

                        var expiresIn = payload["expires_in"]?.Value<double?>();
                        _accessToken = accessToken;
                        _fingerprintToken = fingerprint;
                        _tokenExpiraEmUtc = expiresIn.HasValue && expiresIn.Value > 0
                            ? agora.AddSeconds(expiresIn.Value)
                            : DateTime.MinValue;

                        return ResultadoToken.Ok(accessToken);
                    }
                }
                catch
                {
                    return ResultadoToken.Falha("Falha na autenticação CRESOL.");
                }
            }
        }

        private async Task<HttpResponseMessage> EnviarAsync(HttpRequestMessage request)
        {
            if (_httpClientInjetado != null)
                return await _httpClientInjetado.SendAsync(request).ConfigureAwait(false);

            using (var client = new HttpClient())
                return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
        }

        private string CriarFingerprintAutenticacao()
        {
            var valores = new[]
            {
                CresolConfiguracao.TokenUrlPasswordHomologacao,
                CresolConfiguracao.PasswordClientId,
                CresolConfiguracao.PasswordClientSecret,
                CresolConfiguracao.PasswordScope,
                _configuracao.Usuario,
                _configuracao.Senha
            };
            var bytes = Encoding.UTF8.GetBytes(string.Join("\0", valores.Select(valor => valor ?? string.Empty)));
            try
            {
                using (var sha256 = SHA256.Create())
                    return Convert.ToBase64String(sha256.ComputeHash(bytes));
            }
            finally
            {
                Array.Clear(bytes, 0, bytes.Length);
            }
        }

        private Uri ConstruirUriApi(string caminho, JObject filtros)
        {
            var baseUrl = new Uri(CresolConfiguracao.ApiBaseUrlHomologacao, UriKind.Absolute);
            var endpoint = new Uri(baseUrl, caminho);
            var pares = filtros.Properties()
                .Select(propriedade => Uri.EscapeDataString(propriedade.Name) + "=" + Uri.EscapeDataString(ValorQuery(propriedade.Value)))
                .ToArray();
            return pares.Length == 0 ? endpoint : new Uri(endpoint.AbsoluteUri + "?" + string.Join("&", pares));
        }

        private static string ValorQuery(JToken token)
        {
            var valor = ((JValue)token).Value;
            if (valor is bool booleano)
                return booleano ? "true" : "false";
            if (valor is IFormattable formatavel)
                return formatavel.ToString(null, CultureInfo.InvariantCulture);
            return Convert.ToString(valor, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private BancoResposta CriarRespostaApi(HttpStatusCode statusCode, string corpo)
        {
            var sanitizado = Sanitizar(corpo);
            JToken dados = null;
            if (!string.IsNullOrWhiteSpace(sanitizado))
            {
                try
                {
                    dados = JToken.Parse(sanitizado);
                }
                catch (JsonException)
                {
                    dados = new JValue(sanitizado);
                }
            }

            if ((int)statusCode >= 200 && (int)statusCode <= 299)
                return new BancoResposta { Sucesso = true, StatusCode = (int)statusCode, Dados = dados };

            return BancoResposta.Falha(
                string.IsNullOrWhiteSpace(sanitizado) ? "Falha na chamada à API CRESOL." : sanitizado,
                (int)statusCode);
        }

        private string Sanitizar(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            var segredos = new[] { _accessToken, _configuracao.Senha, CresolConfiguracao.PasswordClientSecret }
                .Where(segredo => !string.IsNullOrEmpty(segredo))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            try
            {
                var json = JToken.Parse(valor);
                SanitizarPropriedadesSensivel(json, segredos);
                return json.ToString(Formatting.None);
            }
            catch (JsonException)
            {
                return SanitizarTexto(valor, segredos);
            }
        }

        private static void SanitizarPropriedadesSensivel(JToken token, IEnumerable<string> segredos)
        {
            if (token is JObject objeto)
            {
                foreach (var propriedade in objeto.Properties().ToList())
                {
                    if (NomePropriedadeSensivel(propriedade.Name))
                        propriedade.Value = "[REDACTED]";
                    else
                        SanitizarPropriedadesSensivel(propriedade.Value, segredos);
                }
            }
            else if (token is JArray array)
            {
                foreach (var item in array)
                    SanitizarPropriedadesSensivel(item, segredos);
            }
            else if (token is JValue valor && valor.Type == JTokenType.String)
            {
                valor.Value = SanitizarTexto(valor.Value<string>(), segredos);
            }
        }

        private static string SanitizarTexto(string valor, IEnumerable<string> segredos)
        {
            var resultado = valor;
            foreach (var segredo in segredos)
            {
                if (!string.IsNullOrEmpty(segredo))
                    resultado = resultado.Replace(segredo, "[REDACTED]");
            }

            return resultado;
        }

        private static bool NomePropriedadeSensivel(string nome)
        {
            var normalizado = new string((nome ?? string.Empty)
                .Where(char.IsLetterOrDigit)
                .ToArray())
                .ToLowerInvariant();

            return normalizado == "token"
                || normalizado.EndsWith("token", StringComparison.Ordinal)
                || normalizado == "jwt"
                || normalizado.EndsWith("jwt", StringComparison.Ordinal)
                || normalizado.Contains("secret")
                || normalizado.Contains("password")
                || normalizado.Contains("passwd")
                || normalizado.Contains("credential")
                || normalizado.Contains("authorization")
                || normalizado.EndsWith("apikey", StringComparison.Ordinal)
                || normalizado.Contains("privatekey");
        }

        private string ValidarCredenciais()
        {
            if (string.IsNullOrWhiteSpace(_configuracao.Usuario)
                || string.IsNullOrWhiteSpace(_configuracao.Senha))
                return "CresolUsuario e CresolSenha são obrigatórios para autenticação PASSWORD.";

            return null;
        }

        private sealed class ResultadoToken
        {
            public bool Sucesso { get; private set; }
            public string AccessToken { get; private set; }
            public string Mensagem { get; private set; }
            public int? StatusCode { get; private set; }

            public static ResultadoToken Ok(string accessToken)
                => new ResultadoToken { Sucesso = true, AccessToken = accessToken };

            public static ResultadoToken Falha(string mensagem, int? statusCode = null)
                => new ResultadoToken { Mensagem = mensagem, StatusCode = statusCode };
        }

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("CRESOL requer JSON próprio; use CresolTitulosIncluir."));

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("CRESOL requer consulta explícita de título."));

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Operação de PIX não suportada para CRESOL."));

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de PIX não suportada para CRESOL."));

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de extrato não suportada para CRESOL."));

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de saldo não suportada para CRESOL."));

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao?.BoletoPadrao == null)
                return Task.FromResult(BancoResposta.Falha("CRESOL pela emissão comum aceita somente um boleto simples."));

            var boleto = requisicao.BoletoPadrao;
            var pagador = boleto.Pagador;
            if (pagador == null)
                return Task.FromResult(BancoResposta.Falha("Pagador não informado para emissão CRESOL."));
            if (_configuracao.IdEspecie <= 0)
                return Task.FromResult(BancoResposta.Falha("CresolIdEspecie deve ser um inteiro positivo."));
            if (boleto.Valor <= 0 || boleto.Vencimento == default(DateTime))
                return Task.FromResult(BancoResposta.Falha("Valor e vencimento válidos são obrigatórios para emissão CRESOL."));

            if (string.IsNullOrWhiteSpace(pagador.Documento)
                || string.IsNullOrWhiteSpace(pagador.Nome)
                || string.IsNullOrWhiteSpace(pagador.Endereco)
                || string.IsNullOrWhiteSpace(pagador.Bairro)
                || string.IsNullOrWhiteSpace(pagador.Cidade)
                || string.IsNullOrWhiteSpace(pagador.Uf)
                || string.IsNullOrWhiteSpace(boleto.NumeroDocumento))
                return Task.FromResult(BancoResposta.Falha("Documento, nome e endereço do pagador, número do documento, bairro, cidade e UF são obrigatórios para emissão CRESOL."));

            var cep = new string((pagador.Cep ?? string.Empty).Where(caractere => caractere >= '0' && caractere <= '9').ToArray());
            if (!int.TryParse(cep, NumberStyles.None, CultureInfo.InvariantCulture, out var cepNumerico))
                return Task.FromResult(BancoResposta.Falha("CEP do pagador deve conter dígitos para emissão CRESOL."));

            var titulo = new JObject
            {
                ["docPagador"] = pagador.Documento,
                ["dtDocumento"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["dtVencimento"] = boleto.Vencimento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["idEspecie"] = _configuracao.IdEspecie,
                ["numeroDocumento"] = boleto.NumeroDocumento,
                ["pagadorBairro"] = pagador.Bairro,
                ["pagadorCep"] = cepNumerico,
                ["pagadorCidade"] = pagador.Cidade,
                ["pagadorEndereco"] = pagador.Endereco,
                ["pagadorNome"] = pagador.Nome,
                ["pagadorUf"] = pagador.Uf,
                ["valorNominal"] = boleto.Valor
            };

            if (!string.IsNullOrWhiteSpace(pagador.Numero))
                titulo["pagadorEnderecoNumero"] = pagador.Numero;
            if (!string.IsNullOrWhiteSpace(boleto.NossoNumero) && boleto.NossoNumero != "0")
            {
                if (!long.TryParse(boleto.NossoNumero, NumberStyles.None, CultureInfo.InvariantCulture, out var nossoNumero) || nossoNumero <= 0)
                    return Task.FromResult(BancoResposta.Falha("NossoNumero deve ser um inteiro positivo para emissão CRESOL."));
                titulo["nossoNumero"] = nossoNumero;
            }

            return TitulosIncluirAsync(new JArray(titulo).ToString(Formatting.None));
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null)
                return Task.FromResult(BancoResposta.Falha("Requisição de consulta CRESOL inválida."));
            if (!string.IsNullOrWhiteSpace(requisicao.Id)
                && !string.Equals(requisicao.Id.Trim(), "0", StringComparison.Ordinal))
                return TituloConsultarAsync(requisicao.Id);

            var filtros = new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.NossoNumero) && requisicao.NossoNumero != "0")
                filtros["nn"] = requisicao.NossoNumero;
            if (requisicao.Inicio.HasValue)
                filtros["dtVencimentoInicial"] = requisicao.Inicio.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (requisicao.Fim.HasValue)
                filtros["dtVencimentoFinal"] = requisicao.Fim.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (!filtros.Properties().Any())
                return Task.FromResult(BancoResposta.Falha("Consulta CRESOL exige ID, NossoNumero ou período de vencimento."));

            return TitulosListarAsync(filtros.ToString(Formatting.None));
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
            => Task.FromResult(BancoResposta.Falha("Operação de PIX não suportada para CRESOL."));

        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de PIX não suportada para CRESOL."));

        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de extrato não suportada para CRESOL."));

        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de saldo não suportada para CRESOL."));
    }
}
