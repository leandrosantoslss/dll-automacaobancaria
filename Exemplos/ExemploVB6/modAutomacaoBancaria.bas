Attribute VB_Name = "modAutomacaoBancaria"
'EXEMPLOS DE USO - DLL BOLETO E DLL AUTOMACAO BANCARIA

Private Sub BoletosImprimir_F9()
Dim ComandoSQL As String, e As String, nomeTabela As String, IDCedente As Long, iCont As Long, iRetornoBoleto As Long, cLayoutBoleto As Integer
Dim rsBoletos As GRecordSet, rsBoleto As GRecordSet, rsCedente As GRecordSet, rsCliente As GRecordSet, rsCidades As New GRecordSet
Dim xBoletoQROK As Boolean, xBoletoSemRemessa As String, xBoletoSelecao As String, xBoletoQRSelecao As String
Dim JuroDia As Double, iRetorno As Boolean

   ComandoSQL = "SELECT ContasReceber.CodigoCedente, Cedente.UsarWSAPI " & _
                "FROM ContasReceber " & _
                "     INNER JOIN Cedente ON ContasReceber.CodigoCedente = Cedente.CodigoCedente " & _
                "WHERE CodigoReceber in (" & Left(xContasSelecao, Len(xContasSelecao) - 1) & ") " & _
                "GROUP BY ContasReceber.CodigoCedente, Cedente.UsarWSAPI"
   Set rsCedente = vgDb(2).OpenRecordSet(ComandoSQL, , , , , , , , , True)
   
   Set rsCidades.Database = vgDb(2)
   rsCidades.Source = "Cidades"
   rsCidades.Top = 1
   
   xBoletoSelecao = ""
   xBoletoSemRemessa = ""
   Do While Not rsCedente.EOF
      If rsCedente!UsarWSAPI Then
boletoWSConsulta:
         xBoletoSelecao = ""
         xBoletoSemRemessa = ""
         xBoletoQRSelecao = ""
         ComandoSQL = "SELECT CodigoReceber, QRCode, RemessaGerada " & _
                      "FROM ContasReceber " & _
                      "WHERE CodigoReceber in (" & Left(xContasSelecao, Len(xContasSelecao) - 1) & ")"
         Set rsBoletos = vgDb(2).OpenRecordSet(ComandoSQL, , , , , , , , , True)
                   
         Do While Not rsBoletos.EOF
            If rsBoletos!RemessaGerada Then
               If Vazio(rsBoletos!QRCode) Then
                  SelAdicionaValor xBoletoSelecao, rsBoletos("CodigoReceber")
               Else
                  SelAdicionaValor xBoletoQRSelecao, rsBoletos("CodigoReceber")
               End If
            Else
               SelAdicionaValor xBoletoSemRemessa, rsBoletos("CodigoReceber")
            End If
            'Vai para o proximo
            rsBoletos.MoveNext
         Loop
         
         'Se tiver boletos sem remessa, vamos enviar/registrar eles antes
         If Not Vazio(xBoletoSemRemessa) Then
            iRetorno = BoletoRegistraAPI(rsCedente!CodigoCedente, Left(xBoletoSemRemessa, Len(xBoletoSemRemessa) - 1))
            DoEvents
            GoTo boletoWSConsulta
         End If
         
         If Not Vazio(xBoletoSelecao) Then
            iRetorno = BoletoConsultaAPI(rsCedente!CodigoCedente, Left(xBoletoSelecao, Len(xBoletoSelecao) - 1))
         End If
      Else
         xBoletoSelecao = ""
         xBoletoSemRemessa = ""
         xBoletoQRSelecao = ""
         ComandoSQL = "SELECT CodigoReceber, QRCode, RemessaGerada " & _
                      "FROM ContasReceber " & _
                      "WHERE CodigoReceber in (" & Left(xContasSelecao, Len(xContasSelecao) - 1) & ")"
         Set rsBoletos = vgDb(2).OpenRecordSet(ComandoSQL, , , , , , , , , True)
                   
         Do While Not rsBoletos.EOF
            If rsBoletos!RemessaGerada Then
               SelAdicionaValor xBoletoSelecao, rsBoletos("CodigoReceber")
            Else
               SelAdicionaValor xBoletoSemRemessa, rsBoletos("CodigoReceber")
            End If
            'Vai para o proximo
            rsBoletos.MoveNext
         Loop
         
         If Not Vazio(xBoletoSemRemessa) Then
            MsgBox "O(s) Boleto(s) selecionado(s) não está(ão) registrado(s) ou remessa gerada para ele(s)!" & vbNewLine & "É necessário gerar e enviar a Remessa Bancária antes da impressão!" & vbNewLine & "Boleto(s): " & Left(xBoletoSemRemessa, Len(xBoletoSemRemessa) - 1), vbInformation + vbOKOnly, vgAtencao
         End If
      End If
      rsCedente.MoveNext
   Loop
   
   Set rsCedente = Nothing
   Set rsBoletos = Nothing
   
   xBoletoSemRemessa = ""

ImprimirBoletoSemQR:
   nomeTabela = "##vCReceber_" & Format(Now, "yyyymmddhhMMss")
   
   If Not xBoletoQROK And Not Vazio(xBoletoQRSelecao) Then
      ComandoSQL = "SELECT CodigoReceber " & _
                   "INTO " & nomeTabela & " " & _
                   "FROM ContasReceber " & _
                   "WHERE CodigoReceber in (" & Left(xBoletoQRSelecao, Len(xBoletoQRSelecao) - 1) & ") " & _
                   "ORDER BY CodigoReceber"
   ElseIf Not Vazio(xBoletoSelecao) Then
      xBoletoQROK = True
      ComandoSQL = "SELECT CodigoReceber " & _
                   "INTO " & nomeTabela & " " & _
                   "FROM ContasReceber " & _
                   "WHERE CodigoReceber in (" & Left(xBoletoSelecao, Len(xBoletoSelecao) - 1) & ") " & _
                   "ORDER BY CodigoReceber"
   Else
      'TR_ExecutaModulos "(Reimprimir Parcelas Selecionadas)", vgPWGrupo, "CodigoReceber|" & Left(xContasSelecao, Len(xContasSelecao) - 1), cpJurosAtrazo.Value & "|" & cpMulta.Value & "|" & xdataJuros & "|" & "LoteSis - Versão " & CStr(App.Major) & "." & CStr(App.Minor) & "." & CStr(App.Revision) & "|" & xdataIni & "|" & xDataFin & "|" & xStatus & "|" & xTpData & "|" & xCedente
      Exit Sub
   End If
   e$ = SQLExecuta(ComandoSQL)
   DoEvents
   
   ComandoSQL = "SELECT CodigoCedente, ISNULL(COUNT(CodigoCedente), 0) r " & _
                "FROM ContasReceber " & _
                "WHERE CodigoReceber in (SELECT CodigoReceber FROM " & nomeTabela & ") " & _
                "GROUP BY CodigoCedente"
   
   Set rsBoletos = vgDb(2).OpenRecordSet(ComandoSQL, , , , , , , , , True)
     
   If rsBoletos.RecordCount = 0 Then Exit Sub
   
   If rsBoletos.RecordCount > 1 Or rsBoletos!CodigoCedente = 0 Then
