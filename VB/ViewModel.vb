Imports System.Collections.ObjectModel

Namespace HighlightModifiedCells

    Public Class ViewModel

        Public Property Customers As ObservableCollection(Of Customer)

        Public Sub New()
            Customers = New ObservableCollection(Of Customer)()
            For i As Integer = 1 To 30 - 1
                Customers.Add(New Customer() With {.ID = i, .Name = "Customer" & i, .RegistrationDate = Date.Now.AddDays(i)})
            Next
        End Sub
    End Class

    Public Class Customer

        Public Property ID As Integer

        Public Property Name As String

        Public Property RegistrationDate As Date
    End Class
End Namespace
