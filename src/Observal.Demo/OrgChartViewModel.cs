using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Observal.Extensions;

namespace Observal.Demo
{
    public class OrgChartViewModel
    {
        private readonly ObservableCollection<Employee> _rootEmployees = new ObservableCollection<Employee>();
        private readonly ObservableCollection<Employee> _filteredEmployees = new ObservableCollection<Employee>();
        private readonly Observer _observer;

        public OrgChartViewModel(IEnumerable<Employee> employees)
        {
            AddChild = new RelayCommand<Employee>(AddChildExecuted);

            foreach (var item in employees)
            {
                _rootEmployees.Add(item);
            }

            var propertyWatcher = new PropertyChangedExtension();
            propertyWatcher.PropertyChanged += (s, e) => FilterEmployee(s);

            var addRemove = new ItemsChangedExtension();
            addRemove.ItemAdded += (s, e) => FilterEmployee(e.Item);
            addRemove.ItemRemoved += (s, e) => FilterEmployee(e.Item);

            _observer = new Observer();
            _observer.AddExtension(new HierarchyExtension().AddChildren<Employee>(e => e.DirectReports));
            _observer.AddExtension(propertyWatcher);
            _observer.AddExtension(addRemove);
            _observer.Add(_rootEmployees);
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
