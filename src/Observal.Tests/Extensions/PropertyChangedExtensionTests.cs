using System.Collections.Generic;
using System.ComponentModel;
using Observal.Extensions;
using NUnit.Framework;

namespace Observal.Tests.Extensions
{
    [TestFixture]
    public class PropertyChangedExtensionTests
    {
        #region Example
        public class Customer : INotifyPropertyChanged
        {
            private string _firstName;
            private string _lastName;

            public event PropertyChangedEventHandler PropertyChanged;

            public string FirstName
            {
                get { return _firstName; }
                set { _firstName = value; OnPropertyChanged(new PropertyChangedEventArgs("FirstName")); }
            }

            public string LastName
            {
                get { return _lastName; }
                set { _lastName = value; OnPropertyChanged(new PropertyChangedEventArgs("LastName")); }
            }

            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, e);
            }
        }
        #endregion

        [Test]
        public void CanBeNotifiedOfPropertyChangedEvents()
        {
            var customer = new Customer();

            var properties = new List<string>();
            
            var observer = new Observer();
            observer.Extend(new PropertyChangedExtension()).WhenPropertyChanges(x => properties.Add(x.PropertyName));
            observer.Add(customer);

            customer.FirstName = "Paul";
            customer.LastName = "Stovell";

            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual("FirstName", properties[0]);
            Assert.AreEqual("LastName", properties[1]);
        }

        [Test]
        public void UnsubscribesUponItemRemoval()
        {
            var customer = new Customer();

            var properties = new List<string>();
            
            var observer = new Observer();
            observer.Extend(new PropertyChangedExtension()).WhenPropertyChanges(x => properties.Add(x.PropertyName));
            
            observer.Add(customer);

            observer.Add(customer);

            customer.FirstName = "Paul 1";
            Assert.AreEqual(1, properties.Count);

            observer.Release(customer);

            customer.FirstName = "Paul 2";
            Assert.AreEqual(2, properties.Count);

            observer.Release(customer);

            customer.FirstName = "Paul 3";
            Assert.AreEqual(2, properties.Count);
        }

        [Test]
        public void IgnoresNonINPCItems()
        {
            var observer = new Observer();
            observer.Extend(new PropertyChangedExtension());
            observer.Add("Item 1");
            observer.Release("Item 1");
        }
    }
}