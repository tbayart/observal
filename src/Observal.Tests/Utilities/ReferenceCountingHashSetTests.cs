using System;
using Observal.Utilities;
using NUnit.Framework;

namespace Observal.Tests.Utilities
{
    [TestFixture]
    public class ReferenceCountingHashSetTests
    {
        [Test]
        public void KeepsRecordOfReferences()
        {
            var items = new ReferenceCountingHashSet();
            items.Add("Item 1");
            items.Add("Item 2");
            items.Add("Item 2");   // Item 2 will have reference count of 1

            Assert.AreEqual(2, items.Count);
            Assert.IsTrue(items.Contains("Item 1"));
            Assert.IsTrue(items.Contains("Item 2"));

            items.Remove("Item 1");
            items.Remove("Item 2");

            Assert.AreEqual(1, items.Count);
            Assert.IsFalse(items.Contains("Item 1"));
            Assert.IsTrue(items.Contains("Item 2"));

            items.Remove("Item 2");
            
            Assert.AreEqual(0, items.Count);
            Assert.IsFalse(items.Contains("Item 2"));
        }

        [Test]
        public void IgnoresNull()
        {
            var items = new ReferenceCountingHashSet();
            items.Add(null);
            items.Add(null);
            items.Add(null);

            Assert.AreEqual(0, items.Count);

            Assert.IsFalse(items.Remove(null));
            items.Remove(null);

            Assert.IsFalse(items.Contains(null));
        }

        [Test]
        public void RemoveReturnValueIndicatesIfRefCountWasZero()
        {
            var items = new ReferenceCountingHashSet();
            items.Add("Item 1");
            items.Add("Item 1");

            Assert.IsFalse(items.Remove("Item 1"));
            Assert.IsTrue(items.Remove("Item 1"));
        }

        [Test]
        public void RemoveIgnoresItemsThatDidntExistAnyway()
        {
            var items = new ReferenceCountingHashSet();
            
            Assert.IsFalse(items.Remove(Guid.NewGuid()));
        }

        [Test]
        public void GetAllReturnsSnapshotListOfItems()
        {
            var items = new ReferenceCountingHashSet();
            items.Add("Item 1");
            items.Add("Item 2");
            items.Add("Item 3");
            items.Add("Item 3");

            var all = items.GetAll();
            Assert.AreEqual(3, all.Count);
            Assert.AreEqual("Item 1", all[0]);
            Assert.AreEqual("Item 2", all[1]);
            Assert.AreEqual("Item 3", all[2]);

            items.Add("Item 4");

            Assert.AreEqual(3, all.Count);
            
            all = items.GetAll();
            
            Assert.AreEqual(4, all.Count);
        }
    }
}
