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
using System;

namespace AutomacaoBancaria.Bancos
{
    public static class BancoFactory
    {
        public static IBancoIntegracao Criar(int codigoBanco, object configuracao)
        {
            if (codigoBanco == 756 && configuracao is SicoobConfiguracao sicoob)
                return new SicoobIntegracao(sicoob);
            if (codigoBanco == 364 && configuracao is EfiConfiguracao efi)
                return new EfiIntegracao(efi);
            if (codigoBanco == 461 && configuracao is AsaasConfiguracao asaas)
                return new AsaasIntegracao(asaas);
            if (codigoBanco == 748 && configuracao is SicrediConfiguracao sicredi)
                return new SicrediIntegracao(sicredi);
            if (codigoBanco == 97 && configuracao is CredisisConfiguracao credisis)
                return new CredisisIntegracao(credisis);
            if (codigoBanco == 1 && configuracao is BBConfiguracao bb)
                return new BBIntegracao(bb);
            if (codigoBanco == 237 && configuracao is BradescoConfiguracao bradesco)
                return new BradescoIntegracao(bradesco);
            if (codigoBanco == 341 && configuracao is ItauConfiguracao itau)
                return new ItauIntegracao(itau);
            if (codigoBanco == 33 && configuracao is SantanderConfiguracao santander)
                return new SantanderIntegracao(santander);
            if (codigoBanco == 41 && configuracao is BanrisulConfiguracao banrisul)
                return new BanrisulIntegracao(banrisul);
            if (codigoBanco == 133 && configuracao is CresolConfiguracao cresol)
                return new CresolIntegracao(cresol);

            throw new ArgumentException("Configuração inválida para o banco informado");
        }

        public static IBancoHibrido CriarHibrido(int codigoBanco, object configuracao)
        {
            if (codigoBanco == 756 && configuracao is SicoobConfiguracao sicoob)
                return new SicoobIntegracao(sicoob);
            if (codigoBanco == 364 && configuracao is EfiConfiguracao efi)
                return new EfiIntegracao(efi);
            if (codigoBanco == 461 && configuracao is AsaasConfiguracao asaas)
                return new AsaasIntegracao(asaas);
            if (codigoBanco == 748 && configuracao is SicrediConfiguracao sicredi)
                return new SicrediIntegracao(sicredi);
            if (codigoBanco == 97 && configuracao is CredisisConfiguracao credisis)
                return new CredisisIntegracao(credisis);
            if (codigoBanco == 1 && configuracao is BBConfiguracao bb)
                return new BBIntegracao(bb);
            if (codigoBanco == 237 && configuracao is BradescoConfiguracao bradesco)
                return new BradescoIntegracao(bradesco);
            if (codigoBanco == 341 && configuracao is ItauConfiguracao itau)
                return new ItauIntegracao(itau);
            if (codigoBanco == 33 && configuracao is SantanderConfiguracao santander)
                return new SantanderIntegracao(santander);
            if (codigoBanco == 41 && configuracao is BanrisulConfiguracao banrisul)
                return new BanrisulIntegracao(banrisul);
            if (codigoBanco == 133 && configuracao is CresolConfiguracao cresol)
                return new CresolIntegracao(cresol);

            throw new ArgumentException("Configuração inválida para o banco informado");
        }

        public static SicoobIntegracao CriarSicoob(SicoobConfiguracao configuracao)
            => new SicoobIntegracao(configuracao);

        public static EfiIntegracao CriarEfi(EfiConfiguracao configuracao)
            => new EfiIntegracao(configuracao);

        public static AsaasIntegracao CriarAsaas(AsaasConfiguracao configuracao)
            => new AsaasIntegracao(configuracao);

        public static SicrediIntegracao CriarSicredi(SicrediConfiguracao configuracao)
            => new SicrediIntegracao(configuracao);

        public static CredisisIntegracao CriarCredisis(CredisisConfiguracao configuracao)
            => new CredisisIntegracao(configuracao);

        public static BBIntegracao CriarBB(BBConfiguracao configuracao)
            => new BBIntegracao(configuracao);

        public static BradescoIntegracao CriarBradesco(BradescoConfiguracao configuracao)
            => new BradescoIntegracao(configuracao);

        public static ItauIntegracao CriarItau(ItauConfiguracao configuracao)
            => new ItauIntegracao(configuracao);

        public static SantanderIntegracao CriarSantander(SantanderConfiguracao configuracao)
            => new SantanderIntegracao(configuracao);

        public static BanrisulIntegracao CriarBanrisul(BanrisulConfiguracao configuracao)
            => new BanrisulIntegracao(configuracao);
    }
}
