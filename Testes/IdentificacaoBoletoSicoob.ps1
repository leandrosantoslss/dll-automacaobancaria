param(
    [string]$DllPath = 'C:\automacaobancaria-api\app\AutomacaoBancaria.dll'
)

$ErrorActionPreference = 'Stop'

$assembly = [Reflection.Assembly]::LoadFrom($DllPath)
$flags = [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::NonPublic
$integrationType = $assembly.GetType('AutomacaoBancaria.Bancos.Sicoob.SicoobIntegracao')
$configType = $assembly.GetType('AutomacaoBancaria.Bancos.Sicoob.SicoobConfiguracao')
$boletoType = $assembly.GetType('AutomacaoBancaria.Bancos.BoletoPadrao')
$pagadorType = $assembly.GetType('AutomacaoBancaria.Bancos.PagadorPadrao')
$createBoleto = $integrationType.GetMethod('CriarBoleto', $flags)

if ($null -eq $createBoleto) { throw 'CriarBoleto do Sicoob não encontrado' }

$config = New-Object -TypeName $configType.FullName
$config.NumeroContrato = 123
$integration = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($integrationType)
$configField = $integrationType.GetField('configuracao', $flags)
$configField.SetValue($integration, $config)

$boleto = New-Object -TypeName $boletoType.FullName
$boleto.Valor = 10
$boleto.Vencimento = [DateTime]::Today.AddDays(1)
$boleto.SeuNumero = 'TESTE'
$boleto.Pagador = New-Object -TypeName $pagadorType.FullName
$boleto.Pagador.Nome = 'Pagador de teste'
$boleto.Pagador.Documento = '12345678901'

$parameters = New-Object object[] 1
$parameters[0] = $boleto.PSObject.BaseObject
$body = $createBoleto.Invoke($integration, $parameters)
if ($null -eq $body) { throw 'CriarBoleto do Sicoob retornou nulo' }
$bodyJson = $body.ToString()
Write-Output "Payload montado: $bodyJson"

if ($body['identificacaoEmissaoBoleto'].ToString() -ne '2') {
    throw "identificacaoEmissaoBoleto deveria ser 2 (Cliente Emite), recebido: $($body['identificacaoEmissaoBoleto'].ToString())"
}
if ($body['identificacaoDistribuicaoBoleto'].ToString() -ne '2') {
    throw "identificacaoDistribuicaoBoleto deveria ser 2 (Cliente Distribui), recebido: $($body['identificacaoDistribuicaoBoleto'].ToString())"
}

$emissaoProperty = $boletoType.GetProperty('IdentificacaoEmissaoBoleto')
$distribuicaoProperty = $boletoType.GetProperty('IdentificacaoDistribuicaoBoleto')
if ($null -eq $emissaoProperty) { throw 'BoletoPadrao não expõe IdentificacaoEmissaoBoleto' }
if ($null -eq $distribuicaoProperty) { throw 'BoletoPadrao não expõe IdentificacaoDistribuicaoBoleto' }

$boleto.IdentificacaoEmissaoBoleto = 1
$boleto.IdentificacaoDistribuicaoBoleto = 1
$body = $createBoleto.Invoke($integration, $parameters)
if ($body['identificacaoEmissaoBoleto'].ToString() -ne '1') {
    throw "Sicoob não usou IdentificacaoEmissaoBoleto informada: $($body['identificacaoEmissaoBoleto'].ToString())"
}
if ($body['identificacaoDistribuicaoBoleto'].ToString() -ne '1') {
    throw "Sicoob não usou IdentificacaoDistribuicaoBoleto informada: $($body['identificacaoDistribuicaoBoleto'].ToString())"
}

'PASS: Sicoob aplica os valores padrão e os valores informados em BoletoPadrao.'
