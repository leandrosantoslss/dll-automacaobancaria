# AutomacaoBancaria DLL

DLL .NET Framework 4.7.2 exposta para COM/VB6 pelo ProgID `AutomacaoBancaria.cUtil`.

## Fluxo comum

1. Crie `AutomacaoBancaria.cUtil` no VB6.
2. Preencha as credenciais do banco antes de selecionar/configurar o banco.
3. Defina `homologacao = True` para sandbox/homologacao e `False` para producao.
4. Chame `Configuracao(codigoBanco)`.
5. Informe boleto, pagador ou PIX e execute a operacao.
6. Leia o JSON retornado e as propriedades de retorno.

```vb
Dim banco As Object
Set banco = CreateObject("AutomacaoBancaria.cUtil")

banco.homologacao = True
banco.Configuracao 756
```

As propriedades publicas de retorno sao limpas antes de cada operacao. `Sucesso = True` indica retorno HTTP/processamento bem-sucedido da integracao. Em caso de falha, use o JSON retornado; defina `DetalharErros = True` durante diagnostico.

## Logs

O log NLog e habilitado por padrao (`HabilitarLog = True`) e grava arquivos diarios em `${basedir}/logs/automacao-bancaria-AAAA-MM-DD.log`, isto e, na pasta `logs` ao lado da DLL/aplicacao. Cada entrada informa `BancoID`, operacao, sucesso, status e erro; excecoes incluem stack trace. Credenciais, certificados e tokens sao removidos antes da gravacao.

Para diagnostico, `DetalharErros = True` inclui `JsonEnvio` sanitizado no log. O `JsonRetorno` sanitizado tambem e registrado para permitir diagnosticar erros de negocio, como rejeicoes SOAP do CrediSIS. PDF, arquivos Base64, credenciais, certificados e tokens sao ocultados.

```vb
banco.HabilitarLog = True
banco.CaminhoLog = "C:\logs\automacao-bancaria" ' Opcional; substitui o destino padrao
banco.DetalharErros = True                 ' Opcional; inclui JsonEnvio sanitizado
```

| Propriedade | Conteudo |
| --- | --- |
| `Sucesso` | Resultado da ultima operacao. |
| `SeuNumero` | Identificacao do titulo no cliente, quando retornada. |
| `NossoNumero` | Nosso numero retornado como string, preservando zeros à esquerda e números longos. |
| `CodigoBoleto` | Código interno retornado para o boleto; no CrediSIS usa `numeroSequencial` quando disponível. |
| `LinhaDigitavel` | Linha digitavel do boleto. |
| `CodigoBarras` | Codigo de barras do boleto. |
| `QRCode` | Payload EMV/QR Code PIX, quando retornado. |
| `pdfBoleto` | PDF Base64, quando a API o disponibiliza. |
| `JsonEnvio` | Corpo enviado pelas integrações Sicoob, Bradesco, CrediSIS, Itaú e Santander. No CrediSIS contém o XML de dados da operação, sem o envelope de autenticação. Nunca contém token ou senha. |
| `JsonRetorno` | JSON completo de `BancoResposta` da última operação. |
| `ArquivoPDFGerado` | Caminho do PDF gravado localmente, quando solicitado e retornado em Base64. |
| `MensagemArquivoPDF` | Motivo pelo qual a gravação do PDF não ocorreu. |

## Metodos comuns

### Boleto

```vb
banco.BoletoLimpar
banco.BoletoDados "123", "PED-123", "DOC-123", 150.5, "2026-12-31", 1, False, True, "Instrucao 1", "", ""
banco.PagadorDados "Cliente", "12345678909", "cliente@empresa.com", "11999999999", "", "Rua A", "100", "Centro", "Sao Paulo", "SP", "01001000"
json = banco.BoletoEmitir()
```

`BoletoDados` recebe: `nossoNumero`, `seuNumero`, `numeroDocumento`, `valor`, `vencimento`, `modalidade`, `gerarPdf`, `cadastrarPix`, tres instrucoes e, opcionalmente, `dataLimitePagamento`, `tipoDesconto`, `valorDesconto`, `tipoMulta`, `dataMulta`, `valorMulta`, `tipoJuros`, `dataJuros`, `valorJuros`, `numeroParcela`, `codigoProtesto`, `numeroDiasProtesto` e `dataPrimeiroDesconto`. As datas devem ser strings, preferencialmente no formato `yyyy-MM-dd`.

Exemplo com os novos campos:

```vb
banco.BoletoDados "", "PED-123", "DOC-123", 150.5, "2026-12-31", 1, False, False, "", "", "", _
    "2027-01-30", 1, 10, 2, "2027-01-01", 5, 1, "2027-01-01", 2, 1, 1, 10, "2027-01-15"
```

Os valores `0` e datas vazias mantêm os padrões da integração. Quando `valorDesconto` for `0`, a DLL automaticamente trata `tipoDesconto` como `0`, removendo o desconto. Para aplicar desconto Sicoob, informe `tipoDesconto` diferente de zero e `valorDesconto` maior que zero. Se `dataPrimeiroDesconto` ficar vazia, a DLL usa a data de vencimento como fallback.

Quando `tipoMulta` ou `tipoJuros` for `0`, as respectivas datas não são enviadas. Quando forem informados, as datas devem ser posteriores à data de vencimento; se ficarem vazias, a DLL usa o dia seguinte ao vencimento.

Se `dataMulta` ou `dataJuros` for informada exatamente igual à data de vencimento, a DLL ajusta automaticamente a respectiva data para o dia seguinte.

