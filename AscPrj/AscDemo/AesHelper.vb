Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class AesHelper

    '''<summary>
    ''' AES解密ff
    ''' </summary>
    ''' <param name="Data">被解密的密文</param>
    ''' <param name="Key">密钥</param>
    ''' <param name="Vector">向量</param>
    ''' <returns>明文</returns>
    Public Shared Function AESDecrypt(Data() As Byte, Key() As Byte, Vector() As Byte) As Byte()
        Dim bKey() As Byte = Key
        Dim bVector() As Byte = Vector
        Dim Cryptograph() As Byte = Nothing
        Dim Aes As Rijndael = Rijndael.Create
        Try

            '// 开辟一块内存流，存储密文
            Using Memory As MemoryStream = New MemoryStream(Data)

                '// 把内存流对象包装成加密流对象
                Aes.Mode = CipherMode.ECB
                Aes.Padding = PaddingMode.Zeros
                Aes.KeySize = 128
                'Using Decryptor As CryptoStream = New CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Read)
                '    ' // 明文存储区
                '    Using originalMemory As MemoryStream = New MemoryStream()
                '        Dim Buffer(1023) As Byte
                '        Dim readBytes As Integer = 0
                '        While (readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0
                '            originalMemory.Read(Buffer, 0, readBytes)
                '        End While
                '        Cryptograph = originalMemory.ToArray()
                '    End Using
                'End Using
                Using Decryptor As CryptoStream = New CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Write)
                    ' // 明文数据写入加密流
                    Decryptor.Write(Data, 0, Data.Length)
                    Decryptor.FlushFinalBlock()
                    Cryptograph = Memory.ToArray()
                End Using
            End Using
        Catch
            Cryptograph = Nothing
        End Try
        Return Cryptograph
    End Function
    ''' <summary>
    ''' AES加密
    ''' </summary>
    ''' <param name="Data">被加密的明文</param>
    ''' <param name="Key">密钥</param>
    ''' <param name="Vector">向量</param>
    ''' <returns>密文</returns>
    Public Shared Function AESEncrypt(Data() As Byte, Key() As Byte, Vector() As Byte) As Byte()
        Dim bKey() As Byte = Key
        Dim bVector() As Byte = Vector
        Dim Cryptograph() As Byte = Nothing
        Dim Aes As Rijndael = Rijndael.Create
        Try

            '// 开辟一块内存流，存储密文
            Using Memory As MemoryStream = New MemoryStream()
                '// 把内存流对象包装成加密流对象
                Aes.Mode = CipherMode.ECB
                Aes.Padding = PaddingMode.Zeros
                Aes.KeySize = 128
                Using Encryptor As CryptoStream = New CryptoStream(Memory, Aes.CreateEncryptor(bKey, bVector), CryptoStreamMode.Write)
                    ' // 明文数据写入加密流
                    Encryptor.Write(Data, 0, Data.Length)
                    Encryptor.FlushFinalBlock()
                    Cryptograph = Memory.ToArray()
                End Using
            End Using
        Catch
            Cryptograph = Nothing
        End Try
        Return Cryptograph
    End Function
    Public Shared Function HexStrToBytes(hexStr As String) As Byte()
        Dim sby((hexStr.Length / 2) - 1) As Byte
        Try
            Dim ins As Integer = 0
            For i As Integer = 0 To hexStr.Length - 1 Step 2
                Dim strs As Object = ((hexStr.Chars(i) & hexStr.Chars(i + 1)))
                sby.SetValue(CByte(CLng("&H" & strs)), ins)
                ins += 1
            Next
        Catch ex As Exception
            sby = Nothing
        End Try
        Return sby
    End Function
    Public Shared Function ByteToHexStr(bytes() As Byte) As String
        Dim returnStr As String = ""
        If Not IsNothing(bytes) Then
            For i = 0 To bytes.Length - 1
                returnStr += bytes(i).ToString("X2")
            Next
        End If
        Return returnStr
    End Function
End Class
