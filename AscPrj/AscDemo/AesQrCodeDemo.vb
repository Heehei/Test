Imports QRCoder

Public Class AesQrCodeDemo
    Private Shared str(79) As String
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        For Each chk As Control In Panel2.Controls
            If TypeOf chk Is CheckBox Then
                AddHandler CType(chk, CheckBox).CheckedChanged, AddressOf CheckBox_CheckedChanged

                str(CInt(CType(chk, CheckBox).Name.Replace("chk", "").Trim) - 1) = "0"
            End If
        Next

    End Sub
    Private Sub chkAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkAll.CheckedChanged
        Dim allBl As Boolean = chkAll.Checked
        For Each chk As Control In Panel2.Controls
            If TypeOf chk Is CheckBox Then
                CType(chk, CheckBox).Checked = allBl
            End If
        Next
    End Sub

    Private Sub CheckBox_CheckedChanged(sender As Object, e As EventArgs)
        If CType(sender, CheckBox).Name = "chkAll" Then Exit Sub
        Dim strI As Integer = CInt(CType(sender, CheckBox).Name.Replace("chk", "").Trim)
        If CType(sender, CheckBox).Checked = True Then
            str(Math.Abs(strI - 80)) = "1"
        Else
            str(Math.Abs(strI - 80)) = "0"
        End If
        txtTk.Text = Join(str, "").Trim
    End Sub
    Private Sub setDownText()
        For Each chk As Control In Panel2.Controls
            If TypeOf chk Is CheckBox Then
                Dim strI As Integer = CInt(CType(chk, CheckBox).Name.Replace("chk", "").Trim)
                If CType(chk, CheckBox).Checked = True Then
                    str(Math.Abs(strI - 80)) = "1"
                Else
                    str(Math.Abs(strI - 80)) = "0"
                End If
            End If
        Next
        txtTk.Text = Join(str, "").Trim
    End Sub
    Private Sub AesQrCodeDemo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpExpireFrom.Value = Now
        dtpExpireTo.Value = Now.AddDays(1)
    End Sub
    Public Shared Function GetUInt32FromBytes(ByVal srcB() As Byte, ByVal startIndex As Integer, ByVal Length As Integer) As UInteger
        Dim B(Length - 1) As Byte
        If Length < 4 Then ReDim B(3)
        For n As Integer = B.Length - Length To B.Length - 1
            B(n) = srcB(startIndex + n - (B.Length - Length))
        Next
        If BitConverter.IsLittleEndian = True Then
            Array.Reverse(B)
        End If
        Return BitConverter.ToUInt32(B, 0)
    End Function
    Public Shared Function GetBytesFromInteger(ByVal srcInt As UInteger, Optional ByVal BytesLength As Integer = 0) As Byte()
        Dim tmpB() As Byte
        tmpB = BitConverter.GetBytes(srcInt)

        Dim ReturnB(BytesLength - 1) As Byte
        'If BytesLength > 0 And tmpB.Length > BytesLength Then
        '    For n As Integer = 0 To BytesLength - 1
        '        ReturnB(n) = tmpB(n)
        '    Next
        '    Array.Reverse(ReturnB)
        '    Return ReturnB
        'Else
        For n As Integer = 0 To BytesLength - 1
            If tmpB.Length > 0 And n < tmpB.Length Then
                ReturnB(n) = tmpB(n)
            Else
                ReturnB(n) = 0
            End If
        Next
        Array.Reverse(ReturnB)
        Return ReturnB
        '  End If
    End Function
    Private Shared Function GetStringFromBytes(ByVal srcB() As Byte, ByVal startIndex As Integer, ByVal Length As Integer) As String
        Dim B(Length - 1) As Byte
        For n As Integer = 0 To Length - 1
            B(n) = srcB(startIndex + n)
        Next
        Return AesHelper.ByteToHexStr(B)
    End Function
    Private Sub txtTk_TextChanged(sender As Object, e As EventArgs) Handles txtTk.TextChanged
        txtHex.Text = BIN_to_HEX(txtTk.Text)
    End Sub
    '二进制转十六进制
    Public Function BIN_to_HEX(ByVal Bin As String) As String
        Dim i As Long
        Dim H As String = ""
        If Len(Bin) Mod 4 <> 0 Then
            Bin = Strings.StrDup(4 - Len(Bin) Mod 4, "0") & Bin
        End If

        For i = 1 To Len(Bin) Step 4
            Select Case Mid(Bin, i, 4)
                Case "0000" : H &= "0"
                Case "0001" : H &= "1"
                Case "0010" : H &= "2"
                Case "0011" : H &= "3"
                Case "0100" : H &= "4"
                Case "0101" : H &= "5"
                Case "0110" : H &= "6"
                Case "0111" : H &= "7"
                Case "1000" : H &= "8"
                Case "1001" : H &= "9"
                Case "1010" : H &= "A"
                Case "1011" : H &= "B"
                Case "1100" : H &= "C"
                Case "1101" : H &= "D"
                Case "1110" : H &= "E"
                Case "1111" : H &= "F"
            End Select
        Next i
        Return H
    End Function

    Private Sub btnAesQR_Click(sender As Object, e As EventArgs) Handles btnAesQR.Click
        setDownText()
        Dim textBytes() As Byte = GetTestBytes()
        If IsNothing(textBytes) Then Exit Sub
        txtAesB.Text = AesHelper.ByteToHexStr(textBytes).Trim
        Dim key() As Byte = AesHelper.HexStrToBytes(txtAesKey.Text.Trim)
        Dim iv() As Byte = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
        Dim en() As Byte = AesHelper.AESEncrypt(textBytes, key, iv)
        txtAesE.Text = AesHelper.ByteToHexStr(en)
        txtAesD.Text = AesHelper.ByteToHexStr(AesHelper.AESDecrypt(en, key, iv))
        Dim qrGenerator As New QRCodeGenerator
        'QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version)
        Dim qrCodeData As QRCodeData = qrGenerator.CreateQrCode(txtAesE.Text, QRCodeGenerator.ECCLevel.Q)
        Dim qrCode As QRCode = New QRCode(qrCodeData)
        PictureBox1.Image = qrCode.GetGraphic(5)
        PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize

    End Sub
    Function Hex2Bin(HexValue As String) As String
        Const BinTbl = "0000000100100011010001010110011110001001101010111100110111101111"
        Dim X As Integer
        Dim Work1 As String
        Work1 = ""
        For X = 1 To Len(HexValue)
            Work1 = Work1 + Mid(BinTbl, Val("&h" + Mid(HexValue, X, 1)) * 4 + 1, 4)
        Next
        Hex2Bin = Work1
    End Function
    Private Function GetTestBytes() As Byte()
        Dim B(95) As Byte
        Dim bIndex As Integer = 0
        '标志 4
        Dim bz As String = "55AA1234"
        B = StrAddZero(B, bz, 4, bIndex)
        '联网类型 1
        Dim netStr As String = ""
        If rdbNet.Checked = True Then
            netStr = "1"
        ElseIf rdbOffline.Checked = True Then
            netStr = "2"
        End If
        B = StrAddZero(B, netStr, 1, bIndex)
        '区域编号 4
        Dim qyid As String = txtAppId.Text.Trim
        Dim maxV As UInteger = UInteger.MaxValue
        Try
            If IsNumeric(qyid) = False Then
                MessageBox.Show("区域编号只能为数字")
                Return Nothing
            End If
            If UInteger.Parse(qyid) > maxV Or UInteger.Parse(qyid) < 0 Then
                MessageBox.Show("区域编号数值只能在0到4294967295")
                Return Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("区域编号只能为数字")
            Return Nothing
        End Try
        Dim qyb() As Byte = GetBytesFromInteger(UInteger.Parse(qyid), 4)
        For Each bt As Byte In qyb
            B(bIndex) = bt
            bIndex += 1
        Next
        'B = StrAddZero(B, qyid, 4, bIndex)
        '卡号
        Dim cardno As String = txtCardNo.Text.Trim
        B = StrAddZero(B, cardno, 4, bIndex)
        Dim uid As String = txtUserID.Text.Trim
        Try
            If IsNumeric(uid) = False Then
                MessageBox.Show("用户编号只能为数字")
                Return Nothing
            End If
            If UInteger.Parse(uid) > maxV Or UInteger.Parse(uid) < 0 Then
                MessageBox.Show("用户编号只能在0到4294967295")
                Return Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("用户编号只能为数字")
            Return Nothing
        End Try
        Dim yhb() As Byte = GetBytesFromInteger(UInteger.Parse(uid), 16)
        For Each bt As Byte In yhb
            B(bIndex) = bt
            bIndex += 1
        Next
        'B = StrAddZero(B, uid, 16, bIndex)
        Dim utStr As String = ""
        If rdbOwner.Checked = True Then
            utStr = "1"
        ElseIf rdbVisitor.Checked = True Then
            utStr = "2"
        End If
        B = StrAddZero(B, utStr, 1, bIndex)
        Dim qrDtBeg As String = (dtpExpireFrom.Value.Year - 2000) & dtpExpireFrom.Value.ToString("MMddHHmm")
        B = StrAddZero(B, qrDtBeg, 5, bIndex)
        Dim qrDtEnd As String = (dtpExpireTo.Value.Year - 2000) & dtpExpireTo.Value.ToString("MMddHHmm")
        B = StrAddZero(B, qrDtEnd, 5, bIndex)
        Dim blstr As String = ""
        B = StrAddZero(B, blstr, 8, bIndex)
        Dim dtstr As String = ""
        If RadioButton1.Checked = True Then
            dtstr = "1"
        ElseIf RadioButton2.Checked = True Then
            dtstr = "0"
        End If
        B = StrAddZero(B, dtstr, 1, bIndex)
        Dim tkstr As String = ""
        If RadioButton3.Checked = True Then
            tkstr = "1"
        ElseIf RadioButton4.Checked = True Then
            tkstr = "0"
        End If
        B = StrAddZero(B, tkstr, 1, bIndex)

        Dim mjstr1 As String = TextBox1.Text.Trim
        B = StrAddZero(B, mjstr1, 3, bIndex)
        Dim mjstr2 As String = TextBox2.Text.Trim
        B = StrAddZero(B, mjstr2, 3, bIndex)
        Dim mjstr3 As String = TextBox3.Text.Trim
        B = StrAddZero(B, mjstr3, 3, bIndex)
        Dim mjstr4 As String = TextBox4.Text.Trim
        B = StrAddZero(B, mjstr4, 3, bIndex)
        Dim mjstr5 As String = TextBox5.Text.Trim
        B = StrAddZero(B, mjstr5, 3, bIndex)
        Dim mjstr6 As String = TextBox6.Text.Trim
        B = StrAddZero(B, mjstr6, 3, bIndex)

        Dim hexstr As String = txtHex.Text.Trim
        B = StrAddZero(B, hexstr, 10, bIndex)

        Dim dtstr1 As String = TextBox11.Text.Trim
        B = StrAddZero(B, dtstr1, 3, bIndex)
        Dim dtstr2 As String = TextBox10.Text.Trim
        B = StrAddZero(B, dtstr2, 3, bIndex)
        Dim dtstr3 As String = TextBox9.Text.Trim
        B = StrAddZero(B, dtstr3, 3, bIndex)
        Dim dtstr4 As String = TextBox8.Text.Trim
        B = StrAddZero(B, dtstr4, 3, bIndex)
        Dim dtstr5 As String = TextBox7.Text.Trim
        B = StrAddZero(B, dtstr5, 3, bIndex)

        B = StrAddZero(B, blstr, 2, bIndex)

        Dim sumi As Integer = 0
        For i As Integer = 0 To bIndex - 1
            sumi += B(i)
        Next
        B(bIndex) = sumi Mod 256
        Return B
    End Function
    Private Function StrAddZero(B() As Byte, str As String, count As Integer, ByRef bIndex As Integer) As Byte()
        'If String.IsNullOrWhiteSpace(str) Or Len(str) Mod 2 <> 0 Then
        '    str = Strings.StrDup(count * 2 - Len(str), "0") & str
        'End If
        str = str.PadLeft(count * 2, "0")
        Dim khy() As Byte = AesHelper.HexStrToBytes(str)
        For i As Integer = 0 To khy.Length - 1
            B(bIndex) = khy(i)
            bIndex += 1
        Next
        Return B
    End Function

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        If IsNothing(PictureBox1.Image) Then Exit Sub
        Dim SaveFileDialog1 As New SaveFileDialog
        SaveFileDialog1.Filter = "All Files (*.*)|bai*.*|BMP Files (*.bmp)|*.bmp|JPGE Files (*.jpg)|*.jpg"
        Dim curDire = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.InitialDirectory = curDire
        SaveFileDialog1.Title = "保存二维码图片"
        SaveFileDialog1.FilterIndex = 3
        SaveFileDialog1.FileName = "二维码图片"
        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then

            If SaveFileDialog1.FileName <> "" Then
                PictureBox1.Image.Save(SaveFileDialog1.FileName)
            End If
        End If
        
    End Sub
End Class