### Baixa e alteração de boleto Sicoob

Após configurar o banco com `Configuracao 756`, a baixa usa somente o nosso número:

```vb
json = banco.BoletoBaixar(2536, 1)
```

Para alterar, preencha `BoletoDados` com o nosso número e os novos valores. Depois chame `BoletoAlterar`:

```vb
banco.BoletoLimpar
banco.BoletoDados "2536", "PED-123", "DOC-123", 150.5, "2026-12-31", 1, False, False, "", "", "", "2027-01-30", 1, 10, 2, "2027-01-01", 5, 1, "2027-01-01", 2, 1, 1, 10
json = banco.BoletoAlterar()
```

Esses métodos estão disponíveis atualmente para o Sicoob e usam `POST /boletos/{nossoNumero}/baixar` e `PATCH /boletos/{nossoNumero}`.

Para consultar:

```vb
banco.BoletoLimparConsulta
banco.BoletoInformarConsulta "", "09700005022582192019", "", "", 1, "", "", False
json = banco.BoletoConsultar()
```

O ultimo parametro de `BoletoInformarConsulta` solicita segunda via. Atualmente ele e usado pelo Sicoob e envia `gerarPdf=true`.

### PIX

```vb
banco.PixLimpar
banco.PixDados "chave-pix", 25.5, 3600, "Pagamento do pedido", "Cliente", "12345678909", "TXID123"
json = banco.PixCriar()

' Exibe o payload EMV retornado em uma janela com QR Code e botao Copiar.
' Use QRCode quando a operacao o tiver preenchido.
If banco.QRCode <> "" Then banco.PixExibirUltimoQRCode

' Ou informe diretamente o campo pixCopiaECola/EMV extraido do JSON retornado.
Call banco.PixExibirQRCode("000201...")

banco.PixConsultar "TXID123", "", Empty, Empty
json = banco.PixConsultar()
```

Para chamadas avancadas, use `EmitirBoletosPadraoJson`, `ConsultarBoletoJson`, `CriarPixJson`, `ConsultarPixJson`, `ConsultarExtratoJson` e `ConsultarSaldoJson`. Os argumentos sao JSON dos tipos `BoletoEmissaoRequest`, `BoletoConsultaRequest`, `PixCriacaoRequest`, `PixConsultaRequest`, `ExtratoRequest` e `SaldoRequest`.

### Janela de QR Code PIX

`PixExibirUltimoQRCode()` abre uma janela modal usando o valor da propriedade `QRCode`. `PixExibirQRCode(conteudo)` abre a mesma janela para qualquer payload PIX copia-e-cola/EMV informado. A janela apresenta a imagem QR e permite copiar o texto de pagamento. O metodo retorna `False` quando o conteudo estiver vazio.

Fluxo recomendado no VB6 apos criar a cobranca:

```vb
Dim json As String

banco.PixLimpar
banco.PixDados "sua-chave-pix", 25.5, 3600, "Pedido 123", "Cliente", "12345678909", "TXID123"
json = banco.PixCriar()

If banco.Sucesso Then
    If banco.QRCode <> "" Then
        Call banco.PixExibirUltimoQRCode()
    Else
        MsgBox "A cobranca foi criada, mas a API nao retornou o QR Code em QRCode. Consulte o JSON: " & json
    End If
Else
    MsgBox "Nao foi possivel criar a cobranca: " & json
End If
```

Quando o banco devolver o copia-e-cola em outro campo do JSON, extraia esse texto e passe-o diretamente ao formulario:

```vb
Dim pixCopiaECola As String
pixCopiaECola = "000201..." ' Valor extraido do JSON retornado pela API

If Not banco.PixExibirQRCode(pixCopiaECola) Then
    MsgBox "Codigo PIX vazio"
End If
```

`PixExibirUltimoQRCode()` deve ser chamado logo apos a operacao que preencheu `QRCode`, pois uma nova operacao limpa as propriedades de retorno. O formulario e modal: a execucao VB6 continua apos o usuario fecha-lo.

### Salvar PDF do boleto

Quando a API retornar o campo `pdfBoleto` em Base64, configure antes de emitir o boleto:

```vb
banco.CriarArquivoPDF = True
banco.CaminhoArquivoPDF = "C:\boletos"
json = banco.BoletoEmitir()

If banco.ArquivoPDFGerado <> "" Then
    MsgBox "PDF gravado em: " & banco.ArquivoPDFGerado
ElseIf banco.MensagemArquivoPDF <> "" Then
    MsgBox banco.MensagemArquivoPDF
End If
```

O nome é gerado como `{identificador}-{NossoNumero}.pdf`. Para bancos com conta corrente, o identificador é `NumeroContaCorrente`; no CrediSIS, que usa convênio em vez de conta corrente, é `CredisisConvenio`. Se o nosso número não estiver disponível no retorno, a DLL usa `boleto` no lugar dele. APIs que retornam apenas URL, e não PDF Base64, não permitem a gravação automática.

### Saldo e extrato

Saldo nao exige argumentos. Limpe o estado antes da consulta e chame `SaldoConsultar`:

```vb
banco.SaldoLimpar
jsonSaldo = banco.SaldoConsultar()
```

`ConsultarSaldoJson()` permanece disponivel como alternativa para chamadas baseadas em JSON.

Para extrato por periodo, informe as datas como strings no formato reconhecido pela DLL, preferencialmente `yyyy-MM-dd`:

```vb
banco.ExtratoLimpar
banco.ExtratoPorPeriodo "2024-11-06", "2024-11-20"
jsonExtrato = banco.ExtratoConsultar()
```

