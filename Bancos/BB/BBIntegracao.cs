using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.BB
{
    public class BBConfiguracao
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AppKey { get; set; }
        public int NumeroConvenio { get; set; }
        public bool Sandbox { get; set; }
        public string Certificado { get; set; }
        public string CertificadoBase64 { get; set; }
        public string CertificadoSenha { get; set; }
        public string BaseUrl { get; set; }
        public string UrlAutenticacao { get; set; }

        public string ObterBaseUrl()
        {
            if (!string.IsNullOrWhiteSpace(BaseUrl)) return BaseUrl.TrimEnd('/');
            return Sandbox ? "https://api.sandbox.bb.com.br/cobrancas/v2" : "https://api.bb.com.br/cobrancas/v2";
        }

        public string ObterUrlAutenticacao()
        {
            if (!string.IsNullOrWhiteSpace(UrlAutenticacao)) return UrlAutenticacao;
            return Sandbox ? "https://oauth.sandbox.bb.com.br/oauth/token" : "https://oauth.bb.com.br/oauth/token";
        }
    }

    public sealed class BBIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly BBConfiguracao configuracao;
        private readonly X509Certificate2 certificado;
        private string token;
        private DateTime tokenExpiraEm;

        public BBIntegracao(BBConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ClientId)) throw new ArgumentException("O ClientId Banco do Brasil é obrigatório", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ClientSecret)) throw new ArgumentException("O ClientSecret Banco do Brasil é obrigatório", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.AppKey)) throw new ArgumentException("A AppKey Banco do Brasil é obrigatória", nameof(configuracao));
            if (configuracao.NumeroConvenio <= 0) throw new ArgumentException("O número do convênio Banco do Brasil é obrigatório", nameof(configuracao));
            certificado = CarregarCertificado(configuracao);
        }

        public override int CodigoBanco => 1;
        public override string Nome => "Banco do Brasil";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => requisicao?.Corpo == null ? Task.FromResult(BancoResposta.Falha("Dados do boleto não informados")) : EnviarAsync(HttpMethod.Post, "/boletos", requisicao.Corpo, null, "cobrancas.boletos-requisicao", requisicao.Cabecalhos);

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            var id = parametros.Value<string>("id") ?? parametros.Value<string>("numeroTituloCliente") ?? parametros.Value<string>("nossoNumero");
            if (string.IsNullOrWhiteSpace(id)) return Task.FromResult(BancoResposta.Falha("Informe id ou numeroTituloCliente para consultar boleto Banco do Brasil"));
            parametros["numeroConvenio"] = parametros.Value<int?>("numeroConvenio") ?? configuracao.NumeroConvenio;
            parametros.Remove("id"); parametros.Remove("numeroTituloCliente"); parametros.Remove("nossoNumero");
            return EnviarAsync(HttpMethod.Get, "/boletos/" + Uri.EscapeDataString(id), null, parametros, "cobrancas.boletos-info", requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
        {
            var id = requisicao?.Parametros?.Value<string>("id") ?? requisicao?.Parametros?.Value<string>("numeroTituloCliente");
            if (string.IsNullOrWhiteSpace(id)) return Task.FromResult(BancoResposta.Falha("Informe id ou numeroTituloCliente para gerar PIX do boleto Banco do Brasil"));
            return EnviarAsync(HttpMethod.Post, "/boletos/" + Uri.EscapeDataString(id) + "/gerar-pix", requisicao.Corpo, null, "cobrancas.boletos-requisicao", requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
        {
            var id = requisicao?.Parametros?.Value<string>("id") ?? requisicao?.Parametros?.Value<string>("numeroTituloCliente");
            if (string.IsNullOrWhiteSpace(id)) return Task.FromResult(BancoResposta.Falha("Informe id ou numeroTituloCliente para consultar PIX do boleto Banco do Brasil"));
            return EnviarAsync(HttpMethod.Get, "/boletos/" + Uri.EscapeDataString(id) + "/pix", null, null, "cobrancas.boletos-info", requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("Consulta de extrato não é disponibilizada pela API de Cobranças Banco do Brasil"));
        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao) => Task.FromResult(BancoResposta.Falha("Consulta de saldo não é disponibilizada pela API de Cobranças Banco do Brasil"));

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao;
                if (boleto == null && requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0) boleto = requisicao.BoletosPadrao[0];
                if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload Banco do Brasil não informado"));
                req.Corpo = CriarBoleto(boleto);
            }
            return EmitirBoletoAsync(req);
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.Id)) parametros["id"] = requisicao.Id;
            else if (!string.IsNullOrWhiteSpace(requisicao.NossoNumero)) parametros["nossoNumero"] = requisicao.NossoNumero;
            return ConsultarBoletoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao.Cabecalhos });
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao) => CriarPixAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao) => ConsultarPixAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao) => ConsultarExtratoAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private JObject CriarBoleto(BoletoPadrao boleto)
        {
            var documento = ApenasDigitos(boleto.Pagador?.Documento);
            var pagador = new JObject
            {
                ["tipoInscricao"] = documento != null && documento.Length > 11 ? 2 : 1,
                ["numeroInscricao"] = documento
            };
            Adicionar(pagador, "nome", boleto.Pagador?.Nome); Adicionar(pagador, "endereco", JuntarEndereco(boleto.Pagador));
            Adicionar(pagador, "bairro", boleto.Pagador?.Bairro); Adicionar(pagador, "cidade", boleto.Pagador?.Cidade);
            Adicionar(pagador, "uf", boleto.Pagador?.Uf); Adicionar(pagador, "telefone", ApenasDigitos(boleto.Pagador?.Telefone)); Adicionar(pagador, "email", boleto.Pagador?.Email);
            if (int.TryParse(ApenasDigitos(boleto.Pagador?.Cep), out var cep)) pagador["cep"] = cep;
            var corpo = new JObject
            {
                ["numeroConvenio"] = configuracao.NumeroConvenio,
                ["codigoModalidade"] = boleto.Modalidade ?? 1,
                ["dataEmissao"] = DateTime.Today.ToString("dd.MM.yyyy"),
                ["dataVencimento"] = boleto.Vencimento.ToString("dd.MM.yyyy"),
                ["valorOriginal"] = boleto.Valor,
                ["codigoAceite"] = "N",
                ["codigoTipoTitulo"] = 2,
                ["numeroTituloBeneficiario"] = boleto.SeuNumero ?? boleto.NumeroDocumento,
                ["indicadorPix"] = boleto.CadastrarPix == true ? "S" : "N",
                ["pagador"] = pagador
            };
            Adicionar(corpo, "numeroTituloCliente", boleto.NossoNumero);
            var mensagem = string.Join(" ", new[] { boleto.Instrucao1, boleto.Instrucao2, boleto.Instrucao3 }).Trim();
            Adicionar(corpo, "mensagemBloquetoOcorrencia", mensagem);
            return corpo;
        }

        private async Task<BancoResposta> EnviarAsync(HttpMethod metodo, string caminho, JToken corpo, JObject parametros, string escopo, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenAsync(escopo);
                if (!autenticacao.Sucesso) return autenticacao;
                using (var handler = CriarHandler())
                using (var client = new HttpClient(handler))
                using (var request = new HttpRequestMessage(metodo, configuracao.ObterBaseUrl() + caminho + CriarQuery(parametros, configuracao.AppKey)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (TaskCanceledException ex) { return FalhaComunicacao("Tempo limite excedido ao chamar a API Banco do Brasil", ex); }
            catch (HttpRequestException ex) { return FalhaComunicacao("Falha de comunicação com a API Banco do Brasil", ex); }
            catch (Exception ex) { return FalhaComunicacao("Erro ao chamar a API Banco do Brasil", ex); }
        }

        private async Task<BancoResposta> ObterTokenAsync(string escopo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token) && tokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
                using (var handler = CriarHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(configuracao.ClientId + ":" + configuracao.ClientSecret)));
                    var corpo = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("scope", escopo) });
                    var resposta = await client.PostAsync(configuracao.ObterUrlAutenticacao(), corpo);
                    var resultado = await LerRespostaAsync(resposta);
                    if (!resultado.Sucesso) return resultado;
                    token = resultado.Dados?.Value<string>("access_token");
                    var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 0;
                    if (string.IsNullOrWhiteSpace(token) || expiraEm <= 0) return BancoResposta.Falha("Resposta de autenticação Banco do Brasil inválida");
                    tokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                    return BancoResposta.Ok(null);
                }
            }
            catch (Exception ex) { return FalhaComunicacao("Erro ao obter o token Banco do Brasil", ex); }
        }

        private HttpClientHandler CriarHandler()
        {
            var handler = new HttpClientHandler();
            if (certificado != null)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(certificado);
            }
            return handler;
        }

        private static X509Certificate2 CarregarCertificado(BBConfiguracao configuracao)
        {
            if (!string.IsNullOrWhiteSpace(configuracao.Certificado) && !string.IsNullOrWhiteSpace(configuracao.CertificadoBase64)) throw new ArgumentException("Informe o caminho do certificado ou o Base64, não ambos");
            if (!string.IsNullOrWhiteSpace(configuracao.CertificadoBase64))
            {
                var base64 = configuracao.CertificadoBase64.Trim();
                var separador = base64.IndexOf(',');
                if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && separador >= 0) base64 = base64.Substring(separador + 1);
                return new X509Certificate2(Convert.FromBase64String(base64), configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
            }
            return string.IsNullOrWhiteSpace(configuracao.Certificado) ? null : new X509Certificate2(configuracao.Certificado, configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
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
            return new BancoResposta { Sucesso = false, Mensagem = dados?["errors"]?.ToString(Formatting.None) ?? dados?["message"]?.ToString() ?? texto ?? "Erro retornado pela API Banco do Brasil", Dados = dados, StatusCode = (int)response.StatusCode };
        }

        private static BancoResposta FalhaComunicacao(string contexto, Exception ex)
        {
            var mensagens = new List<string> { contexto };
            while (ex != null) { if (!string.IsNullOrWhiteSpace(ex.Message) && !mensagens.Contains(ex.Message)) mensagens.Add(ex.Message); ex = ex.InnerException; }
            return BancoResposta.Falha(string.Join(" | ", mensagens));
        }

        private static string CriarQuery(JObject parametros, string appKey)
        {
            var valores = new List<string> { "gw-dev-app-key=" + Uri.EscapeDataString(appKey) };
            if (parametros != null) foreach (var parametro in parametros) if (parametro.Value != null && parametro.Value.Type != JTokenType.Object && parametro.Value.Type != JTokenType.Array) valores.Add(Uri.EscapeDataString(parametro.Key) + "=" + Uri.EscapeDataString(parametro.Value.ToString()));
            return "?" + string.Join("&", valores);
        }

        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos)
        {
            if (cabecalhos == null) return;
            foreach (var cabecalho in cabecalhos) if (cabecalho.Value != null && !request.Headers.Contains(cabecalho.Key)) request.Headers.TryAddWithoutValidation(cabecalho.Key, cabecalho.Value.ToString());
        }

        private static void Adicionar(JObject objeto, string nome, string valor) { if (!string.IsNullOrWhiteSpace(valor)) objeto[nome] = valor; }
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var resultado = new StringBuilder(); foreach (var c in valor) if (char.IsDigit(c)) resultado.Append(c); return resultado.Length == 0 ? null : resultado.ToString(); }
        private static string JuntarEndereco(PagadorPadrao pagador) { if (pagador == null) return null; if (string.IsNullOrWhiteSpace(pagador.Endereco)) return pagador.Numero; if (string.IsNullOrWhiteSpace(pagador.Numero)) return pagador.Endereco; return pagador.Endereco + ", " + pagador.Numero; }
    }
}
