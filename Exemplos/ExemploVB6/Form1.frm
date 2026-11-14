VERSION 5.00
Begin VB.Form Form1 
   ClientHeight    =   5580
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   9015
   BeginProperty Font 
      Name            =   "Tahoma"
      Size            =   8.25
      Charset         =   0
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   LinkTopic       =   "Form1"
   ScaleHeight     =   5580
   ScaleWidth      =   9015
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdCresol 
      Caption         =   "Cresol"
      Height          =   660
      Left            =   7020
      TabIndex        =   13
      Top             =   660
      Width           =   1770
   End
   Begin VB.CommandButton cmdCresolEmitir 
      Caption         =   "Emitir Boleto Cresol"
      Height          =   660
      Left            =   7020
      TabIndex        =   14
      Top             =   1680
      Width           =   1770
   End
   Begin VB.CommandButton cmdSantander 
      Caption         =   "Santander"
      Height          =   660
      Left            =   4860
      TabIndex        =   10
      Top             =   660
      Width           =   1770
   End
   Begin VB.CommandButton cmdCredisis 
      Caption         =   "Credisis"
      Height          =   660
      Left            =   2640
      TabIndex        =   9
      Top             =   1680
      Width           =   1770
   End
   Begin VB.CommandButton cmdBradesco 
      Caption         =   "Bradesco"
      Height          =   660
      Left            =   4860
      TabIndex        =   8
      Top             =   1680
      Width           =   1770
   End
   Begin VB.CommandButton btnSaldo 
      Caption         =   "Saldo"
      Height          =   660
      Left            =   3720
      TabIndex        =   7
      Top             =   4680
      Width           =   1770
   End
   Begin VB.CommandButton btnExtrato 
      Caption         =   "Extrato"
      Height          =   660
      Left            =   480
      TabIndex        =   6
      Top             =   4680
      Width           =   1770
   End
   Begin VB.CommandButton btnConsultarBoleto 
      Caption         =   "Consultar Boleto"
      Height          =   660
      Left            =   3720
      TabIndex        =   5
      Top             =   3240
      Width           =   1770
   End
   Begin VB.CommandButton btnEmitirBoleto 
      Caption         =   "Emitir Boleto"
      Height          =   660
      Left            =   480
      TabIndex        =   4
      Top             =   3240
      Width           =   1770
   End
   Begin VB.CommandButton btnBB 
      Caption         =   "Banco do Brasil"
      Height          =   660
      Left            =   2640
      TabIndex        =   2
      Top             =   660
      Width           =   1770
   End
   Begin VB.CommandButton btnSicredi 
      Caption         =   "Sicredi"
      Height          =   660
      Left            =   300
      TabIndex        =   1
      Top             =   1680
      Width           =   1770
   End
   Begin VB.CommandButton btnSicoob 
      Caption         =   "Sicoob"
      Height          =   660
      Left            =   300
      TabIndex        =   0
      Top             =   660
      Width           =   1770
   End
   Begin VB.Label lblConsultasExtratoSaldo 
      BackStyle       =   0  'Transparent
      Caption         =   "Consultas Extrato/Saldo"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   300
      TabIndex        =   12
      Top             =   4320
      Width           =   2715
   End
   Begin VB.Label lblBoletos 
      BackStyle       =   0  'Transparent
      Caption         =   "Boletos"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   300
      TabIndex        =   11
      Top             =   2820
      Width           =   1275
   End
   Begin VB.Label lblConfiguracao 
      BackStyle       =   0  'Transparent
      Caption         =   "Configuração"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   240
      Left            =   300
      TabIndex        =   3
      Top             =   240
      Width           =   1275
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Dim objAutomacaoBancaria As New AutomacaoBancaria.cUtil
Dim objCresol As AutomacaoBancaria.ICresol
Dim iRetorno As Boolean, retBoleto As String

Private Sub btnConsultarBoleto_Click()
    If iRetorno Then
        objAutomacaoBancaria.BoletoLimparConsulta
        objAutomacaoBancaria.BoletoInformarConsulta "", "09700005022582192020", "", "", 1, "", "", False
        retBoleto = objAutomacaoBancaria.BoletoConsultar()
        
        If objAutomacaoBancaria.Sucesso Then
           MsgBox "Nosso Número: " & objAutomacaoBancaria.nossoNumero & vbNewLine & "Linha Digitável: " & objAutomacaoBancaria.linhaDigitavel & vbNewLine & "Código de Barras: " & objAutomacaoBancaria.codigoBarras, vbInformation + vbOKOnly, "BOLETO: CONSULTA"
        Else
           MsgBox retBoleto, vbExclamation + vbOKOnly, "ERRO"
        End If
    End If
End Sub