ImprimeTask:
      'TR_ExecutaModulos "(Reimprimir Parcelas Selecionadas)", vgPWGrupo, "CodigoReceber|(SELECT CodigoReceber FROM " & nomeTabela & ")", cpJurosAtrazo.Value & "|" & cpMulta.Value & "|" & xdataJuros & "|" & "LoteSis - Versão " & CStr(App.Major) & "." & CStr(App.Minor) & "." & CStr(App.Revision) & "|" & xdataIni & "|" & xDataFin & "|" & xStatus & "|" & xTpData & "|" & xCedente
      'TR_ExecutaModulos "(Reimprimir Parcelas Selecionadas)", vgPWGrupo, "IDContaRec|" & Left(xContasSelecao, Len(xContasSelecao) - 1), cpJurosAtrazo.Value & "|" & cpMulta.Value & "|" & xdataJuros & "|" & "LoteSis - Versão " & CStr(App.Major) & "." & CStr(App.Minor) & "." & CStr(App.Revision) & "|" & xDataIni & "|" & xDataFin & "|" & xStatus & "|" & xTpData & "|" & xCedente
   Else
      IDCedente = SQLExecutaRetorno(ComandoSQL, "CodigoCedente", 0)
      Set rsCedente = vgDb(2).OpenRecordSet("SELECT * FROM Cedente WHERE CodigoCedente = " & IDCedente, , , , , , , , , True)
      If rsBoletos.RecordCount = 0 Then GoTo ImprimeTask
            
      cLayoutBoleto = 6
      
      ComandoSQL = "SELECT CodigoReceber " & _
                   "FROM " & nomeTabela & " " & _
                   "ORDER BY CodigoReceber"
      Set rsBoletos = vgDb(2).OpenRecordSet(ComandoSQL, , , , , , , , , True)
      
      If rsBoletos.RecordCount = 0 Then Exit Sub
      
      iCont = rsBoletos.RecordCount - 1
      ReDim BoletoID(iCont)
      ReDim BoletoNumDocumento(iCont)
      ReDim BoletoNossoNumero(iCont)
      ReDim BoletoDataDocumento(iCont)
      ReDim BoletoVencimento(iCont)
      ReDim BoletoValor(iCont)
      ReDim BoletoJurosDia(iCont)
      ReDim BoletoDesconto(iCont)
      ReDim BoletoAcrescimo(iCont)
      ReDim BoletoParcela(iCont)
      ReDim BoletoParcelaTotal(iCont)
      ReDim BoletoMensagem(iCont)
      ReDim BoletoInstrucao1(iCont)
      ReDim BoletoInstrucao2(iCont)
      ReDim BoletoTipoOcorrencia(iCont)
      ReDim BoletoDataJurosMora(iCont)
      ReDim BoletoTipoMulta(iCont)
      ReDim BoletoMulta(iCont)
      ReDim BoletoLimitePagamento(iCont)
      ReDim BoletoPIXDescricao(iCont)
      ReDim BoletoPIXTXID(iCont)
      ReDim BoletoLinhaDigitavel(iCont)
      ReDim BoletoCodigoBarras(iCont)
      ReDim BoletoPIXCopiaCola(iCont)
      ReDim BoletoPIXQRCode(iCont)
      'Dados do Sacado
      ReDim SacadosCNPJCPF(iCont)
      ReDim SacadosRazaoSocial(iCont)
      ReDim SacadosEndereco(iCont)
      ReDim SacadosEnderecoNumero(iCont)
      ReDim SacadosComplemento(iCont)
      ReDim SacadosBairro(iCont)
      ReDim SacadosCidade(iCont)
      ReDim SacadosCEP(iCont)
      ReDim SacadosUF(iCont)
      ReDim SacadosEmail(iCont)
      ReDim SacadosAvalista(iCont)
      iCont = 0
      
      Do While Not rsBoletos.EOF
      
        Set rsBoleto = vgDb(2).OpenRecordSet("SELECT * FROM ContasReceber WHERE CodigoReceber = " & rsBoletos!CodigoReceber, , , , , , , , , True)
        Set rsCliente = vgDb(2).OpenRecordSet("SELECT * FROM Pessoas WHERE CodigoPessoa = " & rsBoleto!CodigoCliente, , , , , , , , , True)
        ComandoSQL = "SELECT Endereco, Numero, Complemento, Bairro, UF, CEP, CodigoIBGE FROM PessoasEnderecos WHERE CodigoPessoa = " & rsCliente!CodigoPessoa & " AND Principal <> 0"
        rsCidades.PTab "CodigoIBGE", SQLExecutaRetorno(ComandoSQL, "CodigoIBGE", 1100122)
        
        JuroDia = Round(rsBoleto!Valor * (rsCedente!Juros / 30) / 100, 2)
        BoletoID(iCont) = Trim(Str(rsBoleto!CodigoReceber))
        BoletoNumDocumento(iCont) = Trim(CStr(rsBoleto!CodigoReceber))
        BoletoNossoNumero(iCont) = Trim(Str(rsBoleto!CodigoBoleto))
        BoletoDataDocumento(iCont) = Format(rsBoleto!DataEmissao, "yyyy-mm-dd")
        BoletoVencimento(iCont) = Format(rsBoleto!DataVencimento, "yyyy-mm-dd")
        BoletoValor(iCont) = Trim(CStr(Format(rsBoleto!Valor, "#0.00")))
        BoletoJurosDia(iCont) = Trim(CStr(Format(Round(JuroDia, 2), "#0.00")))
        BoletoDesconto(iCont) = "0.00|"
        BoletoAcrescimo(iCont) = "0,00"
        If rsBoleto!SequenciaParcela = 100 Then
           BoletoParcela(iCont) = "010"
        ElseIf rsBoleto!SequenciaParcela > 100 Then
           BoletoParcela(iCont) = "001"
        Else
           BoletoParcela(iCont) = Trim(CStr(rsBoleto!SequenciaParcela))
        End If
        BoletoParcelaTotal(iCont) = Trim(rsBoleto!ParcelaTotal)
        BoletoMensagem(iCont) = rsCedente!InstrucoesCaixa
        BoletoInstrucao1(iCont) = IIf(rsBoleto!CodigoReserva > 0, "ENT - ", "") & rsBoleto!HistoricoParcela
        BoletoInstrucao2(iCont) = IIf(rsCedente!NaoMostraMensagemPadraoDLL, rsCedente!InstrucoesCaixa, "")
        BoletoTipoOcorrencia(iCont) = "01"
        BoletoDataJurosMora(iCont) = Format(rsBoleto!DataVencimento, "yyyy-mm-dd")
        BoletoTipoMulta(iCont) = "2"
        BoletoMulta(iCont) = Trim(CStr(Format(rsCedente!Multa, "#0.00")))
        BoletoLimitePagamento(iCont) = Format(DateAdd("d", 90, rsBoleto!DataVencimento), "yyyy/mm/dd")
        BoletoPIXDescricao(iCont) = ""     'BoletoNumDocumento(iCont)
        BoletoPIXTXID(iCont) = Trim(Str(rsBoleto!CodigoReceber))
        BoletoLinhaDigitavel(iCont) = Trim(rsBoleto!linhaDigitavel)
        BoletoCodigoBarras(iCont) = Trim(rsBoleto!codigoBarras)
        BoletoPIXCopiaCola(iCont) = ""
        BoletoPIXQRCode(iCont) = Trim(rsBoleto!QRCode)
        'Dados do Sacado
        SacadosCNPJCPF(iCont) = Retira(rsCliente!CNPJCPF, "-,.,/", UM_A_UM)
        SacadosRazaoSocial(iCont) = Trim(rsCliente!RazaoSocial)
        SacadosEndereco(iCont) = SQLExecutaRetorno(ComandoSQL, "Endereco", "")
        SacadosEnderecoNumero(iCont) = SQLExecutaRetorno(ComandoSQL, "Numero", "")
        SacadosComplemento(iCont) = SQLExecutaRetorno(ComandoSQL, "Complemento", "")
        SacadosBairro(iCont) = SQLExecutaRetorno(ComandoSQL, "Bairro", "")
        SacadosCidade(iCont) = Trim(rsCidades!Municipio)
        SacadosCEP(iCont) = SQLExecutaRetorno(ComandoSQL, "CEP", "")
        SacadosUF(iCont) = SQLExecutaRetorno(ComandoSQL, "UF", "")
        SacadosEmail(iCont) = SQLExecutaRetorno("SELECT TOP 1 Email FROM PessoasEmails WHERE CodigoPessoa = " & rsCliente!CodigoPessoa, "Email", "")
        SacadosAvalista(iCont) = "" 'SacadosCR!NomeAvalista & "|" & SacadosCR!CPFAvalista
        
        iCont = iCont + 1
             
        rsBoletos.MoveNext
      Loop
      
      iRetornoBoleto = BoletoImprimirDLL(IDCedente, cLayoutBoleto, False, "")
      
      If Not xBoletoQROK And Not Vazio(xBoletoQRSelecao) Then
         xBoletoQROK = True
         GoTo ImprimirBoletoSemQR
      End If
      
      xBoletoSelecao = ""
      xBoletoQRSelecao = ""
      
      Set rsBoletos = Nothing
      Set rsBoleto = Nothing
      Set rsCliente = Nothing
      Set rsCedente = Nothing
   End If
