Imports DevExpress.Data
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports DevExpress.Xpf.Grid.ConditionalFormatting
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Media

Namespace HighlightModifiedCells
    Public Class ChangedCellsHighlightBehavior
        Inherits Behavior(Of GridControl)

        Private UnboundColumnPrefix As String = "IsEdited_"
        Private modifiedCells As New Dictionary(Of CellInfo, Boolean)()
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
            AddHandler AssociatedObject.CustomUnboundColumnData, AddressOf OnGridCustomUnboundColumnData
            AddHandler AssociatedObject.Loaded, AddressOf OnGridLoaded
        End Sub
        Protected Overrides Sub OnDetaching()
            RemoveHandler AssociatedObject.CustomUnboundColumnData, AddressOf OnGridCustomUnboundColumnData
            RemoveHandler View.CellValueChanged, AddressOf OnViewCellValueChanged
        End Sub
        Private Sub OnGridLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            RemoveHandler AssociatedObject.Loaded, AddressOf OnGridLoaded
            AddHandler View.CellValueChanged, AddressOf OnViewCellValueChanged
            InitializeFormattings(CreateUnboundColumns())
        End Sub
        Private Sub OnViewCellValueChanged(ByVal sender As Object, ByVal e As CellValueChangedEventArgs)
            If Not e.Column.FieldName.StartsWith(UnboundColumnPrefix) Then
                AssociatedObject.SetCellValue(e.RowHandle, UnboundColumnPrefix & e.Column.FieldName, True)
            End If
        End Sub
        Private Sub OnGridCustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
            If e.Column.FieldName.StartsWith(UnboundColumnPrefix) Then
                If e.IsGetData Then
                    Dim res As Boolean = False
                    modifiedCells.TryGetValue(New CellInfo(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName), res)
                    e.Value = res
                Else
                    Dim modifiedCell As New CellInfo(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName)
                    modifiedCells(modifiedCell) = True
                End If
            End If
        End Sub
        Private Function CreateUnboundColumns() As List(Of GridColumn)
            Dim unboundColumns As New List(Of GridColumn)()
            For Each column As GridColumn In AssociatedObject.Columns
                Dim stateKeeperUnboundColumn As New GridColumn()
                stateKeeperUnboundColumn.FieldName = UnboundColumnPrefix & column.FieldName
                stateKeeperUnboundColumn.UnboundType = UnboundColumnType.Boolean
                stateKeeperUnboundColumn.Visible = False
                stateKeeperUnboundColumn.ShowInColumnChooser = False
                stateKeeperUnboundColumn.Tag = column.FieldName
                unboundColumns.Add(stateKeeperUnboundColumn)
            Next column
            Return unboundColumns
        End Function
        Private Sub InitializeFormattings(ByVal unboundColumns As List(Of GridColumn))
            For Each unboundColumn As GridColumn In unboundColumns
                AssociatedObject.Columns.Add(unboundColumn)
                View.FormatConditions.Add(New FormatCondition() With { _
                    .FieldName = CStr(unboundColumn.Tag), .Expression = String.Format("[{0}] = true", unboundColumn.FieldName), .Format = New DevExpress.Xpf.Core.ConditionalFormatting.Format() With {.Background = HighlightBrush} _
                })
            Next unboundColumn
        End Sub
    End Class

    Public Class CellInfo
        Public Property Row() As Object
        Public Property FieldName() As String
        Public Sub New(ByVal row As Object, ByVal fieldName As String)
            Me.Row = row
            Me.FieldName = fieldName
        End Sub
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As CellInfo = TryCast(obj, CellInfo)
            Return other IsNot Nothing AndAlso other.FieldName = FieldName AndAlso Object.ReferenceEquals(Me.Row, other.Row)
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return Row.GetHashCode() Xor FieldName.GetHashCode()
        End Function
    End Class
End Namespace
