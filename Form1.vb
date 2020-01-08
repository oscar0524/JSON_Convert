Imports Newtonsoft.Json
Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class Form1
    Dim TEMP_FILE As String = My.Computer.FileSystem.CurrentDirectory() & "\Test.txt"
    Dim RN As New Random
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim Output As New List(Of DataFormat)
        For nth = 0 To 10 - 1
            Dim item As New DataFormat
            item.Name = nth & "NNNAME"
            item.Year = RN.Next(11, 30)
            For n = 0 To RN.Next(11, 30)
                Dim item_His As New HistoryFormat
                item_His.Title = n & "HHHHISTORY"
                item_His.Doing = "asjldfkjas;dlfk"
                item.History.Add(item_His)
            Next
            Output.Add(item)
        Next

        Dim SW As New StreamWriter(TEMP_FILE)
        SW.Write(JsonConvert.SerializeObject(Output)) ' , Formatting.Indented 雖然易讀但不能轉回去
        SW.Close()
        SW.Dispose()

        MessageBox.Show("Create OK", "Message")

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim SR As New StreamReader(TEMP_FILE)
        Dim str As String = SR.ReadToEnd
        SR.Close()
        SR.Dispose()
        Dim TN As New TreeNode
        BuildJsonTree(TN, str)
        'Dim DataList As List(Of DataFormat) = JsonConvert.DeserializeObject(Of List(Of DataFormat))(str)
        TV.Nodes.Clear()
        TV.Nodes.Add(TN)
    End Sub

    Public Sub BuildJsonTree(ByRef TreeNode As TreeNode, ByVal JsonString As String)
        If IsJObject(JsonString) Then
            Dim Jo As JObject = CType(JsonConvert.DeserializeObject(JsonString), JObject)
            If TreeNode.Text = "" Then
                TreeNode.Text = "Object"
            End If
            TreeNode.Text &= " { " & Jo.Count & " }"
            For Each item In Jo
                Dim TN As New TreeNode
                Select Case item.Value.GetType
                    Case GetType(JObject), GetType(JArray)
                        TN = New TreeNode(item.Key)
                        BuildJsonTree(TN, item.Value.ToString())
                    Case Else
                        TN = New TreeNode(item.Key + ":" + item.Value.ToString())

                End Select
                TreeNode.Nodes.Add(TN)
            Next
        ElseIf IsJArray(JsonString) Then
            Dim JA As JArray = CType(JsonConvert.DeserializeObject(JsonString), JArray)
            If TreeNode.Text = "" Then
                TreeNode.Text = "Array"
            End If
            TreeNode.Text &= " [ " & JA.Count & " ]"
            For index = 0 To JA.Count - 1
                Dim TN As New TreeNode("[ " & index & " ]")
                BuildJsonTree(TN, JA(index).ToString)
                TreeNode.Nodes.Add(TN)
            Next
        End If
    End Sub

    Public Function IsJObject(ByVal value As String) As Boolean
        Try
            JObject.Parse(value)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function IsJArray(ByVal value As String) As Boolean
        Try
            JArray.Parse(value)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class

Public Class DataFormat
    Public Property Name As String
    Public Property Year As Integer
    Public Property History As New List(Of HistoryFormat)
End Class

Public Class HistoryFormat
    Public Property Title As String
    Public Property Doing As String
End Class
