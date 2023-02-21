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
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.
<Assembly:AssemblyTitle("EditableUnboundColumn")>
<Assembly:AssemblyDescription("")>
<Assembly:AssemblyConfiguration("")>
<Assembly:AssemblyCompany("DX")>
<Assembly:AssemblyProduct("EditableUnboundColumn")>
<Assembly:AssemblyCopyright("Copyright © DX 2009")>
<Assembly:AssemblyTrademark("")>
<Assembly:AssemblyCulture("")>
' Setting ComVisible to false makes the types in this assembly not visible 
' to COM components.  If you need to access a type in this assembly from 
' COM, set the ComVisible attribute to true on that type.
<Assembly:ComVisible(False)>
'In order to begin building localizable applications, set 
'<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
'inside a <PropertyGroup>.  For example, if you are using US english
'in your source files, set the <UICulture> to en-US.  Then uncomment
'the NeutralResourceLanguage attribute below.  Update the "en-US" in
'the line below to match the UICulture setting in the project file.
'[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
'(used if a resource is not found in the page, 
' or application resource dictionaries)
'(used if a resource is not found in the page, 
' app, or any theme specific resource dictionaries)
<Assembly:ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)> 'where theme specific resource dictionaries are located
'where the generic resource dictionary is located
' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers 
' by using the '*' as shown below:
' [assembly: AssemblyVersion("1.0.*")]
<Assembly:AssemblyVersion("1.0.0.0")>
<Assembly:AssemblyFileVersion("1.0.0.0")>
