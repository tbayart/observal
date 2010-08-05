using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Observal.Demo
{
    public class Employee : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Employee> _employees = new ObservableCollection<Employee>();
        private string _name;
        private decimal _salary;

        public Employee(string name, decimal salary, params Employee[] initialDirectReports)
        {
            _name = name;
            _salary = salary;

            foreach (var item in initialDirectReports)
            {
                _employees.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
        }

        public decimal Salary
        {
            get { return _salary; }
            set { _salary = value; OnPropertyChanged(new PropertyChangedEventArgs("Salary")); }
        }

        public ObservableCollection<Employee> DirectReports
        {
            get { return _employees; }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}