Assinatura COM/VB6: `ExtratoPorPeriodo(dataInicio As String, dataFim As String)`.

Tambem e possivel solicitar extrato pelo JSON:

```vb
jsonExtrato = banco.ConsultarExtratoJson("{""Inicio"":""2024-11-06T00:00:00"",""Fim"":""2024-11-20T23:59:59""}")
```

Use apenas bancos que disponibilizam essas operacoes: Sicoob, Bradesco, EFI (extrato) e Asaas. A configuracao especifica de conta esta descrita nas secoes de cada banco.

## Bancos implementados

| Banco | Codigo | Boleto | PIX | Extrato/Saldo |
| --- | ---: | --- | --- | --- |
| Banco do Brasil | `1` | Sim | PIX vinculado ao boleto | Nao |
| Banrisul | `41` | Sim | Sim, mTLS | Nao |
| Santander | `33` | Sim | PIX vinculado ao boleto | Sim |
| CrediSIS | `97` | Sim | Sim | Nao |
| CRESOL | `133` | Títulos por JSON CRESOL | Nao | Nao |
| Bradesco | `237` | Hibrido | Sim | Sim |
| Itaú | `341` | Consulta e boleto PIX | Sim | Extrato |
| EFI/Gerencianet | `364` | Sim | Sim | Extrato |
| Asaas | `461` | Sim | Sim | Sim |
| Sicredi | `748` | Sim | Sim | Nao |
| Sicoob | `756` | Hibrido | Sim | Sim |

## CRESOL - 133 (homologação)

A integração CRESOL usa somente os endpoints de homologação e o grant OAuth `PASSWORD`. A propriedade geral `homologacao` não alterna a CRESOL para produção. Boletos podem ser emitidos e consultados pelos métodos comuns, além das operações nomeadas `ICresol` que recebem/devolvem JSON no formato próprio da API. Não há suporte CRESOL a emissão em lote, PIX ou extrato.

### Interface COM e configuração

As propriedades específicas são publicadas pela interface COM explícita `ICresol`, implementada pelo coclass legado `cUtil`. Os membros CRESOL não alteram a interface legada `_cUtil`. No VB6, adicione uma referência à typelib `AutomacaoBancaria`, use `cUtil` para os métodos comuns e `Configuracao(133)`, e faça cast/atribuição para `ICresol` para configurar a conta CRESOL:

```vb
Dim util As AutomacaoBancaria.cUtil
Dim cresol As AutomacaoBancaria.ICresol
Dim retorno As String

Set util = New AutomacaoBancaria.cUtil
Set cresol = util

cresol.CresolUsuario = usuarioConfigurado
cresol.CresolSenha = senhaConfigurada
cresol.CresolAccountId = accountIdConfigurado ' Opcional; necessário para algumas contas multi-account
cresol.CresolIdEspecie = 1 ' Informe o idEspecie liberado para a conta

If Not util.Configuracao(133) Then
    MsgBox "Não foi possível configurar o banco CRESOL"
    Exit Sub
End If
' A configuração está pronta; veja abaixo os exemplos completos de emissão e consulta.
```

Informe `CresolUsuario` e `CresolSenha` em tempo de execução por mecanismo seguro da aplicação; não os grave no fonte, em arquivos de configuração distribuídos ou em logs. São constantes internas da DLL, conforme o Guia BC 1739: API `https://api-dev.governarti.com.br/`, token `https://auth-dev.governarti.com.br/auth/realms/cresol/protocol/openid-connect/token`, `grant_type=password`, `client_id=cresolApi`, `client_secret=cr3s0l4p1` e `scope=read`. Não há seleção de grant, certificado mTLS ou configuração de endpoint pela aplicação. O `client_secret` é um valor publicado no Guia, não uma credencial privada por cooperado.

| Propriedade de `ICresol` | Uso |
| --- | --- |
| `CresolUsuario`, `CresolSenha` | Usuário e senha do cooperado, obrigatórios para autenticar no grant `PASSWORD`. |
| `CresolAccountId` | Header `X-Account-Id`, enviado quando informado; pode ser necessário para contas com acesso a mais de uma conta. |
| `CresolIdEspecie` | Código inteiro positivo `idEspecie`, obrigatório na emissão pelo método comum. |
| `CresolCooperativa` | Propriedade mantida na interface para configuração do código da cooperativa; não é enviada no fluxo `PASSWORD`. |

O token é mantido em memória e renovado antes de expirar. Não há fluxo `CLIENT_CREDENTIALS_MTLS`, `implicit`, refresh token ou fallback para outro grant.

### Operações disponíveis

Os métodos comuns `BoletoEmitir` e `BoletoConsultar` retornam o JSON padrão da DLL com `Sucesso`, `Mensagem`, `Dados` e/ou `StatusCode`; `util.JsonRetorno` contém a mesma resposta e `util.Sucesso` indica o resultado. Na emissão comum, a resposta bruta CRESOL fica em `Dados`.

Exemplo de emissão de um boleto simples:

```vb
util.BoletoLimpar
util.BoletoDados "", "PED-123", "DOC-123", 150.5, "2026-12-31"
util.PagadorDados "Cliente Teste", "12345678909", "", "", "", _
    "Rua A", "100", "Centro", "Curitiba", "PR", "80000-000"
retorno = util.BoletoEmitir()
If Not util.Sucesso Then MsgBox retorno
```