End Sub

Public Function BoletoImprimirDLL(cCedente As Long, ModeloImpressao As Integer, somentePDF As Boolean, ArquivoPDF As String) As Boolean
   'ModeloImpressao: 1 - Carne / 2 - Fatura / 3 - Padrão A4 / 4 - Comprovante de Entrega / 5 - Padrão A4 Hibrido / 6 - Carne Hibrido / 7 - Comprovante de Entrega Hibrido
   
   On Error GoTo deuErro

   Dim objBoleto As BoletoDLL.cBoleto, iRetornoBoleto As Long
   Dim rsCedente As New GRecordSet
   
   Set rsCedente = vgDb(2).OpenRecordSet("SELECT * FROM Cedente WHERE CodigoCedente = " & cCedente, , , , , , , , , True)
   If rsCedente.RecordCount = 0 Then GoTo semCedente
   
   Set objBoleto = New BoletoDLL.cBoleto
   
   objBoleto.usaFastReports = True
   objBoleto.LicencaDLL = rsCedente!LicencaDLL
   objBoleto.SoftHouseCNPJ = CNPJSoftHouse
   objBoleto.nomePDF = ArquivoPDF
   objBoleto.somentePDF = somentePDF
   
   ModeloImpressao = 6
   
   If rsCedente!UsarWSAPI Then
      objBoleto.usaBoletoHibrido = True
      objBoleto.somenteImpressao = True
      objBoleto.BancoGeraNossoNumero = False
      objBoleto.web = False
      
      iRetornoBoleto = objBoleto.Cedente(rsCedente!CodigoBanco, rsCedente!Agencia, rsCedente!Agencia_Dv, rsCedente!Cedente, rsCedente!Cedente_Dv, rsCedente!Carteira, rsCedente!CodigoConvenio, rsCedente!CNPJCPF, rsCedente!NomeCedente, rsCedente!Logradouro, rsCedente!Numero, rsCedente!Complemento, rsCedente!Bairro, rsCedente!CEP, rsCedente!Cidade, rsCedente!UF, "N", rsCedente!LayoutArquivo, rsCedente!LayoutLote, rsCedente!LicencaDLL, "EkklesiaSoft - Desenvolvimento de Sistemas")
      
      Select Case ModeloImpressao
        Case 1: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCarne, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 2: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolFatura, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 3: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolPadrao, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 4: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCompEntrega, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 5: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolHibrido, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 6: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCarneHibrido, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case 7: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCompEntregaHibrido, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
        Case Else: iRetornoBoleto = objBoleto.ImprimirBoleto(rsCedente!CodigoBanco, rsCedente!CNPJCPF, BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCarne, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 90, _
                                                          BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                          BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoPIXDescricao(), BoletoPIXTXID(), BoletoLinhaDigitavel(), BoletoCodigoBarras(), BoletoPIXCopiaCola(), BoletoPIXQRCode(), "N", Left(rsCedente!ProtestoCodigo, 1), True)
      End Select
         
   Else
   
      iRetornoBoleto = objBoleto.ImprimirVariosSacados3(rsCedente!CodigoBanco, rsCedente!Agencia, rsCedente!Agencia_Dv, rsCedente!Cedente, rsCedente!Cedente_Dv, rsCedente!Carteira, rsCedente!CodigoConvenio, rsCedente!CNPJCPF, rsCedente!NomeCedente, rsCedente!Logradouro, rsCedente!Numero, rsCedente!Complemento, rsCedente!Bairro, rsCedente!CEP, rsCedente!Cidade, rsCedente!UF, _
                                                        BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCarne, rsCedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 0, _
                                                        BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), _
                                                        BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), rsCedente!EspecieDocumento, False, "", BoletoParcela(), BoletoParcelaTotal(), BoletoDataJurosMora(), "N", Left(rsCedente!ProtestoCodigo, 1), rsCedente!LicencaDLL, CNPJSoftHouse)
   End If
   
   Set rsCedente = Nothing
   Set objBoleto = Nothing

   BoletoImprimirDLL = iRetornoBoleto
   Exit Function
   
semCedente:
   MsgBox "Cadastro do Cedente não localizado!", vbInformation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   Exit Function
   
deuErro:
   MsgBox Err.Description, vbCritical + vbOKOnly, vgAtencao
   Err.Clear
   BoletoImprimirDLL = False
End Function

