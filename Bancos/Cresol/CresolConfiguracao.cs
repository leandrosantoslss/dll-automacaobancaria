using System.Runtime.InteropServices;

namespace AutomacaoBancaria.Bancos.Cresol
{
    [ComVisible(false)]
    public sealed class CresolConfiguracao
    {
        internal const string ApiBaseUrlHomologacao = "https://api-dev.governarti.com.br/";
        internal const string TokenUrlPasswordHomologacao = "https://auth-dev.governarti.com.br/auth/realms/cresol/protocol/openid-connect/token";
        internal const string PasswordGrantType = "password";
        internal const string PasswordClientId = "cresolApi";
        internal const string PasswordClientSecret = "cr3s0l4p1";
        internal const string PasswordScope = "read";

        public string AccountId { get; set; }
        public string Cooperativa { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public int IdEspecie { get; set; }
    }
}
