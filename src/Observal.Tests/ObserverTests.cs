using System;
using NUnit.Framework;

namespace Observal.Tests
{
    [TestFixture]
    public class ObserverTests
    {
        private class CountItemsExtension : IObserverExtension
        {
            public int ItemCount { get; set; }
            
            public void Configure(Observer observer)
            {
                
            }

            public void Attach(Observer observer, object attachedItem)
            {
                ItemCount++;
            }

            public void Detach(Observer observer, object detachedItem)
            {
                ItemCount--;
            }
        }

        [Test]
        public void ObserverNotifiesExtensionsWhenItemsAdded()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            observer.Add("Item 1");
            observer.Add("Item 2");

            Assert.AreEqual(2, counter.ItemCount);
        }

        [Test]
        public void CannotAddExtensionsAfterAddingItems()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            
            observer.Add("Item 1");
            
            Assert.Throws<InvalidOperationException>(() => observer.AddExtension(counter));
        }

        [Test]
        public void ObserverNotifiesExtensionsWhenItemsRemoved()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            observer.Add("Item 1");
            observer.Add("Item 2");
            
            Assert.AreEqual(2, counter.ItemCount);

            observer.Release("Item 2");

            Assert.AreEqual(1, counter.ItemCount);
        }

        [Test]
        public void ObserverIgnoresDuplicatesBasedOnHashCode()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            observer.Add("Item 1");
            observer.Add("Item 2");
            observer.Add("Item 2");

            Assert.AreEqual(2, counter.ItemCount);
        }

        [Test]
        public void ObserverIgnoresRemovalOfItemsThatWerentObservedAnyway()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            observer.Release("Item 42");

            Assert.AreEqual(0, counter.ItemCount);
        }

        [Test]
        public void ObserverExtensionsCannotAddItemsDuringConfiguration()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            observer.AddExtension(new NaughtyExtension());
            Assert.Throws<InvalidOperationException>(() => observer.Add("Jack"));
        }

        [Test]
        public void GetAllCanBeUsedToGetListOfAllItems()
        {
            var observer = new Observer();
            observer.Add("Item 1");
            observer.Add("Item 2");

            var all = observer.GetAll();
            Assert.AreEqual(2, all.Count);
            Assert.AreEqual("Item 1", all[0]);
            Assert.AreEqual("Item 2", all[1]);

            // Note that the collection is a read-only snapshot
            observer.Add("Whatever");
            Assert.AreEqual(2, all.Count);
            var allAgain = observer.GetAll();
            Assert.AreEqual(3, allAgain.Count);
        }

        [Test]
        public void HasExtensionCanBeUsedToFindExtensions()
        {
            var counter = new CountItemsExtension();
            var observer = new Observer();
            observer.AddExtension(counter);

            Assert.IsTrue(observer.HasExtension(x => x is CountItemsExtension));
            Assert.IsFalse(observer.HasExtension(x => x is NaughtyExtension));
        }
    }

    public class NaughtyExtension : IObserverExtension
    {
        public void Configure(Observer observer)
        {
            observer.Add("Foo");
        }

        public void Attach(Observer observer, object attachedItem)
        {
        }

        public void Detach(Observer observer, object detachedItem)
        {
        }
    }
}
