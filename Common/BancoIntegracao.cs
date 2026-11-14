using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace AutomacaoBancaria.Bancos
{
    [Flags]
    public enum BancoCapacidades
    {
        Nenhuma = 0,
        Boleto = 1,
        Pix = 2,
        Extrato = 4,
        Saldo = 8,
        Webhook = 16,
        Outros = 32
    }

    public class BancoRequisicao
    {
        public string Operacao { get; set; }
        public JObject Parametros { get; set; }
        public JToken Corpo { get; set; }
        public JObject Cabecalhos { get; set; }
    }

    public class BancoResposta
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public JToken Dados { get; set; }
        public int? StatusCode { get; set; }

        public static BancoResposta Ok(JToken dados)
            => new BancoResposta { Sucesso = true, Dados = dados };

        public static BancoResposta Falha(string mensagem, int? statusCode = null)
            => new BancoResposta { Sucesso = false, Mensagem = mensagem, StatusCode = statusCode };
    }

    public interface IBancoIntegracao
    {
        int CodigoBanco { get; }
        string Nome { get; }
        BancoCapacidades Capacidades { get; }

        Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao);
        Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao);
        Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao);
        Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao);
        Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao);
        Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao);
    }

    public abstract class BancoIntegracaoBase : IBancoIntegracao
    {
        public abstract int CodigoBanco { get; }
        public abstract string Nome { get; }
        public abstract BancoCapacidades Capacidades { get; }

        public virtual Task<BancoResposta> EmitirBoletoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Operação de boleto não suportada"));

        public virtual Task<BancoResposta> ConsultarBoletoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de boleto não suportada"));

        public virtual Task<BancoResposta> CriarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Operação de PIX não suportada"));

        public virtual Task<BancoResposta> ConsultarPixAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de PIX não suportada"));

        public virtual Task<BancoResposta> ConsultarExtratoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de extrato não suportada"));

        public virtual Task<BancoResposta> ConsultarSaldoAsync(BancoRequisicao requisicao)
            => Task.FromResult(BancoResposta.Falha("Consulta de saldo não suportada"));
    }
}
