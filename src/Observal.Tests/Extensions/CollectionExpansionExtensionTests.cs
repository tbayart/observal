﻿using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Observal.Extensions;

namespace Observal.Tests.Extensions
{
    [TestFixture]
    public class CollectionExpansionExtensionTests
    {
        [Test]
        public void AutoExpandsObservableCollections()
        {
            var observer = new Observer();
            observer.Extend(new CollectionExpansionExtension());

            var observable = new ObservableCollection<object>();
            observable.Add("Item 1");
            observable.Add("Item 2");

            observer.Add(observable);

            Assert.AreEqual(3, observer.GetAll().Count);

            // Since it's INotifyCollectionChanged, new items will be enlisted automatically
            observable.Add("Item 3");

            Assert.AreEqual(4, observer.GetAll().Count);
        }

        [Test]
        public void WeakEventsEnableGarabageCollection()
        {
            var observable = new ObservableCollection<object>();
            observable.Add("Item 1");
            observable.Add("Item 2");

            var observerReference = null as WeakReference;
            new Action(() =>
            {
                var observer = new Observer();
                observer.Extend(new CollectionExpansionExtension()).UseWeakEvents();
                observer.Add(observable);
                observerReference = new WeakReference(observer);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsNull(observerReference.Target);
        }

        [Test]
        public void WeakEventsAreNotEnabledByDefault()
        {
            var observable = new ObservableCollection<object>();
            observable.Add("Item 1");
            observable.Add("Item 2");

            var observerReference = null as WeakReference;
            new Action(() =>
            {
                var observer = new Observer();
                observer.Extend(new CollectionExpansionExtension());
                observer.Add(observable);
                observerReference = new WeakReference(observer);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsNotNull(observerReference.Target);
        }
    }
}