using System;
using System.Collections.ObjectModel;

namespace HighlightModifiedCells {
    public class ViewModel {
        public ObservableCollection<Customer> Customers { get; set; }
        public ViewModel() {
            Customers = new ObservableCollection<Customer>();
            for (int i = 1; i < 30; i++) {
                Customers.Add(new Customer() { ID = i, Name = "Customer" + i, RegistrationDate = DateTime.Now.AddDays(i) });
            }
        }
    }

    public class Customer {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
