using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutomacaoBancaria.Bancos.Credisis
{
    public class CredisisConfiguracao
    {
        public string Token { get; set; }
        public string Convenio { get; set; }
        public string Url { get; set; }

        public string ObterUrl()
            => string.IsNullOrWhiteSpace(Url) ? "https://credisiscobranca.com.br/v2/ws" : Url;
    }

    public sealed class CredisisIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly CredisisConfiguracao configuracao;
        public string JsonEnvio { get; private set; }

        public CredisisIntegracao(CredisisConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.Token)) throw new ArgumentException("O token CrediSIS é obrigatório", nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.Convenio)) throw new ArgumentException("O convênio CrediSIS é obrigatório", nameof(configuracao));
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public override int CodigoBanco => 97;
        public override string Nome => "CrediSIS";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => ExecutarAsync(string.IsNullOrWhiteSpace(requisicao?.Operacao) ? "gerarBoleto" : requisicao.Operacao, CorpoOuVazio(requisicao?.Corpo), requisicao?.Cabecalhos);

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
            => ConsultarBoletoInternoAsync(requisicao);

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("O PIX CrediSIS é gerado junto ao boleto híbrido"));

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
            => ConsultarBoletoInternoAsync(requisicao);

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de extrato não é disponibilizada pelo Web Service CrediSIS Cobrança"));

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de saldo não é disponibilizada pelo Web Service CrediSIS Cobrança"));

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            if (requisicao.Corpo != null) return EmitirBoletoAsync(requisicao.ToRequisicao());
            var boleto = requisicao.BoletoPadrao;
            if (boleto == null && requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0) boleto = requisicao.BoletosPadrao[0];
            if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload CrediSIS não informado"));
            var boletos = requisicao.BoletosPadrao;
            if (boletos != null && boletos.Length > 0 && (boletos.Length > 1 || PossuiNossoNumero(boletos))) return ExecutarAsync("gerarBoletos", CriarBoletosXml(boletos), requisicao.Cabecalhos);
            if (!string.IsNullOrWhiteSpace(boleto.NossoNumero)) return ExecutarAsync("gerarBoletos", CriarBoletosXml(new[] { boleto }), requisicao.Cabecalhos);
            return ExecutarAsync("gerarBoleto", CriarBoletoXml(boleto, false), requisicao.Cabecalhos);
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var id = requisicao.Id;
            var nossoNumero = requisicao.NossoNumero;
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(nossoNumero)) return Task.FromResult(BancoResposta.Falha("Informe Id ou NossoNumero para consultar CrediSIS"));
            var corpo = "<boleto>" + Elemento("idWeb", id) + Elemento("nossonumero", nossoNumero) + "</boleto>";
            return ExecutarAsync("buscarBoleto", corpo, requisicao.Cabecalhos);
        }

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao) => Task.FromResult(BancoResposta.Falha("O PIX CrediSIS é gerado junto ao boleto híbrido"));
        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao) => ConsultarBoletoAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao) => ConsultarExtratoAsync(requisicao?.ToRequisicao());
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private Task<BancoResposta> ConsultarBoletoInternoAsync(BancoRequisicao requisicao)
        {
            var parametros = requisicao?.Parametros;
            var id = parametros?.Value<string>("idWeb") ?? parametros?.Value<string>("id");
            var nossoNumero = parametros?.Value<string>("nossoNumero") ?? parametros?.Value<string>("nossonumero");
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(nossoNumero)) return Task.FromResult(BancoResposta.Falha("Informe idWeb ou nossoNumero para consultar CrediSIS"));
            var corpo = "<boleto>" + Elemento("idWeb", id) + Elemento("nossonumero", nossoNumero) + "</boleto>";
            return ExecutarAsync(string.IsNullOrWhiteSpace(requisicao?.Operacao) ? "buscarBoleto" : requisicao.Operacao, corpo, requisicao?.Cabecalhos);
        }

        private string CriarBoletosXml(BoletoPadrao[] boletos)
        {
            var itens = new StringBuilder();
            foreach (var boleto in boletos) itens.Append(CriarBoletoXml(boleto, true));
            return "<layout>default</layout><boletos>" + itens + "</boletos>";
        }

        private string CriarBoletoXml(BoletoPadrao boleto, bool incluirNossoNumero)
        {
            var pagador = boleto.Pagador;
            var xml = "<boleto>" + Pessoa("pagador", pagador) +
                Elemento("documento", boleto.NumeroDocumento ?? boleto.SeuNumero) +
                Elemento("dataEmissao", DateTime.Today.ToString("yyyy-MM-dd")) +
                Elemento("dataVencimento", boleto.Vencimento.ToString("yyyy-MM-dd")) +
                Elemento("dataLimitePagamento", (boleto.DataLimitePagamento ?? boleto.Vencimento).ToString("yyyy-MM-dd")) +
                Elemento("valor", boleto.Valor.ToString(System.Globalization.CultureInfo.InvariantCulture)) +
                (incluirNossoNumero ? Elemento("nossonumero", boleto.NossoNumero) : string.Empty) +
                Elemento("codigoEspecie", "03") +
                "<protesto><tipo>1</tipo><dias>0</dias></protesto>" + Elemento("instrucao", JuntarInstrucoes(boleto)) + "</boleto>";
            return xml;
        }

        private static bool PossuiNossoNumero(BoletoPadrao[] boletos)
        {
            foreach (var boleto in boletos) if (boleto != null && !string.IsNullOrWhiteSpace(boleto.NossoNumero)) return true;
            return false;
        }

        private async Task<BancoResposta> ExecutarAsync(string operacao, string corpo, JObject cabecalhos)
        {
            if (string.IsNullOrWhiteSpace(operacao)) return BancoResposta.Falha("Operação CrediSIS não informada");
            JsonEnvio = corpo ?? string.Empty;
            var envelope = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:CredisisBoletoInterface\" xmlns:urn1=\"urn:CredisisBoletoInterface-CredisisWebService\">" +
                "<soapenv:Header><urn:Chave><token>" + Escapar(configuracao.Token) + "</token><convenio>" + Escapar(configuracao.Convenio) + "</convenio></urn:Chave></soapenv:Header>" +
                "<soapenv:Body><urn1:" + Escapar(operacao) + ">" + corpo + "</urn1:" + Escapar(operacao) + "></soapenv:Body></soapenv:Envelope>";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, configuracao.ObterUrl()))
            {
                request.Headers.TryAddWithoutValidation("SOAPAction", "\"" + operacao + "\"");
                if (cabecalhos != null)
                    foreach (var item in cabecalhos) if (item.Value != null && !request.Headers.Contains(item.Key)) request.Headers.TryAddWithoutValidation(item.Key, item.Value.ToString());
                request.Content = new StringContent(envelope, Encoding.UTF8, "text/xml");
                var response = await client.SendAsync(request);
                var xml = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) return BancoResposta.Falha(xml, (int)response.StatusCode);
                try
                {
                    var dados = JToken.Parse(JsonConvert.SerializeXNode(XDocument.Parse(xml)));
                    var erros = new System.Collections.Generic.List<string>();
                    foreach (var grupoErros in dados.SelectTokens("$..erros.item"))
                    {
                        var itens = grupoErros as JArray;
                        if (itens != null)
                        {
                            foreach (var item in itens) AdicionarErro(erros, item);
                        }
                        else
                        {
                            AdicionarErro(erros, grupoErros);
                        }
                    }
                    if (erros.Count > 0) return new BancoResposta { Sucesso = false, Mensagem = string.Join(" | ", erros), Dados = dados, StatusCode = (int)response.StatusCode };
                    if (dados.SelectTokens("$..['SOAP-ENV:Fault']").Any()) return new BancoResposta { Sucesso = false, Mensagem = dados.SelectToken("$..faultstring")?.ToString() ?? "SOAP Fault retornado pelo CrediSIS", Dados = dados, StatusCode = (int)response.StatusCode };
                    return BancoResposta.Ok(dados);
                }
                catch (Exception ex) { return BancoResposta.Falha("Resposta XML inválida do CrediSIS: " + ex.Message, (int)response.StatusCode); }
            }
        }

        private static string Pessoa(string nome, PagadorPadrao pessoa)
        {
            if (pessoa == null) return string.Empty;
            return "<" + nome + ">" + Elemento("nome", pessoa.Nome) + Elemento("cpfCnpj", ApenasDigitos(pessoa.Documento)) +
                "<endereco>" + Elemento("endereco", pessoa.Endereco) + Elemento("bairro", pessoa.Bairro) + Elemento("cep", ApenasDigitos(pessoa.Cep)) +
                Elemento("cidade", pessoa.Cidade) + Elemento("uf", pessoa.Uf) + Elemento("numero", pessoa.Numero) + "</endereco></" + nome + ">";
        }

        private static string CorpoOuVazio(JToken corpo) => corpo == null ? "<boleto />" : corpo.Type == JTokenType.String ? corpo.Value<string>() : corpo.ToString(Formatting.None);
        private static string Elemento(string nome, string valor) => string.IsNullOrWhiteSpace(valor) ? string.Empty : "<" + nome + ">" + Escapar(valor) + "</" + nome + ">";
        private static string Escapar(string valor) => string.IsNullOrEmpty(valor) ? string.Empty : valor.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var resultado = new StringBuilder(); foreach (var c in valor) if (char.IsDigit(c)) resultado.Append(c); return resultado.ToString(); }
        private static string JuntarInstrucoes(BoletoPadrao boleto) => string.Join(" ", new[] { boleto.Instrucao1, boleto.Instrucao2, boleto.Instrucao3 }).Trim();

        private static void AdicionarErro(System.Collections.Generic.List<string> erros, JToken item)
        {
            if (item == null) return;
            var codigo = item.Value<string>("code");
            var mensagem = item.Value<string>("message");
            if (!string.IsNullOrWhiteSpace(codigo) || !string.IsNullOrWhiteSpace(mensagem)) erros.Add(string.IsNullOrWhiteSpace(codigo) ? mensagem : codigo + ": " + mensagem);
        }
    }
}
