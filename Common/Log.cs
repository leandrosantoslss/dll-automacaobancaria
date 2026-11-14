using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using AutomacaoBancaria.Bancos;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AutomacaoBancaria.Common
{
    internal static class Log
    {
        private static Logger Logger;
        private static readonly object SyncRoot = new object();
        private static string caminhoConfigurado;
        private static bool destinoConfigurado;

        public static void RegistrarOperacao(bool habilitado, string caminhoLog, int bancoId, string operacao, BancoResposta resposta, bool detalharErros, string jsonEnvio, string jsonRetorno)
        {
            if (!habilitado) return;

            ConfigurarDestino(caminhoLog);
            var logger = ObterLogger();
            if (logger == null) return;
            var mensagem = string.Format("BancoID={0}; Operacao={1}; Sucesso={2}; Status={3}; Erro={4}", bancoId, operacao, resposta?.Sucesso == true, resposta?.StatusCode?.ToString() ?? "n/a", Sanitizar(resposta?.Mensagem));
            if (detalharErros && !string.IsNullOrWhiteSpace(jsonEnvio)) mensagem += "; JsonEnvio=" + SanitizarJson(jsonEnvio);
            if (!string.IsNullOrWhiteSpace(jsonRetorno)) mensagem += "; JsonRetorno=" + SanitizarJson(jsonRetorno);

            if (resposta?.Sucesso == true) logger.Info(mensagem);
            else logger.Error(mensagem);
        }

        public static void RegistrarExcecao(bool habilitado, string caminhoLog, int bancoId, string operacao, Exception excecao, bool detalharErros, string jsonEnvio)
        {
            if (!habilitado) return;

            ConfigurarDestino(caminhoLog);
            var logger = ObterLogger();
            if (logger == null) return;
            var mensagem = string.Format("BancoID={0}; Operacao={1}; Sucesso=False; Status=n/a; Erro={2}; StackTrace={3}", bancoId, operacao, Sanitizar(excecao?.Message), Sanitizar(excecao?.ToString()));
            if (detalharErros && !string.IsNullOrWhiteSpace(jsonEnvio)) mensagem += "; JsonEnvio=" + SanitizarJson(jsonEnvio);
            logger.Error(mensagem);
        }

        public static void RegistrarFalha(bool habilitado, string caminhoLog, int bancoId, string operacao, string erro)
        {
            if (!habilitado) return;

            ConfigurarDestino(caminhoLog);
            var logger = ObterLogger();
            if (logger == null) return;
            logger.Error("BancoID={0}; Operacao={1}; Sucesso=False; Status=n/a; Erro={2}", bancoId, operacao, Sanitizar(erro));
        }

        private static Logger ObterLogger()
        {
            if (Logger != null) return Logger;
            try { Logger = LogManager.GetLogger("AutomacaoBancaria"); return Logger; }
            catch { return null; }
        }

        private static void ConfigurarDestino(string caminhoLog)
        {
            try
            {
                var destino = string.IsNullOrWhiteSpace(caminhoLog) ? null : Path.GetFullPath(caminhoLog.Trim());
                lock (SyncRoot)
                {
                    if (destinoConfigurado && string.Equals(caminhoConfigurado, destino, StringComparison.OrdinalIgnoreCase)) return;

                    if (destino == null)
                    {
                        var arquivoConfiguracao = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLog.config");
                        if (File.Exists(arquivoConfiguracao)) LogManager.Configuration = new XmlLoggingConfiguration(arquivoConfiguracao);
                    }
                    else
                    {
                        var configuracao = new LoggingConfiguration();
                        var arquivo = new FileTarget("arquivo")
                        {
                            FileName = Path.Combine(destino, "automacao-bancaria-${shortdate}.log"),
                            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
                            KeepFileOpen = false,
                            CreateDirs = true
                        };
                        configuracao.AddRule(LogLevel.Info, LogLevel.Fatal, arquivo, "*");
                        LogManager.Configuration = configuracao;
                    }

                    caminhoConfigurado = destino;
                    destinoConfigurado = true;
                }
            }
            catch { }
        }

        private static string SanitizarJson(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            try
            {
                var token = JToken.Parse(valor);
                SanitizarToken(token);
                return token.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch
            {
                return Sanitizar(valor);
            }
        }

        private static void SanitizarToken(JToken token)
        {
            var objeto = token as JObject;
            if (objeto != null)
            {
                foreach (var propriedade in objeto.Properties())
                {
                    if (EhSensivel(propriedade.Name) || (string.Equals(propriedade.Name, "boleto", StringComparison.OrdinalIgnoreCase) && propriedade.Value.Type == JTokenType.String)) propriedade.Value = "[REDACTED]";
                    else SanitizarToken(propriedade.Value);
                }
                return;
            }

            var array = token as JArray;
            if (array == null) return;
            foreach (var item in array) SanitizarToken(item);
        }

        private static bool EhSensivel(string chave)
        {
            var normalizada = (chave ?? "").Replace("_", "").Replace("-", "").ToLowerInvariant();
            return normalizada.Contains("clientsecret") || normalizada.Contains("certificado") || normalizada.Contains("token") || normalizada.Contains("apikey") || normalizada.Contains("senha") || normalizada.Contains("password") || normalizada.Contains("authorization") || normalizada.Contains("pdfboleto") || normalizada == "base64" || normalizada == "arquivo";
        }

        private static string Sanitizar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            return Regex.Replace(valor, "(?i)([\\\"']?(clientsecret|clientcertificadobase64|certificado|token|apikey|senha|password|authorization)[\\\"']?)\\s*[:=]\\s*([\\\"']?)[^,\\s}\\\"']+", "$1=[REDACTED]");
        }
    }
}
