# Manual da API da Cobrança

> Documento convertido do PDF **Manual API da Cobrança**, versão **3.9.1**.
| Campo | Informação |
|---|---|
| Título | Manual API da Cobrança |
| Versão identificada | 3.9.1 (o corpo do documento registra 3.9) |
| Data registrada | 11/11/2025 |
| Autor registrado | Alessandra Linhares Paniz |
| Classificação | Uso Irrestrito |

## Sumário

- [1. Apresentação](#1-apresentação)
- [2. Para quem?](#2-para-quem)
- [3. Objetivo](#3-objetivo)
- [4. Orientações Iniciais](#4-orientações-iniciais)
- [5. Quais as funcionalidades?](#5-quais-as-funcionalidades)
- [6. Iniciando a integração](#6-iniciando-a-integração)
- [7. Recursos disponíveis](#7-recursos-disponíveis)
  - [7.1. Autenticação](#71-autenticação)
  - [7.2. Cadastro de Boletos](#72-cadastro-de-boletos)
  - [7.3. Impressão de Boletos](#73-impressão-de-boletos)
  - [7.4. Comando de Instrução - Pedido de Baixa](#74-comando-de-instrução-pedido-de-baixa)
  - [7.5. Comando de Instrução - Alteração de Vencimento](#75-comando-de-instrução-alteração-de-vencimento)
  - [7.6. Comando de Instrução - Alteração de Desconto](#76-comando-de-instrução-alteração-de-desconto)
  - [7.7. Comando de Instrução - Alteração de Data de Desconto](#77-comando-de-instrução-alteração-de-data-de-desconto)
  - [7.8. Comando de Instrução - Alteração de Juros](#78-comando-de-instrução-alteração-de-juros)
  - [7.9. Comando de Instrução - Alteração de Seu Número](#79-comando-de-instrução-alteração-de-seu-número)
  - [7.10. Comando de Instrução – Conceder Abatimento](#710-comando-de-instrução-conceder-abatimento)
  - [7.11. Comando de Instrução – Cancelar Abatimento Concedido](#711-comando-de-instrução-cancelar-abatimento-concedido)
  - [7.12. Comando de Instrução - Pedido de Protesto](#712-comando-de-instrução-pedido-de-protesto)
  - [7.13. Comando de Instrução – Sustar Protesto e Baixar Título](#713-comando-de-instrução-sustar-protesto-e-baixar-título)
  - [7.14. Comando de Instrução – Sustar Protesto e Manter Título](#714-comando-de-instrução-sustar-protesto-e-manter-título)
  - [7.15. Comando de Instrução – Incluir Negativação](#715-comando-de-instrução-incluir-negativação)
  - [7.16. Comando de Instrução – Excluir de Negativação e Baixar Título](#716-comando-de-instrução-excluir-de-negativação-e-baixar-título)
  - [7.17. Comando de Instrução – Cancelar Protesto Automático](#717-comando-de-instrução-cancelar-protesto-automático)
  - [7.18. Consulta de Boletos por Nosso Número](#718-consulta-de-boletos-por-nosso-número)
  - [7.19. Consulta de Boletos Liquidados por Dia](#719-consulta-de-boletos-liquidados-por-dia)
  - [7.20. Consulta título cadastrado por “idEmpresa” ou “seuNumero”](#720-consulta-título-cadastrado-por-idempresa-ou-seunumero)
  - [7.21. Consulta movimentações financeiras (Francesinha)](#721-consulta-movimentações-financeiras-francesinha)
  - [7.22. Consulta de Boletos com Distribuição de Crédito](#722-consulta-de-boletos-com-distribuição-de-crédito)
  - [7.23. Cancelar Parcelas de Distribuição de Crédito de um Boleto](#723-cancelar-parcelas-de-distribuição-de-crédito-de-um-boleto)
  - [7.24. Liberar Repasse de Distribuição de Crédito de um Boleto](#724-liberar-repasse-de-distribuição-de-crédito-de-um-boleto)
- [8. Geração Nosso Número](#8-geração-nosso-número)
- [9. Layout do Boleto](#9-layout-do-boleto)
- [10. Glossário](#10-glossário)
- [11. Fluxos](#11-fluxos)
  - [11.1. Diagrama de Sequência de chamadas realizadas para o serviço](#111-diagrama-de-sequência-de-chamadas-realizadas-para-o-serviço)
- [12. Apresentação API Recebimento de eventos](#12-apresentação-api-recebimento-de-eventos)
- [13. Objetivo](#13-objetivo)
- [14. Orientações Iniciais](#14-orientações-iniciais)
- [15. Quais as funcionalidades?](#15-quais-as-funcionalidades)
- [16. Parâmetros API de Recebimentos de eventos](#16-parâmetros-api-de-recebimentos-de-eventos)
- [17. Apresentação API de Contratação Webhook](#17-apresentação-api-de-contratação-webhook)
- [18. Para quem?](#18-para-quem)
- [19. Objetivo](#19-objetivo)
- [20. Orientações Iniciais](#20-orientações-iniciais)
- [21. Quais as funcionalidades?](#21-quais-as-funcionalidades)
- [22. Iniciando a integração](#22-iniciando-a-integração)
- [23. Recursos disponíveis](#23-recursos-disponíveis)
  - [23.1. Autenticação](#231-autenticação)
  - [23.2. Realizar Contratação Webhook](#232-realizar-contratação-webhook)
  - [23.3. Consulta Contratos](#233-consulta-contratos)
  - [23.4. Altera Contrato Webhook](#234-altera-contrato-webhook)
- [24. Glossário](#24-glossário)

---

1. Apresentação A API de Cobrança do Sicredi realiza o cadastro de boletos, além de geração de PDF, consultas e Comando de instrução para gestão da sua carteira de boletos, através de integração com seu sistema de gestão, seu site ou seu app. O uso da API de Cobrança traz benefícios como: boletos imediatamente disponíveis para liquidação do pagador, situação da carteira de boletos em tempo real. A API de Cobrança está disponível 24 horas, 7 dias por semana.

2. Para quem? São variados os tipos de negócios que podem empregar este tipo de recebimento. Negócios representados por uma Pessoa Jurídica, seja do ramo da indústria, comércio, prestação de serviço, agronegócio, Governo*, não importando esferas municipais, estaduais ou federal, da administração direta ou indireta, ou Pessoa Física, profissional liberal, produtor rural ou autônomo, que precisem receber recursos oriundos de uma oferta, proposta, venda ou serviço prestado.

3. Objetivo Este manual destina-se às pessoas responsáveis pelo desenvolvimento e adaptação dos sistemas das empresas associadas ao Sicredi e que desejam trabalhar coma Cobrança através da integração via API.

4. Orientações Iniciais Para iniciar o processo de integração da API da Cobrança, o associado deve contratar o produto Cobrança com a pessoa Gerente de conta e optar pela modalidade API (Cobrança Online).

Ao contratar o produto é gerado o código do beneficiário (código do convênio) e o seu uso será obrigatório na integração e na troca de informações entre a empresa ou pessoa associada e o Sicredi.

Logo, para promover a integração sistêmica de registro on-line de Boletos a empresa ou pessoa associada inicialmente precisa:

Ter contratado o produto Cobrança através da sua cooperativa; Ter optado pela modalidade API (Cobrança Online); Ter o termo de contratação/adesão assinado;

5. Quais as funcionalidades? A API da Cobrança contém as seguintes funcionalidades:

Autenticação: Operação responsável pela autenticação do código de acesso e criação do token de autenticação, que permitirá acessar a API de Cobrança.

Cadastro de Boletos: Operação responsável pelo registro do boleto. Com a opção de dois tipos de cobrança:

o Boleto tradicional: Operação responsável pelo cadastro do pagador, do boleto, do informativo e das mensagens relacionadas ao boleto a ser criado. Essa opção trata- se de um boleto com a criação de linha digitável e código de barras.

o Boleto Híbrido: Operação responsável pelo cadastro do pagador, e de todos os dados constantes no boleto tradicional além do QR Code dinâmico vinculado a esse boleto. Dessa forma o pagador pode escolher por qual meio realizar o pagamento, se pela leitura do código de barras ou pelo QR Code. Para cadastrar boletos híbridos é necessário ter optado por essa modalidade na contratação do produto junto à cooperativa/agência. Caso a empresa associada não tenha optado, ela pode a qualquer momento solicitar a habilitação à pessoa gerente de conta.

> **Importante:** Quando o usuário informar a data de carência para juros e a data de

carência para multa, e o beneficiário estiver cadastrado na modalidade híbrida, o boleto será registrado como tradicional, sem a inclusão do QR Code. Essa medida se deve ao fato de que, atualmente, o QR Code não contempla campos específicos para indicar as datas de carência de juros e multa.

o Boleto com Distribuição de Crédito (Rateio): É uma opção, tanto para boleto tradicional, quanto para híbrido, onde o associado poderá informar para quais contas deseja realizar o repasse do crédito do boleto (também chamado no mercado como Split por Pagamento). Nessa opção será possível escolher entre repasse para contas de outras IFs - via TED (Transferência Eletrônica Disponível) e/ou repasse para contas Sicredi de qualquer agência.

Impressão de Boletos: Operação responsável pela impressão e reimpressão (2ª via) dos dados de boletos já emitidos, tradicionais ou híbridos.

Comando de Instrução – Pedido de Baixa: Operação responsável por solicitar o pedido de baixa do boleto.

Comando de Instrução – Alteração de Vencimento: Operação responsável por solicitar a alteração da data de vencimento do boleto.

Comando de Instrução – Alteração de Desconto: Operação responsável por solicitar a alteração do valor de desconto do boleto.

Comando de Instrução – Alteração de Data de Desconto: Operação responsável por solicitar a alteração da data de desconto do boleto.

Comando de Instrução – Alteração de Juros: Operação responsável por solicitar a alteração do valor/percentual de juros do boleto.

Comando de Instrução – Alteração de Seu Número: Operação responsável por solicitar a alteração do seu número do boleto.

Comando de Instrução – Conceder Abatimento: Operação responsável por conceder abatimento a um título.

Comando de Instrução – Cancelar Concessão de Abatimento: Operação responsável por cancelar concessão de abatimento a um título.

Comando de Instrução – Pedido de Protesto: Operação responsável por solicitar pedido de protesto de um título vencido.

Comando de Instrução – Sustar Protesto e Manter Título em Carteira: Operação responsável por sustar o protesto de um título e manter o título em carteira.

Comando de Instrução – Sustar Protesto e Baixar Título: Operação responsável por sustar o protesto de um título e baixar o título.

Comando de Instrução – Pedido de Negativação: Operação responsável por solicitar a negativação de um boleto vencido.

Comando de Instrução – Excluir Negativação e Baixar título: Operação responsável por cancelar um pedido de negativação e baixar título.

Comando de Instrução – Cancelar Protesto Automático: Operação responsável por cancelar o protesto automático de um título que possui essa informação.

Consulta de Boletos por Nosso Número: Operação responsável pela consulta de um determinado boleto, através do Nosso Número.

Consulta de Boletos Liquidados por Dia: Operação responsável pela consulta de boletos liquidados, de uma data específica.

Consulta Movimentações Financeiras (Francesinha): Operação responsável pela consulta de movimentações financeiras (Crédito, Débito e Ambas) de uma data específica.

Consulta de Boletos com Distribuição de Crédito: Operação responsável pela consulta de um determinado boleto, através do Nosso Número, que possua dados de Distribuição de Crédito.

Cancelar Parcelas de um Boleto com Distribuição de Crédito: Operação responsável por realizar o cancelamento de parcelas de um determinado boleto (ou seja, quantidade de rateios cadastrados para o boleto) que possuam Distribuição de Crédito.

Liberar Repasse de um Boleto com Distribuição de Crédito: Operação responsável por realizar a liberação do repasse de créditos de um boleto que possua Distribuição de Crédito. Essa ação somente é necessária quando o boleto possuir a flag existente cadastro do boleto como “Repasse Automático Split” preenchido com “Não”.

Webhook: A API de Recebimento de eventos Webhook é uma API que deve ser desenvolvida pelo associado com o objetivo de se comunicar e realizar uma integração com a API Webhook do Sicredi, a fim de receber os eventos sobre as movimentações que ocorrem em seus títulos de cobrança.

6. Iniciando a integração Para iniciar a integração há um processo constituído de etapas sequenciais para Validação e Produção:

1. Validação/Teste

Acessar o Portal do Desenvolvedor: a. Criação de APP para Homologação b. Solicitação do Access Token de Homologação (Sandbox) c. Testes nas URLS

a. Criação de APP para Homologação: para criar a APP de homologação, o desenvolvedor deve acessar o Portal do Desenvolvedor Sicredi (clique aqui) e criar uma conta ou logar. Após criar uma conta ou logar, o desenvolvedor deve clicar em ‘criar uma nova aplicação’. Ao criar uma nova aplicação deve nomeá-la com o prefixo ‘API Cobrança Coop Código Beneficiário SandBox’ para identificar que é a APP de homologação e selecionar a API da Cobrança [ API

COBRANÇA BOLETO 1.0.0 ] e a API de Autenticação Parceiros na lista de APIs disponíveis [OPEN API – OAUTH - PARCEIROS 1.0.0]. Ao criar uma app você já terá um Client ID gerado automaticamente para essa APP.

1.Criando a APP no portal 2. Selecionando as APIs na lista

b. Geração do Access Token de Homologação (Sandbox): após criar a APP de Homologação, você deve, ainda no portal do desenvolvedor, clicar no menu suporte > abrir chamado. Selecione a opção “COLOCAR AQUI QUAL OPÇÃO”, e preencha o nome da APP com aquele nome que você criou no passo anterior. Pronto, o seu Acess token será gerado e dentro de X dias você poderá visualizar o seu token de homologação no menu ‘minhas apps’, selecionando a app criada e clicando em ‘ver detalhes’.

3. Visualizando o access token

c. Testes nas URLS: No decorrer desse manual você encontrará as URLs para teste dos recursos disponíveis em ambiente de homologação.

2. Produção

Acessar o Portal do Desenvolvedor a. Criação de APP para Produção b. Solicitação do Access Token de Produção

Acessar o Internet Banking c. Geração da Código de acesso (password) no internet banking;

a. Criação de APP para produção: Para criar a APP de produção, você pode repetir os mesmos passos da criação da APP de homologação, no entanto, lembre-se de nomear essa nova APP de uma forma que você identifique que ela se trada da APP de produção, nossa sugestão é: ‘API Cobrança <Coop> <Código Beneficiário > Produção’. Após criar a APP você terá acesso imediato ao Client ID de produção.

b. Geração do Access Token de Produção: Repita os mesmos passos já realizados para a geração de token de homologação, mas dessa vez ao abrir o chamado, preencha o campo Client ID ou Nome da APP com os dados da APP de produção. O seu token será disponibilizado no mesmo prazo anteriormente citado e poderá ser visualizado no menu minhas apps.

4. Visualizando as APPs no menu

É importante lembrar que em todas as operações será necessário informar no cabeçalho da requisição no campo x-api-key o token de autenticação recebido. Esse token é diferente para o ambiente de Homologação (Sandbox) e Produção, deve ser informado o token correspondente ao ambiente que está sendo utilizado.

c. Geração do Código de Acesso (password): a empresa ou pessoa associada deve acessar o Internet Banking, pela conta vinculada ao seu convênio de Cobrança, e selecionar a opção de menu no: Cobrança >> Código de Acesso >> Gerar - Esse menu é apresentado exclusivamente para os beneficiários que possuem a modalidade API(Cobrança Online) habilitada no convênio.

5. Geração de Código de Acesso (password)

Se você ou a sua empresa associada não possui a modalidade habilitada e deseja habilitá-la, converse com a pessoa gerente de conta.

A chave será gerada após informado o dispositivo de segurança. Ela deve ser armazenada e utilizada na configuração da integração da API, pois esse código gerado é utilizado no recurso de autenticação para permitir as operações. Caso o beneficiário possua mais de uma conta com o produto Cobrança contratado e deseje utilizá-las, precisará gerar o token de autenticação para cada conta específica.

## 7. Recursos disponíveis

### 7.1. Autenticação

Em todas as operações será necessário informar no cabeçalho da requisição no campo x-api-key o token de autenticação recebida do Sicredi. Esse token é diferente para o ambiente de Sandbox e Produção, deve ser informado o token correspondente ao ambiente que está sendo utilizado. A operação POST “token” é responsável por criar uma chave criptografada, denominada “access_token”, utilizando o Beneficiário + Cooperativa como usuário (username) e o Código de Acesso

(password) que foi gerada no Internet Banking. O padrão de autenticação é o OAuth2 e o token utiliza o padrão JWT. O formato de entrada é “x-www-form-urlencoded” e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8.

> **Observação:** deve ser respeitada a orientação da segurança da informação referente a expiração

do access_token e do refresh_token, observando os parâmetros “expires_in” (exemplo: 300 segundos) e “refresh_expires_in” (exemplo: 900 segundos) de retorno do serviço respectivamente. Os parâmetros de expiração (“expires_in” e “refresh_expires_in”) podem ser diferentes em Teste e Produção. Para verificar se o acess_token está expirado, deve-se observar o campo “expires_in” que retorna a duração do token em segundos. O access_token deve ser enviado no cabeçalho das chamadas das próximas operações até que ele expire, não deve ser realizado autenticação a cada chamada. Quando o access_token expirar, deve ser utilizado o refresh_token para gerar um novo token.

> **OBS:** Quando da utilização do refresh_token, não será necessário informar os campos

“username” e “password”. Quando o refresh_token estiver expirado, deverá ser realizado o fluxo de autenticação normal, com usuário e senha (username e password).

### ENTRADA

Tipo de

### SAÍDA

### RETORNO (Response)

### FALHAS

### REQUEST – POST

### URL Sandbox (ambiente para homologação)

```text
Ord         Nome         Tipo                       Tamanho         Formatação          Descrição          Obrigatório
                                  Parâmetro

1        x-api-key String       HeaderParam        36              UUID              Access token        Obrigatório
                                                                   38 bits           gerado no
                                                                                     portal do
                                                                                     desenvolvedor

2        context      String    HeaderParam 8                    COBRANCA            Contexto,           Obrigatório
                                                                                     passar fixo
                                                                                     “COBRANCA”

3       Content-            HeaderParam                      application/x-      Indica o tipo    Obrigatório
        Type                                                 www-form-           de arquivo do
                                                             urlencoded          recurso

4       grant_typ String    Body             Até 13          password ou         Tipo de          Obrigatório
        e                                                    refresh_token       autenticação.
                                                                                 Ver diagrama,
                                                                                 pág 113

5       username String     Body             9               Sem                 Código do        Obrigatório para
                                                             formatação          beneficiário +   Grant Type
                                                                                 Código da        igual a
                                                                                 Cooperativa      password

6       password String     Body             64              Sem                 Código de        Obrigatório para
                                                             formatação          Acesso gerado    Grant Type igual
                                                                                 no Internet      password
                                                                                 Banking

7       refresh_t String    Body             753             Sem                 Refresh token    Obrigatório para
        oken                                                 formatação          obtido           Grant Type igual
                                                             dígitos             anteriormente    a refresh_token
                                                                                 na
                                                                                 autenticação

8       scope      String   Body             8               cobranca            Escopo de        Obrigatório para
                                                                                 acesso. Passar   Grant Type
                                                                                 fixo             password
                                                                                 “cobranca”.

                                             Tipo de
    Ord            Nome            Tipo                        Formatação                    Descrição
                                            Parâmetro
                                                                                  Token de autenticação para ser
    1       access_token       String            JSON       576 bits
                                                                                  utilizado

                                                          2017-08-
   2        token_type         String        JSON         09T08:53:11.679- Tipo do Token
                                                          03:00
                                                                           Será utilizado para gerar um
                                                                           novo access_token, sem a
   3        refresh_token      String        JSON         636 dígitos      necessidade de uma nova
                                                                           autenticação com usuário e
                                                                           senha
                                                                           Tempo de expiração (em
   4        expires_in         String        JSON         3 dígitos
                                                                           segundos) do Access Token
                                                                           Tempo de expiração (em
   5        refresh_expires_in String        JSON         6 dígitos
                                                                           segundos) do Refresh Token
                                                          cobranca profile Escopo que o usuário possui
   6        scope              String        JSON
                                                          email            acesso.

             Código                                             Descrição
         HTTP_OK (200)                  Operação realizada com sucesso

                                                         Parâmetros de
             Status                Mensagem                 entrada                     Descrição
                                                        sujeitos a crítica

                                 Ocorreu um erro
                                                                              Erro quando o endereço
                                 ao realizar
HTTP_NOT_FOUND (404)                                   Não possui             (URL) inválido. Deve-se
                                 operação de
                                                                              verificar a url utilizada.
                                 autenticação.

                                                                              Erros ocasionados por
                                                       username,
                                 Invalid user                                 username ou password não
HTTP_UNAUTHORIZED (401)                                password,
                                 credentials                                  informado ou
                                                       context
                                                                              inválidos.

                                Missing
HTTP_UNAUTHORIZED                                                           Erro ocasionado por
                                parameter:       username
(401)                                                                       username não informado.
                                username
                                Could not find a
                                required Access
                                                                            Erro ocasionado quando o
HTTP_UNAUTHORIZED               Token in the
                                                 x-api-key                  parâmetro x-api-key não foi
(401)                           request,
                                                                            informado ou está inválido.
                                identified by
                                HEADER x-api-key
                                                                            Erro ocasionado quando o
                                Missing form
                                                                            parâmetro grant_type não é
HTTP_BAD_REQUEST (400)          parameter:           grant_type
                                                                            informado ou
                                grant_type
                                                                            inválido.
                                                                            Erro ocasionado quando o
HTTP_GATEWAY_TIMEOUT                                                        serviço não conseguiu
                                Gateway Timeout Não se aplica
(504)                                                                       responder no tempo
                                                                            esperado.

```

`https://api-parceiro.sicredi.com.br/sb/auth/openapi/token`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “username” e

“password”: username : 123456789 password: teste123

### URL Produção

`https://api-parceiro.sicredi.com.br/auth/openapi/token`

### RESPONSE

```json
{
"access_token":
"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxMlRFLXliYWxnNjIxSmRCMU9IX0w2Sms5QllxODJtNkFS
Z0NGaDZtQnZBIn0.eyJleHAiOjE2NTY0NTA5MzIsImlhdCI6MTY1NjQ1MDYzMiwianRpIjoiZjNiYjg5YzQtOGRmM
y00OTMxLWExOTItMjUxMjUxZWE1YzNkIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLW9wZW5hcGkucHJkLnNpY3JlZGk
uY2xvdWQvcmVhbG1zL29wZW5hcGkiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiZjozMWY3ODE1MS1jMzdmLTQzO
WYtYTIwOS02NjFjYTQ3MzkzYTE6Q09CUkFOQ0E6MDA0MTQwMTE2IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoib3Bl
bmFwaS1ndy1zZW5zZWRpYSIsInNlc3Npb25fc3RhdGUiOiIwNGQ3YTAwYS05MjhiLTQ3NzQtYTY1MC1mOWQx
ODkzNWE3NDMiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtb3BlbmFwa
SIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW
50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbG
UiXX19LCJzY29wZSI6ImNvYnJhbmNhIHByb2ZpbGUgZW1haWwiLCJzaWQiOiIwNGQ3YTAwYS05MjhiLTQ3NzQ
tYTY1MC1mOWQxODkzNWE3NDMiLCJvcGVuYXBpX3VzZXJuYW1lIjoiMDA0MTQwMTE2Iiwib3BlbmFwaV9jb2
50ZXh0IjoiQ09CUkFOQ0EiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsInByZWZlcnJlZF91c2VybmFtZSI6IjAwNDE0M
DExNiJ9.mVzfyzBhVFwRWVnQkbiD00zitoOfmatRcDN_e_OKo4XKcOY6RU-qlpJ0FOC-
_7Qy5ez7XveNeRyq47ToY6F0skbw-0BswxY3qdQHBr50G-
AxqICYLSUTEamrPc0vLNwADdPracL9P2W_EVEFLae7QeVzc0toNLjvaLJCaGZH6-
wV99qMjf0VUj5nRCUm6ZCllb3NioyI6mXsj0Ea1s8RdgnanWvgvD-
cNjmCmMXVhy7U_MScLIb_7cIrn4TbcbkA3r11b96KzU9cp5BCXHZysDdX0wRFVzQUQfBsf33E40GSNJ-
juFZdV86vWCmx7BjKnC1s3embfX0p-9JnbNUy7Q",
"expires_in": 300,
"refresh_expires_in": 1800,
"refresh_token":
"eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJhMTRlOTdmOC1lZmJiLTQ2YTYtOTYwYy0wNTFlZjA4MT
U3MjEifQ.eyJleHAiOjE2NTY0NTI0MzIsImlhdCI6MTY1NjQ1MDYzMiwianRpIjoiOWIyOWE3YjMtYzRhNS00NGQz
LTg3MjQtZWU4ZGQzYjIwNDAxIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLW9wZW5hcGkucHJkLnNpY3JlZGkuY2xvdW
QvcmVhbG1zL29wZW5hcGkiLCJhdWQiOiJodHRwczovL2F1dGgtb3BlbmFwaS5wcmQuc2ljcmVkaS5jbG91ZC9y
ZWFsbXMvb3BlbmFwaSIsInN1YiI6ImY6MzFmNzgxNTEtYzM3Zi00MzlmLWEyMDktNjYxY2E0NzM5M2ExOkNP
QlJBTkNBOjAwNDE0MDExNiIsInR5cCI6IlJlZnJlc2giLCJhenAiOiJvcGVuYXBpLWd3LXNlbnNlZGlhIiwic2Vzc2lvbl9z
dGF0ZSI6IjA0ZDdhMDBhLTkyOGItNDc3NC1hNjUwLWY5ZDE4OTM1YTc0MyIsInNjb3BlIjoiY29icmFuY2EgcHJvZ
mlsZSBlbWFpbCIsInNpZCI6IjA0ZDdhMDBhLTkyOGItNDc3NC1hNjUwLWY5ZDE4OTM1YTc0MyJ9.Om0gACw3fK
yfSnWyNhlElQSMYYtClCHrZYdGAIc4ul0",
"token_type": "Bearer",
"not-before-policy": 0,
"session_state": "04d7a00a-928b-4774-a650-f9d18935a743",
"scope": "cobranca profile email"
}
```

### Execução (via ferramenta Postman)

Cenário Positivo – Gerando o access_token

> **OBS:** Sempre verificar o tempo de expiração do access_token, através do campo “expires_in” (Exemplo: 300

segundos). Caso esteja expirado, deverá usar o refresh_token para gerar um novo token.

Cenário Positivo – Utilizando o refresh_token para gerar um novo token

> **OBS:** Sempre verificar o tempo de expiração do refresh_token, através do campo “refresh_expires_in”

(Exemplo: 1800 segundos). Caso esteja expirado, deverá gerar um novo access_token. Quando da utilização do refresh_token, não será necessário informar os campos “username” e “password”.

Cenário de Erro <400 - BAD_REQUEST>

Cenários de Erro <401 - UNATHORIZED>

### 7.2. Cadastro de Boletos

A operação POST “criar” é responsável pela geração de boletos individuais de Cobrança (Tradicional, Híbrido ou com Distribuição de Crédito por Boleto). O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### ENTRADA

### SAÍDA

### RETORNO (Response)

### FALHAS

### REQUEST – POST

### URL Sandbox (Ambiente para homologação)

```text
Ord         Nome                Tipo   Parametro Tamanho             Formatação             Descrição         Obrigatório
                                                                                         Access token
                                       Header
1     x-api-key             String                 38            UUID                    gerado no portal Obrigatório
                                       Param
                                                                                         do desenvolvedor
                                                                                         Campo
                                                                                         access_token
                                       Header                    Bearer + Token de
1     Authorization         String                 1413                                  obtido na          Obrigatório
                                       Param                     Autenticação
                                                                                         saída da
                                                                                         autenticação.
                                                                                         Estabelece o
                           Applicatio Header                                             tipo de
2     Content-Type                                               Sem formatação
                           n/json     param                                              conteúdo da
                                                                                         requisição

                                   Header                                               Código da
3   cooperativa           String                  4             Sem formatação                              Obrigatório
                                   Param                                                Cooperativa -
                                   Header                                        Código da
4   posto                 String                  2             Sem formatação                              Obrigatório
                                   Param                                         Agência
                                                                                 Indica o tipo de
                                                                                 cobrança do
                                                                                 boleto, podendo
                                                                Domínios: NORMAL
5   tipoCobranca          String   Body           7                              ser: NORMAL                Obrigatório
                                                                ou HIBRIDO
                                                                                 (Tradicional, sem
                                                                                 QRCode) ou
                                                                                 Híbrido
                                                                                 Código do
    codigoBeneficia
6                         String   Body           5             Sem formatação   Convênio de                Obrigatório
    rio
                                                                                 Informações do
7   pagador               Objeto   Body                                                                     Obrigatório
                                                                                 Pagador
                                                                Domínios:
                                                                                 Tipo de pessoa
8   tipoPessoa            String   Body           15            PESSOA_JURIDICA                             Obrigatório
                                                                                 do Pagador
                                                                ou PESSOA_FISICA
                                                                                        CPF ou CNPJ do
9   documento             String   Body           14            Sem formatação          Pagador do          Obrigatório
                                                                                        boleto
                                                                                        Nome do
                                                                                        Pagador
                                                                                        (Caso o nome
                                                                                        ultrapasse 40
10 nome                   String   Body           200           Sem formatação          caracteres,         Obrigatório
                                                                                        haverá uma
                                                                                        abreviação a
                                                                                        partir do
                                                                                        caractere 40.)
                                                                                                            Opcional
                                                                                                            (obrigatório se o
                                                                                                            cadastro do
                                                                                        Endereço do
11 endereco               String   Body           40            Sem formatação                              beneficiário está
                                                                                        Pagador
                                                                                                            configurado
                                                                                                            para validar
                                                                                                            cep)
                                                                                                            Opcional
                                                                                                            (obrigatório se o
                                                                                        Cidade do
12 cidade                 String   Body           40            Sem formatação                              cadastro do
                                                                                        Pagador
                                                                                                            beneficiário está
                                                                                                            configurado

                                                                                                            para validar
                                                                                                            cep)

                                                                                                            Opcional
                                                                                                            (obrigatório se o
                                                                                                            cadastro do
13 uf                    String   Body           2             Sem formatação          UF do Pagador        beneficiário está
                                                                                                            configurado
                                                                                                            para validar
                                                                                                            cep)
                                                                                                            Opcional
                                                                                                            (obrigatório se o
                                                                                                            cadastro do
14 cep                   String   Body           8             0-9                     CEP do Pagador       beneficiário está
                                                                                                            configurado
                                                                                                            para validar
                                                                                                            cep)
                                                                                       Telefone do
15 telefone              String   Body           11            0-9                                          Opcional
                                                                                       Pagador
                                                                                       Email do
16 email                 String   Body           40            xxx@xx.xx                                    Opcional
                                                                                       Pagador
                                                                                       Informações do
17 beneficiarioFinal Objeto       Body                                                 Beneficiário Final   Opcional
                                                                                       (Avalista)
                                                                                       CPF/CNPJ do
18 documento             String   Body           14            Sem Formatação                               Obrigatório
                                                                                       Beneficiário Final
                                                               Domínios:
19 tipoPessoa            String   Body           15            PESSOA_JURIDICA         Tipo de Pessoa       Obrigatório
                                                               ou PESSOA_FISICA
                                                                                       Nome do
20 nome                  String   Body           40            Sem formatação                               Obrigatório
                                                                                       Beneficiário Final
                                                                                       Logradouro do
21 logradouro            String   Body           40            Sem formatação                               Opcional
                                                                                       Beneficiário Final
                                                                                       Número do
   numeroEnderec
23                       String   Body           20            Sem formatação          endereço do          Opcional
   o
                                                                                       Beneficiário Final
                                                                                       Cidade do
24 cidade                String   Body           25            Sem formatação                               Opcional
                                                                                       Beneficiário Final
                                                                                       UF do
25 uf                    String   Body           2             Sem formatação                               Opcional
                                                                                       Beneficiário Final
                                                                                       CEP do
26 cep                   String   Body           8             0-9                                          Opcional
                                                                                       Beneficiário Final

                                                                                       Telefone do
27 telefone              String   Body           11            0-9                                          Opcional
                                                                                       Beneficiário Final
                                                                                       Email do
28 email                 String   Body           40            xxx@xx.xx                                    Opcional
                                                                                       Beneficiário Final
                                                                                       Espécie de
                                                                                       documento do
                                                                                       título, podendo
                                                                                       ser:

                                                                                        A - DUPLICATA
                                                                                        MERCANTIL
                                                               Domínios:                INDICAÇÃO

                                                               DUPLICATA_MERCAN         B - DUPLICATA
                                                               TIL_INDICACAO            RURAL

                                                               DUPLICATA_RURAL          C - NOTA
                                                                                        PROMISSO
                                                               NOTA_PROMISSORIA         RIA

                                                               NOTA_PROMISSORIA         D - NOTA
                                                               _RURAL                   PROMISSO
                                                                                        RIA RURAL
                                                               NOTA_SEGUROS
                                                                                        E - NOTA
   especieDocume                                               RECIBO                   DE
29                       String   Body           29                                                         Obrigatório
   nto                                                                                  SEGUROS
                                                               LETRA_CAMBIO
                                                                                        G – RECIBO
                                                               NOTA_DEBITO

                                                               DUPLICATA_SERVICO        H - LETRA
                                                               _INDICACAO               DE CAMBIO

                                                               OUTROS                   I - NOTA DE
                                                                                        DÉBITO
                                                               BOLETO_PROPOSTA
                                                                                        J - DUPLICATA DE
                                                               CARTAO_CREDITO           SERVICO
                                                                                        INDICAÇÃO
                                                               BOLETO_DEPOSITO
                                                                                        K – OUTROS

                                                                                        O - BOLETO
                                                                                        PROPOSTA

                                                                                        P - CARTÃO DE
                                                                                        CRÉDITO

                                                                                           Q – BOLETO
                                                                                           DEPÓSITO

                                                                                                               Opcional. Caso
                                                                                                               o beneficiário
                                                                                                               não informe, o
30 nossoNumero             String    Body           9             Sem formatação          Nosso número
                                                                                                               Sicredi gera
                                                                                                               automaticamen
                                                                                                               te.
                                                                                          Número de
                                                                                          controle interno
31 seuNumero               String    Body           10            Sem formatação          do beneficiário      Obrigatório
                                                                                          que faz referência
                                                                                          ao pagador.
                                                                                          Id de controle do
                                                                                          beneficiário.
                                                                                          Semelhante ao
32 idTituloEmpresa         String    Body           25            Sem formatação                               Opcional
                                                                                          “seuNumero”
                                                                                          que permite mais
                                                                                          caracteres.
                                                                                          Data de
32 dataVencimento          Date      Body           10            YYYY-MM-DD              vencimento do        Obrigatório
                                                                                          boleto
                                                                                          Quantidade de
                                                                                                               Opcional. Não
                                                                                          dias, após o
                                                                                                               poderá ser
                                                                                          vencimento, em
                                                                                                               informado, caso
     diasProtestoAut                                                                      que será
33                         Integer   Body           2             Sem formatação                               o campo
     o                                                                                    realizado o
                                                                                                               “diasNegativaca
                                                                                          protesto
                                                                                                               oAuto” esteja
                                                                                          automático do
                                                                                                               preenchido
                                                                                          boleto
                                                                                          Quantidade de        Opcional. Não
     diasNegativacao                                                                      dias, após o         poderá ser
34                         Integer   Body           2             Sem formatação
     Auto                                                                                 vencimento, em       informado, caso
                                                                                          que o boleto será    o campo

                                                                                          negativado       “diasProtestoAu
                                                                                          automaticamente to” esteja
                                                                                          .                preenchido
                                                                                                           Opcional. Caso
                                                                                                           não informado,
                                                                                                           será preenchido
                                                                                          Quantidade de
                                                                                                           com o valor
                                                                                          dias que o QR
                                                                                                           contido no
                                                                                          Code continuará
     validadeAposVe                                                                                        campo de
35                         Integer   Body           4             Sem formatação          válido após o
     ncimento                                                                                              quantidade de
                                                                                          vencimento, caso
                                                                                                           dias de baixa
                                                                                          seja um boleto
                                                                                                           automática
                                                                                          híbrido.
                                                                                                           existente no
                                                                                                           cadastro do
                                                                                                           beneficiário
                                                                                                           Obrigatório.
                                                                                                           (Caso seja tipo
                                                                                                           de boleto
                                                                                                           NORMAL
                                                                                                           (Tradicional) e
36 valor                   number    Body           14,2          Sem formatação          Valor do boleto
                                                                                                           espécie Boleto
                                                                                                           Proposta pode
                                                                                                           ser informado
                                                                                                           0,00)

                                                                                          Tipo de
                                                                                          desconto
                                                                  Domínios: VALOR
37 tipoDesconto            String    Body           10                                    podendo ser:        Opcional
                                                                  ou PERCENTUAL
                                                                                          A - VALOR
                                                                                          B - PERCENTUAL
                                                                                                              Opcional. Será
                                                                                                              obrigatório se o
                                                                                                              campo
                                                                                                              dataDesconto1
                                                                                                              for informado.
                                                                                          Valor de
38 valorDesconto1          number    Body           14,2          Sem formatação                              Não deverá ser
                                                                                          desconto 1
                                                                                                              informado caso
                                                                                                              o desconto
                                                                                                              antecipado
                                                                                                              esteja
                                                                                                              preenchido

                                                                                                          Opcional. Será
                                                                                                          obrigatório se o
                                                                                                          campo
                                                                                                          valorDesconto1
                                                                                                          for informado.
                                                                                                          Não pode ser
                                                                                                          igual as outras
                                                                                       Data limite para   datas de
39 dataDesconto1         Date     Body           10            YYYY-MM-DD              concessão de       desconto e nem
                                                                                       desconto1          maior que
                                                                                                          vencimento.
                                                                                                          Não deverá ser
                                                                                                          informado caso
                                                                                                          o desconto
                                                                                                          antecipado
                                                                                                          esteja
                                                                                                          preenchido
                                                                                                          Opcional. Será
                                                                                                          obrigatório se o
                                                                                                          campo
                                                                                                          dataDesconto2
                                                                                                          for informado.
                                                                                       Valor de
40 valorDesconto2        number   Body           14,2          Sem formatação                             Não deverá ser
                                                                                       desconto 2
                                                                                                          informado caso
                                                                                                          o desconto
                                                                                                          antecipado
                                                                                                          esteja
                                                                                                          preenchido
                                                                                                          Opcional. Será
                                                                                                          obrigatório se o
                                                                                                          campo
                                                                                                          valorDesconto2
                                                                                                          for informado.
                                                                                                          Não pode ser
                                                                                                          igual as outras
                                                                                       Data limite para a datas de
41 dataDesconto2         Date     Body           10            YYYY-MM-DD              concessão de       desconto e nem
                                                                                       desconto 2         maior que
                                                                                                          vencimento.
                                                                                                          Não deverá ser
                                                                                                          informado caso
                                                                                                          o desconto
                                                                                                          antecipado
                                                                                                          esteja
                                                                                                          preenchido

                                                                                                            Opcional. Será
                                                                                                            obrigatório se o
                                                                                                            campo
                                                                                                            dataDesconto3
                                                                                                            for informado.
                                                                                         Valor de
42 valorDesconto3          number   Body           14,2          Sem formatação                             Não deverá ser
                                                                                         desconto 3
                                                                                                            informado caso
                                                                                                            o desconto
                                                                                                            antecipado
                                                                                                            esteja
                                                                                                            preenchido
                                                                                                            Opcional. Será
                                                                                                            obrigatório se o
                                                                                                            campo
                                                                                                            valorDesconto3
                                                                                                            for informado.
                                                                                                            Não pode ser
                                                                                                            igual as outras
                                                                                         Data limite para a datas de
43 dataDesconto3           Date     Body           10            YYYY-MM-DD              concessão de       desconto e nem
                                                                                         desconto 3         maior que
                                                                                                            vencimento.
                                                                                                            Não deverá ser
                                                                                                            informado caso
                                                                                                            o desconto
                                                                                                            antecipado
                                                                                                            esteja
                                                                                                            preenchido
                                                                                                            Opcional. Não
                                                                                         Valor de           deverá ser
     descontoAnteci                                                                                         informado caso
44                         number   Body           14,2          Sem formatação          Desconto
     pado                                                                                                   o desconto
                                                                                         Antecipado
                                                                                                            normal esteja
                                                                                                            preenchido
                                                                                         Tipo de
                                                                                         Juros,
                                                                 Domínios: VALOR
45 tipoJuros               String   Body           10                                    podendo ser:       Opcional
                                                                 ou PERCENTUAL
                                                                                         A - VALOR
                                                                                         B - PERCENTUAL
                                                                                         Tipo de Juros
     tipoJurosPercen                                             Domínios: DIARIO        Percentual:
46                         String   Body           6                                                        Opcional
     tual                                                        ou MENSAL               A - DIARIO
                                                                                         B - MENSAL
                                                                                         Valor de juros a
47 juros                   number   Body           14,2          Sem formatação                             Opcional
                                                                                         cobrar por dia

                                                                                                                  Opcional, se
                                                                                            Data de início        informada o
48 dataInicioJuros         number      Body           10            YYYY-MM-DD              para cobrança         boleto sempre
                                                                                            dos Juros             será tradicional
                                                                                                                  (sem QR Code)
                                                                    Domínios: VALOR
47 tipoMulta               String      Body           10                                    Tipo de multa         Opcional
                                                                    ou PERCENTUAL
                                                                                            Percentual ou
48 multa                   number      Body           5,2           Sem formatação          valor de multa a      Opcional
                                                                                            cobrar
                                                                                                                  Opcional, se
                                                                                            Data de início        informada o
49 dataInicioMulta         number      Body           10            YYYY-MM-DD              para cobrança da      boleto sempre
                                                                                            Multa                 será tradicional
                                                                                                                  (sem QR Code)
                                                                                     Permitido
                                                                                     informar uma
                                                                                     lista de até 5
                          List<Strin
50 informativo                         Body           400           Sem formatação   informativos, com Opcional
                          g>
                                                                                     limite de 80
                                                                                     caracteres para
                                                                                     cada.
                                                                                     Permitido
                                                                                     informar uma
                                                                                     lista de até 4
                          List<Strin
51 mensagem                            Body           320           Sem formatação   mensagens, com Opcional
                          g>
                                                                                     limite de 80
                                                                                     caracteres para
                                                                                     cada
                                                                                                          Opcional. Será
                                                                                     Informações da       obrigatório caso
                          List<Strin                                                 distribuição de      seja um boleto
52 splitBoleto                         Body
                          g>                                                         crédito por          com
                                                                                     boleto.              Distribuição de
                                                                                                          Crédito.
                                                                                     Regra do rateio      Obrigatório caso
                                                                    Domínios:
                                                                                     que será utilizada seja um boleto
                                                                    - MENOR_VALOR
53 regraRateio             String      Body           14                             no Split -           com
                                                                    - VALOR_COBRADO
                                                                                     distribuição do      Distribuição de
                                                                    - VALOR_REGISTRO
                                                                                     crédito.             Crédito
                                                                                                          Obrigatório caso
                                                                                     Repasse
                                                                    Domínios:                             seja um boleto
     repasseAutomat                                                                  automático do
54                         String      Body           3             - SIM                                 com
     icoSplit                                                                        Split - distribuição
                                                                    - NAO                                 Distribuição de
                                                                                     do crédito.
                                                                                                          Crédito

                                                                                                                Obrigatório caso
                                                                  Domínios:                                     seja um boleto
                                                                                          Tipo de valor do
55 tipoValorRateio         String    Body           10            - PERCENTUAL                                  com
                                                                                          rateio do Split.
                                                                  - VALOR                                       Distribuição de
                                                                                                                Crédito.
                                                                                          Contas
                                                                                                                Obrigatório caso
                                                                                          destinatárias do
                                                                                                                seja um boleto
                          List<order                                                      Split do boleto.
56 destinatarios                     Body                                                                       com
                          edMap>                                                          Poderá ser
                                                                                                                Distribuição de
                                                                                          informado de 1 a
                                                                                                                Crédito.
                                                                                          30 contas.
                                                                                                                Obrigatório caso
                                                                                          Código da
                                                                                                                seja um boleto
                                                                                          agência com 4
57 codigoAgencia           String    Body           4             Sem formatação                                com
                                                                                          dígitos
                                                                                                                Distribuição de
                                                                                          numéricos.
                                                                                                                Crédito
                                                                                                                Obrigatório caso
                                                                                          Código do banco       seja um boleto
58 codigoBanco             String    Body           3             Sem formatação          com 3 dígitos         com
                                                                                          numéricos.            Distribuição de
                                                                                                                Crédito.
                                                                                                                Obrigatório caso
                                                                                          Float Split.
                                                                                                                seja um boleto
                                                                                          Poderá ser
59 floatSplit              Integer   Body           2             Sem formatação                                com
                                                                                          informado de 0 a
                                                                                                                Distribuição de
                                                                                          30.
                                                                                                                Crédito.
                                                                                          Nome do
                                                                                          destinatário.
                                                                                          Poderá ser            Obrigatório caso
                                                                                          informado de 3 a      seja um boleto
     nomeDestinatar
60                         String    Body           40            Sem formatação          40 caracteres.        com
     io
                                                                                          Não poderá            Distribuição de
                                                                                          informar              Crédito.
                                                                                          caracteres
                                                                                          especiais.
                                                                                          Número da conta       Obrigatório caso
                                                                                          corrente. Poderá      seja um boleto
     numeroContaCo
61                         String    Body           13            Sem formatação          ser informado de      com
     rrente
                                                                                          4 a 13 caracteres     Distribuição de
                                                                                          numéricos.            Crédito.
                                                                                                                Obrigatório caso
                                                                                          Documento do
                                                                                                                seja um boleto
                                                                                          destinatário.
62 numeroCpfCnpj           String    Body           14            Sem formatação                                com
                                                                                          Poderá ser
                                                                                                                Distribuição de
                                                                                          informado até 14
                                                                                                                Crédito.

                                                                                                caracteres
                                                                                                numéricos.
                                                                                                Número da
                                                                                                parcela do rateio.
                                                                                                                     Obrigatório caso
                                                                                                Poderá ser
                                                                                                                     seja um boleto
                                                                                                informado de 1 a
63 parcelaRateio            Integer        Body           2             Sem formatação                               com
                                                                                                30 parcelas. A
                                                                                                                     Distribuição de
                                                                                                sequência deverá
                                                                                                                     Crédito.
                                                                                                ser em ordem
                                                                                                crescente.
                                                                                                Valor/percentual
                                                                                                                     Obrigatório caso
                                                                                                do rateio. Poderá
                                                                                                                     seja um boleto
      valorPercentual                                                                           ser informado até
64                          Number         Body           14,2          Sem formatação                               com
      Rateio                                                                                    16 dígitos, sendo
                                                                                                                     Distribuição de
                                                                                                dois deles para a
                                                                                                                     Crédito.
                                                                                                casa decimal.

Ord    Nome               Tipo        Tamanho       Formatação           Descrição

1      txid               String      32            Sem formatação       Identificador da transação Pix (quando for tipo de boleto
                                                                         híbrido)

2      qrCode             String      Até 365       Sem formatação       QR CODE utilizado para leitura na opção de Boleto Híbrido

3      linhaDigitável String          47            Sem formatação       Linha Digitável

4      codigoBarras       String      44            Sem formatação       Código de Barras

5      cooperativa        String      4             Sem formatação       Código da Cooperativa do Beneficiário

6      posto              String      2             Sem formatação       Código da Agência do Beneficiário

7      nossoNumero String             9             Sem formatação       Nosso Número

Status                                                                  Descrição
HTTP_ACCEPTED (201)                                                     Operação aceita com sucesso

                                                Parâmetros
           Status             Mensagem           sujeitos à                            Descrição
                                                   crítica
                             Ocorreu um
                             erro ao
HTTP_NOT_FOUND (404)         realizar         Não possui          Erro ocasionado por URL inválida.
                             operação de
                             cadastro
                             Tipo de                              Erro ocasionado por parâmetro obrigatório não
HTTP_UNSUPPORTED_MEDIA                        Parâmetro:
                             conteúdo não                         informado ou inválido.
_TYPE (415)                                   Content-Type
                             suportado

                                              Parâmetro:          Erro ocasionado por parâmetro obrigatório não
HTTP_UNAUTHORIZED (401)      Unauthorized
                                              Authorization       informado ou inválido.
                             Cooperativa
                                                                  Erro ocasionado quando o código de cooperativa
HTTP_UNAUTHORIZED            diferente da     Parâmetro:
                                                                  informado no cadastro for diferente do contido na
(401)                        cooperativa do   cooperativa
                                                                  autenticação.
                             usuário

                             Código de
                             beneficiário    Parâmetro:           Erro ocasionado quando o código de beneficiário
HTTP_UNAUTHORIZED
                             diferente do    codigoBenefici       informado no cadastro for diferente do contido na
(401)
                             beneficiário do ario                 autenticação.
                             usuário

                                              Qualquer
                                              parâmetro que
                                              consta como
                             Campo                            Erros ocasionados por parâmetros obrigatórios não
                                              obrigatório na
HTTP_BAD_REQUEST (400)       obrigatório em                  informados ou
                                              listagem de
                             branco                           inválidos.
                                              parâmetros de
                                              entrada pode
                                              ser criticado
                             O campo
                             cooperativa
                             deve ser
                                              Parâmetro:          Erros ocasionados por parâmetros obrigatórios não
HTTP_BAD_REQUEST (400)       preenchido
                                              cooperativa         preenchidos ou inválidos.
                             com 4 (quatro)
                             caracteres
                             numéricos.

                             O campo
                             posto deve ser
                                              Parâmetro:          Erros ocasionados por parâmetros obrigatórios não
HTTP_BAD_REQUEST (400)       preenchido
                                              posto               preenchidos ou inválidos
                             com 2 (dois)
                             caracteres.
                             O campo tipo
HTTP_BAD_REQUEST (400)       cobrança deve    Parâmetro:          Erro ocasionado por parâmetro obrigatório não
                             ser HíBRIDO      tipoCobranca        informado ou inválido.
                             ou NORMAL
                             O campo
                             código do
                             beneficiario     Parâmetro:
HTTP_BAD_REQUEST (400)                                             Erro ocasionado por parâmetro obrigatório não
                             deve ter 5       codigoBenefici
                                                                  informado ou inválido.
                             (cinco)          ario
                             caracteres
                             numéricos.
                             A data de
                             vencimento do
                                               Parâmetro:
                             boleto é                             Erro ocasionado por parâmetro obrigatório não
HTTP_BAD_REQUEST (400)                        dataVencimen
                             obrigatória e                        preenchido ou inválido.
                                              to
                             deve ser
                             preenchida.

                             Os dias de
                             protesto
                                              Parâmetro:
HTTP_BAD_REQUEST (400)       automático do                        O campo "diasProtestoAuto" não foi preenchido com
                                              diasProtestoA
                             boleto deve                          valores entre 3 e 99.
                                              uto
                             ter entre 3 e
                             99 dias.
                             O valor de
                             juros do
                                                                  O campo "juros" foi preenchido com um valor igual
HTTP_BAD_REQUEST (400)       boleto deve      Parâmetro:
                                                                  ou menor que zero.
                             ser superior a   juros
                             zero, ou não
                             informado.
                             O valor de
                             multa do
                                                                  O campo "multa" foi preenchido com um valor igual
HTTP_BAD_REQUEST (400)       boleto deve      Parâmetro:
                                                                  ou menor que zero.
                             ser superior a   multa
                             zero, ou não
                             informado.
                             O valor do
                             primeiro         Parâmetro:
HTTP_BAD_REQUEST (400)                                            O campo "valorDesconto1" foi preenchido com um
                             desconto do      valorDesconto
                                                                  valor igual ou menor que zero.
                             boleto deve      1
                             ser superior a

                             zero, ou não
                             informado.
                             O valor do
                             segundo
                             desconto do      Parâmetro:
HTTP_BAD_REQUEST (400)                                            O campo "valorDesconto2" foi preenchido com um
                             boleto deve      valorDesconto
                                                                  valor igual ou menor que zero.
                             ser superior a   2
                             zero, ou não
                             informado.
                             O valor do
                             terceiro
                             desconto do
                                              Parâmetro:
HTTP_BAD_REQUEST (400)       boleto deve                          O campo "valorDesconto3" foi preenchido com um
                                              valorDesconto
                             ser superior a                       valor igual ou menor que zero.
                                              3
                             zero, ou não
                             informado.

                             O valor de
                             desconto
                             antecipado do    Parâmetro:          O campo "descontoAntecipado" foi preenchido com
HTTP_BAD_REQUEST (400)
                             boleto deve      descontoAntec       um valor igual ou menor que zero.
                             ser superior a   ipado
                             zero, ou não
                             informado.
                             O campo
                             documento do
HTTP_BAD_REQUEST (400)                        Parâmetro:          O campo "documento" (CPF/CNPJ) do beneficiário
                             beneficiário
                                              documento           final foi preenchido de forma inválida.
                             final está
                             inválido.
                             O nome do
                             beneficiário
HTTP_BAD_REQUEST (400)                        Parâmetro:          O campo "nome" do beneficiário final foi preenchido
                             final permite
                                              nome                com mais de 40 caracteres.
                             até 40
                             caracteres.
                             O e-mail do
                             beneficiário
HTTP_BAD_REQUEST (400)                        Parâmetro:          O campo "email" do beneficiário final foi preenchido
                             final permite
                                              email               com mais de 40 caracteres.
                             até 40
                             caracteres.
                             O nome do
HTTP_BAD_REQUEST (400)                        Parâmetro:
                             pagador é                            O campo "nome" do pagador não foi informado.
                                              nome
                             obrigatório.
                             O nome do
HTTP_BAD_REQUEST (400)       pagador          Parâmetro:          O campo "nome" do pagador foi preenchido com
                             permite até 40   nome                mais de 40 caracteres.
                             caracteres.

                             O campo                               O campo "documento" do pagador (CPF/CNPJ) foi
HTTP_BAD_REQUEST (400)       documento do       Parâmetro:         preenchido de forma inválida. Deve conter entre 11
                             pagador está       documento          e 14 posições.
                             inválido.
                             Negócio:
                             Beneficiário:
                             XXXXXXXXXXX
                             XXX não tem
                                                                   O beneficiário não possui o produto Cobrança Online
HTTP_UNPROCESSABLE_ENTIT     contratado o
                                                Não possui         contratado.
Y (422)                      serviço de
                             Online
                             (ECOMM).

                             Negócio:
                             Beneficiário:
                             XXXXXXXXXXX
                             XXX esta com
HTTP_UNPROCESSABLE_ENTIT     o status de                           O cadastro do beneficiário está encerrado.
                                                Não possui
Y (422)                      convenio                              Necessário verificar com a cooperativa.
                             encerrado.
                             Verificar status
                             com a
                             cooperativa.
                             Negócio:
                                                                   O campo tipoCobranca foi preenchido como
                             Híbrido não
HTTP_UNPROCESSABLE_ENTIT                                           "HIBRIDO". Entretanto, o beneficiário não possui o
                             contratado.        Não possui
Y (422)                                                            produto Pix contratado. Necessário contatar a
                             Favor solicitar
                                                                   cooperativa.
                             a contratação.
                             Negócio:
                             Documento do
                             pagador não
                             pode ser o
                                                Parâmetros:
                             mesmo do
HTTP_UNPROCESSABLE_ENTIT                        documento          O campo "documento", tanto do pagador, quanto do
                             beneficiário
Y (422)                                         (pagador e         beneficiário final, foram preenchidos com o mesmo
                             final para
                                                beneficiario       CPF/CNPJ.
                             espécie de
                                                final)
                             documento
                             DUPLICATA_M
                             ERCANTIL_IND
                             ICACA"
                             Negócio:
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:
                             Documento do                          O campo "documento" do pagador foi preenchido
Y (422)                                         documento
                             Beneficiário                          com o mesmo CPF/CNPJ do beneficiário.
                                                (pagador)
                             não pode ser o

                             mesmo do
                             Pagador.
                             Negócio:
                             Tí-tulo com
                             Protesto
                             Automático, o
                             beneficiário
                                              Parâmetro:
                             final precisa
HTTP_UNPROCESSABLE_ENTIT                      Documento
                             ser pessoa                           Não foram informados os dados do beneficiário final.
Y (422)                                       (beneficiário
                             jurí-dica para
                                              final)
                             espécie de
                             documento
                             DUPLICATA_M
                             ERCANTIL_IND
                             ICACAO
                             Negócio:
                             Documento do
                                              Parâmetro:
HTTP_UNPROCESSABLE_ENTIT     Beneficiário
                                              documento           O campo "documento" do beneficiário final foi
Y (422)                      não pode ser o
                                              (beneficiário       preenchido com o mesmo CPF/CNPJ do beneficiário.
                             mesmo do
                                              final)
                             Beneficiário
                             final.
                             Negócio: Data
                             de vencimento
HTTP_UNPROCESSABLE_ENTIT                      Parâmetro:          O campo "dataVencimento" foi preenchido com uma
                             tem que ser
Y (422)                                       dataVencimen        data retroativa, ou seja, uma data anterior à data
                             posterior ou
                                              to                  atual.
                             igual a data
                             atual.
                                                                  O campo "especieDocumento" foi preenchido com:

                                                                  - Recibo
                             Negócio:                             - Nota de Débito
                             Espécie de                           - Outros
HTTP_UNPROCESSABLE_ENTIT                      Parâmetro:
                             documento                            - Boleto Proposta
Y (422)                                       especieDocum
                             não permite                          - Cartão de Crédito
                                              ento
                             protesto /                           - Boleto Deposito
                             negativação.
                                                                  Para essas espécies, não é permitido informar os
                                                                  campos "diasProtestoAuto" e
                                                                  "diasNegativacaoAuto".
                             Negócio: Não
                             permite nem
HTTP_UNPROCESSABLE_ENTIT
                             juros e nem      Parâmetros:         Os campos "multa" e/ou "juros" foram informados
Y (422)
                             multa para       multa e juros       para título com espécie Boleto Proposta.
                             Espécie de
                             documento

                             BOLETO_PROP
                             OSTA
                             Negócio: Valor
                                                                  O campo "valor" foi preenchido com um valor igual
                             do titulo deve
                                                                  ou menor que zero. Só poderá ser preenchido com
                             ser maior que
HTTP_UNPROCESSABLE_ENTIT                                          valor igual a zero caso a espécie do boleto seja:
                             zero para         Parâmetro:
Y (422)
                             espécie           valor
                                                                  - Boleto Proposta
                             DUPLICATA_M
                                                                  - Cartão de Crédito
                             ERCANTIL_IND
                             ICACAO
                             Negócio: Não
                             permite juros
                             maior ou igual
                             ao valor do
HTTP_UNPROCESSABLE_ENTIT
                             titulo para       Parâmetro:         O campo "juros" foi preenchido com um valor maior
Y (422)
                             Espécie de        juros              ou igual ao valor do título.
                             documento
                             DUPLICATA_M
                             ERCANTIL_IND
                             ICACAO
                             Negócio:
HTTP_UNPROCESSABLE_ENTIT     Percentual
                                               Parâmetro:         O percentual de juros informado foi maior ou igual a
Y (422)                      juros deve ser
                                               juros              100%.
                             menor que
                             100%
                             Negócio: O
                             valor da multa
                             aplicada
                             R$ X.XXXXXX
                             não pode ser
                             menor que
                             R$ 0,01.
                             Exemplo: Se o
HTTP_UNPROCESSABLE_ENTIT     valor do título                      O campo "multa" (percentual) foi preenchido com
                                               Parâmetro:
Y (422)                      for de R$ 5,00                       um valor muito baixo, fazendo com que o cálculo da
                                               multa
                             e a multa for                        multa seja inferior a R$ 0,01.
                             0,01%. O valor
                             aplicado será
                             de R$ 5,0005 e
                             a multa
                             aplicada será
                             de R$ 0,0005 e
                             isso não será
                             permitido.
HTTP_UNPROCESSABLE_ENTIT     Negócio: O                           O campo "valorDesconto1" foi preenchido com um
                                               Parâmetro:
Y (422)                      valor de                             valor maior ou igual ao título.

HTTP_UNPROCESSABLE_ENTIT     Desconto 1       valorDesconto
Y (422)                      deve ser         1
                             menor que o
                             valor do
                             boleto.
                             Negócio: O
                             valor de
HTTP_UNPROCESSABLE_ENTIT     Desconto 2       Parâmetro:          O campo "valorDesconto2" foi preenchido com um
Y (422)                      deve ser         valorDesconto       valor maior ou igual ao título.
                             menor que o      2
                             valor do
                             boleto.
                             Negócio: O
                             valor de
HTTP_UNPROCESSABLE_ENTIT     Desconto 3       Parâmetro:          O campo "valorDesconto3" foi preenchido com um
Y (422)                      deve ser         valorDesconto       valor maior ou igual ao título.
                             menor que o      3
                             valor do
                             boleto.
                             Negócio:
                             Percentual
HTTP_UNPROCESSABLE_ENTIT                      Parâmetro:          O percentual do desconto 1 foi preenchido com um
                             desconto 1
Y (422)                                       valorDesconto       valor maior ou igual a 100%.
                             invalido: deve
                                              1
                             ser menor que
                             100%
                             Negócio:
                             Percentual
HTTP_UNPROCESSABLE_ENTIT                      Parâmetro:          O percentual do desconto 2 foi preenchido com um
                             desconto 2
Y (422)                                       valorDesconto       valor maior ou igual a 100%.
                             invalido: deve
                                              2
                             ser menor que
                             100%
                             Negócio:
                             Percentual
HTTP_UNPROCESSABLE_ENTIT                      Parâmetro:          O percentual do desconto 3 foi preenchido com um
                             desconto 3
Y (422)                                       valorDesconto       valor maior ou igual a 100%.
                             invalido: deve
                                              3
                             ser menor que
                             100%
                             Negócio: Data
HTTP_UNPROCESSABLE_ENTIT     de desconto 1    Parâmetro:          O campo "dataDesconto1" foi preenchido com uma
Y (422)                      não deve ser     dataDesconto        data retroativa, ou seja, menor que a data atual.
                             menor que        1
                             data atual.
HTTP_UNPROCESSABLE_ENTIT     Negócio: Data    Parâmetro:          O campo "dataDesconto2" foi preenchido com uma
Y (422)                      de desconto 2    dataDesconto        data retroativa, ou seja, menor que a data atual.
                             não deve ser     2

                             menor que
                             data atual.
                             Negócio: Data
HTTP_UNPROCESSABLE_ENTIT     de desconto 3      Parâmetro:         O campo "dataDesconto3" foi preenchido com uma
Y (422)                      não deve ser       dataDesconto       data retroativa, ou seja, menor que a data atual.
                             menor que          3
                             data atual.
                             Negócio: Valor
                             desconto
                             antecipado
                             invalido:                             O campo "descontoAntecipado" foi preenchido com
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:
                             maior/igual ao                        um valor que, após calculado, ficaria igual ou maior
Y (422)                                         descontoAntec
                             valor do titulo.                      que o valor do título.
                                                ipado
                             Valor desconto
                             antecipado
                             calculado:
                             XXX.XX
                             Negócio: Dias
                             de negativação
                             inválido para a
                             UF de Mato
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:         O campo "diasNegativacaoAuto" não foi preenchido
                             Grosso do Sul.
Y (422)                                         diasNegativaca     com valores entre 46 e 99.
                             Para este
                                                oAuto
                             estado o
                             mí-nimo
                             precisa ser 46
                             e máximo 99.
                             Negócio: Não
                             é permitido
                                                Parâmetros:
                             cadastro com                          Os campos "diasNegativacaoAuto" e
                                                diasNegativaca
HTTP_UNPROCESSABLE_ENTIT     negativação e                         "diasProtestoAuto" foram preenchidos. É permitido
                                                oAuto e
Y (422)                      protesto                              somente um dos campos.
                                                diasProtestoA
                             automático
                                                uto
                             simultaneame
                             nte.
                             Negócio:
                             Beneficiário:
                             XXXXXXXXXXX
                             XXX optou por
                             validar CEP do
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:         O campo "cep" do pagador foi preenchido com um
                             pagador. O
Y (422)                                         cep                cep inválido.
                             CEP:
                             XXXXXXXX não
                             foi localizado
                             no cadastro de
                             CEPs para o

                             pagador com o
                             documento:
                             XXXXXXXXXXX
                             Negócio: CEP
                             do pagador
                             esta inválido
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:         O campo "cep" do pagador foi preenchido com um
                             com protesto
Y (422)                                         cep                cep inválido.
                             ou
                             negativação
                             automática
                             Negócio:
                             Beneficiário:
                             XXXXXXXXXXX
                             XXX esta
                             parametrizado
HTTP_UNPROCESSABLE_ENTIT     como Terceiro
                                                                   Não foram informados os dados do beneficiário final.
Y (422)                      Habilitado,
                             deve ser
                             enviado
                             informações
                             do beneficiário
                             final.
                             Negócio: Não
                             permitido
                             gerar QR Code
                                                                   O campo "tipoCobranca" foi preenchido como
                             para
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:         "HIBRIDO". Entretanto, quando o cadastro do
                             beneficiário
Y (422)                                         tipoCobranca       beneficiário está configurado para permitir editar o
                             que edita
                                                                   valor, não é possível cadastrar boleto híbrido.
                             valor. Favor
                             solicitar boleto
                             normal.
                             Negócio:
                             Beneficiário                          O campo "diasNegativacaoAuto" foi preenchido.
HTTP_UNPROCESSABLE_ENTIT                        Parâmetro:
                             sem permissão                         Entretanto, o cadastro do beneficiário não possui o
Y (422)                                         diasNegativaca
                             para cadastro                         produto Serasa ativo.
                                                oAuto
                             com
                             negativação
                             Negócio:
HTTP_UNPROCESSABLE_ENTIT     Beneficiário
                                                                   O beneficiário final está com o status de inapto.
Y (422)                      final com          Não se aplica
                             situação
                             inapto.
                             O repasse          Parâmetro:
HTTP_BAD_REQUEST (400)       automático do      repasseAutom       O parâmetro não foi enviado.
                             é obrigatório e    aticoSplit

                             deve ser
                             preenchido.
                             O repasse
                                               Parâmetro:
                             automático do                        O parâmetro foi preenchido com conteúdo diferente
HTTP_BAD_REQUEST (400)                         repasseAutom
                             deve ser SIM                         de SIM ou NÃO.
                                               aticoSplit
                             ou NÃO.
                             O campo tipo
                             de valor do
                                               Parâmetro:
                             rateio é
HTTP_BAD_REQUEST (400)                         tipoValorRatei     O parâmetro não foi enviado.
                             obrigatório e
                                               o
                             deve ser
                             preenchido.
                             O campo tipo
                             de valor do       Parâmetro:
                                                                  O parâmetro foi preenchido com conteúdo diferente
HTTP_BAD_REQUEST (400)       rateio deve ser   tipoValorRatei
                                                                  de VALOR ou PERCENTUAL.
                             VALOR ou          o
                             PERCENTUAL.
                             O campo de
                             regra de rateio
                             deve ser
                             VALOR_COBRA
                                               Parâmetro:
HTTP_BAD_REQUEST (400)       DO,                                  O parâmetro não foi enviado ou está inválido.
                                               regraRateio
                             VALOR_REGIST
                             RO ou
                             MENOR_VALO
                             R.
                             A lista
                             destinatários
                             não pode ser
                             nula ou vazia e   Parâmetro:         O parâmetro não contém pelo menos um
HTTP_BAD_REQUEST (400)
                             deve conter       destinatarios      destinatário.
                             pelo menos
                             um
                             destinatário.
                             A lista
                             destinatários
                             deve conter no
                                               Parâmetro:         O parâmetro não contém pelo menos um
HTTP_BAD_REQUEST (400)       mínimo 1
                                               destinatarios      destinatário.
                             destinatário e
                             no máximo 30
                             destinatários.
                             O código do
                             banco é
                                               Parâmetro:
HTTP_BAD_REQUEST (400)       obrigatório                          O parâmetro foi preenchido com dados inválidos.
                                               codigoBanco
                             aceita
                             somente

                             caracteres
                             numéricos e
                             precisa conter
                             informações
                             válidas.
                             O código do
                             banco deve ter   Parâmetro:          O parâmetro foi preenchido com menos ou mais de
HTTP_BAD_REQUEST (400)
                             3 caracteres     codigoBanco         3 caracteres.
                             númericos.
                             O código da
                                              Parâmetro:
HTTP_BAD_REQUEST (400)       agência é                            O parâmetro não foi enviado.
                                              codigoAgencia
                             obrigatório
                             O código da
                             agência é
                             obrigatório
                             aceita
                             somente          Parâmetro:
HTTP_BAD_REQUEST (400)                                            O parâmetro foi preenchido com dados inválidos.
                             caracteres       codigoAgencia
                             numéricos e
                             precisa conter
                             informações
                             válidas
                             O código da
                             agência deve
                                              Parâmetro:          O parâmetro foi preenchido com menos/mais de 4
HTTP_BAD_REQUEST (400)       ter 4
                                              codigoAgencia       caracteres.
                             caracteres
                             númericos.
                             O número da      Parâmetro:
HTTP_BAD_REQUEST (400)       conta corrente   numeroConta         O parâmetro não foi enviado.
                             é obrigatório    Corrente
                             O número da
                             conta corrente
                                              Parâmetro:
                             deve ter de 4                        O parâmetro foi preenchido com menos de 4
HTTP_BAD_REQUEST (400)                        numeroConta
                             até 13                               caracteres ou mais de 13.
                                              Corrente
                             caracteres
                             númericos.
                             O número da
                             conta é
                             obrigatório
                             aceita
                                              Parâmetro:
                             somente
HTTP_BAD_REQUEST (400)                        numeroConta         O parâmetro foi preenchido com dados inválidos.
                             caracteres
                                              Corrente
                             numéricos e
                             precisa conter
                             informações
                             válidas

                             Documento
                                                Parâmetro:
                             (CPF|CNPJ) do
HTTP_BAD_REQUEST (400)                          numeroCpfCn        O parâmetro não foi enviado.
                             destinatário é
                                                pj
                             obrigatório
                             O CPF/CNPJ é
                             obrigatório e
                             aceita
                             somente            Parâmetro:
HTTP_BAD_REQUEST (400)       caracteres         numeroCpfCn        O parâmetro foi preenchido com dados inválidos.
                             numéricos e        pj
                             precisa conter
                             informações
                             válidas
                             O nome do          Parâmetro:
HTTP_BAD_REQUEST (400)       destinatário é     nomeDestinat       O parâmetro não foi enviado.
                             obrigatório.       ario
                             O nome do          O nome do
                             destinatário       destinatário
                                                                   O parâmetro foi preenchido com menos de 3
HTTP_BAD_REQUEST (400)       permite de 3       permite de 3
                                                                   caracteres ou mais de 40.
                             até 40             até 40
                             caracteres.        caracteres.
                             A parcela de
                                                Parâmetro:
HTTP_BAD_REQUEST (400)       rateio é                              O parâmetro não foi enviado.
                                                parcelaRateio
                             obrigatória.
                             A sequencia
                             de parcelas do
                             rateio está
                             incorreta. Essa
                                                Parâmetro:
HTTP_BAD_REQUEST (400)       sequencia                             O parâmetro foi preenchido com dados inválidos.
                                                parcelaRateio
                             deve ser de 1
                             a 30 em
                             ordem
                             crescente.
                             O Valor de
                                                Parâmetro:
                             percentual de
HTTP_BAD_REQUEST (400)                          valorPercentu      O parâmetro não foi enviado.
                             rateio é
                                                alRateio
                             obrigatório.
                             O valor de
                             percentual de
                             rateio deve ter    Parâmetro:
HTTP_BAD_REQUEST (400)       até 16 dígitos,    valorPercentu      O parâmetro foi preenchido com dados inválidos.
                             sendo dois         alRateio
                             deles para a
                             casa decimal.
                             O float do split   Parâmetro:
HTTP_BAD_REQUEST (400)                                             O parâmetro não foi enviado.
                             é obrigatório.     floatSplit

                               A quantidade
                               de dias do        Parâmetro:
HTTP_BAD_REQUEST (400)                                              O parâmetro foi preenchido com dados inválidos.
                               float deve ser    floatSplit
                               de 1 a 30 dias.
                               Negócio:
                               Quando o
                               campo
                               regraRateio for
                                                 Parâmetros:        Caso o campo regraRateio seja preenchido com os
                               VALOR_COBRA
HTTP_UNPROCESSABLE_ENTIT                         regraRateio e      domínios VALOR_COBRADO ou MENOR_VALOR, o
                               DO ou
Y (422)                                          tipoValorRatei     campo tipoValorRateio deverá ser enviado com o
                               MENOR_VALO
                                                 o                  domínio PERCENTUAL.
                               R, o campo
                               tipoValorRatei
                               o deverá ser
                               PERCENTUAL.
                               Negócio: Não
                               é permitido o
                               cadastro de
                               SPLIT para
                               boletos com
                                                                    Caso o campo regraRateio seja preenchido com o
                               DESCONTO ou
HTTP_UNPROCESSABLE_ENTIT                         Parâmetro:         domínio diferente de VALOR_COBRADO, o cadastro
                               BOLETO
Y (422)                                          regraRateio        não poderá conter descontos, e a espécie do boleto
                               PROPOSTA,
                                                                    deverá ser diferente de BOLETO_PROPOSTA.
                               com regra de
                               rateio
                               diferente de
                               VALOR_COBRA
                               DO.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos`

HEADERS: cooperativa posto Authorization x-api-key Content-Type: application/json

### REQUEST BODY (JSON): BOLETO TRADICIONAL

```json
{
"beneficiarioFinal":{
"cep":91250000,
"cidade":"PORTO ALEGRE",
"documento":"25140124069",
"logradouro":"RUA DOUTOR VARGAS NETO 980",
"nome":"FELIPE OLIVEIRA",
"numeroEndereco":119,
"tipoPessoa":"PESSOA_FISICA",
"uf":"RS"
},
"codigoBeneficiario":"12345",
"dataVencimento":"2022-07-30",
"especieDocumento":"DUPLICATA_MERCANTIL_INDICACAO",
"pagador":{
"cep":"91250000",
"cidade":"PORTO ALEGRE",
"documento":"02738306006",
"nome":"RODRIGO OLIVEIRA",
"tipoPessoa":"PESSOA_FISICA",
"endereco":"RUA DOUTOR VARGAS 150",
"uf":"RS"
},
"tipoCobranca": "NORMAL",
"nossoNumero": 600046210,
"seuNumero":"TESTE",
"valor":50.00,
"tipoDesconto": "VALOR",
"valorDesconto1":10.00,
"dataDesconto1":"2022-07-15",
"valorDesconto2":7.00,
"dataDesconto2":"2022-07-20",
"valorDesconto3":3.00,
"dataDesconto3":"2022-07-30",
"tipoJuros": "VALOR",
"tipoJurosPercentual": "DIARIO",
"juros": 5.00,
"dataInicioJuros":"2025-11-26",
"tipoMulta": "PERCENTUAL",
"multa": 3.00,
"dataInicioMulta":"2025-11-26",
"informativos": [
"info 1",
"info 2",
"info 3",
"info 4",
"info 5"
],
"mensagens": [
"mens 1",
"mens 2",
"mens 3",
"mens 4"
]
}
```

### RESPONSE BODY (JSON)

```json
{
"txid": null,
"qrCode": null,
"linhaDigitavel": "74891125110061420512803153351030188640000009990",
"codigoBarras": "74891886400000099901125100614205120315335103",
"cooperativa": "0512",
"posto": "03",
"nossoNumero": "251006142"
}
```

### REQUEST BODY (JSON): BOLETO HÍBRIDO

```json
{
"beneficiarioFinal":{
"cep":91250000,
"cidade":"PORTO ALEGRE",
"documento":"25140124069",
"logradouro":"RUA DOUTOR VARGAS 980",
"nome":"FELIPE OLIVEIRA",
"numeroEndereco":119,
"tipoPessoa":"PESSOA_FISICA",
"uf":"RS"
},
"codigoBeneficiario":"12345",
"dataVencimento":"2022-07-30",
"especieDocumento":"DUPLICATA_MERCANTIL_INDICACAO",
"pagador":{
"cep":"91250000",
"cidade":"PORTO ALEGRE",
"documento":"02738306006",
"nome":"RODRIGO OLIVEIRA",
"tipoPessoa":"PESSOA_FISICA",
"endereco":"RUA DOUTOR VARGAS NETO 150",
"uf":"RS"
},
"tipoCobranca":"HIBRIDO",
"nossoNumero": 600046210,
"seuNumero":"TESTE",
"valor":50.00,
"tipoDesconto": "VALOR",
"valorDesconto1":10.00,
"dataDesconto1":"2022-07-15",
"valorDesconto2":7.00,
"dataDesconto2":"2022-07-20",
"valorDesconto3":3.00,
"dataDesconto3":"2022-07-30",
"tipoJuros": "VALOR",
"tipoJurosPercentual": "DIARIO",
"juros": 5.00,
"dataInicioJuros":"2025-11-26",
"tipoMulta": "PERCENTUAL",
"multa": 3.00,
"dataInicioMulta":"2025-11-26",
"informativos": [
"info 1",
"info 2",
"info 3",
"info 4",
"info 5"
],
"mensagens": [
"mens 1",
"mens 2",
"mens 3",
"mens 4"
]
}
```

### RESPONSE BODY (JSON)

```json
{
"txid": "f69d2a0076fb4ea2bddd7babd1200525",
"qrCode": "00020101021226930014br.gov.bcb.pix2571pix-qrcode-
h.sicredi.com.br/qr/v2/cobv/6946459e4b6e4c19ab5c9689fe0df30a520400005303986540599.905802BR592
1OLIVEIRA MULTI MARCAS6008BRASILIA62070503***6304E5E1",
"linhaDigitavel": "74891125110061420512803153351030188640000009990",
"codigoBarras": "74891886400000099901125100614205120315335103",
"cooperativa": "0512",
"posto": "03",
"nossoNumero": "251006142"
}
```

### REQUEST BODY (JSON): BOLETO COM SPLIT

```json
{
"beneficiarioFinal":{
"cep":91250000,
"cidade":"PORTO ALEGRE",
"documento":"25140124068",
"logradouro":"RUA DOUTOR VARGAS NETO 980",
"nome":"FELIPE OLIVEIRA",
"numeroEndereco":119,
"tipoPessoa":"PESSOA_FISICA",
"uf":"RS"
},
"codigoBeneficiario":"94787",
"dataVencimento":"2023-07-30",
"especieDocumento":"DUPLICATA_MERCANTIL_INDICACAO",
"pagador":{
"cep":"91250000",
"cidade":"PORTO ALEGRE",
"documento":"02738306004",
"nome":"RODRIGO OLIVEIRA",
"tipoPessoa":"PESSOA_FISICA",
"endereco":"RUA DOUTOR VARGAS 150",
"uf":"RS"
},
"tipoCobranca": "NORMAL",
"nossoNumero": 600046210,
"seuNumero":"TESTE",
"valor":50.00,
"tipoDesconto": "VALOR",
"valorDesconto1":10.00,
"dataDesconto1":"2023-07-15",
"valorDesconto2":7.00,
"dataDesconto2":"2023-07-20",
"valorDesconto3":3.00,
"dataDesconto3":"2023-07-30",
"tipoJuros": "VALOR",
"tipoJurosPercentual": "DIARIO",
"juros": 5.00,
"dataInicioJuros":"2025-11-26",
"tipoMulta": "PERCENTUAL",
"multa": 3.00,
"dataInicioMulta":"2025-11-26",
"informativos": [
"info 1",
"info 2",
"info 3",
"info 4",
"info 5"
],
"mensagens": [
"mens 1",
"mens 2",
"mens 3",
"mens 4"
],
"splitBoleto": {
"repasseAutomaticoSplit": "SIM",
"tipoValorRateio": "PERCENTUAL",
"regraRateio": "VALOR_COBRADO",
"destinatarios":[
{
"codigoBanco": "237",
"codigoAgencia": "0434",
"numeroContaCorrente": "2323232323",
"numeroCpfCnpj": "02738306004",
"nomeDestinatario": "DECIO OLIVEIRA",
"parcelaRateio": "1",
"valorPercentualRateio": 24.22,
"floatSplit": 20
}
]
}
}
```

### Execução (via ferramenta Postman)

Cenário Positivo:

Cenário de erro <BAD_REQUEST>

### 7.3. Impressão de Boletos

A operação GET “Geração de Boleto em Formato PDF” é responsável pela impressão e reimpressão (2ª via) dos boletos de Cobrança em formato PDF. O formato de entrada é sempre JSON. Já o formato de saída será arquivo binário (octet-stream), em caso de sucesso, e JSON, em caso de falha.

### Entrada

Tipo de

### Saída: Boleto em formato PDF, no formato binário

### RETORNO (Response)

### FALHAS

### REQUEST – GET

### URL Sandbox (ambiente para homologação)

```text
Ord       Nome             Tipo                   Tamanho        Formatação             Descrição      Obrigatório
                                   Parâmetro

1     x-api-key       String      HeaderParam     36         UUID                  Token de acesso     Obrigatório
                                                                                   fornecido pelo
                                                                                   Sicredi

2     authorization String        HeaderParam     1413       Bearer + Token de Acess token obtido Obrigatório
                                                             Autenticação      na autenticação

3     linhaDigitavel String       QueryParam      47         Sem formatação        Código da linha      Obrigatório
                                                                                   digitável do boleto.

Para salvar o boleto, via postman, deverá clicar na opção “Download”

 Status                                                 Descrição
 HTTP_CREATED (201)                                     Operação criada com sucesso

                                                        Parâmetros de
                Status             Mensagem                entrada               Descrição
                                                      sujeitos a crítica
HTTP_UNAUTHORIZED (401)        Could not find a       Parâmetro:         Erros ocasionados por
                               required Access        x-api-key          parâmetros obrigatórios
                               Token in the                              não informados ou
                               request, identified                       inválidos.
                               by HEADER x-api-
                               key
HTTP_UNAUTHORIZED (401)        UNAUTHORIZED           Parâmetro:           Erros ocasionados por
                                                      Authorization        parâmetros obrigatórios
                                                                           não informados ou
                                                                           inválidos.

HTTP_UNAUTHORIZED (401)             Código de              Parâmetro:           Erro ocasionado quando a
                                    beneficiário           linhaDigitavel       linha digitável informada
                                    diferente do                                é de beneficiário e/ou
                                    beneficiário do                             agência diferente.
                                    usuário

HTTP_BAD_REQUEST (400)              A linha digitavel    Parâmetro:             Erros ocasionados por
                                    deve ter 47 dígitos. linhaDigitavel         parâmetros obrigatórios
                                                                                não informados ou
                                                                                inválidos.

 HTTP_UNPROCESSABLE_ENTITY           Boleto não           Parâmetro:            Erro ocasionado quando
(422)                               localizado. Verifique linhaDigitavel        o boleto não é localizado
                                    os dados informados                         através da linha digitável
                                    ou entre em contato                         informada.
                                    com o Beneficiário.

HTTP_TOO_MANY_REQUESTS (429) Too many requests                                  Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um
                                                                                determinado período de
                                                                                tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/pdf`

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/pdf`

HEADERS:

Authorization x-api-key Content-Type: application/json

PARAM: LinhaDigitavel

### Execução (via ferramenta Postman)

Parâmetros:

Headers:

Cenário Positivo:

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.4. Comando de Instrução - Pedido de Baixa

A operação PATCH “Geração de baixa para boletos” é responsável por realizar a instrução de baixa do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord          Nome                  Tipo      Parâmetro                    Formatação        Descrição   Obrigatório
                                                               Tamanho

1     x-api-key               String        HeaderPar      36             UUID         Token de       Obrigatório
                                            am                                         acesso
                                                                                       fornecido pelo
                                                                                       Sicredi
2     authorization           String         Header        1413           Bearer +     Acess token Obrigatório
                                                                          Token de     obtido na
                                                                          Autenticação autenticação
3     Content-Type            application/jso Header                      Sem        Tipo conteúdo Obrigatório
                              n                                           formatação
4     cooperativa             String          Header       4              Sem        Código da      Obrigatório
                                                                          formatação cooperativa do
                                                                                     beneficiário

5    posto                    String            Header        2              Sem        Código da    Obrigatório
                                                                             formatação posto do
                                                                                        beneficiário
6    codigoBeneficiario       String            Header        5              Sem        Código do    Obrigatório
                                                                             formatação beneficiário
7    nossoNumero              String            Path          9              Sem        Nosso Número Obrigatório
                                                                             formatação

Ord      Nome                          Tipo            Tamanho        Formatação Descrição

1        transactionId                 string          36             Sem             Identificador da transação
                                                                      formatação
2        dataMovimento                 String          10             Sem             Data da instrução
                                                                      formatação
3        codigoBeneficiario            String          5              Sem             Código do beneficiário
                                                                      formatação
4        nossoNumero                   String          9              Sem             Nosso Número
                                                                      formatação
5        cooperativa                   String          4              Sem             Código da cooperativa do
                                                                      formatação      beneficiário
6        posto                         String          2              Sem             Código da agência do beneficiário
                                                                      formatação
7        statusComando                 String          17             Sem             Status da operação gerada, que
                                                                      formatação      será:

                                                                                      - MOVIMENTO_ENVIADO
8        dataHoraRegistro              String (date-   26             Sem             Data e hora da instrução
                                       time)                          formatação
9        tipoMensagem                  String          5              Sem             Tipo de instrução, que será:
                                                                      formatação
                                                                                      BAIXA

    Status                                                             Descrição
    HTTP_ACCEPTED (202)                                                Operação recebida com sucesso

                                                     Parâmetros de
               Status             Mensagem              entrada                   Descrição
                                                    sujeitos a crítica
HTTP_UNAUTHORIZED (401)       Could not find a     Parâmetro:          Erros ocasionados por
                              required Access      x-api-key           parâmetros obrigatórios não
                              Token in the                             informados ou inválidos.
                              request, identified
                              by HEADER x-api-
                              key
HTTP_UNAUTHORIZED (401)       UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                  Authorization           parâmetro não foi informado
                                                                          ou está inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:         Erro ocasionado quando o
                              beneficiário         codigoBeneficiario código de beneficiário
                              diferente do                            informado é diferente do
                              beneficiário do                         beneficiário contido no token
                              usuário                                 de autenticação
                                                                      (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:         Erro ocasionado quando a
                              diferente da         Cooperativa        cooperativa informada é
                              cooperativa do                          diferente da cooperativa
                              usuário                                 contida no token de
                                                                      autenticação (authorization).

HTTP_BAD_REQUEST (400)        Título não           Parâmetro:             Erro ocasionado quando o
                              encontrado           nossoNumero            nosso número informado não
                                                                          foi encontrado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está aguardando
                              aguardando                                  confirmação de entrada.
                              confirmação.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está rejeitado.
                              rejeitado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está baixado.
                              baixado.

 HTTP_UNPROCESSABLE_ENTITY           Operação não        Não se aplica           Erro ocasionado quando o
(422)                               permitida: Título já                         título está liquidado.
                                    liquidado.
 HTTP_UNPROCESSABLE_ENTITY           Operação não        Não se aplica           Erro ocasionado quando o
(422)                               permitida: Título                            título está em fluxo de
                                    em fluxo de                                  negativação/protesto.
                                    negativação ou
                                    protesto.
 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação     Não se aplica           Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova tentativa
                                    processamento,                               de instrução para um boleto
                                    aguarde alguns                               que está com a solicitação
                                    instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/baixa`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/baixa`

HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PATCH: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.5. Comando de Instrução - Alteração de Vencimento

Vencimento A operação PATCH “Alteração de vencimento” é responsável por realizar a instrução de alteração de vencimento do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho Obrigat

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo                                Formatação           Descrição
                                           Parâme                                                          ório
                                             tro
1         x-api-key             String     Header       36          UUID              Token de       Obrigat
                                           Param                                      acesso         ório
                                                                                      fornecido pelo
                                                                                      Sicredi
2         authorization         String      Header      1413        Bearer + Token de Acess token Obrigat
                                                                    Autenticação      obtido na      ório
                                                                                      autenticação
3         Content-Type          application/ Header                 Sem formatação        Tipo conteúdo Obrigat
                                json                                                                     ório
4         cooperativa           String       Header     4           Sem formatação        Código da      Obrigat
                                                                                          cooperativa do ório
                                                                                          beneficiário

5         Posto                 String       Header       2          Sem formatação        Código da     Obrigat
                                                                                           agência do    ório
                                                                                           beneficiário
6         codigoBeneficiario String          Header       5          Sem formatação        Código do     Obrigat
                                                                                           beneficiário ório
7         dataVencimento        String       Body         10         YYYY-MM-DD            Data de       Obrigat
                                (date)                                                     vencimento do ório
                                                                                           boleto
8         nossoNumero           String       Patch        9          Sem formatação        Nosso Número Obrigat
                                                                                                        ório

Ord        Nome                     Tipo            Tam           Formatação Descrição
1          transactionId            string          36            Sem             Identificador da transação
                                                                  formatação
2          dataMovimento            String        10              Sem             Data da instrução
                                                                  formatação

3          codigoBeneficiario       String          5             Sem             Código do beneficiário
                                                                  formatação

4          nossoNumero              String          9             Sem             Nosso Número
                                                                  formatação

5          cooperativa              String          4             Sem             Código da cooperativa do
                                                                  formatação      beneficiário

6          posto                    String          2             Sem             Código da agência do
                                                                  formatação      beneficiário

7          statusComando            String          17            Sem             Status da operação gerada, que
                                                                  formatação      será:

                                                                                  - MOVIMENTO_ENVIADO
8          dataHoraRegistro         String          26            Sem             Data e hora da instrução
                                    (date-time)                   formatação

9         tipoMensagem          String        17              Sem             Tipo de instrução, que será:
                                                              formatação
                                                                              ALTERA_VENCIMENTO

 Status                                                        Descrição
 HTTP_ACCEPTED (202)                                           Operação recebida com sucesso

    Importante: Quando os beneficiários alterarem a data de vencimento dos boletos que possuam campos
    de carência para início de juros e/ou multa, o sistema atualizará tanto a data de vencimento quanto as
    datas de carência, preservando a mesma diferença de dias entre elas.
    Exemplo:
    Se a data de vencimento original for 10/01/2025 e a data de início da cobrança de juros for 20/01/2025,
    ao alterar o vencimento para 15/01/2025, a data de início da cobrança de juros será ajustada para
    25/01/2025, mantendo o intervalo de 10 dias.

                                                             Parâmetros de
                Status                   Mensagem               entrada                 Descrição
                                                            sujeitos a crítica
HTTP_UNAUTHORIZED (401)              Could not find a      Parâmetro:          Erros ocasionado por
                                     required Access       x-api-key           parâmetro obrigatório não
                                     Token in the                              informado ou inválido.
                                     request, identified
                                     by HEADER x-api-
                                     key
HTTP_UNAUTHORIZED (401)              UNAUTHORIZED Parâmetro:                      Erro ocasionado por
                                                         Authorization            parâmetro obrigatório não
                                                                                  foi informado ou inválido.

HTTP_UNAUTHORIZED (401)              Código de             Parâmetro:         Erro ocasionado quando o
                                     beneficiário          codigoBeneficiario código de beneficiário
                                     diferente do                             informado é diferente do
                                     beneficiário do                          beneficiário contido no
                                     usuário                                  token de autenticação
                                                                              (authorization).

HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:     Erro ocasionado quando a
                              diferente da         Cooperativa    cooperativa informada é
                              cooperativa do                      diferente da cooperativa
                              usuário                             contida no token de
                                                                  autenticação
                                                                  (authorization).
HTTP_BAD_REQUEST (400)        A data de            Parâmetro:     Erro ocasionado quando a
                              vencimento do        dataVencimento data de vencimento não é
                              boleto é                            informada ou inválida.
                              obrigatória e deve
                              ser preenchida.
HTTP_BAD_REQUEST (400)        Título não           Parâmetro:             Erro ocasionado quando o
                              encontrado           nossoNumero            nosso número informado
                                                                          não foi encontrado.

 HTTP_UNPROCESSABLE_ENTITY    Operação não        Parâmetro:     Erro ocasionado quando a
(422)                         permitida:          dataVencimento data de vencimento
                              Vencimento não                     informada é retroativa.
                              deve ser inferior a
                              DD/MM/YYYY.

 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está aguardando
                              aguardando                                  confirmação de entrada.
                              confirmação.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está rejeitado.
                              rejeitado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está baixado.
                              baixado.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está liquidado.
                              liquidado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está em fluxo de
                              em fluxo de                                 negativação/protesto.
                              negativação ou
                              protesto.

 HTTP_UNPROCESSABLE_ENTITY             Sua solicitação     Não se aplica          Erro ocasionado quando é
(422)                                 anterior está em                            realizada uma nova
                                      processamento,                              tentativa de instrução para
                                      aguarde alguns                              um boleto que está com a
                                      instantes.                                  solicitação anterior em
                                                                                  processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                      Erro ocasionado quando
                                                                                  são enviadas muitas
                                                                                  requisições em um curto
                                                                                  espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/data-vencimento`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/data-vencimento`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{“dataVencimento”: “XXXX-MM-DD”}
```

PATCH:

nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.6. Comando de Instrução - Alteração de Desconto

Desconto A operação PATCH “Alteração de desconto” é responsável por realizar a instrução de alteração do valor/percentual de desconto do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho Obrigat

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo                                Formatação           Descrição
                                           Parâme                                                          ório
                                             tro
1         x-api-key             String     Header       36          UUID              Token de       Obrigat
                                           Param                                      acesso         ório
                                                                                      fornecido pelo
                                                                                      Sicredi
2         authorization         String      Header      1413        Bearer + Token de Acess token Obrigat
                                                                    Autenticação      obtido na      ório
                                                                                      autenticação
3         Content-Type          application/ Header                 Sem formatação        Tipo conteúdo Obrigat
                                json                                                                    ório

4         cooperativa           String       Header       4          Sem formatação        Código da      Obrigat
                                                                                           cooperativa do ório
                                                                                           beneficiário
5         Posto                 String       Header       2          Sem formatação        Código da      Obrigat
                                                                                           agência do     ório
                                                                                           beneficiário
6         codigoBeneficiario String          Header       5          Sem formatação        Código do      Obrigat
                                                                                           beneficiário ório
7         nossoNumero           String       Patch        9          Sem formatação        Nosso Número Obrigat
                                                                                                          ório

8         valorDesconto1        Number       Body         14,2       Sem formatação         Valor do       Opcion
                                                                                           desconto 1.     al
                                                                                           Deverá ser
                                                                                           informado ao
                                                                                           menos um dos
                                                                                           três descontos.
9         valorDesconto2        Number       Body         14,2       Sem formatação         Valor do       Opcion
                                                                                           desconto 2.     al
                                                                                           Deverá ser
                                                                                           informado ao
                                                                                           menos um dos
                                                                                           três descontos.
10        valorDesconto3        Number       Body         14,2       Sem formatação         Valor do       Opcion
                                                                                           desconto 3.     al
                                                                                           Deverá ser
                                                                                           informado ao
                                                                                           menos um dos
                                                                                           três descontos.

Ord        Nome                     Tipo            Tam           Formatação Descrição
1          transactionId            string          36            Sem             Identificador da transação
                                                                  formatação
2          dataMovimento            String        10              Sem             Data da instrução
                                                                  formatação

3          codigoBeneficiario       String          5             Sem             Código do beneficiário
                                                                  formatação

4          nossoNumero              String          9             Sem             Nosso Número
                                                                  formatação

5         cooperativa          String        4               Sem             Código da cooperativa do
                                                             formatação      beneficiário

6         posto                String        2               Sem             Código da agência do
                                                             formatação      beneficiário

7         statusComando        String        17              Sem             Status da operação gerada, que
                                                             formatação      será:

                                                                             - MOVIMENTO_ENVIADO
8         dataHoraRegistro     String        26              Sem             Data e hora da instrução
                               (date-time)                   formatação

9         tipoMensagem         String        21              Sem             Tipo de instrução, que será:
                                                             formatação
                                                                             ALTERA_VALOR_DESCONTO

 Status                                                       Descrição
 HTTP_ACCEPTED (202)                                          Operação recebida com sucesso

                                                            Parâmetros de
                Status                  Mensagem               entrada                 Descrição
                                                           sujeitos a crítica
HTTP_UNAUTHORIZED (401)             Could not find a      Parâmetro:          Erros ocasionado por
                                    required Access       x-api-key           parâmetro obrigatório não
                                    Token in the                              informado ou inválido.
                                    request, identified
                                    by HEADER x-api-
                                    key
HTTP_UNAUTHORIZED (401)             UNAUTHORIZED Parâmetro:                      Erro ocasionado por
                                                        Authorization            parâmetro obrigatório não
                                                                                 foi informado ou inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:         Erro ocasionado quando o
                              beneficiário         codigoBeneficiario código de beneficiário
                              diferente do                            informado é diferente do
                              beneficiário do                         beneficiário contido no
                              usuário                                 token de autenticação
                                                                      (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:         Erro ocasionado quando a
                              diferente da         Cooperativa        cooperativa informada é
                              cooperativa do                          diferente da cooperativa
                              usuário                                 contida no token de
                                                                      autenticação
                                                                      (authorization).
HTTP_BAD_REQUEST (400)        Título não           Parâmetro:         Erro ocasionado quando o
                              encontrado           nossoNumero        nosso número informado
                                                                      não foi encontrado.

HTTP_BAD_REQUEST (400)         O valor do          Parâmetro:             Erro ocasionado quando o
                              primeiro desconto valorDesconto1            valor do primeiro desconto
                              do boleto deve ser                          está zerado (0.00).
                              superior a zero, ou
                              não informado.
HTTP_BAD_REQUEST (400)         O valor do          valorDesconto2         Erro ocasionado quando o
                              segundo desconto                            valor do segundo desconto
                              do boleto deve ser                          está zerado (0.00).
                              superior a zero, ou
                              não informado.
HTTP_BAD_REQUEST (400)         O valor do terceiro valorDesconto3         Erro ocasionado quando o
                              desconto do boleto                          valor do terceiro desconto
                              deve ser superior a                         está zerado (0.00).
                              zero, ou não
                              informado.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando
(422)                         permitida:                                  não é informado pelo
                              Necessário                                  menos um dos três
                              informar pelo                               descontos.
                              menos um dos
                              descontos.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está aguardando
                              aguardando                                  confirmação de entrada.
                              confirmação.

 HTTP_UNPROCESSABLE_ENTITY         Operação não          Não se aplica          Erro ocasionado quando o
(422)                              permitida: Título                            título está rejeitado.
                                   rejeitado.
 HTTP_UNPROCESSABLE_ENTITY         Operação não         Não se aplica           Erro ocasionado quando o
(422)                              permitida: Título já                         título está baixado.
                                   baixado.
 HTTP_UNPROCESSABLE_ENTITY          Operação não        Não se aplica           Erro ocasionado quando o
(422)                              permitida: Título já                         título está liquidado.
                                   liquidado.
 HTTP_UNPROCESSABLE_ENTITY         Operação não          Não se aplica          Erro ocasionado quando o
(422)                              permitida: Título                            título está em fluxo de
                                   em fluxo de                                  negativação/protesto.
                                   negativação ou
                                   protesto.

 HTTP_UNPROCESSABLE_ENTITY          Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                              anterior está em                             realizada uma nova
                                   processamento,                               tentativa de instrução para
                                   aguarde alguns                               um boleto que está com a
                                   instantes.                                   solicitação anterior em
                                                                                processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                    Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um curto
                                                                                espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/desconto`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/desconto`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{
"valorDesconto1": 0.00,
"valorDesconto2": 0.00,
"valorDesconto3": 0.00
}
```

PATCH:

nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.7. Comando de Instrução - Alteração de Data de Desconto

de Desconto A operação PATCH “Alteração de data de desconto” é responsável por realizar a instrução de alteração da data de desconto do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho Obrigat

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo                                 Formatação           Descrição
                                            Parâme                                                          ório
                                              tro
1         x-api-key             String      Header       36          UUID              Token de       Obrigat
                                            Param                                      acesso         ório
                                                                                       fornecido pelo
                                                                                       Sicredi
2         authorization         String      Header       1413        Bearer + Token de Acess token Obrigat
                                                                     Autenticação      obtido na      ório
                                                                                       autenticação
3         Content-Type          application/ Header                  Sem formatação        Tipo conteúdo Obrigat
                                json                                                                      ório
4         cooperativa           String       Header      4           Sem formatação        Código da      Obrigat
                                                                                           cooperativa do ório
                                                                                           beneficiário
5         Posto                 String      Header       2           Sem formatação        Código da      Obrigat
                                                                                           agência do     ório
                                                                                           beneficiário
6         codigoBeneficiario String         Header       5           Sem formatação        Código do      Obrigat
                                                                                           beneficiário ório
7         nossoNumero           String      Patch        9           Sem formatação        Nosso Número Obrigat
                                                                                                          ório

8         data1                 String (date) Body       10          YYYY-MM-DD            Data do         Opcion
                                                                                           desconto 1.     al
                                                                                           Deverá ser

                                                                                           informado ao
                                                                                           menos uma
                                                                                           das três datas.
9         data2                 String (date) Body         10        YYYY-MM-DD             Data do        Opcion
                                                                                           desconto 2.     al
                                                                                           Deverá ser
                                                                                           informado ao
                                                                                           menos uma
                                                                                           das três datas.
10        data3                 String (date) Body         10        YYYY-MM-DD             Data do        Opcion
                                                                                           desconto 3.     al
                                                                                           Deverá ser
                                                                                           informado ao
                                                                                           menos uma
                                                                                           das três datas.

Ord        Nome                     Tipo             Tam          Formatação Descrição
1          transactionId            string           36           Sem             Identificador da transação
                                                                  formatação
2          dataMovimento            String        10              Sem             Data da instrução
                                                                  formatação

3          codigoBeneficiario       String           5            Sem             Código do beneficiário
                                                                  formatação

4          nossoNumero              String           9            Sem             Nosso Número
                                                                  formatação

5          cooperativa              String           4            Sem             Código da cooperativa do
                                                                  formatação      beneficiário

6          posto                    String           2            Sem             Código da agência do
                                                                  formatação      beneficiário

7          statusComando            String           17           Sem             Status da operação gerada, que
                                                                  formatação      será:

                                                                                  - MOVIMENTO_ENVIADO
8          dataHoraRegistro         String           26           Sem             Data e hora da instrução
                                    (date-time)                   formatação

9         tipoMensagem         String        20              Sem             Tipo de instrução, que será:
                                                             formatação
                                                                             ALTERA_DATA_DESCONTO

 Status                                                       Descrição
 HTTP_ACCEPTED (202)                                          Operação recebida com sucesso

                                                            Parâmetros de
                Status                  Mensagem               entrada                 Descrição
                                                           sujeitos a crítica
HTTP_UNAUTHORIZED (401)             Could not find a      Parâmetro:          Erros ocasionado por
                                    required Access       x-api-key           parâmetro obrigatório não
                                    Token in the                              informado ou inválido.
                                    request, identified
                                    by HEADER x-api-
                                    key
HTTP_UNAUTHORIZED (401)             UNAUTHORIZED Parâmetro:                      Erro ocasionado por
                                                        Authorization            parâmetro obrigatório não
                                                                                 foi informado ou inválido.

HTTP_UNAUTHORIZED (401)             Código de             Parâmetro:         Erro ocasionado quando o
                                    beneficiário          codigoBeneficiario código de beneficiário
                                    diferente do                             informado é diferente do
                                    beneficiário do                          beneficiário contido no
                                    usuário                                  token de autenticação
                                                                             (authorization).

HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:             Erro ocasionado quando a
                              diferente da         Cooperativa            cooperativa informada é
                              cooperativa do                              diferente da cooperativa
                              usuário                                     contida no token de
                                                                          autenticação
                                                                          (authorization).
HTTP_BAD_REQUEST (400)        Título não           Parâmetro:             Erro ocasionado quando o
                              encontrado           nossoNumero            nosso número informado
                                                                          não foi encontrado.

 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando
(422)                         permitida:                                  não é informado ao menos
                              Necessário                                  uma das três datas de
                              informar pelo                               desconto.
                              menos um dos
                              descontos.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Parâmetro:             Erro ocasionado quando a
(422)                         permitida: A data data2                     data de desconto 2 é
                              de desconto 2 deve                          anterior à data de desconto
                              ser posterior a data                        1.
                              de desconto 1.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Parâmetro:             Erro ocasionado quando a
(422)                         permitida: A data data3                     data de desconto 3 é
                              de desconto 3 deve                          anterior à data de desconto
                              ser posterior a data                        2.
                              de desconto 2.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está aguardando
                              aguardando                                  confirmação de entrada.
                              confirmação.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está rejeitado.
                              rejeitado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está baixado.
                              baixado.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está liquidado.
                              liquidado.

 HTTP_UNPROCESSABLE_ENTITY          Operação não         Não se aplica          Erro ocasionado quando o
(422)                               permitida: Título                           título está em fluxo de
                                    em fluxo de                                 negativação/protesto.
                                    negativação ou
                                    protesto.

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação     Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                            realizada uma nova
                                    processamento,                              tentativa de instrução para
                                    aguarde alguns                              um boleto que está com a
                                    instantes.                                  solicitação anterior em
                                                                                processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                    Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um curto
                                                                                espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/data-desconto`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/data-desconto`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{
"data1": "YYYY-MM-DD",
"data2": "YYYY-MM-DD",
"data3": "YYYY-MM-DD"
}
```

PATCH:

nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.8. Comando de Instrução - Alteração de Juros

Juros A operação PATCH “Alteração de juros” é responsável por realizar a instrução de alteração do valor/percentual de juros do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho Obrigat

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo                                 Formatação           Descrição
                                             Parâme                                                         ório
                                               tro
1         x-api-key             String       Header       36         UUID              Token de       Obrigat
                                             Param                                     acesso         ório
                                                                                       fornecido pelo
                                                                                       Sicredi
2         authorization         String       Header       1413       Bearer + Token de Acess token Obrigat
                                                                     Autenticação      obtido na      ório
                                                                                       autenticação
3         Content-Type          application/ Header                  Sem formatação        Tipo conteúdo Obrigat
                                json                                                                      ório
4         cooperativa           String       Header       4          Sem formatação        Código da      Obrigat
                                                                                           cooperativa do ório
                                                                                           beneficiário
5         Posto                 String       Header       2          Sem formatação        Código da      Obrigat
                                                                                           agência do     ório
                                                                                           beneficiário
6         codigoBeneficiario String          Header       5          Sem formatação        Código do      Obrigat
                                                                                           beneficiário ório
7         nossoNumero           String       Patch        9          Sem formatação        Nosso Número Obrigat
                                                                                                          ório

8         valorOuPercentual Number           Body         14,2       Sem formatação         Valor ou       Obrigat
                                                                                           percentual de ório
                                                                                           juros do boleto

Ord        Nome                     Tipo            Tam           Formatação Descrição
1          transactionId            string          36            Sem             Identificador da transação
                                                                  formatação
2          dataMovimento            String        10              Sem             Data da instrução
                                                                  formatação

3          codigoBeneficiario       String          5             Sem             Código do beneficiário
                                                                  formatação

4          nossoNumero              String          9             Sem             Nosso Número
                                                                  formatação

5         cooperativa           String        4               Sem             Código da cooperativa do
                                                              formatação      beneficiário

6         posto                 String        2               Sem             Código da agência do
                                                              formatação      beneficiário

7         statusComando         String        17              Sem             Status da operação gerada, que
                                                              formatação      será:

                                                                              - MOVIMENTO_ENVIADO
8         dataHoraRegistro      String        26              Sem             Data e hora da instrução
                                (date-time)                   formatação

9         tipoMensagem          String        12              Sem             Tipo de instrução, que será:
                                                              formatação
                                                                              ALTERA_JUROS

 Status                                                        Descrição
 HTTP_ACCEPTED (202)                                           Operação recebida com sucesso

    Importante:
    Apenas o valor dos juros poderá ser alterado; o tipo de juro permanecerá inalterado.

                                                             Parâmetros de
                Status                   Mensagem               entrada                 Descrição
                                                            sujeitos a crítica
HTTP_UNAUTHORIZED (401)              Could not find a      Parâmetro:          Erros ocasionado por
                                     required Access       x-api-key           parâmetro obrigatório não
                                     Token in the                              informado ou inválido.
                                     request, identified
                                     by HEADER x-api-
                                     key

HTTP_UNAUTHORIZED (401)       UNAUTHORIZED         Parâmetro:             Erro ocasionado por
                                                   Authorization          parâmetro obrigatório não
                                                                          foi informado ou inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:         Erro ocasionado quando o
                              beneficiário         codigoBeneficiario código de beneficiário
                              diferente do                            informado é diferente do
                              beneficiário do                         beneficiário contido no
                              usuário                                 token de autenticação
                                                                      (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:         Erro ocasionado quando a
                              diferente da         Cooperativa        cooperativa informada é
                              cooperativa do                          diferente da cooperativa
                              usuário                                 contida no token de
                                                                      autenticação
                                                                      (authorization).
HTTP_BAD_REQUEST (400)        Título não           Parâmetro:         Erro ocasionado quando o
                              encontrado           nossoNumero        nosso número informado
                                                                      não foi encontrado.

HTTP_BAD_REQUEST (400)        O valor ou         Parâmetro:        Erro ocasionado quando o
                              percentual é       valorOuPercentual parâmetro não foi
                              obrigatório e deve                   informado.
                              ser preenchido.
 HTTP_UNPROCESSABLE_ENTITY    Operação não       Não se aplica     Erro ocasionado quando o
(422)                         permitida: Título                    título está aguardando
                              aguardando                           confirmação de entrada.
                              confirmação.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título                           título está rejeitado.
                              rejeitado.
 HTTP_UNPROCESSABLE_ENTITY    Operação não         Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está baixado.
                              baixado.
 HTTP_UNPROCESSABLE_ENTITY     Operação não        Não se aplica          Erro ocasionado quando o
(422)                         permitida: Título já                        título está liquidado.
                              liquidado.

 HTTP_UNPROCESSABLE_ENTITY          Operação não         Não se aplica          Erro ocasionado quando o
(422)                               permitida: Título                           título está em fluxo de
                                    em fluxo de                                 negativação/protesto.
                                    negativação ou
                                    protesto.

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação     Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                            realizada uma nova
                                    processamento,                              tentativa de instrução para
                                    aguarde alguns                              um boleto que está com a
                                    instantes.                                  solicitação anterior em
                                                                                processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                    Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um curto
                                                                                espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/juros`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/juros`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto

codigoBeneficiario

BODY:

```json
{
"valorOuPercentual": "0.00"
}
```

PATCH:

nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.9. Comando de Instrução - Alteração de Seu Número

Número A operação PATCH “Alteração de seu número” é responsável por realizar a instrução de alteração do Seu Número do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo                                Formatação           Descrição     Obrigatório
                                           Parâme
                                             tro
1         x-api-key             String     Header       36          UUID              Token de       Obrigatório
                                           Param                                      acesso
                                                                                      fornecido pelo
                                                                                      Sicredi
2         authorization         String      Header      1413        Bearer + Token de Acess token Obrigatório
                                                                    Autenticação      obtido na
                                                                                      autenticação
3         Content-Type          application/ Header                 Sem formatação        Tipo conteúdo Obrigatório
                                json
4         cooperativa           String       Header     4           Sem formatação        Código da      Obrigatório
                                                                                          cooperativa do
                                                                                          beneficiário
5         Posto                 String      Header      2           Sem formatação        Código do      Obrigatório
                                                                                          posto do
                                                                                          beneficiário

6         codigoBeneficiario String          Header       5          Sem formatação        Código do    Obrigatório
                                                                                           beneficiário
7         nossoNumero           String       Path         9          Sem formatação        Nosso Número Obrigatório

8         seuNumero             String       Body         10         Sem formatação        Seu número Obrigatório
                                                                                           do boleto.
                                                                                           Normalmente
                                                                                           usado neste
                                                                                           campo o
                                                                                           número da
                                                                                           nota fiscal
                                                                                           gerada para o
                                                                                           pagador.

Ord        Nome                     Tipo            Tam           Formatação Descrição
1          transactionId            string          36            Sem             Identificador da transação
                                                                  formatação
2          dataMovimento            String          10            Sem             Data da instrução
                                                                  formatação

3          codigoBeneficiario       String          5             Sem             Código do beneficiário
                                                                  formatação

4          nossoNumero              String          9             Sem             Nosso Número
                                                                  formatação

5          cooperativa              String          4             Sem             Código da cooperativa do
                                                                  formatação      beneficiário

6          posto                    String          2             Sem             Código da agência do
                                                                  formatação      beneficiário

7          statusComando            String          17            Sem             Status da operação gerada, que
                                                                  formatação      será:

                                                                                  - MOVIMENTO_ENVIADO
8          dataHoraRegistro         String          26            Sem             Data e hora da instrução
                                    (date-time)                   formatação

9         tipoMensagem         String        16              Sem             Tipo de instrução, que será:
                                                             formatação
                                                                             ALTERA_SEUNUMERO

 Status                                                       Descrição
 HTTP_ACCEPTED (202)                                          Operação recebida com sucesso

                                                            Parâmetros de
                Status                  Mensagem               entrada                 Descrição
                                                           sujeitos a crítica
HTTP_UNAUTHORIZED (401)             Could not find a      Parâmetro:          Erros ocasionado por
                                    required Access       x-api-key           parâmetro obrigatório não
                                    Token in the                              informado ou inválido.
                                    request, identified
                                    by HEADER x-api-
                                    key
HTTP_UNAUTHORIZED (401)             UNAUTHORIZED Parâmetro:                      Erro ocasionado por
                                                        Authorization            parâmetro obrigatório não
                                                                                 foi informado ou inválido.

HTTP_UNAUTHORIZED (401)             Código de             Parâmetro:         Erro ocasionado quando o
                                    beneficiário          codigoBeneficiario código de beneficiário
                                    diferente do                             informado é diferente do
                                    beneficiário do                          beneficiário contido no
                                    usuário                                  token de autenticação
                                                                             (authorization).
HTTP_UNAUTHORIZED (401)             Cooperativa           Parâmetro:         Erro ocasionado quando a
                                    diferente da          Cooperativa        cooperativa informada é
                                    cooperativa do                           diferente da cooperativa
                                    usuário                                  contida no token de
                                                                             autenticação
                                                                             (authorization).
HTTP_BAD_REQUEST (400)              Título não            Parâmetro:         Erro ocasionado quando o
                                    encontrado            nossoNumero        nosso número informado
                                                                             não foi encontrado.

HTTP_BAD_REQUEST (400)           O campo seu       Parâmetro:                 Erro ocasionado quando o
                                 numero deve ter seuNumero                    parâmetro foi preenchido
                                 entre 1 (um) e 10                            com mais de 10 caracteres
                                 (dez) caracteres.                            ou não foi informado.
 HTTP_UNPROCESSABLE_ENTITY       Operação não      Não se aplica              Erro ocasionado quando o
(422)                            permitida: Título                            título está aguardando
                                 aguardando                                   confirmação de entrada.
                                 confirmação.
 HTTP_UNPROCESSABLE_ENTITY       Operação não          Não se aplica          Erro ocasionado quando o
(422)                            permitida: Título                            título está rejeitado.
                                 rejeitado.
 HTTP_UNPROCESSABLE_ENTITY       Operação não         Não se aplica           Erro ocasionado quando o
(422)                            permitida: Título já                         título está baixado.
                                 baixado.
 HTTP_UNPROCESSABLE_ENTITY        Operação não        Não se aplica           Erro ocasionado quando o
(422)                            permitida: Título já                         título está liquidado.
                                 liquidado.
 HTTP_UNPROCESSABLE_ENTITY       Operação não          Não se aplica          Erro ocasionado quando o
(422)                            permitida: Título                            título está em fluxo de
                                 em fluxo de                                  negativação/protesto.
                                 negativação ou
                                 protesto.

 HTTP_UNPROCESSABLE_ENTITY        Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                            anterior está em                             realizada uma nova
                                 processamento,                               tentativa de instrução para
                                 aguarde alguns                               um boleto que está com a
                                 instantes.                                   solicitação anterior em
                                                                              processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                  Erro ocasionado quando
                                                                              são enviadas muitas
                                                                              requisições em um curto
                                                                              espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/seu-numero`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/seu-numero`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{
"seuNumero": "0123456789"
}
```

PATCH:

nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.10. Comando de Instrução – Conceder Abatimento

Abatimento A operação PATCH “Conceder Abatimento” é responsável por realizar a instrução responsável por conceder um valor de abatimento para um boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord         Nome                 Tipo   Parâmetro                    Formatação        Descrição     Obrigatório
                                                        Tamanho

1       x-api-key               String            HeaderPar     36             UUID              Token de acesso Obrigatório
                                                  am                                             fornecido pelo
                                                                                                 Sicredi

2       authorization           String            Header        1413           Bearer +     Acess token            Obrigatório
                                                                               Token de     obtido na
                                                                               Autenticação autenticação
3       Content-Type            application/jso Header                         Sem        Tipo conteúdo Obrigatório
                                n                                              formatação
4       cooperativa             String          Header          4              Sem        Código da       Obrigatório
                                                                               formatação cooperativa do
                                                                                          beneficiário
5       posto                   String            Header        2              Sem        Código do posto Obrigatório
                                                                               formatação do beneficiário

6       codigoBeneficiario      String            Header        5              Sem               Código do         Obrigatório
                                                                               formatação        beneficiário
7       nossoNumero             String            Path          9              Sem               Nosso Número      Obrigatório
                                                                               formatação
8       valorAbatimento         String            Body          14,2           Sem               Valor (Real) de Obrigatório
                                                                               formatação        abatimento a ser
                                                                                                 concedido no
                                                                                                 boleto.

    Ord         Nome                     Tipo            Tam            Formatação Descrição
    1           transactionId            string          36             Sem             Identificador da transação
                                                                        formatação

    2           dataMovimento            String          10             Sem             Data da instrução
                                                                        formatação

    3           codigoBeneficiario       String          5              Sem             Código do beneficiário
                                                                        formatação

    4           nossoNumero              String          9              Sem             Nosso Número
                                                                        formatação

5         cooperativa          String        4               Sem             Código da cooperativa do
                                                             formatação      beneficiário

6         posto                String        2               Sem             Código da agência do
                                                             formatação      beneficiário

7         statusComando        String        17              Sem             Status da operação gerada, que
                                                             formatação      será:

                                                                             - MOVIMENTO_ENVIADO
8         dataHoraRegistro     String        26              Sem             Data e hora da instrução
                               (date-time)                   formatação

9         tipoMensagem         String        16              Sem             Tipo de instrução, que será:
                                                             formatação
                                                                             PEDIDO_ABATIMENTO

 Status                                                       Descrição
 HTTP_ACCEPTED (202)                                          Operação recebida com sucesso

                                                            Parâmetros de
                Status                  Mensagem               entrada                 Descrição
                                                           sujeitos a crítica
HTTP_UNAUTHORIZED (401)             Could not find a      Parâmetro:          Erros ocasionado por
                                    required Access       x-api-key           parâmetro obrigatório não
                                    Token in the                              informado ou inválido.
                                    request, identified
                                    by HEADER x-api-
                                    key

HTTP_UNAUTHORIZED (401)       UNAUTHORIZED          Parâmetro:             Erro ocasionado por
                                                    Authorization          parâmetro obrigatório não
                                                                           foi informado ou inválido.

HTTP_UNAUTHORIZED (401)       Código de             Parâmetro:         Erro ocasionado quando o
                              beneficiário          codigoBeneficiario código de beneficiário
                              diferente do                             informado é diferente do
                              beneficiário do                          beneficiário contido no
                              usuário                                  token de autenticação
                                                                       (authorization).
HTTP_BAD_REQUEST (400)         0136 - Instrução Parâmetro:             Erro ocasionado quando o
                              inválida: Título não nossoNumero         nosso número informado
                              encontrado. (Título                      não foi encontrado.
                              não encontrado).
HTTP_BAD_REQUEST (400)         0082 - Instrução Parâmetro:             Erro ocasionado quando o
                              inválida: título já nossoNumero          título já possui uma
                              possui Instrução de                      instrução de abatimento
                              abatimento                               concedida.
                              concedido.
 HTTP_UNPROCESSABLE_ENTITY     0075 - Instrução Não se aplica          Erro ocasionado quando o
(422)                         inválida: título está                    título está aguardando
                              aguardando                               confirmação de entrada.
                              confirmação.
 HTTP_UNPROCESSABLE_ENTITY     0077 - Instrução     Não se aplica          Erro ocasionado quando o
(422)                         inválida: título já                          título está liquidado.
                              liquidado.
 HTTP_UNPROCESSABLE_ENTITY    0068 - Instrução      Não se aplica          Erro ocasionado quando o
(422)                         inválida: título                             título está em fluxo de
                              enviado para                                 negativação/protesto.
                              Cartório/Serasa.

 HTTP_UNPROCESSABLE_ENTITY    010- Valor de         Não se aplica          Erro ocasionado quando o
(422)                         abatimento                                   valor de abatimento
                              inválido                                     informado é inválido.

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova
                                    processamento,                               tentativa de instrução para
                                    aguarde alguns                               um boleto que está com a
                                    instantes.                                   solicitação anterior em
                                                                                 processamento.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando
                                                                                 são enviadas muitas
                                                                                 requisições em um curto
                                                                                 espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/conceder-`

abatimento

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/conceder-abatimento`

HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{
"valorAbatimento": "12.34"
}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.11. Comando de Instrução – Cancelar Abatimento Concedido

Abatimento Concedido A operação PATCH “Cancelar Abatimento Concedido” é responsável por realizar a instrução de cancelar o abatimento concedido para um boleto que possui abatimento vinculado. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord          Nome                  Tipo      Parâmetro                    Formatação           Descrição      Obrigatório
                                                               Tamanho

1     x-api-key               String        HeaderPar      36             UUID              Token de acesso Obrigatório
                                            am                                              fornecido pelo
                                                                                            Sicredi

2     Authorization           String         Header        1413           Bearer +     Acess token            Obrigatório
                                                                          Token de     obtido na
                                                                          Autenticação autenticação
3     Content-Type            application/jso Header                      Sem        Tipo conteúdo            Obrigatório
                              n                                           formatação
4     cooperativa             String          Header       4              Sem        Código da                Obrigatório
                                                                          formatação cooperativa do
                                                                                     beneficiário

5    posto                    String             Header           2              Sem        Código do posto Obrigatório
                                                                                 formatação do beneficiário

6    codigoBeneficiario       String             Header           5              Sem        Código do              Obrigatório
                                                                                 formatação beneficiário
7    nossoNumero              String             Path             9              Sem        Nosso Número           Obrigatório
                                                                                 formatação

Ord      Nome                          Tipo            Tamanho        Formatação Descrição

1        transactionId                 string          36             Sem             Identificador da transação
                                                                      formatação
2        dataMovimento                 String         10              Sem             Data em que instrução foi realizada
                                                                      formatação
3        codigoBeneficiario            String          5              Sem             Código do beneficiário
                                                                      formatação
4        nossoNumero                   String          9              Sem             Nosso Número
                                                                      formatação
5        cooperativa                   String          4              Sem             Código da cooperativa do beneficiário
                                                                      formatação
6        posto                         String          2              Sem             Código da agência do beneficiário
                                                                      formatação
7        statusComando                 String          17             Sem             Status da operação gerada, que será:
                                                                      formatação
                                                                                      - MOVIMENTO_ENVIADO

8        dataHoraRegistro              String          26             Sem             Data e hora da instrução
                                       (date-time)                    formatação
9        tipoMensagem                  String          5              Sem             Tipo de instrução, que será:
                                                                      formatação
                                                                                      PEDIDO_CANCELAMENTO_ABATIMENTO

       RETORNO (HttpStatus):

    Status                                                                 Descrição
    HTTP_ACCEPTED (202)                                                    Operação recebida com sucesso

                                                     Parâmetros de
               Status             Mensagem              entrada                   Descrição
                                                    sujeitos a crítica
HTTP_UNAUTHORIZED (401)       Could not find a     Parâmetro:          Erros ocasionados por
                              required Access      x-api-key           parâmetros obrigatórios não
                              Token in the                             informados ou inválidos.
                              request, identified
                              by HEADER x-api-
                              key
HTTP_UNAUTHORIZED (401)       UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                  Authorization           parâmetro não foi informado
                                                                          ou está inválido.

HTTP_UNAUTHORIZED (401)       Código de             Parâmetro:         Erro ocasionado quando o
                              beneficiário          codigoBeneficiario código de beneficiário
                              diferente do                             informado é diferente do
                              beneficiário do                          beneficiário contido no token
                              usuário                                  de autenticação
                                                                       (authorization).
HTTP_BAD_REQUEST (422)         “0017 - Título não Parâmetro:           Erro ocasionado quando o
                              cadastrado.”          nossoNumero        nosso número informado não
                                                                       foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY     “0075 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: título está                    título está aguardando
                              aguardando                               confirmação de entrada.
                              confirmação.”
 HTTP_UNPROCESSABLE_ENTITY     "0084 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: não existe                     título não possui abatimento
                              instrução prévia de                      vinculado.
                              Concessão de
                              Abatimento."
 HTTP_UNPROCESSABLE_ENTITY     "0077 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: título já                      título está liquidado.
                              liquidado."

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova tentativa
                                    processamento,                               de instrução para um boleto
                                    aguarde alguns                               que está com a solicitação
                                    instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/cancelar-`

abatimento

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/cancelar-abatimento`

HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.12. Comando de Instrução - Pedido de Protesto

Protesto A operação PATCH “Pedido de Protesto” é responsável por realizar a instrução de pedido de protesto para um boleto vencido. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord           Nome                 Tipo         Parâmetro                    Formatação           Descrição      Obrigatório
                                                                  Tamanho

1     x-api-key               String            HeaderPar     36             UUID              Token de acesso Obrigatório
                                                am                                             fornecido pelo
                                                                                               Sicredi

2     authorization           String            Header        1413           Bearer +     Acess token            Obrigatório
                                                                             Token de     obtido na
                                                                             Autenticação autenticação
3     Content-Type            application/jso Header                         Sem        Tipo conteúdo Obrigatório
                              n                                              formatação
4     cooperativa             String          Header          4              Sem        Código da       Obrigatório
                                                                             formatação cooperativa do
                                                                                        beneficiário
5     posto                   String            Header        2              Sem        Código do posto Obrigatório
                                                                             formatação do beneficiário

6     codigoBeneficiario      String            Header        5              Sem        Código do                Obrigatório
                                                                             formatação beneficiário
7     nossoNumero             String            Path          9              Sem        Nosso Número             Obrigatório
                                                                             formatação

Ord       Nome                         Tipo            Tamanho        Formatação Descrição

1         transactionId                string          36             Sem             Identificador da transação
                                                                      formatação
2         dataMovimento                String          10             Sem             Data em que instrução foi realizada
                                                                      formatação
3         codigoBeneficiario           String          5              Sem             Código do beneficiário
                                                                      formatação
4         nossoNumero                  String          9              Sem             Nosso Número
                                                                      formatação
5         cooperativa                  String          4              Sem             Código da cooperativa do
                                                                      formatação      beneficiário
6         posto                        String          2              Sem             Código da agência do beneficiário
                                                                      formatação

7        statusComando            String           17              Sem             Status da operação gerada, que será:
                                                                   formatação
                                                                                   - MOVIMENTO_ENVIADO

8        dataHoraRegistro         String (date-    26              Sem             Data e hora da instrução
                                  time)                            formatação
9        tipoMensagem             String           5               Sem             Tipo de instrução, que será:
                                                                   formatação
                                                                                   PEDIDO_PROTESTO

       RETORNO (HttpStatus):

    Status                                                          Descrição
    HTTP_ACCEPTED (202)                                             Operação recebida com sucesso

                                                                  Parâmetros de
                   Status                      Mensagem              entrada                   Descrição
                                                                 sujeitos a crítica
HTTP_UNAUTHORIZED (401)                    Could not find a     Parâmetro:          Erros ocasionados por
                                           required Access      x-api-key           parâmetros obrigatórios não
                                           Token in the                             informados ou inválidos.
                                           request, identified
                                           by HEADER x-api-
                                           key
HTTP_UNAUTHORIZED (401)                    UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                               Authorization           parâmetro não foi informado
                                                                                       ou está inválido.

HTTP_UNAUTHORIZED (401)                    Código de            Parâmetro:         Erro ocasionado quando o
                                           beneficiário         codigoBeneficiario código de beneficiário
                                           diferente do                            informado é diferente do
                                           beneficiário do                         beneficiário contido no token
                                           usuário                                 de autenticação
                                                                                   (authorization).

HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:             Erro ocasionado quando a
                              diferente da         Cooperativa            cooperativa informada é
                              cooperativa do                              diferente da cooperativa
                              usuário                                     contida no token de
                                                                          autenticação (authorization).

HTTP_BAD_REQUEST (422)        “0017 - Título não Parâmetro:               Erro ocasionado quando o
                              cadastrado.”       nossoNumero              nosso número informado não
                                                                          foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY     “0075 - Instrução Não se aplica            Erro ocasionado quando o
(422)                         inválida: título está                       título está aguardando
                              aguardando                                  confirmação de entrada.
                              confirmação.”
 HTTP_UNPROCESSABLE_ENTITY     “0085 - Instrução Não se aplica            Erro ocasionado quando o
(422)                         inválida: espécie de                        título possui uma espécie de
                              documento não                               documento que não permite
                              permite Protesto                            pedido de protesto.
                              de Títulos."
 HTTP_UNPROCESSABLE_ENTITY     "0068 - Instrução Não se aplica            Erro ocasionado quando o
(422)                         inválida: título                            título já possui um pedido de
                              enviado para                                protesto em andamento.
                              Cartório."
 HTTP_UNPROCESSABLE_ENTITY     "0078 - Instrução Não se aplica            Erro ocasionado quando o
(422)                         inválida: título já                         título está baixado.
                              baixado."
 HTTP_UNPROCESSABLE_ENTITY     "0077 - Instrução Não se aplica            Erro ocasionado quando o
(422)                         inválida: título já                         título está liquidado.
                              liquidado."

 HTTP_UNPROCESSABLE_ENTITY     "0071 - Instrução Não se aplica            Erro ocasionado quando há
(422)                         inválida: título                            uma tentativa de comando de
                              ainda não venceu."                          protesto para um título ainda
                                                                          não vencido.

 HTTP_UNPROCESSABLE_ENTITY     Sua solicitação     Não se aplica          Erro ocasionado quando é
(422)                         anterior está em                            realizada uma nova tentativa
                              processamento,                              de instrução para um boleto
                              aguarde alguns                              que está com a solicitação
                              instantes.                                  anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/protesto`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/protesto`

HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.13. Comando de Instrução – Sustar Protesto e Baixar Título

e Baixar Título A operação PATCH “Sustar Protesto e Baixar Título” é responsável por realizar a instrução que susta o protesto de um boleto que está em fluxo de cartório e realiza a baixa do boleto. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord           Nome                 Tipo         Parâmetro                    Formatação           Descrição      Obrigatório
                                                                  Tamanho

1     x-api-key               String            HeaderPar     36             UUID              Token de acesso Obrigatório
                                                am                                             fornecido pelo
                                                                                               Sicredi

2     authorization           String            Header        1413           Bearer +     Acess token            Obrigatório
                                                                             Token de     obtido na
                                                                             Autenticação autenticação
3     Content-Type            application/jso Header                         Sem        Tipo conteúdo Obrigatório
                              n                                              formatação
4     cooperativa             String          Header          4              Sem        Código da       Obrigatório
                                                                             formatação cooperativa do
                                                                                        beneficiário
5     posto                   String            Header        2              Sem        Código do posto Obrigatório
                                                                             formatação do beneficiário

6     codigoBeneficiario      String            Header        5              Sem        Código do                Obrigatório
                                                                             formatação beneficiário
7     nossoNumero             String            Path          9              Sem        Nosso Número             Obrigatório
                                                                             formatação

Ord       Nome                         Tipo            Tamanho        Formatação Descrição

1         transactionId                string          36             Sem             Identificador da transação
                                                                      formatação
2         dataMovimento                String          10             Sem             Data em que instrução foi realizada
                                                                      formatação

3        codigoBeneficiario       String           5               Sem             Código do beneficiário
                                                                   formatação
4        nossoNumero              String           9               Sem             Nosso Número
                                                                   formatação
5        cooperativa              String           4               Sem             Código da cooperativa do
                                                                   formatação      beneficiário
6        posto                    String           2               Sem             Código da agência do beneficiário
                                                                   formatação
7        statusComando            String           17              Sem             Status da operação gerada, que será:
                                                                   formatação
                                                                                   - MOVIMENTO_ENVIADO

8        dataHoraRegistro         String (date-    26              Sem             Data e hora da instrução
                                  time)                            formatação
9        tipoMensagem             String           5               Sem             Tipo de instrução, que será:
                                                                   formatação
                                                                                   PEDIDO_SUSTAR_BAIXAR_PROTESTO

       RETORNO (HttpStatus):

    Status                                                          Descrição
    HTTP_ACCEPTED (202)                                             Operação recebida com sucesso

                                                                  Parâmetros de
                   Status                     Mensagem               entrada                   Descrição
                                                                 sujeitos a crítica
HTTP_UNAUTHORIZED (401)                    Could not find a     Parâmetro:          Erros ocasionados por
                                           required Access      x-api-key           parâmetros obrigatórios não
                                           Token in the                             informados ou inválidos.
                                           request, identified
                                           by HEADER x-api-
                                           key
HTTP_UNAUTHORIZED (401)                    UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                               Authorization           parâmetro não foi informado
                                                                                       ou está inválido.

HTTP_UNAUTHORIZED (401)           Código de             Parâmetro:         Erro ocasionado quando o
                                  beneficiário          codigoBeneficiario código de beneficiário
                                  diferente do                             informado é diferente do
                                  beneficiário do                          beneficiário contido no token
                                  usuário                                  de autenticação
                                                                           (authorization).
HTTP_BAD_REQUEST (422)             “0017 - Título não Parâmetro:           Erro ocasionado quando o
                                  cadastrado.”          nossoNumero        nosso número informado não
                                                                           foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY         “0075 - Instrução Não se aplica         Erro ocasionado quando o
(422)                             inválida: título está                    título está aguardando
                                  aguardando                               confirmação de entrada.
                                  confirmação.”
 HTTP_UNPROCESSABLE_ENTITY         “0079 - Instrução Não se aplica         Erro ocasionado quando o
(422)                             inválida: título não                     título não possui nenhuma
                                  foi encaminhado                          instrução de protesto para que
                                  para Cartório.”                          seja comandada a sustação.
 HTTP_UNPROCESSABLE_ENTITY         "0131 - Operação Não se aplica          Erro ocasionado quando o
(422)                             não permitida para                       título está vinculado a uma
                                  o tipo de carteira                       operação de crédito.
                                  Descontada."
 HTTP_UNPROCESSABLE_ENTITY         "0077 - Instrução Não se aplica         Erro ocasionado quando o
(422)                             inválida: título já                      título está liquidado.
                                  liquidado."

 HTTP_UNPROCESSABLE_ENTITY         Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                             anterior está em                             realizada uma nova tentativa
                                  processamento,                               de instrução para um boleto
                                  aguarde alguns                               que está com a solicitação
                                  instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                   Erro ocasionado quando são
                                                                               enviadas muitas requisições
                                                                               em um curto espaço de
                                                                               tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-protesto-`

baixar-titulo

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-protesto-`

baixar-titulo HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.14. Comando de Instrução – Sustar Protesto e Manter Título

e Manter Título A operação PATCH “Sustar Protesto e Manter Título” é responsável por realizar a instrução que susta o protesto de um boleto que está em fluxo de cartório e mantém o boleto em carteira. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord           Nome                 Tipo         Parâmetro                    Formatação           Descrição      Obrigatório
                                                                  Tamanho

1     x-api-key               String            HeaderPar     36             UUID              Token de acesso Obrigatório
                                                am                                             fornecido pelo
                                                                                               Sicredi

2     authorization           String            Header        1413           Bearer +     Acess token            Obrigatório
                                                                             Token de     obtido na
                                                                             Autenticação autenticação
3     Content-Type            application/jso Header                         Sem        Tipo conteúdo Obrigatório
                              n                                              formatação
4     cooperativa             String          Header          4              Sem        Código da       Obrigatório
                                                                             formatação cooperativa do
                                                                                        beneficiário
5     posto                   String            Header        2              Sem        Código do posto Obrigatório
                                                                             formatação do beneficiário

6     codigoBeneficiario      String            Header        5              Sem        Código do                Obrigatório
                                                                             formatação beneficiário
7     nossoNumero             String            Path          9              Sem        Nosso Número             Obrigatório
                                                                             formatação

Ord       Nome                         Tipo            Tamanho       Formatação Descrição

1         transactionId                string          36            Sem            Identificador da transação
                                                                     formatação
2         dataMovimento                String          10            Sem            Data em que instrução foi realizada
                                                                     formatação
3         codigoBeneficiario           String          5             Sem            Código do beneficiário
                                                                     formatação
4         nossoNumero                  String          9             Sem            Nosso Número
                                                                     formatação
5         cooperativa                  String          4             Sem            Código da cooperativa do beneficiário
                                                                     formatação
6         posto                        String          2             Sem            Código da agência do beneficiário
                                                                     formatação

7        statusComando            String          17             Sem             Status da operação gerada, que será:
                                                                 formatação
                                                                                 - MOVIMENTO_ENVIADO

8        dataHoraRegistro         String (date-   26             Sem             Data e hora da instrução
                                  time)                          formatação
9        tipoMensagem             String          5              Sem             Tipo de instrução, que será:
                                                                 formatação
                                                                                 PEDIDO_SUSTAR_MANTER_PROTESTO

       RETORNO (HttpStatus):

    Status                                                          Descrição
    HTTP_ACCEPTED (202)                                             Operação recebida com sucesso

                                                                  Parâmetros de
                   Status                      Mensagem              entrada                   Descrição
                                                                 sujeitos a crítica
HTTP_UNAUTHORIZED (401)                    Could not find a     Parâmetro:          Erros ocasionados por
                                           required Access      x-api-key           parâmetros obrigatórios não
                                           Token in the                             informados ou inválidos.
                                           request, identified
                                           by HEADER x-api-
                                           key
HTTP_UNAUTHORIZED (401)                    UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                               Authorization           parâmetro não foi informado
                                                                                       ou está inválido.

HTTP_UNAUTHORIZED (401)                    Código de          Parâmetro:         Erro ocasionado quando o
                                           beneficiário       codigoBeneficiario código de beneficiário
                                           diferente do                          informado é diferente do
                                           beneficiário do                       beneficiário contido no token
                                           usuário                               de autenticação
                                                                                 (authorization).
HTTP_BAD_REQUEST (422)                     “0017 - Título não Parâmetro:         Erro ocasionado quando o
                                           cadastrado.”       nossoNumero        nosso número informado não
                                                                                 foi encontrado ou não existe.

 HTTP_UNPROCESSABLE_ENTITY           “0075 - Instrução Não se aplica             Erro ocasionado quando o
(422)                               inválida: título está                        título está aguardando
                                    aguardando                                   confirmação de entrada.
                                    confirmação.”
 HTTP_UNPROCESSABLE_ENTITY           “0079 - Instrução Não se aplica             Erro ocasionado quando o
(422)                               inválida: título não                         título não possui nenhuma
                                    foi encaminhado                              instrução de protesto para que
                                    para Cartório.”                              seja comandada a sustação.
 HTTP_UNPROCESSABLE_ENTITY           "0131 - Operação Não se aplica              Erro ocasionado quando o
(422)                               não permitida para                           título está vinculado a uma
                                    o tipo de carteira                           operação de crédito.
                                    Descontada."
 HTTP_UNPROCESSABLE_ENTITY           "0077 - Instrução Não se aplica             Erro ocasionado quando o
(422)                               inválida: título já                          título está liquidado.
                                    liquidado."

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova tentativa
                                    processamento,                               de instrução para um boleto
                                    aguarde alguns                               que está com a solicitação
                                    instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-protesto-`

manter-titulo

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-protesto-`

manter-titulo HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.15. Comando de Instrução – Incluir Negativação

Negativação A operação PATCH “Incluir Negativação” é responsável por realizar um comando de instrução que inclui um pedido de negativação para um boleto vencido. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord          Nome                  Tipo      Parâmetro                    Formatação           Descrição      Obrigatório
                                                               Tamanho

1     x-api-key               String        HeaderPar      36             UUID              Token de acesso Obrigatório
                                            am                                              fornecido pelo
                                                                                            Sicredi

2     authorization           String         Header        1413           Bearer +     Acess token            Obrigatório
                                                                          Token de     obtido na
                                                                          Autenticação autenticação
3     Content-Type            application/jso Header                      Sem        Tipo conteúdo            Obrigatório
                              n                                           formatação
4     cooperativa             String          Header       4              Sem        Código da                Obrigatório
                                                                          formatação cooperativa do
                                                                                     beneficiário

5    posto                    String            Header        2              Sem        Código do posto Obrigatório
                                                                             formatação do beneficiário

6    codigoBeneficiario       String            Header        5              Sem        Código do              Obrigatório
                                                                             formatação beneficiário
7    nossoNumero              String            Path          9              Sem        Nosso Número           Obrigatório
                                                                             formatação

Ord      Nome                          Tipo            Tamanho        Formatação Descrição

1        transactionId                 string          36             Sem             Identificador da transação
                                                                      formatação
2        dataMovimento                 String          10             Sem             Data em que instrução foi realizada
                                                                      formatação
3        codigoBeneficiario            String          5              Sem             Código do beneficiário
                                                                      formatação
4        nossoNumero                   String          9              Sem             Nosso Número
                                                                      formatação
5        cooperativa                   String          4              Sem             Código da cooperativa do
                                                                      formatação      beneficiário
6        posto                         String          2              Sem             Código da agência do beneficiário
                                                                      formatação
7        statusComando                 String          17             Sem             Status da operação gerada, que será:
                                                                      formatação
                                                                                      - MOVIMENTO_ENVIADO

8        dataHoraRegistro              String (date-   26             Sem             Data e hora da instrução
                                       time)                          formatação
9        tipoMensagem                  String          5              Sem             Tipo de instrução, que será:
                                                                      formatação
                                                                                      PEDIDO_NEGATIVACAO

       RETORNO (HttpStatus):

    Status                                                             Descrição
    HTTP_ACCEPTED (202)                                                Operação recebida com sucesso

                                                     Parâmetros de
               Status             Mensagem              entrada                   Descrição
                                                    sujeitos a crítica
HTTP_UNAUTHORIZED (401)       Could not find a     Parâmetro:          Erros ocasionados por
                              required Access      x-api-key           parâmetros obrigatórios não
                              Token in the                             informados ou inválidos.
                              request, identified
                              by HEADER x-api-
                              key
HTTP_UNAUTHORIZED (401)       UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                  Authorization           parâmetro não foi informado
                                                                          ou está inválido.

HTTP_UNAUTHORIZED (401)       Código de             Parâmetro:         Erro ocasionado quando o
                              beneficiário          codigoBeneficiario código de beneficiário
                              diferente do                             informado é diferente do
                              beneficiário do                          beneficiário contido no token
                              usuário                                  de autenticação
                                                                       (authorization).
HTTP_BAD_REQUEST (422)         “0017 - Título não Parâmetro:           Erro ocasionado quando o
                              cadastrado.”          nossoNumero        nosso número informado não
                                                                       foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY     “0075 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: título está                    título está aguardando
                              aguardando                               confirmação de entrada.
                              confirmação.”
 HTTP_UNPROCESSABLE_ENTITY     “0071 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: título                         título não possui nenhuma
                              ainda não venceu.”                       instrução de protesto para que
                                                                       seja comandada a sustação.
 HTTP_UNPROCESSABLE_ENTITY     "0131 - Operação Não se aplica          Erro ocasionado quando o
(422)                         não permitida para                       título está vinculado a uma
                              o tipo de carteira                       operação de crédito.
                              Descontada."
 HTTP_UNPROCESSABLE_ENTITY     "0077 - Instrução Não se aplica         Erro ocasionado quando o
(422)                         inválida: título já                      título está liquidado.
                              liquidado."

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova tentativa
                                    processamento,                               de instrução para um boleto
                                    aguarde alguns                               que está com a solicitação
                                    instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/negativacao`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/negativacao`

HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.16. Comando de Instrução – Excluir de Negativação e Baixar Título

Negativação e Baixar Título A operação PATCH “Excluir Negativação e Baixar Título” é responsável por realizar um comando de instrução que susta o pedido de negativação e realiza a baixa de um boleto vencido. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord           Nome                 Tipo      Parâmetro                    Formatação           Descrição      Obrigatório
                                                               Tamanho

1     x-api-key               String        HeaderPar      36             UUID              Token de acesso Obrigatório
                                            am                                              fornecido pelo
                                                                                            Sicredi

2     authorization           String         Header        1413           Bearer +     Acess token            Obrigatório
                                                                          Token de     obtido na
                                                                          Autenticação autenticação
3     Content-Type            application/jso Header                      Sem        Tipo conteúdo Obrigatório
                              n                                           formatação
4     cooperativa             String          Header       4              Sem        Código da       Obrigatório
                                                                          formatação cooperativa do
                                                                                     beneficiário
5     posto                   String         Header        2              Sem        Código do posto Obrigatório
                                                                          formatação do beneficiário

6     codigoBeneficiario      String         Header        5              Sem        Código do                Obrigatório
                                                                          formatação beneficiário
7     nossoNumero             String         Path          9              Sem        Nosso Número             Obrigatório
                                                                          formatação

Ord      Nome                      Tipo         Tamanho         Formatação Descrição

1        transactionId             string       36              Sem           Identificador da transação
                                                                formatação
2        dataMovimento             String       10              Sem           Data em que instrução foi realizada
                                                                formatação

3        codigoBeneficiario       String          5              Sem            Código do beneficiário
                                                                 formatação
4        nossoNumero              String          9              Sem            Nosso Número
                                                                 formatação
5        cooperativa              String          4              Sem            Código da cooperativa do beneficiário
                                                                 formatação
6        posto                    String          2              Sem            Código da agência do beneficiário
                                                                 formatação
7        statusComando            String          17             Sem            Status da operação gerada, que será:
                                                                 formatação
                                                                                - MOVIMENTO_ENVIADO

8        dataHoraRegistro         String          26             Sem            Data e hora da instrução
                                  (date-time)                    formatação
9        tipoMensagem             String          5              Sem            Tipo de instrução, que será:
                                                                 formatação
                                                                                PEDIDO_SUSTAR_BAIXAR_NEGATIVACAO

       RETORNO (HttpStatus):

    Status                                                            Descrição
    HTTP_ACCEPTED (202)                                               Operação recebida com sucesso

                                                                    Parâmetros de
                   Status                       Mensagem               entrada                   Descrição
                                                                   sujeitos a crítica
HTTP_UNAUTHORIZED (401)                    Could not find a       Parâmetro:          Erros ocasionados por
                                           required Access        x-api-key           parâmetros obrigatórios não
                                           Token in the                               informados ou inválidos.
                                           request, identified
                                           by HEADER x-api-
                                           key
HTTP_UNAUTHORIZED (401)                    UNAUTHORIZED Parâmetro:                       Erro ocasionado quando o
                                                               Authorization             parâmetro não foi informado
                                                                                         ou está inválido.

HTTP_UNAUTHORIZED (401)           Código de             Parâmetro:         Erro ocasionado quando o
                                  beneficiário          codigoBeneficiario código de beneficiário
                                  diferente do                             informado é diferente do
                                  beneficiário do                          beneficiário contido no token
                                  usuário                                  de autenticação
                                                                           (authorization).
HTTP_BAD_REQUEST (422)             “0017 - Título não Parâmetro:           Erro ocasionado quando o
                                  cadastrado.”          nossoNumero        nosso número informado não
                                                                           foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY         “0075 - Instrução Não se aplica         Erro ocasionado quando o
(422)                             inválida: título está                    título está aguardando
                                  aguardando                               confirmação de entrada.
                                  confirmação.”
 HTTP_UNPROCESSABLE_ENTITY         “0285 - Título sem Não se aplica        Erro ocasionado quando o
(422)                             instrução de                             título não possui nenhuma
                                  negativação. Favor                       instrução de protesto para que
                                  verificar.”                              seja comandada a sustação.
 HTTP_UNPROCESSABLE_ENTITY         "0131 - Operação Não se aplica          Erro ocasionado quando o
(422)                             não permitida para                       título está vinculado a uma
                                  o tipo de carteira                       operação de crédito.
                                  Descontada."
 HTTP_UNPROCESSABLE_ENTITY         "0077 - Instrução Não se aplica         Erro ocasionado quando o
(422)                             inválida: título já                      título está liquidado.
                                  liquidado."

 HTTP_UNPROCESSABLE_ENTITY         Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                             anterior está em                             realizada uma nova tentativa
                                  processamento,                               de instrução para um boleto
                                  aguarde alguns                               que está com a solicitação
                                  instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                   Erro ocasionado quando são
                                                                               enviadas muitas requisições
                                                                               em um curto espaço de
                                                                               tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-`

negativacao-baixar-titulo

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/sustar-negativacao-`

baixar-titulo HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

HEADERS:

BODY:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.17. Comando de Instrução – Cancelar Protesto Automático

Protesto Automático A operação PATCH “Cancelar Protesto Automático” é responsável por realizar a instrução de cancelar o protesto automático de um título que tenha protesto vinculado. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente de homologação)

```text
Ord           Nome                    Tipo      Parâmetro                    Formatação           Descrição      Obrigatório
                                                                  Tamanho

1     x-api-key               String            HeaderPar     36             UUID              Token de acesso Obrigatório
                                                am                                             fornecido pelo
                                                                                               Sicredi

2     Authorization           String            Header        1413           Bearer +     Acess token            Obrigatório
                                                                             Token de     obtido na
                                                                             Autenticação autenticação
3     Content-Type            application/jso Header                         Sem        Tipo conteúdo Obrigatório
                              n                                              formatação
4     cooperativa             String          Header          4              Sem        Código da       Obrigatório
                                                                             formatação cooperativa do
                                                                                        beneficiário
5     posto                   String            Header        2              Sem        Código do posto Obrigatório
                                                                             formatação do beneficiário

6     codigoBeneficiario      String            Header        5              Sem        Código do                Obrigatório
                                                                             formatação beneficiário
7     nossoNumero             String            Path          9              Sem        Nosso Número             Obrigatório
                                                                             formatação

Ord Nome                          Tipo     Tamanho Formatação Descrição

1       transactionId             string   36          Sem           Identificador da transação
                                                       formatação
2       dataMovimento             String   10          Sem           Data em que instrução foi realizada
                                                       formatação

3      codigoBeneficiario String           5          Sem             Código do beneficiário
                                                      formatação
4      nossoNumero                String   9          Sem             Nosso Número
                                                      formatação
5      cooperativa                String   4          Sem             Código da cooperativa do beneficiário
                                                      formatação
6      posto                      String   2          Sem             Código da agência do beneficiário
                                                      formatação
7      statusComando              String   17         Sem             Status da operação gerada, que será:
                                                      formatação
                                                                      - MOVIMENTO_ENVIADO

8      dataHoraRegistro           String   26         Sem             Data e hora da instrução
                                  (date-              formatação
                                  time)
9      tipoMensagem               String   5          Sem             Tipo de instrução, que será:
                                                      formatação
                                                                      PEDIDO_CANCELAMENTO_PROTESTO_AUTOMATICO

       RETORNO (HttpStatus):

    Status                                                               Descrição
    HTTP_ACCEPTED (202)                                                  Operação recebida com sucesso

                                                                       Parâmetros de
                   Status                          Mensagem               entrada                   Descrição
                                                                      sujeitos a crítica
HTTP_UNAUTHORIZED (401)                         Could not find a     Parâmetro:          Erros ocasionados por
                                                required Access      x-api-key           parâmetros obrigatórios não
                                                Token in the                             informados ou inválidos.
                                                request, identified
                                                by HEADER x-api-
                                                key
HTTP_UNAUTHORIZED (401)                         UNAUTHORIZED Parâmetro:                     Erro ocasionado quando o
                                                                    Authorization           parâmetro não foi informado
                                                                                            ou está inválido.

HTTP_UNAUTHORIZED (401)             Código de             Parâmetro:         Erro ocasionado quando o
                                    beneficiário          codigoBeneficiario código de beneficiário
                                    diferente do                             informado é diferente do
                                    beneficiário do                          beneficiário contido no token
                                    usuário                                  de autenticação
                                                                             (authorization).
HTTP_BAD_REQUEST (422)               “0017 - Título não Parâmetro:           Erro ocasionado quando o
                                    cadastrado.”          nossoNumero        nosso número informado não
                                                                             foi encontrado ou não existe.
 HTTP_UNPROCESSABLE_ENTITY           “0075 - Instrução Não se aplica         Erro ocasionado quando o
(422)                               inválida: título está                    título está aguardando
                                    aguardando                               confirmação de entrada.
                                    confirmação.”
 HTTP_UNPROCESSABLE_ENTITY           " 068-Instrucao      Não se aplica      Erro ocasionado quando o
(422)                               invalida: Título                         título não possui abatimento
                                    enviado para                             vinculado.
                                    Cartório."
 HTTP_UNPROCESSABLE_ENTITY           "0077 - Instrução Não se aplica         Erro ocasionado quando o
(422)                               inválida: título já                      título está liquidado.
                                    liquidado."

 HTTP_UNPROCESSABLE_ENTITY           Sua solicitação      Não se aplica          Erro ocasionado quando é
(422)                               anterior está em                             realizada uma nova tentativa
                                    processamento,                               de instrução para um boleto
                                    aguarde alguns                               que está com a solicitação
                                    instantes.                                   anterior em processamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                     Erro ocasionado quando são
                                                                                 enviadas muitas requisições
                                                                                 em um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/{nossoNumero}/cancelar-protesto-`

automatico

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789

posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/{nossoNumero}/cancelar-protesto-`

automatico HEADERS: Authorization x-api-key Content-Type: application/json Cooperativa posto codigoBeneficiario

BODY:

```json
{}
```

PathParam: nossoNumero

### Execução (via ferramenta Postman)

Headers:

Body:

> **OBS:** O body permanece vazio, conforme imagem acima.

Cenário Positivo:

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.18. Consulta de Boletos por Nosso Número

A operação GET “Consulta por Nosso Número” é responsável por realizar a consulta de um único boleto, através do Nosso Número. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

▪ OBS: O recurso de “Consulta de Boletos por Nosso Número” possui atualmente uma nova versão (“v2”) com as seguintes diferenças: ▪ Novo header (data-movimento) do tipo bollean que ao ser informado = “true” retorna a data do movimento de liquidação utilizando o próximo dia útil. Na “v1” quando as liquidações ocorrem em finais de semana e/ou feriados a data de liquidação apresentada é o dia em que ocorreu a liquidação, sem considerar o próximo dia útil. Abaixo segue mais detalhes sobre o novo header (Ord 8).

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – GET

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo     Parâmetro                     Formatação         Descrição      Obrigatório

1         x-api-key             String      Header         36           UUID              Token de acesso Obrigatório
                                                                                          fornecido pelo
                                                                                          Sicredi

2       authorization         String      Header         1413         Bearer + Token Acess token           Obrigatório
                                                                      de Autenticação obtido na
                                                                                      autenticação
3       Content-Type          application/ Header                     Sem formatação Tipo conteúdo         Obrigatório
                              json
4       cooperativa           String       Header        4            Sem formatação Código da             Obrigatório
                                                                                     cooperativa do
                                                                                     beneficiário
5       Posto                 String      Header         2            Sem formatação Código da             Obrigatório
                                                                                     agência do
                                                                                     beneficiário
6       codigoBeneficiario String         Param          5            Sem formatação Código do             Obrigatório
                                                                                     beneficiário
7       nossoNumero           String      Param          9            Sem formatação Nosso Número          Obrigatório

8       data-movimento        Boolean    HeaderPar       true /       Sem formatação Obs: Esse      Opcional
                                         am              false                       header só deve
                                                                                     ser informado
                                                                                     caso esteja
                                                                                     utilizando o
                                                                                     endpoint “v2”.

                                                                                        Responsável por
                                                                                        apresentar a
                                                                                        data de
                                                                                        movimento da
                                                                                        liquidação
                                                                                        utilizando
                                                                                        próximo dia útil
                                                                                        quando
                                                                                        liquidações são
                                                                                        efetuadas em
                                                                                        dias não úteis.

                                                                                        Opções:
                                                                                        - true
                                                                                         (retorna campo
                                                                                        “dataMovimento
                                                                                        ”)
                                                                                        - false

                                                                                     (não retorna o
                                                                                     campo
                                                                                     “dataMovimento
                                                                                     ”)

Ord Nome                                      Tipo            Tamanho Formatação                   Descrição
1    linhaDigitavel                           String          47           Sem formatação          Linha digitável
                                                                                                   do boleto
2    codigoBarras                             String          44           Sem formatação          Código de barras
                                                                                                   do boleto

3    carteira                                 String                       Sem formatação          Tipo de carteira,
                                                                                                   podendo ser:

                                                                                                   A – SIMPLES
                                                                                                   B–
                                                                                                   CAUCIONADA
                                                                                                   C–
                                                                                                   DESCONTADA
                                                                                                   D – VINCULADA
4    seuNumero                                String          10           Sem formatação          Seu Número

5    nossoNumero                              String          9            Sem formatação          Nosso Número

6    Pagador                  codigo          String          5            Sem formatação          Codigo do
                                                                                                   pagador
     (Grupo com os dados
     do pagador)              documento       String          14           Sem formatação          Documento do
                                                                                                   pagador

                              nome            String          40           Sem formatação          Nome do
                                                                                                   pagador

7    beneficiarioFinal        codigo          String         3             Sem formatação          Código do
                                                                                                   beneficiário final
     (Grupo com os dados
     do beneficiário final)
                              documento       String          14           Sem formatação          Documento do
     OBS: Caso não exista                                                                          beneficiário final
     beneficiário final

      vinculado ao boleto, o    nome          String          40           Sem formatação          Nome do
      grupo não será exibido.                                                                      beneficiário final

8     dataEmissao                             String          10           YYYY-MM-DD              Data de emissão
                                              (date)                                               do boleto

9     dataVencimento                          String          10           YYYY-MM-DD              Data de
                                              (date)                                               vencimento do
                                                                                                   boleto
10    valorNominal                            Bigdecimal      14,2         Sem formatação          Valor nominal
                                                                                                   do boleto

11    situação                                String                       Sem formatação          Situação do
                                                                                                   boleto, podendo
                                                                                                   ser:

                                                                                                   - EM CARTEIRA
                                                                                                   - EM CARTEIRA
                                                                                                   PIX
                                                                                                   - VENCIDO
                                                                                                   - LIQUIDADO
                                                                                                   - LIQUIDADO
                                                                                                   CARTORIO
                                                                                                   - LIQUIDADO
                                                                                                   REDE
                                                                                                   - LIQUIDADO
                                                                                                   COMPE
                                                                                                   - LIQUIDADO PIX
                                                                                                   - LIQUIDADO
                                                                                                   CHEQUE
                                                                                                   - BAIXADO POR
                                                                                                   SOLICITACAO
                                                                                                   - PROTESTADO
                                                                                                   - EM CARTORIO
                                                                                                   - NEGATIVADO
                                                                                                   - AGUARDANDO
                                                                                                   ENTRADA EM
                                                                                                   CARTORIO
                                                                                                   - AGUARDANDO
                                                                                                   SUSTACAO DE
                                                                                                   CARTORIO
                                                                                                   - REJEITADO

12    txId                            String                       Sem formatação          Identificador da
                                                                                           transação Pix

13    codigoQrCode                    String                       Sem formatação          QRCODE do
                                                                                           boleto híbrido

                                                                   Domínios: VALOR ou
14    tipoMulta                       String          10                                   Tipo de multa
                                                                   PERCENTUAL

15    multa                           Number          14,2         Sem formatação          Percentual de
                                                                                           multa do boleto

                                                                                           Data de início
16    dataInicioMulta                 number          10           YYYY-MM-DD              para cobrança
                                                                                           da Multa

17    abatimento                      Number          14,2         Sem formatação          Valor de
                                                                                           abatimento
                                                                                           concedido do
                                                                                           boleto
18    tipoJuros                       String         14            Domínios:               Tipo de juros do
                                                                   A – VALOR               boleto
                                                                   B - PERCENTUAL

                                                                                           Tipo de Juros
                                                                   Domínios: DIARIO ou     Percentual:
19    tipoJurosPercentual             String          6
                                                                   MENSAL                  A - DIARIO
                                                                                           B - MENSAL
20    juros                           Number          14,2         Sem formatação          Valor/percentual
                                                                                           de juros do
                                                                                           boleto

                                                                                           Data de início da
21    dataInicioJuros                 number          10           YYYY-MM-DD              cobrança de
                                                                                           Juros

22    diasProtesto                    Integer         2            Sem formatação          Quantidade de
                                                                                           dias para
                                                                                           protesto
                                                                                           automático do
                                                                                           boleto

23    idTituloEmpresa                         String          25           Sem formatação          idTituloEmpresa
                                                                                                   enviado no
                                                                                                   momento do
                                                                                                   cadastro do
                                                                                                   título. (Obs: Só
                                                                                                   será retornado
                                                                                                   se o título
                                                                                                   possuir essa
                                                                                                   informação).
24    validadeAPosVencimento                  Integer         4            Sem formatação          Validade após
                                                                                                   vencimento

25    diasNegativacao                         Integer         2            Sem formatação          Dias para
                                                                                                   negativação do
                                                                                                   boleto

26    tipoDesconto                            String          14           Domínios:               Tipo de
                                                                                                   desconto do
                                                                           A – VALOR               boleto, podendo
                                                                           B - PERCENTUAL          ser:

                                                                                                   A – VALOR
                                                                                                   B - PERCENTUAL
27    descontoAntecipacao                     Number          14,2         Sem formatação          Valor do
                                                                                                   desconto
                                                                                                   antecipado do
                                                                                                   boleto
28    descontos                numeroOrdem Integer           1             Domínios:                Número da
                                                                                                   ordem, podendo
      (Lista com até 3                                                     1                       ser:
      descontos)                                                           2
                                                                           3                       1- Desconto 1
      Obs: Caso não exista                                                                         2- Desconto 2
      desconto cadastrado, a                                                                       3- Desconto 3
      lista retorna vazia:

      "descontos": []          valorDesconto Bigdecimal      14,2           "valorDesconto": 0.0   Valor/percentual
                                                                                                   de desconto

                               dataLimite        String(date) 10              "dataLimite": "YYYY-    Data limite do
                                                                              MM-DD"                  desconto

29    dadosLiquidacao          data              String(date)                 YYYY-MM-              Data e hora da
                                                                              DD'T':HH:mm:ss.SSS'Z' liquidação do
      (Grupo que retorna os                                                                         boleto
      valores de liquidação do
                               valor             Bigdecimal     14,2          Sem formatação          Valor da
      boleto)
                                                                                                      liquidação do
                                                                                                      boleto
      OBS: Esse grupo só será
      exibido caso o boleto   multa              Bigdecimal      14,2         Sem formtação           Valor da multa
      esteja liquidado.                                                                               cobrado na
                                                                                                      liquidação

                               abatimento        Bigdecimal      14,2         Sem formatação          Valor do
                                                                                                      abatimento
                                                                                                      concedido na
                                                                                                      liquidação
                               juros             Bigdecimal      14,2         Sem formatação          Valor de juros
                                                                                                      cobrado na
                                                                                                      liquidação

                               desconto          Bigdecimal      14,2         Sem formatação          Valor do
                                                                                                      desconto
                                                                                                      concedido na
                                                                                                      liquidação
30    dataMovimento            String (data)                     10           "YYYY-MM-DD"            Data do
                                                                                                      Movimento
      (Campo retorna Data
      do Movimento que foi                                                                            OBS: só será
      efetivada liquidação)                                                                           exibida caso
                                                                                                      enviado o
                                                                                                      headerParam
                                                                                                      data-movimento
                                                                                                      = true.
31    dataPrevisaoPagamento String (data)                       10             "YYYY-MM-DD"           Data de Previsão
                                                                                                      do crédito na
     (Campo retorna data do                                                                           conta do
     crédito do boleto na                                                                             beneficiário de
     conta do beneficiário)                                                                           cobrança.

                                                                                                      OBS. Só será
                                                                                                      exibido para

                                                                                               boletos
                                                                                               liquidados.

 Status                                                 Descrição
 HTTP_OK (200)                                          Operação realizada com sucesso

                                                      Parâmetros de
                Status             Mensagem              entrada                 Descrição
                                                     sujeitos a crítica
HTTP_UNAUTHORIZED (401)        Could not find a     Parâmetro:          Erros ocasionado por
                               required Access      x-api-key           parâmetro obrigatório não
                               Token in the                             informado ou inválido.
                               request, identified
                               by HEADER x-api-
                               key
HTTP_UNAUTHORIZED (401)        UNAUTHORIZED Parâmetro:                     Erro ocasionado por
                                                   Authorization           parâmetro obrigatório não
                                                                           foi informado ou inválido.

HTTP_UNAUTHORIZED (401)        Código de            Parâmetro:         Erro ocasionado quando o
                               beneficiário         codigoBeneficiario código de beneficiário
                               diferente do                            informado é diferente do
                               beneficiário do                         beneficiário contido no
                               usuário                                 token de autenticação
                                                                       (authorization).

HTTP_UNAUTHORIZED (401)            Cooperativa           Parâmetro:             Erro ocasionado quando a
                                   diferente da          Cooperativa            cooperativa informada é
                                   cooperativa do                               diferente da cooperativa
                                   usuário                                      contida no token de
                                                                                autenticação
                                                                                (authorization).
HTTP_NOT_FOUND (404)               Titulo não            Parâmetro:             Erro ocasionado quando
                                   encontrado            nossoNumero            não existe resultado para a
                                                                                consulta informada.

HTTP_BAD_REQUEST (400)       O campo nosso     Parâmetro:                       Erro ocasionado quando o
                             numero deve ter nossoNumero                        parâmetro foi preenchido
                             apenas 9 (nove)                                    com menos ou mais de 9
                             caracteres                                         caracteres.
                             numéricos.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                    Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um curto
                                                                                espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto”, “codigoBeneficiario” e “nossoNumero”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

Versão 1 - “v1”

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos`

HEADERS:

Authorization x-api-key

Content-Type: application/json Cooperativa posto

PARAMS:

codigoBeneficiario nossoNumero

Versão 2 - “v2” (Não possui sandbox)

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v2/boletos`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto data-movimento

PARAMS:

codigoBeneficiario nossoNumero

### RESPONSE BODY

```json
{
"linhaDigitavel": "74891121150039736789903123451001187340000000050",
"codigoBarras": "74897937700000099891122224595067890312345109",
"carteira": "SIMPLES",
"seuNumero": "TESTE",
"nossoNumero": "211001292",
"pagador": {
"codigo": "XXXXX",
"documento": "000000000",
"nome": "TESTE COBRANCA"
},
"beneficiarioFinal": {
"codigo": "000",
"documento": "00000000000",
"nome": "COBRANCA TESTE"
},
"dataEmissao": "2022-08-25",
"dataVencimento": "2022-09-23",
"valorNominal": 1,
"situacao": "EM CARTEIRA PIX",
"txId": "79031878967e4032a89c59eb3f232cda",
"codigoQrCode": " 00020126930014br.gov.bcb.pix2571pix-qrcode-
h.sicredi.com.br/qr/v2/cobv/528520acdd5f4740b63b9b643ca2bcf99999999999999999999BR5903PIX
6006Cidade62070503***630441AC",
"tipoMulta": "PERCENTUAL",
"multa": 0,
"dataInicioMulta":"2025-11-26",
"abatimento": 0,
"tipoJuros": "A - VALOR",
"tipoJurosPercentual": "DIARIO",
"juros": 0,
"dataInicioJuros":"2025-11-26",
"diasProtesto": 0,
"validadeAposVencimento": 1,
"diasNegativacao": 0,
"tipoDesconto": "A - VALOR",
"descontoAntecipacao": 0,
"descontos": [
{
"numeroOrdem": 1,
"valorDesconto": 10,
"dataLimite": "2022-09-03"
},
{
"numeroOrdem": 2,
"valorDesconto": 8,
"dataLimite": "2022-09-05"
},
{
"numeroOrdem": 3,
"valorDesconto": 5,
"dataLimite": "2022-09-07"
}
]
}
```

], "dataMovimento": "2024-01-25", -> Essa informação será apresentada somente na v2 "boletoComSplit": "NAO" }

### Execução (via ferramenta Postman)

Headers: Chamada endpoint v1:

Chamada endpoint v2:

Params:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 400 – BAD_REQUEST

### 7.19. Consulta de Boletos Liquidados por Dia

A operação GET “Consulta de Boletos Liquidados por Dia” é responsável por realizar a consulta de boletos já liquidados, através de uma data específica. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response.

O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

▪ OBS: Quando os boletos são pagos através do PIX nos finais de semana, o valor é creditado no mesmo dia da liquidação. No entanto, a informação da data de liquidação será disponibilizada com data do dia útil seguinte. Para identificar a informação de pagamento, recomendamos consultar a data do dia útil seguinte à liquidação PIX.

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – GET

### URL Sandbox (ambiente para homologação)

```text
    Ord           Nome             Tipo     Parâmetro                     Formatação         Descrição      Obrigatório

1         x-api-key             String      Header         36           UUID              Token de acesso Obrigatório
                                                                                          fornecido pelo
                                                                                          Sicredi

2         authorization         String      Header         1413         Bearer + Token Acess token          Obrigatório
                                                                        de Autenticação obtido na
                                                                                        autenticação
3         Content-Type          application/ Header                     Sem formatação Tipo conteúdo        Não
                                x-www-                                                                      obrigatório
                                form-
                                urlencoded
4         cooperativa           String       Header        4            Sem formatação Código da            Obrigatório
                                                                                       cooperativa do
                                                                                       beneficiário
5         posto                 String      Header         2            Sem formatação Código da            Obrigatório
                                                                                       agência do
                                                                                       beneficiário
6         codigoBeneficiario String         Param          5            Sem formatação Código do            Obrigatório
                                                                                       beneficiário
7         dia                   String      Param          10           DD/MM/YYYY      Data em que o       Obrigatório
                                                                                       boleto foi
                                                                                       liquidado
8         cpfCnpjBeneficiario String        Param          14           Sem formatação CPF ou CNPJ do Opcional
          Final                                                                        beneficiário final

9        pagina                  Integer   Param          2            Sem formatação Número da         Opcional
                                                                                      página da
                                                                                      consulta

Ord Nome                                        Tipo          Tamanho Formatação         Descrição
1     items       cooperativa                   String        4          Sem             Cooperativa do beneficiário
                                                                         formatação      do boleto

2                 codigoBeneficiario            String        5          Sem             Código do beneficiário do
                                                                         formatação      boleto

3                 cooperativaPostoBeneficiario String         6          Sem             Código da cooperativa e
                                                                         formatação      posto do beneficiário

4                 nossoNumero                   String        9          Sem             Nosso Número
                                                                         formatação

5                 seuNumero                     String        10         Sem             Seu Número do boleto
                                                                         formatação

6                 tipoCarteira                  String        20         Sem             Tipo de carteira, podendo
                                                                         formatação      ser:

                                                                                         - CARTEIRA_SIMPLES
                                                                                         - CARTEIRA_ CAUCIONADA
                                                                                         - CARTEIRA_DESCONTADA
                                                                                         - CARTEIRA_VINCULADA
7                 dataPagamento                 String        10         YYYY-MM-DD      Data da liquidação do
                                                                                         boleto

8                 valor                         Number 14,2              Sem             Valor nominal do boleto
                                                                         formatação

9                 valorLiquidado                Number 14,2              Sem             Valor da liquidação do
                                                                         formatação      boleto

10                jurosLiquido                  Number 14,2              Sem             Valor de juros do boleto
                                                                         formatação      liquidado

11                descontoLiquido               Number 14,2              Sem             Valor de desconto do
                                                                         formatação      boleto liquidado

12                multaLiquida                  Number 14,2              Sem             Valor de multa do boleto
                                                                         formatação      liquidado

13               abatimentoLiquido            Number 14,2              Sem                 Valor de abatimento do
                                                                       formatação          boleto liquidado

14               tipoLiquidacao               String      23           Sem                 Tipo da liquidação,
                                                                       formatação          podendo ser:

                                                                                           - AVISO DE PAGAMENTO
                                                                                           REDE
                                                                                           - AVISO DE PAGAMENTO
                                                                                           COMPE
                                                                                           - REDE
                                                                                           - COMPE
                                                                                           - PIX
15    hasNext                                 String      5            Sem                 Esse campo indica se
                                                                       formatação          existem mais páginas ou
                                                                                           não, podendo ser:

                                                                                           - false (indica que não
                                                                                           existem mais páginas)
                                                                                           - true (existem mais
                                                                                           páginas)

                                                                                           OBS: Cada página retorna
                                                                                           500 registros.

 Status                                                        Descrição
 HTTP_OK (200)                                                 Operação realizada com sucesso

                                                         Parâmetros de entrada
                Status                  Mensagem                                             Descrição
                                                            sujeitos a crítica

HTTP_UNAUTHORIZED (401)              Could not find a Parâmetro:                       Erros ocasionado por
                                     required Access x-api-key                         parâmetro obrigatório
                                     Token in the                                      não informado ou
                                     request, identified                               inválido.
                                     by HEADER x-api-
                                     key

HTTP_UNAUTHORIZED (401)       UNAUTHORIZED         Parâmetro:                    Erro ocasionado por
                                                   Authorization                 parâmetro obrigatório
                                                                                 não foi informado ou
                                                                                 inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:                    Erro ocasionado quando
                              beneficiário         codigoBeneficiario            o código de beneficiário
                              diferente do                                       informado é diferente
                              beneficiário do                                    do beneficiário contido
                              usuário                                            no token de
                                                                                 autenticação
                                                                                 (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:                    Erro ocasionado quando
                              diferente da         Cooperativa                   a cooperativa informada
                              cooperativa do                                     é diferente da
                              usuário                                            cooperativa contida no
                                                                                 token de autenticação
                                                                                 (authorization).
HTTP_BAD_REQUEST (400)        O dia de pesquisa Parâmetro:                       Erro ocasionado quando
                              deve ter 10         dia                            a data de pesquisa está
                              caracteres e seguir                                inválida.
                              o padrão:
                              DD/MM/YYYY.
HTTP_BAD_REQUEST (400)        BAD_REQUEST         Parâmetro:                     Erro ocasionado quando
                                                  dia                            a data de pesquisa não
                                                                                 foi informada.
HTTP_BAD_REQUEST (400)        O campo código Parâmetro:                          Erro ocasionado quando
                              do beneficiário    codigoBeneficiario              o código do beneficiário
                              deve ter 5 (cinco)                                 informado está inválido
                              caracteres                                         ou não foi informado.
                              numéricos.
HTTP_BAD_REQUEST (400)         Documento do      Parâmetro:               Erro ocasionado quando
                              beneficiário final cpfCnpjBeneficiarioFinal o cpf/cnpj do
                              inválido.                                   beneficiário final
                                                                          informado é inválido.

 HTTP_TOO_MANY_REQUESTS       Too many             Não se aplica                 Erro ocasionado quando
(429)                         requests                                           são enviadas muitas
                                                                                 requisições em um curto
                                                                                 espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/liquidados/dia`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/liquidados/dia`

HEADERS:

Authorization x-api-key Content-Type: application/x-www-form-urlencoded Cooperativa Posto

PARAMS:

codigoBeneficiario dia cpfCnpjBeneficiarioFinal pagina

### RESPONSE BODY

```json
{
"items": [
{
"cooperativa": "6789",
"codigoBeneficiario": "12345",
"cooperativaPostoBeneficiario": "678903",
"nossoNumero": "000000000",
"seuNumero": "TESTE",
"tipoCarteira": "CARTEIRA_NORMAL",
"dataPagamento": "2022-11-14",
"valor": 0.5,
"valorLiquidado": 0.5,
"jurosLiquido": 0,
"descontoLiquido": 0,
"multaLiquida": 0,
"abatimentoLiquido": 0,
"tipoLiquidacao": "REDE"
}
],
"hasNext": false
}
```

### Execução (via ferramenta Postman)

Headers:

Params:

Cenário Positivo:

Cenário Negativo: 401 – UNAUTHORIZED

Cenário Negativo: 400 – BAD_REQUEST

### 7.20. Consulta título cadastrado por “idEmpresa” ou “seuNumero”

“idEmpresa” ou “seuNumero” A operação GET “Verifica se título já cadastrado ‘idEmpresa’ ou ‘seuNumero’” pode ser utilizada com o objetivo de verificar se já existe um título cadastrado na base Sicredi com aquela informação de “idTituloEmpresa” ou “seuNumero”, visando evitar uma duplicidade no cadastro desse título. O formato de entrada e saída é JSON onde caso exista registros, eles serão retornados em uma lista que irá conter os dados do título pesquisado de acordo com o filtro utilizado. Considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8.

### Entrada

Tipo de Tamanho

### Saída

### FALHAS

### URL Produção

```text
Ord           Nome          Tipo     Parâmetr                      Formatação      Descrição      Obrigatório
                                         o

1       x-api-key             String      Header         36           UUID              Token de acesso Obrigatório
                                                                                        fornecido pelo
                                                                                        Sicredi

2       Authorization         String      Header         1413         Bearer + Token Acess token          Obrigatório
                                                                      de Autenticação obtido na
                                                                                      autenticação
3       Content-Type          application/ Header                     Sem formatação Tipo conteúdo        Obrigatório
                              x-www-
                              form-
                              urlencoded
4       posto                 String       Header        2            Sem formatação Código do posto Obrigatório
                                                                                     do beneficiário

5       cooperativa           String      Header         4            Sem formatação Código da       Obrigatório
                                                                                     cooperativa do
                                                                                     beneficiário
6       codigoBeneficiario String         Param          5            Sem formatação Código do       Obrigatório
                                                                                     beneficiário
7       seuNumero             String      Param          10           Sem formatação “seuNumero” Opcional
                                                                                     informado no
                                                                                     ato do cadastro
                                                                                     do título
8       idTituloEmpresa       String      Param          25           Sem formatação “idTituloEmpres Opcional
                                                                                     a” informado no
                                                                                     ato do cadastro
                                                                                     do título

Ord Nome                                  Tipo               Tamanho Formatação                  Descrição
1    linhaDigitavel                       String             47         Sem formatação           Linha digitável do
                                                                                                 boleto

2    codigoBarras                         String             44         Sem formatação           Código de barras do
                                                                                                 boleto

3    qrCode                               String             201        Sem formatação           QRCode PIX em caso
                                                                                                 de títulos híbridos.
                                                                                                 Se o título for
                                                                                                 normal não será
                                                                                                 retornado essa
                                                                                                 informação.

4     seuNumero                             String           10           Sem formatação          Seu Número do
                                                                                                  título cadastrado.

5     idTituloEmpresa                       String           25           Sem formatação          idTituloEmpresa
                                                                                                  caso o título possua
                                                                                                  essa informação.
6     valorNominal                          Number           14,2                                 Valor nominal do
                                                                                                  título cadastrado.

7     Pagador              documento        String           14           Sem formatação          Documento do
      (Object)                                                                                    pagador

                           nome             String           40           Sem formatação          Nome do pagador

 Status                                                             Descrição
 HTTP_OK (200)                                                      Operação realizada com sucesso.

                                                                  Parâmetros de entrada
                 Status                   Mensagem                                                    Descrição
                                                                     sujeitos a crítica

HTTP_UNAUTHORIZED (401)                Could not find a Parâmetro:                           Erros ocasionado por
                                       required Access x-api-key                             parâmetro obrigatório
                                       Token in the                                          não informado ou
                                       request, identified                                   inválido.
                                       by HEADER x-api-
                                       key
HTTP_UNAUTHORIZED (401)                UNAUTHORIZED Parâmetro:                               Erro ocasionado por
                                                           Authorization                     parâmetro obrigatório
                                                                                             não foi informado ou
                                                                                             inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:                    Erro ocasionado
                              beneficiário         codigoBeneficiario            quando o código de
                              diferente do                                       beneficiário informado
                              beneficiário do                                    é diferente do
                              usuário                                            beneficiário contido no
                                                                                 token de autenticação
                                                                                 (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:                    Erro ocasionado
                              diferente da         Cooperativa                   quando a cooperativa
                              cooperativa do                                     informada é diferente
                              usuário                                            da cooperativa contida
                                                                                 no token de
                                                                                 autenticação
                                                                                 (authorization).
HTTP_BAD_REQUEST (400)        O id titulo       Parâmetro:                       Erro ocasionado
                              empresa permite idTituloEmpresa                    quando o id informado
                              até 25 caracteres                                  ultrapassa 25
                                                                                 caracteres.

HTTP_BAD_REQUEST (400)        O seu numero do Parâmetro:                       Erro ocasionado
                              boleto deve ter   seuNumero                      quando o seu numero
                              até 10 caracteres                                informado ultrapassa
                                                                               10 caracteres.
HTTP_BAD_REQUEST (400)         Informe apenas      Parâmetro:                  Erro ocasionado
                              uma das duas         seuNumero e idTituloEmpresa quando os dois
                              opções:                                          parâmetros são
                              seuNumero ou                                     informados
                              idTituloEmpresa                                  simultaneamente.
HTTP_BAD_REQUEST (400)         Obrigatório     Parâmetro:                  Erro ocasionado
                              informar         seuNumero e idTituloEmpresa quando nenhum dos
                              seuNumero ou                                 dois parâmetros é
                              idTituloEmpresa.                             informado.

HTTP_BAD_REQUEST (404)        Título não           Parâmetro:                  Erro ocasionado
                              encontrado           seuNumero e idTituloEmpresa quando nenhum título
                                                                               é encontrado para
                                                                               aquele seuNumero ou
                                                                               idTituloEmpresa
 HTTP_TOO_MANY_REQUESTS       Too many             Não se aplica                 Erro ocasionado
(429)                         requests                                           quando são enviadas
                                                                                 muitas requisições em
                                                                                 um curto espaço de
                                                                                 tempo.

```

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/cadastrados`

HEADERS:

Authorization x-api-key Content-Type application/json cooperativa posto

PARAMS:

codigoBeneficiario idTituloEmpresa seuNumero

### RESPONSE BODY

[

```json
{
"codigoBarras": "74892976100000000000000000009999990800000004",
"linhaDigitavel": "74800000000001070000000000000000000009999900200",
"qrCode": "00020126910014br.gov.bcb.pix2569pix-
qrcode.sicredi.com.br/qr/v2/cobv/xxxxxxxxxxxxxx996d58209b00000a65204000050000000000BR5903P
IX6006Cidade62070503***6304CC72",
"seuNumero": "TESTE",
"idTituloEmpresa": "9999-TESTE-89ABC",
"valorNominal": 2,
"pagador": {
"documento": "99999900012",
"nome": "TESTE PAGADOR"
}
}
```

]

### Execução (via ferramenta Postman)

HEADERS:

PARAMS:

▪ Cenário Positivo:

### 7.21. Consulta movimentações financeiras (Francesinha)

(Francesinha) A operação GET “Lista de movimentações financeiras” é responsável por realizar a consulta das movimentações financeiras de crédito e/ou débito de uma determinada data. O formato de entrada e saída é JSON onde os registros retornados estão paginados de acordo com a quantidade total de movimentações financeiras da data informada, sendo apresentados em 1000 registros por página

Considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

▪ Observação: Atualmente a consulta de movimentações financeiras está parametrizada para um retorno de no máximo 1 mil registros por data de movimento. Essa parametrização pode aumentar conforme necessidade.

### Entrada

Tipo de Tamanho

### Saída

### FALHAS

### REQUEST – GET

### URL Sandbox (ambiente para homologação)

```text
    Ord                Nome         Tipo    Parâmetr                      Formatação         Descrição      Obrigatório
                                                o
1             x-api-key         String      Header         36           UUID              Token de acesso Obrigatório
                                                                                          fornecido pelo
                                                                                          Sicredi

2             Authorization     String      Header         1413         Bearer + Token Acess token          Obrigatório
                                                                        de Autenticação obtido na
                                                                                        autenticação
3             Content-Type      application/ Header                     Sem formatação Tipo conteúdo        Obrigatório
                                x-www-
                                form-
                                urlencoded
4             beneficiario      String       Param         5            Sem formatação Código do            Obrigatório
                                                                                       beneficiário

5             agencia           String      Param          4            Sem formatação Código da            Obrigatório
                                                                                       agência do
                                                                                       beneficiário
6             dataLancamento    String      Param          10           DD/MM/YYYY     Código do            Obrigatório
                                                                                       beneficiário
7             tipoMovimento     String      ENUM                        - CREDITO      Tipo de              Obrigatório
                                                                        - DEBITO       movimento
                                                                        - AMBOS        (CREDITO e/ou
                                                                                       DEBITO) a ser
                                                                                       retornado
8             pagina            Integer     Param          0            Sem formatação Número da            Opcional
                                                                                       página da
                                                                                       consulta

Ord      Nome                            Tipo             Tamanho        Formatação        Descrição
1        resultado     agencia           String            4             Sem formatação    Agência do associado.

2                      posto             String            2             Sem formatação    Código do posto do
                                                                                           beneficiário.

3                      beneficiario      String            5             Sem formatação    Código do beneficiário
                                                                                           associado.

4                      nossoNumero       String            9             Sem formatação    Nosso Número

5                      seuNumero         String            10            Sem formatação    Seu Número do boleto

6                      nomePagador       String            40            Sem formatação    Nome do pagador
                                                                                           referente a liquidação.

7                      identPagador      String            14            Sem formatação    Documento de
                                                                                           identificação do pagador
                                                                                           (CPF/CNPJ)
8                      datamovimento     String            10            YYYY/MM/DD        Data da movimentação
                                                                                           financeira

9                      dataLancamento    String            10            YYYY/MM/DD        Data do lançamento
                                                                                           financeiro

10                     valorNominal      Number            14,2          00.0              Valor nominal da
                                                                                           liquidação

11                     valorAbatimento   Number            14,2          00.0 | 0.00       Valor de abatimento

12                     valorDesconto     Number            14,2          00.0 | 0.00       Valor de desconto

13                     valorJuros        Number           14,2           00.00 | 0.00      Valor de juros

14                     valorMulta        Number           14,2           00.00 | 0.00      Valor de multa

15                     valorMovimento    Number           14,2           00.0 | 0.00       Valor da movimentação

16                     tipoMovimento     String           CREDITO        Sem formatação    Tipo de movimentação
                                                          e/ou                             financeira.
                                                          DEBITO

17                      descMovimento       String                          Sem formatação    Descrição do movimento
                                                                                              realizado

18                      carteira            String                          Sem formatação    Tipo de carteira,
                                                                                              podendo ser:

                                                                                              A – SIMPLES
                                                                                              B – CAUCIONADA
                                                                                              C – DESCONTADA
                                                                                              D – VINCULADA
19                      agDistribuicao      String           4              Sem formatação    Agência da conta de
                                                                                              distribuição de valores
                                                                                              do associado.
20                      contaDistribuicao   String           6              Sem formatação    Conta de distribuição de
                                                                                              valores do associado.

21                      percDistribuicao    Integer                         100 | 50 | 75     Percentual de
                                                                                              distribuição de valores.

22                      valorDistribuicao   Number           14,2           00.0 | 0.00       Valor distribuição de
                                                                                              crédito.

23                      codTxId             String                                            TXID do título híbrido.
                                                                                              Esse campo só é
                                                                                              retornado em títulos
                                                                                              que possuem QRCode
                                                                                              (Boleto Híbrido)

24        total                             Long                            13                Indica a quantidade
                                                                                              total de registros
                                                                                              retornados na pesquisa.
25        pagina                            Long                           0                  Indica a página atual da
                                                                                              pesquisa.

25.2      totalPaginas                      Integer                         1                 Indica o número de
                                                                                              páginas.

25.3      quantidadePorPagina               Long             300            300               Indica a quantidade de
                                                                                              registros por página.

Status                                                              Descrição
HTTP_OK (200)                                                       Operação realizada com sucesso.

                                                       Parâmetros de entrada
              Status             Mensagem                                               Descrição
                                                          sujeitos a crítica

HTTP_UNAUTHORIZED (401)       Could not find a Parâmetro:                        Erros ocasionado por
                              required Access x-api-key                          parâmetro obrigatório
                              Token in the                                       não informado ou
                              request, identified                                inválido.
                              by HEADER x-api-
                              key
HTTP_UNAUTHORIZED (401)       UNAUTHORIZED Parâmetro:                            Erro ocasionado por
                                                  Authorization                  parâmetro obrigatório
                                                                                 não foi informado ou
                                                                                 inválido.

HTTP_UNAUTHORIZED (401)       Código de            Parâmetro:                    Erro ocasionado
                              beneficiário         codigoBeneficiario            quando o código de
                              diferente do                                       beneficiário informado
                              beneficiário do                                    é diferente do
                              usuário                                            beneficiário contido no
                                                                                 token de autenticação
                                                                                 (authorization).
HTTP_UNAUTHORIZED (401)       Cooperativa          Parâmetro:                    Erro ocasionado
                              diferente da         Cooperativa                   quando a cooperativa
                              cooperativa do                                     informada é diferente
                              usuário                                            da cooperativa contida
                                                                                 no token de
                                                                                 autenticação
                                                                                 (authorization).
HTTP_BAD_REQUEST (400)        O dia de pesquisa Parâmetro:                       Erro ocasionado
                              deve ter 10         dia                            quando a data de
                              caracteres e seguir                                pesquisa está inválida.
                              o padrão:
                              DD/MM/YYYY.
HTTP_BAD_REQUEST (400)        BAD_REQUEST         Parâmetro:                     Erro ocasionado
                                                  dia                            quando a data de
                                                                                 pesquisa não foi
                                                                                 informada.
HTTP_BAD_REQUEST (400)        O campo código Parâmetro:                          Erro ocasionado
                              do beneficiário    codigoBeneficiario              quando o código do
                              deve ter 5 (cinco)                                 beneficiário informado
                              caracteres                                         está inválido ou não foi
                              numéricos.                                         informado.

HTTP_BAD_REQUEST (400)              Documento do      Parâmetro:                       Erro ocasionado
                                   beneficiário final cpfCnpjBeneficiarioFinal         quando o cpf/cnpj do
                                   inválido.                                           beneficiário final
                                                                                       informado é inválido.

 HTTP_TOO_MANY_REQUESTS            Too many              Não se aplica                 Erro ocasionado
(429)                              requests                                            quando são enviadas
                                                                                       muitas requisições em
                                                                                       um curto espaço de
                                                                                       tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/v1/cobranca-financeiro/movimentacoes/`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“codigoBeneficiario”, “posto”: codigoBeneficiario: 12345 cooperativa: 6789 posto: 03

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/v1/cobranca-financeiro/movimentacoes/`

HEADERS:

Authorization x-api-key Content-Type: application/x-www-form-urlencoded

PARAMS:

pagina codigoBeneficiario cooperativa posto dataLancamento tipoMovimento posto

### RESPONSE BODY

```json
{
"resultado": [
{
"agencia": "9999",
"posto": "00",
"beneficiario": "99999",
"nossoNumero": "60000999",
"seuNumero": "TESTE",
"nomePagador": "MOCK TESTE",
"identPagador": "99999999999",
"dataMovimento": "2023-09-25",
"dataLancamento": "2023-09-26",
"valorNominal": 0.01,
"valorAbatimento": 0,
"valorDesconto": 0,
"valorJuros": 0,
"valorMulta": 0,
"valorMovimento": 0.01,
"tipoMovimento": "CREDITO",
"descMovimento": "LIQUIDACAO REDE",
"carteira": "SIMPLES",
"agDistribuicao": "9999",
"contaDistribuicao": "999999",
"percDistribuicao": 100,
"valorDistribuicao": 0.01,
"codTxId": "99999999999a1a11a6aaa11a1aaa1aa" – Presente somente se for título híbrido
}
],
"total": 1,
"pagina": 0,
"totalPaginas": 1,
"quantidadePorPagina": 1000
}
```

### Execução (via ferramenta Postman)

HEADERS:

PARAMS:

▪ Cenário Positivo:

▪ Cenário Negativo: 401 – UNAUTHORIZED

▪ Cenário Negativo: 400 – BAD_REQUEST

▪ Cenário Negativo: 400 – BAD_REQUEST

### 7.22. Consulta de Boletos com Distribuição de Crédito

Crédito A operação GET “Consulta de Boletos com Distribuição de Crédito” é responsável por realizar a consulta de um único boleto, através do Nosso Número, e que possua distribuição de crédito cadastrada. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – GET

### URL Sandbox (ambiente para homologação)

```text
    Ord              Nome          Tipo     Parâmetro                      Formatação        Descrição      Obrigatório

1            x-api-key          String      Header           36          UUID             Token de acesso Obrigatório
                                                                                          fornecido pelo
                                                                                          Sicredi

2            authorization      String      Header           1413        Bearer + Token Acess token         Obrigatório
                                                                         de Autenticação obtido na
                                                                                         autenticação
3            Content-Type       application/ Header                      Sem formatação Tipo conteúdo       Obrigatório
                                json
4            cooperativa        String       Header          4           Sem formatação Código da           Obrigatório
                                                                                        cooperativa do
                                                                                        beneficiário
5            Posto              String      Header           2           Sem formatação Código da           Obrigatório
                                                                                        agência do
                                                                                        beneficiário
6            codigoBeneficiario String      Param            5           Sem formatação Código do           Obrigatório
                                                                                        beneficiário
7            nossoNumero        String      Param            9           Sem formatação Nosso Número        Obrigatório

8            retornaDadosSplit String       Param            3           Domínios:         Campo que        Opcional
                                                                         - SIM            indica se os
                                                                         - NÃO            dados da
                                                                                          distribuição de
                                                                                          crédito
                                                                                          retornarão na
                                                                                          consulta.

Ord Nome                                            Tipo            Tamanho Formatação                   Descrição
1         linhaDigitavel                            String          47           Sem formatação          Linha digitável do
                                                                                                         boleto
2         codigoBarras                              String          44           Sem formatação          Código de barras
                                                                                                         do boleto

3     carteira                                 String                       Sem formatação          Tipo de carteira,
                                                                                                    podendo ser:

                                                                                                    A – SIMPLES
                                                                                                    B – CAUCIONADA
                                                                                                    C – DESCONTADA
                                                                                                    D – VINCULADA
4     seuNumero                                String         10            Sem formatação          Seu Número

5     nossoNumero                              String         9             Sem formatação          Nosso Número

6     Pagador              codigo              String         5             Sem formatação          Codigo do
                                                                                                    pagador
     (Grupo com os
     dados do              documento           String         14            Sem formatação          Documento do
     pagador)                                                                                       pagador

                           nome                String         40            Sem formatação          Nome do pagador

7     beneficiarioFinal codigo                 String         3            Sem formatação           Código do
                                                                                                    beneficiário final
     (Grupo com os
     dados do
     beneficiário          documento           String         14            Sem formatação          Documento do
     final)                                                                                         beneficiário final

     OBS: Caso não
                        nome                   String         40            Sem formatação          Nome do
     exista
                                                                                                    beneficiário final
     beneficiário final
     vinculado ao
     boleto, o grupo
     não será
     exibido.
8    dataEmissao                               String         10            YYYY-MM-DD              Data de emissão
                                               (date)                                               do boleto

9     dataVencimento                           String         10            YYYY-MM-DD              Data de
                                               (date)                                               vencimento do
                                                                                                    boleto
10    valorNominal                             Bigdecimal     14,2          Sem formatação          Valor nominal do
                                                                                                    boleto

11    situação                         String                       Sem formatação        Situação do
                                                                                          boleto, podendo
                                                                                          ser:

                                                                                          - EM CARTEIRA
                                                                                          - EM CARTEIRA
                                                                                          PIX
                                                                                          - VENCIDO
                                                                                          - LIQUIDADO
                                                                                          - LIQUIDADO
                                                                                          CARTORIO
                                                                                          - LIQUIDADO
                                                                                          REDE
                                                                                          - LIQUIDADO
                                                                                          COMPE
                                                                                          - LIQUIDADO PIX
                                                                                          - LIQUIDADO
                                                                                          CHEQUE
                                                                                          - BAIXADO POR
                                                                                          SOLICITACAO
                                                                                          - PROTESTADO
                                                                                          - EM CARTORIO
                                                                                          - NEGATIVADO
                                                                                          - AGUARDANDO
                                                                                          ENTRADA EM
                                                                                          CARTORIO
                                                                                          - AGUARDANDO
                                                                                          SUSTACAO DE
                                                                                          CARTORIO
                                                                                          - REJEITADO
12    txId                             String                       Sem formatação        Identificador da
                                                                                          transação Pix

13    codigoQrCode                     String                       Sem formatação        QRCODE do
                                                                                          boleto híbrido

14    multa                            Number         14,2          Sem formatação        Percentual de
                                                                                          multa do boleto

15    abatimento                       Number         14,2          Sem formatação        Valor de
                                                                                          abatimento
                                                                                          concedido do
                                                                                          boleto

16    tipoJuros                                      String         14            Domínios:              Tipo de juros do
                                                                                  A - VALOR              boleto
                                                                                  B - PERCENTUAL

17    juros                                          Number         14,2          Sem formatação         Valor/percentual
                                                                                                         de juros do
                                                                                                         boleto

18    diasProtesto                                   Integer        2             Sem formatação         Quantidade de
                                                                                                         dias para protesto
                                                                                                         automático do
                                                                                                         boleto
19    validadeAPosVencimento                         Integer        4             Sem formatação         Validade após
                                                                                                         vencimento

20    diasNegativacao                                Integer        2             Sem formatação         Dias para
                                                                                                         negativação do
                                                                                                         boleto

21    tipoDesconto                                   String         14            Domínios:              Tipo de desconto
                                                                                                         do boleto,
                                                                                  A - VALOR              podendo ser:
                                                                                  B - PERCENTUAL
                                                                                                         A - VALOR
                                                                                                         B - PERCENTUAL
22    descontoAntecipacao                            Number         14,2          Sem formatação         Valor do desconto
                                                                                                         antecipado do
                                                                                                         boleto

23    descontos                numeroOrdem           Integer        1            Domínios:                Número da
                                                                                                         ordem, podendo
     (Lista com até 3                                                            1                       ser:
     descontos)                                                                  2
                                                                                 3                       1- Desconto 1
     Obs: Caso não                                                                                       2- Desconto 2
     exista desconto                                                                                     3- Desconto 3
     cadastrado, a

     lista retorna             valorDesconto              Bigdecimal     14,2           "valorDesconto": 0.0   Valor/percentual
     vazia:                                                                                                    de desconto

     "descontos": []

                               dataLimite                 String(date) 10             "dataLimite": "YYYY-     Data limite do
                                                                                      MM-DD"                   desconto

24   retornaDadosSplit                                    String         3             Domínios:            Indica se o boleto
                                                                                                            possui
                                                                                      SIM                   distribuição de
                                                                                      NAO                   crédito
24    dadosLiquidacao data                                String(date)                YYYY-MM-              Data e hora da
                                                                                      DD'T':HH:mm:ss.SSS'Z' liquidação do
     (Grupo que                                                                                             boleto
     retorna os
                               valor                      Bigdecimal     14,2          Sem formatação          Valor da
     valores de
                                                                                                               liquidação do
     liquidação do
                                                                                                               boleto
     boleto)
                               multa                      Bigdecimal     14,2          Sem formatação          Valor da multa
     OBS: Esse grupo                                                                                           cobrado na
     só será exibido                                                                                           liquidação
     caso o boleto
     esteja liquidado.         abatimento                 Bigdecimal     14,2          Sem formatação          Valor do
                                                                                                               abatimento
                                                                                                               concedido na
                                                                                                               liquidação
                               juros                      Bigdecimal     14,2          Sem formatação          Valor de juros
                                                                                                               cobrado na
                                                                                                               liquidação

                               desconto                   Bigdecimal     14,2          Sem formatação          Valor do desconto
                                                                                                               concedido na
                                                                                                               liquidação

25    dadosSplit                                          String         24            Sem formatação          Situação do
                                                                                                               cadastro da
     (Grupo com os             situacaoCadastro                                                                distribuição de
     dados da                                                                                                  crédito, podendo
                                                                                                               ser:

distribuição de                                                                                          - CADASTRADO
crédito)                                                                                                 COM SUCESSO
                                                                                                         - AGUARDANDO
                                                                                                         CREDITO
                                                                                                         - ENCAMINHADO
                                                                                                         PARA CREDITO
                          valorBoleto               Number         14,2          Sem formatação          Valor do boleto

                          dataLiquidacaoSplit       String         10            YYYY-MM-DD              Data de
                                                                                                         liquidação do
                                                                                                         boleto

                      valorLiquidacaoBoleto         Number         14,2          Sem formatação          Valor de
                                                                                                         liquidação do
                                                                                                         boleto

                      tipoTransferencia             String         3             Sem formatação          Tipo de
                                                                                                         transferência,
                                                                                                         podendo ser:

                                                                                                         - TED
                          regraRateio               String         31            Sem formatação          Tipo de regra do
                                                                                                         rateio, podendo
                                                                                                         ser:

                                                                                                         - VALOR
                                                                                                         COBRADO
                                                                                                         - VALOR DO
                                                                                                         REGISTRO
                                                                                                         - MENOR VALOR:
                                                                                                         REGISTRADO OU
                                                                                                         PAGO
                          tipoValorRateio           String         10            Sem formatação          Tipo do valor de
                                                                                                         rateio, podendo
                                                                                                         ser:

                                                                                                         - VALOR
                                                                                                         - PERCENTUAL
                          tipoRepasse               String         10            Sem formatação          Tipo do repasse,
                                                                                                         podendo ser:

                                                                                                         - MANUAL
                                                                                                         - AUTOMATICO

 parcelas                 parcelaRateio           Integer        2             Sem formatação        Número da
                                                                                                     parcela da
(Grupo com os                                                                                        distribuição de
dados de cada                                                                                        crédito
parcela de                valorPercentualRateio   Number         14,2          Sem formatação        Valor/Percentual
distribuição de                                                                                      da distribuição de
crédito)                                                                                             crédito

                          codigoBanco             String         3             Sem formatação        Código do banco

                          codigoAgencia           String         4             Sem formatação        Código da
                                                                                                     agência

                          numeroContaCorrente String             13            Sem formatação        Número da conta
                                                                                                     corrente

                          numeroCpfCnpj           String         14            Sem formatação        CPF/CNPJ do
                                                                                                     destinatário

                          nomeDestinatario        String         40            Sem formatação        Nome do
                                                                                                     destinatário

                          floatRepasse            Integer        2             Sem formatação        Float do repasse
                                                                                                     da distribuição do
                                                                                                     crédito do boleto

                          statusRepasseParcela    String         36            Sem formatação        Status do repasse
                                                                                                     da parcela da
                                                                                                     distribuição do
                                                                                                     crédito do boleto,
                                                                                                     podendo ser:

                                                                                                     1) AGUARDANDO
                                                                                                     LIQUIDACAO
                                                                                                     BOLETO
                                                                                                     2) CANCELADO
                                                                                                     PELO USUARIO

                                                                                                           3) CANCELADO -
                                                                                                           ERRO CALCULO
                                                                                                           SPLIT
                                                                                                           4) AGUARDANDO
                                                                                                           PRAZO FLOAT
                                                                                                           5) AGUARDANDO
                                                                                                           LIBERACAO
                                                                                                           BENEFICIARIO
                                                                                                           6) CANCELADO -
                                                                                                           PRAZO
                                                                                                           LIBERACAO
                                                                                                           EXCEDIDO
                                                                                                           7) ENVIADO PARA
                                                                                                           EFETIVACAO
                                                                                                           8) REPASSE
                                                                                                           EFETIVADO
                                                                                                           9) CANCELADO -
                                                                                                           ERRO AO
                                                                                                           EFETIVAR
                                                                                                           CREDITO
                              dataPrevistaRepasse     String         10            YYYY-MM-DD              Data prevista
                                                                                                           para ser realizado
                                                                                                           o repasse

                              valorRepasse            Number         14,2          Sem formatação          Valor do repasse

                              dataEfetivacaoRepasse String           10            YYYY-MM-DD              Data da
                                                                                                           efetivação do
                                                                                                           repasse

                              informacaoAdicional     String         50            Sem formatação          Informação
                                                                                                           adicional. Será
                                                                                                           utilizado em
                                                                                                           situações que o
                                                                                                           crédito não foi
                                                                                                           efetivado.

Status                                                              Descrição
HTTP_OK (200)                                                       Operação realizada com sucesso

                                                      Parâmetros de
               Status              Mensagem              entrada                 Descrição
                                                     sujeitos a crítica
HTTP_UNAUTHORIZED (401)        Could not find a     Parâmetro:          Erros ocasionado por
                               required Access      x-api-key           parâmetro obrigatório não
                               Token in the                             informado ou inválido.
                               request, identified
                               by HEADER x-api-
                               key
HTTP_UNAUTHORIZED (401)        UNAUTHORIZED Parâmetro:                     Erro ocasionado por
                                                   Authorization           parâmetro obrigatório não
                                                                           foi informado ou inválido.

HTTP_UNAUTHORIZED (401)        Código de            Parâmetro:         Erro ocasionado quando o
                               beneficiário         codigoBeneficiario código de beneficiário
                               diferente do                            informado é diferente do
                               beneficiário do                         beneficiário contido no
                               usuário                                 token de autenticação
                                                                       (authorization).
HTTP_UNAUTHORIZED (401)        Cooperativa          Parâmetro:         Erro ocasionado quando a
                               diferente da         Cooperativa        cooperativa informada é
                               cooperativa do                          diferente da cooperativa
                               usuário                                 contida no token de
                                                                       autenticação
                                                                       (authorization).
HTTP_NOT_FOUND (404)           Titulo não           Parâmetro:         Erro ocasionado quando
                               encontrado           nossoNumero        não existe resultado para a
                                                                       consulta informada.

HTTP_BAD_REQUEST (400)       O campo nosso     Parâmetro:                  Erro ocasionado quando o
                             numero deve ter nossoNumero                   parâmetro foi preenchido
                             apenas 9 (nove)                               com menos ou mais de 9
                             caracteres                                    caracteres.
                             numéricos.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica               Erro ocasionado quando
                                                                           são enviadas muitas
                                                                           requisições em um curto
                                                                           espaço de tempo.

HTTP_BAD_REQUEST (400)              O campo           Parâmetro:        Erro ocasionado por
                                    retornaDadosSplit retornaDadosSplit parâmetro ser preenchido
                                    deve ser SIM ou                     com dados inválidos.
                                    NAO.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v2/boletos`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto”, “codigoBeneficiario” e “nossoNumero”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v2/boletos`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto

PARAMS:

codigoBeneficiario nossoNumero retornaDadosSplit

### RESPONSE BODY

```json
{
"linhaDigitavel": "74891160090066690434710123451009194270000100000",
"codigoBarras": "74891942700001000001160000666904341012345100",
"carteira": "SIMPLES",
"seuNumero": "TESTE",
"nossoNumero": "666666666",
"pagador": {
"codigo": "YOHI2",
"documento": "85355592000117",
"nome": "TESTE SPLIT"
},
"beneficiarioFinal": {
"codigo": "0EN",
"documento": "25140124068",
"nome": "TESTE SPLIT"
},
"dataEmissao": "2023-05-30",
"dataVencimento": "2023-07-30",
"valorNominal": 1000,
"situacao": "EM CARTEIRA",
"multa": 3,
"abatimento": 0,
"tipoJuros": "A - VALOR",
"juros": 5,
"diasProtesto": 0,
"validadeAposVencimento": 120,
"diasNegativacao": 0,
"tipoDesconto": "A - VALOR",
"descontoAntecipacao": 0,
"descontos": [
{
"numeroOrdem": 1,
"valorDesconto": 10,
"dataLimite": "2023-07-15"
},
{
"numeroOrdem": 2,
"valorDesconto": 7,
"dataLimite": "2023-07-20"
},
{
"numeroOrdem": 3,
"valorDesconto": 3,
"dataLimite": "2023-07-30"
}
],
"boletoComSplit": "SIM",
"dadosSplit": {
"situacaoCadastro": "SPLIT_CADASTRADO",
"valorBoleto": 1000,
"dataLiquidacaoBoleto": "",
"valorLiquidacaoBoleto": null,
"tipoTransferencia": "TED",
"regraRateio": "VALOR_COBRADO",
"tipoValorRateio": "PERCENTUAL",
"tipoRepasse": "AUTOMATICO",
"parcelas": [
{
"parcelaRateio": 1,
"valorPercentualRateio": 3,
"codigoBanco": "237",
"codigoAgencia": "0434",
"numeroContaCorrente": "2323232323",
"numeroCpfCnpj": "02738306004",
"nomeDestinatario": "TESTE SPLIT",
"floatRepasse": 20,
"statusRepasseParcela": "AGUARDANDO_LIQUIDACAO_BOLETO",
"dataPrevistaRepasse": "",
"valorRepasse": null,
"dataEfetivacaoRepasse": ""
}
]
}
Execução (via ferramenta Postman)
Headers:
Params:
Cenário Positivo:
Cenário Negativo: 401 – UNAUTHORIZED
Cenário Negativo: 400 – BAD_REQUEST
```

### 7.23. Cancelar Parcelas de Distribuição de Crédito de um Boleto

Crédito de um Boleto A operação PATCH “Cancelar Parcelas de Distribuição de Crédito” é responsável por realizar o cancelamento de uma ou mais parcelas da distribuição de crédito de um boleto. O formato de entrada e

saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
Ord           Nome              Tipo   Parâmetro                     Formatação             Descrição   Obrigatório

1     x-api-key             String     HeaderPar      36          UUID            Token de       Obrigatório
                                       am                                         acesso
                                                                                  fornecido pelo
                                                                                  Sicredi
2     authorization         String     Header         1413        Bearer + Token Acess token Obrigatório
                                                                  de Autenticação obtido na
                                                                                  autenticação
3     Content-Type          applicatio Header                     Sem formatação Tipo conteúdo Obrigatório
                            n/json
4     cooperativa           String     Header         4           Sem formatação Código da      Obrigatório
                                                                                 cooperativa do
                                                                                 beneficiário
5     Posto                 String     Header         2           Sem formatação Código da      Obrigatório
                                                                                 agência do
                                                                                 beneficiário
6     codigoBeneficiario String        Body           5           Sem formatação Código do      Obrigatório
                                                                                 beneficiário
7     nossoNumero           String     Body           9           Sem formatação Nosso Número Obrigatório

8     numeroParcelas        Integer    Body           2           Sem formatação Número das             Obrigatório
                                                                                 parcelas que
                                                                                 serão
                                                                                 canceladas.
                                                                                 Caso não
                                                                                 informe pelo
                                                                                 menos uma,
                                                                                 todas serão
                                                                                 canceladas.

Ord        Nome                 Tipo           Tam             Formatação Descrição
1          parcelasCanceladas   Integer        2               Sem             Número das parcelas que foram
                                                               formatação      canceladas.

 Status                                                         Descrição
 HTTP_OK (200)                                                  Operação realizada com sucesso

                                                              Parâmetros de
                 Status                   Mensagem               entrada                 Descrição
                                                             sujeitos a crítica
HTTP_UNAUTHORIZED (401)                Could not find a     Parâmetro:          Erros ocasionado por
                                       required Access      x-api-key           parâmetro obrigatório não
                                       Token in the                             informado ou inválido.
                                       request, identified
                                       by HEADER x-api-
                                       key
HTTP_UNAUTHORIZED (401)                UNAUTHORIZED Parâmetro:                     Erro ocasionado por
                                                           Authorization           parâmetro obrigatório não
                                                                                   foi informado ou inválido.

HTTP_BAD_REQUEST (400)                 O campo numero numeroParcelas               Erros ocasionado por
                                       parcelas deve                               parâmetro inválido.
                                       conter uma lista de
                                       caracteres
                                       numéricos.
HTTP_NOT_FOUND (404)                   Boleto não          Não se aplica           Boleto não foi encontrado.
                                       encontrado.

HTTP_UNPROCESSABLE_ENTITY                   Boleto sem            Não se aplica          O boleto não possui
(422)                                       parcelas aptas a                             parcelas aptas para
                                            cancelar o split.                            cancelamento.

HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                             Erro ocasionado quando
                                                                                         são enviadas muitas
                                                                                         requisições em um curto
                                                                                         espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/split/cancelar-split`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/split/cancelar-split`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto

BODY:

```json
{
"codigoBeneficiario": "12345",
"nossoNumero": " 600001234 ",
"numeroParcelas": [ "01", "02", "03" ]
}
```

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo: Informando as parcelas que seja cancelar.

Cenário Positivo: Sem enviar o campo “numeroParcelas”. Portanto, todas serão canceladas.

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 404 – NOT_FOUND

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

### 7.24. Liberar Repasse de Distribuição de Crédito de um Boleto

Crédito de um Boleto A operação PATCH “Liberar Repasse de Distribuição de Crédito” é responsável por liberar o repasse do crédito das parcelas de um boleto com Split. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### Entrada

Tipo de Tamanho

### Saída

### RETORNO (Response)

### FALHAS

### REQUEST – PATCH

### URL Sandbox (ambiente para homologação)

```text
Ord          Nome               Tipo   Parâmetro                   Formatação             Descrição   Obrigatório

1     x-api-key             String     HeaderPar    36          UUID                Token de       Obrigatório
                                       am                                           acesso
                                                                                    fornecido pelo
                                                                                    Sicredi

2       authorization         String     Header           1413       Bearer + Token Acess token          Obrigatório
                                                                     de Autenticação obtido na
                                                                                     autenticação
3       Content-Type          applicatio Header                      Sem formatação Tipo conteúdo Obrigatório
                              n/json
4       cooperativa           String     Header           4          Sem formatação Código da      Obrigatório
                                                                                    cooperativa do
                                                                                    beneficiário
5       Posto                 String     Header           2          Sem formatação Código da      Obrigatório
                                                                                    agência do
                                                                                    beneficiário
6       codigoBeneficiario String        Body             5          Sem formatação Código do      Obrigatório
                                                                                    beneficiário
7       nossoNumero           String     Body             9          Sem formatação Nosso Número Obrigatório

Ord          Nome                      Tipo           Tam             Formatação Descrição
1            parcelasRepasse           Integer        2               Sem             Número das parcelas de
                                                                      formatação      distribuição de crédito que
                                                                                      foram liberadas.

    Status                                                             Descrição
    HTTP_OK (200)                                                      Operação realizada com sucesso

                                                                      Parâmetros de
                   Status                         Mensagem               entrada                 Descrição
                                                                     sujeitos a crítica
HTTP_UNAUTHORIZED (401)                       Could not find a      Parâmetro:          Erros ocasionado por
                                              required Access       x-api-key           parâmetro obrigatório não
                                              Token in the                              informado ou inválido.
                                              request, identified
                                              by HEADER x-api-
                                              key

HTTP_UNAUTHORIZED (401)            UNAUTHORIZED          Parâmetro:             Erro ocasionado por
                                                         Authorization          parâmetro obrigatório não
                                                                                foi informado ou inválido.

HTTP_BAD_REQUEST (400)             O campo código do codigoBeneficiario Erros ocasionado por
                                   beneficiario deve                    parâmetro inválido ou não
                                   ter 5 (cinco)                        informado.
                                   caracteres
                                   numéricos.
HTTP_BAD_REQUEST (400)             O campo nosso     nossoNumero        Erros ocasionado por
                                   numero deve ter                      parâmetro inválido ou não
                                   apenas 9 (nove)                      informado.
                                   caracteres
                                   numéricos.
HTTP_NOT_FOUND (404)               Boleto não        Não se aplica      Boleto não foi encontrado.
                                   encontrado.

HTTP_UNPROCESSABLE_ENTITY          BOLETO COM     Não se aplica                 O boleto não possui
(422)                              REPASSE MANUAL                               parcelas de distribuição
                                   JÁ LIBERADO.                                 aptas para liberar o
                                                                                repasse.
HTTP_TOO_MANY_REQUESTS (429) Too many requests Não se aplica                    Erro ocasionado quando
                                                                                são enviadas muitas
                                                                                requisições em um curto
                                                                                espaço de tempo.

```

`https://api-parceiro.sicredi.com.br/sb/cobranca/boleto/v1/boletos/split/liberar-repasse`

> **OBS:** Para validações no ambiente Sandbox, utilizar os seguintes valores para os campos “cooperativa”,

“posto” e “codigoBeneficiario”: cooperativa : 6789 posto: 03 codigoBeneficiario: 12345

### URL Produção

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/boletos/split/liberar-repasse`

HEADERS:

Authorization x-api-key Content-Type: application/json Cooperativa posto

BODY:

```json
{
"codigoBeneficiario": "12345",
"nossoNumero": " 600001234 ",
}
```

### Execução (via ferramenta Postman)

Headers:

Body:

Cenário Positivo:

Cenário Negativo: 400 – BAD_REQUEST

Cenário Negativo: 404 – NOT_FOUND

Cenário Negativo: 422 – UNPROCESSABLE_ENTITY

## 8. Geração Nosso Número

Se não for enviado o campo “nossoNumero” será retornado um “nossoNumero” gerado automaticamente pelo webservice EComm-API. Caso seja enviado o campo “nossoNumero” na emissão do boleto, segue a regra para a criação: REGRA PARA GERAÇÃO DO NOSSO NÚMERO: AABnnnnnD => formato do nossoNumero onde AA é o ano, B é o indicador de geração do nosso número Byte, nnnnn é o sequencial e “D” é o dígito verificador. Exemplo:

```text
      18 - Ano atual
      2 a 9 - byte de geração
      nnnnn - número sequencial (livre escolha, de 00000 a 99999).
      D - Dígito verificador calculado
      ou seja, a nomenclatura correta é: 18200001-D

Fórmula para cálculo do dígito verificador pelo módulo 11:
a) Relacionar os códigos da cooperativa de crédito/agência do beneficiário (aaaa), posto do
beneficiário(pp), código do beneficiário
   (ccccc), ano atual (yy), indicador de geração do nosso número Byte(b) e o número sequencial do
beneficiário
(nnnnn): aaaappcccccyybnnnnn;
                Caso o posto do beneficiário(pp) seja alfanumérico ele deverá ser informado como “00”,
possibilitando assim o cálculo do DV do Nosso Número.

b) atribuir os pesos (de 2 a 9) correspondentes para cada dígito, começando da direita para a esquerda,
efetuando cada multiplicação:

c) somar o resultado de cada multiplicação;
d) dividir o resultado da soma por 11 (onze);
e) identificar o resto da divisão;
f) dígito verificador será o resultado da subtração: 11 - resto da divisão. Se o resultado da subtração for
10 (dez) ou 11 (onze), o dígito verificador será 0 (zero).
Exemplo:
cooperativa: 0100
posto: 02
cedente: 00248
ano: 18
byte da geração: 2 (nosso número gerado pelo cedente)
número sequencial: 00001

Cálculo para encontrar o DV do “Nosso Número”:

Somatório do resultado de cada multiplicação = 142
 142 / 11 = 12,9091
 12 x 11 = 132
142 - 132 = 10
 11 - 10 =          1 (Caso o resultado seja 10 ou 11, o DV será 0)
DV = 1
Exemplo do Nosso Número gerado = 182000011

```

## 9. Layout do Boleto

Caso o beneficiário opte por gerar a impressão do boleto, deverá providenciar a emissão do mesmo de acordo com o layout especificado pelo Sicredi, conforme abaixo:

## 10. Glossário

Beneficiário: Empresa ou pessoa associada do Sicredi que contratou o produto Cobrança com a sua cooperativa.

Código de Beneficiário / Código de convênio: Código gerado no momento da contratação do produto Cobrança na sua agência.

Beneficiário final: O beneficiário final (em substituição ao termo SACADOR/AVALISTA), quando indicado no boleto, representa o destinatário final do direito que receberá os recursos provenientes da liquidação do boleto.

Pagador: É a pessoa física ou jurídica titular da dívida. O banco recebe o pagamento feito pelo sacado e transfere o valor pago para a conta do beneficiário.

Número da cooperativa: É o número da cooperativa a qual pertence a conta vinculada ao convênio de cobrança da pessoa associada. Esse número é o qual junto com o número de conta o associado pode logar no seu internet banking.

Número de posto: É o número da agência a qual pertence a conta vinculada ao convênio de Cobrança da pessoa associada. Você pode encontrar essa informação acessando (ou solicitando à empresa associada que acesse) o internet banking, no menu “Cobrança”, na opção ‘Cadastrar títulos’. Ela encontrará a seguinte informação:

```text
                                     1234 46 12346   NOME DA EMPRESA

                                                                Código de beneficiário
                                                        Posto
                                          Cooperativa

Situações dos boletos: o boleto pode ter diversas situações, são elas:
             o    TODOS
             o    EM_CARTEIRA: boleto disponível para pagamento, ainda não foi pago (pode ou não estar dentro do
                  vencimento).
             o    LIQUIDADO: boleto já foi pago pelo pagador.
             o    BAIXADO_SOLICITACAO: Baixa solicitada pelo associado via comando de instrução ‘Baixa’.
                  Normalmente a baixa ocorre quando o associado recebeu o pagamento da dívida de alguma outra
                  forma e deseja que o boleto não esteja mais apto para pagamento.
             o    PROTESTADO: Boleto foi protestado em cartório, por solicitação do associado via comando de
                  instrução ‘Solicitação de Protesto’ ou por protesto automático.
             o    VENCIDO: Boleto está vencido e não foi pago ainda.
             o    EM_CARTORIO: Associado solicitou o protesto
             o    AGUARDANDO_ENTRADA_CARTORIO: Solicitação de protesto feita, mas o boleto ainda não foi
                  enviado ao cartório.
             o    AGUARDANDO_SUSTACAO_CARTORIO: Solicitação para sustar (cancelar) o protesto foi feita,
                  aguardando processamento pelo cartório.

            o    REJEITADO: Boleto não confirmado devido à existência de críticas (erros ou inconsistências
                 identificadas no processo).
            o    NEGATIVADO: Boleto foi incluído em cadastro de inadimplentes (negativação concluída).
            o    AGUARDANDO_NEGATIVACAO: Solicitação de negativação feita, mas ainda não processada.
            o    AGUARDANDO_RETIRADA_NEGATIVACAO: Solicitação para retirar a negativação feita, aguardando
                 processamento.

```

## 11. Fluxos

### 11.1. Diagrama de Sequência de chamadas realizadas para o serviço

A integração com o produto o Webhook Cobrança é realizado em duas etapas distintas que serão descritas neste manual.

1ª Etapa: Desenvolvimento de uma API para Recebimento de Eventos Webhook: É uma API desenvolvida pelo associado, responsável por receber os eventos (movimentações) dos títulos via webhook.

2ª Etapa: Contratação do produto via API de contratação informando a URL desenvolvida na etapa acima.

API associado para Recebimentos de eventos Webhook

## 12. Apresentação API Recebimento de eventos

Recebimento de eventos A API de Recebimento de eventos Webhook é uma API que deve ser desenvolvida pelo associado com o objetivo de se comunicar e realizar uma integração com a API Webhook do Sicredi, a fim de receber os eventos sobre as movimentações que ocorrem em seus títulos de cobrança.

## 13. Objetivo

Este manual destina-se às pessoas responsáveis pelo desenvolvimento e adaptação dos sistemas das empresas associadas ao Sicredi que desejam utilizar o Webhook para receber eventos de movimentações refere aos seus títulos de cobrança. O objetivo desse manual é orientar os desenvolvedores na construção de uma api para o Recebimentos de eventos que deve estar preparada para receber uma requisição POST, realizada por uma API do Sicredi, que irá conter os dados de uma determinada movimentação do título (evento). Segue abaixo uma ilustração desse processo:

## 14. Orientações Iniciais

Para iniciar o processo desenvolvimento de sua API de Recebimentos de eventos Webhook e promover a integração sistêmica, você deve seguir as orientações abaixo:

▪ O Protocolo de comunicação deve ser HTTPS;

▪ Sua API deve utilizar protocolo TLS 1.2 nas requisições;

▪ O certificado de sua API não pode ser auto assinado;

▪ A API Sicredi possui um timeout padrão de 10 segundos por requisição. Dessa forma, sua API de Recebimentos de eventos deverá responder o request em até 10 segundos. Em caso de ausência de resposta no tempo estipulado, o evento será marcado como “NÃO ENTREGUE”;

▪ O código de retorno HTTP de esperado pela API de Recebimentos de eventos é 200;

▪ Nesse primeiro momento, sua API de Recebimentos de eventos não deve possuir qualquer tipo de autenticação. Estamos no processo de discussão e implementação de uma autenticação via certificado de segurança que ainda não foi finalizado;

▪ Ao fim do processo de desenvolvimento da API de Recebimentos de eventos, o associado deverá informar ao Sicredi o endpoint (URL) de sua API para que a API Sicredi consiga iniciar o processo de integração.

## 15. Quais as funcionalidades?

A API de Recebimentos de eventos deverá conter as seguintes funcionalidades:

▪ POST - Recebimento de Eventos: Método que irá ser chamado via API Sicredi para o recebimento dos eventos (movimentações). Neste primeiro os eventos disponibilizados são: o LIQUIDACAO_PIX: Liquidações efetuadas via PIX. o LIQUIDACAO_REDE **: Liquidações efetuadas nos canais Sicredi.

o LIQUIDACAO_COMPE_H5: Liquidações efetuadas em outras instituições (Fora da Rede SICREDI) o LIQUIDACAO_COMPE_H6: Liquidações com valor superior a 250 mil efetuadas em outras instituições (Fora da Rede SICREDI) o LIQUIDACAO_COMPE_H8: Recebimento de liquidação fora da Rede SICREDI- Contingência (Alguma inconsistência de sistema). o LIQUIDACAO_CARTORIO: Liquidações efetuadas em cartórios. o ESTORNO_LIQUIDACAO_REDE: Liquidações que foram estornadas.

** Observação LIQUIDAÇÃO_REDE: Essa liquidação é a única que pode ter estorno no mesmo dia. Havendo estorno será enviado o evento “ESTORNO_LIQUIDACAO_REDE”. Fica a critério do associado acatar de imediato a liquidação ou aguardar o fim do dia para garantir que não ocorreu um estorno.

## 16. Parâmetros API de Recebimentos de eventos

Recebimentos de eventos Para iniciar a integração, a API Sicredi irá encapsular os dados do evento descritos abaixo e irá realizar uma chamada, utilizando a URL disponibilizada pelo associado, em sua API de Recebimentos de eventos Webhook, que deve estar preparada para receber via POST uma requisição com os seguintes parâmetros:

2. Exemplo JSON dos parâmetros:

```json
{
"agencia": "9999",
"posto": "99",
"beneficiario": "12345",
"nossoNumero": "221000144",
"dataEvento":[2024,3,20,11,40,39,24000000]
"movimento": "LIQUIDACAO_PIX",
"valorLiquidacao": "101.01",
"valorDesconto": "0",
"valorJuros": "0",
"valorMulta": "0",
"valorAbatimento": "0",
“carteira”: “CARTEIRA SIMPLES”
"dataPrevisaoPagamento":[2024,3,20]
"idEventoWebhook": "N000000000000000000000000000000LIQUIDACAO_PIX"
}
```

### ENTRADA

### RETORNO (Response)

```text
Ord             Nome              Tipo       Tamanho              Formatação                      Descrição                Obrigatório

1      agencia                  String   4              Sem formatação                      Código da                  Obrigatório
                                                                                            Cooperativa

2      posto                    String   2              Sem formatação                      Código do Posto            Obrigatório

3      beneficiario             String   5              Sem formatação                      Código do                  Obrigatório
                                                                                            Convênio de

4      nossoNumero              String   9              Sem formatação                      Nosso número do título     Obrigatório

5    dataEvento               Array    40               [YYYY,MM,DD,00,00,00                Data e hora que ocorreu    Obrigatório
                              (Date)                    .000]                               o evento

6    movimento                String   753                Domínios:                         Lista de eventos que o     Obrigatório
                                                          "LIQUIDACAO_PIX",                 associado receberá
                                                          "LIQUIDACAO_COMPE_                informações em caso
                                                          H5",                              de movimentação.
                                                          "LIQUIDACAO_COMPE_
                                                          H6",                              "LIQUIDACAO_REDE"
                                                          "LIQUIDACAO_COMPE_                *Obs: Essa liquidação
                                                          H8",                              é a única que pode ter
                                                          * "LIQUIDACAO_REDE",              estorno no mesmo
                                                          "LIQUIDACAO_CARTORI               dia. Havendo estorno
                                                          O"                                será enviado o evento
                                                          "ESTORNO_LIQUIDACA                “ESTORNO_LIQUIDAC
                                                          O_REDE"                           AO
                                                                                            _REDE”.
                                                                                            Fica a critério do
                                                                                            associado acatar de
                                                                                            imediato a liquidação
                                                                                            ou aguardar o fim do
                                                                                            dia para garantir que
                                                                                            não     ocorreu   um
                                                                                            estorno.

7    valorLiquidacao          String   14,2                Sem formatação                   Valor da liquidação        Obrigatório

8    valorDesconto            String   14,2                Sem formatação                   Valor de desconto          Obrigatório
                                                                                            aplicado na liquidação.

9    valorJuros               String   14,2                Sem formatação                   Valor de juros aplicado    Obrigatório
                                                                                            na liquidação

10    valorMulta               String   5,2                 Sem formatação                   Valor de multa aplicado    Obrigatório
                                                                                             na liquidação

11    valorAbatimento          String   14,2                Sem formatação                   Valor de abatimento        Obrigatório
                                                                                             aplicado na liquidação

12    carteira                 String   30               Domínios:                           Tipo de carteira do        Opcional
                                                                                             título
                                                         CARTEIRA SIMPLES
                                                         CARTEIRA CAUCIONADA
                                                         CARTEIRA DESCONTADA
                                                         CARTEIRA VINCULADA
13    dataPrevisaoPagam        Array    40               [YYYY,MM,DD]                        Data de prevista para o    Opcional
      ento                     (Date)                                                        crédito da liquidação

14    idEventoWebhook          String   50                  Sem formatação                   Código de identificação    Obrigatório
                                                                                             único de cada evento.
                                                                                             Cada evento possui um
                                                                                             identificador único,
                                                                                             representado por esse
                                                                                             parâmetro.
15    idTituloEmpresa          String   25                  Sem formatação                   ID de controle do          Opcional
                                                                                             associado (semelhante
                                                                                             ao seuNumero)
                                                                                             informado no momento
                                                                                             do cadastro do título.
                                                                                             (item 32, tópico
                                                                                             ‘Cadastro de Boletos’)

                                                                                             (Obs: Só será enviada
                                                                                             essa informação no
                                                                                             evento de liquidação se
                                                                                             o título possuir essa
                                                                                             informação e o
                                                                                             associado tiver optado
                                                                                             por receber essa

                                                                                 informação no
                                                                                 momento da
                                                                                 contratação/alteração.)

         Código                                                        Descrição
HTTP_OK (200)                                            Operação realizada com sucesso

                           API de Contratação Webhook

```

## 17. Apresentação API de Contratação Webhook

Contratação Webhook A API de Contrato Webhook realiza a criação de um contrato para os beneficiários que desejam utilizar este produto, que tem como finalidade o recebimento de eventos sobre as movimentações que ocorrem em seus títulos de cobrança. O Webhook é um recurso que possibilita o envio de dados em tempo real entre dois sistemas. Através do Webhook, diferentes sistemas podem ser integrados para que os eventos entre produtor e cliente permaneçam sincronizados.

## 18. Para quem?

São variados os tipos de negócios que podem empregar este tipo de recebimento. Negócios representados por uma Pessoa Jurídica, seja do ramo da indústria, comércio, prestação de serviço, agronegócio, Governo*, não importando esferas municipais, estaduais ou federal, da administração direta ou indireta, ou Pessoa Física, profissional liberal, produtor rural ou autônomo, que precisem receber recursos oriundos de uma oferta, proposta, venda ou serviço prestado.

## 19. Objetivo

Este manual destina-se às pessoas responsáveis pelo desenvolvimento e adaptação dos sistemas das empresas associadas ao Sicredi que desejam realizar contratação do Webhook e receber os eventos de movimentações dos títulos através da integração via API.

## 20. Orientações Iniciais

Para iniciar o processo de contratação do Webhook, o associado deve possuir um convênio ATIVO com o produto Cobrança do Sicredi e possuir a modalidade Cobrança Online.

Logo, para promover a integração sistêmica de registro on-line de Boletos a empresa ou pessoa associada inicialmente precisa:

▪ Ter contratado o produto Cobrança através da sua cooperativa ▪ Ter optado pela modalidade API (Cobrança Online); ▪ Ter o termo de contratação/adesão assinado;

## 21. Quais as funcionalidades?

A API de Contratação Webhook contém as seguintes funcionalidades:

▪ Autenticação: Operação responsável pela autenticação do código de acesso e criação do token de autenticação, que permitirá acessar a API de Contratação Webhook.

▪ Cadastro de Contrato Webhook: Operação responsável pela criação de um contrato para utilização do Webhook. Cada associado poderá possuir apenas 1 contrato por código de beneficiário e cooperativa. Nesse primeiro momento, está disponível a contratação de apenas 1 evento:

o LIQUIDAÇÃO: Onde o associado receberá informações de qualquer movimentação relacionada as liquidações de seus boletos, sejam elas: o LIQUIDACAO_PIX: Liquidações efetuadas via PIX. o LIQUIDACAO_REDE **: Liquidações efetuadas nos canais Sicredi.

o LIQUIDACAO_COMPE_H5: Liquidações efetuadas em outras instituições (Fora da Rede SICREDI) o LIQUIDACAO_COMPE_H6: Liquidações com valor superior a 250 mil efetuadas em outras instituições (Fora da Rede SICREDI) o LIQUIDACAO_COMPE_H8: Recebimento de liquidação fora da Rede SICREDI- Contingência (Alguma inconsistência de sistema). o LIQUIDACAO_CARTORIO: Liquidações efetuadas em cartórios. o ESTORNO_LIQUIDACAO_REDE: Liquidações que foram estornadas.

** Observação LIQUIDAÇÃO_REDE: Essa liquidação é a única que pode ter estorno no mesmo dia. Havendo estorno será enviado o evento “ESTORNO_LIQUIDACAO_REDE”. Fica a critério do associado acatar de imediato a liquidação ou aguardar o fim do dia para garantir que não ocorreu um estorno.

▪ Consulta de Contrato Webhook: Operação responsável pela consulta dos dados de um contrato através do código de beneficiário, cooperativa e posto.

▪ Altera Contrato Webhook: Operação responsável por alterar dados de um contrato através de um IdContrato.

> **Observação:** O tópico abaixo (22. Iniciando a Integração) é

destinado para os usuários que não conhecem o processo de integração da API. Caso já tenha conhecimento neste processo, o tópico abaixo pode ser ignorado, visto que a autenticação na API de Contratação Webhook é realizada com as mesmas credenciais da API (Cobrança Online).

## 22. Iniciando a integração

Para iniciar a integração há um processo constituído de etapas sequenciais para Validação e Produção:

3. Validação/Teste

Acessar o Portal do Desenvolvedor: d. Criação de APP para Homologação e. Solicitação do Access Token de Homologação (Sandbox) f. Testes nas URLS

b. Criação de APP para Homologação: para criar a APP de homologação, o desenvolvedor deve acessar o Portal do Desenvolvedor Sicredi (clique aqui) e criar uma conta ou logar. Após criar uma conta ou logar, o desenvolvedor deve clicar em ‘criar uma nova aplicação’. Ao criar uma nova aplicação deve nomeá-la com o prefixo ‘API Cobrança <Coop> <Código Beneficiário > SandBox’ para identificar que é a APP de homologação e selecionar a API da Cobrança [ API COBRANÇA BOLETO 1.0.0 ] e a API de Autenticação Parceiros na lista de APIs disponíveis [API AUTH - OPENAPI - PARCEIROS 1.0.0]. Ao criar uma app você já terá um Client ID gerado automaticamente para essa APP.

1.Criando a APP no portal 2. Selecionando as APIs na lista

b. Geração do Access Token de Homologação (Sandbox): após criar a APP de Homologação, você deve, ainda no portal do desenvolvedor, clicar no menu suporte > abrir chamado. Selecione a opção “COLOCAR AQUI QUAL OPÇÃO”, e preencha o nome da APP com aquele nome que você criou no passo anterior. Pronto, o seu Acess token será gerado e dentro de X dias você poderá visualizar o seu token de homologação no menu ‘minhas apps’, selecionando a app criada e clicando em ‘ver detalhes’.

3. Visualizando o access token

c. Testes nas URLS: No decorrer desse manual você encontrará as URLs para teste dos recursos disponíveis em ambiente de homologação.

3. Produção

Acessar o Portal do Desenvolvedor d. Criação de APP para Produção e. Solicitação do Access Token de Produção

Acessar o Internet Banking f. Geração da Código de acesso (password) no internet banking;

d. Criação de APP para produção: Para criar a APP de produção, você pode repetir os mesmos passos da criação da APP de homologação, no entanto, lembre-se de nomear essa nova APP de uma forma que você identifique que ela se trada da APP de produção, nossa sugestão é: ‘API Cobrança <Coop> <Código Beneficiário > Produção’. Após criar a APP você terá acesso imediato ao Client ID de produção.

e. Geração do Access Token de Produção: Repita os mesmos passos já realizados para a geração de token de homologação, mas dessa vez ao abrir o chamado, preencha o campo Client ID ou Nome da APP com os dados da APP de produção. O seu token será disponibilizado no mesmo prazo anteriormente citado e poderá ser visualizado no menu minhas apps.

4. Visualizando as APPs no menu

É importante lembrar que em todas as operações será necessário informar no cabeçalho da requisição no campo x-api-key o token de autenticação recebido. Esse token é diferente para o ambiente de Homologação (Sandbox) e Produção, deve ser informado o token correspondente ao ambiente que está sendo utilizado.

f. Geração do Código de Acesso (password): a empresa ou pessoa associada deve acessar o Internet Banking, pela conta vinculada ao seu convênio de Cobrança, e selecionar a opção de menu no: Cobrança >> Código de Acesso >> Gerar - Esse menu é apresentado

exclusivamente para os beneficiários que possuem a modalidade API(Cobrança Online) habilitada no convênio.

5. Geração de Código de Acesso (password)

Se você ou a sua empresa associada não possui a modalidade habilitada e deseja habilitá-la, converse com a pessoa gerente de conta.

A chave será gerada após informado o dispositivo de segurança. Ela deve ser armazenada e utilizada na configuração da integração da API, pois esse código gerado é utilizado no recurso de autenticação para permitir as operações. Caso o beneficiário possua mais de uma conta com o produto Cobrança contratado e deseje utilizá-las, precisará gerar o token de autenticação para cada conta específica.

## 23. Recursos disponíveis

### 23.1. Autenticação

Em todas as operações será necessário informar no cabeçalho da requisição no campo x-api-key o token de autenticação recebida do Sicredi. Esse token é diferente para o ambiente de Sandbox e Produção, deve ser informado o token correspondente ao ambiente que está sendo utilizado. A operação POST “token” é responsável por criar uma chave criptografada, denominada “access_token”, utilizando o Beneficiário + Cooperativa como usuário (username) e o Código de Acesso (password) que foi gerada no Internet Banking. O padrão de autenticação é o OAuth2 e o token utiliza o padrão JWT.

O formato de entrada é “x-www-form-urlencoded” e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8.

> **Observação:** deve ser respeitada a orientação da segurança da informação referente a expiração

do access_token e do refresh_token, observando os parâmetros “expires_in” (exemplo: 300 segundos) e “refresh_expires_in” (exemplo: 900 segundos) de retorno do serviço respectivamente. Os parâmetros de expiração (“expires_in” e “refresh_expires_in”) podem ser diferentes em Teste e Produção. Para verificar se o acess_token está expirado, deve-se observar o campo “expires_in” que retorna a duração do token em segundos. O access_token deve ser enviado no cabeçalho das chamadas das próximas operações até que ele expire, não deve ser realizado autenticação a cada chamada. Quando o access_token expirar, deve ser utilizado o refresh_token para gerar um novo token.

> **OBS:** Quando da utilização do refresh_token, não será necessário informar os campos

“username” e “password”. Quando o refresh_token estiver expirado, deverá ser realizado o fluxo de autenticação normal, com usuário e senha (username e password).

### ENTRADA

Tipo de

### SAÍDA

### RETORNO (Response)

### FALHAS

### URL Produção

```text
Ord         Nome         Tipo                       Tamanho         Formatação          Descrição        Obrigatório
                                  Parâmetro

1        x-api-key String       HeaderParam        36              UUID              Access token        Obrigatório
                                                                   38 bits           gerado no
                                                                                     portal do
                                                                                     desenvolvedor

2        context      String    HeaderParam 8                    COBRANCA            Contexto,           Obrigatório
                                                                                     passar fixo
                                                                                     “COBRANCA”

3           Content-              HeaderParam                      application/x-    Indica o tipo     Obrigatório
            Type                                                   www-form-         de arquivo do
                                                                   urlencoded        recurso

4           grant_typ String      Body            Até 13           password ou       Tipo de           Obrigatório
            e                                                      refresh_token     autenticação.
                                                                                     Ver diagrama,
                                                                                     pág 113

5           username String       Body            9                Sem               Código do         Obrigatório
                                                                   formatação        beneficiário +    para Grant
                                                                                     Código da         Type igual a
                                                                                     Cooperativa       password

6           password String       Body            64               Sem               Código de         Obrigatório
                                                                   formatação        Acesso gerado     para Grant
                                                                                     no Internet       Type igual
                                                                                     Banking           password

7           refresh_t String      Body            753              Sem               Refresh token     Obrigatório
            oken                                                   formatação        obtido            para Grant
                                                                   dígitos           anteriormente     Type igual a
                                                                                     na                refresh_toke
                                                                                     autenticação      n

8           scope        String   Body            8                cobranca          Escopo de         Obrigatório
                                                                                     acesso. Passar    para Grant
                                                                                     fixo              Type
                                                                                     “cobranca”.       password

                                                        Tipo de
          Ord                Nome         Tipo                          Formatação                Descrição
                                                       Parâmetro
                                                                                           Token de autenticação
    1                 access_token       String         JSON         576 bits
                                                                                           para ser utilizado

                                                                2017-08-
 2                 token_type         String       JSON         09T08:53:11.679- Tipo do Token
                                                                03:00
                                                                                 Será utilizado para gerar
                                                                                 um novo access_token,
 3                 refresh_token      String       JSON         636 dígitos      sem a necessidade de
                                                                                 uma nova autenticação
                                                                                 com usuário e senha
                                                                                 Tempo de expiração (em
 4                 expires_in         String       JSON         3 dígitos        segundos) do Access
                                                                                 Token
                                                                                 Tempo de expiração (em
 5                 refresh_expires_in String       JSON         6 dígitos        segundos) do Refresh
                                                                                 Token
                                                                cobranca profile Escopo que o usuário
 6                 scope              String       JSON
                                                                email            possui acesso.

              Código                                                Descrição
        HTTP_OK (200)                          Operação realizada com sucesso

                                                           Parâmetros de
                Status                 Mensagem               entrada                     Descrição
                                                          sujeitos a crítica

                                    Ocorreu um erro
                                                                                Erro quando o endereço
                                    ao realizar
HTTP_NOT_FOUND (404)                                      Não possui            (URL) inválido. Deve-se
                                    operação de
                                                                                verificar a url utilizada.
                                    autenticação.

                                                          username,             Erros ocasionados por
                                    Invalid user
HTTP_UNAUTHORIZED (401)                                   password,             username ou password não
                                    credentials
                                                          context               informado ou

                                                                          inválidos.

                               Missing
HTTP_UNAUTHORIZED                                                         Erro ocasionado por
                               parameter:       username
(401)                                                                     username não informado.
                               username
                               Could not find a
                               required Access
                                                                          Erro ocasionado quando o
HTTP_UNAUTHORIZED              Token in the
                                                x-api-key                 parâmetro x-api-key não foi
(401)                          request,
                                                                          informado ou está inválido.
                               identified by
                               HEADER x-api-key
                                                                          Erro ocasionado quando o
                               Missing form                               parâmetro grant_type não é
HTTP_BAD_REQUEST (400)         parameter:          grant_type             informado ou
                               grant_type
                                                                          inválido.
                                                                          Erro ocasionado quando o
HTTP_GATEWAY_TIMEOUT                                                      serviço não conseguiu
                               Gateway Timeout Não se aplica
(504)                                                                     responder no tempo
                                                                          esperado.

```

`https://api-parceiro.sicredi.com.br/auth/openapi/token`

### RESPONSE

```json
{
"access_token":
"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxMlRFLXliYWxnNjIxSmRCMU9IX0w2Sms5QllxODJtNkFS
Z0NGaDZtQnZBIn0.eyJleHAiOjE2NTY0NTA5MzIsImlhdCI6MTY1NjQ1MDYzMiwianRpIjoiZjNiYjg5YzQtOGRmM
y00OTMxLWExOTItMjUxMjUxZWE1YzNkIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLW9wZW5hcGkucHJkLnNpY3JlZGk
uY2xvdWQvcmVhbG1zL29wZW5hcGkiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiZjozMWY3ODE1MS1jMzdmLTQzO
WYtYTIwOS02NjFjYTQ3MzkzYTE6Q09CUkFOQ0E6MDA0MTQwMTE2IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoib3Bl
bmFwaS1ndy1zZW5zZWRpYSIsInNlc3Npb25fc3RhdGUiOiIwNGQ3YTAwYS05MjhiLTQ3NzQtYTY1MC1mOWQx
ODkzNWE3NDMiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtb3BlbmFwa
SIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW
50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbG
UiXX19LCJzY29wZSI6ImNvYnJhbmNhIHByb2ZpbGUgZW1haWwiLCJzaWQiOiIwNGQ3YTAwYS05MjhiLTQ3NzQ
tYTY1MC1mOWQxODkzNWE3NDMiLCJvcGVuYXBpX3VzZXJuYW1lIjoiMDA0MTQwMTE2Iiwib3BlbmFwaV9jb2
50ZXh0IjoiQ09CUkFOQ0EiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsInByZWZlcnJlZF91c2VybmFtZSI6IjAwNDE0M
DExNiJ9.mVzfyzBhVFwRWVnQkbiD00zitoOfmatRcDN_e_OKo4XKcOY6RU-qlpJ0FOC-
_7Qy5ez7XveNeRyq47ToY6F0skbw-0BswxY3qdQHBr50G-
AxqICYLSUTEamrPc0vLNwADdPracL9P2W_EVEFLae7QeVzc0toNLjvaLJCaGZH6-
wV99qMjf0VUj5nRCUm6ZCllb3NioyI6mXsj0Ea1s8RdgnanWvgvD-
cNjmCmMXVhy7U_MScLIb_7cIrn4TbcbkA3r11b96KzU9cp5BCXHZysDdX0wRFVzQUQfBsf33E40GSNJ-
juFZdV86vWCmx7BjKnC1s3embfX0p-9JnbNUy7Q",
"expires_in": 300,
"refresh_expires_in": 1800,
"refresh_token":
"eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJhMTRlOTdmOC1lZmJiLTQ2YTYtOTYwYy0wNTFlZjA4MT
U3MjEifQ.eyJleHAiOjE2NTY0NTI0MzIsImlhdCI6MTY1NjQ1MDYzMiwianRpIjoiOWIyOWE3YjMtYzRhNS00NGQz
LTg3MjQtZWU4ZGQzYjIwNDAxIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLW9wZW5hcGkucHJkLnNpY3JlZGkuY2xvdW
QvcmVhbG1zL29wZW5hcGkiLCJhdWQiOiJodHRwczovL2F1dGgtb3BlbmFwaS5wcmQuc2ljcmVkaS5jbG91ZC9y
ZWFsbXMvb3BlbmFwaSIsInN1YiI6ImY6MzFmNzgxNTEtYzM3Zi00MzlmLWEyMDktNjYxY2E0NzM5M2ExOkNP
QlJBTkNBOjAwNDE0MDExNiIsInR5cCI6IlJlZnJlc2giLCJhenAiOiJvcGVuYXBpLWd3LXNlbnNlZGlhIiwic2Vzc2lvbl9z
dGF0ZSI6IjA0ZDdhMDBhLTkyOGItNDc3NC1hNjUwLWY5ZDE4OTM1YTc0MyIsInNjb3BlIjoiY29icmFuY2EgcHJvZ
mlsZSBlbWFpbCIsInNpZCI6IjA0ZDdhMDBhLTkyOGItNDc3NC1hNjUwLWY5ZDE4OTM1YTc0MyJ9.Om0gACw3fK
yfSnWyNhlElQSMYYtClCHrZYdGAIc4ul0",
"token_type": "Bearer",
"not-before-policy": 0,
"session_state": "04d7a00a-928b-4774-a650-f9d18935a743",
"scope": "cobranca profile email"
}
```

### Execução (via ferramenta Postman)

▪ Cenário Positivo – Gerando o access_token

> **OBS:** Sempre verificar o tempo de expiração do access_token, através do campo “expires_in” (Exemplo: 300

segundos). Caso esteja expirado, deverá usar o refresh_token para gerar um novo token.

▪ Cenário Positivo – Utilizando o refresh_token para gerar um novo token

> **OBS:** Sempre verificar o tempo de expiração do refresh_token, através do campo “refresh_expires_in”

(Exemplo: 1800 segundos). Caso esteja expirado, deverá gerar um novo access_token. Quando da utilização do refresh_token, não será necessário informar os campos “username” e “password”.

▪ Cenário de Erro <400 - BAD_REQUEST>

▪ Cenários de Erro <401 - UNATHORIZED>

### 23.2. Realizar Contratação Webhook

A operação POST “criar” é responsável por gerar um novo contrato Webhook. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso cadastro responde em milissegundos.

### ENTRADA

### SAÍDA

### RETORNO (Response)

### FALHAS

### URL Produção

```text
Ord         Nome                 Tipo     Parametro Tamanho             Formatação              Descrição         Obrigatório
                                                                                           Access token
                                          Header                                           gerado no
1     x-api-key              String                    38           UUID                                          Obrigatório
                                          Param                                            portal do
                                                                                           desenvolvedor
                                                                                      Campo
                                                                                      access_token
                                          Header                    Bearer + Token de
1     Authorization          String                    1413                           obtido na saída             Obrigatório
                                          Param                     Autenticação
                                                                                      da
                                                                                      autenticação.
                            Application   Header                                           Estabelece o
2     Content-Type                                                  Sem formatação
                            /json         param                                            tipo de

                                                                                          conteúdo da
                                                                                          requisição

                                                                                          Código da
3    cooperativa            String      Body          4            Sem formatação                                Obrigatório
                                                                                          Cooperativa -

4    posto                  String      Body          2            Sem formatação         Código da Agência      Obrigatório
                                                                                          Código do
5    codBeneficiario        String      Body          5            Sem formatação         Convênio de          Obrigatório
                                                                   Domínios:              Eventos enviados via
7    eventos                List<String> Body                                                                  Obrigatório
                                                                   LIQUIDACAO             Webhook
                                                                                          URL de integração
                                                                                          entre Sicredi e
                                                                                          associado.
8    url                    String      Body          256          Sem formatação         Obs: A url deve      Obrigatório
                                                                                          possuir,
                                                                                          obrigatoriamente, o
                                                                                          protocolo (https)
                                                                   Domínios: ATIVO,       Status da URL que
9    urlStatus              String      Body          10           INATIVO,               está sendo utilizada   Obrigatório
                                                                   BLOQUEADO              na comunicação.
                                                                   Domínios: ATIVO,
                                                                                          Status do contrato
10   contratoStatus         String      Body          10           INATIVO,                                      Obrigatório
                                                                                          gerado
                                                                   BLOQUEADO
                                                                                          Nome do
11   nomeResponsavel        String      Body          50           Sem formatação         responsável pelo       Opcional
                                                                                          contrato

                                                                                          Email do
12   email                  String      Body          40           Sem formatação                                Opcional
                                                                                          responsável

                                                                                          Telefone do
13   telefone              String       Body         20            Sem formatação                                Opcional
                                                                                          Responsável

                                                                                          Flag que indica se
                                                                                          ao enviar o evento
                                                                                          de liquidação, caso
     enviarIdTituloEmp                                             Domínios:              o título possua
14                     Boolean          Body                                                                     Opcional
     resa                                                          true/false             ‘idTituloEmpresa’
                                                                                          essa informação
                                                                                          será enviada no
                                                                                          evento.

                                                                                              Obs: Só informar
                                                                                              esse campo se sua
                                                                                              API de recebimento
                                                                                              de eventos possuir
15    header                String        Body                         Sem formatação         algum tipo de           Opcional
                                                                                              autenticação para
                                                                                              ser acessada.

                                                                                              Valor da secret-id
                                                                                              Obs: Só informar
                                                                                              esse campo se sua
                                                                                              API de recebimento
                                                                                              de eventos possuir
16    token                 String        Body                         Sem formatação         algum tipo de           Opcional
                                                                                              autenticação para
                                                                                              ser acessada.

                                                                                              Valor da senha

Ord    Nome                      Tipo           Tamanho Formatação                                      Descrição

1      idContrato                String         25           Sem formatação                             Identificador único do
                                                                                                        contrato.

2      cooperativa               String         4            Sem formatação                             Código da Cooperativa do
                                                                                                        Beneficiário.

3      posto                     String         2            Sem formatação                             Código da Posto do
                                                                                                        Beneficiário.
4      codBeneficiario           String         5            Sem formatação                             Código do Beneficiário de
                                                                                                        Cobrança.

5      eventos                   List<String>                Domínios:                                  Lista de eventos que o
                                                             "LIQUIDACAO_PIX",                          associado receberá
                                                             "LIQUIDACAO_COMPE_H5",                     informações em caso de
                                                             "LIQUIDACAO_COMPE_H6",                     movimentação.
                                                             "LIQUIDACAO_COMPE_H8",
                                                             * "LIQUIDACAO_REDE",                       "LIQUIDACAO_REDE"
                                                             "LIQUIDACAO_CARTORIO"                      *Obs: Essa liquidação é a
                                                             "AVISO_PAGAMENTO_COMPE",                   única que pode ter
                                                             "ESTORNO_LIQUIDACAO_REDE"                  estorno no mesmo dia.
                                                                                                        Havendo estorno será

                                                                                                  enviado      o      evento
                                                                                                  “ESTORNO_LIQUIDACAO
                                                                                                  _REDE”.
                                                                                                  Fica    a    critério   do
                                                                                                  associado     acatar    de
                                                                                                  imediato a liquidação ou
                                                                                                  aguardar o fim do dia para
                                                                                                  garantir que não ocorreu
                                                                                                  um estorno.

6        url                       String      256         Sem formatação                         URL de integração entre
                                                                                                  Sicredi e associado
7        urlStatus                 String      10          Domínios: ATIVO, INATIVO,              Status da URL que está
                                                           BLOQUEADO                              sendo utilizada na
                                                                                                  comunicação.
8        contratoStatus            String      10          Domínios: ATIVO, INATIVO,
                                                                                                  Status do contrato gerado
                                                           BLOQUEADO
9        nomeResponsavel           String      50          Sem formatação                         Nome do responsável
                                                                                                  pelo contrato
10       Email                     String      40          Sem formatação                         Email do responsável

11       telefone                  String      20          Sem formatação
                                                                                                  Telefone do Responsável

12       header                    String                  Sem formatação                         Valor da secret-id

13       token                     String                  Sem formatação                         Valor da senha
                                                                                                  Flag que indica se envia
14       enviarIdTituloEmpresa Boolean                     Domínios: true/false
                                                                                                  ‘idTituloEmpresa’

Status                                                              Descrição
HTTP_ACCEPTED (201)                                                 Operação aceita com sucesso

                                                               Parâmetros sujeitos
                  Status                    Mensagem                                              Descrição
                                                                    à crítica

                             Ocorreu um erro ao
HTTP_NOT_FOUND (404)         realizar operação de    Não possui                    Erro ocasionado por URL inválida.
                             cadastro

                                                                                   Erro ocasionado por parâmetro
HTTP_UNSUPPORTED_MEDIA       Tipo de conteúdo        Parâmetro:                    obrigatório não informado ou
_TYPE (415)                  não suportado           Content-Type                  inválido.

                                                                                   Erro ocasionado por parâmetro
                                                     Parâmetro:
HTTP_UNAUTHORIZED (401)      Unauthorized                                          obrigatório não informado ou
                                                     Authorization
                                                                                   inválido.
                             Cooperativa                                           Erro ocasionado quando o código de
HTTP_UNAUTHORIZED            diferente da            Parâmetro:                    cooperativa informado no cadastro
(401)                        cooperativa do          cooperativa                   for diferente do contido na
                             usuário                                               autenticação.

                             Código de
                                                                                   Erro ocasionado quando o código de
                             beneficiário
HTTP_UNAUTHORIZED                                    Parâmetro:                    beneficiário informado no cadastro
                             diferente do
(401)                                                codigoBeneficiario            for diferente do contido na
                             beneficiário do
                                                                                   autenticação.
                             usuário

                                                     Qualquer parâmetro
                                                     que consta como
                                                     obrigatório na                Erros ocasionados por parâmetros
                             Campo obrigatório
HTTP_BAD_REQUEST (400)                               listagem de                   obrigatórios não informados ou
                             em branco
                                                     parâmetros de                 inválidos.
                                                     entrada pode ser
                                                     criticado

                                                                                    Erros ocasionados por parâmetros
                             “Cooperativa            Parâmetro:
HTTP_BAD_REQUEST (400)                                                             obrigatórios não preenchidos ou
                             inválida”               cooperativa
                                                                                   inválidos.

                                                                                    Erros ocasionados por parâmetros
                                                     Parâmetro:
HTTP_BAD_REQUEST (400)       “Posto inválido”                                      obrigatórios não preenchidos ou
                                                     posto
                                                                                   inválidos

                                                                                   Erro ocasionado por parâmetro
HTTP_BAD_REQUEST (400)       “Evento deve conter     Parâmetro:
                                                                                   obrigatório não informado ou
                             LIQUIDACAO”             evento
                                                                                   inválido.

                                                                                        Erro ocasionado por parâmetro
HTTP_BAD_REQUEST (400)                                   Parâmetro:
                              “URL é obrigatória”                                      obrigatório não informado ou
                                                          url
                                                                                       inválido.

                                                                                        O campo foi preenchido em branco
                                                         Parâmetro:
HTTP_BAD_REQUEST (400)        “URL inválida”                                           ("") ou o tipo de protocolo
                                                          url
                                                                                       informado foi diferente de "http" ou
                                                                                       "https".

                              “URL Status não foi
HTTP_BAD_REQUEST (400)                                   Parâmetro:                    O parâmetro não foi informado ou
                              informado ou está
                                                         urlStatus                     foi preenchido com valor inválido.
                              inválida”
                              “O campo URL Status                                      O campo “urlStatus” foi preenchido
HTTP_BAD_REQUEST (400)        deve ser ATIVO,            Parâmetro:                    com um valor diferente dos
                              INATIVO,                   urlStatus                     domínios: ATIVO, INATIVO,
                              BLOQUEADO”                                               BLOQUEADO
                                                                                       O campo "contratoStatrus" não foi
HTTP_BAD_REQUEST (400)        “Contrato Status é         Parâmetro:
                                                                                       preenchido.
                              obrigatório”               contratoStatus
                              O campo Contrato                                         O campo “contratoStatus” foi
HTTP_BAD_REQUEST (400)        Status deve ser            Parâmetro:                    preenchido com um valor diferente
                              ATIVO, INATIVO ou          contratoStauts                dos domínios: ATIVO, INATIVO,
                              BLOQUEADO.                                               BLOQUEADO
                                                                                       O campo “email” foi preenchido com
HTTP_BAD_REQUEST (400)                                   Parâmetro:
                              “Email inválido”                                         um endereço de email inválido.
                                                         email
                                                                                       Exemplo: Sem o @.

                              “Contrato já
UNPROCESSABLE_ENTITY                                                                   Já existe um contrato cadastrado
                              existente.”
(422)                                                                                  com os mesmos dados (coop e bnf)

```

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/webhook/contrato/`

HEADERS: Authorization x-api-key Content-Type: application/json

### REQUEST BODY (JSON)

```json
{
"cooperativa": "0123",
"posto": "12",
"codBeneficiario": "12345",
"eventos": [
"LIQUIDACAO"
],
"url": "https://teste.instituicao.cloud/v1/contratos",
"urlStatus": "ATIVO",
"contratoStatus": "ATIVO",
"nomeResponsavel": "NOME RESPONSAVEL",
"email": "TESTE@EMAIL.COM.BR",
"telefone": "51 999999999"
}
```

### RESPONSE BODY (JSON)

```json
{
"idContrato": "63a241a590364b703bbe6aac",
"cooperativa": "0123",
"posto": "12",
"codBeneficiario": "12345",
"eventos": [
"LIQUIDACAO_PIX",
"LIQUIDACAO_COMPE_H5",
"LIQUIDACAO_COMPE_H6",
"LIQUIDACAO_COMPE_H8",
"LIQUIDACAO_REDE",
"LIQUIDACAO_CARTORIO",
"AVISO_PAGAMENTO_COMPE",
"ESTORNO_LIQUIDACAO_REDE"
],
"url": "https://teste.instituicao.cloud/v1/contratos",
"urlStatus": "ATIVO",
"contratoStatus": "ATIVO",
"nomeResponsavel": "NOME RESPONSAVEL",
"email": "TESTE@EMAIL.COM.BR",
"telefone": "51 999999999"
}
```

### Execução (via ferramenta Postman)

▪ Cenário Positivo:

▪ Cenário de erro <Contrato já existente>

### 23.3. Consulta Contratos

A operação GET “Consulta Contratos” é responsável pela é responsável por realizar a consulta de contratos de um determinado beneficiário. Nesse método você verificar seus contratos ATIVOS, INATIVOS ou BLOQUEADOS, bem como os dados do seu contrato. O formato de entrada é sempre JSON, e a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response em caso de sucesso, e JSON, em caso de falha.

### Entrada

Tipo de

### Saída

### FALHAS

### REQUEST – GET

### URL Produção

```text
Ord       Nome             Tipo                       Tamanho        Formatação              Descrição                Obrigatório
                                   Parâmetro

1     x-api-key        String     HeaderParam         36         UUID                  Token de acesso       Obrigatório
                                                                                       fornecido pelo
                                                                                       Sicredi

2     Authorization String        HeaderParam         1413       Bearer + Token de Acess token obtido Obrigatório
                                                                 Autenticação      na autenticação

3     cooperativa      String     QueryParam          4          Sem formatação        Código da             Obrigatório
                                                                                       cooperativa.

      posto            String     QueryParam          4          Sem formatação        Código do Posto       Obrigatório

      beneficiário     String     QueryParam          5          Sem formatação        Código do             Obrigatório
                                                                                       Beneficiário

Ord Nome                          Tipo          Tamanho Formatação                                              Descrição

1     idContrato                  String        25             Sem formatação                                   Identificador único do
                                                                                                                contrato.

2     cooperativa                 String        4              Sem formatação                                   Código da Cooperativa
                                                                                                                do Beneficiário.

3     posto                       String        2              Sem formatação                                   Código da Posto do
                                                                                                                Beneficiário.

4    codBeneficiario             String         5          Sem formatação                            Código do Beneficiário
                                                                                                     de Cobrança.

5    eventos                     List<String>              Domínios:                                 Lista de eventos que o
                                                           "LIQUIDACAO_PIX","LIQUIDACAO_COMPE        associado receberá
                                                           _H5",                                     informações em caso de
                                                           "LIQUIDACAO_COMPE_H6",                    movimentação.
                                                           "LIQUIDACAO_COMPE_H8",
                                                           "LIQUIDACAO_REDE","LIQUIDACAO_CART
                                                           ORIO"
                                                           "AVISO_PAGAMENTO_COMPE",
                                                           "ESTORNO_LIQUIDACAO_REDE"

6    url                         String         256        Sem formatação                            URL de integração entre
                                                                                                     Sicredi e associado
7    urlStatus                   String         10         Domínios: ATIVO, INATIVO, BLOQUEADO       Status da URL que está
                                                                                                     sendo utilizada na
                                                                                                     comunicação.
8    contratoStatus              String         10         Domínios: ATIVO, INATIVO, BLOQUEADO       Status do contrato
                                                                                                     gerado
9    nomeResponsavel             String         50         Sem formatação                            Nome do responsável
                                                                                                     pelo contrato
10   Email                       String         40         Sem formatação                            Email do responsável

11   telefone                    String         20         Sem formatação
                                                                                                     Telefone do Responsável
12   enviarIdTituloEmpresa Boolean                         Domínios: true/false                      Flag que indica se envia
                                                                                                     ‘idTituloEmpresa’
13   header                      String                    Sem formatação                            Valor da secret-id

14   token                       String                    Sem formatação                            Valor da senha

                                                                          Parâmetros de
                 Status                               Mensagem               entrada                      Descrição
                                                                         sujeitos a crítica
HTTP_UNAUTHORIZED (401)                     Could not find a required Parâmetro:              Erros ocasionados por
                                            Access Token in the       x-api-key               parâmetros obrigatórios não
                                            request, identified by                            informados ou inválidos.
                                            HEADER x-api-key

HTTP_UNAUTHORIZED (401)             UNAUTHORIZED                  Parâmetro:          Erros ocasionados por
                                                                  Authorization       parâmetros obrigatórios não
                                                                                      informados ou inválidos.

HTTP_UNAUTHORIZED (401)             Código de beneficiário    Parâmetro:              Erro ocasionado quando a linha
                                    diferente do beneficiário linhaDigitavel          digitável informada é de
                                    do usuário                                        beneficiário e/ou agência
                                                                                      diferente.

HTTP_BAD_REQUEST (400)              “Cooperativa inválida”.       Parâmetro:          Erros ocasionados por
                                                                  cooperativa         parâmetros obrigatórios não
                                                                                      informados ou inválidos.

HTTP_BAD_REQUEST (400)              “Posto Inválido”              Parâmetro:          Erros ocasionados por
                                                                  posto               parâmetros obrigatórios não
                                                                                      informados ou inválidos.

HTTP_BAD_REQUEST (400)               “Código do Beneficiário Parâmetro:               Erros ocasionados por
                                    inválido”                beneficiario             parâmetros obrigatórios não
                                                                                      informados ou inválidos.

UNPROCESSABLE_ENTITY (422)          “Nenhum contrato                                  Erros ocasionados caso o
                                    encontrado”                                       beneficiário pesquisado não
                                                                                      possua contrato.

```

`https://api-`

parceiro.sicredi.com.br/cobranca/boleto/v1/webhook/contratos/?cooperativa=0100&posto=99&beneficiari o=12345 HEADERS:

Authorization x-api-key

PARAM: cooperativa posto beneficiario

### Execução (via ferramenta Postman)

▪ Parâmetros:

▪ Headers:

▪ Cenário Positivo:

▪ Cenário Negativo: 400 – BAD_REQUEST (Não informou código beneficiário)

▪ Cenário Negativo: 400 – Bad Request (Cooperativa inválida)

▪ Cenário Negativo: 401 – UNAUTHORIZED

### 23.4. Altera Contrato Webhook

A operação PUT “criar/alterar” é responsável por alterar dados de um contrato Webhook já existente. O formato de entrada e saída é JSON, considerando que a saída sempre é composta da entidade a ser retornada e um código HTTP dentro de um Response. O tipo de codificação utilizado no Response é Unicode UTF-8. Por padrão o recurso de alterar responde em milissegundos.

### ENTRADA

### SAÍDA

### RETORNO (Response)

### FALHAS

### REQUEST – GET

### URL Produção

```text
Ord           Nome               Tipo     Parametro Tamanho             Formatação              Descrição         Obrigatório
                                                                                           Access token
                                          Header                                           gerado no
1     x-api-key              String                    38           UUID                                          Obrigatório
                                          Param                                            portal do
                                                                                           desenvolvedor
                                                                                      Campo
                                                                                      access_token
                                          Header                    Bearer + Token de
2     Authorization          String                    1413                           obtido na saída             Obrigatório
                                          Param                     Autenticação
                                                                                      da
                                                                                      autenticação.
                                                                                           Estabelece o
                            Application   Header                                           tipo de
3     Content-Type                                                  Sem formatação
                            /json         param                                            conteúdo da
                                                                                           requisição
4     idContrato             String       Patch        25           Sem formatação         Identificador          Obrigatório
                                                                                           único do
                                                                                           contrato
                                                                                           Código da
5     cooperativa            String       Body         4            Sem formatação                                Obrigatório
                                                                                           Cooperativa -

6     posto                  String       Body         2            Sem formatação         Código da Agência      Obrigatório
                                                                                           Código do
7     codBeneficiario        String       Body         5            Sem formatação         Convênio de          Obrigatório
                                                                    Domínios:              Eventos enviados via
8     eventos                List<String> Body                                                                  Obrigatório
                                                                    LIQUIDACAO             Webhook
                                                                                           URL de integração
                                                                                           entre Sicredi e
                                                                                           associado.
9     url                    String       Body         256          Sem formatação         Obs: A url deve      Obrigatório
                                                                                           possuir,
                                                                                           obrigatoriamente, o
                                                                                           protocolo (https)
                                                                    Domínios: ATIVO,       Status da URL que
10    urlStatus              String       Body         10           INATIVO,               está sendo utilizada   Obrigatório
                                                                    BLOQUEADO              na comunicação.

                                                                  Domínios: ATIVO,
                                                                                         Status do contrato
11    contratoStatus         String     Body         10           INATIVO,                                      Obrigatório
                                                                                         gerado
                                                                  BLOQUEADO
                                                                                         Nome do
12    nomeResponsavel        String     Body         50           Sem formatação         responsável pelo       Opcional
                                                                                         contrato

                                                                                         Email do
13    email                  String     Body         40           Sem formatação                                Opcional
                                                                                         responsável

                                                                                         Telefone do
14    telefone              String      Body        20            Sem formatação                                Opcional
                                                                                         Responsável

                                                                                         Flag que indica se
                                                                                         ao enviar o evento
                                                                                         de liquidação, caso
      enviarIdTituloEmp                                           Domínios:              o título possua
15                      Boolean         Body                                                                    Opcional
      resa                                                        true/false             ‘idTituloEmpresa’
                                                                                         essa informação
                                                                                         será enviada no
                                                                                         evento.
                                                                                         Obs: Só informar
                                                                                         esse campo se sua
                                                                                         API de recebimento
                                                                                         de eventos possuir
16    header                String      Body                      Sem formatação         algum tipo de          Opcional
                                                                                         autenticação para
                                                                                         ser acessada.

                                                                                         Valor da secret-id
                                                                                         Obs: Só informar
                                                                                         esse campo se sua
                                                                                         API de recebimento
                                                                                         de eventos possuir
17    token                 String      Body                      Sem formatação         algum tipo de          Opcional
                                                                                         autenticação para
                                                                                         ser acessada.

                                                                                         Valor da secret-id

Ord   Nome                       Tipo      Tamanho Formatação                                      Descrição

1    idContrato                 String         25          Sem formatação                        Identificador único do
                                                                                                 contrato.

2    cooperativa                String         4           Sem formatação                        Código da Cooperativa
                                                                                                 do Beneficiário.

3    posto                      String         2           Sem formatação                        Código da Posto do
                                                                                                 Beneficiário.
4    codBeneficiario            String         5           Sem formatação                        Código do Beneficiário
                                                                                                 de Cobrança.

5    eventos                    List<String>               Domínios:                             Lista de eventos que o
                                                           "LIQUIDACAO_PIX",                     associado receberá
                                                           "LIQUIDACAO_COMPE_H5",                informações em caso de
                                                           "LIQUIDACAO_COMPE_H6",                movimentação.
                                                           "LIQUIDACAO_COMPE_H8",
                                                           * "LIQUIDACAO_REDE",                  "LIQUIDACAO_REDE"
                                                           "LIQUIDACAO_CARTORIO"                 *Obs: Essa liquidação é a
                                                           "AVISO_PAGAMENTO_COMPE",              única que pode ter
                                                           "ESTORNO_LIQUIDACAO_REDE"             estorno no mesmo dia.
                                                                                                 Havendo estorno será
                                                                                                 enviado      o    evento
                                                                                                 “ESTORNO_LIQUIDACAO
                                                                                                 _REDE”.
                                                                                                 Fica a critério do
                                                                                                 associado acatar de
                                                                                                 imediato a liquidação ou
                                                                                                 aguardar o fim do dia
                                                                                                 para garantir que não
                                                                                                 ocorreu um estorno.

6    url                        String         256         Sem formatação                        URL de integração entre
                                                                                                 Sicredi e associado
7    urlStatus                  String         10          Domínios: ATIVO, INATIVO,             Status da URL que está
                                                           BLOQUEADO                             sendo utilizada na
                                                                                                 comunicação.
8    contratoStatus             String         10          Domínios: ATIVO, INATIVO,             Status do contrato
                                                           BLOQUEADO                             gerado
9    nomeResponsavel            String         50          Sem formatação                        Nome do responsável
                                                                                                 pelo contrato
10   Email                      String         40          Sem formatação                        Email do responsável

11   telefone                   String         20          Sem formatação
                                                                                                 Telefone do Responsável

12       enviarIdTituloEmpresa Boolean                       Domínios: true/false                          Flag que indica se envia
                                                                                                           ‘idTituloEmpresa’
13       header                     String                   Sem formatação                                Valor da secret-id

14       token                      String                   Sem formatação                                Valor da senha

Status                                                                 Descrição
HTTP_ACCEPTED (200)                                                    Operação aceita com sucesso

                                                                  Parâmetros sujeitos
                 Status                      Mensagem                                                       Descrição
                                                                       à crítica

                                         Ocorreu um erro ao
HTTP_NOT_FOUND (404)                     realizar operação de    Não possui                    Erro ocasionado por URL inválida.
                                         cadastro

                                                                                               Erro ocasionado por parâmetro
HTTP_UNSUPPORTED_MEDIA_TY                Tipo de conteúdo        Parâmetro:                    obrigatório não informado ou
PE (415)                                 não suportado           Content-Type                  inválido.

                                                                                               Erro ocasionado por parâmetro
                                                                 Parâmetro:
HTTP_UNAUTHORIZED (401)                  Unauthorized                                          obrigatório não informado ou
                                                                 Authorization
                                                                                               inválido.
                                         Cooperativa                                           Erro ocasionado quando o código de
HTTP_UNAUTHORIZED                        diferente da            Parâmetro:                    cooperativa informado no cadastro
(401)                                    cooperativa do          cooperativa                   for diferente do contido na
                                         usuário                                               autenticação.

                                Código de
                                                                                      Erro ocasionado quando o código de
                                beneficiário
HTTP_UNAUTHORIZED                                       Parâmetro:                    beneficiário informado no cadastro
                                diferente do
(401)                                                   codigoBeneficiario            for diferente do contido na
                                beneficiário do
                                                                                      autenticação.
                                usuário

                                                                                      Erro ocasionado quando o id de
HTTP_UNAUTHORIZED
                                “Erro interno ao        Parâmetro:                    contrato informado corresponde a
(422)
                                procurar contrato.”     Pacth: id                     um id que não existe na base de
                                                                                      dados.

                                                        Qualquer parâmetro
                                                        que consta como
                                                        obrigatório na                Erros ocasionados por parâmetros
                                Campo obrigatório
HTTP_BAD_REQUEST (400)                                  listagem de                   obrigatórios não informados ou
                                em branco
                                                        parâmetros de                 inválidos.
                                                        entrada pode ser
                                                        criticado

                                                                                       Erros ocasionados por parâmetros
                                “Cooperativa            Parâmetro:
HTTP_BAD_REQUEST (400)                                                                obrigatórios não preenchidos ou
                                inválida”               cooperativa
                                                                                      inválidos.

                                                                                       Erros ocasionados por parâmetros
                                                        Parâmetro:
HTTP_BAD_REQUEST (400)          “Posto inválido”                                      obrigatórios não preenchidos ou
                                                        posto
                                                                                      inválidos

                                                                                      Erro ocasionado por parâmetro
HTTP_BAD_REQUEST (400)          “Evento deve conter     Parâmetro:
                                                                                      obrigatório não informado ou
                                LIQUIDACAO”             evento
                                                                                      inválido.
                                                                                       Erro ocasionado por parâmetro
HTTP_BAD_REQUEST (400)                                  Parâmetro:
                                “URL é obrigatória”                                   obrigatório não informado ou
                                                         url
                                                                                      inválido.

                                                                                       O campo foi preenchido em branco
                                                        Parâmetro:
HTTP_BAD_REQUEST (400)          “URL inválida”                                        ("") ou o tipo de protocolo
                                                         url
                                                                                      informado foi diferente de "http" ou
                                                                                      "https".

                                “URL Status não foi
HTTP_BAD_REQUEST (400)                                  Parâmetro:                    O parâmetro não foi informado ou
                                informado ou está
                                                        urlStatus                     foi preenchido com valor inválido.
                                inválida”

                                    “O campo URL Status                                    O campo “urlStatus” foi preenchido
HTTP_BAD_REQUEST (400)              deve ser ATIVO,          Parâmetro:                    com um valor diferente dos
                                    INATIVO,                 urlStatus                     domínios: ATIVO, INATIVO,
                                    BLOQUEADO”                                             BLOQUEADO
                                                                                           O campo "contratoStatrus" não foi
HTTP_BAD_REQUEST (400)              “Contrato Status é       Parâmetro:
                                                                                           preenchido.
                                    obrigatório”             contratoStatus
                                    O campo Contrato                                       O campo “contratoStatus” foi
HTTP_BAD_REQUEST (400)              Status deve ser          Parâmetro:                    preenchido com um valor diferente
                                    ATIVO, INATIVO ou        contratoStauts                dos domínios: ATIVO, INATIVO,
                                    BLOQUEADO.                                             BLOQUEADO
                                                                                           O campo “email” foi preenchido com
HTTP_BAD_REQUEST (400)                                       Parâmetro:
                                    “Email inválido”                                       um endereço de email inválido.
                                                             email
                                                                                           Exemplo: Sem o @.

```

`https://api-parceiro.sicredi.com.br/cobranca/boleto/v1/webhook/contrato/{id}`

HEADERS: Authorization x-api-key Content-Type: application/json

PATH: id

### REQUEST BODY (JSON)

```json
{
"cooperativa": "0123",
"posto": "12",
"codBeneficiario": "12345",
"eventos": [
"LIQUIDACAO"
],
"url": "https://teste.instituicao.cloud/v1/contratos",
"urlStatus": "ATIVO",
"contratoStatus": "ATIVO",
"nomeResponsavel": "NOME RESPONSAVEL",
"email": "TESTE@EMAIL.COM.BR",
"telefone": "51 999999999"
}
```

### RESPONSE BODY (JSON)

```json
{
"idContrato": "63a241a590364b703bbe6aac",
"cooperativa": "0123",
"posto": "12",
"codBeneficiario": "12345",
"eventos": [
"LIQUIDACAO_PIX",
"LIQUIDACAO_COMPE_H5",
"LIQUIDACAO_COMPE_H6",
"LIQUIDACAO_COMPE_H8",
"LIQUIDACAO_REDE",
"LIQUIDACAO_CARTORIO",
"AVISO_PAGAMENTO_COMPE",
"ESTORNO_LIQUIDACAO_REDE"
],
"url": "https://teste.instituicao.cloud/v1/contratos",
"urlStatus": "ATIVO",
"contratoStatus": "ATIVO",
"nomeResponsavel": "NOME RESPONSAVEL",
"email": "TESTE@EMAIL.COM.BR",
"telefone": "51 999999999"
}
```

### Execução (via ferramenta Postman)

▪ Body Request:

```text
   ▪   Headers/Path Param:

   ▪   Cenário Positivo:

   ▪   Cenário de erro <Beneficiário não autenticado> :

```

## 24. Glossário

▪ Beneficiário: Empresa ou pessoa associada do Sicredi que contratou o produto Cobrança com a sua cooperativa.

▪ Código de Beneficiário / Código de convênio: Código gerado no momento da contratação do produto Cobrança na sua agência.

▪ Número da cooperativa: É o número da cooperativa a qual pertence a conta vinculada ao convênio de cobrança da pessoa associada. Esse número é o qual junto com o número de conta o associado pode logar no seu internet banking.

▪ Número de posto: É o número da agência a qual pertence a conta vinculada ao convênio de Cobrança da pessoa associada. Você pode encontrar essa informação acessando (ou solicitando à empresa associada que acesse) o internet banking, no menu “Cobrança”, na opção ‘Cadastrar títulos’. Ela encontrará a seguinte informação:

```text
                          1234 46 12346 NOME DA EMPRESA
                                                  Código de beneficiário
                                          Posto
                               Cooperativa

```
