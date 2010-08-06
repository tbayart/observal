using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Observal.Extensions;
using NUnit.Framework;

namespace Observal.Tests.Extensions
{
    [TestFixture]
    public class HierarchicyExtensionTests
    {
        [DebuggerDisplay("Employee: {Name}")]
        public class Employee
        {
            private readonly string _name;
            private readonly ObservableCollection<Employee> _friends = new ObservableCollection<Employee>();
            private readonly ObservableCollection<Employee> _directReports;

            public Employee(string name, params Employee[] directReports)
            {
                _name = name;
                _directReports = new ObservableCollection<Employee>(directReports);
            }

            public string Name
            {
                get { return _name; }
            }

            public ObservableCollection<Employee> Friends
            {
                get { return _friends; }
            }

            public ObservableCollection<Employee> DirectReports
            {
                get { return _directReports; }
            }
        }

        [Test]
        public void CanExpandHierarchy()
        {
            var boss = new Employee("Pointy Haired Boss",
                new Employee("Catbert",
                    new Employee("Dilbert"),
                    new Employee("Wally"),
                    new Employee("Alice")));

            var observer = new Observer();
            observer.Extend(new CollectionExpansionExtension());
            observer.Extend(new HierarchyExtension()
                .AddChildren<Employee>(x => x.DirectReports));
            
            observer.Add(boss);

            Assert.AreEqual(5, observer.GetAll().OfType<Employee>().Count());
        }


        [Test]
        public void CanDetachBranchOfTree()
        {
            var boss = new Employee("Pointy Haired Boss",
                new Employee("Catbert",
                    new Employee("Dilbert"),
                    new Employee("Wally"),
                    new Employee("Alice")));

            var observer = new Observer();
            observer.Extend(new CollectionExpansionExtension());
            observer.Extend(new HierarchyExtension()
                .AddChildren<Employee>(x => x.DirectReports));

            observer.Add(boss);

            Assert.AreEqual(5, observer.GetAll().OfType<Employee>().Count());

            // Catbert is fired - all direct reports go
            boss.DirectReports.Clear();

            Assert.AreEqual(1, observer.GetAll().OfType<Employee>().Count());
        }

        [Test]
        public void IsNotConfusedWhenItemOccursTwiceInHierarchy()
        {
            var boss = new Employee("Pointy Haired Boss",
                new Employee("Catbert",
                    new Employee("Dilbert"),
                    new Employee("Wally"),
                    new Employee("Alice")));

            var catbert = boss.DirectReports[0];
            var dilbert = catbert.DirectReports[0];
            var wally = catbert.DirectReports[1];
            dilbert.Friends.Add(wally);

            var observer = new Observer();
            observer.Extend(new CollectionExpansionExtension());
            observer.Extend(new HierarchyExtension()
                .AddChildren<Employee>(x => x.DirectReports)
                .AddChildren<Employee>(x => x.Friends));

            observer.Add(boss);

            Assert.AreEqual(5, observer.GetAll().OfType<Employee>().Count());

            // Wally has been fired, but is still Dilbert's friend
            catbert.DirectReports.Remove(wally);

            Assert.AreEqual(5, observer.GetAll().OfType<Employee>().Count());

            // Unemployed Wally has moved on and found new friends
            dilbert.Friends.Remove(wally);

            Assert.AreEqual(4, observer.GetAll().OfType<Employee>().Count());
        }

        [Test]
        public void DoesNotAddCollectionExpansionExtensionIfAlreadyExists()
        {
            var observer = new Observer();
            observer.Extend(new CollectionExpansionExtension());
            observer.Extend(new HierarchyExtension());
            observer.Add(3);
        }
    }
}