Public Sub BoletoRemessaConsultaAPI(IDCedente As Long, numRemessa As Long)
Dim remessaJSON As String, rsCedente As New GRecordSet, rsBoletos As New GRecordSet, vsSQL As String
Dim iRetornoBoleto As Long, retBoleto As String, p As Object, i As Long, iCont As Long, iContEnvio As Long, cQtdeRegistros As Long, processando As String
Dim objAutomacaoBancaria As AutomacaoBancaria.cUtil, cPagador As Long, cPagadorCPFCNPJ As String, cToken As String
   
   On Error GoTo deuErro
   
   Set rsCedente = vgDb(2).OpenRecordSet("SELECT * FROM TbCedente WHERE IdCedente = " & IDCedente, , , , , , , , , True)
   If rsCedente.RecordCount = 0 Then GoTo semCedente
   
   Set objAutomacaoBancaria = New AutomacaoBancaria.cUtil
   
   vsSQL = "SELECT CodigoReceber, TaskID, CodigoBoleto, NossoNumero, LinhaDigitavel, CodigoBarras FROM ContasReceber WHERE CodigoCedente = " & IDCedente & " AND CodigoRemessaBanco = " & numRemessa
   Set rsBoletos = vgDb(2).OpenRecordSet(vsSQL, , , , , , , , , True)
   
   iCont = 0
   cQtdeRegistros = (rsBoletos.RecordCount - 1)
   iContEnvio = 0
   For i = 0 To cQtdeRegistros
        iContEnvio = (iContEnvio + 1)
        If i = 0 Then
           objAutomacaoBancaria.CNPJCPFEmitente = rsCedente!CNPJCPF
           objAutomacaoBancaria.CNPJCPFSoftHouse = CNPJSoftHouse
           objAutomacaoBancaria.ClientCertificado = ""  '"C:\Users\leand\OneDrive\EkklesiaSoft\Certificado\certificado.pfx"
           objAutomacaoBancaria.ClientCertificadoBase64 = rsCedente!WSAPIClientCertificado
           objAutomacaoBancaria.ClientCertificadoSenha = rsCedente!WSAPIClientCertificadoSenha
           objAutomacaoBancaria.ClientID = rsCedente!WSAPIClientID
           objAutomacaoBancaria.ClientSecret = rsCedente!WSAPIClientSecret
           objAutomacaoBancaria.LicencaDLL = rsCedente!WSAPILicencaDLL
           objAutomacaoBancaria.NumeroContaCorrente = rsCedente!Cedente & rsCedente!Cedente_Dv
           objAutomacaoBancaria.NumeroContrato = rsCedente!CodigoConvenio
           objAutomacaoBancaria.urlWebhook = rsCedente!WSAPIURL
           objAutomacaoBancaria.HabilitarLog = True
           objAutomacaoBancaria.CaminhoLog = rsCedente!DiretorioRemessa & "\Logs\"
           objAutomacaoBancaria.CriarArquivoPDF = True
           objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
            
           iRetornoBoleto = objAutomacaoBancaria.Configuracao(rsCedente!CodigoBanco)
        End If
        
        If iRetornoBoleto Then
           Call objAutomacaoBancaria.BoletoLimparConsulta
           Call objAutomacaoBancaria.BoletoInformarConsulta(rsBoletos!CodigoReceber, rsBoletos!nossoNumero, "", "", 1, "", "")
           retBoleto = objAutomacaoBancaria.BoletoConsultar()
        End If
        
        If objAutomacaoBancaria.Sucesso Then
            vsSQL = "UPDATE ContasReceber SET " & _
                    "NossoNumero = '" & objAutomacaoBancaria.nossoNumero & "', " & _
                    "LinhaDigitavel = '" & objAutomacaoBancaria.linhaDigitavel & "', " & _
                    "CodigoBarras = '" & objAutomacaoBancaria.codigoBarras & "', " & _
                    "QRCode = '" & objAutomacaoBancaria.QRCode & "', " & _
                    "WSRetornoEnvio = '" & retBoleto & "' " & _
                    "WHERE CodigoReceber = " & rsBoletos!CodigoReceber
            SQLExecuta vsSQL
        End If
        
        rsBoletos.MoveNext
   Next
   
   Exit Sub
   Resume
   
semCedente:
   MsgBox "Cadastro do Cedente não localizado!", vbInformation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   Exit Sub

DeuExcecao:
   MsgBox retBoleto, vbExclamation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
   Exit Sub
   
deuErro:
   MsgBox Err.Description + vbCrLf + processando, vbCritical + vbOKOnly, vgAtencao
   Err.Clear
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
End Sub

Public Function BoletoConsultaAPI(IDCedente As Long, boletoLista As String) As Boolean
Dim remessaJSON As String, rsCedente As New GRecordSet, rsBoletos As New GRecordSet, vsSQL As String
Dim iRetornoBoleto As Boolean, retBoleto As String, p As Object, i As Long, iCont As Long, iContEnvio As Long, cQtdeRegistros As Long, processando As String
Dim objAutomacaoBancaria As AutomacaoBancaria.cUtil, cPagador As Long, cPagadorCPFCNPJ As String, cToken As String
   
   On Error GoTo deuErro
   
   Set rsCedente = vgDb(2).OpenRecordSet("SELECT * FROM Cedente WHERE CodigoCedente = " & IDCedente, , , , , , , , , True)
   If rsCedente.RecordCount = 0 Then GoTo semCedente
   
   Set objAutomacaoBancaria = New AutomacaoBancaria.cUtil
   
   vsSQL = "SELECT CodigoReceber, TaskID, CodigoBoleto, NossoNumero, LinhaDigitavel, CodigoBarras FROM ContasReceber WHERE CodigoCedente = " & IDCedente & " AND CodigoReceber in (" & boletoLista & ")"
   Set rsBoletos = vgDb(2).OpenRecordSet(vsSQL, , , , , , , , , True)
   
   iCont = 0
   cQtdeRegistros = (rsBoletos.RecordCount - 1)
   iContEnvio = 0
   For i = 0 To cQtdeRegistros
        iContEnvio = (iContEnvio + 1)
        If i = 0 Then
           objAutomacaoBancaria.CNPJCPFEmitente = rsCedente!CNPJCPF
           objAutomacaoBancaria.CNPJCPFSoftHouse = CNPJSoftHouse
           objAutomacaoBancaria.ClientCertificado = ""  '"C:\Users\leand\OneDrive\EkklesiaSoft\Certificado\certificado.pfx"
           objAutomacaoBancaria.ClientCertificadoBase64 = rsCedente!WSAPIClientCertificado
           objAutomacaoBancaria.ClientCertificadoSenha = rsCedente!WSAPIClientCertificadoSenha
           objAutomacaoBancaria.ClientID = rsCedente!WSAPIClientID
           objAutomacaoBancaria.ClientSecret = rsCedente!WSAPIClientSecret
           objAutomacaoBancaria.LicencaDLL = rsCedente!WSAPILicencaDLL
           objAutomacaoBancaria.NumeroContaCorrente = rsCedente!Cedente & rsCedente!Cedente_Dv
           objAutomacaoBancaria.NumeroContrato = rsCedente!CodigoConvenio
           objAutomacaoBancaria.urlWebhook = rsCedente!WSAPIURL
           objAutomacaoBancaria.HabilitarLog = True
           objAutomacaoBancaria.CaminhoLog = rsCedente!DiretorioRemessa & "\Logs\"
           objAutomacaoBancaria.CriarArquivoPDF = True
           objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
            
           iRetornoBoleto = objAutomacaoBancaria.Configuracao(rsCedente!CodigoBanco)
        End If
        
        If iRetornoBoleto Then
           Call objAutomacaoBancaria.BoletoLimparConsulta
           Call objAutomacaoBancaria.BoletoInformarConsulta("", rsBoletos!CodigoBoleto, "", "", 1, "", "")
           retBoleto = objAutomacaoBancaria.BoletoConsultar()
        End If
        
        If objAutomacaoBancaria.Sucesso Then
            vsSQL = "UPDATE ContasReceber SET " & _
                    "NossoNumero = '" & objAutomacaoBancaria.nossoNumero & "', " & _
                    "LinhaDigitavel = '" & objAutomacaoBancaria.linhaDigitavel & "', " & _
                    "CodigoBarras = '" & objAutomacaoBancaria.codigoBarras & "', " & _
                    "QRCode = '" & objAutomacaoBancaria.QRCode & "', " & _
                    "WSRetornoEnvio = '" & retBoleto & "' " & _
                    "WHERE CodigoReceber = " & rsBoletos!CodigoReceber
            SQLExecuta vsSQL
        End If
                
        rsBoletos.MoveNext
   Next
   
   BoletoConsultaAPI = True
   
   Exit Function
   Resume
   
