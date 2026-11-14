using AutomacaoBancaria.Bancos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos.Bradesco
{
    public class BradescoConfiguracao
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CodigoUsuario { get; set; }
        public string CpfCnpjBeneficiario { get; set; }
        public string FilialBeneficiario { get; set; }
        public string ControleBeneficiario { get; set; }
        public int Produto { get; set; } = 9;
        public string Negociacao { get; set; }
        public string Agencia { get; set; }
        public string ContaCorrente { get; set; }
        public string TipoConta { get; set; } = "cc";
        public bool Sandbox { get; set; }
        public string Certificado { get; set; }
        public string CertificadoBase64 { get; set; }
        public string CertificadoSenha { get; set; }

        public string BaseUrl => Sandbox ? "https://openapisandbox.prebanco.com.br" : "https://openapi.bradesco.com.br";
    }

    public sealed class BradescoIntegracao : BancoIntegracaoBase, IBancoHibrido
    {
        private readonly BradescoConfiguracao configuracao;
        private readonly X509Certificate2 certificado;
        private string token;
        private DateTime tokenExpiraEm;
        private string pixToken;
        private DateTime pixTokenExpiraEm;
        private string contaToken;
        private DateTime contaTokenExpiraEm;
        public string JsonEnvio { get; private set; }

        public BradescoIntegracao(BradescoConfiguracao configuracao)
        {
            this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            if (string.IsNullOrWhiteSpace(configuracao.ClientId) || string.IsNullOrWhiteSpace(configuracao.ClientSecret)) throw new ArgumentException("ClientId e ClientSecret Bradesco são obrigatórios", nameof(configuracao));
            certificado = CarregarCertificado(configuracao);
        }

        public override int CodigoBanco => 237;
        public override string Nome => "Bradesco";
        public override BancoCapacidades Capacidades => BancoCapacidades.Boleto | BancoCapacidades.Pix | BancoCapacidades.Outros;

        public override Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConfiguracaoCobranca();
            return erro != null ? Task.FromResult(BancoResposta.Falha(erro)) : requisicao?.Corpo == null ? Task.FromResult(BancoResposta.Falha("Dados do boleto não informados")) : EnviarAsync("/boleto-hibrido/cobranca-registro/v1/gerarBoleto", requisicao.Corpo, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConfiguracaoCobranca();
            if (erro != null) return Task.FromResult(BancoResposta.Falha(erro));
            var parametros = requisicao?.Parametros ?? new JObject();
            var nossoNumero = parametros.Value<string>("nossoNumero") ?? parametros.Value<string>("id");
            if (string.IsNullOrWhiteSpace(nossoNumero)) return Task.FromResult(BancoResposta.Falha("Informe nossoNumero para consultar boleto Bradesco"));
            var corpo = new JObject
            {
                ["contaProduto"] = Numero(configuracao.Negociacao),
                ["controleCpfCnpjUsuario"] = Numero(configuracao.ControleBeneficiario),
                ["cpfCnpjUsuario"] = Numero(configuracao.CpfCnpjBeneficiario),
                ["filialCnpjUsuario"] = Numero(configuracao.FilialBeneficiario),
                ["idProduto"] = configuracao.Produto,
                ["nomePersonalizado"] = "",
                ["nossoNumero"] = Numero(nossoNumero),
                ["seqTitulo"] = 0,
                ["status"] = 0
            };
            return EnviarAsync("/boleto-hibrido/cobranca-consulta-titulo/v1/consultar", corpo, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
        {
            if (requisicao?.Corpo == null) return Task.FromResult(BancoResposta.Falha("Dados da cobrança PIX não informados"));
            var txId = requisicao.Parametros?.Value<string>("txId") ?? requisicao.Parametros?.Value<string>("transactionId");
            return EnviarPixAsync(string.IsNullOrWhiteSpace(txId) ? HttpMethod.Post : HttpMethod.Put, "/v2/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId)), requisicao.Corpo, null, requisicao.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
        {
            var parametros = requisicao?.Parametros ?? new JObject();
            var txId = parametros.Value<string>("txId") ?? parametros.Value<string>("transactionId");
            var endToEndId = parametros.Value<string>("endToEndId");
            parametros.Remove("txId"); parametros.Remove("transactionId"); parametros.Remove("endToEndId");
            var caminho = !string.IsNullOrWhiteSpace(endToEndId) ? "/v2/pix/" + Uri.EscapeDataString(endToEndId) : "/v2/cob" + (string.IsNullOrWhiteSpace(txId) ? "" : "/" + Uri.EscapeDataString(txId));
            return EnviarPixAsync(HttpMethod.Get, caminho, null, parametros, requisicao?.Cabecalhos);
        }
        public override Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConfiguracaoConta();
            if (erro != null) return Task.FromResult(BancoResposta.Falha(erro));
            var parametros = requisicao?.Parametros ?? new JObject();
            var inicio = parametros.Value<DateTime?>("inicio");
            var fim = parametros.Value<DateTime?>("fim");
            if (!inicio.HasValue || !fim.HasValue) return Task.FromResult(BancoResposta.Falha("Informe inicio e fim para consultar extrato Bradesco"));
            parametros["agencia"] = configuracao.Agencia;
            parametros["conta"] = configuracao.ContaCorrente;
            parametros["tipo"] = string.IsNullOrWhiteSpace(configuracao.TipoConta) ? "cc" : configuracao.TipoConta;
            parametros["dataInicio"] = inicio.Value.ToString("ddMMyyyy");
            parametros["dataFim"] = fim.Value.ToString("ddMMyyyy");
            parametros.Remove("inicio"); parametros.Remove("fim"); parametros.Remove("mes"); parametros.Remove("ano"); parametros.Remove("diaInicial"); parametros.Remove("diaFinal");
            return EnviarContaAsync("/v1/fornecimento-extratos-contas/extratos", parametros, requisicao?.Cabecalhos);
        }

        public override Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
        {
            var erro = ValidarConfiguracaoConta();
            if (erro != null) return Task.FromResult(BancoResposta.Falha(erro));
            var parametros = requisicao?.Parametros ?? new JObject();
            parametros["agencia"] = configuracao.Agencia;
            parametros["conta"] = configuracao.ContaCorrente;
            return EnviarContaAsync("/v1/fornecimento-saldos-contas/saldos", parametros, requisicao?.Cabecalhos);
        }

        public Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                var boleto = requisicao.BoletoPadrao ?? (requisicao.BoletosPadrao != null && requisicao.BoletosPadrao.Length > 0 ? requisicao.BoletosPadrao[0] : null);
                if (boleto == null) return Task.FromResult(BancoResposta.Falha("Boleto ou payload Bradesco não informado"));
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

        public Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var req = requisicao.ToRequisicao();
            if (req.Corpo == null)
            {
                if (string.IsNullOrWhiteSpace(requisicao.Chave) || !requisicao.Valor.HasValue || requisicao.Valor.Value <= 0) return Task.FromResult(BancoResposta.Falha("Chave e valor são obrigatórios para criar PIX Bradesco"));
                req.Corpo = new JObject
                {
                    ["calendario"] = new JObject { ["expiracao"] = (requisicao.Expiracao ?? 3600).ToString(CultureInfo.InvariantCulture) },
                    ["valor"] = new JObject { ["original"] = requisicao.Valor.Value.ToString("0.00", CultureInfo.InvariantCulture), ["modalidadeAlteracao"] = 0 },
                    ["chave"] = requisicao.Chave,
                    ["solicitacaoPagador"] = requisicao.SolicitacaoPagador ?? ""
                };
                var documento = ApenasDigitos(requisicao.DocumentoDevedor);
                if (!string.IsNullOrWhiteSpace(requisicao.NomeDevedor) && !string.IsNullOrWhiteSpace(documento)) req.Corpo["devedor"] = new JObject { [documento.Length > 11 ? "cnpj" : "cpf"] = documento, ["nome"] = requisicao.NomeDevedor };
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
            if (requisicao.Inicio.HasValue) parametros["inicio"] = requisicao.Inicio.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            if (requisicao.Fim.HasValue) parametros["fim"] = requisicao.Fim.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return ConsultarPixAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao.Cabecalhos });
        }
        public Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao)
        {
            if (requisicao == null) return Task.FromResult(BancoResposta.Falha("Requisição inválida"));
            var parametros = requisicao.Parametros ?? new JObject();
            if (requisicao.Inicio.HasValue) parametros["inicio"] = requisicao.Inicio;
            if (requisicao.Fim.HasValue) parametros["fim"] = requisicao.Fim;
            return ConsultarExtratoAsync(new BancoRequisicao { Parametros = parametros, Cabecalhos = requisicao.Cabecalhos });
        }
        public Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao) => ConsultarSaldoAsync(requisicao?.ToRequisicao());

        private JObject CriarBoleto(BoletoPadrao boleto)
        {
            var documento = ApenasDigitos(boleto.Pagador?.Documento);
            var cep = ApenasDigitos(boleto.Pagador?.Cep);
            var corpo = new JObject
            {
                ["ctitloCobrCdent"] = 0, ["registrarTitulo"] = 1,
                ["nroCpfCnpjBenef"] = Numero(configuracao.CpfCnpjBeneficiario), ["codUsuario"] = configuracao.CodigoUsuario,
                ["filCpfCnpjBenef"] = configuracao.FilialBeneficiario, ["tipoAcesso"] = 2, ["digCpfCnpjBenef"] = Numero(configuracao.ControleBeneficiario),
                ["cidtfdProdCobr"] = configuracao.Produto, ["cnegocCobr"] = Numero(configuracao.Negociacao), ["tipoRegistro"] = 1, ["codigoBanco"] = 237,
                ["demisTitloCobr"] = DateTime.Today.ToString("dd.MM.yyyy"), ["ctitloCliCdent"] = boleto.SeuNumero ?? boleto.NumeroDocumento,
                ["dvctoTitloCobr"] = boleto.Vencimento.ToString("dd.MM.yyyy"), ["vnmnalTitloCobr"] = boleto.Valor * 100,
                ["cindcdEconmMoeda"] = 9, ["cespceTitloCobr"] = 2, ["cindcdAceitSacdo"] = "N", ["cformaEmisPplta"] = "02",
                ["cindcdPgtoParcial"] = "N", ["isacdoTitloCobr"] = boleto.Pagador?.Nome, ["enroLogdrSacdo"] = boleto.Pagador?.Numero,
                ["elogdrSacdoTitlo"] = boleto.Pagador?.Endereco, ["ecomplLogdrSacdo"] = "", ["ccepSacdoTitlo"] = cep?.Substring(0, Math.Min(5, cep.Length)) ?? "",
                ["ccomplCepSacdo"] = cep != null && cep.Length > 5 ? cep.Substring(5) : "", ["ebairoLogdrSacdo"] = boleto.Pagador?.Bairro,
                ["imunSacdoTitlo"] = boleto.Pagador?.Cidade, ["indCpfCnpjSacdo"] = documento?.Length > 11 ? 2 : 1, ["csglUfSacdo"] = boleto.Pagador?.Uf,
                ["renderEletrSacdo"] = boleto.Pagador?.Email ?? "", ["nroCpfCnpjSacdo"] = Numero(documento), ["fase"] = "1",
                ["cindcdCobrMisto"] = boleto.CadastrarPix == true ? "S" : "N", ["idLoc"] = ""
            };
            corpo["cpssoaJuridContr"] = ""; corpo["ctpoContrNegoc"] = ""; corpo["nseqContrNegoc"] = ""; corpo["eNseqContrNegoc"] = "";
            corpo["filler"] = ""; corpo["cprodtServcOper"] = ""; corpo["cidtfdTpoVcto"] = ""; corpo["qmoedaNegocTitlo"] = 0;
            corpo["ctpoProteTitlo"] = 0; corpo["ctpoPrzProte"] = 0; corpo["ctpoPrzDecurs"] = 0; corpo["ctpoProteDecurs"] = 0;
            corpo["cctrlPartcTitlo"] = 0; corpo["qtdePgtoParcial"] = 0; corpo["qtdDecurPrz"] = "0"; corpo["codNegativacao"] = "0";
            corpo["diasNegativacao"] = "0"; corpo["ptxJuroVcto"] = 0; corpo["filler1"] = ""; corpo["vdiaJuroMora"] = 0;
            corpo["pmultaAplicVcto"] = 0; corpo["qdiaInicJuro"] = 0; corpo["vmultaAtrsoPgto"] = 0; corpo["qdiaInicMulta"] = 0;
            corpo["pdescBonifPgto01"] = 0; corpo["vdescBonifPgto01"] = 0; corpo["dlimDescBonif1"] = ""; corpo["pdescBonifPgto02"] = 0;
            corpo["vdescBonifPgto02"] = 0; corpo["dlimDescBonif2"] = ""; corpo["pdescBonifPgto03"] = 0; corpo["vdescBonifPgto03"] = 0;
            corpo["dlimDescBonif3"] = ""; corpo["ctpoPrzCobr"] = 0; corpo["pdescBonifPgto"] = 0; corpo["dlimBonifPgto"] = "";
            corpo["vdescBonifPgto"] = 0; corpo["vabtmtTitloCobr"] = 0; corpo["filler2"] = ""; corpo["viofPgtoTitlo"] = 0;
            corpo["cdddFoneSacdo"] = 0; corpo["bancoDeb"] = 0; corpo["cfoneSacdoTitlo"] = 0; corpo["agenciaDebDv"] = 0;
            corpo["agenciaDeb"] = 0; corpo["bancoCentProt"] = 0; corpo["contaDeb"] = 0; corpo["isacdrAvalsTitlo"] = "";
            corpo["agenciaDvCentPr"] = 0; corpo["enroLogdrSacdr"] = "0"; corpo["elogdrSacdrAvals"] = ""; corpo["ecomplLogdrSacdr"] = "";
            corpo["ccomplCepSacdr"] = 0; corpo["ebairoLogdrSacdr"] = ""; corpo["csglUfSacdr"] = ""; corpo["ccepSacdrTitlo"] = 0;
            corpo["imunSacdrAvals"] = ""; corpo["indCpfCnpjSacdr"] = 0; corpo["renderEletrSacdr"] = ""; corpo["nroCpfCnpjSacdr"] = 0;
            corpo["cdddFoneSacdr"] = 0; corpo["filler3"] = "0"; corpo["cfoneSacdrTitlo"] = 0; corpo["iconcPgtoSpi"] = "";
            corpo["ialiasAdsaoCta"] = ""; corpo["ilinkGeracQrcd"] = ""; corpo["caliasAdsaoCta"] = ""; corpo["wqrcdPdraoMercd"] = "";
            corpo["validadeAposVencimento"] = ""; corpo["filler4"] = "";
            return corpo;
        }

        private async Task<BancoResposta> EnviarAsync(string caminho, JToken corpo, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenAsync();
                if (!autenticacao.Sucesso) return autenticacao;
                JsonEnvio = corpo?.ToString(Formatting.Indented) ?? "";
                using (var client = new HttpClient(CriarHandler()))
                using (var request = new HttpRequestMessage(HttpMethod.Post, configuracao.BaseUrl + caminho))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (TaskCanceledException ex) { return FalhaComunicacao("Tempo limite excedido ao chamar a API Bradesco", ex); }
            catch (HttpRequestException ex) { return FalhaComunicacao("Falha de comunicação com a API Bradesco", ex); }
            catch (Exception ex) { return FalhaComunicacao("Erro ao chamar a API Bradesco", ex); }
        }

        private async Task<BancoResposta> EnviarPixAsync(HttpMethod metodo, string caminho, JToken corpo, JObject parametros, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenPixAsync();
                if (!autenticacao.Sucesso) return autenticacao;
                JsonEnvio = corpo?.ToString(Formatting.Indented) ?? "";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(metodo, ObterBaseUrlPix() + caminho + CriarQuery(parametros)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pixToken);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    if (corpo != null) request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (TaskCanceledException ex) { return FalhaComunicacao("Tempo limite excedido ao chamar a API PIX Bradesco", ex); }
            catch (HttpRequestException ex) { return FalhaComunicacao("Falha de comunicação com a API PIX Bradesco", ex); }
            catch (Exception ex) { return FalhaComunicacao("Erro ao chamar a API PIX Bradesco", ex); }
        }

        private async Task<BancoResposta> EnviarContaAsync(string caminho, JObject parametros, JObject cabecalhos)
        {
            try
            {
                var autenticacao = await ObterTokenContaAsync();
                if (!autenticacao.Sucesso) return autenticacao;
                JsonEnvio = "";
                using (var client = new HttpClient(CriarHandler()))
                using (var request = new HttpRequestMessage(HttpMethod.Get, configuracao.BaseUrl + caminho + CriarQuery(parametros)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", contaToken);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AdicionarCabecalhos(request, cabecalhos);
                    return await LerRespostaAsync(await client.SendAsync(request));
                }
            }
            catch (TaskCanceledException ex) { return FalhaComunicacao("Tempo limite excedido ao chamar a API de conta Bradesco", ex); }
            catch (HttpRequestException ex) { return FalhaComunicacao("Falha de comunicação com a API de conta Bradesco", ex); }
            catch (Exception ex) { return FalhaComunicacao("Erro ao chamar a API de conta Bradesco", ex); }
        }

        private async Task<BancoResposta> ObterTokenContaAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(contaToken) && contaTokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
                using (var client = new HttpClient(CriarHandler()))
                {
                    var valores = new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("client_id", configuracao.ClientId), new KeyValuePair<string, string>("client_secret", configuracao.ClientSecret) };
                    var versao = configuracao.Sandbox ? "v2" : "v1";
                    var resposta = await client.PostAsync(configuracao.BaseUrl + "/auth/server-mtls/" + versao + "/token", new FormUrlEncodedContent(valores));
                    var resultado = await LerRespostaAsync(resposta);
                    if (!resultado.Sucesso) return resultado;
                    contaToken = resultado.Dados?.Value<string>("access_token");
                    var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 0;
                    if (string.IsNullOrWhiteSpace(contaToken) || expiraEm <= 0) return BancoResposta.Falha("Resposta de autenticação de conta Bradesco inválida");
                    contaTokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                    return BancoResposta.Ok(null);
                }
            }
            catch (Exception ex) { return FalhaComunicacao("Erro ao obter o token de conta Bradesco", ex); }
        }

        private async Task<BancoResposta> ObterTokenPixAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(pixToken) && pixTokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
                using (var client = new HttpClient())
                {
                    var valores = new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("client_id", configuracao.ClientId), new KeyValuePair<string, string>("client_secret", configuracao.ClientSecret), new KeyValuePair<string, string>("scope", "") };
                    var resposta = await client.PostAsync(ObterBaseUrlPix() + "/auth/server/oauth/token", new FormUrlEncodedContent(valores));
                    var resultado = await LerRespostaAsync(resposta);
                    if (!resultado.Sucesso) return resultado;
                    pixToken = resultado.Dados?.Value<string>("access_token");
                    var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 0;
                    if (string.IsNullOrWhiteSpace(pixToken) || expiraEm <= 0) return BancoResposta.Falha("Resposta de autenticação PIX Bradesco inválida");
                    pixTokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                    return BancoResposta.Ok(null);
                }
            }
            catch (Exception ex) { return FalhaComunicacao("Erro ao obter o token PIX Bradesco", ex); }
        }

        private async Task<BancoResposta> ObterTokenAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token) && tokenExpiraEm > DateTime.UtcNow.AddSeconds(30)) return BancoResposta.Ok(null);
                using (var client = new HttpClient(CriarHandler()))
                {
                    var valores = new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), new KeyValuePair<string, string>("client_id", configuracao.ClientId), new KeyValuePair<string, string>("client_secret", configuracao.ClientSecret) };
                    var resposta = await client.PostAsync(configuracao.BaseUrl + "/auth/server-mtls/v2/token", new FormUrlEncodedContent(valores));
                    var resultado = await LerRespostaAsync(resposta);
                    if (!resultado.Sucesso) return resultado;
                    token = resultado.Dados?.Value<string>("access_token");
                    var expiraEm = resultado.Dados?.Value<int?>("expires_in") ?? 0;
                    if (string.IsNullOrWhiteSpace(token) || expiraEm <= 0) return BancoResposta.Falha("Resposta de autenticação Bradesco inválida");
                    tokenExpiraEm = DateTime.UtcNow.AddSeconds(expiraEm);
                    return BancoResposta.Ok(null);
                }
            }
            catch (Exception ex) { return FalhaComunicacao("Erro ao obter o token Bradesco", ex); }
        }

        private HttpClientHandler CriarHandler()
        {
            var handler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
            if (certificado != null) handler.ClientCertificates.Add(certificado);
            return handler;
        }

        private static X509Certificate2 CarregarCertificado(BradescoConfiguracao configuracao)
        {
            if (!string.IsNullOrWhiteSpace(configuracao.Certificado) && !string.IsNullOrWhiteSpace(configuracao.CertificadoBase64)) throw new ArgumentException("Informe o caminho do certificado ou o Base64, não ambos");
            if (!string.IsNullOrWhiteSpace(configuracao.CertificadoBase64)) return new X509Certificate2(Convert.FromBase64String(RemoverPrefixoData(configuracao.CertificadoBase64)), configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
            return string.IsNullOrWhiteSpace(configuracao.Certificado) ? null : new X509Certificate2(configuracao.Certificado, configuracao.CertificadoSenha, X509KeyStorageFlags.MachineKeySet);
        }

        private static async Task<BancoResposta> LerRespostaAsync(HttpResponseMessage response)
        {
            var texto = await response.Content.ReadAsStringAsync();
            JToken dados = null;
            if (!string.IsNullOrWhiteSpace(texto)) { try { dados = JToken.Parse(texto); } catch { dados = new JObject { ["conteudo"] = texto }; } }
            if (response.IsSuccessStatusCode) return BancoResposta.Ok(dados);
            return new BancoResposta { Sucesso = false, Mensagem = dados?["message"]?.ToString() ?? dados?["mensagem"]?.ToString() ?? texto ?? "Erro retornado pela API Bradesco", Dados = dados, StatusCode = (int)response.StatusCode };
        }

        private static BancoResposta FalhaComunicacao(string contexto, Exception ex)
        {
            var mensagens = new List<string> { contexto };
            while (ex != null) { if (!string.IsNullOrWhiteSpace(ex.Message) && !mensagens.Contains(ex.Message)) mensagens.Add(ex.Message); ex = ex.InnerException; }
            return BancoResposta.Falha(string.Join(" | ", mensagens));
        }

        private static void AdicionarCabecalhos(HttpRequestMessage request, JObject cabecalhos) { if (cabecalhos != null) foreach (var cabecalho in cabecalhos) if (cabecalho.Value != null && !request.Headers.Contains(cabecalho.Key)) request.Headers.TryAddWithoutValidation(cabecalho.Key, cabecalho.Value.ToString()); }
        private string ValidarConfiguracaoCobranca()
        {
            if (certificado == null) return "O certificado mTLS Bradesco é obrigatório para cobrança híbrida";
            if (string.IsNullOrWhiteSpace(configuracao.CodigoUsuario) || string.IsNullOrWhiteSpace(configuracao.CpfCnpjBeneficiario) || string.IsNullOrWhiteSpace(configuracao.FilialBeneficiario) || string.IsNullOrWhiteSpace(configuracao.ControleBeneficiario) || string.IsNullOrWhiteSpace(configuracao.Negociacao)) return "Dados de contrato Bradesco incompletos para cobrança híbrida";
            return null;
        }
        private string ValidarConfiguracaoConta()
        {
            if (certificado == null) return "O certificado mTLS Bradesco é obrigatório para consulta de conta";
            if (string.IsNullOrWhiteSpace(configuracao.Agencia) || string.IsNullOrWhiteSpace(configuracao.ContaCorrente)) return "Agencia e ContaCorrente Bradesco são obrigatórias para consulta de conta";
            return null;
        }
        private string ObterBaseUrlPix() => configuracao.Sandbox ? "https://openapisandbox.prebanco.com.br" : "https://qrpix.bradesco.com.br";
        private static string CriarQuery(JObject parametros)
        {
            if (parametros == null || !parametros.HasValues) return "";
            var valores = new List<string>();
            foreach (var parametro in parametros) if (parametro.Value != null && parametro.Value.Type != JTokenType.Object && parametro.Value.Type != JTokenType.Array) valores.Add(Uri.EscapeDataString(parametro.Key) + "=" + Uri.EscapeDataString(parametro.Value.ToString()));
            return valores.Count == 0 ? "" : "?" + string.Join("&", valores);
        }
        private static JToken Numero(string valor) { return long.TryParse(valor, out var numero) ? new JValue(numero) : new JValue(valor ?? ""); }
        private static string ApenasDigitos(string valor) { if (string.IsNullOrWhiteSpace(valor)) return null; var resultado = new StringBuilder(); foreach (var c in valor) if (char.IsDigit(c)) resultado.Append(c); return resultado.Length == 0 ? null : resultado.ToString(); }
        private static string RemoverPrefixoData(string valor) { var base64 = valor.Trim(); var separador = base64.IndexOf(','); return base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && separador >= 0 ? base64.Substring(separador + 1) : base64; }
    }
}