`BoletoEmitir` envia um título por chamada. `CresolIdEspecie` deve estar configurado antes da emissão; `dtDocumento` é preenchida automaticamente com `DateTime.Today`. O campo `numeroDocumento`, dados cadastrais/endereço do pagador, CEP, valor e vencimento são obrigatórios. O CEP pode conter máscara, mas precisa resultar em dígitos. `NossoNumero` pode ficar vazio para geração pela API ou conter um inteiro positivo. O número do endereço é enviado quando informado. Os demais campos comuns que não fazem parte do mapeamento CRESOL (por exemplo, itens, instruções, desconto, multa, juros, PIX e opções de PDF) não são enviados. `BoletosEmitir`/emissão em lote e os métodos JSON genéricos continuam fora do fluxo comum CRESOL.

Para consulta, informe o ID para priorizar a rota de consulta direta. Sem ID, use nosso número e/ou o período; `dataInicio` e `dataFim` filtram a data de vencimento:

```vb
util.BoletoLimparConsulta
util.BoletoInformarConsulta "", "1001", "", "", 0, "2026-12-01", "2026-12-31"
retorno = util.BoletoConsultar()
If Not util.Sucesso Then MsgBox retorno
```

Quando o ID é informado, os demais filtros são ignorados. Sem ID, nosso número é enviado como `nn` e as datas como `dtVencimentoInicial`/`dtVencimentoFinal`. Linha digitável, código de barras, modalidade e segunda via não são critérios implementados para consulta CRESOL. Informe ao menos ID, nosso número ou uma das datas do período.

As operações nomeadas de `ICresol` continuam disponíveis para chamadas de baixo nível em JSON CRESOL. Os wrappers retornam JSON serializado com `Sucesso`, `Mensagem`, `Dados` e/ou `StatusCode`; `Dados` preserva o corpo da API.

| Wrapper em `ICresol` | Uso |
| --- | --- |
| `CresolTitulosListar(filtrosJson)` | Lista títulos; filtros são um objeto JSON restrito às chaves publicadas no Swagger. |
| `CresolTituloConsultar(id)` | Consulta um título por ID. |
| `CresolTitulosIncluir(json)` | Envia o array JSON CRESOL; a resposta documentada pode ser HTTP `203`. Não há repetição automática. |
| `CresolTituloAlterar(id, json)` / `CresolTituloBaixar(id)` | Altera vencimento ou baixa um título. |
| `CresolPagadoresListar(filtrosJson)` / `CresolPagadorConsultar(id)` | Lista ou consulta pagadores. |
| `CresolPagadorIncluir(json)` / `CresolPagadorAlterar(id, json)` / `CresolPagadorExcluir(id)` | Mantém pagadores usando JSON CRESOL. |
| `CresolOcorrenciasSequenciais(dateStart, dateEnd)` / `CresolOcorrenciaConsultar(sequencial)` | Lista ocorrências por período ou consulta pelo sequencial. Informe as datas no formato `yyyy-MM-dd`. |

Exemplo de leitura de pagadores:

```vb
retorno = cresol.CresolPagadoresListar("{""page"":0,""size"":20,""ativo"":true}")
If Not util.Sucesso Then MsgBox retorno
```

Em mutações JSON, use somente registros descartáveis de homologação e payloads aceitos pelo Swagger vigente. A inclusão de título de baixo nível espera um array JSON; alteração exige `dtVencimento`. A DLL não faz retry automático de inclusão, alteração ou baixa. Em respostas de processamento pendente, consulte o título posteriormente.

## Banrisul - 041

Configuracao:

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.BanrisulBeneficiario = "codigo-beneficiario" ' Enviado em bergs-beneficiario
banco.BanrisulPartnerId = "partner-id"             ' Opcional, quando contratado
banco.BanrisulCoPartnerId = "co-partner-id"         ' Opcional, quando contratado
banco.ClientCertificado = "C:\certificados\banrisul.pfx" ' Obrigatorio para PIX mTLS
banco.ClientCertificadoSenha = "senha"
banco.homologacao = True
banco.Configuracao 41
```

A cobranca e os pagamentos usam OAuth2 `client_credentials` normal. Em homologacao, os defaults sao `https://apidev.banrisul.com.br/auth/oauth/v2/token` e `https://apidev.banrisul.com.br/cobranca/v1`; em producao, o host e `https://api.banrisul.com.br`. PIX usa OAuth e API separados com mTLS: `https://mtls-api-pix-homologacao.banrisul.com.br/pix/v2` no sandbox e `https://mtls-api-pix.banrisul.com.br/pix/v2` em producao.

Use `BoletoEmitir` e `BoletoConsultar` normalmente. Com `segundaVia=True`, a consulta solicita `GET /boletos/{id}/emitir`; quando o banco retornar Base64 em `pdfBoleto`, a opcao `CriarArquivoPDF` grava o arquivo como nas demais integracoes. PIX cria `POST /cob` ou `PUT /cob/{txid}` e consulta por `TxId`, `EndToEndId` ou periodo, sempre via mTLS.

As URLs podem ser substituidas pelas propriedades `BanrisulUrlAutenticacao`, `BanrisulUrlCobranca`, `BanrisulUrlPagamentos`, `BanrisulUrlPix` e `BanrisulUrlAutenticacaoPix`, para acomodar endpoints liberados no contrato/portal do parceiro.

### Pagamentos de terceiros

Os quatro metodos recebem o JSON exatamente conforme o produto de pagamentos contratado, evitando que a DLL descarte campos de versoes diferentes da API:

```vb
' Consulta dados e regras do titulo antes de pagar.
json = banco.BanrisulConsultarTituloJson("{\"codigo_barra\":\"00190500954014481606906809350314337370000000100\"}")

' Solicita, confirma e consulta o status usando os payloads do OpenAPI liberado ao parceiro.
json = banco.BanrisulSolicitarPagamentoJson("{\"...\":\"...\"}")
json = banco.BanrisulConfirmarPagamentoJson("{\"...\":\"...\"}")
json = banco.BanrisulConsultarStatusPagamentoJson("{\"...\":\"...\"}")
```

Os endpoints padrao do OpenAPI sao, respectivamente, `GET /pagamentos/v1/titulos/{codigo_barra}`, `POST /pagamentos/v1/titulos/{codigo_barra}`, `PATCH /pagamentos/v1/titulos/{codigo_barra}` e `GET /pagamentos/v1/status/{id_documento}`. Os tres primeiros payloads devem informar `codigo_barra`; o ultimo deve informar `id_documento`.

## Banco do Brasil - 001

Configuracao:

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.BBAppKey = "gw-dev-app-key"
banco.NumeroContrato = 1234567  ' Numero do convenio
banco.ClientCertificado = "C:\certificados\bb.pfx" ' Opcional, se exigido pela credencial
banco.ClientCertificadoSenha = "senha"
banco.Configuracao 1
```

Usa OAuth2 `client_credentials`, a `BBAppKey` como `gw-dev-app-key` e a API Cobrancas v2. Emite e consulta boletos. `CadastrarPix=True` solicita PIX vinculado; use o identificador do titulo retornado para gerar/consultar o PIX vinculado por JSON avancado.

## CrediSIS - 097

Configuracao:

```vb
banco.CredisisToken = "token"
banco.CredisisConvenio = "convenio"
banco.CredisisUrl = "https://credisiscobranca.com.br/v2/ws" ' Opcional: URL customizada
banco.Configuracao 97
```

Integra a cobranca CrediSIS via SOAP/XML. Use os metodos comuns para boleto e as operacoes PIX habilitadas pela credencial/convenio. Quando `NossoNumero` for informado, a DLL usa automaticamente o metodo SOAP `gerarBoletos`, conforme exigencia pratica do Web Service. Quando nao for informado, usa `gerarBoleto` e permite que o CrediSIS gere o nosso numero. Erros de negocio retornados no XML SOAP agora fazem `Sucesso=False`, mesmo quando o HTTP retorna `200`.

Na consulta CrediSIS, use o `NossoNumero` completo retornado pelo registro ou o `idWeb`. `CodigoBoleto` corresponde ao `numeroSequencial` interno e não é o identificador aceito pelo método `buscarBoleto`.

## Santander - 033

### Configuração para boletos

Para emissão e consulta de boletos Santander, informe obrigatoriamente:

| Propriedade | Obrigatória | Uso |
| --- | --- | --- |
| `ClientID` | Sim | Identificação OAuth2 e valor de `X-Application-Key`. |
| `ClientSecret` | Sim | Segredo OAuth2. |
| `SantanderWorkspaceId` | Sim | Workspace usado no registro e consulta por ID. |
| `SantanderCovenantCode` | Sim | Código do convênio/beneficiário. |
| `SantanderBankNumber` | Sim | Identificador `bankNumber` enviado no registro. |
| `homologacao` | Sim | Use `True` para sandbox e `False` para produção. |
| `CertificadoDigitalBase64` | Conforme credencial | PFX/P12 Base64 para mTLS. Recomendado quando o portal associar certificado ao ClientID. |
| `CertificadoDigitalSenha` | Conforme credencial | Senha do PFX/P12. |

Propriedades opcionais:

| Propriedade | Uso |
| --- | --- |
| `SantanderParticipantCode` | Código do participante, quando exigido pelo convênio. |
| `SantanderPixChave` | Chave PIX para boleto híbrido. Obrigatória quando `CadastrarPix=True`. |
| `SantanderPixChaveTipo` | Tipo da chave PIX: `EMAIL`, `CPF`, `CNPJ`, `CELULAR` ou `ALEATORIA`. |
| `SantanderBaseUrl` | Sobrescreve a URL padrão do ambiente. |
| `SantanderUrlAutenticacao` | Sobrescreve a URL padrão do token. |

`SantanderCnpjBanco` e `SantanderAgenciaConta` não são necessários para emissão ou consulta de boletos; são usados apenas nas consultas de saldo e extrato.

Exemplo mínimo para homologação:

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.SantanderWorkspaceId = "workspace-id"
banco.SantanderCovenantCode = "codigo-convenio"
banco.SantanderBankNumber = "numero-banco"
banco.SantanderParticipantCode = "codigo-participante"
banco.SantanderPixChave = "chave-pix" ' Necessaria somente para boleto hibrido
banco.SantanderPixChaveTipo = "EMAIL" ' CPF, CNPJ, CELULAR, EMAIL ou ALEATORIA conforme a chave
banco.CertificadoDigitalBase64 = "base64-do-pfx-ou-p12"
banco.CertificadoDigitalSenha = "senha-do-certificado"
banco.homologacao = True
banco.Configuracao 33
```

Usa OAuth2 `client_credentials`, `Authorization: Bearer` e o header `X-Application-Key` com o `ClientID`. Quando o portal exigir mTLS, informe `CertificadoDigitalBase64` e `CertificadoDigitalSenha`; o certificado é usado no token e nas chamadas da API. A URL padrao e `https://trust-open.api.santander.com.br`; com `homologacao=True`, a DLL usa `https://trust-sandbox.api.santander.com.br`. Altere `SantanderBaseUrl` e `SantanderUrlAutenticacao` se o Santander fornecer URLs distintas para o contrato.

