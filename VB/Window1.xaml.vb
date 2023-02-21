Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports DevExpress.Xpf.Grid
Imports DevExpress.Data

Namespace HighlightModifiedCells

    ''' <summary>
    ''' Interaction logic for Window1.xaml
    ''' </summary>
    Public Partial Class Window1
        Inherits Window

        Const IsEditedPrefix As String = "IsEdited_"

        Private list As List(Of TestData)

        Private editedCells As Dictionary(Of CellInfo, Boolean) = New Dictionary(Of CellInfo, Boolean)()

        Public Sub New()
            Me.InitializeComponent()
            list = New List(Of TestData)()
            For i As Integer = 0 To 20 - 1
                list.Add(New TestData() With {.Number1 = i, .Number2 = i * 10, .Text1 = "row " & i, .Text2 = "ROW " & i})
            Next

            Dim columnCount As Integer = Me.grid.Columns.Count
            Me.grid.Columns.BeginUpdate()
            For i As Integer = 0 To columnCount - 1
                Me.grid.Columns(i).CellStyle = Me.CreateStyle("CellStyle", Me.grid.Columns(i))
                Dim isEditedColumn As GridColumn = New GridColumn() With {.FieldName = IsEditedPrefix & Me.grid.Columns(i).FieldName, .Visible = False, .UnboundType = UnboundColumnType.Boolean}
                Me.grid.Columns.Add(isEditedColumn)
            Next

            Me.grid.Columns.EndUpdate()
            Me.grid.ItemsSource = list
        End Sub

        Private Function CreateStyle(ByVal baseStyleResourceKey As String, ByVal baseColumn As GridColumn) As Style
            Dim style As Style = New Style() With {.BasedOn = CType(FindResource(baseStyleResourceKey), Style), .TargetType = GetType(CellContentPresenter)}
            Dim trigger As DataTrigger = New DataTrigger() With {.Binding = New Binding("Data." & IsEditedPrefix & baseColumn.FieldName), .Value = True}
            trigger.Setters.Add(New Setter() With {.[Property] = Border.BackgroundProperty, .Value = Brushes.Red})
            style.Triggers.Add(trigger)
            Return style
        End Function

        Private Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
            If e.Column.FieldName.StartsWith(IsEditedPrefix) Then
                Dim key As CellInfo = New CellInfo(e.ListSourceRowIndex, e.Column.FieldName)
                If e.IsGetData Then
                    e.Value = GetIsEdited(key)
                End If

                If e.IsSetData Then
                    SetIsEdited(key, CBool(e.Value))
                End If
            End If
        End Sub

        Private Function GetIsEdited(ByVal key As CellInfo) As Boolean
            Dim isSelected As Boolean
            If editedCells.TryGetValue(key, isSelected) Then Return isSelected
            Return False
        End Function

        Private Sub SetIsEdited(ByVal key As CellInfo, ByVal value As Boolean)
            If value Then
                editedCells(key) = value
            Else
                editedCells.Remove(key)
            End If
        End Sub

        Private Sub view_CellValueChanged(ByVal sender As Object, ByVal e As CellValueEventArgs)
            If Not e.Column.FieldName.StartsWith(IsEditedPrefix) Then Me.grid.SetCellValue(e.RowHandle, IsEditedPrefix & e.Column.FieldName, True)
        End Sub

        Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim tempList As List(Of CellInfo) = New List(Of CellInfo)(editedCells.Keys)
            For Each info As CellInfo In tempList
                Me.grid.SetCellValue(Me.grid.GetRowHandleByListIndex(info.ListIndex), info.FieldName, False)
            Next
        End Sub
    End Class

    Public Class TestData

        Public Property Number1 As Integer

        Public Property Number2 As Integer

        Public Property Text1 As String

        Public Property Text2 As String
    End Class

    Public Class CellInfo

        Private _ListIndex As Integer, _FieldName As String

        Public Property ListIndex As Integer
            Get
                Return _ListIndex
            End Get

            Private Set(ByVal value As Integer)
                _ListIndex = value
            End Set
        End Property

        Public Property FieldName As String
            Get
                Return _FieldName
            End Get

            Private Set(ByVal value As String)
                _FieldName = value
            End Set
        End Property

        Public Sub New(ByVal listIndex As Integer, ByVal fieldName As String)
            Me.ListIndex = listIndex
            Me.FieldName = fieldName
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As CellInfo = TryCast(obj, CellInfo)
            Return other IsNot Nothing AndAlso Equals(other.FieldName, FieldName) AndAlso other.ListIndex = ListIndex
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return ListIndex Xor FieldName.GetHashCode()
        End Function
    End Class
End Namespace
