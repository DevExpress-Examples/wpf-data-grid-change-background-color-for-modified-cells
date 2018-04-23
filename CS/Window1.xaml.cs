using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Wpf.Grid;

namespace HighlightModifiedCells {

    public partial class Window1 : Window {
        const string IsEditedPrefix = "IsEdited_";
        List<TestData> list;
        Dictionary<CellInfo, bool> editedCells = 
            new Dictionary<CellInfo, bool>();

        public Window1() {

            InitializeComponent();

            list = new List<TestData>();
            for(int i = 0; i < 20; i++) {
                list.Add(new TestData() { Number1 = i, 
                    Number2 = i * 10, 
                    Text1 = "row " + i, 
                    Text2 = "ROW " + i  });
            }
            int columnCount = grid.Columns.Count;

            grid.Columns.BeginUpdate();

            for(int i = 0; i < columnCount; i++) {
                grid.Columns[i].CellStyle = CreateStyle("CellNormalStyle", grid.Columns[i]);

                GridColumn isEditedColumn = new GridColumn() {
                    FieldName = IsEditedPrefix + grid.Columns[i].FieldName, 
                    Visible = false, 
                    UnboundType = UnboundColumnType.Boolean,
                };
                grid.Columns.Add(isEditedColumn);
            }

            grid.Columns.EndUpdate();
            grid.DataSource = list;
        }

        Style CreateStyle(string baseStyleResourceKey, GridColumn baseColumn) {
            Style style = new Style() { BasedOn = 
                (Style)FindResource(baseStyleResourceKey), TargetType = typeof(Control) };

            DataTrigger trigger = new DataTrigger() { Binding = 
                new Binding(RowData.DataContextProperty.Name + "." + IsEditedPrefix + 
                    baseColumn.FieldName), Value = true };

            trigger.Setters.Add(new Setter() { Property = 
                Control.BackgroundProperty, Value = Brushes.Red });

            style.Triggers.Add(trigger);
            return style;
        }

        private void grid_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if(e.Column.FieldName.StartsWith(IsEditedPrefix)) {
                CellInfo key = new CellInfo(e.ListSourceRowIndex, e.Column.FieldName);
                if(e.IsGetData) {
                    e.Value = GetIsEdited(key);
                }
                if(e.IsSetData) {
                    SetIsEdited(key, (bool)e.Value);
                }
            }
        }

        bool GetIsEdited(CellInfo key) {
            bool isSelected;
            if(editedCells.TryGetValue(key, out isSelected))
                return isSelected;
            return false;
        }

        void SetIsEdited(CellInfo key, bool value) {
            if(value)
                editedCells[key] = value;
            else
                editedCells.Remove(key);
        }

        private void view_CellValueChanged(object sender, CellValueEventArgs e) {
            if(!e.Column.FieldName.StartsWith(IsEditedPrefix))
                grid.SetCellValue(e.RowHandle, IsEditedPrefix + 
                    e.Column.FieldName, true);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            List<CellInfo> tempList = new List<CellInfo>(editedCells.Keys);

            foreach(CellInfo info in tempList) {
                grid.SetCellValue(grid.GetRowHandleByListIndex(info.ListIndex), 
                    info.FieldName, false);
            }
        }

    }

    public class TestData {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
    }

    public class CellInfo {
        public int ListIndex { get; private set; }
        public string FieldName { get; private set; }

        public CellInfo(int listIndex, string fieldName) {
            ListIndex = listIndex;
            FieldName = fieldName;
        }

        public override bool Equals(object obj) {
            CellInfo other = obj as CellInfo;
            return other != null && other.FieldName == FieldName && 
                other.ListIndex == ListIndex;
        }

        public override int GetHashCode() {
            return ListIndex ^ FieldName.GetHashCode();
        }
    }
}
