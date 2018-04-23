Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports DevExpress.Data
Imports DevExpress.Wpf.Grid

Namespace HighlightModifiedCells

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
				grid.Columns(i).CellStyle = CreateStyle("CellNormalStyle", grid.Columns(i))

				Dim isEditedColumn As New GridColumn() With {.FieldName = IsEditedPrefix & grid.Columns(i).FieldName, .Visible = False, .UnboundType = UnboundColumnType.Boolean}
				grid.Columns.Add(isEditedColumn)
			Next i

			grid.Columns.EndUpdate()
			grid.DataSource = list
		End Sub

		Private Function CreateStyle(ByVal baseStyleResourceKey As String, ByVal baseColumn As GridColumn) As Style
			Dim style As New Style() With {.BasedOn = CType(FindResource(baseStyleResourceKey), Style), .TargetType = GetType(Control)}

			Dim trigger As New DataTrigger() With {.Binding = New Binding(RowData.DataContextProperty.Name & "." & IsEditedPrefix & baseColumn.FieldName), .Value = True}

			trigger.Setters.Add(New Setter() With {.Property = Control.BackgroundProperty, .Value = Brushes.Red})

			style.Triggers.Add(trigger)
			Return style
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
			Dim isSelected As Boolean
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
			If (Not e.Column.FieldName.StartsWith(IsEditedPrefix)) Then
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
		Private privateNumber1 As Integer
		Public Property Number1() As Integer
			Get
				Return privateNumber1
			End Get
			Set(ByVal value As Integer)
				privateNumber1 = value
			End Set
		End Property
		Private privateNumber2 As Integer
		Public Property Number2() As Integer
			Get
				Return privateNumber2
			End Get
			Set(ByVal value As Integer)
				privateNumber2 = value
			End Set
		End Property
		Private privateText1 As String
		Public Property Text1() As String
			Get
				Return privateText1
			End Get
			Set(ByVal value As String)
				privateText1 = value
			End Set
		End Property
		Private privateText2 As String
		Public Property Text2() As String
			Get
				Return privateText2
			End Get
			Set(ByVal value As String)
				privateText2 = value
			End Set
		End Property
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
			ListIndex = listIndex
			FieldName = fieldName
		End Sub

		Public Overrides Overloads Function Equals(ByVal obj As Object) As Boolean
			Dim other As CellInfo = TryCast(obj, CellInfo)
			Return other IsNot Nothing AndAlso other.FieldName = FieldName AndAlso other.ListIndex = ListIndex
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return ListIndex Xor FieldName.GetHashCode()
		End Function
	End Class
End Namespace
