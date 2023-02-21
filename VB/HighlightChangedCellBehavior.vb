Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media

Namespace HighlightModifiedCells

    Public Class ChangedCellsHighlightBehavior
        Inherits DevExpress.Mvvm.UI.Interactivity.Behavior(Of DevExpress.Xpf.Grid.GridControl)

        Const UnboundColumnPrefix As String = "IsEdited_"

        Private modifiedCells As System.Collections.Generic.Dictionary(Of System.Tuple(Of Object, String), Boolean) = New System.Collections.Generic.Dictionary(Of System.Tuple(Of Object, String), Boolean)()

        Private originalValues As System.Collections.Generic.Dictionary(Of System.Tuple(Of Object, String), Object) = New System.Collections.Generic.Dictionary(Of System.Tuple(Of Object, String), Object)()

        Public Shared ReadOnly HighlightBrushProperty As System.Windows.DependencyProperty = System.Windows.DependencyProperty.Register("HighlightBrush", GetType(System.Windows.Media.Brush), GetType(HighlightModifiedCells.ChangedCellsHighlightBehavior))

        Public Property HighlightBrush As Brush
            Get
                Return CType(Me.GetValue(HighlightModifiedCells.ChangedCellsHighlightBehavior.HighlightBrushProperty), System.Windows.Media.Brush)
            End Get

            Set(ByVal value As Brush)
                Me.SetValue(HighlightModifiedCells.ChangedCellsHighlightBehavior.HighlightBrushProperty, value)
            End Set
        End Property

        Protected ReadOnly Property View As TableView
            Get
                Return CType(Me.AssociatedObject.View, DevExpress.Xpf.Grid.TableView)
            End Get
        End Property

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            AddHandler Me.AssociatedObject.CustomUnboundColumnData, AddressOf Me.OnGridCustomUnboundColumnData
            AddHandler Me.AssociatedObject.Loaded, AddressOf Me.OnGridLoaded
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler Me.AssociatedObject.CustomUnboundColumnData, AddressOf Me.OnGridCustomUnboundColumnData
            RemoveHandler Me.View.CellValueChanged, AddressOf Me.OnViewCellValueChanged
            MyBase.OnDetaching()
        End Sub

        Private Sub OnGridLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            RemoveHandler Me.AssociatedObject.Loaded, AddressOf Me.OnGridLoaded
            AddHandler Me.View.CellValueChanged, AddressOf Me.OnViewCellValueChanged
            Me.CreateColumnsAndConditions()
        End Sub

        Private Sub OnViewCellValueChanged(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.CellValueChangedEventArgs)
            If Not HighlightModifiedCells.ChangedCellsHighlightBehavior.IsServiceColumn(e.Column) Then
                Dim fieldName = $"{HighlightModifiedCells.ChangedCellsHighlightBehavior.UnboundColumnPrefix} {e.Column.FieldName}"
                Dim key = System.Tuple.Create(e.Row, fieldName)
                Dim originalValue As Object
                Dim isModified As Boolean = True
                If Me.originalValues.TryGetValue(key, originalValue) Then
                    isModified = Not System.[Object].Equals(originalValue, e.Value)
                Else
                    Me.originalValues(key) = e.OldValue
                End If

                Me.AssociatedObject.SetCellValue(e.RowHandle, fieldName, isModified)
            End If
        End Sub

        Private Sub OnGridCustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.GridColumnDataEventArgs)
            If HighlightModifiedCells.ChangedCellsHighlightBehavior.IsServiceColumn(e.Column) Then
                Dim key = System.Tuple.Create(Me.AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName)
                If e.IsGetData Then
                    Dim res As Boolean
                    e.Value = If(Me.modifiedCells.TryGetValue(key, res), res, False)
                End If

                If e.IsSetData Then Me.modifiedCells(key) = CBool(e.Value)
            End If
        End Sub

        Private Shared Function IsServiceColumn(ByVal column As DevExpress.Xpf.Grid.GridColumn) As Boolean
            Return column.FieldName.StartsWith(HighlightModifiedCells.ChangedCellsHighlightBehavior.UnboundColumnPrefix)
        End Function

        Private Sub CreateColumnsAndConditions()
            For Each column As DevExpress.Xpf.Grid.GridColumn In Me.AssociatedObject.Columns.ToList()
                Dim unboundColumn As DevExpress.Xpf.Grid.GridColumn = New DevExpress.Xpf.Grid.GridColumn()
                unboundColumn.FieldName = HighlightModifiedCells.ChangedCellsHighlightBehavior.UnboundColumnPrefix & column.FieldName
                unboundColumn.UnboundDataType = GetType(Boolean)
                unboundColumn.Visible = False
                unboundColumn.ShowInColumnChooser = False
                Me.AssociatedObject.Columns.Add(unboundColumn)
                Me.View.FormatConditions.Add(New DevExpress.Xpf.Grid.FormatCondition() With {.FieldName = column.FieldName, .Expression = String.Format("[{0}] = true", unboundColumn.FieldName), .Format = New DevExpress.Xpf.Core.ConditionalFormatting.Format() With {.Background = Me.HighlightBrush}})
            Next
        End Sub
    End Class
End Namespace
