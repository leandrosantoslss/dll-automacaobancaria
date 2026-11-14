using AutomacaoBancaria;
using AutomacaoBancaria.Bancos;
using AutomacaoBancaria.Bancos.Cresol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AutomacaoBancaria.Bancos.Cresol.Testes
{
    public class CresolFacadeTests
    {
        private static readonly KeyValuePair<string, int>[] CamposCresol =
        {
            new KeyValuePair<string, int>("CresolAccountId", 2),
            new KeyValuePair<string, int>("CresolCooperativa", 3),
            new KeyValuePair<string, int>("CresolUsuario", 6),
            new KeyValuePair<string, int>("CresolSenha", 7),
            new KeyValuePair<string, int>("CresolIdEspecie", 24)
        };

        private static readonly KeyValuePair<string, int>[] MetodosCresol =
        {
            new KeyValuePair<string, int>("CresolTitulosListar", 12),
            new KeyValuePair<string, int>("CresolTituloConsultar", 13),
            new KeyValuePair<string, int>("CresolTitulosIncluir", 14),
            new KeyValuePair<string, int>("CresolTituloAlterar", 15),
            new KeyValuePair<string, int>("CresolTituloBaixar", 16),
            new KeyValuePair<string, int>("CresolPagadoresListar", 17),
            new KeyValuePair<string, int>("CresolPagadorConsultar", 18),
            new KeyValuePair<string, int>("CresolPagadorIncluir", 19),
            new KeyValuePair<string, int>("CresolPagadorAlterar", 20),
            new KeyValuePair<string, int>("CresolPagadorExcluir", 21),
            new KeyValuePair<string, int>("CresolOcorrenciasSequenciais", 22),
            new KeyValuePair<string, int>("CresolOcorrenciaConsultar", 23)
        };

        private static readonly string[] PropriedadesNaoExpostas =
        {
            "CresolBaseUrl", "CresolTipoAutenticacao", "CresolTokenUrl", "CresolClientId",
            "CresolClientSecret", "CresolScope", "CresolCertificadoThumbprint"
        };

        private const string GuidInterfaceCresol = "B4329EB4-8BA5-4B8D-AE5C-593F537EA94F";

        [Fact]
        public void Bank_factory_creates_cresol_in_both_interfaces()
        {
            var configuracao = new CresolConfiguracao();

            var integracao = BancoFactory.Criar(133, configuracao);
            var hibrida = BancoFactory.CriarHibrido(133, configuracao);

            Assert.IsType<CresolIntegracao>(integracao);
            Assert.IsType<CresolIntegracao>(hibrida);
            Assert.Equal(133, integracao.CodigoBanco);
            Assert.Equal("CRESOL", integracao.Nome);
        }

        [Fact]
        public void Bank_factory_rejects_wrong_configuration_for_code_133()
        {
            Assert.Throws<ArgumentException>(() => BancoFactory.Criar(133, new object()));
            Assert.Throws<ArgumentException>(() => BancoFactory.CriarHibrido(133, new object()));
        }

        [Fact]
        public void Cresol_com_surface_uses_explicit_dual_interface_with_stable_dispids()
        {
            var tipo = typeof(cUtil);
            var interfaceCresol = tipo.Assembly.GetType("AutomacaoBancaria.ICresol");

            Assert.NotNull(interfaceCresol);
            Assert.True(interfaceCresol.IsInterface);
            Assert.Equal(GuidInterfaceCresol, interfaceCresol.GetCustomAttribute<GuidAttribute>().Value.ToUpperInvariant());
            Assert.Equal(ComInterfaceType.InterfaceIsDual, interfaceCresol.GetCustomAttribute<InterfaceTypeAttribute>().Value);
            Assert.True(interfaceCresol.GetCustomAttribute<ComVisibleAttribute>().Value);
            Assert.Contains(interfaceCresol, tipo.GetInterfaces());
            Assert.Equal(ClassInterfaceType.AutoDual, tipo.GetCustomAttribute<ClassInterfaceAttribute>().Value);

            for (var index = 0; index < CamposCresol.Length; index++)
            {
                var campo = CamposCresol[index];
                var propriedade = interfaceCresol.GetProperty(campo.Key);
                Assert.NotNull(propriedade);
                Assert.Equal(campo.Key == "CresolIdEspecie" ? typeof(int) : typeof(string), propriedade.PropertyType);
                Assert.Equal(campo.Value, propriedade.GetCustomAttribute<DispIdAttribute>().Value);
                Assert.Null(tipo.GetField(campo.Key, BindingFlags.Instance | BindingFlags.Public));
            }

            foreach (var metodoEsperado in MetodosCresol)
            {
                var metodo = interfaceCresol.GetMethod(metodoEsperado.Key);
                Assert.NotNull(metodo);
                Assert.Equal(typeof(string), metodo.ReturnType);
                Assert.Equal(metodoEsperado.Value, metodo.GetCustomAttribute<DispIdAttribute>().Value);
                Assert.Null(tipo.GetMethod(metodoEsperado.Key, BindingFlags.Instance | BindingFlags.Public));
            }

            Assert.All(PropriedadesNaoExpostas, nome => Assert.Null(interfaceCresol.GetProperty(nome)));
        }

        [Fact]
        public void Cresol_oauth_constants_are_not_configurable_in_com_or_configuration_types()
        {
            var configuracao = new CresolConfiguracao();
            var interfaceCresol = ObterInterfaceCresol();

            foreach (var propriedade in PropriedadesNaoExpostas)
            {
                var nomeConfiguracao = propriedade.Replace("Cresol", string.Empty);
                Assert.Null(configuracao.GetType().GetProperty(nomeConfiguracao));
            }
        }

        [Fact]
        public void Cresol_id_especie_is_forwarded_to_integration_configuration()
        {
            var util = new cUtil { BancoID = 133 };
            DefinirPropriedadeInterface(util, "CresolIdEspecie", 7);

            var metodo = typeof(cUtil).GetMethod("ObterCresolIntegracao", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(metodo);
            var integracao = (CresolIntegracao)metodo.Invoke(util, null);
            var campoConfiguracao = typeof(CresolIntegracao).GetField("_configuracao", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(campoConfiguracao);
            var configuracao = campoConfiguracao.GetValue(integracao);
            Assert.Equal(7, configuracao.GetType().GetProperty("IdEspecie").GetValue(configuracao));
        }

        [Fact]
        public void Configuracao_and_internal_bank_router_accept_code_133_without_network()
        {
            var util = new cUtil();

            Assert.True(util.Configuracao(133));
            Assert.Equal(133, util.BancoID);

            var getBanco = typeof(cUtil).GetMethod("GetBanco", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(getBanco);
            Assert.IsAssignableFrom<IBancoHibrido>(getBanco.Invoke(util, null));
        }

        [Fact]
        public void Named_wrapper_validates_input_without_authenticating_or_leaking_secrets()
        {
            var util = new cUtil { BancoID = 133 };
            var interfaceCresol = ObterInterfaceCresol();
            DefinirPropriedadeInterface(util, "CresolSenha", "segredo-falso-teste");
            DefinirPropriedadeInterface(util, "CresolUsuario", "usuario-falso-teste");
            var wrapper = interfaceCresol.GetMethod("CresolTitulosListar");
            Assert.NotNull(wrapper);

            var json = (string)wrapper.Invoke(util, new object[] { "{\"campoNaoPublicado\":\"x\"}" });
            var resposta = JObject.Parse(json);

            Assert.False(resposta["Sucesso"].ToObject<bool>());
            Assert.Contains("filtro", resposta["Mensagem"].ToString(), StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("segredo-falso-teste", json);
            Assert.Equal(string.Empty, util.JsonEnvio);
            Assert.Equal(json, util.JsonRetorno);
        }

        [Fact]
        public void Named_wrapper_rejects_calls_when_bank_id_is_not_133()
        {
            var util = new cUtil { BancoID = 748 };
            var wrapper = ObterInterfaceCresol().GetMethod("CresolTituloConsultar");
            Assert.NotNull(wrapper);

            var json = (string)wrapper.Invoke(util, new object[] { "1" });

            Assert.Contains("BancoID", JObject.Parse(json)["Mensagem"].ToString());
            Assert.DoesNotContain("Bearer", json);
        }

        [Fact]
        public void Batch_and_json_boleto_paths_remain_outside_cresol_common_flow()
        {
            var util = new cUtil { BancoID = 133 };

            AssertCresolJsonFailure(util.BoletosEmitir());
            AssertCresolJsonFailure(util.EmitirBoletosPadraoJson("null"));
            AssertCresolJsonFailure(util.ConsultarBoletoJson("null"));
        }

        [Fact]
        public void Common_boleto_emission_maps_required_fields_to_cresol_payload()
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(System.Net.HttpStatusCode.OK, "{\"access_token\":\"token-teste\",\"expires_in\":3600}");
            handler.Enfileirar(System.Net.HttpStatusCode.NonAuthoritativeInformation, "{\"id\":41}");
            var config = new CresolConfiguracao
            {
                Usuario = "usuario-teste",
                Senha = "senha-teste",
                AccountId = "conta-teste"
            };
            DefinirPropriedadeConfiguracao(config, "IdEspecie", 7);

            using (var client = new System.Net.Http.HttpClient(handler))
            {
                var util = new cUtil { BancoID = 133 };
                InjetarCresol(util, new CresolIntegracao(config, client, () => DateTime.UtcNow));
                DefinirPropriedadeInterface(util, "CresolIdEspecie", 7);
                util.BoletoDados("1001", "SEU-1", "DOC-1", 123.45, DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"));
                util.PagadorDados("Pagador Teste", "00123456789", "", "", "", "Rua A", "10", "Centro", "Curitiba", "PR", "80010-000");

                var resposta = JObject.Parse(util.BoletoEmitir());

                Assert.True(resposta["Sucesso"].ToObject<bool>());
                Assert.Equal(203, resposta["StatusCode"].ToObject<int>());
            }

            Assert.Equal(2, handler.Requisicoes.Count);
            var post = handler.Requisicoes[1];
            Assert.Equal("POST", post.Metodo);
            Assert.Equal("https://api-dev.governarti.com.br/titulos", post.Uri.AbsoluteUri);
            var titulo = (JObject)JArray.Parse(post.Corpo).Single();
            Assert.Equal("00123456789", titulo["docPagador"].Value<string>());
            Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), titulo["dtDocumento"].Value<string>());
            Assert.Equal(DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"), titulo["dtVencimento"].Value<string>());
            Assert.Equal(7, titulo["idEspecie"].Value<int>());
            Assert.Equal("DOC-1", titulo["numeroDocumento"].Value<string>());
            Assert.Equal(1001L, titulo["nossoNumero"].Value<long>());
            Assert.Equal("Pagador Teste", titulo["pagadorNome"].Value<string>());
            Assert.Equal("Rua A", titulo["pagadorEndereco"].Value<string>());
            Assert.Equal("10", titulo["pagadorEnderecoNumero"].Value<string>());
            Assert.Equal(80010000, titulo["pagadorCep"].Value<int>());
            Assert.Equal("Centro", titulo["pagadorBairro"].Value<string>());
            Assert.Equal("Curitiba", titulo["pagadorCidade"].Value<string>());
            Assert.Equal("PR", titulo["pagadorUf"].Value<string>());
            Assert.Equal(123.45m, titulo["valorNominal"].Value<decimal>());
        }

        [Fact]
        public void Common_boleto_consultation_maps_our_number_and_due_date_range()
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(System.Net.HttpStatusCode.OK, "{\"access_token\":\"token-teste\",\"expires_in\":3600}");
            handler.Enfileirar(System.Net.HttpStatusCode.OK, "[]");
            var config = new CresolConfiguracao { Usuario = "usuario-teste", Senha = "senha-teste" };

            using (var client = new System.Net.Http.HttpClient(handler))
            {
                var util = new cUtil { BancoID = 133 };
                InjetarCresol(util, new CresolIntegracao(config, client, () => DateTime.UtcNow));
                util.BoletoInformarConsulta("", "NN-10", "", "", 0, "2026-11-01", "2026-11-30");

                var jsonRetorno = util.BoletoConsultar();
                Assert.True(JObject.Parse(jsonRetorno)["Sucesso"].ToObject<bool>(), jsonRetorno);
            }

            Assert.Equal("https://api-dev.governarti.com.br/titulos", handler.Requisicoes[1].Uri.GetLeftPart(UriPartial.Path));
            Assert.Contains("nn=NN-10", handler.Requisicoes[1].Uri.Query);
            Assert.Contains("dtVencimentoInicial=2026-11-01", handler.Requisicoes[1].Uri.Query);
            Assert.Contains("dtVencimentoFinal=2026-11-30", handler.Requisicoes[1].Uri.Query);
        }

        [Fact]
        public void Common_boleto_consultation_prioritizes_id_over_other_filters()
        {
            var handler = new RecordingHttpMessageHandler();
            handler.Enfileirar(System.Net.HttpStatusCode.OK, "{\"access_token\":\"token-teste\",\"expires_in\":3600}");
            handler.Enfileirar(System.Net.HttpStatusCode.OK, "{\"id\":41}");
            var config = new CresolConfiguracao { Usuario = "usuario-teste", Senha = "senha-teste" };

            using (var client = new System.Net.Http.HttpClient(handler))
            {
                var util = new cUtil { BancoID = 133 };
                InjetarCresol(util, new CresolIntegracao(config, client, () => DateTime.UtcNow));
                util.BoletoInformarConsulta("41", "NN-10", "", "", 0, "2026-11-01", "2026-11-30");

                Assert.True(JObject.Parse(util.BoletoConsultar())["Sucesso"].ToObject<bool>());
            }

            Assert.Equal("https://api-dev.governarti.com.br/titulos/41", handler.Requisicoes[1].Uri.AbsoluteUri);
        }

        private static void AssertCresolJsonFailure(string json)
        {
            var resposta = JObject.Parse(json);
            Assert.False(resposta["Sucesso"].ToObject<bool>());
            Assert.Contains("JSON CRESOL", resposta["Mensagem"].ToString());
        }

        private static Type ObterInterfaceCresol()
        {
            var interfaceCresol = typeof(cUtil).Assembly.GetType("AutomacaoBancaria.ICresol");
            Assert.NotNull(interfaceCresol);
            return interfaceCresol;
        }

        private static void InjetarCresol(cUtil util, CresolIntegracao integracao)
        {
            var campo = typeof(cUtil).GetField("_cresolIntegracao", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(campo);
            campo.SetValue(util, integracao);
        }

        private static void DefinirPropriedadeConfiguracao(CresolConfiguracao configuracao, string nome, object valor)
        {
            var propriedade = configuracao.GetType().GetProperty(nome);
            Assert.NotNull(propriedade);
            propriedade.SetValue(configuracao, valor);
        }

        private static void DefinirPropriedadeInterface(cUtil util, string nome, object valor)
        {
            var propriedade = ObterInterfaceCresol().GetProperty(nome);
            Assert.NotNull(propriedade);
            propriedade.SetValue(util, valor);
        }
    }
}
