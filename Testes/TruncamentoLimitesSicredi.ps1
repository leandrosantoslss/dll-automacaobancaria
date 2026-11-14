param(
    [string]$DllPath = 'C:\automacaobancaria-api\app\AutomacaoBancaria.dll'
)

$ErrorActionPreference = 'Stop'

[Reflection.Assembly]::LoadFrom($DllPath) | Out-Null
$flags = [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::NonPublic
$assembly = [Reflection.Assembly]::LoadFrom($DllPath)
$utilType = $assembly.GetType('AutomacaoBancaria.cUtil')
$boletoType = $assembly.GetType('AutomacaoBancaria.Bancos.BoletoPadrao')
$pagadorType = $assembly.GetType('AutomacaoBancaria.Bancos.PagadorPadrao')
$requestType = $assembly.GetType('AutomacaoBancaria.Bancos.BoletoEmissaoRequest')
$sicrediType = $assembly.GetType('AutomacaoBancaria.Bancos.Sicredi.SicrediIntegracao')
$sicrediConfigType = $assembly.GetType('AutomacaoBancaria.Bancos.Sicredi.SicrediConfiguracao')
$sicrediFormatacaoType = $assembly.GetType('AutomacaoBancaria.Bancos.Sicredi.SicrediFormatacao')
$validate = $utilType.GetMethod('ValidarEmissaoBoletos', $flags)
$createBody = $sicrediType.GetMethod('CriarCorpoBoleto', $flags)
$staticFlags = [Reflection.BindingFlags]::Static -bor [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::NonPublic
$maskNossoNumero = if ($null -eq $sicrediFormatacaoType) { $null } else { $sicrediFormatacaoType.GetMethod('MascararNossoNumero', $staticFlags) }

if ($null -eq $validate) { throw 'ValidarEmissaoBoletos não encontrado' }
if ($null -eq $createBody) { throw 'CriarCorpoBoleto não encontrado' }
if ($null -eq $maskNossoNumero) { throw 'MascararNossoNumero do Sicredi não encontrado' }

function New-Request {
    $boleto = New-Object -TypeName $boletoType.FullName
    $boleto.Valor = 10
    $boleto.Vencimento = [DateTime]::Today.AddDays(1)
    $boleto.NossoNumero = '12.345'
    $boleto.SeuNumero = ('S' * 20) -join ''
    $boleto.NumeroDocumento = ('D' * 20) -join ''
    $boleto.Instrucao1 = ('I' * 100) -join ''

    $boleto.Pagador = New-Object -TypeName $pagadorType.FullName
    $boleto.Pagador.Nome = ('P' * 50) -join ''
    $boleto.Pagador.Documento = '123456789012345'
    $boleto.Pagador.Email = (('e' * 45) -join '') + '@x.com'
    $boleto.Pagador.Telefone = '551199999999999'
    $boleto.Pagador.Endereco = ('E' * 50) -join ''
    $boleto.Pagador.Cidade = ('C' * 50) -join ''
    $boleto.Pagador.Uf = 'SPX'
    $boleto.Pagador.Cep = '123456789012'

    $request = New-Object -TypeName $requestType.FullName
    $request.BoletoPadrao = $boleto
    return $request
}

function Invoke-Validation($bankId) {
    $util = New-Object -TypeName $utilType.FullName
    $util.BancoID = $bankId
    $request = (New-Request).PSObject.BaseObject
    $parameters = New-Object object[] 1
    $parameters[0] = $request.PSObject.BaseObject
    $error = $validate.Invoke($util, $parameters)
    if ($null -ne $error) { throw "Validação retornou erro: $error" }
    return $request.BoletoPadrao
}

$sicredi = Invoke-Validation 748
$maskParameters = New-Object object[] 1
$maskParameters[0] = '1'
$consultaNossoNumero = $maskNossoNumero.Invoke($null, $maskParameters)
if ($consultaNossoNumero -ne '000000001') { throw "Sicredi não mascarou NossoNumero de consulta: $consultaNossoNumero" }
if ($sicredi.Pagador.Nome.Length -ne 40) { throw 'Sicredi não truncou Pagador.Nome para 40' }
if ($sicredi.Pagador.Email.Length -ne 40) { throw 'Sicredi não truncou Pagador.Email para 40' }
if ($sicredi.Pagador.Telefone.Length -ne 11) { throw 'Sicredi não truncou Pagador.Telefone para 11' }
if ($sicredi.Pagador.Endereco.Length -ne 40) { throw 'Sicredi não truncou Pagador.Endereco para 40' }
if ($sicredi.Pagador.Cidade.Length -ne 40) { throw 'Sicredi não truncou Pagador.Cidade para 40' }
if ($sicredi.Pagador.Uf.Length -ne 2) { throw 'Sicredi não truncou Pagador.Uf para 2' }
if ($sicredi.Pagador.Cep.Length -ne 8) { throw 'Sicredi não truncou Pagador.Cep para 8' }
if ($sicredi.Pagador.Documento.Length -ne 14) { throw 'Sicredi não truncou Pagador.Documento para 14' }
if ($sicredi.NossoNumero -ne '000012345') { throw "Sicredi não mascarou NossoNumero: $($sicredi.NossoNumero)" }
if ($sicredi.SeuNumero.Length -ne 10) { throw 'Sicredi não truncou SeuNumero para 10' }
if ($sicredi.NumeroDocumento.Length -ne 10) { throw 'Sicredi não truncou NumeroDocumento para 10' }
if ($sicredi.Instrucao1.Length -ne 80) { throw 'Sicredi não truncou Instrucao1 para 80' }

$outroBanco = Invoke-Validation 341
if ($outroBanco.Pagador.Nome.Length -ne 50) { throw 'Banco sem perfil foi alterado indevidamente' }

$config = New-Object -TypeName $sicrediConfigType.FullName
$config.ApiKey = 'teste'
$config.Sandbox = $true
$sicrediIntegration = New-Object -TypeName $sicrediType.FullName -ArgumentList $config
$bodyBoleto = (New-Request).BoletoPadrao
$bodyParameters = New-Object object[] 1
$bodyParameters[0] = $bodyBoleto.PSObject.BaseObject
$body = $createBody.Invoke($sicrediIntegration, $bodyParameters)
if ($body['tipoCobranca'].ToString() -ne 'HIBRIDO') { throw 'Sicredi não fixou tipoCobranca como HIBRIDO' }

'PASS: limites Sicredi aplicados e banco sem perfil preservado.'