semCedente:
   MsgBox "Cadastro do Cedente não localizado!", vbInformation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   BoletoConsultaAPI = False
   Exit Function

DeuExcecao:
   MsgBox objAutomacaoBancaria.JsonEnvio, vbExclamation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
   BoletoConsultaAPI = False
   Exit Function
   
deuErro:
   MsgBox Err.Description + vbCrLf + processando, vbCritical + vbOKOnly, vgAtencao
   Err.Clear
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
   BoletoConsultaAPI = False
End Function

Public Function BoletoRegistraAPI(IDCedente As Long, boletoLista As String) As Boolean
Dim remessaJSON As String, rsCedente As New GRecordSet, rsBoletos As New GRecordSet, rsCidades As New GRecordSet
Dim iRetornoBoleto As Boolean, retBoleto As String, p As Object, i As Long, iCont As Long, iContEnvio As Long, cQtdeRegistros As Long, processando As String
Dim objAutomacaoBancaria As AutomacaoBancaria.cUtil, cPagador As Long, cPagadorCPFCNPJ As String, cToken As String, vsSQL As String
Dim JuroDia As Double, cRemessa As Long, ComandoSQL As String
   
   On Error GoTo deuErro
   
   Set rsCedente = vgDb(2).OpenRecordSet("SELECT * FROM Cedente WHERE CodigoCedente = " & IDCedente, , , , , , , , , True)
   If rsCedente.RecordCount = 0 Then GoTo semCedente
   
   Set objAutomacaoBancaria = New AutomacaoBancaria.cUtil
   
   vsSQL = "SELECT cr.CodigoReceber, cr.CodigoParcela, cr.DataEmissao, cr.DataVencimento, cr.Valor, cr.SequenciaParcela, cr.ParcelaTotal, cr.HistoricoParcela, cr.CodigoBoleto, " & _
           "       c.CodigoPessoa, c.RazaoSocial, c.CNPJCPF, c.TipoPessoa, c.DataNascimento, cr.NossoNumero, cr.LinhaDigitavel, cr.CodigoBarras, cr.QRCode, cr.API " & _
           "FROM ContasReceber as cr " & _
           "     LEFT OUTER JOIN Pessoas as c On cr.CodigoCliente = c.CodigoPessoa " & _
           "WHERE cr.CodigoCedente = " & IDCedente & " AND cr.CodigoReceber in (" & boletoLista & ") " & _
           "ORDER BY cr.CodigoReceber"
   Set rsBoletos = vgDb(2).OpenRecordSet(vsSQL, , , , , , , , , True)
   
   Set rsCidades.Database = vgDb(2)
   rsCidades.Source = "Cidades"
   rsCidades.Top = 1
     
   iCont = 0
   ReDim BoletoID(iCont)
   ReDim BoletoNumDocumento(iCont)
   ReDim BoletoNossoNumero(iCont)
   ReDim BoletoDataDocumento(iCont)
   ReDim BoletoVencimento(iCont)
   ReDim BoletoValor(iCont)
   ReDim BoletoJurosDia(iCont)
   ReDim BoletoDesconto(iCont)
   ReDim BoletoAcrescimo(iCont)
   ReDim BoletoParcela(iCont)
   ReDim BoletoParcelaTotal(iCont)
   ReDim BoletoMensagem(iCont)
   ReDim BoletoInstrucao1(iCont)
   ReDim BoletoInstrucao2(iCont)
   ReDim BoletoTipoOcorrencia(iCont)
   ReDim BoletoDataJurosMora(iCont)
   ReDim BoletoTipoMulta(iCont)
   ReDim BoletoMulta(iCont)
   ReDim BoletoLimitePagamento(iCont)
   'Dados do Sacado
   ReDim SacadosCNPJCPF(iCont)
   ReDim SacadosRazaoSocial(iCont)
   ReDim SacadosEndereco(iCont)
   ReDim SacadosEnderecoNumero(iCont)
   ReDim SacadosComplemento(iCont)
   ReDim SacadosBairro(iCont)
   ReDim SacadosCidade(iCont)
   ReDim SacadosCEP(iCont)
   ReDim SacadosUF(iCont)
   ReDim SacadosTelefone(iCont)
   ReDim SacadosEmail(iCont)
   ReDim SacadosDataNascimento(iCont)
   ReDim SacadosAvalista(iCont)
   
   iCont = 0
   cQtdeRegistros = (rsBoletos.RecordCount - 1)
   iContEnvio = 0
   Do While Not rsBoletos.EOF
        If iContEnvio = 0 Then
                     
           objAutomacaoBancaria.CNPJCPFEmitente = rsCedente!CNPJCPF
           objAutomacaoBancaria.CNPJCPFSoftHouse = CNPJSoftHouse
           objAutomacaoBancaria.ClientCertificado = ""  '"C:\Users\leand\OneDrive\EkklesiaSoft\Certificado\certificado.pfx"
           objAutomacaoBancaria.ClientCertificadoBase64 = rsCedente!WSAPIClientCertificado
           objAutomacaoBancaria.ClientCertificadoSenha = rsCedente!WSAPIClientCertificadoSenha
           objAutomacaoBancaria.ClientID = rsCedente!WSAPIClientID
           objAutomacaoBancaria.ClientSecret = rsCedente!WSAPIClientSecret
           objAutomacaoBancaria.LicencaDLL = rsCedente!WSAPILicencaDLL
           objAutomacaoBancaria.NumeroContaCorrente = rsCedente!Cedente & rsCedente!Cedente_Dv
           objAutomacaoBancaria.NumeroContrato = rsCedente!CodigoConvenio
           objAutomacaoBancaria.urlWebhook = rsCedente!WSAPIURL
           objAutomacaoBancaria.HabilitarLog = True
           objAutomacaoBancaria.CaminhoLog = rsCedente!DiretorioRemessa & "\Logs\"
           objAutomacaoBancaria.CriarArquivoPDF = True
           objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
            
           iRetornoBoleto = objAutomacaoBancaria.Configuracao(rsCedente!CodigoBanco)
           
           iContEnvio = 1
        End If
        
        'Dados do Boleto
        processando = "Dados do Boleto"
        JuroDia = Round(rsBoletos!Valor * (rsCedente!Juros / 30) / 100, 2)
        BoletoID(iCont) = Trim(CStr(rsBoletos!CodigoReceber))
        BoletoNumDocumento(iCont) = Trim$(CStr(rsBoletos!CodigoReceber))
        BoletoNossoNumero(iCont) = Trim(CStr(rsBoletos!CodigoBoleto))
        BoletoDataDocumento(iCont) = Format(rsBoletos!DataEmissao, "yyyy/mm/dd")
        BoletoVencimento(iCont) = Format(rsBoletos!DataVencimento, "yyyy/mm/dd")
        BoletoValor(iCont) = Trim(CStr(Format(rsBoletos!Valor, "#0.00")))
        BoletoJurosDia(iCont) = Trim(CStr(Format(Round(JuroDia, 2), "#0.00")))
        BoletoDesconto(iCont) = "0,00"
        BoletoAcrescimo(iCont) = "0,00"
        If rsBoletos!SequenciaParcela = 100 Then
           BoletoParcela(iCont) = "010"
        ElseIf rsBoletos!SequenciaParcela > 100 Then
           BoletoParcela(iCont) = "001"
        Else
           BoletoParcela(iCont) = Trim(CStr(rsBoletos!SequenciaParcela))
        End If
        BoletoParcelaTotal(iCont) = Trim(rsBoletos!ParcelaTotal)
        BoletoMensagem(iCont) = rsCedente!InstrucoesCaixa
        BoletoInstrucao1(iCont) = "Parc.: " & Trim(CStr(rsBoletos!CodigoParcela)) & " " & Trim(CStr(rsBoletos!SequenciaParcela)) & "/" & Trim(rsBoletos!ParcelaTotal)
        BoletoInstrucao2(iCont) = "" 'CedenteCR!Linha2
        BoletoTipoOcorrencia(iCont) = "01"
        BoletoDataJurosMora(iCont) = Format(rsBoletos!DataVencimento, "yyyy/mm/dd")
        BoletoTipoMulta(iCont) = "2"
        BoletoMulta(iCont) = Trim(CStr(Format(rsCedente!Multa, "#0.00")))
        BoletoLimitePagamento(iCont) = Format(DateAdd("d", 90, rsBoletos!DataVencimento), "yyyy/mm/dd")
        'Dados do Sacado
        ComandoSQL = "SELECT Endereco, Numero, Complemento, Bairro, UF, CEP, CodigoIBGE FROM PessoasEnderecos WHERE CodigoPessoa = " & rsBoletos!CodigoPessoa & " AND Principal <> 0"
        rsCidades.PTab "CodigoIBGE", SQLExecutaRetorno(ComandoSQL, "CodigoIBGE", 1100122)
        SacadosCNPJCPF(iCont) = Retira(rsBoletos!CNPJCPF, "-,.,/", UM_A_UM)
        SacadosRazaoSocial(iCont) = rsBoletos!RazaoSocial
        SacadosEndereco(iCont) = SQLExecutaRetorno(ComandoSQL, "Endereco", "")
        SacadosEnderecoNumero(iCont) = SQLExecutaRetorno(ComandoSQL, "Numero", "")
        SacadosComplemento(iCont) = SQLExecutaRetorno(ComandoSQL, "Complemento", "")
        SacadosBairro(iCont) = SQLExecutaRetorno(ComandoSQL, "Bairro", "")
        SacadosCidade(iCont) = Trim(rsCidades!Municipio)
        SacadosCEP(iCont) = SQLExecutaRetorno(ComandoSQL, "CEP", "")
        SacadosUF(iCont) = SQLExecutaRetorno(ComandoSQL, "UF", "")
        SacadosTelefone(iCont) = SQLExecutaRetorno("SELECT TOP 1 Telefone FROM PessoasTelefones WHERE CodigoPessoa = " & rsBoletos!CodigoPessoa, "Telefone", "")
        SacadosEmail(iCont) = SQLExecutaRetorno("SELECT TOP 1 Email FROM PessoasEmails WHERE CodigoPessoa = " & rsBoletos!CodigoPessoa, "Email", "")
        SacadosAvalista(iCont) = "" 'SacadosCR!NomeAvalista & "|" & SacadosCR!CPFAvalista
        If Not Vazio(rsBoletos!DataNascimento) And rsBoletos!TipoPessoa = 0 Then
           SacadosDataNascimento(iCont) = Format(rsBoletos!DataNascimento, "yyyy/mm/dd")
        Else
           SacadosDataNascimento(iCont) = ""
        End If
        
        DoEvents
        'HourglassShow "Aguarde, consultando boleto no banco."
        If Not Vazio(rsBoletos!nossoNumero) Then
           Call objAutomacaoBancaria.BoletoLimparConsulta
           Call objAutomacaoBancaria.BoletoInformarConsulta("", rsBoletos!CodigoBoleto, "", "", 1, "", "")
           retBoleto = objAutomacaoBancaria.BoletoConsultar()
        End If
        'If Not iRetornoBoleto Then GoTo DeuExcecao
        If objAutomacaoBancaria.Sucesso Then GoTo proximoBoleto
        
        If iRetornoBoleto And Not objAutomacaoBancaria.Sucesso Then
            processando = "Registrando Boleto"
            'frmpRemessa.lblStatus.Caption = processando & ": " & BoletoNossoNumero(i) & " - " & BoletoNumDocumento(i)
            vsSQL = "EXEC tbRemessaBancariaIncluir " & IDCedente & ", 'Gerada RB Ocor. 01 - Entrada de Títulos', 1"
            SQLExecuta vsSQL
           
            cRemessa = SQLExecutaRetorno("SELECT MAX(CodigoRemessaBanco) r FROM RemessaBancaria WHERE CodigoCedente = " & IDCedente, "r", 0)
            
            DoEvents
            Call objAutomacaoBancaria.BoletoLimpar
            Call objAutomacaoBancaria.PagadorDados(SacadosRazaoSocial(i), SacadosCNPJCPF(i), SacadosEmail(i), SacadosTelefone(i), SacadosDataNascimento(i), SacadosEndereco(i), SacadosEnderecoNumero(i), SacadosBairro(i), SacadosCidade(i), SacadosUF(i), SacadosCEP(i))
            Call objAutomacaoBancaria.BoletoDados(BoletoNossoNumero(i), BoletoNumDocumento(i), BoletoNumDocumento(i), BoletoValor(i), BoletoVencimento(i), 1, True, True, BoletoInstrucao1(i), BoletoInstrucao2(i), "", BoletoLimitePagamento(i), 0, BoletoDesconto(i), BoletoTipoMulta(i), BoletoDataJurosMora(i), BoletoMulta(i), 1, BoletoDataJurosMora(i), BoletoJurosDia(i), BoletoParcela(i), Left(rsCedente!ProtestoCodigo, 1), rsCedente!ProtestoDias)
            retBoleto = objAutomacaoBancaria.BoletoEmitir

            If Not objAutomacaoBancaria.Sucesso Then
               processando = "Deu erro no envio"
               vsSQL = "UPDATE ContasReceber SET " & _
                       "WSRetornoEnvio = '" & retBoleto & "' " & _
                       "WHERE CodigoReceber = " & rsBoletos!CodigoReceber
               SQLExecuta vsSQL
               DoEvents
               GoTo DeuExcecao
            Else
               processando = "Gravando informações no BD"
               vsSQL = "UPDATE ContasReceber SET " & _
                       "API = 1, " & _
                       "RemessaGerada = 1, " & _
                       "RemessaConfirmada = 1, " & _
                       "CodigoRemessaBanco = " & cRemessa & ", " & _
                       "NossoNumero = '" & objAutomacaoBancaria.nossoNumero & "', " & _
                       "LinhaDigitavel = '" & objAutomacaoBancaria.linhaDigitavel & "', " & _
                       "CodigoBarras = '" & objAutomacaoBancaria.codigoBarras & "', " & _
                       "QRCode = '" & objAutomacaoBancaria.QRCode & "', " & _
                       "WSRetornoEnvio = '" & retBoleto & "' " & _
                       "WHERE CodigoReceber = " & rsBoletos!CodigoReceber
               SQLExecuta vsSQL
            End If
