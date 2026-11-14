using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Banrisul
{
    public class BanrisulConfiguracao
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Beneficiario { get; set; }
        public string PartnerId { get; set; }
        public string CoPartnerId { get; set; }
        public bool Sandbox { get; set; }
        public string Certificado { get; set; }
        public string CertificadoBase64 { get; set; }
        public string CertificadoSenha { get; set; }
        public string UrlAutenticacao { get; set; }
        public string UrlCobranca { get; set; }
        public string UrlPagamentos { get; set; }
        public string UrlPix { get; set; }
        public string UrlAutenticacaoPix { get; set; }

        public string ObterUrlAutenticacao() => string.IsNullOrWhiteSpace(UrlAutenticacao) ? (Sandbox ? "https://apidev.banrisul.com.br/auth/oauth/v2/token" : "https://api.banrisul.com.br/auth/oauth/v2/token") : UrlAutenticacao.TrimEnd('/');
        public string ObterUrlCobranca() => string.IsNullOrWhiteSpace(UrlCobranca) ? (Sandbox ? "https://apidev.banrisul.com.br/cobranca/v1" : "https://api.banrisul.com.br/cobranca/v1") : UrlCobranca.TrimEnd('/');
        public string ObterUrlPagamentos() => string.IsNullOrWhiteSpace(UrlPagamentos) ? (Sandbox ? "https://apidev.banrisul.com.br/pagamentos/v1" : "https://api.banrisul.com.br/pagamentos/v1") : UrlPagamentos.TrimEnd('/');
        public string ObterUrlPix() => string.IsNullOrWhiteSpace(UrlPix) ? (Sandbox ? "https://mtls-api-pix-homologacao.banrisul.com.br/pix/v2" : "https://mtls-api-pix.banrisul.com.br/pix/v2") : UrlPix.TrimEnd('/');
        public string ObterUrlAutenticacaoPix() => string.IsNullOrWhiteSpace(UrlAutenticacaoPix) ? (Sandbox ? "https://mtls-api-pix-homologacao.banrisul.com.br/auth/oauth/v2/token" : "https://mtls-api-pix.banrisul.com.br/auth/oauth/v2/token") : UrlAutenticacaoPix.TrimEnd('/');
    }

    public sealed class BanrisulIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly BanrisulConfiguracao configuracao;
        private readonly X509Certificate2 certificado;
        private string token;
        private DateTime tokenExpiraEm;
        private string pixToken;
        private DateTime pixTokenExpiraEm;

        public BanrisulIntegracao(BanrisulConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ClientId) || string.IsNullOrWhiteSpace(configuracao.ClientSecret)) throw new ArgumentException("ClientId e ClientSecret Banrisul são obrigatórios", nameof(configuracao));
            certificado = CarregarCertificado(configuracao);
        }

        public override int CodigoBanco => 41;
        public override string Nome => "Banrisul";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => requisicao?.Corpo == null ? Task.FromResult(BancoResposta.Falha("Dados do boleto não informados")) : EnviarNormalAsync(HttpMethod.Post, configuracao.ObterUrlCobranca() + "/boletos", requisicao.Corpo, null, requisicao.Cabecalhos);

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            var nossoNumero = parametros.Value<string>("nossoNumero");
            if (string.IsNullOrWhiteSpace(nossoNumero)) return Task.FromResult(BancoResposta.Falha("Informe nossoNumero para consultar boleto Banrisul"));
            return EnviarNormalAsync(HttpMethod.Get, configuracao.ObterUrlCobranca() + "/boletos/" + Uri.EscapeDataString(nossoNumero), null, null, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
        {
            if (requisicao?.Corpo == null) return Task.FromResult(BancoResposta.Falha("Dados da cobrança PIX não informados"));
            var txId = requisicao.Parametros?.Value<string>("txId") ?? requisicao.Parametros?.Value<string>("transactionId");
            var caminho = "/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId));
            return EnviarPixAsync(string.IsNullOrWhiteSpace(txId) ? HttpMethod.Post : HttpMethod.Put, caminho, requisicao.Corpo, null, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            var txId = parametros.Value<string>("txId") ?? parametros.Value<string>("transactionId");
            var e2eId = parametros.Value<string>("endToEndId") ?? parametros.Value<string>("e2eid");
            parametros.Remove("txId"); parametros.Remove("transactionId"); parametros.Remove("endToEndId"); parametros.Remove("e2eid");
            var caminho = !string.IsNullOrWhiteSpace(e2eId) ? "/pix/" + Uri.EscapeDataString(e2eId) : "/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId));
            return EnviarPixAsync(HttpMethod.Get, caminho, null, parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("Consulta de extrato não é disponibilizada pela integração Banrisul"));
        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("Consulta de saldo não é disponibilizada pela integração Banrisul"));

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao ?? (requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0 ? requisicao.BoletosPadrao[0] : null);
                if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload Banrisul não informado"));
                req.Corpo = CriarBoleto(boleto);
            }
            return EmitirBoletoAsync(req);
        }

        public async Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null) return BancoResposta.Falha("Requisição inválida");
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.NossoNumero)) parametros["nossoNumero"] = requisicao.NossoNumero;
            if (!requisicao.SegundaVia) return await ConsultarBoletoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao.Cabecalhos });
            var nossoNumero = parametros.Value<string>("nossoNumero");
            if (string.IsNullOrWhiteSpace(nossoNumero)) return BancoResposta.Falha("Informe nossoNumero para segunda via Banrisul");
            return await EnviarNormalAsync(HttpMethod.Get, configuracao.ObterUrlCobranca() + "/boletos/" + Uri.EscapeDataString(nossoNumero) + "/emitir", null, null, requisicao.Cabecalhos);
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                if (string.IsNullOrWhiteSpace(requisicao.Chave) || !requisicao.Valor.HasValue || requisicao.Valor.Value <= 0) return Task.FromResult(BancoResposta.Falha("Chave e valor são obrigatórios para criar PIX Banrisul"));
                req.Corpo = new JObject { ["calendario"] = new JObject { ["expiracao"] = requisicao.Expiracao ?? 3600 }, ["valor"] = new JObject { ["original"] = requisicao.Valor.Value.ToString("0.00", CultureInfo.InvariantCulture) }, ["chave"] = requisicao.Chave, ["solicitacaoPagador"] = requisicao.SolicitacaoPagador ?? "" };
                var documento = ApenasDigitos(requisicao.DocumentoDevedor);
                if (!string.IsNullOrWhiteSpace(documento) && !string.IsNullOrWhiteSpace(requisicao.NomeDevedor)) req.Corpo["devedor"] = new JObject { [documento.Length > 11 ? "cnpj" : "cpf"] = documento, ["nome"] = requisicao.NomeDevedor };
            }
            if (req.Parametros == null && !string.IsNullOrWhiteSpace(requisicao.TxId)) req.Parametros = new JObject { ["txId"] = requisicao.TxId };
            return CriarPixAsync(req);
        }

        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.TxId)) parametros["txId"] = requisicao.TxId;
            if (!string.IsNullOrWhiteSpace(requisicao.EndToEndId)) parametros["endToEndId"] = requisicao.EndToEndId;
            if (requisicao.Inicio.HasValue) parametros["inicio"] = requisicao.Inicio.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (requisicao.Fim.HasValue) parametros["fim"] = requisicao.Fim.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            return ConsultarPixAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao.Cabecalhos });
        }

        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao) => ConsultarExtratoAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        public Task<BancoResposta> ConsultarTituloAsync(JToken payload, JObject cabecalhos = null)
            => EnviarNormalAsync(HttpMethod.Get, configuracao.ObterUrlPagamentos() + "/titulos/" + Identificador(payload, "codigo_barra"), null, null, cabecalhos);
        public Task<BancoResposta> SolicitarPagamentoAsync(JToken payload, JObject cabecalhos = null)
            => EnviarNormalAsync(HttpMethod.Post, configuracao.ObterUrlPagamentos() + "/titulos/" + Identificador(payload, "codigo_barra"), payload, null, cabecalhos);
        public Task<BancoResposta> ConfirmarPagamentoAsync(JToken payload, JObject cabecalhos = null)
            => EnviarNormalAsync(new HttpMethod("PATCH"), configuracao.ObterUrlPagamentos() + "/titulos/" + Identificador(payload, "codigo_barra"), payload, null, cabecalhos);
        public Task<BancoResposta> ConsultarStatusPagamentoAsync(JToken payload, JObject cabecalhos = null)
            => EnviarNormalAsync(HttpMethod.Get, configuracao.ObterUrlPagamentos() + "/status/" + Identificador(payload, "id_documento"), null, null, cabecalhos);

        private JObject CriarBoleto(BoletoPadrao boleto)
        {
            var pagador = new JObject { ["nome"] = boleto.Pagador?.Nome, ["documento"] = ApenasDigitos(boleto.Pagador?.Documento) };
            Adicionar(pagador, "email", boleto.Pagador?.Email); Adicionar(pagador, "endereco", boleto.Pagador?.Endereco); Adicionar(pagador, "numero", boleto.Pagador?.Numero); Adicionar(pagador, "bairro", boleto.Pagador?.Bairro); Adicionar(pagador, "cidade", boleto.Pagador?.Cidade); Adicionar(pagador, "uf", boleto.Pagador?.Uf); Adicionar(pagador, "cep", ApenasDigitos(boleto.Pagador?.Cep));
            var corpo = new JObject { ["seuNumero"] = boleto.SeuNumero ?? boleto.NumeroDocumento, ["nossoNumero"] = boleto.NossoNumero, ["valor"] = boleto.Valor.ToString("0.00", CultureInfo.InvariantCulture), ["dataVencimento"] = boleto.Vencimento.ToString("yyyy-MM-dd"), ["pagador"] = pagador };
            if (boleto.Modalidade.HasValue) corpo["modalidade"] = boleto.Modalidade.Value;
            return corpo;
        }

        private async Task<BancoResposta> EnviarNormalAsync(HttpMethod metodo, string url, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            var autenticacao = await ObterTokenAsync(false); if (!autenticacao.Sucesso) return autenticacao;
            return await EnviarAsync(metodo, url + Query(parametros), corpo, cabecalhos, token, false);
        }

        private async Task<BancoResposta> EnviarPixAsync(HttpMethod metodo, string caminho, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            var autenticacao = await ObterTokenAsync(true); if (!autenticacao.Sucesso) return autenticacao;
            return await EnviarAsync(metodo, configuracao.ObterUrlPix() + caminho + Query(parametros), corpo, cabecalhos, pixToken, true);
        }

        private async Task<BancoResposta> ObterTokenAsync(bool pix)
        {
            var atual = pix ? pixToken : token; var expiraEm = pix ? pixTokenExpiraEm : tokenExpiraEm;
            if (!string.IsNullOrWhiteSpace(atual) && expiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
            using (var client = new HttpClient(pix ? CriarHandler() : new HttpClientHandler()))
            using (var request = new HttpRequestMessage(HttpMethod.Post, pix ? configuracao.ObterUrlAutenticacaoPix() : configuracao.ObterUrlAutenticacao()))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(configuracao.ClientId + ":" + configuracao.ClientSecret)));
                request.Content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });
                var resposta = await LerRespostaAsync(await client.SendAsync(request));
                if (!resposta.Sucesso) return resposta;
                var novoToken = resposta.Dados?.Value<string>("access_token"); var segundos = resposta.Dados?.Value<int?>("expires_in") ?? 0;
                if (string.IsNullOrWhiteSpace(novoToken) || segundos <= 0) return BancoResposta.Falha("Resposta de autenticação Banrisul inválida");
                if (pix) { pixToken = novoToken; pixTokenExpiraEm = DateTime.UtcNow.AddSeconds(segundos); } else { token = novoToken; tokenExpiraEm = DateTime.UtcNow.AddSeconds(segundos); }
                return BancoResposta.Ok(null);
            }
        }

        private async Task<BancoResposta> EnviarAsync(HttpMethod metodo, string url, JToken corpo, JObject cabecalhos, string accessToken, bool mtls)
        {
            try
            {
                using (var client = new HttpClient(mtls ? CriarHandler() : new HttpClientHandler()))
                using (var request = new HttpRequestMessage(metodo, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrWhiteSpace(configuracao.Beneficiario)) request.Headers.TryAddWithoutValidation("bergs-beneficiario", configuracao.Beneficiario);
                    if (!string.IsNullOrWhiteSpace(configuracao.PartnerId)) request.Headers.TryAddWithoutValidation("partner-id", configuracao.PartnerId);
                    if (!string.IsNullOrWhiteSpace(configuracao.CoPartnerId)) request.Headers.TryAddWithoutValidation("co-partner-id", configuracao.CoPartnerId);
                    AdicionarCabecalhos(request, cabecalhos);
                    if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (Exception ex) { return BancoResposta.Falha("Falha de comunicação com a API Banrisul: " + ex.Message); }
        }

        private HttpClientHandler CriarHandler()
        {
            if (certificado == null) throw new InvalidOperationException("Certificado PFX/P12 é obrigatório para PIX Banrisul via mTLS");
            var handler = new HttpClientHandler(); handler.ClientCertificates.Add(certificado); return handler;
        }
        private static X509Certificate2 CarregarCertificado(BanrisulConfiguracao c)
        {
            if (!string.IsNullOrWhiteSpace(c.CertificadoBase64)) return new X509Certificate2(Convert.FromBase64String(c.CertificadoBase64), c.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
            if (!string.IsNullOrWhiteSpace(c.Certificado) && File.Exists(c.Certificado)) return new X509Certificate2(c.Certificado, c.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
            return null;
        }
        private static async Task<BancoResposta> LerRespostaAsync(HttpResponseMessage resposta)
        {
            if (string.Equals(resposta.Content.Headers.ContentType?.MediaType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pdf = Convert.ToBase64String(await resposta.Content.ReadAsByteArrayAsync());
                return resposta.IsSuccessStatusCode ? BancoResposta.Ok(new JObject { ["pdfBoleto"] = pdf }) : BancoResposta.Falha("Erro retornado pela API Banrisul", (int)resposta.StatusCode);
            }
            var texto = await resposta.Content.ReadAsStringAsync(); JToken dados = null;
            if (!string.IsNullOrWhiteSpace(texto)) { try { dados = JToken.Parse(texto); } catch { dados = new JObject { ["conteudo"] = texto }; } }
            if (resposta.IsSuccessStatusCode) return BancoResposta.Ok(dados);
            return BancoResposta.Falha(dados?["message"]?.ToString() ?? dados?["mensagem"]?.ToString() ?? texto ?? "Erro retornado pela API Banrisul", (int)resposta.StatusCode);
        }
        private static string Query(JObject parametros)
        {
            if (parametros == null || parametros.Count == 0) return ""; var valores = new List<string>();
            foreach (var p in parametros) if (p.Value != null && p.Value.Type != JTokenType.Object && p.Value.Type != JTokenType.Array) valores.Add(Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value.ToString()));
            return valores.Count == 0 ? "" : "?" + string.Join("&", valores);
        }
        private static string Identificador(JToken payload, string nome)
        {
            var valor = payload?[nome]?.ToString();
            if (string.IsNullOrWhiteSpace(valor)) throw new ArgumentException(nome + " é obrigatório no payload JSON Banrisul");
            return Uri.EscapeDataString(valor);
        }
        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos) { if (cabecalhos != null) foreach (var h in cabecalhos) if (h.Value != null && !request.Headers.Contains(h.Key)) request.Headers.TryAddWithoutValidation(h.Key, h.Value.ToString()); }
        private static void Adicionar(JObject objeto, string nome, string valor) { if (!string.IsNullOrWhiteSpace(valor)) objeto[nome] = valor; }
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var resultado = new StringBuilder(); foreach (var c in valor) if (char.IsDigit(c)) resultado.Append(c); return resultado.Length == 0 ? null : resultado.ToString(); }
    }
}
