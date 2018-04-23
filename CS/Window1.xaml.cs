// Developer Express Code Central Example:
// How to change background color for modified cells
// 
// This example shows how to highlight grid cells that have been edited by a user.
// When a user changes a value and leaves the modified cell, the cell turns
// red.
// Update:
// This approach may look cumbersome if you have a lot of columns.
// We've prepared another example demonstrating how to accomplish a similar task
// with a dictionary of cell states: http://www.devexpress.com/scid=E4025.
// See
// Also:
// http://www.devexpress.com/scid=E841
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1297

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid;
using DevExpress.Data;
using DevExpress.Xpf.Grid.Themes;
using DevExpress.Xpf.Core;

namespace HighlightModifiedCells {
    public partial class Window1 : DXWindow {
        public Window1() {
            InitializeComponent();
        }
    }
}