proximoBoleto:
        End If
        Delay 3
        rsBoletos.MoveNext
   Loop
   
   'HourglassClose
   BoletoRegistraAPI = True
   
   Exit Function
   Resume
   
semCedente:
   MsgBox "Cadastro do Cedente não localizado!", vbInformation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   BoletoRegistraAPI = False
   'HourglassClose
   Exit Function

DeuExcecao:
   MsgBox retBoleto, vbExclamation + vbOKOnly, vgAtencao
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
   BoletoRegistraAPI = False
   'HourglassClose
   Exit Function
   
deuErro:
   MsgBox Err.Description + vbCrLf + processando, vbCritical + vbOKOnly, vgAtencao
   Err.Clear
   Set rsCedente = Nothing
   Set objAutomacaoBancaria = Nothing
   BoletoRegistraAPI = False
   'HourglassClose
End Function

Private Function GerarRemessa_F9() As Boolean  ' {* Apelidos *} 750
Dim xNomeArqRemessa As String, ComandoSQL As String, e As String, xNomeArquivo As String, i As Integer, envioOK As Boolean
 If Cedente!CodigoBanco = 756 Then xNomeArquivo = "\Remessa\REM_" & LPad(Cedente!Cedente, 7, "0") & "_" & Format(Date, "ddmmyyyy") & "_" & Format(Time, "hhmmss") & "_" & LPad(Cedente!CodRemessa, 5, "0") & ".txt"
 If Not TemBoleto Then Exit Function

 If Cedente!UsarWSAPI Then
     objAutomacaoBancaria.CNPJCPFEmitente = Cedente!CNPJCPF
     objAutomacaoBancaria.CNPJCPFSoftHouse = CNPJSoftHouse
     objAutomacaoBancaria.ClientCertificado = ""  '"C:\Users\leand\OneDrive\EkklesiaSoft\Certificado\certificado.pfx"
     objAutomacaoBancaria.ClientCertificadoBase64 = Cedente!WSAPIClientCertificado
     objAutomacaoBancaria.ClientCertificadoSenha = Cedente!WSAPIClientCertificadoSenha
     objAutomacaoBancaria.ClientID = Cedente!WSAPIClientID
     objAutomacaoBancaria.ClientSecret = Cedente!WSAPIClientSecret
     objAutomacaoBancaria.LicencaDLL = Cedente!WSAPILicencaDLL
     objAutomacaoBancaria.NumeroContaCorrente = Cedente!Cedente & Cedente!Cedente_Dv
     objAutomacaoBancaria.NumeroContrato = Cedente!CodigoConvenio
     objAutomacaoBancaria.urlWebhook = Cedente!WSAPIURL
     objAutomacaoBancaria.HabilitarLog = True
     objAutomacaoBancaria.CaminhoLog = rsCedente!DiretorioRemessa & "\Logs\"
     objAutomacaoBancaria.CriarArquivoPDF = True
     objAutomacaoBancaria.CaminhoArquivoPDF = "C:\EkklesiaSoft\Boleto\PDF\"
     
     iRetorno = objAutomacaoBancaria.Configuracao(Cedente!CodigoBanco)
     
     If iRetorno Then
        For i = 0 To iContTotal

            Call objAutomacaoBancaria.BoletoLimparConsulta
            Call objAutomacaoBancaria.BoletoInformarConsulta("", BoletoNossoNumero(i), "", "", 1, "", "")
            retBoleto = objAutomacaoBancaria.BoletoConsultar()
            
            If objAutomacaoBancaria.Sucesso Then
               If Not envioOK Then envioOK = True
               DoEvents
               ComandoSQL = "UPDATE ContasReceber SET " & _
                            "RemessaGerada = 1, " & _
                            "RemessaConfirmada = 1, " & _
                            "CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & ", " & _
                            "API = 1, " & _
                            "NossoNumero = '" & objAutomacaoBancaria.nossoNumero & "', " & _
                            "LinhaDigitavel = '" & objAutomacaoBancaria.linhaDigitavel & "', " & _
                            "CodigoBarras = '" & objAutomacaoBancaria.codigoBarras & "', " & _
                            "QRCode = '" & objAutomacaoBancaria.QRCode & "' " & _
                            "WHERE CodigoReceber = " & BoletoNumDocumento(i)
               e$ = SQLExecuta(ComandoSQL)
               DoEvents
               ComandoSQL = "UPDATE RemessaBancariaItens SET " & _
                            "Confirma = 1 " & _
                            "WHERE CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & " AND CodigoReceber = " & BoletoNumDocumento(i)
               e$ = SQLExecuta(ComandoSQL)
               
               GoTo proximoBoleto
            End If
            
            Call objAutomacaoBancaria.BoletoLimpar
            Call objAutomacaoBancaria.PagadorDados(SacadosRazaoSocial(i), SacadosCNPJCPF(i), SacadosEmail(i), SacadosTelefone(i), SacadosDataNascimento(i), SacadosEndereco(i), SacadosEnderecoNumero(i), SacadosBairro(i), SacadosCidade(i), SacadosUF(i), SacadosCEP(i))
            Call objAutomacaoBancaria.BoletoDados(BoletoNossoNumero(i), BoletoNumDocumento(i), BoletoNumDocumento(i), BoletoValor(i), BoletoVencimento(i), 1, True, True, BoletoInstrucao1(i), BoletoInstrucao2(i), "", BoletoLimitePagamento(i), 0, BoletoDesconto(i), BoletoTipoMulta(i), BoletoDataJurosMora(i), BoletoMulta(i), 1, BoletoDataJurosMora(i), BoletoJurosDia(i), BoletoParcela(i), Left(Cedente!ProtestoCodigo, 1), Cedente!ProtestoDias)
            retBoleto = objAutomacaoBancaria.BoletoEmitir
            
            If objAutomacaoBancaria.Sucesso Then
               If Not envioOK Then envioOK = True
               DoEvents
               ComandoSQL = "UPDATE ContasReceber SET " & _
                            "RemessaGerada = 1, " & _
                            "RemessaConfirmada = 1, " & _
                            "CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & ", " & _
                            "API = 1, " & _
                            "NossoNumero = '" & objAutomacaoBancaria.nossoNumero & "', " & _
                            "LinhaDigitavel = '" & objAutomacaoBancaria.linhaDigitavel & "', " & _
                            "CodigoBarras = '" & objAutomacaoBancaria.codigoBarras & "', " & _
                            "QRCode = '" & objAutomacaoBancaria.QRCode & "' " & _
                            "WHERE CodigoReceber = " & BoletoNumDocumento(i)
               e$ = SQLExecuta(ComandoSQL)
               DoEvents
               ComandoSQL = "UPDATE RemessaBancariaItens SET " & _
                            "Confirma = 1 " & _
                            "WHERE CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & " AND CodigoReceber = " & BoletoNumDocumento(i)
               e$ = SQLExecuta(ComandoSQL)
            Else
               ComandoSQL = "UPDATE ContasReceber SET " & _
                            "WSRetornoEnvio = '" & retBoleto & "' " & _
                            "WHERE CodigoReceber = " & BoletoNumDocumento(i)
               e$ = SQLExecuta(ComandoSQL)
            End If
