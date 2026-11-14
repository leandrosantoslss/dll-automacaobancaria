using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos
{
    public class BancoOperacaoRequest
    {
        public string Operacao { get; set; }
        public JObject Parametros { get; set; }
        public JToken Corpo { get; set; }
        public JObject Cabecalhos { get; set; }

        public BancoRequisicao ToRequisicao()
            => new BancoRequisicao { Operacao = Operacao, Parametros = Parametros, Corpo = Corpo, Cabecalhos = Cabecalhos };
    }

    public class BoletoEmissaoRequest : BancoOperacaoRequest
    {
        public JToken Boletos { get; set; }
        public BoletoPadrao BoletoPadrao { get; set; }
        public BoletoPadrao[] BoletosPadrao { get; set; }
    }

    public class BoletoConsultaRequest : BancoOperacaoRequest
    {
        public string Id { get; set; }
        public string NossoNumero { get; set; }
        public string LinhaDigitavel { get; set; }
        public string CodigoBarras { get; set; }
        public int? Modalidade { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public bool SegundaVia { get; set; }
    }

    public class PixCriacaoRequest : BancoOperacaoRequest
    {
        public string Chave { get; set; }
        public decimal? Valor { get; set; }
        public int? Expiracao { get; set; }
        public string SolicitacaoPagador { get; set; }
        public string NomeDevedor { get; set; }
        public string DocumentoDevedor { get; set; }
        public string TxId { get; set; }
    }

    public class PixConsultaRequest : BancoOperacaoRequest
    {
        public string TxId { get; set; }
        public string EndToEndId { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fim { get; set; }
    }

    public class ExtratoRequest : BancoOperacaoRequest
    {
        public int? Mes { get; set; }
        public int? Ano { get; set; }
        public int? DiaInicial { get; set; }
        public int? DiaFinal { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public string NomeArquivo { get; set; }
    }

    public class SaldoRequest : BancoOperacaoRequest
    {
    }

    public class BoletoPadrao
    {
        public string NossoNumero { get; set; }
        public string SeuNumero { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal Valor { get; set; }
        public DateTime Vencimento { get; set; }
        public DateTime? DataLimitePagamento { get; set; }
        public DateTime? DataPrimeiroDesconto { get; set; }
        public int? TipoDesconto { get; set; }
        public decimal? ValorDesconto { get; set; }
        public int? TipoMulta { get; set; }
        public DateTime? DataMulta { get; set; }
        public decimal? ValorMulta { get; set; }
        public int? TipoJuros { get; set; }
        public DateTime? DataJuros { get; set; }
        public decimal? ValorJuros { get; set; }
        public int? NumeroParcela { get; set; }
        public int? CodigoProtesto { get; set; }
        public int? NumeroDiasProtesto { get; set; }
        public int? Modalidade { get; set; }
        /// <summary>Identifica quem emite o boleto no Sicoob. O padrão é 2 (Cliente Emite).</summary>
        public int? IdentificacaoEmissaoBoleto { get; set; }
        /// <summary>Identifica quem distribui o boleto no Sicoob. O padrão é 2 (Cliente Distribui).</summary>
        public int? IdentificacaoDistribuicaoBoleto { get; set; }
        public bool? GerarPdf { get; set; }
        public bool? CadastrarPix { get; set; }
        public string Instrucao1 { get; set; }
        public string Instrucao2 { get; set; }
        public string Instrucao3 { get; set; }
        public PagadorPadrao Pagador { get; set; }
        public ItemBoletoPadrao[] Itens { get; set; }
    }

    public class PagadorPadrao
    {
        public string Nome { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
    }

    public class ItemBoletoPadrao
    {
        public string Nome { get; set; }
        public int Quantidade { get; set; } = 1;
        public decimal Valor { get; set; }
    }

    public interface IBancoHibrido
    {
        int CodigoBanco { get; }
        string Nome { get; }
        BancoCapacidades Capacidades { get; }
        Task<BancoResposta> EmitirBoletoAsync(BoletoEmissaoRequest requisicao);
        Task<BancoResposta> ConsultarBoletoAsync(BoletoConsultaRequest requisicao);
        Task<BancoResposta> CriarPixAsync(PixCriacaoRequest requisicao);
        Task<BancoResposta> ConsultarPixAsync(PixConsultaRequest requisicao);
        Task<BancoResposta> ConsultarExtratoAsync(ExtratoRequest requisicao);
        Task<BancoResposta> ConsultarSaldoAsync(SaldoRequest requisicao);
    }
}
