using AutomacaoBancaria.Bancos;
using AutomacaoBancaria.Bancos.EFI;
using AutomacaoBancaria.Bancos.Asaas;
using AutomacaoBancaria.Bancos.Sicoob;
using AutomacaoBancaria.Bancos.Sicredi;
using AutomacaoBancaria.Bancos.Credisis;
using AutomacaoBancaria.Bancos.BB;
using AutomacaoBancaria.Bancos.Bradesco;
using AutomacaoBancaria.Bancos.Itau;
using AutomacaoBancaria.Bancos.Santander;
using AutomacaoBancaria.Bancos.Banrisul;
using AutomacaoBancaria.Bancos.Cresol;
using AutomacaoBancaria.Common;
using AutomacaoBancaria.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;

namespace AutomacaoBancaria
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("AutomacaoBancaria.cUtil")]
    [ComVisible(true)]
    public class cUtil : ICresol
    {
        #region  Variáveis Públicas
        public string LicencaDLL = "";
        public bool DetalharErros = false;
        public bool HabilitarLog = true;
        public string CaminhoLog = "";
        public string JsonEnvio = "";
        public string JsonRetorno = "";
        public bool Sucesso = false;
        public string SeuNumero = "";
        public string NossoNumero = "";
        public string CodigoBoleto = "";
        public string LinhaDigitavel = "";
        public string CodigoBarras = "";
        public string PIXCopiaCola = "";
        public string QRCode = "";
        public string pdfBoleto = "";
        public bool CriarArquivoPDF = false;
        public string CaminhoArquivoPDF = "";
        public string ArquivoPDFGerado = "";
        public string MensagemArquivoPDF = "";
        public int CodigoSolicitacaoMovimentacao = 0;
        public int QuantidadeArquivosMovimentacao = 0;
        public string IdArquivosMovimentacao = "";
        public string CaminhoArquivosMovimentacao = "";
        public string ArquivoMovimentacaoGerado = "";
        public string MensagemArquivoMovimentacao = "";
        public string JsonArquivosMovimentacao = "[]";
        public string[] NomesArquivosMovimentacao = new string[0];
        public string[] ConteudosArquivosMovimentacao = new string[0];
        public string CNPJCPFSoftHouse = "";
        public string CNPJCPFEmitente = "";
        public int BancoID = 0;
        public string ClientID = "";
        public string ClientSecret = "";
        public string AsaasApiKey = "";
        public string AsaasUserAgent = "AutomacaoBancaria/1.0";
        public string SicrediApiKey = "";
        public string SicrediCodigoAcesso = "";
        public string SicrediCodigoBeneficiario = "";
        public string SicrediCooperativa = "";
        public string SicrediPosto = "";
        public string CredisisToken = "";
        public string CredisisConvenio = "";
        public string CredisisUrl = "";
        public string BBAppKey = "";
        public string BradescoCodigoUsuario = "";
        public string BradescoCpfCnpjBeneficiario = "";
        public string BradescoFilialBeneficiario = "";
        public string BradescoControleBeneficiario = "";
        public int BradescoProduto = 9;
        public string BradescoNegociacao = "";
        public string BradescoAgencia = "";
        public string BradescoContaCorrente = "";
        public string BradescoTipoConta = "cc";
        public string ItauApiKey = "";
        public string ItauIdBeneficiario = "";
        public string ItauCodigoCarteira = "";
        public string ItauAgencia = "";
        public string ItauConta = "";
        public string ItauDac = "";
        public string ItauFrancesaId = "";
        public string ItauUrlAutenticacao = "";
        public string ItauBaseUrlPix = "";
        public string ItauBaseUrlRecebimentos = "";
        public string ItauBaseUrlExtrato = "";
        public string ItauBaseUrlBoletosCash = "";
        public string SantanderWorkspaceId = "";
        public string SantanderCovenantCode = "";
        public string SantanderBankNumber = "";
        public string SantanderParticipantCode = "";
        public string SantanderPixChave = "";
        public string SantanderPixChaveTipo = "EMAIL";
        public string SantanderBaseUrl = "";
        public string SantanderUrlAutenticacao = "";
        public string SantanderCnpjBanco = "";
        public string SantanderAgenciaConta = "";
        public string CertificadoDigitalBase64 = "";
        public string CertificadoDigitalSenha = "";
        public string BanrisulBeneficiario = "";
        public string BanrisulPartnerId = "";
        public string BanrisulCoPartnerId = "";
        public string BanrisulUrlAutenticacao = "";
        public string BanrisulUrlCobranca = "";
        public string BanrisulUrlPagamentos = "";
        public string BanrisulUrlPix = "";
        public string BanrisulUrlAutenticacaoPix = "";
        public string ClientCertificado = "";
        public string ClientCertificadoBase64 = "";
        public string ClientChavePrivada = "";
        public string ClientCertificadoSenha = "";
        public bool homologacao = false;
        public string PIXChave = "";
        public string urlWebhook = "";
        public int NumeroContrato = 0;
        public int NumeroContaCorrente = 0;
        
        #endregion

        #region Variáveis Privadas
        private string TokenChave = "";
        private string BancoURL = "";
        private string BancoEndPoint = "";
        private BoletoPadrao boletoPadrao = new BoletoPadrao();
        private PagadorPadrao pagadorPadrao = new PagadorPadrao();
        private List<ItemBoletoPadrao> itensBoleto = new List<ItemBoletoPadrao>();
        private List<BoletoPadrao> boletosPadrao = new List<BoletoPadrao>();
        private List<ItemBoletoPadrao> itensBoletoAtual = new List<ItemBoletoPadrao>();
        private PixCriacaoRequest pixCriacao = new PixCriacaoRequest();
        private PixConsultaRequest pixConsulta = new PixConsultaRequest();
        private ExtratoRequest extratoConsulta = new ExtratoRequest();
        private SaldoRequest saldoConsulta = new SaldoRequest();
        private BoletoConsultaRequest boletoConsulta = new BoletoConsultaRequest();
        private CresolConfiguracao _cresolConfiguracao;
        private CresolIntegracao _cresolIntegracao;
        #endregion

        #region Métodos Privados
        private bool BancoInformacoes(int BancoID, string TipoEndPonit)
        {
            try
            {
                BancoURL = "";
                BancoEndPoint = "";
                switch (BancoID)
                {
                    case 756:
                        {
                            if (homologacao)
                            {
                                ClientID = "9b5e603e428cc477a2841e2683c92d21";
                                TokenChave = "1301865f-c6bc-38f3-9f49-666dbcfc59c3";
                                switch (TipoEndPonit)
                                {
                                    case "boleto": BancoURL = "https://sandbox.sicoob.com.br/sicoob/sandbox/cobranca-bancaria/v3"; BancoEndPoint = "/boletos"; break;
                                    case "extrato": BancoURL = "https://sandbox.sicoob.com.br/sicoob/sandbox/conta-corrente/v4"; BancoEndPoint = "/extrato"; break;
                                    case "pagador": BancoURL = "https://sandbox.sicoob.com.br/sicoob/sandbox/cobranca-bancaria/v3"; BancoEndPoint = "/pagadores"; break;
                                    case "pix": BancoURL = "https://sandbox.sicoob.com.br/sicoob/sandbox/pix/api/v2"; BancoEndPoint = ""; break;
                                    case "saldo": BancoURL = "https://sandbox.sicoob.com.br/sicoob/sandbox/conta-corrente/v4"; BancoEndPoint = "/saldo"; break;
                                    case "token": BancoURL = "https://auth.sicoob.com.br/auth/realms/cooperado/protocol/openid-connect/token"; BancoEndPoint = ""; break;
                                    case "webhook": BancoURL = "https://api.sicoob.com.br/cobranca-bancaria/v3"; BancoEndPoint = "/webhooks"; break;
                                }
                            }
                            else
                            {
                                switch (TipoEndPonit)
                                {
                                    case "boleto": BancoURL = "https://api.sicoob.com.br/cobranca-bancaria/v3"; BancoEndPoint = "/boletos"; break;
                                    case "extrato": BancoURL = "https://api.sicoob.com.br/conta-corrente/v4"; BancoEndPoint = "/extrato"; break;
                                    case "pagador": BancoURL = "https://api.sicoob.com.br/cobranca-bancaria/v3"; BancoEndPoint = "/pagadores"; break;
                                    case "pix": BancoURL = "https://api.sicoob.com.br/pix/api/v2"; BancoEndPoint = ""; break;
                                    case "saldo": BancoURL = "https://api.sicoob.com.br/conta-corrente/v4"; BancoEndPoint = "/saldo"; break;
                                    case "token": BancoURL = "https://auth.sicoob.com.br/auth/realms/cooperado/protocol/openid-connect/token"; BancoEndPoint = ""; break;
                                    case "webhook": BancoURL = "https://api.sicoob.com.br/cobranca-bancaria/v3"; BancoEndPoint = "/webhooks"; break;
                                }
                            }
                            break;
                        }
                    case 461:
                        BancoURL = homologacao ? "https://api-sandbox.asaas.com/v3" : "https://api.asaas.com/v3";
                        BancoEndPoint = TipoEndPonit == "saldo" ? "/finance/balance" :
                            TipoEndPonit == "extrato" ? "/financialTransactions" : "/payments";
                        break;
                    case 748:
                        BancoURL = homologacao ? "https://api-parceiro.sicredi.com.br/sb" : "https://api-parceiro.sicredi.com.br";
                        BancoEndPoint = TipoEndPonit == "token" ? "/auth/openapi/token" : "/cobranca/boleto/v1/boletos";
                        break;
                    case 97:
                        BancoURL = string.IsNullOrWhiteSpace(CredisisUrl) ? "https://credisiscobranca.com.br/v2/ws" : CredisisUrl;
                        BancoEndPoint = "";
                        break;
                    case 133:
                        BancoURL = TipoEndPonit == "token"
                            ? CresolConfiguracao.TokenUrlPasswordHomologacao
                            : CresolConfiguracao.ApiBaseUrlHomologacao;
                        BancoEndPoint = "";
                        break;
                    case 1:
                        BancoURL = homologacao ? "https://api.sandbox.bb.com.br/cobrancas/v2" : "https://api.bb.com.br/cobrancas/v2";
                        BancoEndPoint = "/boletos";
                        break;
                    case 237:
                        BancoURL = homologacao ? "https://openapisandbox.prebanco.com.br" : "https://openapi.bradesco.com.br";
                        BancoEndPoint = "/boleto-hibrido/cobranca-registro/v1/gerarBoleto";
                        break;
                    case 341:
                        BancoURL = string.IsNullOrWhiteSpace(ItauBaseUrlBoletosCash) ? "https://secure.gateway.api.itau.com.br/boletoscash/v2" : ItauBaseUrlBoletosCash;
                        BancoEndPoint = "/boletos";
                        break;
                    case 33:
                        BancoURL = string.IsNullOrWhiteSpace(SantanderBaseUrl) ? "https://trust-open.api.santander.com.br" : SantanderBaseUrl;
                        BancoEndPoint = "/collection_bill_management/v2/workspaces/" + SantanderWorkspaceId + "/bank_slips";
                        break;
                    case 41:
                        BancoURL = string.IsNullOrWhiteSpace(BanrisulUrlAutenticacao) ? (homologacao ? "https://apidev.banrisul.com.br/auth/oauth/v2/token" : "https://api.banrisul.com.br/auth/oauth/v2/token") : BanrisulUrlAutenticacao;
                        BancoEndPoint = "";
                        break;
                    default: return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Métodos Públicos

        #region Configurações
        public bool Configuracao(int BancoID)
        {
            try
            {
                this.BancoID = BancoID;
                return BancoInformacoes(BancoID, "token");
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Certificado
        public bool CertificadoCarregarBase64(string caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho)) return false;
            ClientCertificadoBase64 = Convert.ToBase64String(File.ReadAllBytes(caminho));
            ClientCertificado = "";
            return true;
        }

        public bool CertificadoSelecionar()
        {
            using (var dialogo = new System.Windows.Forms.OpenFileDialog())
            {
                dialogo.Filter = "Certificados PFX/P12 (*.pfx;*.p12)|*.pfx;*.p12|Todos os arquivos (*.*)|*.*";
                dialogo.Multiselect = false;
                return dialogo.ShowDialog() == System.Windows.Forms.DialogResult.OK && CertificadoCarregarBase64(dialogo.FileName);
            }
        }
        #endregion

        #region Usando JSON
        public string EmitirBoletosPadraoJson(string json)
        {
            if (BancoID == 133) return FalhaCresol("CRESOL requer JSON CRESOL específico; use CresolTitulosIncluir.");
            var req = JsonConvert.DeserializeObject<BoletoEmissaoRequest>(json);
            var erro = ValidarEmissaoBoletos(req);
            if (erro != null) return Falha(erro);
            var banco = GetBanco();
            return Executar(() => banco.EmitirBoletoAsync(req), banco);
        }

        public string ConsultarBoletoJson(string json)
        {
            if (BancoID == 133) return FalhaCresol("CRESOL requer consulta JSON CRESOL explícita; use CresolTituloConsultar ou CresolTitulosListar.");
            var req = JsonConvert.DeserializeObject<BoletoConsultaRequest>(json);
            var erro = ValidarConsultaBoleto(req);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarBoletoAsync(req));
        }

        public string CriarPixJson(string json)
        {
            var req = JsonConvert.DeserializeObject<PixCriacaoRequest>(json);
            var erro = ValidarPixCriacao(req);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().CriarPixAsync(req));
        }

        public string ConsultarPixJson(string json)
        {
            var req = JsonConvert.DeserializeObject<PixConsultaRequest>(json);
            var erro = ValidarPixConsulta(req);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarPixAsync(req));
        }

        public string ConsultarExtratoJson(string json)
        {
            var req = JsonConvert.DeserializeObject<ExtratoRequest>(json);
            var erro = ValidarExtrato(req);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarExtratoAsync(req));
        }

        public string ConsultarSaldoJson(string json = null)
        {
            var req = string.IsNullOrWhiteSpace(json) ? new SaldoRequest() : JsonConvert.DeserializeObject<SaldoRequest>(json);
            return Executar(() => GetBanco().ConsultarSaldoAsync(req));
        }

        public string BanrisulConsultarTituloJson(string json)
        {
            var banco = GetBanco() as BanrisulIntegracao;
            if (banco == null) return Falha("BancoID deve ser 41 para pagamentos Banrisul");
            return Executar(() => banco.ConsultarTituloAsync(ValidarJsonPayload(json)), banco);
        }

        public string BanrisulSolicitarPagamentoJson(string json)
        {
            var banco = GetBanco() as BanrisulIntegracao;
            if (banco == null) return Falha("BancoID deve ser 41 para pagamentos Banrisul");
            return Executar(() => banco.SolicitarPagamentoAsync(ValidarJsonPayload(json)), banco);
        }

        public string BanrisulConfirmarPagamentoJson(string json)
        {
            var banco = GetBanco() as BanrisulIntegracao;
            if (banco == null) return Falha("BancoID deve ser 41 para pagamentos Banrisul");
            return Executar(() => banco.ConfirmarPagamentoAsync(ValidarJsonPayload(json)), banco);
        }

        public string BanrisulConsultarStatusPagamentoJson(string json)
        {
            var banco = GetBanco() as BanrisulIntegracao;
            if (banco == null) return Falha("BancoID deve ser 41 para pagamentos Banrisul");
            return Executar(() => banco.ConsultarStatusPagamentoAsync(ValidarJsonPayload(json)), banco);
        }

        #endregion

        #region Métodos de Boletos
        public void BoletoLimpar()
        {
            boletoPadrao = new BoletoPadrao();
            pagadorPadrao = new PagadorPadrao();
            itensBoleto.Clear();
            boletosPadrao.Clear();
            itensBoletoAtual.Clear();
        }

        public void BoletoDados(string nossoNumero, string seuNumero, string numeroDocumento, double valor, string vencimento, int modalidade = 0, bool gerarPdf = false, bool cadastrarPix = false, string instrucao1 = "", string instrucao2 = "", string instrucao3 = "", string dataLimitePagamento = "", int tipoDesconto = 0, double valorDesconto = 0, int tipoMulta = 0, string dataMulta = "", double valorMulta = 0, int tipoJuros = 0, string dataJuros = "", double valorJuros = 0, int numeroParcela = 0, int codigoProtesto = 0, int numeroDiasProtesto = 0, string dataPrimeiroDesconto = "")
        {
            boletoPadrao.NossoNumero = nossoNumero;
            boletoPadrao.SeuNumero = seuNumero;
            boletoPadrao.NumeroDocumento = numeroDocumento;
            boletoPadrao.Valor = Convert.ToDecimal(valor);
            boletoPadrao.Vencimento = Convert.ToDateTime(vencimento);
            boletoPadrao.Modalidade = modalidade == 0 ? (int?)null : modalidade;
            boletoPadrao.GerarPdf = gerarPdf;
            boletoPadrao.CadastrarPix = cadastrarPix;
            boletoPadrao.Instrucao1 = instrucao1;
            boletoPadrao.Instrucao2 = instrucao2;
            boletoPadrao.Instrucao3 = instrucao3;
            if (!string.IsNullOrWhiteSpace(dataLimitePagamento)) boletoPadrao.DataLimitePagamento = Convert.ToDateTime(dataLimitePagamento);
            if (!string.IsNullOrWhiteSpace(dataPrimeiroDesconto)) boletoPadrao.DataPrimeiroDesconto = Convert.ToDateTime(dataPrimeiroDesconto);
            boletoPadrao.ValorDesconto = valorDesconto == 0 ? (decimal?)null : Convert.ToDecimal(valorDesconto);
            boletoPadrao.TipoDesconto = tipoDesconto == 0 || valorDesconto == 0 ? (int?)null : tipoDesconto;
            boletoPadrao.TipoMulta = tipoMulta == 0 ? (int?)null : tipoMulta;
            if (!string.IsNullOrWhiteSpace(dataMulta))
            {
                var dataInformadaMulta = Convert.ToDateTime(dataMulta);
                boletoPadrao.DataMulta = dataInformadaMulta.Date == boletoPadrao.Vencimento.Date ? dataInformadaMulta.AddDays(1) : dataInformadaMulta;
            }
            boletoPadrao.ValorMulta = valorMulta == 0 ? (decimal?)null : Convert.ToDecimal(valorMulta);
            boletoPadrao.TipoJuros = tipoJuros == 0 ? (int?)null : tipoJuros;
            if (!string.IsNullOrWhiteSpace(dataJuros))
            {
                var dataInformadaJuros = Convert.ToDateTime(dataJuros);
                boletoPadrao.DataJuros = dataInformadaJuros.Date == boletoPadrao.Vencimento.Date ? dataInformadaJuros.AddDays(1) : dataInformadaJuros;
            }
            boletoPadrao.ValorJuros = valorJuros == 0 ? (decimal?)null : Convert.ToDecimal(valorJuros);
            boletoPadrao.NumeroParcela = numeroParcela == 0 ? (int?)null : numeroParcela;
            boletoPadrao.CodigoProtesto = codigoProtesto == 0 ? (int?)null : codigoProtesto;
            boletoPadrao.NumeroDiasProtesto = numeroDiasProtesto == 0 ? (int?)null : numeroDiasProtesto;
            boletoPadrao.IdentificacaoEmissaoBoleto = 2;
            boletoPadrao.IdentificacaoDistribuicaoBoleto = 2;
        }

        public void PagadorDados(string nome, string documento, string email, string telefone, string dataNascimento, string endereco, string numero, string bairro, string cidade, string uf, string cep)
        {
            pagadorPadrao.Nome = nome;
            pagadorPadrao.Documento = documento;
            pagadorPadrao.Email = email;
            pagadorPadrao.Telefone = telefone;
            if (!string.IsNullOrEmpty(dataNascimento))
                pagadorPadrao.DataNascimento = Convert.ToDateTime(dataNascimento);
            pagadorPadrao.Endereco = endereco;
            pagadorPadrao.Numero = numero;
            pagadorPadrao.Bairro = bairro;
            pagadorPadrao.Cidade = cidade;
            pagadorPadrao.Uf = uf;
            pagadorPadrao.Cep = cep;
        }

        public void BoletoAdicionarItem(string nome, int quantidade, decimal valor)
        {
            itensBoleto.Add(new ItemBoletoPadrao
            {
                Nome = nome,
                Quantidade = quantidade <= 0 ? 1 : quantidade,
                Valor = valor
            });
        }

        public void BoletoLimparItens()
        {
            itensBoleto.Clear();
        }

        public void BoletoIniciarNovo()
        {
            boletoPadrao = new BoletoPadrao();
            pagadorPadrao = new PagadorPadrao();
            itensBoletoAtual.Clear();
        }

        public string BoletoEmitir()
        {
            boletoPadrao.Pagador = pagadorPadrao;
            boletoPadrao.Itens = itensBoleto.Count == 0 ? null : itensBoleto.ToArray();
            var req = new BoletoEmissaoRequest { BoletoPadrao = boletoPadrao };
            var erro = ValidarEmissaoBoletos(req);
            if (erro != null) return Falha(erro);
            var banco = GetBanco();
            return Executar(() => banco.EmitirBoletoAsync(req), banco);
        }

        public string BoletosEmitir()
        {
            if (BancoID == 133) return FalhaCresol("CRESOL requer JSON CRESOL específico; use CresolTitulosIncluir.");
            if (boletosPadrao.Count == 0)
            {
                boletoPadrao.Pagador = pagadorPadrao;
                boletoPadrao.Itens = itensBoleto.Count == 0 ? null : itensBoleto.ToArray();
                boletosPadrao.Add(boletoPadrao);
            }
            var req = new BoletoEmissaoRequest { BoletosPadrao = boletosPadrao.ToArray() };
            var erro = ValidarEmissaoBoletos(req);
            if (erro != null) return Falha(erro);
            var banco = GetBanco();
            return Executar(() => banco.EmitirBoletoAsync(req), banco);
        }

        public void BoletoLimparConsulta()
        {
            boletoConsulta = new BoletoConsultaRequest();
        }

        public void BoletoInformarConsulta(string id, string nossoNumero, string linhaDigitavel, string codigoBarras, int modalidade, string dataInicio, string dataFim, bool segundaVia = false)
        {
            boletoConsulta.Id = id;
            boletoConsulta.NossoNumero = string.IsNullOrWhiteSpace(nossoNumero) || nossoNumero == "0" ? null : nossoNumero;
            boletoConsulta.LinhaDigitavel = linhaDigitavel;
            boletoConsulta.CodigoBarras = codigoBarras;
            boletoConsulta.Modalidade = modalidade == 0 ? (int?)null : modalidade;
            if (!string.IsNullOrEmpty(dataInicio))
                boletoConsulta.Inicio = Convert.ToDateTime(dataInicio);
            if (!string.IsNullOrEmpty(dataFim))
                boletoConsulta.Fim = Convert.ToDateTime(dataFim);
            boletoConsulta.SegundaVia = segundaVia;
        }

        public string BoletoConsultar()
        {
            var erro = ValidarConsultaBoleto(boletoConsulta);
            if (erro != null) return Falha(erro);
            var banco = GetBanco();
            return Executar(() => banco.ConsultarBoletoAsync(boletoConsulta), banco);
        }

        public string BoletoBaixar(int nossoNumero, int modalidade = 0)
        {
            if (BancoID != 756) return Falha("BoletoBaixar está disponível somente para o banco Sicoob (756)");
            if (nossoNumero <= 0) return Falha("nossoNumero é obrigatório para baixa Sicoob");
            var banco = GetBanco() as SicoobIntegracao;
            if (banco == null) return Falha("Não foi possível carregar a integração Sicoob");
            return Executar(() => banco.BaixarBoletoAsync(nossoNumero, modalidade == 0 ? (int?)null : modalidade), banco);
        }

        public string BoletoAlterar()
        {
            if (BancoID != 756) return Falha("BoletoAlterar está disponível somente para o banco Sicoob (756)");
            if (string.IsNullOrWhiteSpace(boletoPadrao.NossoNumero)) return Falha("Informe nossoNumero em BoletoDados antes de alterar o boleto");
            var banco = GetBanco() as SicoobIntegracao;
            if (banco == null) return Falha("Não foi possível carregar a integração Sicoob");
            return Executar(() => banco.AlterarBoletoAsync(boletoPadrao), banco);
        }

        #endregion

        #region Movimentação Sicoob
        public string MovimentacaoSolicitar(int tipoMovimento, string dataInicial, string dataFinal)
        {
            if (!DateTime.TryParse(dataInicial, out var inicio) || !DateTime.TryParse(dataFinal, out var fim)) return Falha("Informe dataInicial e dataFinal válidas");
            var banco = GetBanco() as SicoobIntegracao;
            if (banco == null) return Falha("Movimentação está disponível somente para o banco Sicoob (756)");
            var retorno = Executar(() => banco.SolicitarMovimentacaoAsync(tipoMovimento, inicio, fim), banco);
            AtualizarMovimentacao();
            return retorno;
        }

        public string MovimentacaoConsultar(int codigoSolicitacao)
        {
            var banco = GetBanco() as SicoobIntegracao;
            if (banco == null) return Falha("Movimentação está disponível somente para o banco Sicoob (756)");
            var retorno = Executar(() => banco.ConsultarMovimentacaoAsync(codigoSolicitacao), banco);
            AtualizarMovimentacao();
            return retorno;
        }

        public string MovimentacaoBaixar(int codigoSolicitacao, int idArquivo)
        {
            var banco = GetBanco() as SicoobIntegracao;
            if (banco == null) return Falha("Movimentação está disponível somente para o banco Sicoob (756)");
            var retorno = Executar(() => banco.BaixarMovimentacaoAsync(codigoSolicitacao, idArquivo), banco);
            AtualizarMovimentacao();
            SalvarArquivoMovimentacao();
            DescompactarArquivoMovimentacao();
            return retorno;
        }
        #endregion

        #region Métodos de PIX

        public void PixLimpar()
        {
            pixCriacao = new PixCriacaoRequest();
            pixConsulta = new PixConsultaRequest();
        }

        public void PixDados(string chave, decimal valor, int expiracao, string solicitacaoPagador, string nomeDevedor, string documentoDevedor, string txId)
        {
            pixCriacao.Chave = chave;
            pixCriacao.Valor = valor;
            pixCriacao.Expiracao = expiracao == 0 ? (int?)null : expiracao;
            pixCriacao.SolicitacaoPagador = solicitacaoPagador;
            pixCriacao.NomeDevedor = nomeDevedor;
            pixCriacao.DocumentoDevedor = documentoDevedor;
            pixCriacao.TxId = txId;
        }

        public void PixConsultar(string txId, string endToEndId, string dataInicio, string dataFim)
        {
            pixConsulta.TxId = txId;
            pixConsulta.EndToEndId = endToEndId;
            if (!string.IsNullOrEmpty(dataInicio))
                pixConsulta.Inicio = Convert.ToDateTime(dataInicio);
            if (!string.IsNullOrEmpty(dataFim))
                pixConsulta.Fim = Convert.ToDateTime(dataFim);
        }

        public string PixCriar()
        {
            var erro = ValidarPixCriacao(pixCriacao);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().CriarPixAsync(pixCriacao));
        }

        public string PixConsultar()
        {
            var erro = ValidarPixConsulta(pixConsulta);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarPixAsync(pixConsulta));
        }

        public bool PixExibirQRCode(string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo)) return false;
            FrmPixQRCode.Exibir(conteudo);
            return true;
        }

        public bool PixExibirUltimoQRCode()
        {
            return PixExibirQRCode(QRCode);
        }

        #endregion

        #region Métodos de Extrato
        public void ExtratoLimpar()
        {
            extratoConsulta = new ExtratoRequest();
        }

        public void ExtratoPorPeriodo(string dataInicio, string dataFim)
        {
            if (!string.IsNullOrEmpty(dataInicio))
                extratoConsulta.Inicio = Convert.ToDateTime(dataInicio);
            if (!string.IsNullOrEmpty(dataFim))
                extratoConsulta.Fim = Convert.ToDateTime(dataFim);
        }

        public void ExtratoPorMes(int mes, int ano, int diaInicial, int diaFinal)
        {
            extratoConsulta.Mes = mes == 0 ? (int?)null : mes;
            extratoConsulta.Ano = ano == 0 ? (int?)null : ano;
            extratoConsulta.DiaInicial = diaInicial == 0 ? (int?)null : diaInicial;
            extratoConsulta.DiaFinal = diaFinal == 0 ? (int?)null : diaFinal;
        }

        public void ExtratoArquivo(string nomeArquivo)
        {
            extratoConsulta.NomeArquivo = nomeArquivo;
        }

        public string ExtratoConsultar()
        {
            var erro = ValidarExtrato(extratoConsulta);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarExtratoAsync(extratoConsulta));
        }

        #endregion

        #region Métodos de Saldo
        public void SaldoLimpar()
        {
            saldoConsulta = new SaldoRequest();
        }

        public string SaldoConsultar()
        {
            var erro = ValidarSaldo(saldoConsulta);
            if (erro != null) return Falha(erro);
            return Executar(() => GetBanco().ConsultarSaldoAsync(saldoConsulta));
        }

        #endregion

        #endregion

        #region Métodos privados
        private IBancoHibrido GetBanco()
        {
            if (BancoID == 756)
            {
                return new SicoobIntegracao(new SicoobConfiguracao
                {
                    ClientId = ClientID,
                    Sandbox = homologacao,
                    Certificado = ClientCertificado,
                    CertificadoBase64 = ClientCertificadoBase64,
                    CertificadoSenha = ClientCertificadoSenha,
                    NumeroContrato = NumeroContrato,
                    NumeroContaCorrente = NumeroContaCorrente
                });
            }

            if (BancoID == 364)
            {
                return new EfiIntegracao(new EfiConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    Sandbox = homologacao,
                    Certificado = ClientCertificado,
                    CertificadoBase64 = ClientCertificadoBase64
                });
            }

            if (BancoID == 461)
            {
                return new AsaasIntegracao(new AsaasConfiguracao
                {
                    ApiKey = string.IsNullOrWhiteSpace(AsaasApiKey) ? ClientSecret : AsaasApiKey,
                    Sandbox = homologacao,
                    UserAgent = AsaasUserAgent
                });
            }

            if (BancoID == 748)
            {
                return new SicrediIntegracao(new SicrediConfiguracao
                {
                    ApiKey = SicrediApiKey,
                    CodigoAcesso = SicrediCodigoAcesso,
                    CodigoBeneficiario = SicrediCodigoBeneficiario,
                    Cooperativa = SicrediCooperativa,
                    Posto = SicrediPosto,
                    Sandbox = homologacao
                });
            }

            if (BancoID == 97)
            {
                return new CredisisIntegracao(new CredisisConfiguracao
                {
                    Token = CredisisToken,
                    Convenio = CredisisConvenio,
                    Url = CredisisUrl
                });
            }

            if (BancoID == 1)
            {
                return new BBIntegracao(new BBConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    AppKey = BBAppKey,
                    NumeroConvenio = NumeroContrato,
                    Sandbox = homologacao,
                    Certificado = ClientCertificado,
                    CertificadoBase64 = ClientCertificadoBase64,
                    CertificadoSenha = ClientCertificadoSenha
                });
            }

            if (BancoID == 237)
            {
                return new BradescoIntegracao(new BradescoConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    CodigoUsuario = BradescoCodigoUsuario,
                    CpfCnpjBeneficiario = BradescoCpfCnpjBeneficiario,
                    FilialBeneficiario = BradescoFilialBeneficiario,
                    ControleBeneficiario = BradescoControleBeneficiario,
                    Produto = BradescoProduto,
                    Negociacao = BradescoNegociacao,
                    Agencia = BradescoAgencia,
                    ContaCorrente = string.IsNullOrWhiteSpace(BradescoContaCorrente) ? NumeroContaCorrente.ToString() : BradescoContaCorrente,
                    TipoConta = BradescoTipoConta,
                    Sandbox = homologacao,
                    Certificado = ClientCertificado,
                    CertificadoBase64 = ClientCertificadoBase64,
                    CertificadoSenha = ClientCertificadoSenha
                });
            }

            if (BancoID == 341)
            {
                return new ItauIntegracao(new ItauConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    ApiKey = ItauApiKey,
                    IdBeneficiario = ItauIdBeneficiario,
                    CodigoCarteira = ItauCodigoCarteira,
                    Agencia = ItauAgencia,
                    Conta = ItauConta,
                    Dac = ItauDac,
                    FrancesaId = ItauFrancesaId,
                    PixChave = PIXChave,
                    Sandbox = homologacao,
                    UrlAutenticacao = ItauUrlAutenticacao,
                    BaseUrlPix = ItauBaseUrlPix,
                    BaseUrlRecebimentos = ItauBaseUrlRecebimentos,
                    BaseUrlExtrato = ItauBaseUrlExtrato,
                    BaseUrlBoletosCash = ItauBaseUrlBoletosCash
                });
            }

            if (BancoID == 33)
            {
                return new SantanderIntegracao(new SantanderConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    WorkspaceId = SantanderWorkspaceId,
                    CovenantCode = SantanderCovenantCode,
                    BankNumber = SantanderBankNumber,
                    ParticipantCode = SantanderParticipantCode,
                    PixChave = SantanderPixChave,
                    PixChaveTipo = SantanderPixChaveTipo,
                    Sandbox = homologacao,
                    BaseUrl = SantanderBaseUrl,
                    UrlAutenticacao = SantanderUrlAutenticacao,
                    CnpjBanco = SantanderCnpjBanco,
                    AgenciaConta = SantanderAgenciaConta,
                    Certificado = string.IsNullOrWhiteSpace(CertificadoDigitalBase64) ? ClientCertificado : "",
                    CertificadoBase64 = string.IsNullOrWhiteSpace(CertificadoDigitalBase64) ? ClientCertificadoBase64 : CertificadoDigitalBase64,
                    CertificadoSenha = string.IsNullOrWhiteSpace(CertificadoDigitalSenha) ? ClientCertificadoSenha : CertificadoDigitalSenha
                });
            }

            if (BancoID == 41)
            {
                return new BanrisulIntegracao(new BanrisulConfiguracao
                {
                    ClientId = ClientID,
                    ClientSecret = ClientSecret,
                    Beneficiario = BanrisulBeneficiario,
                    PartnerId = BanrisulPartnerId,
                    CoPartnerId = BanrisulCoPartnerId,
                    Sandbox = homologacao,
                    Certificado = ClientCertificado,
                    CertificadoBase64 = ClientCertificadoBase64,
                    CertificadoSenha = ClientCertificadoSenha,
                    UrlAutenticacao = BanrisulUrlAutenticacao,
                    UrlCobranca = BanrisulUrlCobranca,
                    UrlPagamentos = BanrisulUrlPagamentos,
                    UrlPix = BanrisulUrlPix,
                    UrlAutenticacaoPix = BanrisulUrlAutenticacaoPix
                });
            }

            if (BancoID == 133)
                return ObterCresolIntegracao();

            throw new ArgumentException("BancoID inválido");
        }

        private string Executar(Func<Task<BancoResposta>> operacao, IBancoHibrido banco = null, [CallerMemberName] string nomeOperacao = null)
        {
            LimparRetorno();
            try
            {
                // A chamada COM/VB6 é síncrona; executar o fluxo async fora do contexto COM evita deadlock.
                var resposta = Task.Run(operacao).GetAwaiter().GetResult();
                if (banco is SicoobIntegracao sicoob) JsonEnvio = sicoob.JsonEnvio ?? "";
                if (banco is BradescoIntegracao bradesco) JsonEnvio = bradesco.JsonEnvio ?? "";
                if (banco is ItauIntegracao itau) JsonEnvio = itau.JsonEnvio ?? "";
                if (banco is SantanderIntegracao santander) JsonEnvio = santander.JsonEnvio ?? "";
                if (banco is CredisisIntegracao credisis) JsonEnvio = credisis.JsonEnvio ?? "";
                AtualizarRetorno(resposta);
                SalvarArquivoPDF(resposta);
                JsonRetorno = JsonConvert.SerializeObject(resposta);
                Log.RegistrarOperacao(HabilitarLog, CaminhoLog, BancoID, nomeOperacao, resposta, DetalharErros, JsonEnvio, JsonRetorno);
                return JsonRetorno;
            }
            catch (Exception ex)
            {
                var erro = new BancoResposta { Sucesso = false, Mensagem = DetalharErros ? ex.ToString() : MensagemErro(ex) };
                JsonRetorno = JsonConvert.SerializeObject(erro);
                Log.RegistrarExcecao(HabilitarLog, CaminhoLog, BancoID, nomeOperacao, ex, DetalharErros, JsonEnvio);
                return JsonRetorno;
            }
        }

        private static string MensagemErro(Exception ex)
        {
            var mensagens = new List<string>();
            while (ex != null)
            {
                var mensagem = ex.Message?.Trim();
                if (!string.IsNullOrWhiteSpace(mensagem) && !mensagens.Contains(mensagem)) mensagens.Add(mensagem);
                ex = ex.InnerException;
            }
            return mensagens.Count == 0 ? "Erro inesperado ao executar a operação bancária" : string.Join(" | ", mensagens);
        }

        private string Falha(string mensagem, [CallerMemberName] string nomeOperacao = null)
        {
            LimparRetorno();
            var erro = new BancoResposta { Sucesso = false, Mensagem = mensagem };
            JsonRetorno = JsonConvert.SerializeObject(erro);
            Log.RegistrarFalha(HabilitarLog, CaminhoLog, BancoID, nomeOperacao, mensagem);
            return JsonRetorno;
        }

        private static JToken ValidarJsonPayload(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Payload JSON é obrigatório", nameof(json));
            return JToken.Parse(json);
        }

        private void AtualizarRetorno(BancoResposta resposta)
        {
            Sucesso = resposta?.Sucesso == true;
            if (!Sucesso) return;

            var dados = resposta.Dados;
            var resultado = dados is JObject objetoDados ? objetoDados["resultado"] ?? dados : dados;
            if (resultado is JArray itens && itens.Count > 0) resultado = itens[0];
            if (!(resultado is JObject objeto)) return;

            var titulo = objeto;
            if (titulo["linhaDigitavel"] == null && titulo["linha_digitavel"] == null && titulo["nossonumero"] == null)
            {
                JToken tituloRetornado = null;
                if (dados != null)
                    foreach (var candidato in dados.SelectTokens("$..titulos.item"))
                    {
                        tituloRetornado = candidato;
                        break;
                    }
                if (tituloRetornado is JArray itensRetornados && itensRetornados.Count > 0) tituloRetornado = itensRetornados[0];
                if (tituloRetornado is JObject) titulo = (JObject)tituloRetornado;
            }

            SeuNumero = titulo.Value<string>("seuNumero") ?? titulo.Value<string>("numeroTituloBeneficiario") ?? titulo.Value<string>("clientNumber") ?? titulo.Value<string>("documento") ?? "";
            NossoNumero = titulo.Value<string>("nossoNumero") ?? titulo.Value<string>("nossonumero") ?? titulo.Value<string>("numero") ?? "";
            CodigoBoleto = titulo.Value<string>("codigoBoleto") ?? titulo.Value<string>("numeroSequencial") ?? NossoNumero;
            LinhaDigitavel = titulo.Value<string>("linhaDigitavel") ?? titulo.Value<string>("linha_digitavel") ?? titulo.Value<string>("digitableLine") ?? "";
            CodigoBarras = titulo.Value<string>("codigoBarras") ?? titulo.Value<string>("codigoBarraNumerico") ?? titulo.Value<string>("codigo_barras") ?? titulo.Value<string>("barcode") ?? "";
            var pixCopiaCola = titulo.Value<string>("pixCopiaCola");
            PIXCopiaCola = pixCopiaCola ?? titulo.Value<string>("pixCopiaECola") ?? "";
            var qrCode = titulo["qrCode"];
            QRCode = qrCode?.Type == JTokenType.String ? qrCode.Value<string>() : titulo.Value<string>("codigoQrCode") ?? titulo.Value<string>("pixCopiaECola") ?? pixCopiaCola ?? titulo.Value<string>("qrCodePix") ?? titulo.Value<string>("emv") ?? qrCode?["emv"]?.Value<string>() ?? "";
            pdfBoleto = titulo.Value<string>("pdfBoleto") ?? EncontrarPdfBase64(dados) ?? "";
        }

        private void SalvarArquivoPDF(BancoResposta resposta)
        {
            ArquivoPDFGerado = "";
            MensagemArquivoPDF = "";
            if (!CriarArquivoPDF || resposta?.Sucesso != true) return;
            if (string.IsNullOrWhiteSpace(pdfBoleto))
            {
                MensagemArquivoPDF = "A API não retornou o PDF em Base64 (pdfBoleto/boleto)";
                return;
            }
            if (string.IsNullOrWhiteSpace(CaminhoArquivoPDF))
            {
                MensagemArquivoPDF = "CaminhoArquivoPDF não informado";
                return;
            }
            try
            {
                var base64 = pdfBoleto.Trim();
                var separador = base64.IndexOf(',');
                if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && separador >= 0) base64 = base64.Substring(separador + 1);
                var nossoNumero = ObterNossoNumero(resposta.Dados);
                if (string.IsNullOrWhiteSpace(nossoNumero)) nossoNumero = !string.IsNullOrWhiteSpace(NossoNumero) ? NossoNumero : "boleto";
                Directory.CreateDirectory(CaminhoArquivoPDF);
                var identificadorConta = BancoID == 97 && !string.IsNullOrWhiteSpace(CredisisConvenio) ? CredisisConvenio : NumeroContaCorrente.ToString();
                identificadorConta = SanitizarNomeArquivo(identificadorConta);
                ArquivoPDFGerado = Path.Combine(CaminhoArquivoPDF, identificadorConta + "-" + SanitizarNomeArquivo(nossoNumero) + ".pdf");
                File.WriteAllBytes(ArquivoPDFGerado, Convert.FromBase64String(base64));
            }
            catch (Exception ex)
            {
                ArquivoPDFGerado = "";
                MensagemArquivoPDF = "Não foi possível gravar o PDF: " + ex.Message;
            }
        }

        private static string EncontrarPdfBase64(JToken dados)
        {
            if (dados == null) return null;
            foreach (var candidato in dados.SelectTokens("$..boleto"))
            {
                if (candidato.Type != JTokenType.String) continue;
                var valor = candidato.Value<string>();
                if (!string.IsNullOrWhiteSpace(valor) && (valor.TrimStart().StartsWith("JVBERi", StringComparison.Ordinal) || valor.TrimStart().StartsWith("data:application/pdf", StringComparison.OrdinalIgnoreCase))) return valor;
            }
            return null;
        }

        private static string SanitizarNomeArquivo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return "boleto";
            foreach (var caractere in Path.GetInvalidFileNameChars()) valor = valor.Replace(caractere, '_');
            return valor;
        }

        private void AtualizarMovimentacao()
        {
            CodigoSolicitacaoMovimentacao = 0;
            QuantidadeArquivosMovimentacao = 0;
            IdArquivosMovimentacao = "";
            ArquivoMovimentacaoGerado = "";
            MensagemArquivoMovimentacao = "";
            if (!Sucesso || string.IsNullOrWhiteSpace(JsonRetorno)) return;
            try
            {
                var resultado = JObject.Parse(JsonRetorno).SelectToken("Dados.resultado") as JObject;
                if (resultado == null) return;
                CodigoSolicitacaoMovimentacao = resultado.Value<int?>("codigoSolicitacao") ?? 0;
                QuantidadeArquivosMovimentacao = resultado.Value<int?>("quantidadeArquivo") ?? 0;
                if (resultado["idArquivos"] != null) IdArquivosMovimentacao = resultado["idArquivos"].ToString(Formatting.None);
            }
            catch (Exception ex) { MensagemArquivoMovimentacao = "Não foi possível interpretar o retorno da movimentação: " + ex.Message; }
        }

        private void SalvarArquivoMovimentacao()
        {
            if (!Sucesso || string.IsNullOrWhiteSpace(JsonRetorno)) return;
            try
            {
                var resultado = JObject.Parse(JsonRetorno).SelectToken("Dados.resultado") as JObject;
                var arquivo = resultado?.Value<string>("arquivo");
                var nomeArquivo = resultado?.Value<string>("nomeArquivo");
                if (string.IsNullOrWhiteSpace(arquivo) || string.IsNullOrWhiteSpace(nomeArquivo)) return;
                if (string.IsNullOrWhiteSpace(CaminhoArquivosMovimentacao))
                {
                    MensagemArquivoMovimentacao = "CaminhoArquivosMovimentacao não informado";
                    return;
                }
                Directory.CreateDirectory(CaminhoArquivosMovimentacao);
                ArquivoMovimentacaoGerado = Path.Combine(CaminhoArquivosMovimentacao, Path.GetFileName(nomeArquivo));
                File.WriteAllBytes(ArquivoMovimentacaoGerado, Convert.FromBase64String(arquivo));
            }
            catch (Exception ex)
            {
                ArquivoMovimentacaoGerado = "";
                MensagemArquivoMovimentacao = "Não foi possível gravar o arquivo de movimentação: " + ex.Message;
            }
        }

        private void DescompactarArquivoMovimentacao()
        {
            JsonArquivosMovimentacao = "[]";
            NomesArquivosMovimentacao = new string[0];
            ConteudosArquivosMovimentacao = new string[0];
            if (!Sucesso || string.IsNullOrWhiteSpace(JsonRetorno)) return;
            try
            {
                var resultado = JObject.Parse(JsonRetorno).SelectToken("Dados.resultado") as JObject;
                var base64 = resultado?.Value<string>("arquivo");
                if (string.IsNullOrWhiteSpace(base64)) return;
                var nomes = new List<string>();
                var conteudos = new List<string>();
                var arquivosJson = new JArray();
                using (var memoria = new MemoryStream(Convert.FromBase64String(base64)))
                using (var zip = new ZipArchive(memoria, ZipArchiveMode.Read))
                {
                    foreach (var entrada in zip.Entries)
                    {
                        if (string.IsNullOrWhiteSpace(entrada.Name)) continue;
                        string conteudo;
                        using (var leitor = new StreamReader(entrada.Open(), Encoding.UTF8)) conteudo = leitor.ReadToEnd();
                        nomes.Add(entrada.Name);
                        conteudos.Add(conteudo);
                        try { arquivosJson.Add(new JObject { ["nomeArquivo"] = entrada.Name, ["conteudo"] = JToken.Parse(conteudo) }); }
                        catch { arquivosJson.Add(new JObject { ["nomeArquivo"] = entrada.Name, ["conteudo"] = conteudo }); }
                        if (!string.IsNullOrWhiteSpace(CaminhoArquivosMovimentacao))
                        {
                            Directory.CreateDirectory(CaminhoArquivosMovimentacao);
                            File.WriteAllText(Path.Combine(CaminhoArquivosMovimentacao, Path.GetFileName(entrada.Name)), conteudo, Encoding.UTF8);
                        }
                    }
                }
                NomesArquivosMovimentacao = nomes.ToArray();
                ConteudosArquivosMovimentacao = conteudos.ToArray();
                JsonArquivosMovimentacao = arquivosJson.ToString(Formatting.None);
            }
            catch (Exception ex)
            {
                MensagemArquivoMovimentacao = "Não foi possível descompactar o arquivo de movimentação: " + ex.Message;
            }
        }

        private static string ObterNossoNumero(JToken dados)
        {
            var resultado = dados?["resultado"] ?? dados;
            if (resultado is JArray itens && itens.Count > 0) resultado = itens[0];
            return resultado?["nossoNumero"]?.ToString() ?? resultado?["numero"]?.ToString() ?? resultado?["numero_nosso_numero"]?.ToString();
        }

        private void LimparRetorno()
        {
            Sucesso = false;
            SeuNumero = "";
            NossoNumero = "";
            CodigoBoleto = "";
            LinhaDigitavel = "";
            CodigoBarras = "";
            PIXCopiaCola = "";
            QRCode = "";
            pdfBoleto = "";
            ArquivoPDFGerado = "";
            MensagemArquivoPDF = "";
            JsonRetorno = "";
            CodigoSolicitacaoMovimentacao = 0;
            QuantidadeArquivosMovimentacao = 0;
            IdArquivosMovimentacao = "";
            ArquivoMovimentacaoGerado = "";
            MensagemArquivoMovimentacao = "";
            JsonArquivosMovimentacao = "[]";
            NomesArquivosMovimentacao = new string[0];
            ConteudosArquivosMovimentacao = new string[0];
        }

        #endregion

        #region Validações
        private string ValidarEmissaoBoletos(BoletoEmissaoRequest req)
        {
            if (req == null) return "Requisição inválida";
            if (req.BoletosPadrao != null && req.BoletosPadrao.Length > 0)
            {
                for (var i = 0; i < req.BoletosPadrao.Length; i++)
                {
                    var msg = ValidarBoleto(req.BoletosPadrao[i]);
                    if (msg != null) return $"Boleto[{i}]: {msg}";
                }
                return null;
            }
            if (req.BoletoPadrao != null) return ValidarBoleto(req.BoletoPadrao);
            return "BoletoPadrao ou BoletosPadrao é obrigatório";
        }

        private string ValidarBoleto(BoletoPadrao boleto)
        {
            if (boleto == null) return "Dados do boleto não informados";
            if (boleto.Valor <= 0) return "Valor inválido";
            if (boleto.Vencimento == default(DateTime)) return "Vencimento inválido";
            if (boleto.Pagador == null) return "Pagador não informado";
            if (string.IsNullOrWhiteSpace(boleto.Pagador.Nome)) return "Pagador.Nome é obrigatório";
            if (string.IsNullOrWhiteSpace(boleto.Pagador.Documento)) return "Pagador.Documento é obrigatório";
            AplicarLimitesBanco(boleto);
            return null;
        }

        private void AplicarLimitesBanco(BoletoPadrao boleto)
        {
            if (BancoID == 748) AplicarLimitesSicredi(boleto);
        }

        private static void AplicarLimitesSicredi(BoletoPadrao boleto)
        {
            boleto.NossoNumero = SicrediFormatacao.MascararNossoNumero(boleto.NossoNumero);
            boleto.SeuNumero = Truncar(boleto.SeuNumero, 10);
            boleto.NumeroDocumento = Truncar(boleto.NumeroDocumento, 10);
            boleto.Instrucao1 = Truncar(boleto.Instrucao1, 80);
            boleto.Instrucao2 = Truncar(boleto.Instrucao2, 80);
            boleto.Instrucao3 = Truncar(boleto.Instrucao3, 80);

            var pagador = boleto.Pagador;
            pagador.Nome = Truncar(pagador.Nome, 40);
            pagador.Documento = TruncarDigitos(pagador.Documento, 14);
            pagador.Email = Truncar(pagador.Email, 40);
            pagador.Telefone = TruncarDigitos(pagador.Telefone, 11);
            pagador.Endereco = Truncar(pagador.Endereco, 40);
            pagador.Cidade = Truncar(pagador.Cidade, 40);
            pagador.Uf = Truncar(pagador.Uf, 2);
            pagador.Cep = TruncarDigitos(pagador.Cep, 8);
        }

        private static string Truncar(string valor, int limite)
        {
            if (string.IsNullOrEmpty(valor) || valor.Length <= limite) return valor;
            return valor.Substring(0, limite);
        }

        private static string TruncarDigitos(string valor, int limite)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            var resultado = new StringBuilder(limite);
            foreach (var caractere in valor)
            {
                if (!char.IsDigit(caractere)) continue;
                resultado.Append(caractere);
                if (resultado.Length == limite) break;
            }
            return resultado.Length == 0 ? valor : resultado.ToString();
        }

        private string ValidarPixCriacao(PixCriacaoRequest req)
        {
            if (req == null) return "Requisição inválida";
            if (string.IsNullOrWhiteSpace(req.Chave)) return "Chave do PIX é obrigatória";
            if (!req.Valor.HasValue || req.Valor.Value <= 0) return "Valor do PIX inválido";
            return null;
        }

        private string ValidarPixConsulta(PixConsultaRequest req)
        {
            if (req == null) return "Requisição inválida";
            if (!string.IsNullOrWhiteSpace(req.TxId)) return null;
            if (!string.IsNullOrWhiteSpace(req.EndToEndId)) return null;
            if (req.Inicio.HasValue || req.Fim.HasValue) return null;
            return "Informe TxId, EndToEndId ou período";
        }

        private string ValidarExtrato(ExtratoRequest req)
        {
            if (req == null) return "Requisição inválida";
            if (!string.IsNullOrWhiteSpace(req.NomeArquivo)) return null;
            if (req.Mes.HasValue && req.Ano.HasValue) return null;
            if (req.Inicio.HasValue || req.Fim.HasValue) return null;
            return "Informe NomeArquivo, Mes/Ano ou período";
        }

        private string ValidarSaldo(SaldoRequest req)
        {
            if (req == null) return "Requisição inválida";
            return null;
        }

        private string ValidarConsultaBoleto(BoletoConsultaRequest req)
        {
            if (req == null) return "Requisição inválida";
            if (!string.IsNullOrWhiteSpace(req.Id)) return null;
            if (!string.IsNullOrWhiteSpace(req.NossoNumero)) return null;
            if (!string.IsNullOrWhiteSpace(req.LinhaDigitavel)) return null;
            if (!string.IsNullOrWhiteSpace(req.CodigoBarras)) return null;
            if (req.Inicio.HasValue || req.Fim.HasValue) return null;
            return "Informe Id, NossoNumero, LinhaDigitavel, CodigoBarras ou período";
        }

        private string _cresolAccountId = "";
        private string _cresolCooperativa = "";
        private string _cresolUsuario = "";
        private string _cresolSenha = "";
        private int _cresolIdEspecie;

        string ICresol.CresolAccountId { get => _cresolAccountId; set => _cresolAccountId = value; }
        string ICresol.CresolCooperativa { get => _cresolCooperativa; set => _cresolCooperativa = value; }
        string ICresol.CresolUsuario { get => _cresolUsuario; set => _cresolUsuario = value; }
        string ICresol.CresolSenha { get => _cresolSenha; set => _cresolSenha = value; }
        int ICresol.CresolIdEspecie { get => _cresolIdEspecie; set => _cresolIdEspecie = value; }

        string ICresol.CresolTitulosListar(string filtrosJson)
            => ExecutarCresol(integracao => integracao.TitulosListarAsync(filtrosJson), nameof(ICresol.CresolTitulosListar));

        string ICresol.CresolTituloConsultar(string id)
            => ExecutarCresol(integracao => integracao.TituloConsultarAsync(id), nameof(ICresol.CresolTituloConsultar));

        string ICresol.CresolTitulosIncluir(string json)
            => ExecutarCresol(integracao => integracao.TitulosIncluirAsync(json), nameof(ICresol.CresolTitulosIncluir));

        string ICresol.CresolTituloAlterar(string id, string json)
            => ExecutarCresol(integracao => integracao.TituloAlterarAsync(id, json), nameof(ICresol.CresolTituloAlterar));

        string ICresol.CresolTituloBaixar(string id)
            => ExecutarCresol(integracao => integracao.TituloBaixarAsync(id), nameof(ICresol.CresolTituloBaixar));

        string ICresol.CresolPagadoresListar(string filtrosJson)
            => ExecutarCresol(integracao => integracao.PagadoresListarAsync(filtrosJson), nameof(ICresol.CresolPagadoresListar));

        string ICresol.CresolPagadorConsultar(string id)
            => ExecutarCresol(integracao => integracao.PagadorConsultarAsync(id), nameof(ICresol.CresolPagadorConsultar));

        string ICresol.CresolPagadorIncluir(string json)
            => ExecutarCresol(integracao => integracao.PagadorIncluirAsync(json), nameof(ICresol.CresolPagadorIncluir));

        string ICresol.CresolPagadorAlterar(string id, string json)
            => ExecutarCresol(integracao => integracao.PagadorAlterarAsync(id, json), nameof(ICresol.CresolPagadorAlterar));

        string ICresol.CresolPagadorExcluir(string id)
            => ExecutarCresol(integracao => integracao.PagadorExcluirAsync(id), nameof(ICresol.CresolPagadorExcluir));

        string ICresol.CresolOcorrenciasSequenciais(string dateStart, string dateEnd)
            => ExecutarCresol(integracao => integracao.OcorrenciasSequenciaisAsync(dateStart, dateEnd), nameof(ICresol.CresolOcorrenciasSequenciais));

        string ICresol.CresolOcorrenciaConsultar(string sequencial)
            => ExecutarCresol(integracao => integracao.OcorrenciaConsultarAsync(sequencial), nameof(ICresol.CresolOcorrenciaConsultar));

        private CresolIntegracao ObterCresolIntegracao()
        {
            if (_cresolConfiguracao == null)
                _cresolConfiguracao = new CresolConfiguracao();

            _cresolConfiguracao.AccountId = _cresolAccountId;
            _cresolConfiguracao.Cooperativa = _cresolCooperativa;
            _cresolConfiguracao.Usuario = _cresolUsuario;
            _cresolConfiguracao.Senha = _cresolSenha;
            _cresolConfiguracao.IdEspecie = _cresolIdEspecie;

            if (_cresolIntegracao == null)
                _cresolIntegracao = new CresolIntegracao(_cresolConfiguracao);

            return _cresolIntegracao;
        }

        private string ExecutarCresol(Func<CresolIntegracao, Task<BancoResposta>> operacao, string nomeOperacao)
        {
            JsonEnvio = "";
            if (BancoID != 133)
                return Falha("BancoID deve ser 133 para operações CRESOL.", nomeOperacao);

            var integracao = ObterCresolIntegracao();
            return Executar(() => operacao(integracao), integracao, nomeOperacao);
        }

        private string FalhaCresol(string mensagem, [CallerMemberName] string nomeOperacao = null)
        {
            JsonEnvio = "";
            return Falha(mensagem, nomeOperacao);
        }

        #endregion

    }
}
