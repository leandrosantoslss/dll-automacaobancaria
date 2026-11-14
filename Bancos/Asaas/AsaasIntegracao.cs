using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Asaas
{
    public class AsaasConfiguracao
    {
        public string ApiKey { get; set; }
        public bool Sandbox { get; set; }
        public string UserAgent { get; set; } = "AutomacaoBancaria/1.0";
        public string BaseUrl { get; set; }

        public string ObterBaseUrl()
        {
            if (!string.IsNullOrWhiteSpace(BaseUrl))
                return BaseUrl.TrimEnd('/');
            return Sandbox ? "https://api-sandbox.asaas.com/v3" : "https://api.asaas.com/v3";
        }
    }

    public sealed class AsaasIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly AsaasConfiguracao configuracao;

        public AsaasIntegracao(AsaasConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ApiKey))
                throw new ArgumentException("A chave da API do Asaas é obrigatória", nameof(configuracao));
        }

        public override int CodigoBanco => 461;
        public override string Nome => "Asaas";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Extrato | BancoCapacidades.Saldo | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "CreateBoleto");

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "GetPayment");

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "CreatePix");

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "GetPixQrCode");

        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "ListFinancialTransactions");

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
            => ExecutarAsync(requisicao, "GetBalance");

        public async Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return BancoResposta.Falha("Requisição inválida");
            var req = requisicao.ToRequisicao();
            if (req.Corpo != null)
                return await ExecutarAsync(req, string.IsNullOrWhiteSpace(req.Operacao) ? "CreatePayment" : req.Operacao);

            var boleto = requisicao.BoletoPadrao;
            if (boleto == null && requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0)
                boleto = requisicao.BoletosPadrao[0];
            if (boleto == null)
                return BancoResposta.Falha("Boleto ou payload do Asaas não informado");

            var customerId = await ObterOuCriarClienteAsync(boleto.Pagador, req.Parametros);
            if (!customerId.Sucesso) return customerId;
            var corpo = new JObject
            {
                ["customer"] = customerId.Dados["id"],
                ["billingType"] = "BOLETO",
                ["value"] = boleto.Valor,
                ["dueDate"] = boleto.Vencimento.ToString("yyyy-MM-dd")
            };
            if (!string.IsNullOrWhiteSpace(boleto.SeuNumero)) corpo["externalReference"] = boleto.SeuNumero;
            if (!string.IsNullOrWhiteSpace(boleto.NumeroDocumento)) corpo["description"] = boleto.NumeroDocumento;
            return await ExecutarAsync(new BancoRequisicao { Corpo = corpo }, "CreatePayment");
        }

        public async Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
        {
            if (requisicao == null) return BancoResposta.Falha("Requisição inválida");
            var req = requisicao.ToRequisicao();
            if (req.Corpo != null)
                return await ExecutarAsync(req, string.IsNullOrWhiteSpace(req.Operacao) ? "CreatePix" : req.Operacao);

            var parametros = req.Parametros ?? new JObject();
            var customerId = parametros.Value<string>("customerId");
            if (string.IsNullOrWhiteSpace(customerId) && !string.IsNullOrWhiteSpace(requisicao.DocumentoDevedor))
            {
                var cliente = await ObterOuCriarClienteAsync(new PagadorPadrao
                {
                    Nome = requisicao.NomeDevedor,
                    Documento = requisicao.DocumentoDevedor
                }, parametros);
                if (!cliente.Sucesso) return cliente;
                customerId = cliente.Dados.Value<string>("id");
            }
            if (string.IsNullOrWhiteSpace(customerId))
                return BancoResposta.Falha("customerId ou dados do devedor são obrigatórios no Asaas");

            var corpo = new JObject
            {
                ["customer"] = customerId,
                ["billingType"] = "PIX",
                ["value"] = requisicao.Valor,
                ["dueDate"] = DateTime.Today.AddSeconds(requisicao.Expiracao ?? 3600).ToString("yyyy-MM-dd")
            };
            if (!string.IsNullOrWhiteSpace(requisicao.SolicitacaoPagador)) corpo["description"] = requisicao.SolicitacaoPagador;
            return await ExecutarAsync(new BancoRequisicao { Corpo = corpo }, "CreatePayment");
        }

        public Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.Id)) parametros["id"] = requisicao.Id;
            if (requisicao.Inicio.HasValue) parametros["dueDate[ge]"] = requisicao.Inicio.Value.ToString("yyyy-MM-dd");
            if (requisicao.Fim.HasValue) parametros["dueDate[le]"] = requisicao.Fim.Value.ToString("yyyy-MM-dd");
            var operacao = string.IsNullOrWhiteSpace(requisicao.Operacao)
                ? (!string.IsNullOrWhiteSpace(requisicao.Id) ? "GetPayment" : "ListPayments")
                : requisicao.Operacao;
            if (operacao == "ListPayments") parametros["billingType"] = "BOLETO";
            return ExecutarAsync(new BancoRequisicao { Operacao = operacao, Parametros = parametros }, operacao);
        }

        public Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (!string.IsNullOrWhiteSpace(requisicao.TxId)) parametros["id"] = requisicao.TxId;
            var operacao = string.IsNullOrWhiteSpace(requisicao.Operacao) ? "GetPixQrCode" : requisicao.Operacao;
            return ExecutarAsync(new BancoRequisicao { Operacao = operacao, Parametros = parametros }, operacao);
        }

        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (requisicao.Inicio.HasValue) parametros["startDate"] = requisicao.Inicio.Value.ToString("yyyy-MM-dd");
            if (requisicao.Fim.HasValue) parametros["finishDate"] = requisicao.Fim.Value.ToString("yyyy-MM-dd");
            var operacao = string.IsNullOrWhiteSpace(requisicao.Operacao) ? "ListFinancialTransactions" : requisicao.Operacao;
            return ExecutarAsync(new BancoRequisicao { Operacao = operacao, Parametros = parametros }, operacao);
        }

        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao)
        {
            var operacao = string.IsNullOrWhiteSpace(requisicao?.Operacao) ? "GetBalance" : requisicao.Operacao;
            return ExecutarAsync(requisicao?.ToRequisicao() ?? new BancoRequisicao(), operacao);
        }

        private async Task<BancoResposta> ExecutarAsync(BancoRequisicao requisicao, string operacao)
        {
            requisicao = requisicao ?? new BancoRequisicao();
            var parametros = requisicao.Parametros ?? new JObject();
            var metodo = "GET";
            var caminho = "";
            JToken corpo = requisicao.Corpo;

            switch (operacao)
            {
                case "CreateCustomer": metodo = "POST"; caminho = "/customers"; break;
                case "FindCustomer": caminho = "/customers"; break;
                case "CreatePayment":
                case "CreateBoleto":
                case "CreatePix": metodo = "POST"; caminho = "/payments"; break;
                case "GetPayment": caminho = "/payments/" + Required(parametros, "id", "paymentId"); break;
                case "ListPayments": caminho = "/payments"; break;
                case "GetPixQrCode": caminho = "/payments/" + Required(parametros, "id", "paymentId") + "/pixQrCode"; break;
                case "GetIdentificationField": caminho = "/payments/" + Required(parametros, "id", "paymentId") + "/identificationField"; break;
                case "GetBalance": caminho = "/finance/balance"; break;
                case "ListFinancialTransactions": caminho = "/financialTransactions"; break;
                default:
                    caminho = parametros.Value<string>("path");
                    if (string.IsNullOrWhiteSpace(caminho)) return BancoResposta.Falha("Operação Asaas não suportada: " + operacao);
                    metodo = parametros.Value<string>("method") ?? "GET";
                    break;
            }

            var query = new List<string>();
            foreach (var item in parametros)
            {
                if (item.Key == "id" || item.Key == "paymentId" || item.Key == "path" || item.Key == "method" || item.Value == null) continue;
                if (item.Value.Type != JTokenType.Array && item.Value.Type != JTokenType.Object)
                    query.Add(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value.ToString()));
            }
            if (query.Count > 0 && metodo == "GET") caminho += "?" + string.Join("&", query);
            if (operacao == "FindCustomer" && query.Count == 0)
                return BancoResposta.Falha("Informe cpfCnpj, email ou externalReference para localizar o cliente");

            return await EnviarAsync(metodo, caminho, corpo);
        }

        private async Task<BancoResposta> ObterOuCriarClienteAsync(PagadorPadrao pagador, JObject parametros)
        {
            var id = parametros?.Value<string>("customerId");
            if (!string.IsNullOrWhiteSpace(id)) return BancoResposta.Ok(new JObject { ["id"] = id });
            if (pagador == null || string.IsNullOrWhiteSpace(pagador.Nome) || string.IsNullOrWhiteSpace(pagador.Documento))
                return BancoResposta.Falha("Nome e Documento do pagador são obrigatórios para criar o cliente Asaas");

            var consulta = await ExecutarAsync(new BancoRequisicao
            {
                Parametros = new JObject { ["cpfCnpj"] = pagador.Documento }
            }, "FindCustomer");
            var existente = consulta.Dados?["data"] as JArray;
            if (consulta.Sucesso && existente != null && existente.Count > 0)
                return BancoResposta.Ok(existente[0]);
            if (!consulta.Sucesso && consulta.StatusCode != 404) return consulta;

            var corpo = new JObject { ["name"] = pagador.Nome, ["cpfCnpj"] = pagador.Documento };
            if (!string.IsNullOrWhiteSpace(pagador.Email)) corpo["email"] = pagador.Email;
            if (!string.IsNullOrWhiteSpace(pagador.Telefone)) corpo["mobilePhone"] = pagador.Telefone;
            return await ExecutarAsync(new BancoRequisicao { Corpo = corpo }, "CreateCustomer");
        }

        private async Task<BancoResposta> EnviarAsync(string metodo, string caminho, JToken corpo)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("access_token", configuracao.ApiKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(configuracao.UserAgent ?? "AutomacaoBancaria/1.0");
                var request = new HttpRequestMessage(new HttpMethod(metodo), configuracao.ObterBaseUrl() + (caminho.StartsWith("/") ? caminho : "/" + caminho));
                if (corpo != null && metodo != "GET")
                    request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var texto = await response.Content.ReadAsStringAsync();
                JToken dados = string.IsNullOrWhiteSpace(texto) ? null : JToken.Parse(texto);
                if (!response.IsSuccessStatusCode)
                    return BancoResposta.Falha(dados?["errors"]?.ToString(Formatting.None) ?? "Erro retornado pela API do Asaas", (int)response.StatusCode);
                return BancoResposta.Ok(dados);
            }
        }

        private static string Required(JObject parametros, params string[] nomes)
        {
            foreach (var nome in nomes)
            {
                var valor = parametros.Value<string>(nome);
                if (!string.IsNullOrWhiteSpace(valor)) return valor;
            }
            throw new ArgumentException("Informe " + string.Join(" ou ", nomes) + " nos parâmetros");
        }
    }
}
