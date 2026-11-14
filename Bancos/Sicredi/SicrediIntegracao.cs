using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Sicredi
{
    public class SicrediConfiguracao
    {
        public string ApiKey { get; set; }
        public string CodigoAcesso { get; set; }
        public string CodigoBeneficiario { get; set; }
        public string Cooperativa { get; set; }
        public string Posto { get; set; }
        public bool Sandbox { get; set; }
        public string BaseUrl { get; set; }

        public string ObterBaseUrl()
        {
            if (!string.IsNullOrWhiteSpace(BaseUrl)) return BaseUrl.TrimEnd('/');
            return Sandbox ? "https://api-parceiro.sicredi.com.br/sb" : "https://api-parceiro.sicredi.com.br";
        }
    }

    public sealed class SicrediIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private const string SandboxCodigoAcesso = "teste123";
        private const string SandboxCodigoBeneficiario = "12345";
        private const string SandboxCooperativa = "6789";
        private const string SandboxPosto = "03";
        private readonly SicrediConfiguracao configuracao;
        private string accessToken;
        private string refreshToken;
        private DateTime accessTokenExpiraEm;

        public SicrediIntegracao(SicrediConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (configuracao.Sandbox)
            {
                configuracao.CodigoAcesso = SandboxCodigoAcesso;
                configuracao.CodigoBeneficiario = SandboxCodigoBeneficiario;
                configuracao.Cooperativa = SandboxCooperativa;
                configuracao.Posto = SandboxPosto;
            }
            if (string.IsNullOrWhiteSpace(configuracao.ApiKey)) throw new ArgumentException("A x-api-key do Sicredi é obrigatória", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.CodigoAcesso)) throw new ArgumentException("O código de acesso do Sicredi é obrigatório", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.CodigoBeneficiario)) throw new ArgumentException("O código do beneficiário Sicredi é obrigatório", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.Cooperativa)) throw new ArgumentException("A cooperativa Sicredi é obrigatória", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.Posto)) throw new ArgumentException("O posto Sicredi é obrigatório", nameof(configuracao));
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public override int CodigoBanco => 748;
        public override string Nome => "Sicredi";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => EnviarCobrancaAsync(HttpMethod.Post, "/cobranca/boleto/v1/boletos", requisicao?.Corpo, requisicao?.Parametros, requisicao?.Cabecalhos);

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
            => ConsultarBoletoInternoAsync(requisicao?.Parametros, requisicao?.Cabecalhos);

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("O Sicredi cria PIX somente pelo registro de boleto híbrido"));

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
            => ConsultarBoletoInternoAsync(requisicao?.Parametros, requisicao?.Cabecalhos);

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de extrato não é disponibilizada pela API de Cobrança Sicredi"));

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de saldo não é disponibilizada pela API de Cobrança Sicredi"));

        public async Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return BancoResposta.Falha("Requisição inválida");
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao;
                if (boleto == null && requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0) boleto = requisicao.BoletosPadrao[0];
                if (boleto == null) return BancoResposta.Falha("Boleto ou payload Sicredi não informado");
                req.Corpo = CriarCorpoBoleto(boleto);
            }
            return await EmitirBoletoAsync(req);
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.NossoNumero)) parametros["nossoNumero"] = requisicao.NossoNumero;
            return ConsultarBoletoInternoAsync(parametros, requisicao.Cabecalhos);
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
            => Task.FromResult(BancoResposta.Falha("O Sicredi cria PIX somente pelo registro de boleto híbrido"));

        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao)
            => ConsultarBoletoInternoAsync(requisicao?.Parametros, requisicao?.Cabecalhos);

        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao) => ConsultarExtratoAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private Task<BancoResposta> ConsultarBoletoInternoAsync(JObject parametros, JObject cabecalhos)
        {
            parametros = parametros ?? new JObject();
            var nossoNumero = SicrediFormatacao.MascararNossoNumero(parametros.Value<string>("nossoNumero"));
            if (string.IsNullOrWhiteSpace(nossoNumero)) return Task.FromResult(BancoResposta.Falha("nossoNumero é obrigatório para consulta Sicredi"));
            parametros["nossoNumero"] = nossoNumero;
            if (string.IsNullOrWhiteSpace(parametros.Value<string>("codigoBeneficiario"))) parametros["codigoBeneficiario"] = configuracao.CodigoBeneficiario;
            return EnviarCobrancaAsync(HttpMethod.Get, "/cobranca/boleto/v1/boletos", null, parametros, cabecalhos);
        }

        private JObject CriarCorpoBoleto(BoletoPadrao boleto)
        {
            var documento = ApenasDigitos(boleto.Pagador?.Documento);
            var pagador = new JObject { ["tipoPessoa"] = documento != null && documento.Length > 11 ? "PESSOA_JURIDICA" : "PESSOA_FISICA", ["documento"] = documento, ["nome"] = boleto.Pagador?.Nome };
            AdicionarSePreenchido(pagador, "endereco", boleto.Pagador?.Endereco);
            AdicionarSePreenchido(pagador, "cidade", boleto.Pagador?.Cidade);
            AdicionarSePreenchido(pagador, "uf", boleto.Pagador?.Uf);
            AdicionarSePreenchido(pagador, "cep", ApenasDigitos(boleto.Pagador?.Cep));
            AdicionarSePreenchido(pagador, "telefone", ApenasDigitos(boleto.Pagador?.Telefone));
            AdicionarSePreenchido(pagador, "email", boleto.Pagador?.Email);
            var corpo = new JObject
            {
                ["tipoCobranca"] = "HIBRIDO",
                ["codigoBeneficiario"] = configuracao.CodigoBeneficiario,
                ["pagador"] = pagador,
                ["especieDocumento"] = "DUPLICATA_MERCANTIL_INDICACAO",
                ["seuNumero"] = boleto.SeuNumero ?? boleto.NumeroDocumento ?? Guid.NewGuid().ToString("N").Substring(0, 10),
                ["dataVencimento"] = boleto.Vencimento.ToString("yyyy-MM-dd"),
                ["valor"] = boleto.Valor
            };
            AdicionarSePreenchido(corpo, "nossoNumero", boleto.NossoNumero);
            var informativos = new JArray();
            AdicionarSePreenchido(informativos, boleto.Instrucao1);
            AdicionarSePreenchido(informativos, boleto.Instrucao2);
            AdicionarSePreenchido(informativos, boleto.Instrucao3);
            if (informativos.Count > 0) corpo["informativos"] = informativos;
            return corpo;
        }

        private async Task<BancoResposta> EnviarCobrancaAsync(HttpMethod metodo, string caminho, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            var autenticacao = await ObterAccessTokenAsync();
            if (!autenticacao.Sucesso) return autenticacao;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(metodo, configuracao.ObterBaseUrl() + caminho + CriarQuery(parametros)))
            {
                request.Headers.Add("x-api-key", configuracao.ApiKey);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("cooperativa", configuracao.Cooperativa);
                request.Headers.Add("posto", configuracao.Posto);
                AdicionarCabecalhos(request, cabecalhos);
                if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                return await LerRespostaAsync(await client.SendAsync(request));
            }
        }

        private async Task<BancoResposta> ObterAccessTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(accessToken) && accessTokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
            var valores = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("scope", "cobranca") };
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                valores.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                valores.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
            }
            else
            {
                valores.Add(new KeyValuePair<string, string>("grant_type", "password"));
                valores.Add(new KeyValuePair<string, string>("username", configuracao.CodigoBeneficiario + configuracao.Cooperativa));
                valores.Add(new KeyValuePair<string, string>("password", configuracao.CodigoAcesso));
            }
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, configuracao.ObterBaseUrl() + "/auth/openapi/token"))
            {
                request.Headers.Add("x-api-key", configuracao.ApiKey);
                request.Headers.Add("context", "COBRANCA");
                request.Content = new FormUrlEncodedContent(valores);
                var resposta = await LerRespostaAsync(await client.SendAsync(request));
                if (!resposta.Sucesso) return resposta;
                accessToken = resposta.Dados?.Value<string>("access_token");
                refreshToken = resposta.Dados?.Value<string>("refresh_token");
                var expiraEm = resposta.Dados?.Value<int?>("expires_in") ?? 0;
                if (string.IsNullOrWhiteSpace(accessToken) || expiraEm <= 0) return BancoResposta.Falha("Resposta de autenticação Sicredi inválida");
                accessTokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                return BancoResposta.Ok(null);
            }
        }

        private static async Task<BancoResposta> LerRespostaAsync(HttpResponseMessage response)
        {
            var texto = await response.Content.ReadAsStringAsync();
            JToken dados = null;
            if (!string.IsNullOrWhiteSpace(texto))
            {
                try { dados = JToken.Parse(texto); }
                catch { dados = new JObject { ["conteudo"] = texto }; }
            }
            if (response.IsSuccessStatusCode) return BancoResposta.Ok(dados);
            return BancoResposta.Falha(dados?["message"]?.ToString() ?? dados?["mensagem"]?.ToString() ?? texto ?? "Erro retornado pela API do Sicredi", (int)response.StatusCode);
        }

        private static string CriarQuery(JObject parametros)
        {
            if (parametros == null || parametros.Count == 0) return string.Empty;
            var valores = new List<string>();
            foreach (var parametro in parametros)
                if (parametro.Value != null && parametro.Value.Type != JTokenType.Object && parametro.Value.Type != JTokenType.Array)
                    valores.Add(Uri.EscapeDataString(parametro.Key) + "=" + Uri.EscapeDataString(parametro.Value.ToString()));
            return valores.Count == 0 ? string.Empty : "?" + string.Join("&", valores);
        }

        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos)
        {
            if (cabecalhos == null) return;
            foreach (var cabecalho in cabecalhos)
                if (cabecalho.Value != null && !request.Headers.Contains(cabecalho.Key)) request.Headers.TryAddWithoutValidation(cabecalho.Key, cabecalho.Value.ToString());
        }

        private static void AdicionarSePreenchido(JObject objeto, string nome, string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor)) objeto[nome] = valor;
        }

        private static void AdicionarSePreenchido(JArray array, string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor)) array.Add(valor);
        }

        private static string ApenasDigitos(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            var resultado = new StringBuilder();
            foreach (var caractere in valor) if (char.IsDigit(caractere)) resultado.Append(caractere);
            return resultado.Length == 0 ? null : resultado.ToString();
        }
    }

    internal static class SicrediFormatacao
    {
        public static string MascararNossoNumero(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            var digitos = new StringBuilder(9);
            foreach (var caractere in valor)
            {
                if (!char.IsDigit(caractere)) continue;
                digitos.Append(caractere);
                if (digitos.Length == 9) break;
            }
            return digitos.Length == 0 ? null : digitos.ToString().PadLeft(9, '0');
        }
    }
}