`BoletoEmitir` registra o titulo no workspace. Para boleto hibrido, defina `CadastrarPix=True` em `BoletoDados` e informe `SantanderPixChave`. `BoletoConsultar` consulta pelo ID retornado pelo workspace ou pelo número informado; quando o número é informado, a DLL usa a rota detalhada `/bills/{bankNumber}.{covenantCode}` com `tipoConsulta=default`. A resposta Santander usa `barcode`, `digitableLine` e `qrCodePix`; a DLL normaliza esses campos para `CodigoBarras`, `LinhaDigitavel` e `QRCode`. A colecao suporta baixa e segunda via, mas nao documenta retorno/schema suficiente para expor esses fluxos pelo modelo simplificado atual. Nao ha endpoint de PIX avulso.

Exemplo de emissão:

```vb
banco.BoletoLimpar
banco.BoletoDados "", "PED-123", "DOC-123", 150.5, "2026-12-31", 1, False, False
banco.PagadorDados "Cliente Teste", "12345678909", "cliente@teste.com", "11999999999", "", "Rua A", "100", "Centro", "Sao Paulo", "SP", "01001000"
json = banco.BoletoEmitir()
```

Exemplo de consulta pelo identificador numérico Santander:

```vb
banco.BoletoLimparConsulta
banco.BoletoInformarConsulta "", "1014", "", "", 0, "", "", False
json = banco.BoletoConsultar()
```

Nesse exemplo, a DLL chama `/bills/1014.3567206?tipoConsulta=default`. Substitua `1014` pelo `bankNumber` válido do título/convênio Santander. Para consulta por ID do workspace, informe esse ID no primeiro parâmetro de `BoletoInformarConsulta`.

### Saldo e extrato

As coleções Santander de conta corrente não declaram OAuth ou headers para essas consultas. A DLL segue esse contrato e executa as chamadas sem autenticação adicional, preservando quaisquer cabeçalhos avançados enviados por JSON.

```vb
' Saldo
banco.SaldoLimpar
jsonSaldo = banco.SaldoConsultar()

' Extrato
banco.ExtratoLimpar
banco.ExtratoPorPeriodo "2024-11-01", "2024-11-30"
jsonExtrato = banco.ExtratoConsultar()
```

O saldo usa `GET /bank_account_information/v1/banks/{CNPJ}/balances/{agencia.conta}`. O extrato usa `GET /bank_account_information/v1/banks/{CNPJ}/statements/{agencia.conta}` com `initialDate`, `finalDate`, `_offset=1` e `_limit=50`.

## Bradesco - 237

### Boleto hibrido

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.ClientCertificado = "C:\certificados\bradesco.pfx"
banco.ClientCertificadoSenha = "senha"
banco.BradescoCodigoUsuario = "APISERVIC"
banco.BradescoCpfCnpjBeneficiario = "12345678"
banco.BradescoFilialBeneficiario = "0001"
banco.BradescoControleBeneficiario = "90"
banco.BradescoProduto = 9
banco.BradescoNegociacao = "12345678901"
banco.BradescoAgencia = "3750"
banco.BradescoContaCorrente = "75557"
banco.BradescoTipoConta = "cc"
banco.Configuracao 237
```

O boleto hibrido usa mTLS, `gerarBoleto` e `consultar`. Defina `CadastrarPix=True` em `BoletoDados` para solicitar QR Code PIX junto ao boleto. Os dados de contrato e certificado sao obrigatorios para essas operacoes.

### PIX

A API PIX usa outro OAuth e nao exige os dados de contrato/mTLS do boleto:

```vb
banco.ClientID = "client-id-pix"
banco.ClientSecret = "client-secret-pix"
banco.Configuracao 237
```

`PixCriar` cria cobranca imediata em `/v2/cob`; quando `TxId` for informado, usa `PUT /v2/cob/{txid}`. `PixConsultar` consulta por `TxId`, periodo ou `EndToEndId` recebido.

### Saldo e extrato

As consultas de conta exigem o mesmo `ClientID`, `ClientSecret` e certificado mTLS do Bradesco, mais `BradescoAgencia` e `BradescoContaCorrente`. `BradescoTipoConta` deve permanecer `"cc"` para conta corrente.

```vb
' Saldo
banco.SaldoLimpar
json = banco.SaldoConsultar()

' Extrato de 6 a 20 de novembro de 2024
banco.ExtratoLimpar
banco.ExtratoPorPeriodo "2024-11-06", "2024-11-20"
json = banco.ExtratoConsultar()
```

O extrato chama `GET /v1/fornecimento-extratos-contas/extratos` com `agencia`, `conta`, `tipo=cc`, `dataInicio` e `dataFim` no formato `ddMMyyyy`. O saldo chama `GET /v1/fornecimento-saldos-contas/saldos`. A autenticacao usa mTLS e `auth/server-mtls/v2/token` no sandbox, ou `v1/token` em producao.

## Itaú - 341

As coleções Itaú fornecidas usam OAuth2 `client_credentials`, `x-itau-apikey` e `x-itau-correlationID`. O token sandbox padrão é `https://sandbox.devportal.itau.com.br/api/oauth/jwt`. Para produção, configure a URL de autenticação e as URLs dos produtos fornecidas pelo portal Itaú, pois os arquivos não definem todos os domínios operacionais.

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.ItauApiKey = "api-key"
banco.ItauIdBeneficiario = "id-beneficiario"
banco.ItauCodigoCarteira = "carteira"
banco.ItauFrancesaId = "id-francesa"
banco.PIXChave = "sua-chave-pix"

