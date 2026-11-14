using System.Runtime.InteropServices;

namespace AutomacaoBancaria
{
    [ComVisible(true)]
    [Guid("B4329EB4-8BA5-4B8D-AE5C-593F537EA94F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ICresol
    {
        [DispId(2)] string CresolAccountId { get; set; }
        [DispId(3)] string CresolCooperativa { get; set; }
        [DispId(6)] string CresolUsuario { get; set; }
        [DispId(7)] string CresolSenha { get; set; }

        [DispId(12)] string CresolTitulosListar(string filtrosJson);
        [DispId(13)] string CresolTituloConsultar(string id);
        [DispId(14)] string CresolTitulosIncluir(string json);
        [DispId(15)] string CresolTituloAlterar(string id, string json);
        [DispId(16)] string CresolTituloBaixar(string id);
        [DispId(17)] string CresolPagadoresListar(string filtrosJson);
        [DispId(18)] string CresolPagadorConsultar(string id);
        [DispId(19)] string CresolPagadorIncluir(string json);
        [DispId(20)] string CresolPagadorAlterar(string id, string json);
        [DispId(21)] string CresolPagadorExcluir(string id);
        [DispId(22)] string CresolOcorrenciasSequenciais(string dateStart, string dateEnd);
        [DispId(23)] string CresolOcorrenciaConsultar(string sequencial);
        [DispId(24)] int CresolIdEspecie { get; set; }
    }
}
