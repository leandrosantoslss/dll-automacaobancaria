# Manual de Integração via Web Service — CrediSIS Cobrança

> Documento convertido do PDF **MANUAL_WEB_SERVICE.pdf**, referente ao **banco 097 — Credisis**.
| Campo | Informação |
|---|---|
| Sistema | CrediSIS Cobrança |
| Escopo | Integração via Web Service para serviços de cobrança e boletos |
| Versão identificada | Abril de 2026 |
| Autor registrado | Heberton Souza |
| Banco | 097 — Credisis |

## Sumário

- [Conheça Nossas Cooperativas](#conheça-nossas-cooperativas)
- [Contextualização](#contextualização)
- [1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO](#1-arquitetura-de-comunicação-com-o-cooperado)
  - [2.1. MODELO CONCEITUAL](#21-modelo-conceitual)
  - [2.2. EMISSÃO DOS BOLETOS](#22-emissão-dos-boletos)
  - [2.3. CONSULTA BOLETO](#23-consulta-boleto)
  - [2.4. BAIXA MANUAL / CANCELAMENTO DOS BOLETOS](#24-baixa-manual-cancelamento-dos-boletos)
- [3. PADRÃO DE COMUNICAÇÃO](#3-padrão-de-comunicação)
  - [3.1. PADRÃO DE MENSAGENS](#31-padrão-de-mensagens)
- [4. ESTRUTURA DE DADOS DO WEB SERVICE](#4-estrutura-de-dados-do-web-service)
  - [4.1. MODELO OPERACIONAL](#41-modelo-operacional)
  - [4.2. SERVIÇOS SÍNCRONOS](#42-serviços-síncronos)
- [5. ENDEREÇO DO WEB SERVICE](#5-endereço-do-web-service)
  - [5.1. PARÂMETROS DE ENTRADA:](#51-parâmetros-de-entrada)
  - [5.2. PARÂMETROS DE SAÍDA:](#52-parâmetros-de-saída)
  - [5.3. Métodos de Requisições e retorno ao Credisis Cobrança](#53-métodos-de-requisições-e-retorno-ao-credisis-cobrança)
    - [5.3.1. Método alterarBoleto](#531-método-alterarboleto)
      - [5.3.1.1. Retorno da requisição do método alterarBoleto](#5311-retorno-da-requisição-do-método-alterarboleto)
    - [5.3.2. Método baixarBoleto](#532-método-baixarboleto)
      - [5.3.2.1. Retorno da requisição do método baixarBoleto](#5321-retorno-da-requisição-do-método-baixarboleto)
    - [5.3.3. Método buscarBoleto](#533-método-buscarboleto)
      - [5.3.3.1. Retorno da requisição do método buscarBoleto](#5331-retorno-da-requisição-do-método-buscarboleto)
    - [5.3.4. Método buscarBoletos](#534-método-buscarboletos)
      - [5.3.4.1. Retorno da requisição do método buscarBoletos](#5341-retorno-da-requisição-do-método-buscarboletos)
    - [5.3.5. Método gerarBoleto](#535-método-gerarboleto)
      - [5.3.5.1. Retorno da requisição do método gerarBoleto](#5351-retorno-da-requisição-do-método-gerarboleto)
    - [5.3.6. Método gerarBoletos](#536-método-gerarboletos)
      - [5.3.6.1. Retorno da requisição do método gerarBoletos](#5361-retorno-da-requisição-do-método-gerarboletos)
- [6. FORMATOS E PADRÕES UTILIZADOS](#6-formatos-e-padrões-utilizados)
- [7. LAYOUT DE ARQUIVOS XML](#7-layout-de-arquivos-xml)
  - [7.1. LAYOUT DE ENTIDADES](#71-layout-de-entidades)
    - [7.1.1. CHAVE](#711-chave)
    - [7.1.2. PERIODO](#712-periodo)
    - [7.1.3. ENDERECO](#713-endereco)
    - [7.1.4. PESSOA](#714-pessoa)
    - [7.1.5. CONTATO](#715-contato)
    - [7.1.6. DESCONTO](#716-desconto)
    - [7.1.7. CARENCIA](#717-carencia)
    - [7.1.8. MULTA](#718-multa)
    - [7.1.9. JUROS](#719-juros)
    - [7.1.10. OPERACAO](#7110-operacao)
    - [7.1.11. ERRO](#7111-erro)
    - [7.1.12. TITULOREQUEST](#7112-titulorequest)
    - [7.1.13. TITULORESPONSE](#7113-tituloresponse)
    - [7.1.14. BOLETOREQUEST](#7114-boletorequest)
    - [7.1.15. BOLETORESPONSE](#7115-boletoresponse)
    - [7.1.18. BOLETOSREQUEST](#7118-boletosrequest)
    - [7.1.19. BAIXARBOLETOREQUEST](#7119-baixarboletorequest)
    - [7.1.20. BAIXARBOLETORESPONSE](#7120-baixarboletoresponse)
- [8. INTERFACE DE COMUNICAÇÃO](#8-interface-de-comunicação)

---

## Conheça nossas cooperativas

Nossas cooperativas de crédito são os canais, do Sistema CrediSIS, que levam soluções financeiras aos cooperados nas mais diversas realidades, sempre fomentando o cooperativismo e contribuindo para o desenvolvimento das comunidades onde estão inseridas. Nosso objetivo diário é expandir nosso alcance, contribuindo para que os horizontes de nossos cooperados sejam cada vez mais amplos. Alcançamos esse propósito ao oferecer soluções financeiras alinhadas às necessidades de seus negócios. Dessa forma, promovemos o desenvolvimento das pessoas, o crescimento das empresas e o fortalecimento da comunidade. Localize sua Cooperativa mais próxima, acessando o link:

`https://credisis.com.br/cooperativas/`

0002 – CREDISIS JICRED 0004 – CREDISIS SUDOESTE 0005 – CREDISIS CREDIARI

0009 – CREDISIS OESTE 0011 – CREDISIS CAPITALCREDI 0012 – CREDISIS CREDIBRAS

0017 – CREDISIS CREDBEM 0020 – CREDISIS COOPERUFPA 0022 – CREDISIS COOPESA

0023 – CREDISIS CREDIPLAN 0024 – CREDISIS CREDISUL 0025 – CREDISIS PRIMACREDI

0026 – CREDISIS CRESERV 0027 – CREDISIS UNINDUSTRIA

## Contextualização

Este manual do aplicativo Web Service tem como objetivo apresentar as especificações e os critérios técnicos necessários para a integração de informações entre o sistema desenvolvido pelo Beneficiário e o Sistema de Cobrança CrediSIS. Essa integração é realizada por meio de uma API disponibilizada no acesso do Cooperado pelo site. Após a geração da chave de acesso, o sistema do Cooperado se conecta ao aplicativo Web Service, permitindo a troca de informações online. Por meio dessa integração, é possível automatizar os processos de geração, consulta, cancelamento, alteração e baixa de boletos. O boleto padronizado do Sistema CrediSIS é transmitido diretamente ao aplicativo Web Service, sendo necessário apenas que o Beneficiário utilize uma impressora a laser para garantir a impressão legível do código de barras, evitando divergências ou recusas no momento da leitura. Além disso, o sistema contempla a emissão do boleto híbrido, que possibilita ao pagador realizar o pagamento tanto por meio do código de barras tradicional quanto por QR Code PIX. Essa funcionalidade amplia as opções de pagamento, proporcionando maior comodidade, agilidade e segurança ao pagador, além de contribuir para a redução de inadimplência e para a otimização do processo de liquidação dos títulos. Em razão do aumento significativo de inconformidades no produto Cobrança e da necessidade contínua de aprimoramento do serviço pelas instituições participantes do Sistema Financeiro Nacional, foi instituído um Grupo de Trabalho no âmbito da FEBRABAN. Esse grupo tem como finalidade discutir e implementar ações voltadas ao fortalecimento do produto, utilizando como base a infraestrutura segura do SFN, da Núclea/CIP e do DDA – Débito Direto Autorizado. Como resultado dessas iniciativas, foram adotadas medidas de segurança e validações mais rigorosas, com foco na mitigação de fraudes, na prevenção de pagamentos duplicados e na elevação dos níveis de confidencialidade, integridade, confiabilidade e rastreabilidade das informações constantes nos boletos.

O registro centralizado dos dados dos boletos em um banco nacional unificado permite que o pagador realize o pagamento em qualquer instituição financeira ou cooperativa, inclusive após o vencimento, com atualização automática dos valores. Além disso, a cobrança registrada oferece aos emissores beneficiários (Cooperados) maior controle e gestão dos boletos emitidos, possibilitando o acompanhamento detalhado e rastreável de todas as etapas do ciclo de vida do título. Essa estrutura também viabiliza a utilização de serviços complementares, como protesto em cartório e antecipação de recebíveis (títulos descontados), contribuindo para a otimização do fluxo de caixa e da gestão financeira do cooperado.

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

Nesta seção, serão apresentados os processos arquiteturais da integração entre o cooperado beneficiário e o CrediSIS Cobrança. No fluxograma abaixo é apresentado o fluxo macro da integração entre CrediSIS Cobrança e o Sistema do Cooperado através da integração via Web Service.

### 2.1. MODELO CONCEITUAL

Através do Web Service, o sistema de Boletos do CrediSIS disponibilizará serviços que poderão ser acessados pelos sistemas dos cooperados. A seguir, estão resumidos os serviços disponíveis e suas respectivas funcionalidades. No diagrama abaixo é possível verificar a representação da integração entre CrediSIS Cobrança e o Sistema do Cooperado, através dos métodos disponíveis na integração via Web Service.

### 2.2. EMISSÃO DOS BOLETOS

Este serviço compreende a emissão do Boleto CrediSIS. Após o recebimento da solicitação, o boleto será submetido a validações e, estando em conformidade, será gerado e armazenado em nossa base de dados. Caso o convênio emissor esteja parametrizado como híbrido, o boleto será emitido contendo tanto o código de barras quanto o QR Code PIX, permitindo múltiplas formas de pagamento. Concluído o processamento da emissão, e desde que todos os parâmetros da requisição estejam corretos, serão retornadas as informações de identificação do boleto, bem como um conteúdo em Base64 contendo a representação do boleto com o código de barras e, quando aplicável, o QR Code PIX. Caso seja identificada qualquer inconsistência na requisição, serão retornadas as respectivas mensagens de erro.

### 2.3. CONSULTA BOLETO

Esse serviço compreende a consulta de um boleto, quando efetuada a sua recepção, o Web Service tentará encontrar o boleto ou grupo de parcelas em questão, caso seja localizado, o mesmo será retornado, senão, será retornado os erros ocorridos. Através do mecanismo de consulta que o sistema irá retornar as informações e situação atual do boleto, desde a inclusão, até a liquidação do boleto pago via banco.

### 2.4. BAIXA MANUAL / CANCELAMENTO DOS BOLETOS

Esse serviço compreende a baixa manual ou cancelamento de um boleto válido, quando for realizado esse procedimento pelo cooperado através do aplicativo Web Service o mesmo será baixado ou cancelado na nossa base de dados do Cobrança e o Qr Code PIX inativado. No Sistema Credisis Cobrança, existem alguns status relacionados ao cancelamento ou à baixa de boletos, que podem ser retornados na consulta de um boleto. Em razão disso, a seguir serão apresentados todos esses status.

- Baixa Manual → Baixa realizada manualmente no sistema, sem que tenha

ocorrido a liquidação financeira do boleto. É utilizada quando o título precisa ser encerrado por decisão operacional do cooperado.

- Cancelamento → Cancelamento formal do boleto junto à Núclea – PCR,

impedindo sua liquidação. É utilizado quando o título não deve mais permanecer disponível para pagamento.

- Baixa por Pagamento → Baixa automática gerada a partir da liquidação do

boleto por meio do código de barras em uma instituição financeira.

- Baixa por Pix → Baixa automática gerada a partir da liquidação via QR

Code Pix vinculado ao boleto híbrido.

## 3. PADRÃO DE COMUNICAÇÃO

O meio de comunicação entre os sistemas de informação dos Cooperados e o Sistema de Boletos será a Internet, sendo necessária a identificação do usuário por meio de um token de autenticação. Esse token estará disponível para geração na aplicação de emissão de boletos CrediSIS Cobrança. A troca de mensagens entre o Web Service do Sistema de Boletos e o sistema do Cooperado será realizada utilizando o padrão SOAP, com mensagens no formato XML, adotando o estilo de codificação Document/Literal, wrapped. A opção wrapped indica que as chamadas aos métodos do serviço permitem a passagem de múltiplos parâmetros encapsulados em um único elemento XML. Para a descrição dos serviços disponibilizados, será utilizado um documento WSDL (Web Service Description Language), padrão recomendado para a especificação de serviços SOAP, contendo as definições das operações, parâmetros, estruturas de dados e pontos de acesso do Web Service.

As chamadas aos serviços serão feitas enviando como parâmetro um documento XML a ser processado pelo sistema.

### 3.1. PADRÃO DE MENSAGENS

As mensagens XML seguem a recomendação W3C para XML 1.0 (www.w3.org/TR/REC-xml), com codificação de caracteres em UTF-8. Chamadas e respostas do WebService do Credisis Boletos usam um padrão XML único, com cabeçalho definindo versão e codificação. A seguir, a estrutura do layout das mensagens de retorno.

Exemplo:

```xml
 <?xml version="1.0"?>
 <SOAP-ENV:Envelope
        xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
        xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
   <SOAP-ENV:Header
                       SOAP-ENV:encodingStyle=http://schemas.xmlsoap.org/soap/encoding/
        xmlns:NS1="urn:CredisisBoletoInterface">
        <NS1:token xsi:type="NS1:token">TOKEN_DE_ACESSO</NS1:token>
   </SOAP-ENV:Header>
   <SOAP-ENV:Body
        xmlns:NS2="urn:CredisisBoletoInterface-CredisisWebService"
        SOAP-ENV:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"
        xmlns:NS3="urn:CredisisBoletoInterface">
        parametros...
   </SOAP-ENV:Body>
 </SOAP-ENV:Envelope>
```

## 4. ESTRUTURA DE DADOS DO WEB SERVICE

Haverá um único Web Service que disponibilizará todos os serviços apresentados neste documento.

O fluxo de comunicação será sempre iniciado pelo sistema do cooperado, por meio do envio de uma mensagem XML ao Web Service, contendo a solicitação do serviço desejado.

### 4.1. MODELO OPERACIONAL

As operações realizadas por meio do Web Service geram registros que possuem um status associado, o qual indica a situação da operação durante o processamento e o registro junto à Núclea – PCR. A seguir, são apresentados os possíveis status das operações, suas definições funcionais e respectivas naturezas:

```text
Status                          Definição Funcional                                                Natureza
NAO_ENVIADO                     Operação ainda não enviada para                                    Transitório
                                registro na Núclea – PCR. Representa
                                o status inicial da operação após sua
                                geração no Web Service.
PENDENTE                        Operação          enviada        e     aguardando                  Transitório
                                processamento/registro na Núclea –
                                PCR.
REJEITADO                       Operação rejeitada pela Núclea – PCR                               Final
                                durante o processo de registro.
CONCLUIDO                       Operação processada e registrada com                               Final
                                sucesso na Núclea – PCR.

```

> **Observação:**

- O status NAO_ENVIADO representa o estado inicial da operação após sua

geração no Web Service, indicando que ainda não houve envio para registro na Núclea – PCR.

- Para operações de Inclusão, Alteração, Baixa Manual, Cancelamento e

Baixa Pix, esse status é transitório e evolui para PENDENTE, CONCLUIDO ou REJEITADO após o envio para a registradora.

- O status NAO_ENVIADO pode ser considerado final apenas para operações

que não dependam de registro externo, como Cartório ou Operação de Desconto (Operação de Crédito).

- O status PENDENTE indica que a operação foi enviada e encontra-se em

processamento na Núclea – PCR. Trata-se de um status transitório, que evolui para CONCLUIDO ou REJEITADO após o retorno da registradora.

- O status REJEITADO indica que a operação não foi aceita pela Núclea –

PCR durante o processo de registro. É um status final, sendo necessária a consulta à mensagem de erro retornada para análise e eventual correção.

- O status CONCLUIDO indica que a operação foi processada e registrada

com sucesso na Núclea – PCR, representando a consolidação definitiva da operação junto à registradora.

### 4.1. MODELO OPERACIONAL

A forma de processamento das solicitações de serviços no projeto CrediSIS Boletos será de forma síncrona, ou seja, a requisição será respondida na mesma conexão.

### 4.2. SERVIÇOS SÍNCRONOS

As solicitações de serviços de implementação síncrona são processadas imediatamente e o resultado do processamento é obtido em uma única conexão. Abaixo, o fluxo simplificado de funcionamento:

Etapas do processo ideal:

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

solicitação de serviço para o Web Service; 2. O Web Service recebe a mensagem de solicitação de serviço e encaminha ao aplicativo Boletos que irá processar o serviço solicitado;

## 3. PADRÃO DE COMUNICAÇÃO

o processamento, devolvendo uma mensagem de resultado do processamento ao Web Service;

## 4. ESTRUTURA DE DADOS DO WEB SERVICE

encaminha ao aplicativo do cooperado;

## 5. ENDEREÇO DO WEB SERVICE

processamento e caso não exista outra mensagem, encerra a conexão.

## 5. ENDEREÇO DO WEB SERVICE

O endereço do Web Service que o contribuinte deverá acessar, para realizar a comunicação com o sistema CrediSIS Boletos, deverá seguir o seguinte padrão:

`https://credisiscobranca.com.br/ws?wsdl`

`https://credisiscobranca.com.br/v2/ws?wsdl`

### 5.1. PARÂMETROS DE ENTRADA:

- “Operação” refere-se ao tipo de execução do Web Service.

- “Token” refere-se ao ApiKey, chave de 128 bytes gerada a partir do

sistema CrediSIS Cobrança WEB.

- “Arquivo XML” refere-se ao arquivo no formato citado neste documento,

contendo as informações referentes ao boleto.

### 5.2. PARÂMETROS DE SAÍDA:

- “Mensagem XML” refere-se ao arquivo de retorno que segue o modelo

citado neste documento como o padrão XML, onde serão retornadas as mensagens de erro encontradas na importação do arquivo.

### 5.3. Métodos de Requisições e retorno ao Credisis Cobrança

Nesta seção serão apresentados os métodos de requisição ao Credisis Cobrança e os retornos para a requisição de cada um destes métodos.

#### 5.3.1. Método alterarBoleto

O método alterarBoleto é indicado para quando o beneficiário deseja realizar alguma modificação nas informações do boleto, como: vencimento, data limite de pagamento, valor, entre outros. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
   <soapenv:Header>
      <urn:Chave>
         <token>?</token>
         <convenio>?</convenio>
      </urn:Chave>
   </soapenv:Header>
   <soapenv:Body>
      <urn1:alterarBoleto>
         <boleto>
            <idWeb>?</idWeb>
            <valor>?</valor>
            <protesto>
               <tipo>?</tipo>
               <dias>?</dias>
            </protesto>
            <dataVencimento>?</dataVencimento>
            <!--Optional:-->
            <instrucao>?</instrucao>
            <!--Optional:-->
            <multa>
               <valor>?</valor>
               <carencia>
                  <tipo>?</tipo>
                  <dias>?</dias>
               </carencia>
               <tipo>?</tipo>
            </multa>
            <!--Optional:-->
            <juros>
               <valor>?</valor>
               <carencia>
                  <tipo>?</tipo>
                  <dias>?</dias>
               </carencia>
               <tipo>?</tipo>
            </juros>
            <!--Optional:-->
            <desconto1>
               <valor>?</valor>
               <!--Optional:-->
               <data>?</data>
               <tipo>?</tipo>
            </desconto1>
            <!--Optional:-->
            <desconto2>
               <valor>?</valor>
               <!--Optional:-->
               <data>?</data>
               <tipo>?</tipo>
            </desconto2>
            <!--Optional:-->
            <desconto3>
               <valor>?</valor>
               <!--Optional:-->
               <data>?</data>
               <tipo>?</tipo>
            </desconto3>
            <documento>?</documento>
            <dataLimitePagamento>?</dataLimitePagamento>
         </boleto>
      </urn1:alterarBoleto>
   </soapenv:Body>
</soapenv:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

##### 5.3.1.1. Retorno da requisição do método alterarBoleto

Ao realizar a integração pelo método de alterarBoleto, em caso de sucesso o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:alterarBoletoResponse>
         <boleto>
<idWeb>512fb65a8d3e4b3bce407776bf063939c49bc7befa36d9f30e7e3a158ecafda00a2ba238ed74
d843e8ec960478a33625db61e88645ace334247f16e02eedab14</idWeb>
            <operacao>
               <operacao>ALTERACAO</operacao>
               <valor>0.2</valor>
               <observacao/>
               <sistema>WS</sistema>
               <status>NAO_ENVIADO</status>
               <dataHora>2026-02-13</dataHora>
            </operacao>
         </boleto>
      </ns1:alterarBoletoResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

#### 5.3.2. Método baixarBoleto

O método baixarBoleto é indicado para quando o beneficiário deseja realizar a operação de baixa do boleto, deixando como liquidado no CrediSIS Cobrança. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
   <soapenv:Header>
      <urn:Chave>
         <token>?</token>
         <convenio>?</convenio>
      </urn:Chave>
   </soapenv:Header>
   <soapenv:Body>
      <urn1:baixarBoleto>
         <boleto>
            <idWeb>?</idWeb>
            <valor>?</valor>
            <operacao>?</operacao>
         </boleto>
      </urn1:baixarBoleto>
   </soapenv:Body>
</soapenv:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

##### 5.3.2.1. Retorno da requisição do método baixarBoleto

Ao realizar a integração pelo método de baixarBoleto, em caso de sucesso o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:baixarBoletoResponse>
         <retorno>
            <operacao>
               <operacao>NOME DA OPERACAO</operacao>
               <valor>202</valor>
               <observacao/>
               <sistema>WS</sistema>
               <status>NAO_ENVIADO</status>
               <dataHora>2026-02-13</dataHora>
            </operacao>
         </retorno>
      </ns1:baixarBoletoResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

#### 5.3.3. Método buscarBoleto

O método buscarBoleto é indicado para quando o beneficiário deseja realizar alguma consulta de boleto, sendo permitido a consulta de apenas 1 (um) boleto por vez. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
   <soapenv:Header>
      <urn:Chave>
         <token>?</token>
         <convenio>?</convenio>
      </urn:Chave>
   </soapenv:Header>
   <soapenv:Body>
      <urn1:buscarBoleto>
         <boleto>
            <!--Optional:-->
            <idWeb>?</idWeb>
            <!--Optional:-->
            <grupoParcela>?</grupoParcela>
            <!--Optional:-->
            <nossonumero>?</nossonumero>
            <!--Optional:-->
            <layout>?</layout>
            <!--Optional:-->
            <formato>?</formato>
         </boleto>
      </urn1:buscarBoleto>
   </soapenv:Body>
</soapenv:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

##### 5.3.3.1. Retorno da requisição do método buscarBoleto

Ao realizar a integração pelo método de buscarBoleto, em caso de sucesso o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:buscarBoletoResponse>
         <boleto>
            <boleto>BASE 64 DO BOLETO</boleto>
            <titulos>
               <item>
<idWeb>83ba364a063a2113748d2c79fb1bc1c000fc0a8cf4f2fe465c4e743d7081361f34c4813ec80c
29b21d1ac7fa39db855365b4753498fe73e00c26f35bea016f11</idWeb>
<grupoParcela>9e791b48fbda446ff911ac3e896f773ab7862540793600d31324d02cbfcd1bf7d43bc
362d9dde7a9c66b3f7b3d035c5606b48e77d2397d01071e35277fe4377c</grupoParcela>
                  <pagador>
                     <nome>NOME DO COOPERADO</nome>
                     <cpfCnpj>CPF/CNPJ DO COOPERADO</cpfCnpj>
                     <identidade/>
                     <dataNascimento>1993-02-02</dataNascimento>
                     <endereco>
                        <endereco>rua a</endereco>
                            <bairro>teste</bairro>
                            <cep>76900344</cep>
                            <cidade>Ji-Paraná</cidade>
                            <uf>RO</uf>
                            <numero>10</numero>
                         </endereco>
                         <contatos/>
                      </pagador>
                      <numeroSequencial>479969</numeroSequencial>
                      <nossonumero>09710002009871479969</nossonumero>
<linhaDigitavel>09790000070971000200898714799693313850000000002</linhaDigitavel>
<codigoBarras>09793138500000000020000009710002009871479969</codigoBarras>
                  <documento>2</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-03-14</dataVencimento>
                  <dataLimitePagamento>2026-03-15</dataLimitePagamento>
                  <valor>0.02</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>1</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                      <valor>0</valor>
                      <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                      <tipo>2</tipo>
                      <dias>1</dias>
                  </protesto>
                  <operacoes>
                      <item>
                         <operacao>INCLUSAO</operacao>
                         <valor>0</valor>
                         <observacao/>
                         <sistema>WEB</sistema>
                         <status>NAO_ENVIADO</status>
                         <dataHora>2026-02-12</dataHora>
                      </item>
                  </operacoes>
                  <descricao/>
                   <pixCopiaCola/>00020101021226830014br.gov.bcb.pix2561pix-
pgtos.api.credisis.com.br/cobv/BOLETO097100020098710007285204000053039865802BR5924M
ONICA6009JI-PARANA61087690101562070503***6304996D</pixCopiaCola>
               </item>
            </titulos>
         </boleto>
      </ns1:buscarBoletoResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

#### 5.3.4. Método buscarBoletos

O método buscarBoletos é indicado para quando o beneficiário deseja a emissão de mais de um boleto na mesma requisição. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
   <soapenv:Header>
      <urn:Chave>
         <token>?</token>
         <convenio>?</convenio>
      </urn:Chave>
   </soapenv:Header>
   <soapenv:Body>
      <urn1:buscarBoletos>
         <boleto>
            <periodo>
               <inicio>?</inicio>
               <fim>?</fim>
               <!--Optional:-->
               <tipo>?</tipo>
            </periodo>
            <!--Optional:-->
            <operacoes>
               <!--1 or more repetitions:-->
               <item>?</item>
            </operacoes>
         </boleto>
      </urn1:buscarBoletos>
   </soapenv:Body>
</soapenv:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

##### 5.3.4.1. Retorno da requisição do método buscarBoletos

Ao realizar a integração pelo método de buscarBoletos, em caso de sucesso serão retornadas todas as operações realizadas no período em formato, o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:buscarBoletosResponse>
         <boleto>
            <titulos>
               <item>
<idWeb>f15f47b0f0474cdb3beb0d5c502ae9cb8ba13ac21a70fbba931b413520057bfafedac7a9a8d6
e16387da21a705c7e8cc5a544c71ddef4fad9e55edafbc4fdde3</idWeb>
<grupoParcela>9e791b48fbda446ff911ac3e896f773ab7862540793600d31324d02cbfcd1bf7d43bc
362d9dde7a9c66b3f7b3d035c5606b48e77d2397d01071e35277fe4377c</grupoParcela>
                  <pagador>
                     <nome>Nome Cooperado</nome>
                     <cpfCnpj>CPF/CNPJ DO COOPERADO</cpfCnpj>
                     <identidade/>
                     <dataNascimento>1993-02-02</dataNascimento>
                     <endereco>
                        <endereco>rua a</endereco>
                            <bairro>teste</bairro>
                            <cep>76900344</cep>
                            <cidade>Ji-Paraná</cidade>
                            <uf>RO</uf>
                            <numero>10</numero>
                         </endereco>
                         <contatos/>
                      </pagador>
                      <numeroSequencial>479966</numeroSequencial>
                      <nossonumero>09710002009871479966</nossonumero>
<linhaDigitavel>09790000070971000200898714799669113850000000001</linhaDigitavel>
<codigoBarras>09791138500000000010000009710002009871479966</codigoBarras>
                  <documento>1</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-03-14</dataVencimento>
                  <dataLimitePagamento>2026-03-15</dataLimitePagamento>
                  <valor>0.01</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <instrucao>Teste</instrucao>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>1</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>false</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                     <desconto3>
                         <valor>0</valor>
                         <tipo>3</tipo>
                     </desconto3>
                     <protesto>
                         <tipo>2</tipo>
                         <dias>1</dias>
                     </protesto>
                     <operacoes>
                         <item>
                            <operacao>BAIXA_PIX</operacao>
                            <valor>0.01</valor>
                            <observacao/>
                            <sistema>WS</sistema>
                            <status>NAO_ENVIADO</status>
                            <dataHora>2026-02-12</dataHora>
                            <dataCredito>2026-02-12</dataCredito>
                         </item>
                         <item>
                            <operacao>INCLUSAO</operacao>
                            <valor>0</valor>
                            <observacao/>
                            <sistema>WEB</sistema>
                            <status>NAO_ENVIADO</status>
                            <dataHora>2026-02-12</dataHora>
                         </item>
                     </operacoes>
                     <descricao/>
                  </item>
                  <item>
<idWeb>5738977e8bee3883338ef1f8c6c318c819c30ab14b0540011658523b114b69bedd059bb3295d
1d2172c3851221b7cf7caab8c8edb763fd326a81c6669848d023</idWeb>
<grupoParcela>9e791b48fbda446ff911ac3e896f773ab7862540793600d31324d02cbfcd1bf7d43bc
362d9dde7a9c66b3f7b3d035c5606b48e77d2397d01071e35277fe4377c</grupoParcela>
                  <pagador>
                     <nome>NOME DO COOPERADO</nome>
                     <cpfCnpj>CPF/CNPJ DO COOPERADO</cpfCnpj>
                     <identidade/>
                     <dataNascimento>1993-02-02</dataNascimento>
                     <endereco>
                        <endereco>rua a</endereco>
                        <bairro>teste</bairro>
                        <cep>76900344</cep>
                        <cidade>Ji-Paraná</cidade>
                        <uf>RO</uf>
                        <numero>10</numero>
                         </endereco>
                         <contatos/>
                      </pagador>
                      <numeroSequencial>479967</numeroSequencial>
                      <nossonumero>09710002009871479967</nossonumero>
<linhaDigitavel>09790000070971000200898714799677114320000000070</linhaDigitavel>
<codigoBarras>09791143200000000700000009710002009871479967</codigoBarras>
                  <documento>0010</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-04-30</dataVencimento>
                  <dataLimitePagamento>2026-05-30</dataLimitePagamento>
                  <valor>0.7</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                     <tipo>1</tipo>
                      <dias>0</dias>
                  </protesto>
                  <operacoes>
                      <item>
                         <operacao>INCLUSAO</operacao>
                         <valor>0.7</valor>
                         <observacao/>
                         <sistema>WS</sistema>
                         <status>NAO_ENVIADO</status>
                         <dataHora>2026-02-12</dataHora>
                      </item>
                  </operacoes>
                  <descricao><![CDATA[<table style="width:100%"><tr><th
colspan="4">CONVÊNIOS</th><th colspan="4">DESCONTOS</th></tr><tr><td>Ref.
Mês</td><td>Descrição</td><td>Tipo/Plano</td><td>Valor</td><!-- Repita para
DESCONTOS --></tr><!-- Adicione linhas aqui para cada entrada nas colunas CONVÊNIOS
e DESCONTOS --><!-- Exemplo de linha --
><tr><td>06/2014</td><td>[Descrição]</td><td>Teste Romário</td><td>R$
1000,00</td><!-- Repita para DESCONTOS --></tr><!-- Continue adicionando linhas
conforme necessário --></table>]]></descricao>
                  <pixCopiaCola></pixCopiaCola>
               </item>
               <item>
<idWeb>512fb65a8d3e4b3bce407776bf063939c49bc7befa36d9f30e7e3a158ecafda00a2ba238ed74
d843e8ec960478a33625db61e88645ace334247f16e02eedab14</idWeb>
<grupoParcela>9e791b48fbda446ff911ac3e896f773ab7862540793600d31324d02cbfcd1bf7d43bc
362d9dde7a9c66b3f7b3d035c5606b48e77d2397d01071e35277fe4377c</grupoParcela>
                  <pagador>
                     <nome>Romário Vitorino Ferreira</nome>
                     <cpfCnpj>01111621225</cpfCnpj>
                     <identidade/>
                     <dataNascimento>1993-02-02</dataNascimento>
                     <endereco>
                        <endereco>rua a</endereco>
                        <bairro>teste</bairro>
                        <cep>76900344</cep>
                        <cidade>Ji-Paraná</cidade>
                        <uf>RO</uf>
                        <numero>10</numero>
                     </endereco>
                     <contatos/>
                  </pagador>
                  <numeroSequencial>479968</numeroSequencial>
                  <nossonumero>09710002009871479968</nossonumero>
<linhaDigitavel>09790000070971000200898714799685114320000000070</linhaDigitavel>
<codigoBarras>09791143200000000700000009710002009871479968</codigoBarras>
                  <documento>1</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-03-01</dataVencimento>
                  <dataLimitePagamento>2026-05-30</dataLimitePagamento>
                  <valor>0.2</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                     <tipo>1</tipo>
                     <dias>0</dias>
                  </protesto>
                  <operacoes>
                     <item>
                        <operacao>ALTERACAO</operacao>
                        <valor>0.2</valor>
                        <observacao/>
                         <sistema>WS</sistema>
                         <status>NAO_ENVIADO</status>
                         <dataHora>2026-02-13</dataHora>
                      </item>
                      <item>
                         <operacao>INCLUSAO</operacao>
                         <valor>0.7</valor>
                         <observacao/>
                         <sistema>WS</sistema>
                         <status>NAO_ENVIADO</status>
                         <dataHora>2026-02-12</dataHora>
                      </item>
                  </operacoes>
                  <descricao><![CDATA[<table style="width:100%"><tr><th
colspan="4">CONVÊNIOS</th><th colspan="4">DESCONTOS</th></tr><tr><td>Ref.
Mês</td><td>Descrição</td><td>Tipo/Plano</td><td>Valor</td><!-- Repita para
DESCONTOS --></tr><!-- Adicione linhas aqui para cada entrada nas colunas CONVÊNIOS
e DESCONTOS --><!-- Exemplo de linha --
><tr><td>06/2014</td><td>[Descrição]</td><td>Teste Romário</td><td>R$
1000,00</td><!-- Repita para DESCONTOS --></tr><!-- Continue adicionando linhas
conforme necessário --></table>]]></descricao>
                  <pixCopiaCola></pixCopiaCola>
               </item>
               <item>
<idWeb>83ba364a063a2113748d2c79fb1bc1c000fc0a8cf4f2fe465c4e743d7081361f34c4813ec80c
29b21d1ac7fa39db855365b4753498fe73e00c26f35bea016f11</idWeb>
<grupoParcela>9e791b48fbda446ff911ac3e896f773ab7862540793600d31324d02cbfcd1bf7d43bc
362d9dde7a9c66b3f7b3d035c5606b48e77d2397d01071e35277fe4377c</grupoParcela>
                  <pagador>
                      <nome>NOME DO COOPERADO</nome>
                      <cpfCnpj>CPF/CNPJ DO COOPERADO</cpfCnpj>
                      <identidade/>
                      <dataNascimento>1993-02-02</dataNascimento>
                      <endereco>
                         <endereco>rua a</endereco>
                         <bairro>teste</bairro>
                         <cep>76900344</cep>
                         <cidade>Ji-Paraná</cidade>
                         <uf>RO</uf>
                         <numero>10</numero>
                      </endereco>
                      <contatos/>
                  </pagador>
                  <numeroSequencial>479969</numeroSequencial>
                  <nossonumero>09710002009871479969</nossonumero>
<linhaDigitavel>09790000070971000200898714799693313850000000002</linhaDigitavel>
<codigoBarras>09793138500000000020000009710002009871479969</codigoBarras>
                  <documento>2</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-03-14</dataVencimento>
                  <dataLimitePagamento>2026-03-15</dataLimitePagamento>
                  <valor>0.02</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>1</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>2</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                     <tipo>2</tipo>
                     <dias>1</dias>
                  </protesto>
                  <operacoes>
                     <item>
                        <operacao>INCLUSAO</operacao>
                        <valor>0</valor>
                        <observacao/>
                         <sistema>WEB</sistema>
                         <status>NAO_ENVIADO</status>
                         <dataHora>2026-02-12</dataHora>
                      </item>
                  </operacoes>
                  <descricao/>
                   <pixCopiaCola/>00020101021226830014br.gov.bcb.pix2561pix-
pgtos.api.credisis.com.br/cobv/BOLETO097100020098710007285204000053039865802BR5924M
ONICA6009JI-PARANA61087690101562070503***6304996D</pixCopiaCola>
               </item>
            </titulos>
         </boleto>
      </ns1:buscarBoletosResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

#### 5.3.5. Método gerarBoleto

O método gerarBoleto é indicado para quando o beneficiário deseja fazer a emissão de um boleto, sendo permitido a consulta de apenas 1 (um) boleto por vez. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
   <soapenv:Header>
      <urn:Chave>
         <token>?</token>
         <convenio>?</convenio>
      </urn:Chave>
   </soapenv:Header>
   <soapenv:Body>
      <urn1:gerarBoleto>
         <boleto>
            <!--Optional:-->
            <avalista>
               <nome>?</nome>
               <!--Optional:-->
               <nomeFantasia>?</nomeFantasia>
               <cpfCnpj>?</cpfCnpj>
               <!--Optional:-->
               <identidade>?</identidade>
               <!--Optional:-->
               <dataNascimento>?</dataNascimento>
               <endereco>
                  <endereco>?</endereco>
                  <bairro>?</bairro>
            <!--Optional:-->
            <complemento>?</complemento>
            <cep>?</cep>
            <cidade>?</cidade>
            <uf>?</uf>
            <numero>?</numero>
         </endereco>
         <contatos>
            <!--Zero or more repetitions:-->
            <item>
                <contato>?</contato>
                <tipoContato>?</tipoContato>
            </item>
         </contatos>
      </avalista>
      <pagador>
         <nome>?</nome>
         <!--Optional:-->
         <nomeFantasia>?</nomeFantasia>
         <cpfCnpj>?</cpfCnpj>
         <!--Optional:-->
         <identidade>?</identidade>
         <!--Optional:-->
         <dataNascimento>?</dataNascimento>
         <endereco>
            <endereco>?</endereco>
            <bairro>?</bairro>
            <!--Optional:-->
            <complemento>?</complemento>
            <cep>?</cep>
            <cidade>?</cidade>
            <uf>?</uf>
            <numero>?</numero>
         </endereco>
         <contatos>
            <!--Zero or more repetitions:-->
            <item>
                <contato>?</contato>
                <tipoContato>?</tipoContato>
            </item>
         </contatos>
      </pagador>
      <documento>?</documento>
      <dataEmissao>?</dataEmissao>
      <dataVencimento>?</dataVencimento>
      <dataLimitePagamento>?</dataLimitePagamento>
      <valor>?</valor>
      <!--Optional:-->
      <grupoParcela>?</grupoParcela>
      <!--Optional:-->
      <nossonumero>?</nossonumero>
      <!--Optional:-->
      <quantidadeParcelas>?</quantidadeParcelas>
      <!--Optional:-->
      <intervaloParcela>?</intervaloParcela>
      <!--Optional:-->
      <layout>?</layout>
      <!--Optional:-->
      <formato>?</formato>
      <codigoEspecie>?</codigoEspecie>
      <protesto>
         <tipo>?</tipo>
         <dias>?</dias>
      </protesto>
      <!--Optional:-->
      <tipoEnvio>?</tipoEnvio>
      <!--Optional:-->
      <instrucao>?</instrucao>
      <!--Optional:-->
      <multa>
         <valor>?</valor>
         <carencia>
            <tipo>?</tipo>
            <dias>?</dias>
         </carencia>
         <tipo>?</tipo>
      </multa>
      <!--Optional:-->
      <juros>
         <valor>?</valor>
         <carencia>
            <tipo>?</tipo>
            <dias>?</dias>
         </carencia>
         <tipo>?</tipo>
      </juros>
      <!--Optional:-->
      <desconto1>
         <valor>?</valor>
         <!--Optional:-->
         <data>?</data>
         <tipo>?</tipo>
      </desconto1>
      <!--Optional:-->
      <desconto2>
         <valor>?</valor>
               <!--Optional:-->
               <data>?</data>
               <tipo>?</tipo>
            </desconto2>
            <!--Optional:-->
            <desconto3>
               <valor>?</valor>
               <!--Optional:-->
               <data>?</data>
               <tipo>?</tipo>
            </desconto3>
         </boleto>
      </urn1:gerarBoleto>
   </soapenv:Body>
</soapenv:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

##### 5.3.5.1. Retorno da requisição do método gerarBoleto

Ao realizar a integração pelo método de gerarBoleto, em caso de sucesso o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:gerarBoletoResponse>
         <boleto>
            <boleto> Base64 do boleto</boleto>
            <titulos>
               <item>
                  <idWeb>5738977e8bee3883338ef1f8c6c318c8</idWeb>
                  <grupoParcela>7d01071e35277fe4377c</grupoParcela>
                  <pagador>
                      <nome>Nome do Beneficiário</nome>
                      <cpfCnpj>CPF/CNPJ do Beneficiário</cpfCnpj>
                      <identidade/>
                      <dataNascimento>1993-02-02</dataNascimento>
                      <endereco>
                         <endereco>rua a</endereco>
                         <bairro>teste</bairro>
                         <cep>76900344</cep>
                         <cidade>Ji-Paraná</cidade>
                            <uf>RO</uf>
                            <numero>10</numero>
                         </endereco>
                         <contatos/>
                      </pagador>
                      <numeroSequencial>479967</numeroSequencial>
                      <nossonumero>09710002009871479967</nossonumero>
<linhaDigitavel>09790000070971000200898714799677114320000000070</linhaDigitavel>
<codigoBarras>09791143200000000700000009710002009871479967</codigoBarras>
                  <documento>0010</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-04-30</dataVencimento>
                  <dataLimitePagamento>2026-05-30</dataLimitePagamento>
                  <valor>0.7</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                     <tipo>1</tipo>
                     <dias>0</dias>
                  </protesto>
                  <operacoes>
                     <item>
                        <operacao>INCLUSAO</operacao>
                        <valor>0.7</valor>
                        <observacao/>
                        <sistema>WS</sistema>
                        <status>NAO_ENVIADO</status>
                        <dataHora>2026-02-12</dataHora>
                     </item>
                  </operacoes>
                  <descricao><![CDATA[<table style="width:100%"><tr><th
colspan="4">CONVÊNIOS</th><th colspan="4">DESCONTOS</th></tr><tr><td>Ref.
Mês</td><td>Descrição</td><td>Tipo/Plano</td><td>Valor</td><!-- Repita para
DESCONTOS --></tr><!-- Adicione linhas aqui para cada entrada nas colunas CONVÊNIOS
e DESCONTOS --><!-- Exemplo de linha --
><tr><td>06/2014</td><td>[Descrição]</td><td>Teste</td><td>R$ 1000,00</td><!--
Repita para DESCONTOS --></tr><!-- Continue adicionando linhas conforme necessário
--></table>]]></descricao>
               </item>
            </titulos>
         </boleto>
      </ns1:gerarBoletoResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

> **Observação:** Quando o convênio for emissor de boleto híbrido, no retorno do boleto

criado, na tag <boleto> será retornado o base64 do PDF do boleto já contemplando o Qr Code, caso não seja emissor de boleto híbrido, será retornado apenas o PDF do boleto com código de barras.

#### 5.3.6. Método gerarBoletos

O método gerarBoletos é indicado para quando o beneficiário deseja a emissão de mais de um boleto na mesma requisição. No XML abaixo é apresentada a estrutura necessária para esta requisição.

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:urn="urn:CredisisBoletoInterface" xmlns:urn1="urn:CredisisBoletoInterface-
CredisisWebService">
<soapenv:Header>
   <urn:Chave>
      <token>?</token>
      <convenio>?</convenio>
   </urn:Chave>
</soapenv:Header>
<soapenv:Body>
   <urn1:gerarBoletos>
      <layout>?</layout>
      <boletos>
         <!--1 to 500 repetitions:-->
         <boleto>
            <!--Optional:-->
            <avalista>
               <nome>?</nome>
               <!--Optional:-->
               <nomeFantasia>?</nomeFantasia>
               <cpfCnpj>?</cpfCnpj>
               <!--Optional:-->
               <identidade>?</identidade>
               <!--Optional:-->
               <dataNascimento>?</dataNascimento>
               <endereco>
                  <endereco>?</endereco>
                  <bairro>?</bairro>
                  <!--Optional:-->
                  <complemento>?</complemento>
                  <cep>?</cep>
                  <cidade>?</cidade>
                  <uf>?</uf>
                  <numero>?</numero>
               </endereco>
               <contatos>
                  <!--Zero or more repetitions:-->
                  <item>
                     <contato>?</contato>
                     <tipoContato>?</tipoContato>
                  </item>
               </contatos>
            </avalista>
            <pagador>
               <nome>?</nome>
               <!--Optional:-->
               <nomeFantasia>?</nomeFantasia>
               <cpfCnpj>?</cpfCnpj>
               <!--Optional:-->
               <identidade>?</identidade>
               <!--Optional:-->
              <dataNascimento>?</dataNascimento>
              <endereco>
                 <endereco>?</endereco>
                 <bairro>?</bairro>
                 <!--Optional:-->
                 <complemento>?</complemento>
                 <cep>?</cep>
                 <cidade>?</cidade>
                 <uf>?</uf>
                 <numero>?</numero>
              </endereco>
              <contatos>
                 <!--Zero or more repetitions:-->
                 <item>
                    <contato>?</contato>
                    <tipoContato>?</tipoContato>
                 </item>
              </contatos>
           </pagador>
           <documento>?</documento>
           <dataEmissao>?</dataEmissao>
           <dataVencimento>?</dataVencimento>
           <dataLimitePagamento>?</dataLimitePagamento>
           <valor>?</valor>
           <!--Optional:-->
           <grupoParcela>?</grupoParcela>
           <!--Optional:-->
           <nossonumero>?</nossonumero>
           <!--Optional:-->
           <quantidadeParcelas>?</quantidadeParcelas>
           <!--Optional:-->
           <intervaloParcela>?</intervaloParcela>
           <!--Optional:-->
           <layout>?</layout>
           <!--Optional:-->
           <formato>?</formato>
           <codigoEspecie>?</codigoEspecie>
           <protesto>
              <tipo>?</tipo>
              <dias>?</dias>
           </protesto>
           <!--Optional:-->
           <tipoEnvio>?</tipoEnvio>
           <!--Optional:-->
           <instrucao>?</instrucao>
           <!--Optional:-->
           <multa>
              <valor>?</valor>
                  <carencia>
                      <tipo>?</tipo>
                      <dias>?</dias>
                  </carencia>
                  <tipo>?</tipo>
               </multa>
               <!--Optional:-->
               <juros>
                  <valor>?</valor>
                  <carencia>
                      <tipo>?</tipo>
                      <dias>?</dias>
                  </carencia>
                  <tipo>?</tipo>
               </juros>
               <!--Optional:-->
               <desconto1>
                  <valor>?</valor>
                  <!--Optional:-->
                  <data>?</data>
                  <tipo>?</tipo>
               </desconto1>
               <!--Optional:-->
               <desconto2>
                  <valor>?</valor>
                  <!--Optional:-->
                  <data>?</data>
                  <tipo>?</tipo>
               </desconto2>
               <!--Optional:-->
               <desconto3>
                  <valor>?</valor>
                  <!--Optional:-->
                  <data>?</data>
                  <tipo>?</tipo>
               </desconto3>
            </boleto>
         </boletos>
      </urn1:gerarBoletos>
   </soapenv:Body>
</soapenv:Envelope>
```

##### 5.3.6.1. Retorno da requisição do método gerarBoletos

Ao realizar a integração pelo método de gerarBoletos, em caso de sucesso o XML de retorno que será recebido será similar ao apresentado abaixo:

```xml
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:ns1="urn:CredisisBoletoInterface-CredisisWebService">
   <SOAP-ENV:Body>
      <ns1:gerarBoletosResponse>
         <boletos>
               <boleto>BASE 64 DO BOLETO</boleto>
             <titulos>
                <item>
<idWeb>8d40a89e7390dbb8169cdd87a9562f47e1d706b1bf61f18eadbbd065624b98a0659141405eaa
15cdfe41b9559a53403b1d9c442d36c94f837969157f6382b703</idWeb>
<grupoParcela>af1dd115a77384e63d859020c2b75748f8bddd5dd14906b186c4fba7fc66f6abff78d
ba1e872195ee1d9e299d89ba8b0cf2b12c3e833ea548e0d23e36081f3c6</grupoParcela>
                  <pagador>
                     <nome>NOME DO COOPERADO</nome>
                     <cpfCnpj>CPFCNPJ DO COOPERADO</cpfCnpj>
                     <identidade/>
                     <dataNascimento>1993-02-02</dataNascimento>
                     <endereco>
                        <endereco>rua a</endereco>
                        <bairro>teste</bairro>
                        <cep>76900344</cep>
                        <cidade>Ji-Paraná</cidade>
                        <uf>RO</uf>
                        <numero>10</numero>
                     </endereco>
                     <contatos/>
                  </pagador>
                  <numeroSequencial>479970</numeroSequencial>
                  <nossonumero>09710002009871479970</nossonumero>
<linhaDigitavel>09790000070971000200898714799701114320000000070</linhaDigitavel>
<codigoBarras>09791143200000000700000009710002009871479970</codigoBarras>
                  <documento>0010</documento>
                  <dataEmissao>2026-02-12</dataEmissao>
                  <dataVencimento>2026-04-30</dataVencimento>
                  <dataLimitePagamento>2026-05-30</dataLimitePagamento>
                  <valor>0.7</valor>
                  <quantidadeParcelas>1</quantidadeParcelas>
                  <parcela>1</parcela>
                  <codigoEspecie>03</codigoEspecie>
                  <multa>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </multa>
                  <aberto>true</aberto>
                  <juros>
                     <valor>0</valor>
                     <carencia>
                        <tipo>1</tipo>
                        <dias>0</dias>
                     </carencia>
                     <tipo>3</tipo>
                  </juros>
                  <desconto1>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto1>
                  <desconto2>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto2>
                  <desconto3>
                     <valor>0</valor>
                     <tipo>3</tipo>
                  </desconto3>
                  <protesto>
                     <tipo>1</tipo>
                     <dias>0</dias>
                  </protesto>
                  <operacoes>
                     <item>
                        <operacao>INCLUSAO</operacao>
                        <valor>0.7</valor>
                        <observacao/>
                        <sistema>WS</sistema>
                        <status>NAO_ENVIADO</status>
                        <dataHora>2026-02-13</dataHora>
                     </item>
                  </operacoes>
                  <descricao/>
               </item>
            </titulos>
         </boletos>
      </ns1:gerarBoletosResponse>
   </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

> **Observação:** Os campos da estrutura XML apresentados com a indicação <!--Optional:--

> não são de envio obrigatório. Dessa forma, fica a critério do Beneficiário informá-los ou não, conforme a necessidade da integração.

## 6. FORMATOS E PADRÕES UTILIZADOS

```text
  Formato                  Observações
  Data                     Formato: AAAA-MM-DD
                           Onde:
                           DD = dia com 2 caracteres
                           MM = mês com 2 caracteres
                           AAAA = ano com 4 caracteres
  DataHora                 Formato: AAAA-MM-DD HH:mm:SS
                           HH = hora (24 horas)
                           mm = minuto
                           SS = Segundos
  Valores                  Formato: 0.00
  Decimais                 O ponto (.) deve ser utilizado para separar a parte inteira da
                           fracionária.
                           Exemplo:
                           48.56
                           1.00
                           0.50

```

Abaixo segue algumas formatações de dados que devem ser seguidas para a geração correta na estrutura dos arquivos:

- Não deve ser inserido caractere não significativo para preencher o tamanho

completo do campo, ou seja, zeros antes de número ou espaço em branco após cadeia de caracteres.

- A posição do campo é definida na estrutura do documento XML através de TAGs

(<tag>conteúdo</tag>).

- Para reduzir o tamanho final do arquivo XML dos Boletos alguns cuidados de

programação deverão ser assumidos:

- Não incluir "zeros não significativos" para campos numéricos;

- Não incluir "espaços" no início ou no final de campos numéricos e

alfanuméricos;

- Não incluir comentários no arquivo XML;

## 7. LAYOUT DE ARQUIVOS XML

Neste tópico serão apresentados os layouts dos arquivos XMLS usados para a integração via Web Service.

### 7.1. LAYOUT DE ENTIDADES

#### 7.1.1. CHAVE

```text
TAG                          DESCRIÇÃO                                                              TIPO            OP

token                        Chave de 128 bytes gerada a partir do sistema                          Data            Não
                             CrediSIS Cobrança WEB.

convenio                     Código do convênio do cooperado.                                       Data            Não

```

#### 7.1.2. PERIODO

```text
TAG                          DESCRIÇÃO                                                              TIPO            OP

inicio                       Data início do período                                                 Data            Não

fim                          Data final do período                                                  Data            Não

tipo                         Tipo do período, valores possíveis:                                    String          Sim
                                  ●    emissao
                                  ●    vencimento
                                  ●    cancelamento
                                  ●    baixa_manual

```

#### 7.1.3. ENDERECO

```text
TAG                          DESCRIÇÃO                                                       TIPO                 OP

endereco                     Nome da rua                                                     String               Não

bairro                       Nome do bairro                                                  String               Não

complemento                  Complemento de endereço                                         String               Sim

cep                          CEP                                                             String               Não

cidade                       Nome da cidade                                                  String               Não

uf                           Unidade Federativa                                              String               Não

numero                       Número                                                          String               Não

```

#### 7.1.4. PESSOA

```text
TAG                           DESCRIÇÃO                                                     TIPO                 OP

nome                          Nome                                                          String               Não

nomeFantasia                  Nome fantasia                                                 String               Sim

cpfCnpj                       CPF ou CNPJ                                                   String               Não

identidade                    Identidade                                                    String               Sim

dataNascimento                Data de nascimento                                            Data                 Sim

endereco                      Endereço                                                      Endereco             Não

contatos                      Lista de contatos                                             Contato              Não

```

#### 7.1.5. CONTATO

```text
TAG                         DESCRIÇÃO                                                       TIPO                OP

contato                     Informação do contato                                           String              Não

tipoContato                 Tipo do contato, valos possíveis:1, 2, 3, 4.                    Integer             Não
```

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

2. Telefone comercial

## 3. PADRÃO DE COMUNICAÇÃO

## 4. ESTRUTURA DE DADOS DO WEB SERVICE

#### 7.1.6. DESCONTO

```text
TAG                         DESCRIÇÃO                                                       TIPO                OP

valor                       Valor do desconto                                               String              Não

data                        Data limite desconto                                            String              Sim

tipo                        Tipo do desconto, valores possíveis: 1, 2, 3.                   Integer             Não
```

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

2. Porcentagem

## 3. PADRÃO DE COMUNICAÇÃO

#### 7.1.7. CARENCIA

```text
TAG                         DESCRIÇÃO                                                       TIPO                OP

dias                        Dias da carência                                                Integer             Não

tipo                       Tipo do desconto, valores possíveis: 1, 2.                      Integer             Não
```

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

2. Dias corridos

#### 7.1.8. MULTA

```text
TAG                        DESCRIÇÃO                                                       TIPO                OP

valor                      Valor do desconto                                               String              Não

carencia                   Carencia da multa                                               Carencia            Não

tipo                       Tipo do desconto, valores possíveis: 1, 2, 3.                   Integer             Não
```

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

2. Porcentagem

## 3. PADRÃO DE COMUNICAÇÃO

#### 7.1.9. JUROS

```text
TAG                        DESCRIÇÃO                                                       TIPO                OP

valor                      Valor do desconto                                               String              Não

carencia                   Carência do juros                                               Carencia            Não

tipo                       Tipo do desconto, valores possíveis: 1, 2, 3.                   Integer             Não
```

## 1. ARQUITETURA DE COMUNICAÇÃO COM O COOPERADO

2. Porcentagem

## 3. PADRÃO DE COMUNICAÇÃO

#### 7.1.10. OPERACAO

```text
TAG                        DESCRIÇÃO                                                       TIPO                OP

operacao                   Tipo da operação, valores possíveis:                            String              Não
                                ●    INCLUSAO
                                ●    BAIXA
                                ●    BAIXA_MANUAL
                                ●    CANCELAMENTO
                                ●    BAIXA_PIX
                                ●    ALTERACAO
                                ●    ANTECIPACAO
                                ●    APROVACAO_ANTECIPACAO
                                ●    CANCELAMENTO_ANTECIPACAO
valor                      Valor da operação                                               String              Não

observacao                  Observação da operação                                          String              Sim

sistema                     Tipo de sistema de origem                                       String              Não

status                      Status da operação junto a CIP                                  String              Não

dataHora                    Data e hora de quando ocorreu a transação                       DataHora            Não

```

#### 7.1.11. ERRO

```text
TAG                         DESCRIÇÃO                                                       TIPO                OP

code                        Código de erro                                                  String              Não

message                     Mensagem do erro                                                String              Não

```

#### 7.1.12. TITULOREQUEST

```text
TAG                                    DESCRIÇÃO                                              TIPO              OP

avalista                               Informações do avalista                                Pessoa            Sim

pagador                                Informações do pagador                                 Pessoa            Não

documento                              Documento (uso do cooperado)                           String            Não

dataEmissao                            Data de emissão do título                              Data              Não

dataVencimento                         Data de vencimento do título                           Data              Não

dataLimitePagamento                    Data limite para pagamento                             Data              Não

valor                                  Valor do título                                        Float             Não

quantidadeParcelas                     Quantidade de parcelas (para cálculo de Integer                          Não
                                       parcelamento)

intervaloParcela                       Utilizado no cálculo de parcelamento Integer                             Não
                                       (para recalcular as seguintes datas:
                                       emissão, vencimento, limite pagamento,
                                       descontos)

codigoEspecie                          Tipo do título, valores possíveis:                     String            Não
                                       03 - DMI > DUPLICATA
                                       MERCANTIL P/ INDICAÇÃO
                                       05 - DSI > DUPLICATA DE SERVIÇO
                                       P/ INDICAÇÃO
                                       12 - NP > NOTA PROMISSÓRIA
                                       17 - RC > RECIBO
                                       20 - AP > APÓLICE DE SEGURO

                                       21 - ME > MENSALIDADE ESCOLAR
                                       23 - NF > NOTA FISCAL
protesto                               Informação de negativação do título                    Carencia          Não

tipoEnvio                              Tipo envio para instituição e negativação String                         Sim
                                       (Cartorio). Valores possíveis:
                                               CARTORIO
                                               NENHUM
instrucao                              Instrução para impressão do boleto                     String            Sim

multa                                  Informação de multa                                    Multa             Sim

juros                                  Informação de juros                                    Juros             Sim

desconto1                              Informação de 1º desconto                              Desconto          Sim

desconto2                              Informação do 2º desconto                              Desconto          Sim

desconto3                              Informação do 3º desconto                              Desconto          Sim

```

#### 7.1.13. TITULORESPONSE

```text
TAG                                    DESCRIÇÃO                                            TIPO                        OP

idWeb                                  Identificação do título                              String                      Não

grupoParcela                           Identificação do grupo parcela                       String                      Não

avalista                               Informações do avalista                              Pessoa                      Sim

pagador                                Informações do pagador                               Pessoa                      Não

numeroSequencial                       Número sequencial do título                          Integer                     Não

nossonumero                            Nosso número                                         String                      Não

linhaDigitavel                         Linha digitável do título                            String                      Não

codigoBarras                           Número do código de barras do boleto                 String                      Não

documento                              Documento (uso do cooperado)                         String                      Não

dataEmissao                            Data de emissão do título                            Data                        Não

dataVencimento                         Data de vencimento do título                         Data                        Não

dataLimitePagamento                    Data limite para pagamento                           Data                        Não

valor                                  Valor do título                                      Float                       Não

quantidadeParcelas                     Quantidade de parcelas (para cálculo de Integer                                  Não

                                       parcelamento)

parcela                                Número da parcela                                    Integer                     Não

codigoEspecie                          Tipo do título, valores possíveis:                   String                      Não
                                       03 - DMI > DUPLICATA
                                       MERCANTIL P/ INDICAÇÃO
                                       05 - DSI > DUPLICATA DE
                                       SERVIÇO P/ INDICAÇÃO
                                       12 - NP > NOTA PROMISSÓRIA
                                       17 - RC > RECIBO
                                       20 - AP > APÓLICE DE SEGURO
                                       21 - ME > MENSALIDADE
                                       ESCOLAR
                                       23 - NF > NOTA FISCAL
protesto                               Informação de negativação do título                  Carencia                    Não

instrucao                              Instrução para impressão do boleto                   String                      Sim

aberto                                 Indica se o título está em aberto para Boolean                                   Sim
                                       efetuação de baixa

multa                                  Informação de multa                                  Multa                       Não

juros                                  Informação de juros                                  Juros                       Não

desconto1                              Informação de 1º desconto                            Desconto                    Não

desconto2                              Informação do 2º desconto                            Desconto                    Não

desconto3                              Informação do 3º desconto                            Desconto                    Não

operacoes                              Lista de operações realizadas no título              Operacao[]                  Não

pixCopiaCola                           Chave Pix Cópia e Cola                               String                      Sim

```

#### 7.1.14. BOLETOREQUEST

```text
TAG                         DESCRIÇÃO                                                       TIPO                OP

idWeb                       Identificação do boleto                                         String              Sim

grupoParcela                Identificação do grupo parcela                                  String              Sim

layout                      default / 3vias / carne                                         String              Sim

formato                     pdf / html                                                      String              Sim

```

#### 7.1.15. BOLETORESPONSE

```text
TAG                         DESCRIÇÃO                                               TIPO                            OP

boleto                      Base64 do boleto impresso.                              String                          Sim

erros                       Caso houver erros no processamento da Erro[]                                            Sim
                            requisição, esse campo será preenchido.

titulos                     Títulos encontrados                                     TituloResponse[]                Sim

            7.1.16. ALTERARTITULOREQUEST

```

```text
TAG                         DESCRIÇÃO                                               TIPO                            OP

idWeb                       Identificador do título                                 String                          Não

valor                       Valor do título                                         Float                           Não

protesto                    Carência para negativação do título                     Carencia                        Não

dataVencimento              Data do vencimento do título                            Data                            Não

instrucao                   Instrução para impressão do boleto                      String                          Sim

multa                       Informação de multa                                     Multa                           Sim

juros                       Informação de juros                                     Juros                           Sim

desconto1                   Informação de 1º desconto                               Desconto                        Sim

desconto2                   Informação do 2º desconto                               Desconto                        Sim

desconto3                   Informação do 3º desconto                               Desconto                        Sim

documento                   Documento (uso do cooperado)                            String                          Não

dataLimitePagamento Data limite para pagamento                                      Data                            Não

            7.1.17. ALTERARTITULORESPONSE

```

```text
TAG                         DESCRIÇÃO                                               TIPO                            OP

idWeb                       Identificador do título                                 String                          Não

operacao                    Operação gerada da operação                             Operacao                        Sim

erros                      Caso houver um erro no processamento Erro                                               Sim
                           da baixa, esse campo deverá vir
                           preenchido com os erros.

```

#### 7.1.18. BOLETOSREQUEST

```text
TAG                        DESCRIÇÃO                                                    TIPO                      OP

periodo                    Período para filtro                                          Periodo                   Não

operacoes                  Lista de operações para filtro, valores String[]                                       Sim
                           possíveis:
                             ●       INCLUSAO
                             ●       BAIXA
                             ●       BAIXA_MANUAL
                             ●       CANCELAMENTO
                             ●       ALTERACAO
                             ●       DESCONTADO

```

#### 7.1.19. BAIXARBOLETOREQUEST

```text
TAG                        DESCRIÇÃO                                                       TIPO                   OP

idWeb                      Identificação do título                                         String                 Não

valor                      Valor da operação                                               Float                  Não

operacao                   Operação de baixa, valores possíveis:                           String                 Não
                                 ● BAIXA_MANUAL
                                 ● CANCELAMENTO

```

#### 7.1.20. BAIXARBOLETORESPONSE

```text
TAG                        DESCRIÇÃO                                                        TIPO                  OP

operacao                   Operação gerada da baixa                                         Operacao              Sim

erros                      Caso houver um erro no processamento da Erro[]                                         Sim
                           baixa, esse campo deverá vir preenchido com os
                           erros.

```

## 8. INTERFACE DE COMUNICAÇÃO

Método: buscarBoleto

Descrição Envio Resposta

Busca título por sua identificação ou BoletoRequest BoletoResponse identificação do grupo de parcela a qual ele pertence. Retorna informação do (s) título (s), junto com sua impressão em Base64.

Método: buscarBoletos

Descrição Envio Resposta

Busca título por um determinado período. BoletosRequest BoletoResponse Retorna uma lista de títulos.

Método: baixarBoleto

Descrição Envio Resposta

Baixa um título, informando sua BaixarBoletoRequest BaixarBoletoResponse identificação, o valor da operação e o tipo da operação. Retorna a operação registrada.

Método: gerarBoleto

Descrição Envio Resposta

Método responsável por registrar um título. TituloRequest BoletoResponse Retorna o título registrado, junto com o Base64 de sua impressão.

Método: alterarBoleto Obs: Método disponível apenas no seguinte endereço: https://credisiscobranca.com.br/v2/ws?wsdl

Descrição Envio Resposta Método responsável realizar alterações no título. AlteracaoTituloRequest AlteracaoTituloResponse Retorna a operação de alteração.
