Imports DevExpress.Data
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media

Namespace HighlightModifiedCells
	Public Class ChangedCellsHighlightBehavior
		Inherits Behavior(Of GridControl)

		Private Const UnboundColumnPrefix As String = "IsEdited_"
		Private modifiedCells As New Dictionary(Of Tuple(Of Object, String), Boolean)()
		Private originalValues As New Dictionary(Of Tuple(Of Object, String), Object)()
		Public Shared ReadOnly HighlightBrushProperty As DependencyProperty = DependencyProperty.Register("HighlightBrush", GetType(Brush), GetType(ChangedCellsHighlightBehavior))
		Public Property HighlightBrush() As Brush
			Get
				Return CType(GetValue(HighlightBrushProperty), Brush)
			End Get
			Set(ByVal value As Brush)
				SetValue(HighlightBrushProperty, value)
			End Set
		End Property
		Protected ReadOnly Property View() As TableView
			Get
				Return CType(AssociatedObject.View, TableView)
			End Get
		End Property
		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			AddHandler AssociatedObject.CustomUnboundColumnData, AddressOf OnGridCustomUnboundColumnData
			AddHandler AssociatedObject.Loaded, AddressOf OnGridLoaded
		End Sub
		Protected Overrides Sub OnDetaching()
			RemoveHandler AssociatedObject.CustomUnboundColumnData, AddressOf OnGridCustomUnboundColumnData
			RemoveHandler View.CellValueChanged, AddressOf OnViewCellValueChanged
			MyBase.OnDetaching()
		End Sub
		Private Sub OnGridLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
			RemoveHandler AssociatedObject.Loaded, AddressOf OnGridLoaded
			AddHandler View.CellValueChanged, AddressOf OnViewCellValueChanged
			CreateColumnsAndConditions()
		End Sub
		Private Sub OnViewCellValueChanged(ByVal sender As Object, ByVal e As CellValueChangedEventArgs)
			If Not IsServiceColumn(e.Column) Then
				Dim fieldName = $"{UnboundColumnPrefix}{e.Column.FieldName}"
				Dim key = Tuple.Create(e.Row, fieldName)
				Dim originalValue As Object = Nothing
				Dim isModified As Boolean = True
				If originalValues.TryGetValue(key, originalValue) Then
					isModified = Not Equals(originalValue, e.Value)
				Else
					originalValues(key) = e.OldValue
				End If
				AssociatedObject.SetCellValue(e.RowHandle, fieldName, isModified)
			End If
		End Sub
		Private Sub OnGridCustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
			If IsServiceColumn(e.Column) Then
				Dim key = Tuple.Create(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName)
				If e.IsGetData Then
					Dim res As Boolean = Nothing
					e.Value = If(modifiedCells.TryGetValue(key, res), res, False)
				End If
				If e.IsSetData Then
					modifiedCells(key) = CBool(e.Value)
				End If
			End If
		End Sub
		Private Shared Function IsServiceColumn(ByVal column As GridColumn) As Boolean
			Return column.FieldName.StartsWith(UnboundColumnPrefix)
		End Function
		Private Sub CreateColumnsAndConditions()
			For Each column As GridColumn In AssociatedObject.Columns.ToList()
				Dim unboundColumn As New GridColumn()
				unboundColumn.FieldName = UnboundColumnPrefix & column.FieldName
				unboundColumn.UnboundType = UnboundColumnType.Boolean
				unboundColumn.Visible = False
				unboundColumn.ShowInColumnChooser = False
				AssociatedObject.Columns.Add(unboundColumn)
				View.FormatConditions.Add(New FormatCondition() With {
					.FieldName = column.FieldName,
					.Expression = String.Format("[{0}] = true", unboundColumn.FieldName),
					.Format = New DevExpress.Xpf.Core.ConditionalFormatting.Format() With {.Background = HighlightBrush}
				})
			Next column
		End Sub
	End Class
End Namespace
