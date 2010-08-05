using System;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Observal.Tests
{
    [TestFixture]
    public class ObservableCollectionTrackerTests
    {
        [Test]
        public void AddedCallbackIsRaisedForExistingItems()
        {
            var addedCount = 0;
            var removedCount = 0;

            var collection = new ObservableCollection<object>();
            collection.Add("Item 1");
            collection.Add("Item 2");
            var tracker = new ObservableCollectionTracker(x => addedCount++, x => removedCount++);
            tracker.Attach(collection);

            Assert.AreEqual(2, addedCount);
            Assert.AreEqual(0, removedCount);
        }

        [Test]
        public void AddedCallbackIsRaisedForNewItems()
        {
            var addedCount = 0;
            var removedCount = 0;

            var collection = new ObservableCollection<object>();
            var tracker = new ObservableCollectionTracker(x => addedCount++, x => removedCount++);
            tracker.Attach(collection);

            collection.Add("Item 1");
            
            Assert.AreEqual(1, addedCount);
            Assert.AreEqual(0, removedCount);
        }

        [Test]
        public void RemovedCallbackIsRaisedForDeletedItems()
        {
            var addedCount = 0;
            var removedCount = 0;

            var collection = new ObservableCollection<object>();
            var tracker = new ObservableCollectionTracker(x => addedCount++, x => removedCount++);
            collection.Add("Item 1");
            tracker.Attach(collection);

            collection.Remove("Item 1");
            
            Assert.AreEqual(1, addedCount);
            Assert.AreEqual(1, removedCount);
        }

        [Test]
        public void CanOnlyBeAttachedToOneSourceAtOnce()
        {
            var collection = new ObservableCollection<object>();
            var collection2 = new ObservableCollection<object>();
            var tracker = new ObservableCollectionTracker(x => { }, x => { });
            tracker.Attach(collection);
            Assert.Throws<InvalidOperationException>(() => tracker.Attach(collection2));
        }

        [Test]
        public void IgnoresAttachToNull()
        {
            var collection = new ObservableCollection<object>();
            var tracker = new ObservableCollectionTracker(x => { }, x => { });
            tracker.Attach(null);
            tracker.Attach(collection);
        }

        [Test]
        public void IgnoresDuplicates()
        {
            var addedCount = 0;
            var removedCount = 0;

            var collection = new ObservableCollection<object>();
            collection.Add("Item 1");
            collection.Add("Item 2");
            collection.Add("Item 2");
            var tracker = new ObservableCollectionTracker(x => addedCount++, x => removedCount++);
            tracker.Attach(collection);

            Assert.AreEqual(2, addedCount);
            Assert.AreEqual(0, removedCount);

            collection.Add("Item 3");
            collection.Add("Item 4");
            collection.Add("Item 4");

            Assert.AreEqual(4, addedCount);
            Assert.AreEqual(0, removedCount);

            collection.Remove("Item 4");
            collection.Remove("Item 4");

            Assert.AreEqual(1, removedCount);
        }
    }
}
