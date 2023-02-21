using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HighlightModifiedCells {
    public class ChangedCellsHighlightBehavior : Behavior<GridControl> {
        const string UnboundColumnPrefix = "IsEdited_";
        Dictionary<Tuple<object, string>, bool> modifiedCells = new Dictionary<Tuple<object, string>, bool>();
        Dictionary<Tuple<object, string>, object> originalValues = new Dictionary<Tuple<object, string>, object>();
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(ChangedCellsHighlightBehavior));
        public Brush HighlightBrush {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }
        protected TableView View {
            get { return (TableView)AssociatedObject.View; }
        }
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.CustomUnboundColumnData += OnGridCustomUnboundColumnData;
            AssociatedObject.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching() {
            AssociatedObject.CustomUnboundColumnData -= OnGridCustomUnboundColumnData;
            View.CellValueChanged -= OnViewCellValueChanged;
            base.OnDetaching();
        }
        void OnGridLoaded(object sender, System.Windows.RoutedEventArgs e) {
            AssociatedObject.Loaded -= OnGridLoaded;
            View.CellValueChanged += OnViewCellValueChanged;
            CreateColumnsAndConditions();
        }
        void OnViewCellValueChanged(object sender, CellValueChangedEventArgs e) {
            if (!IsServiceColumn(e.Column)) {
                var fieldName = $"{UnboundColumnPrefix}{e.Column.FieldName}";
                var key = Tuple.Create(e.Row, fieldName);
                object originalValue;
                bool isModified = true;
                if (originalValues.TryGetValue(key, out originalValue))
                    isModified = !Equals(originalValue, e.Value);
                else
                    originalValues[key] = e.OldValue;
                AssociatedObject.SetCellValue(e.RowHandle, fieldName, isModified);
            }
        }
        void OnGridCustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if (IsServiceColumn(e.Column)) {
                var key = Tuple.Create(AssociatedObject.GetRowByListIndex(e.ListSourceRowIndex), e.Column.FieldName);
                if (e.IsGetData) {
                    bool res;
                    e.Value = modifiedCells.TryGetValue(key, out res) ? res : false;
                }
                if (e.IsSetData)
                    modifiedCells[key] = (bool)e.Value;
            }
        }
        static bool IsServiceColumn(GridColumn column) {
            return column.FieldName.StartsWith(UnboundColumnPrefix);
        }
        void CreateColumnsAndConditions() {
            foreach (GridColumn column in AssociatedObject.Columns.ToList()) {
                GridColumn unboundColumn = new GridColumn();
                unboundColumn.FieldName = UnboundColumnPrefix + column.FieldName;
                unboundColumn.UnboundDataType = typeof(bool);
                unboundColumn.Visible = false;
                unboundColumn.ShowInColumnChooser = false;
                AssociatedObject.Columns.Add(unboundColumn);
                View.FormatConditions.Add(new FormatCondition() {
                    FieldName = column.FieldName,
                    Expression = string.Format("[{0}] = true", unboundColumn.FieldName),
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format() { Background = HighlightBrush }
                });
            }
        }
    }
}