Private Sub btnEmitirBoleto_Click()
    If iRetorno Then
        objAutomacaoBancaria.BoletoLimpar
        objAutomacaoBancaria.BoletoDados "1", "PED-126", "DOC-126", 32.92, "2026-08-30", 1, False, False, "", "", "", "2026-11-30", 0, 0, 2, "2026-08-31", 5, 1, "2026-08-31", 2, 1, 1, 10
        objAutomacaoBancaria.PagadorDados "LEANDRO DA SILVA SANTOS", "641.446.582-87", "contaot@lotesis.com.br.com", "41992363930", "", "Rua José Platner", "367", "PLanta Almirante", "Almirante Tamandaré", "PR", "83508-532"
        retBoleto = objAutomacaoBancaria.BoletoEmitir()
    
        If objAutomacaoBancaria.Sucesso Then
           MsgBox "Nosso Número: " & objAutomacaoBancaria.nossoNumero & vbNewLine & "Linha Digitável: " & objAutomacaoBancaria.linhaDigitavel & vbNewLine & "Código de Barras: " & objAutomacaoBancaria.codigoBarras, vbInformation + vbOKOnly, "BOLETO: EMISSÃO"
           If objAutomacaoBancaria.ArquivoPDFGerado <> "" Then MsgBox objAutomacaoBancaria.ArquivoPDFGerado, vbInformation + vbOKOnly, "BOLETO PDF"
        Else
           MsgBox retBoleto, vbExclamation + vbOKOnly, "ERRO"
        End If
    End If
End Sub

Private Sub btnExtrato_Click()
    If iRetorno Then
        objAutomacaoBancaria.ExtratoLimpar
        objAutomacaoBancaria.ExtratoPorPeriodo CDate("2026-07-06"), CDate("2026-07-20")
        retBoleto = objAutomacaoBancaria.ExtratoConsultar()
        
        If objAutomacaoBancaria.Sucesso Then
            MsgBox retBoleto, vbInformation + vbOKOnly, "EXTRATO"
        End If
    End If
End Sub

Private Sub btnSaldo_Click()
    If iRetorno Then
        objAutomacaoBancaria.SaldoLimpar
        retBoleto = objAutomacaoBancaria.SaldoConsultar()
        
        If objAutomacaoBancaria.Sucesso Then
            MsgBox retBoleto, vbInformation + vbOKOnly, "SALDO"
        End If
    End If
End Sub

Private Sub btnBB_Click()
    objAutomacaoBancaria.CNPJCPFEmitente = "29.639.780/0001-04"
    objAutomacaoBancaria.CNPJCPFSoftHouse = "29.639.780/0001-04"
    objAutomacaoBancaria.ClientID = "client-id"
    objAutomacaoBancaria.ClientSecret = "client-secret"
    objAutomacaoBancaria.BBAppKey = "gw-dev-app-key"
    objAutomacaoBancaria.NumeroContrato = 1234567  ' Numero do convenio
    objAutomacaoBancaria.ClientCertificado = "C:\certificados\bb.pfx" ' Opcional, se exigido pela credencial
    objAutomacaoBancaria.ClientCertificadoSenha = "senha"
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"

    iRetorno = objAutomacaoBancaria.Configuracao(1)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: BANCO DO BRASIL"
    End If
End Sub

Private Sub btnSicoob_Click()
    objAutomacaoBancaria.CNPJCPFEmitente = "29.639.780/0001-04"
    objAutomacaoBancaria.CNPJCPFSoftHouse = "29.639.780/0001-04"
    objAutomacaoBancaria.ClientCertificado = "C:\Certificado\certificado.pfx"
    objAutomacaoBancaria.ClientCertificadoBase64 = ""
    objAutomacaoBancaria.ClientCertificadoSenha = "senha-do-certificado"
    objAutomacaoBancaria.ClientID = "client-id"
    objAutomacaoBancaria.ClientSecret = ""
    objAutomacaoBancaria.LicencaDLL = ""
    objAutomacaoBancaria.NumeroContaCorrente = 12345
    objAutomacaoBancaria.NumeroContrato = 1234567
    objAutomacaoBancaria.urlWebhook = ""
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
    
    iRetorno = objAutomacaoBancaria.Configuracao(756)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: SICOOB"
    End If
End Sub

Private Sub btnSicredi_Click()
    objAutomacaoBancaria.CNPJCPFEmitente = "29.639.780/0001-04"
    objAutomacaoBancaria.CNPJCPFSoftHouse = "29.639.780/0001-04"
    objAutomacaoBancaria.SicrediApiKey = "api-key"
    objAutomacaoBancaria.SicrediCodigoAcesso = "codigo-acesso"
    objAutomacaoBancaria.SicrediCodigoBeneficiario = "codigo-beneficiario"
    objAutomacaoBancaria.SicrediCooperativa = "cooperativa"
    objAutomacaoBancaria.SicrediPosto = "posto"
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"

    iRetorno = objAutomacaoBancaria.Configuracao(748)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: SICREDI"
    End If
End Sub