' URLs recebidas no portal Itaú para o ambiente contratado
banco.ItauBaseUrlPix = "https://.../regulatorio-pix/v2"
banco.ItauBaseUrlRecebimentos = "https://.../recebimentos-pix/v1"
banco.ItauBaseUrlExtrato = "https://.../extrato/v1"
banco.ItauBaseUrlBoletosCash = "https://secure.gateway.api.itau.com.br/boletoscash/v2"
banco.ItauUrlAutenticacao = "https://.../api/oauth/jwt" ' Obrigatoria em producao
banco.Configuracao 341
```

`PixCriar` cria cobrança imediata em `/cob` ou, quando houver `TxId`, em `/cob/{txid}`. `PixConsultar` consulta cobrança por `TxId`, lista por período ou consulta PIX recebido por `EndToEndId`.

`BoletoEmitir` gera boleto PIX em `/boletos-pix`; a emissão convencional não está documentada nas coleções recebidas. `BoletoConsultar` usa Boletos Cash `/boletos`, exigindo `ItauIdBeneficiario` e normalmente `ItauCodigoCarteira`.

Para extrato, informe `ItauFrancesaId`, defina o período e execute:

```vb
banco.ExtratoLimpar
banco.ExtratoPorPeriodo "2024-11-01", "2024-11-30"
json = banco.ExtratoConsultar()
```

Não há endpoint de saldo nas coleções Itaú analisadas; `SaldoConsultar()` retornará uma mensagem de operação não suportada.

## EFI/Gerencianet - 364

Configuracao:

```vb
banco.ClientID = "client-id"
banco.ClientSecret = "client-secret"
banco.ClientCertificado = "C:\certificados\efi.p12"
banco.ClientCertificadoSenha = "senha"
banco.Configuracao 364
```

Suporta boleto, cobranca PIX, consulta PIX e extrato conforme as permissoes da conta EFI. O certificado pode ser informado tambem em `ClientCertificadoBase64`.

## Asaas - 461

Configuracao:

```vb
banco.AsaasApiKey = "sua-api-key"
banco.AsaasUserAgent = "MinhaAplicacao/1.0" ' Opcional
banco.Configuracao 461
```

Caso `AsaasApiKey` esteja vazia, a DLL usa `ClientSecret` como chave. Suporta boleto/cobranca, PIX, extrato e saldo.

## Sicredi - 748

Configuracao:

```vb
banco.SicrediApiKey = "api-key"
banco.SicrediCodigoAcesso = "codigo-acesso"
banco.SicrediCodigoBeneficiario = "codigo-beneficiario"
banco.SicrediCooperativa = "cooperativa"
banco.SicrediPosto = "posto"
banco.homologacao = True ' Sandbox; False em producao
banco.Configuracao 748
```

`SicrediApiKey` é o valor do header `x-api-key` da APP correspondente ao ambiente. `SicrediCodigoAcesso` é o código gerado no Internet Banking para o convênio; a DLL monta o usuário de autenticação concatenando `SicrediCodigoBeneficiario` + `SicrediCooperativa`. `SicrediCooperativa` e `SicrediPosto` também são enviados como headers nas operações de cobrança.

Em sandbox, a DLL usa `https://api-parceiro.sicredi.com.br/sb`; em produção, usa `https://api-parceiro.sicredi.com.br`. O token é obtido em `/auth/openapi/token` com OAuth2 `password`, `scope=cobranca`, `context=COBRANCA` e `Content-Type=application/x-www-form-urlencoded`. A integração força TLS 1.2, requisito do endpoint Sicredi em processos VB6/.NET Framework antigos.

### Valores específicos do sandbox

Conforme o manual da API de Cobrança Sicredi 3.9.1, a autenticação de validação usa os valores fixos abaixo:

Quando `homologacao=True`, a DLL ignora qualquer valor informado pelo usuário nesses quatro campos e aplica automaticamente os valores sandbox abaixo. A `SicrediApiKey` continua obrigatória e deve ser a chave/token de homologação fornecida pelo Portal do Desenvolvedor.

| Campo enviado | Valor sandbox | Configuração na DLL |
| --- | --- | --- |
| `x-api-key` | Chave/token de acesso fornecido pelo Portal do Desenvolvedor | `SicrediApiKey` |
| `username` | `123456789` | `SicrediCodigoBeneficiario=12345` + `SicrediCooperativa=6789` |
| `password` | `teste123` | `SicrediCodigoAcesso` |
| `cooperativa` | `6789` | `SicrediCooperativa` |
| `posto` | `03` | `SicrediPosto` |
| `codigoBeneficiario` | `12345` | `SicrediCodigoBeneficiario` |

No fluxo atual do BancoID `748`, `ClientID` e `ClientSecret` são propriedades genéricas usadas por outros bancos e não são enviados na autenticação Sicredi. Portanto, não os use como `SicrediCodigoAcesso` ou como parte do `username`.

Configuração de teste do sandbox:

```vb
banco.SicrediApiKey = "x-api-key-fornecida-pelo-portal"
banco.SicrediCodigoAcesso = "teste123"
banco.SicrediCodigoBeneficiario = "12345"
banco.SicrediCooperativa = "6789"
banco.SicrediPosto = "03"
banco.homologacao = True
banco.Configuracao 748
```

Integra boleto e boleto híbrido com PIX pelas APIs Sicredi. Confirme os códigos de acesso, beneficiário, cooperativa, posto e `x-api-key` emitidos para o ambiente correto antes de testar. O `x-api-key`, o código de acesso e as credenciais de sandbox são diferentes das credenciais de produção.

