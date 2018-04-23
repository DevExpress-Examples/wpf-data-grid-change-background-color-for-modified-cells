using DevExpress.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.ConditionalFormatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace HighlightModifiedCells {
    public class ChangedCellsHighlightBehavior : Behavior<GridControl> {
        string UnboundColumnPrefix = "IsEdited_";
        Dictionary<CellInfo, bool> modifiedCells = new Dictionary<CellInfo, bool>();
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(ChangedCellsHighlightBehavior));
        public Brush HighlightBrush {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }
        protected TableView View {
            get {
                return (TableView)AssociatedObject.View;
            }
        }
        protected override void OnAttached() {
            AssociatedObject.CustomUnboundColumnData += OnGridCustomUnboundColumnData;
            AssociatedObject.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching() {
            AssociatedObject.CustomUnboundColumnData -= OnGridCustomUnboundColumnData;
            View.CellValueChanged -= OnViewCellValueChanged;
        }
        void OnGridLoaded(object sender, System.Windows.RoutedEventArgs e) {
            AssociatedObject.Loaded -= OnGridLoaded;
            View.CellValueChanged += OnViewCellValueChanged;
            InitializeFormattings(CreateUnboundColumns());
        }
        void OnViewCellValueChanged(object sender, CellValueChangedEventArgs e) {
            if (!e.Column.FieldName.StartsWith(UnboundColumnPrefix)) {
                AssociatedObject.SetCellValue(e.RowHandle, UnboundColumnPrefix + e.Column.FieldName, true);
            }
        }
        void OnGridCustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if (e.Column.FieldName.StartsWith(UnboundColumnPrefix)) {
                if (e.IsGetData) {
                    bool res = false;
                    modifiedCells.TryGetValue(new CellInfo(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName), out res);
                    e.Value = res;
                }
                else {
                    CellInfo modifiedCell = new CellInfo(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName);
                    modifiedCells[modifiedCell] = true;
                }
            }
        }
        List<GridColumn> CreateUnboundColumns() {
            List<GridColumn> unboundColumns = new List<GridColumn>();
            foreach (GridColumn column in AssociatedObject.Columns) {
                GridColumn stateKeeperUnboundColumn = new GridColumn();
                stateKeeperUnboundColumn.FieldName = UnboundColumnPrefix + column.FieldName;
                stateKeeperUnboundColumn.UnboundType = UnboundColumnType.Boolean;
                stateKeeperUnboundColumn.Visible = false;
                stateKeeperUnboundColumn.ShowInColumnChooser = false;
                stateKeeperUnboundColumn.Tag = column.FieldName;
                unboundColumns.Add(stateKeeperUnboundColumn);
            }
            return unboundColumns;
        }
        void InitializeFormattings(List<GridColumn> unboundColumns) {
            foreach (GridColumn unboundColumn in unboundColumns) {
                AssociatedObject.Columns.Add(unboundColumn);
                View.FormatConditions.Add(new FormatCondition() { FieldName = (string)unboundColumn.Tag, Expression = string.Format("[{0}] = true", unboundColumn.FieldName), Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format() { Background = HighlightBrush } });
            }
        }
    }

    public class CellInfo {
        public object Row { get; set; }
        public string FieldName { get; set; }
        public CellInfo(object row, string fieldName) {
            Row = row;
            FieldName = fieldName;
        }
        public override bool Equals(object obj) {
            CellInfo other = obj as CellInfo;
            return other != null && other.FieldName == FieldName && object.ReferenceEquals(this.Row, other.Row);
        }
        public override int GetHashCode() {
            return Row.GetHashCode() ^ FieldName.GetHashCode();
        }
    }
}
