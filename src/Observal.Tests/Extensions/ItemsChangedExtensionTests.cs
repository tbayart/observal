using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Observal.Extensions;

namespace Observal.Tests.Extensions
{
    [TestFixture]
    public class ItemsChangedExtensionTests
    {
        [Test]
        public void NotifiesOfAdds()
        {
            var added = new List<string>();

            var observer = new Observer();
            observer.Extend(new ItemsChangedExtension()).WhenAdded(x => added.Add((string)x));
            observer.Add("Item 1");
            observer.Add("Item 2");

            Assert.AreEqual(2, added.Count);
            Assert.AreEqual("Item 1", added[0]);
            Assert.AreEqual("Item 2", added[1]);
        }

        [Test]
        public void NotifiesOfRemoves()
        {
            var removed = new List<string>();

            var observer = new Observer();
            observer.Extend(new ItemsChangedExtension()).WhenRemoved(x => removed.Add((string)x));
            observer.Add("Item 1");
            observer.Add("Item 2");
            observer.Release("Item 2");

            Assert.AreEqual(1, removed.Count);
            Assert.AreEqual("Item 2", removed[0]);
        }

        [Test]
        public void NotifiesOfAddsAndRemoves()
        {
            var addedOrRemoved = new List<string>();
            
            var observer = new Observer();
            observer.Extend(new ItemsChangedExtension()).WhenAddedOrRemoved(x => addedOrRemoved.Add((string)x));
            observer.Add("Item 1");
            observer.Add("Item 2");
            observer.Release("Item 2");

            Assert.AreEqual(3, addedOrRemoved.Count);
            Assert.AreEqual("Item 1", addedOrRemoved[0]);
            Assert.AreEqual("Item 2", addedOrRemoved[1]);
            Assert.AreEqual("Item 2", addedOrRemoved[2]);
        }
    }
}