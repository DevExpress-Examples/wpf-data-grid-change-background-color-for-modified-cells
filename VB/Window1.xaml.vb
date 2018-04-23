' Developer Express Code Central Example:
' How to change background color for modified cells
' 
' This example shows how to highlight grid cells that have been edited by a user.
' When a user changes a value and leaves the modified cell, the cell turns
' red.
' Update:
' This approach may look cumbersome if you have a lot of columns.
' We've prepared another example demonstrating how to accomplish a similar task
' with a dictionary of cell states: http://www.devexpress.com/scid=E4025.
' See
' Also:
' http://www.devexpress.com/scid=E841
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E1297

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports DevExpress.Xpf.Grid
Imports DevExpress.Data
Imports DevExpress.Xpf.Grid.Themes

Namespace HighlightModifiedCells
    ''' <summary>
    ''' Interaction logic for Window1.xaml
    ''' </summary>
    Partial Public Class Window1
        Inherits Window

        Private Const IsEditedPrefix As String = "IsEdited_"
        Private list As List(Of TestData)
        Private editedCells As New Dictionary(Of CellInfo, Boolean)()

        Public Sub New()
            InitializeComponent()
            list = New List(Of TestData)()
            For i As Integer = 0 To 19
                list.Add(New TestData() With {.Number1 = i, .Number2 = i * 10, .Text1 = "row " & i, .Text2 = "ROW " & i})
            Next i

            Dim columnCount As Integer = grid.Columns.Count
            grid.Columns.BeginUpdate()
            For i As Integer = 0 To columnCount - 1
                grid.Columns(i).CellStyle = CreateStyle(grid.Columns(i))

                Dim isEditedColumn As New GridColumn() With {.FieldName = IsEditedPrefix & grid.Columns(i).FieldName, .Visible = False, .UnboundType = UnboundColumnType.Boolean}
                grid.Columns.Add(isEditedColumn)
            Next i
            grid.Columns.EndUpdate()

            grid.ItemsSource = list
        End Sub
        Private Function CreateStyle(ByVal baseColumn As GridColumn) As Style
            Dim baseStyleKey = New GridRowThemeKeyExtension() With {.ResourceKey = GridRowThemeKeys.LightweightCellStyle}

            Dim style_Renamed As New Style() With {.BasedOn = DirectCast(FindResource(baseStyleKey), Style), .TargetType = GetType(LightweightCellEditor)}
            Dim trigger As New DataTrigger() With {.Binding = New Binding("Data." & IsEditedPrefix & baseColumn.FieldName), .Value = True}
            trigger.Setters.Add(New Setter() With {.Property = LightweightCellEditor.ForegroundProperty, .Value = Brushes.Red})
            style_Renamed.Triggers.Add(trigger)
            Return style_Renamed
        End Function
        Private Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
            If e.Column.FieldName.StartsWith(IsEditedPrefix) Then
                Dim key As New CellInfo(e.ListSourceRowIndex, e.Column.FieldName)
                If e.IsGetData Then
                    e.Value = GetIsEdited(key)
                End If
                If e.IsSetData Then
                    SetIsEdited(key, CBool(e.Value))
                End If
            End If
        End Sub
        Private Function GetIsEdited(ByVal key As CellInfo) As Boolean
            Dim isSelected As Boolean = Nothing
            If editedCells.TryGetValue(key, isSelected) Then
                Return isSelected
            End If
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
            If Not e.Column.FieldName.StartsWith(IsEditedPrefix) Then
                grid.SetCellValue(e.RowHandle, IsEditedPrefix & e.Column.FieldName, True)
            End If
        End Sub

        Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim tempList As New List(Of CellInfo)(editedCells.Keys)
            For Each info As CellInfo In tempList
                grid.SetCellValue(grid.GetRowHandleByListIndex(info.ListIndex), info.FieldName, False)
            Next info
        End Sub

    End Class
    Public Class TestData
        Public Property Number1() As Integer
        Public Property Number2() As Integer
        Public Property Text1() As String
        Public Property Text2() As String
    End Class
    Public Class CellInfo
        Private privateListIndex As Integer
        Public Property ListIndex() As Integer
            Get
                Return privateListIndex
            End Get
            Private Set(ByVal value As Integer)
                privateListIndex = value
            End Set
        End Property
        Private privateFieldName As String
        Public Property FieldName() As String
            Get
                Return privateFieldName
            End Get
            Private Set(ByVal value As String)
                privateFieldName = value
            End Set
        End Property
        Public Sub New(ByVal listIndex As Integer, ByVal fieldName As String)
            Me.ListIndex = listIndex
            Me.FieldName = fieldName
        End Sub
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As CellInfo = TryCast(obj, CellInfo)
            Return other IsNot Nothing AndAlso other.FieldName = FieldName AndAlso other.ListIndex = ListIndex
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return ListIndex Xor FieldName.GetHashCode()
        End Function
    End Class
End Namespace