proximoBoleto:
            'fsoSalva "jsonBoletoEnvio.json", retBoleto, "C:\temp\", False
        Next
         
        If envioOK Then
           ComandoSQL = "UPDATE RemessaBancaria SET " & _
                        "NumeroRemessaBanco = '" & LPad(Cedente!CodRemessa, 8, "0") & "', " & _
                        "DataGravacao = dbo.dthr(), " & _
                        "API = 1, " & _
                        "Gerado = 1 " & _
                        "WHERE CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value
           e$ = SQLExecuta(ComandoSQL)
           DoEvents
           ComandoSQL = "UPDATE Cedente SET CodRemessa = CodRemessa + 1 WHERE CodigoCedente = " & cCedente
           e$ = SQLExecuta(ComandoSQL)
        
        End If
     End If
 Else
     objBoleto.Desenvolvedor = "EkklesiaSoft - Desenvolvimento de Sistemas"
     objBoleto.LicencaDLL = Cedente!LicencaDLL
     objBoleto.SoftHouseCNPJ = CNPJSoftHouse
     objBoleto.usaFastReports = True
    
     If Cedente!LayoutRemessa = "CNAB240" Then iRetorno = objBoleto.RemessaGerar3(Cedente!CodigoBanco, Cedente!Agencia, Cedente!Agencia_Dv, Cedente!Cedente, Cedente!Cedente_Dv, Cedente!Carteira, Cedente!CodigoConvenio, Cedente!CNPJCPF, Cedente!NomeCedente, Cedente!Logradouro, Cedente!Numero, Cedente!Complemento, Cedente!Bairro, Cedente!CEP, Cedente!Cidade, Cedente!UF, _
                                                                                  BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCompEntrega, Cedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 0, BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), BoletoParcela(), _
                                                                                  BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoTipoOcorrencia(), Cedente!EspecieDocumento, eLayoutRemessa_CNAB240, Cedente!CodRemessa, Cedente!DiretorioRemessa & xNomeArquivo, xNomeArqRemessa, Cedente!Aceite, Left(Cedente!ProtestoCodigo, 1), "N", Cedente!LayoutArquivo, Cedente!LayoutLote, Cedente!LicencaDLL, CNPJSoftHouse)
     If Cedente!LayoutRemessa = "CNAB400" Then iRetorno = objBoleto.RemessaGerar3(Cedente!CodigoBanco, Cedente!Agencia, Cedente!Agencia_Dv, Cedente!Cedente, Cedente!Cedente_Dv, Cedente!Carteira, Cedente!CodigoConvenio, Cedente!CNPJCPF, Cedente!NomeCedente, Cedente!Logradouro, Cedente!Numero, Cedente!Complemento, Cedente!Bairro, Cedente!CEP, Cedente!Cidade, Cedente!UF, _
                                                                                  BoletoNossoNumero(), BoletoNumDocumento(), BoletoDataDocumento(), BoletoVencimento(), BoletoValor(), eTipoImpressao_bolCompEntrega, Cedente!ProtestoDias, BoletoTipoMulta(), BoletoMulta(), 0, BoletoJurosDia(), BoletoDesconto(), SacadosCNPJCPF(), SacadosRazaoSocial(), SacadosEndereco(), SacadosEnderecoNumero(), SacadosComplemento(), SacadosBairro(), SacadosCidade(), SacadosCEP(), SacadosUF(), SacadosEmail(), SacadosAvalista(), BoletoMensagem(), BoletoInstrucao1(), BoletoInstrucao2(), BoletoAcrescimo(), BoletoParcela(), _
                                                                                  BoletoParcelaTotal(), BoletoDataJurosMora(), BoletoTipoOcorrencia(), Cedente!EspecieDocumento, eLayoutRemessa_CNAB400, Cedente!CodRemessa, Cedente!DiretorioRemessa & xNomeArquivo, xNomeArqRemessa, Cedente!Aceite, Left(Cedente!ProtestoCodigo, 1), "N", Cedente!LayoutArquivo, Cedente!LayoutLote, Cedente!LicencaDLL, CNPJSoftHouse)
     If iRetorno And Not Vazio(xNomeArqRemessa) Then
        ComandoSQL = "UPDATE ContasReceber SET " & _
                     "RemessaGerada = 1, " & _
                     "CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & " " & _
                     "WHERE CodigoReceber in (SELECT CodigoReceber FROM RemessaBancariaItens WHERE CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value & ")"
        e$ = SQLExecuta(ComandoSQL)
        DoEvents
        ComandoSQL = "UPDATE RemessaBancaria SET " & _
                     "NumeroRemessaBanco = '" & LPad(Cedente!CodRemessa, 8, "0") & "', " & _
                     "NomeArquivo = '" & Cedente!DiretorioRemessa & "Remessa\" & xNomeArqRemessa & "', " & _
                     "DataGravacao = dbo.dt(), " & _
                     "Conteudo = '" & fsoAbre(Cedente!DiretorioRemessa & "Remessa\" & xNomeArqRemessa) & "', " & _
                     "Gerado = 1 " & _
                     "WHERE CodigoRemessaBanco = " & frmRemBanc.cpCodigoRemessaBanco.Value
        e$ = SQLExecuta(ComandoSQL)
        DoEvents
        ComandoSQL = "UPDATE Cedente SET CodRemessa = CodRemessa + 1 WHERE CodigoCedente = " & cCedente
        e$ = SQLExecuta(ComandoSQL)
     End If
 End If
 frmRemBanc.Reposition True
 GerarRemessa_F9 = False  'Deve retornar um valor lógico
End Function