Private Sub cmdBradesco_Click()
    objAutomacaoBancaria.ClientID = "client-id"
    objAutomacaoBancaria.ClientSecret = "client-secret"
    objAutomacaoBancaria.ClientCertificado = "C:\certificados\bradesco.pfx"
    objAutomacaoBancaria.ClientCertificadoSenha = "senha"
    objAutomacaoBancaria.BradescoCodigoUsuario = "APISERVIC"
    objAutomacaoBancaria.BradescoCpfCnpjBeneficiario = "12345678"
    objAutomacaoBancaria.BradescoFilialBeneficiario = "0001"
    objAutomacaoBancaria.BradescoControleBeneficiario = "90"
    objAutomacaoBancaria.BradescoProduto = 9
    objAutomacaoBancaria.BradescoNegociacao = "12345678901"
    objAutomacaoBancaria.BradescoAgencia = "3750"
    objAutomacaoBancaria.BradescoContaCorrente = "75557"
    objAutomacaoBancaria.BradescoTipoConta = "cc"
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
    
    'Para PIX os parametros abaixo tem valores diferentes do que para boleto hibrido
    objAutomacaoBancaria.ClientID = "client-id-pix"
    objAutomacaoBancaria.ClientSecret = "client-secret-pix"

    iRetorno = objAutomacaoBancaria.Configuracao(237)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: BRADESCO"
    End If
End Sub

Private Sub cmdCredisis_Click()
    objAutomacaoBancaria.CredisisToken = "d86bf595d4e62ea6498bc0593c3536806a6d03022c0e276a2501408ff4ea5cc8cd297904b1e216494f362f5daa9be0e04fae3933f08b812111845b85168947d4"
    objAutomacaoBancaria.CredisisConvenio = "022582"
    'objAutomacaoBancaria.CredisisUrl = "https://credisiscobranca.com.br/v2/ws" ' Opcional: URL customizada
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"

    iRetorno = objAutomacaoBancaria.Configuracao(97)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: CREDISIS"
    End If
End Sub

Private Sub cmdCresol_Click()
    objCresol.CresolUsuario = "usuario-cresol"
    objCresol.CresolSenha = "senha-cresol"
    objCresol.CresolAccountId = "" ' Opcional: informe quando o token der acesso a mais de uma conta
    objCresol.CresolIdEspecie = 1 ' idEspecie liberado para a conta (obrigatorio na emissao)

    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"

    iRetorno = objAutomacaoBancaria.Configuracao(133)

    If (iRetorno) Then
        MsgBox "Configuracao realizada com sucesso!", vbInformation + vbOKOnly, "Configuracao: CRESOL"
    End If
End Sub

Private Sub cmdCresolEmitir_Click()
    If iRetorno Then
        ' CRESOL exige pagador com documento/nome/endereco/bairro/cidade/UF/CEP e numeroDocumento.
        ' NossoNumero vazio deixa a API gerar o numero automaticamente.
        objAutomacaoBancaria.BoletoLimpar
        objAutomacaoBancaria.BoletoDados "", "PED-CRES-001", "DOC-CRES-001", 32.92, "2026-12-30"
        objAutomacaoBancaria.PagadorDados "LEANDRO DA SILVA SANTOS", "641.446.582-87", "contaot@lotesis.com.br.com", "41992363930", "", "Rua Jose Platner", "367", "PLanta Almirante", "Almirante Tamandare", "PR", "83508-532"
        retBoleto = objAutomacaoBancaria.BoletoEmitir()

        If objAutomacaoBancaria.Sucesso Then
           MsgBox "Nosso Numero: " & objAutomacaoBancaria.nossoNumero & vbNewLine & "Linha Digitavel: " & objAutomacaoBancaria.linhaDigitavel & vbNewLine & "Codigo de Barras: " & objAutomacaoBancaria.codigoBarras, vbInformation + vbOKOnly, "BOLETO CRESOL: EMISSAO"
        Else
           MsgBox retBoleto, vbExclamation + vbOKOnly, "ERRO"
        End If
    End If
End Sub

Private Sub cmdSantander_Click()
    objAutomacaoBancaria.ClientID = "client-id"
    objAutomacaoBancaria.ClientSecret = "client-secret"
    objAutomacaoBancaria.SantanderWorkspaceId = "workspace-id"
    objAutomacaoBancaria.SantanderCovenantCode = "codigo-convenio"
    objAutomacaoBancaria.SantanderBankNumber = "numero-banco"
    objAutomacaoBancaria.CertificadoDigitalBase64 = "base64-do-pfx"
    objAutomacaoBancaria.CertificadoDigitalSenha = "senha"
    objAutomacaoBancaria.homologacao = True
    objAutomacaoBancaria.HabilitarLog = True
    objAutomacaoBancaria.CaminhoLog = "C:\EkklesiaSoft\Boleto\Logs\"
    objAutomacaoBancaria.CriarArquivoPDF = True
    objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
    
    iRetorno = objAutomacaoBancaria.Configuracao(33)

    If (iRetorno) Then
        MsgBox "Configuração realizada com sucesso!", vbInformation + vbOKOnly, "Configuração: SANTANDER"
    End If
End Sub

Private Sub Form_Load()
    Set objAutomacaoBancaria = New AutomacaoBancaria.cUtil
    
    Set objCresol = objAutomacaoBancaria
End Sub
