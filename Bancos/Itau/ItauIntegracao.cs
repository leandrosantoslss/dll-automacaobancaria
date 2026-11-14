using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Itau
{
    public class ItauConfiguracao
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
        public string IdBeneficiario { get; set; }
        public string CodigoCarteira { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Dac { get; set; }
        public string FrancesaId { get; set; }
        public string PixChave { get; set; }
        public bool Sandbox { get; set; }
        public string UrlAutenticacao { get; set; }
        public string BaseUrlPix { get; set; }
        public string BaseUrlRecebimentos { get; set; }
        public string BaseUrlExtrato { get; set; }
        public string BaseUrlBoletosCash { get; set; }

        public string ObterUrlAutenticacao() => !string.IsNullOrWhiteSpace(UrlAutenticacao) ? UrlAutenticacao : Sandbox ? "https://sandbox.devportal.itau.com.br/api/oauth/jwt" : null;
        public string ObterBaseBoletosCash() => string.IsNullOrWhiteSpace(BaseUrlBoletosCash) ? "https://secure.gateway.api.itau.com.br/boletoscash/v2" : BaseUrlBoletosCash.TrimEnd('/');
    }

    public sealed class ItauIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly ItauConfiguracao configuracao;
        private string token;
        private DateTime tokenExpiraEm;
        public string JsonEnvio { get; private set; }

        public ItauIntegracao(ItauConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ClientId) || string.IsNullOrWhiteSpace(configuracao.ClientSecret) || string.IsNullOrWhiteSpace(configuracao.ApiKey)) throw new ArgumentException("ClientId, ClientSecret e ApiKey Itaú são obrigatórios", nameof(configuracao));
        }

        public override int CodigoBanco => 341;
        public override string Nome => "Itaú";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Extrato | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
        {
            var baseUrl = ObterUrl(configuracao.BaseUrlRecebimentos, "BaseUrlRecebimentos");
            return baseUrl == null ? Task.FromResult(BancoResposta.Falha("Informe ItauBaseUrlRecebimentos para emitir boleto PIX")) : requisicao?.Corpo == null ? Task.FromResult(BancoResposta.Falha("Dados do boleto não informados")) : EnviarAsync(HttpMethod.Post, baseUrl + "/boletos-pix", requisicao.Corpo, null, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
        {
            if (string.IsNullOrWhiteSpace(configuracao.IdBeneficiario)) return Task.FromResult(BancoResposta.Falha("ItauIdBeneficiario é obrigatório para consultar boleto"));
            var parametros = requisicao?.Parametros ?? new JObject();
            parametros["id_beneficiario"] = configuracao.IdBeneficiario;
            if (!string.IsNullOrWhiteSpace(configuracao.CodigoCarteira)) parametros["codigo_carteira"] = configuracao.CodigoCarteira;
            return EnviarAsync(HttpMethod.Get, configuracao.ObterBaseBoletosCash() + "/boletos", null, parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
        {
            var baseUrl = ObterUrl(configuracao.BaseUrlPix, "BaseUrlPix");
            if (baseUrl == null) return Task.FromResult(BancoResposta.Falha("Informe ItauBaseUrlPix para criar PIX"));
            if (requisicao?.Corpo == null) return Task.FromResult(BancoResposta.Falha("Dados da cobrança PIX não informados"));
            var txId = requisicao.Parametros?.Value<string>("txId") ?? requisicao.Parametros?.Value<string>("transactionId");
            return EnviarAsync(string.IsNullOrWhiteSpace(txId) ? HttpMethod.Post : HttpMethod.Put, baseUrl + "/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId)), requisicao.Corpo, null, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
        {
            var baseUrl = ObterUrl(configuracao.BaseUrlPix, "BaseUrlPix");
            if (baseUrl == null) return Task.FromResult(BancoResposta.Falha("Informe ItauBaseUrlPix para consultar PIX"));
            var parametros = requisicao?.Parametros ?? new JObject();
            var txId = parametros.Value<string>("txId") ?? parametros.Value<string>("transactionId");
            var endToEndId = parametros.Value<string>("endToEndId");
            parametros.Remove("txId"); parametros.Remove("transactionId"); parametros.Remove("endToEndId");
            var caminho = !string.IsNullOrWhiteSpace(endToEndId) ? "/pix/" + Uri.EscapeDataString(endToEndId) : "/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId));
            return EnviarAsync(HttpMethod.Get, baseUrl + caminho, null, parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
        {
            var baseUrl = ObterUrl(configuracao.BaseUrlExtrato, "BaseUrlExtrato");
            if (baseUrl == null) return Task.FromResult(BancoResposta.Falha("Informe ItauBaseUrlExtrato para consultar extrato"));
            if (string.IsNullOrWhiteSpace(configuracao.FrancesaId)) return Task.FromResult(BancoResposta.Falha("ItauFrancesaId é obrigatório para consultar extrato"));
            var parametros = requisicao?.Parametros ?? new JObject();
            var inicio = parametros.Value<DateTime?>("inicio");
            if (inicio.HasValue) parametros["data"] = inicio.Value.ToString("yyyy-MM-dd");
            parametros.Remove("inicio"); parametros.Remove("fim");
            return EnviarAsync(HttpMethod.Get, baseUrl + "/francesas/" + Uri.EscapeDataString(configuracao.FrancesaId) + "/movimentacoes", null, parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("A documentação Itaú fornecida não possui endpoint de saldo"));

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao ?? (requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0 ? requisicao.BoletosPadrao[0] : null);
                if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload Itaú não informado"));
                req.Corpo = CriarBoletoPix(boleto);
            }
            return EmitirBoletoAsync(req);
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao?.Id)) parametros["nosso_numero"] = requisicao.Id;
            else if (!string.IsNullOrWhiteSpace(requisicao?.NossoNumero)) parametros["nosso_numero"] = requisicao.NossoNumero;
            return ConsultarBoletoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao?.Cabecalhos });
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
        {
            if (requisicao == null || string.IsNullOrWhiteSpace(requisicao.Chave) || !requisicao.Valor.HasValue || requisicao.Valor <= 0) return Task.FromResult(BancoResposta.Falha("Chave e valor são obrigatórios para criar PIX Itaú"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null) req.Corpo = CriarPix(requisicao);
            if (req.Parametros == null && !string.IsNullOrWhiteSpace(requisicao.TxId)) req.Parametros = new JObject { ["txId"] = requisicao.TxId };
            return CriarPixAsync(req);
        }

        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao?.TxId)) parametros["txId"] = requisicao.TxId;
            if (!string.IsNullOrWhiteSpace(requisicao?.EndToEndId)) parametros["endToEndId"] = requisicao.EndToEndId;
            if (requisicao?.Inicio.HasValue == true) parametros["inicio"] = requisicao.Inicio.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (requisicao?.Fim.HasValue == true) parametros["fim"] = requisicao.Fim.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            return ConsultarPixAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao?.Cabecalhos });
        }
        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            if (requisicao?.Inicio.HasValue == true) parametros["inicio"] = requisicao.Inicio;
            if (requisicao?.Fim.HasValue == true) parametros["fim"] = requisicao.Fim;
            return ConsultarExtratoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao?.Cabecalhos });
        }
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private JObject CriarPix(PixCriacaoRequest requisicao)
        {
            var documento = ApenasDigitos(requisicao.DocumentoDevedor);
            var corpo = new JObject
            {
                ["valor"] = new JObject { ["original"] = requisicao.Valor.Value.ToString("0.00", CultureInfo.InvariantCulture), ["modalidadeAlteracao"] = 0 },
                ["chave"] = requisicao.Chave,
                ["calendario"] = new JObject { ["expiracao"] = requisicao.Expiracao ?? 3600 },
                ["solicitacaoPagador"] = requisicao.SolicitacaoPagador ?? ""
            };
            if (!string.IsNullOrWhiteSpace(requisicao.NomeDevedor) && !string.IsNullOrWhiteSpace(documento)) corpo["devedor"] = new JObject { [documento.Length > 11 ? "cnpj" : "cpf"] = documento, ["nome"] = requisicao.NomeDevedor };
            return corpo;
        }

        private JObject CriarBoletoPix(BoletoPadrao boleto)
        {
            if (string.IsNullOrWhiteSpace(configuracao.IdBeneficiario) || string.IsNullOrWhiteSpace(configuracao.CodigoCarteira)) throw new ArgumentException("ItauIdBeneficiario e ItauCodigoCarteira são obrigatórios para boleto PIX");
            var pagador = new JObject { ["pessoa"] = new JObject { ["nome_pessoa"] = boleto.Pagador?.Nome ?? "" } };
            if (boleto.Pagador != null) pagador["endereco"] = new JObject { ["nome_logradouro"] = boleto.Pagador.Endereco ?? "", ["nome_cidade"] = boleto.Pagador.Cidade ?? "", ["sigla_UF"] = boleto.Pagador.Uf ?? "", ["numero_CEP"] = ApenasDigitos(boleto.Pagador.Cep) ?? "" };
            return new JObject
            {
                ["etapa_processo_boleto"] = "CRIACAO",
                ["dado_boleto"] = new JObject
                {
                    ["dados_individuais_boleto"] = new JArray(new JObject { ["data_vencimento"] = boleto.Vencimento.ToString("yyyy-MM-dd"), ["valor_titulo"] = boleto.Valor.ToString("0.00", CultureInfo.InvariantCulture), ["numero_nosso_numero"] = boleto.NossoNumero ?? "", ["data_limite_pagamento"] = boleto.Vencimento.ToString("yyyy-MM-dd"), ["texto_seu_numero"] = boleto.SeuNumero ?? boleto.NumeroDocumento ?? "" }),
                    ["tipo_boleto"] = "HIBRIDO", ["pagador"] = pagador, ["codigo_carteira"] = configuracao.CodigoCarteira, ["codigo_especie"] = "99", ["data_emissao"] = DateTime.Today.ToString("yyyy-MM-dd")
                },
                ["beneficiario"] = new JObject { ["id_beneficiario"] = configuracao.IdBeneficiario },
                ["codigo_canal_operacao"] = "API",
                ["dados_qrcode"] = new JObject { ["chave"] = configuracao.PixChave ?? "", ["tipo_cobranca"] = "COBV" }
            };
        }

        private async Task<BancoResposta> EnviarAsync(HttpMethod metodo, string url, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenAsync();
                if (!autenticacao.Sucesso) return autenticacao;
                JsonEnvio = corpo?.ToString(Formatting.Indented) ?? "";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(metodo, url + CriarQuery(parametros)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Add("x-itau-apikey", configuracao.ApiKey);
                    request.Headers.Add("x-itau-correlationID", Guid.NewGuid().ToString());
                    AdicionarCabecalhos(request, cabecalhos);
                    if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (Exception ex) { return BancoResposta.Falha("Erro ao chamar a API Itaú: " + ex.Message); }
        }

        private async Task<BancoResposta> ObterTokenAsync()
        {
            var url = configuracao.ObterUrlAutenticacao();
            if (string.IsNullOrWhiteSpace(url)) return BancoResposta.Falha("Informe ItauUrlAutenticacao para produção Itaú");
            if (!string.IsNullOrWhiteSpace(token) && tokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
            using (var client = new HttpClient())
            {
                var valores = new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("client_id", configuracao.ClientId), new KeyValuePair<string, string>("client_secret", configuracao.ClientSecret) };
                var resposta = await client.PostAsync(url, new FormUrlEncodedContent(valores));
                var resultado = await LerRespostaAsync(resposta);
                if (!resultado.Sucesso) return resultado;
                token = resultado.Dados?.Value<string>("access_token");
                var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 300;
                if (string.IsNullOrWhiteSpace(token)) return BancoResposta.Falha("Resposta de autenticação Itaú inválida");
                tokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                return BancoResposta.Ok(null);
            }
        }

        private static async Task<BancoResposta> LerRespostaAsync(HttpResponseMessage resposta)
        {
            var texto = await resposta.Content.ReadAsStringAsync();
            JToken dados = null;
            if (!string.IsNullOrWhiteSpace(texto)) { try { dados = JToken.Parse(texto); } catch { dados = new JObject { ["conteudo"] = texto }; } }
            return resposta.IsSuccessStatusCode ? BancoResposta.Ok(dados) : new BancoResposta { Sucesso = false, Mensagem = dados?["message"]?.ToString() ?? texto, Dados = dados, StatusCode = (int)resposta.StatusCode };
        }

        private static string ObterUrl(string url, string nome) => string.IsNullOrWhiteSpace(url) ? null : url.TrimEnd('/');
        private static string CriarQuery(JObject parametros) { if (parametros == null || !parametros.HasValues) return ""; var valores = new List<string>(); foreach (var item in parametros) if (item.Value != null && item.Value.Type != JTokenType.Object && item.Value.Type != JTokenType.Array) valores.Add(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value.ToString())); return valores.Count == 0 ? "" : "?" + string.Join("&", valores); }
        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos) { if (cabecalhos != null) foreach (var item in cabecalhos) if (item.Value != null && !request.Headers.Contains(item.Key)) request.Headers.TryAddWithoutValidation(item.Key, item.Value.ToString()); }
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var saida = new StringBuilder(); foreach (var caractere in valor) if (char.IsDigit(caractere)) saida.Append(caractere); return saida.Length == 0 ? null : saida.ToString(); }
    }
}
