using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Santander
{
    public class SantanderConfiguracao
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string WorkspaceId { get; set; }
        public string CovenantCode { get; set; }
        public string BankNumber { get; set; }
        public string ParticipantCode { get; set; }
        public string PixChave { get; set; }
        public string PixChaveTipo { get; set; } = "EMAIL";
        public string CnpjBanco { get; set; }
        public string AgenciaConta { get; set; }
        public bool Sandbox { get; set; }
        public string BaseUrl { get; set; }
        public string UrlAutenticacao { get; set; }
        public string Certificado { get; set; }
        public string CertificadoBase64 { get; set; }
        public string CertificadoSenha { get; set; }
        public string ObterBaseUrl() => string.IsNullOrWhiteSpace(BaseUrl) ? (Sandbox ? "https://trust-sandbox.api.santander.com.br" : "https://trust-open.api.santander.com.br") : BaseUrl.TrimEnd('/');
        public string ObterUrlAutenticacao() => string.IsNullOrWhiteSpace(UrlAutenticacao) ? ObterBaseUrl() + "/auth/oauth/v2/token" : UrlAutenticacao;
    }

    public sealed class SantanderIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly SantanderConfiguracao configuracao;
        private readonly X509Certificate2 certificado;
        private string token;
        private DateTime tokenExpiraEm;
        public string JsonEnvio { get; private set; }

        public SantanderIntegracao(SantanderConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            certificado = CarregarCertificado(configuracao);
        }

        public override int CodigoBanco => 33;
        public override string Nome => "Santander";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarCobranca();
            return erro != null ? Task.FromResult(BancoResposta.Falha(erro)) : requisicao?.Corpo == null ? Task.FromResult(BancoResposta.Falha("Dados do boleto não informados")) : EnviarAsync(HttpMethod.Post, "/collection_bill_management/v2/workspaces/" + Uri.EscapeDataString(configuracao.WorkspaceId) + "/bank_slips", requisicao.Corpo, null, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarCobranca();
            if (erro != null) return Task.FromResult(BancoResposta.Falha(erro));
            var parametros = requisicao?.Parametros ?? new JObject();
            var id = parametros.Value<string>("id") ?? parametros.Value<string>("bankSlipId");
            if (!string.IsNullOrWhiteSpace(id)) return EnviarAsync(HttpMethod.Get, "/collection_bill_management/v2/workspaces/" + Uri.EscapeDataString(configuracao.WorkspaceId) + "/bank_slips/" + Uri.EscapeDataString(id), null, null, requisicao?.Cabecalhos);
            parametros["beneficiaryCode"] = configuracao.CovenantCode;
            var nossoNumero = parametros.Value<string>("nossoNumero") ?? parametros.Value<string>("bankNumber");
            if (!string.IsNullOrWhiteSpace(nossoNumero))
            {
                parametros.Remove("nossoNumero"); parametros.Remove("bankNumber"); parametros.Remove("beneficiaryCode");
                var tipoConsulta = parametros.Value<string>("tipoConsulta") ?? "default";
                parametros.Remove("tipoConsulta");
                return EnviarAsync(HttpMethod.Get, "/collection_bill_management/v2/bills/" + Uri.EscapeDataString(nossoNumero + "." + configuracao.CovenantCode), null, new JObject { ["tipoConsulta"] = tipoConsulta }, requisicao?.Cabecalhos);
            }
            return EnviarAsync(HttpMethod.Get, "/collection_bill_management/v2/bills", null, parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("A documentação Santander fornecida suporta PIX apenas vinculado ao boleto híbrido"));
        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("A documentação Santander fornecida não possui consulta PIX avulsa"));
        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConta();
            if (erro != null) return Task.FromResult(BancoResposta.Falha(erro));
            var parametros = requisicao?.Parametros ?? new JObject();
            var inicio = parametros.Value<DateTime?>("inicio"); var fim = parametros.Value<DateTime?>("fim");
            if (!inicio.HasValue || !fim.HasValue) return Task.FromResult(BancoResposta.Falha("Informe inicio e fim para consultar extrato Santander"));
            parametros.Remove("inicio"); parametros.Remove("fim");
            parametros["_offset"] = parametros.Value<int?>("_offset") ?? 1;
            parametros["_limit"] = parametros.Value<int?>("_limit") ?? 50;
            parametros["initialDate"] = inicio.Value.ToString("yyyy-MM-dd"); parametros["finalDate"] = fim.Value.ToString("yyyy-MM-dd");
            return EnviarContaAsync("/bank_account_information/v1/banks/" + Uri.EscapeDataString(configuracao.CnpjBanco) + "/statements/" + Uri.EscapeDataString(configuracao.AgenciaConta), parametros, requisicao?.Cabecalhos);
        }
        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConta();
            return erro != null ? Task.FromResult(BancoResposta.Falha(erro)) : EnviarContaAsync("/bank_account_information/v1/banks/" + Uri.EscapeDataString(configuracao.CnpjBanco) + "/balances/" + Uri.EscapeDataString(configuracao.AgenciaConta), null, requisicao?.Cabecalhos);
        }

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao ?? (requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0 ? requisicao.BoletosPadrao[0] : null);
                if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload Santander não informado"));
                req.Corpo = CriarBoleto(boleto);
            }
            return EmitirBoletoAsync(req);
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao?.Id)) parametros["id"] = requisicao.Id;
            else if (!string.IsNullOrWhiteSpace(requisicao?.NossoNumero)) parametros["bankNumber"] = requisicao.NossoNumero;
            return ConsultarBoletoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao?.Cabecalhos });
        }
        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao) => CriarPixAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao) => ConsultarPixAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            if (requisicao?.Inicio.HasValue == true) parametros["inicio"] = requisicao.Inicio;
            if (requisicao?.Fim.HasValue == true) parametros["fim"] = requisicao.Fim;
            return ConsultarExtratoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao?.Cabecalhos });
        }
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private JObject CriarBoleto(BoletoPadrao boleto)
        {
            var documento = ApenasDigitos(boleto.Pagador?.Documento);
            var corpo = new JObject
            {
                ["environment"] = configuracao.Sandbox ? "TESTE" : "PRODUCAO", ["nsuCode"] = boleto.SeuNumero ?? boleto.NumeroDocumento ?? Guid.NewGuid().ToString("N").Substring(0, 11), ["nsuDate"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["covenantCode"] = Numero(configuracao.CovenantCode), ["bankNumber"] = configuracao.BankNumber, ["clientNumber"] = boleto.SeuNumero ?? boleto.NumeroDocumento ?? "", ["dueDate"] = boleto.Vencimento.ToString("yyyy-MM-dd"), ["issueDate"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["participantCode"] = configuracao.ParticipantCode ?? "", ["nominalValue"] = boleto.Valor, ["documentKind"] = "DUPLICATA_MERCANTIL", ["paymentType"] = "REGISTRO", ["writeOffQuantityDays"] = "30",
                ["payer"] = new JObject { ["name"] = boleto.Pagador?.Nome ?? "", ["documentType"] = documento?.Length > 11 ? "CNPJ" : "CPF", ["documentNumber"] = documento ?? "", ["address"] = boleto.Pagador?.Endereco ?? "", ["neighborhood"] = boleto.Pagador?.Bairro ?? "", ["city"] = boleto.Pagador?.Cidade ?? "", ["state"] = boleto.Pagador?.Uf ?? "", ["zipCode"] = ApenasDigitos(boleto.Pagador?.Cep) ?? "" }
            };
            if (boleto.CadastrarPix == true && !string.IsNullOrWhiteSpace(configuracao.PixChave)) corpo["key"] = new JObject { ["type"] = configuracao.PixChaveTipo, ["dictKey"] = configuracao.PixChave };
            var mensagens = new JArray(); if (!string.IsNullOrWhiteSpace(boleto.Instrucao1)) mensagens.Add(boleto.Instrucao1); if (!string.IsNullOrWhiteSpace(boleto.Instrucao2)) mensagens.Add(boleto.Instrucao2); if (!string.IsNullOrWhiteSpace(boleto.Instrucao3)) mensagens.Add(boleto.Instrucao3); if (mensagens.Count > 0) corpo["messages"] = mensagens;
            return corpo;
        }

        private async Task<BancoResposta> EnviarAsync(HttpMethod metodo, string caminho, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenAsync();
                if (!autenticacao.Sucesso) return autenticacao;
                JsonEnvio = corpo?.ToString(Formatting.Indented) ?? "";
                using (var client = new HttpClient(CriarHandler()))
                using (var request = new HttpRequestMessage(metodo, configuracao.ObterBaseUrl() + caminho + CriarQuery(parametros)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Add("X-Application-Key", configuracao.ClientId);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (Exception ex) { return BancoResposta.Falha("Erro ao chamar a API Santander: " + ex.Message); }
        }

        private async Task<BancoResposta> EnviarContaAsync(string caminho, JObject parametros, JObject cabecalhos)
        {
            try
            {
                using (var client = new HttpClient(CriarHandler()))
                using (var request = new HttpRequestMessage(HttpMethod.Get, configuracao.ObterBaseUrl() + caminho + CriarQuery(parametros)))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (Exception ex) { return BancoResposta.Falha("Erro ao chamar a API de conta Santander: " + ex.Message); }
        }

        private async Task<BancoResposta> ObterTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(token) && tokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
            using (var client = new HttpClient(CriarHandler()))
            {
                var valores = new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("client_id", configuracao.ClientId), new KeyValuePair<string, string>("client_secret", configuracao.ClientSecret) };
                var resposta = await client.PostAsync(configuracao.ObterUrlAutenticacao(), new FormUrlEncodedContent(valores));
                var resultado = await LerRespostaAsync(resposta);
                if (!resultado.Sucesso) return resultado;
                token = resultado.Dados?.Value<string>("access_token");
                var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 300;
                if (string.IsNullOrWhiteSpace(token)) return BancoResposta.Falha("Resposta de autenticação Santander inválida");
                tokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                return BancoResposta.Ok(null);
            }
        }

        private static async Task<BancoResposta> LerRespostaAsync(HttpResponseMessage resposta)
        {
            var texto = await resposta.Content.ReadAsStringAsync(); JToken dados = null;
            if (!string.IsNullOrWhiteSpace(texto)) { try { dados = JToken.Parse(texto); } catch { dados = new JObject { ["conteudo"] = texto }; } }
            return resposta.IsSuccessStatusCode ? BancoResposta.Ok(dados) : new BancoResposta { Sucesso = false, Mensagem = dados?["message"]?.ToString() ?? texto, Dados = dados, StatusCode = (int)resposta.StatusCode };
        }

        private HttpClientHandler CriarHandler()
        {
            var handler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
            if (certificado != null) handler.ClientCertificates.Add(certificado);
            return handler;
        }

        private static X509Certificate2 CarregarCertificado(SantanderConfiguracao configuracao)
        {
            if (!string.IsNullOrWhiteSpace(configuracao.Certificado) && !string.IsNullOrWhiteSpace(configuracao.CertificadoBase64)) throw new ArgumentException("Informe o caminho do certificado ou o Base64 Santander, não ambos");
            if (!string.IsNullOrWhiteSpace(configuracao.CertificadoBase64))
            {
                var base64 = configuracao.CertificadoBase64.Trim();
                var separador = base64.IndexOf(',');
                if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && separador >= 0) base64 = base64.Substring(separador + 1);
                return new X509Certificate2(Convert.FromBase64String(base64), configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
            }
            return string.IsNullOrWhiteSpace(configuracao.Certificado) ? null : new X509Certificate2(configuracao.Certificado, configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
        }

        private string ValidarCobranca()
        {
            if (string.IsNullOrWhiteSpace(configuracao.ClientId) || string.IsNullOrWhiteSpace(configuracao.ClientSecret) || string.IsNullOrWhiteSpace(configuracao.WorkspaceId) || string.IsNullOrWhiteSpace(configuracao.CovenantCode) || string.IsNullOrWhiteSpace(configuracao.BankNumber)) return "Credenciais, WorkspaceId, CovenantCode e BankNumber Santander são obrigatórios para cobrança";
            return null;
        }
        private string ValidarConta()
        {
            if (string.IsNullOrWhiteSpace(configuracao.CnpjBanco) || string.IsNullOrWhiteSpace(configuracao.AgenciaConta)) return "SantanderCnpjBanco e SantanderAgenciaConta são obrigatórios para consulta de conta";
            return null;
        }
        private static JToken Numero(string valor) { return long.TryParse(valor, out var numero) ? new JValue(numero) : new JValue(valor ?? ""); }
        private static string CriarQuery(JObject parametros) { if (parametros == null || !parametros.HasValues) return ""; var valores = new List<string>(); foreach (var item in parametros) if (item.Value != null && item.Value.Type != JTokenType.Object && item.Value.Type != JTokenType.Array) valores.Add(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value.ToString())); return valores.Count == 0 ? "" : "?" + string.Join("&", valores); }
        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos) { if (cabecalhos != null) foreach (var item in cabecalhos) if (item.Value != null && !request.Headers.Contains(item.Key)) request.Headers.TryAddWithoutValidation(item.Key, item.Value.ToString()); }
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var saida = new StringBuilder(); foreach (var caractere in valor) if (char.IsDigit(caractere)) saida.Append(caractere); return saida.Length == 0 ? null : saida.ToString(); }
    }
}
