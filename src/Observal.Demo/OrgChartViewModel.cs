using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Observal.Extensions;

namespace Observal.Demo
{
    public class OrgChartViewModel
    {
        private readonly ObservableCollection<Employee> _rootEmployees = new ObservableCollection<Employee>();
        private readonly ObservableCollection<Employee> _filteredEmployees = new ObservableCollection<Employee>();
        
        public OrgChartViewModel(IEnumerable<Employee> employees)
        {
            AddChild = new RelayCommand<Employee>(AddChildExecuted, x => x != null);

            foreach (var item in employees)
            {
                _rootEmployees.Add(item);
            }

            var observer = new Observer();
            observer.Extend(new TraverseExtension()).Follow<Employee>(e => e.DirectReports);
            observer.Extend(new CollectionExpansionExtension());
            observer.Extend(new PropertyChangedExtension()).WhenPropertyChanges(x => FilterEmployee(x.Source));
            observer.Extend(new ItemsChangedExtension()).WhenAdded(FilterEmployee);
            observer.Add(_rootEmployees);
        }

        private void FilterEmployee(object item)
        {
            var employee = item as Employee;
            if (employee == null)
                return;

            if (employee.Salary < 100000)
            {
                if (!FilteredEmployees.Contains(employee))
                    FilteredEmployees.Add(employee);
            }
            else
            {
                FilteredEmployees.Remove(employee);
            }
        }

        public ICommand AddChild { get; set; }

        public ObservableCollection<Employee> RootEmployees
        {
            get { return _rootEmployees; }
        }

        public ObservableCollection<Employee> FilteredEmployees
        {
            get { return _filteredEmployees; }
        }

        private void AddChildExecuted(Employee selectedItem)
        {
            if (selectedItem == null)
                return;

            var child = new Employee("New Employee", 0.0M);
            selectedItem.DirectReports.Add(child);
        }

        public System.ComponentModel.PropertyChangedEventHandler PropertyWatcher_PropertyChanged { get; set; }
    }
}
