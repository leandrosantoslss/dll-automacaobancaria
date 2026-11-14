param(
    [Parameter(Mandatory = $true)]
    [string]$DllPath
)

$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot
$newtonsoftPath = Join-Path $projectRoot 'packages\Newtonsoft.Json.13.0.4\lib\net45\Newtonsoft.Json.dll'
Add-Type -Path $newtonsoftPath
$assembly = [Reflection.Assembly]::LoadFrom($DllPath)

$utilType = $assembly.GetType('AutomacaoBancaria.cUtil')
$respostaType = $assembly.GetType('AutomacaoBancaria.Bancos.BancoResposta')
if ($null -eq $utilType) { throw 'AutomacaoBancaria.cUtil não encontrado' }
if ($null -eq $respostaType) { throw 'BancoResposta não encontrado' }

$flags = [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::NonPublic
$atualizarRetorno = $utilType.GetMethod('AtualizarRetorno', $flags)
$limparRetorno = $utilType.GetMethod('LimparRetorno', $flags)

if ($null -eq $atualizarRetorno) { throw 'AtualizarRetorno não encontrado' }
if ($null -eq $limparRetorno) { throw 'LimparRetorno não encontrado' }

$payload = 'PIX-COPIA-COLA-TESTE'
$dados = [Newtonsoft.Json.Linq.JToken]::Parse('{"?xml":{"@version":"1.0","@encoding":"UTF-8"},"SOAP-ENV:Envelope":{"SOAP-ENV:Body":{"ns1:buscarBoletoResponse":{"boleto":{"titulos":{"item":{"nossonumero":"09790002018204361686","pixCopiaCola":"PIX-COPIA-COLA-TESTE"}}}}}}}')
$resposta = [Activator]::CreateInstance($respostaType)
$resposta.Sucesso = $true
$resposta.Dados = $dados
$util = [Activator]::CreateInstance($utilType)

$parametros = New-Object object[] 1
$parametros[0] = $resposta
$null = $atualizarRetorno.Invoke($util, $parametros)

if ($util.QRCode -ne $payload) {
    throw "QRCode deveria conter o payload EMV; recebido '$($util.QRCode)'"
}
if ($util.PIXCopiaCola -ne $payload) {
    throw "PIXCopiaCola deveria conter o payload EMV; recebido '$($util.PIXCopiaCola)'"
}

$null = $limparRetorno.Invoke($util, $null)
if (-not [string]::IsNullOrEmpty($util.PIXCopiaCola)) {
    throw "PIXCopiaCola deveria ser limpo entre operações; recebido '$($util.PIXCopiaCola)'"
}

'PASS: CrediSIS expõe pixCopiaCola em QRCode e PIXCopiaCola e limpa o retorno.'