## Sicoob - 756

Configuracao:

```vb
banco.ClientID = "client-id"
banco.NumeroContrato = 1234567
banco.NumeroContaCorrente = 12345
banco.ClientCertificado = "C:\certificados\sicoob.pfx"
banco.ClientCertificadoSenha = "senha"
banco.Configuracao 756
```

Use `ClientCertificado` **ou** `ClientCertificadoBase64`, nunca os dois. A integracao usa OAuth2 e mTLS. Suporta boleto, segunda via, PIX, extrato e saldo. A consulta de boleto e segunda via usam somente `NossoNumero`; a DLL envia `NumeroContrato` como `numeroCliente` e a modalidade informada, ou `1` como padrão. Para segunda via, passe `True` no ultimo argumento de `BoletoInformarConsulta`. O retorno pode preencher `pdfBoleto` com Base64.

Para saldo, configure tambem `NumeroContaCorrente`, chame `SaldoLimpar` e depois `SaldoConsultar()`. Para extrato, informe as datas como strings ISO, por exemplo `ExtratoPorPeriodo "2024-11-01", "2024-11-30"`, e execute `ExtratoConsultar()`; a integracao usa mes e ano derivados da data inicial.

### Movimentação de cobrança

A movimentação Sicoob ocorre em três etapas. O banco deve habilitar os escopos `cobranca_boletos_solicitacao_movimentacao_incluir`, `cobranca_boletos_solicitacao_movimentacao_consultar` e `cobranca_boletos_solicitacao_movimentacao_download` para o `ClientID`.

```vb
' 1. Solicita o processamento de até dois dias.
' Tipo: 1 Entrada, 2 Prorrogação, 3 A vencer, 4 Vencido,
'       5 Liquidação, 6 Baixa.
json = banco.MovimentacaoSolicitar(5, "2024-11-01", "2024-11-02")

If banco.Sucesso Then
    codigoSolicitacao = banco.CodigoSolicitacaoMovimentacao
End If
```

```vb
' 2. Consulte até o processamento estar disponível.
json = banco.MovimentacaoConsultar(codigoSolicitacao)

If banco.Sucesso Then
    quantidade = banco.QuantidadeArquivosMovimentacao
    idsArquivosJson = banco.IdArquivosMovimentacao
    ' idsArquivosJson contém, por exemplo: [30025254,30025255]
End If
```

```vb
' 3. Baixe cada idArquivo retornado. O arquivo é um ZIP Base64.
banco.CaminhoArquivosMovimentacao = "C:\movimentacoes-sicoob"
json = banco.MovimentacaoBaixar(codigoSolicitacao, 30025254)

If banco.ArquivoMovimentacaoGerado <> "" Then
    MsgBox "ZIP gravado em: " & banco.ArquivoMovimentacaoGerado
ElseIf banco.MensagemArquivoMovimentacao <> "" Then
    MsgBox banco.MensagemArquivoMovimentacao
End If
```

`NumeroContrato` é enviado como `numeroCliente`. O período máximo é de dois dias. O ZIP é descompactado automaticamente e seus conteúdos ficam disponíveis em:

```vb
jsonArquivos = banco.JsonArquivosMovimentacao
nomes = banco.NomesArquivosMovimentacao
conteudos = banco.ConteudosArquivosMovimentacao
```

`JsonArquivosMovimentacao` é um array JSON com `nomeArquivo` e `conteudo`. Os arrays `NomesArquivosMovimentacao` e `ConteudosArquivosMovimentacao` são arrays de strings compatíveis com VB6. Quando `CaminhoArquivosMovimentacao` estiver preenchido, a DLL grava o ZIP e cada arquivo extraído nesse mesmo diretório.

## Certificados

`ClientCertificado` recebe o caminho do PFX/P12. `ClientCertificadoBase64` recebe o arquivo inteiro codificado em Base64, com ou sem prefixo `data:`. `ClientCertificadoSenha` recebe a senha correspondente. Nao grave senhas ou certificados no codigo-fonte.

Para carregar o certificado e preencher automaticamente `ClientCertificadoBase64`, use:

```vb
' Abre a janela para selecionar um arquivo .pfx ou .p12.
If Not banco.CertificadoSelecionar() Then
    MsgBox "Certificado não selecionado"
Else
	certificadoBase64 = banco.ClientCertificadoBase64   ' Exemplo: gravar certificadoBase64 no banco de dados da aplicação	
End If

' Ou informe diretamente o caminho do arquivo.
If Not banco.CertificadoCarregarBase64("C:\certificados\empresa.pfx") Then
    MsgBox "Certificado não encontrado"
Else
	certificadoBase64 = banco.ClientCertificadoBase64   ' Exemplo: gravar certificadoBase64 no banco de dados da aplicação	
End If

banco.ClientCertificadoSenha = "senha-do-certificado"
```

O conteúdo Base64 fica disponível em `ClientCertificadoBase64` para envio, armazenamento seguro ou configuração posterior da integração. O método limpa `ClientCertificado` para respeitar bancos que aceitam caminho ou Base64, mas não os dois ao mesmo tempo.

## Observacoes

- Os ambientes de homologacao dependem de credenciais emitidas para sandbox pelo banco.
- Cada banco possui regras proprias de formato para nosso numero, convenio, produto, carteira e documentos. Valide esses dados com o banco.
- A propriedade pública `NossoNumero` e o campo de entrada de `BoletoInformarConsulta` são strings para preservar zeros à esquerda e identificadores maiores que `Int32`.
