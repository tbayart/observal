using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Observal.Extensions;

namespace Observal.Tests.Extensions
{
    [TestFixture]
    public class ObserverExtensionTests
    {
        public class CounterExtension : ObserverExtension
        {
            public int Count { get; private set; }

            public void SomeSetting()
            {
                AssertNotConfiguredYet("Should not be invoked after config.");
            }

            protected override void Attach(object attachedItem)
            {
                Count++;
            }

            protected override void Detach(object detachedItem)
            {
                Count--;
            }
        }

        [Test]
        public void CanUseAssertNotConfiguredYetToEnsureSettingsAreSetBeforeConfigure()
        {
            var ex = new CounterExtension();

            var observer = new Observer();
            observer.Extend(ex).SomeSetting();

            observer.Add("Hello"); // Configured

            Assert.Throws<InvalidOperationException>(ex.SomeSetting);
        }

        [Test]
        public void EnsuresCannotBeConfiguredTwice()
        {
            var ex = new CounterExtension();

            var observer = new Observer();
            observer.Extend(ex).SomeSetting();

            observer.Add("Hello"); // Configured

            Assert.Throws<InvalidOperationException>(() => ((IObserverExtension)ex).Configure(new Observer()));
        }
    }
}
