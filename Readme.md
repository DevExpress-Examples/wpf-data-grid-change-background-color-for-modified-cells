<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128648669/22.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1297)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# WPF Data Grid - Change Background Color for Modified Cells

This example demonstrates how to highlight grid cells that were edited by a user.

![image](https://user-images.githubusercontent.com/65009440/220330527-260df3c5-f67b-4afc-bc6d-6e2193b20121.png)

This approach does not highlight cells if you change their values at the data source level. In this case, you can use the [DataUpdateFormatCondition](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.DataUpdateFormatCondition) instead.

## Implementation Details

This example creates invisible [unbound columns](https://docs.devexpress.com/WPF/6124/controls-and-libraries/data-grid/grid-view-data-layout/columns-and-card-fields/unbound-columns) for each grid column. These Boolean unbound columns contain information on whether the corresponding grid column's cell was modified by a user. The [CustomUnboundColumnData](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl.CustomUnboundColumnData) event populates unbound columns with data. The specified [format condition](https://docs.devexpress.com/WPF/17130/controls-and-libraries/data-grid/conditional-formatting) highlights cells based on data in unbound columns.

## Files to Review

* [HighlightChangedCellBehavior.cs](./CS/HighlightChangedCellBehavior.cs) (VB: [HighlightChangedCellBehavior.vb](./VB/HighlightChangedCellBehavior.vb))
* [ViewModel.cs](./CS/ViewModel.cs) (VB: [ViewModel.vb](./VB/ViewModel.vb))
* [Window1.xaml](./CS/Window1.xaml) (VB: [Window1.xaml](./VB/Window1.xaml))

## Documentation

* [Behaviors](https://docs.devexpress.com/WPF/17442/mvvm-framework/behaviors)
* [Conditional Formatting](https://docs.devexpress.com/WPF/17130/controls-and-libraries/data-grid/conditional-formatting)
* [Unbound Columns](https://docs.devexpress.com/WPF/6124/controls-and-libraries/data-grid/grid-view-data-layout/columns-and-card-fields/unbound-columns)
* [Format Changing Values](https://docs.devexpress.com/WPF/118929/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-changing-values)

## More Examples

* [WPF MVVM Behaviors - Create a Custom Attached Behavior](https://github.com/DevExpress-Examples/wpf-mvvm-behaviors-create-a-custom-behavior)
* [WPF Data Grid - Apply Conditional Formatting in Code Behind](https://github.com/DevExpress-Examples/how-to-apply-conditional-formatting-in-code-behind-t281415)
* [WPF Data Grid - Create Unbound Columns](https://github.com/DevExpress-Examples/how-to-create-unbound-columns-e1503)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-data-grid-change-background-color-for-modified-cells&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-data-grid-change-background-color-for-modified-cells&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
