Public Class HardwareID
    Protected Class WMI_CLASSES
        Public Shared ReadOnly MOTHERBOARD As String() = {"Win32_BaseBoard", "Name", "Manufacturer", "Version"}
        Public Shared ReadOnly GPU As String() = {"Win32_VideoController", "Name", "DeviceID", "DriverVersion"}
        Public Shared ReadOnly CDROM As String() = {"Win32_CDROMDrive", "Name", "Manufacturer", "DeviceID"}
        Public Shared ReadOnly CPU As String() = {"Win32_Processor", "Name", "Manufacturer", "ProcessorId"}
        Public Shared ReadOnly HDD As String() = {"Win32_DiskDrive", "Name", "Manufacturer", "Model"}
        Public Shared ReadOnly BIOS As String() = {"Win32_BIOS", "Name", "Manufacturer", "Version"}
    End Class

    Public Property LastID As String

    Sub New() : End Sub

    Public Function Generate() As String
        Dim wmiData As String() = {
            GetProperties(WMI_CLASSES.MOTHERBOARD),
            GetProperties(WMI_CLASSES.CDROM),
            GetProperties(WMI_CLASSES.BIOS),
            GetProperties(WMI_CLASSES.GPU),
            GetProperties(WMI_CLASSES.CPU),
            GetProperties(WMI_CLASSES.HDD)
        }
        LastID = GetHash(String.Join(String.Empty, wmiData))
        Return LastID
    End Function

    Private Function GetProperties(ByVal wmiData As String()) As String
        Dim properties As New StringBuilder()
        Dim query As String = GenerateQuery(wmiData)
        Using moSearcher As New ManagementObjectSearcher("root\CIMV2", query)
            Using moCollection As ManagementObjectCollection = moSearcher.Get()
                For Each mbObject As ManagementObject In moCollection
                    Using mbObject
                        For i As Integer = 1 To wmiData.Length - 1
                            properties.Append(mbObject(wmiData(i)))
                        Next
                    End Using
                Next
            End Using
        End Using
        Return properties.ToString()
    End Function

    Private Function GenerateQuery(ByVal wmiData As String()) As String
        Dim query As New StringBuilder()
        Dim wmiClass As String = String.Empty
        query.Append("SELECT ")
        For i As Integer = 0 To wmiData.Length - 1
            If (i = 0) Then
                wmiClass = wmiData(i)
            Else
                query.Append(If(i < wmiData.Length - 1, $"{wmiData(i)}, ", $"{wmiData(i)} "))
            End If
        Next
        query.Append($"FROM {wmiClass}")
        Return query.ToString()
    End Function

    Private Function GetHash(ByVal data As String) As String
        Using SHA As New SHA1CryptoServiceProvider()
            Dim hash As Byte() = SHA.ComputeHash(Encoding.Default.GetBytes(data))
            Return GetHexString(hash)
        End Using
    End Function

    Private Function GetHexString(ByVal bt As Byte()) As String
        Dim sBuilder = New StringBuilder()
        For i As Integer = 0 To bt.Count - 1
            Dim b = bt(i)
            Dim n = b
            Dim n1 = n And 15
            Dim n2 = (n >> 4) And 15
            If n2 > 9 Then
                sBuilder.Append(ChrW((n2 - 10 + AscW("A"))).ToString(CultureInfo.InvariantCulture))
            Else
                sBuilder.Append(n2.ToString(CultureInfo.InvariantCulture))
            End If
            If n1 > 9 Then
                sBuilder.Append(ChrW((n1 - 10 + AscW("A"c))).ToString(CultureInfo.InvariantCulture))
            Else
                sBuilder.Append(n1.ToString(CultureInfo.InvariantCulture))
            End If
            If (i + 1) <> bt.Count AndAlso (i + 1) Mod 2 = 0 Then
                sBuilder.Append("-")
            End If
        Next
        Return sBuilder.ToString()
    End Function
End Class