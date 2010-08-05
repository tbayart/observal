using System;
using Observal.Utilities;
using NUnit.Framework;

namespace Observal.Tests.Utilities
{
    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void ArgumentNullTest()
        {
            Guard.ArgumentNotNull("foo", "x");
            Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNull(null, "x"));
        }

        [Test]
        public void ArgumentNullOrEmptyTest()
        {
            Guard.ArgumentNotNullOrEmpty("foo", "x");
            Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNullOrEmpty(null, "x"));
            Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty("", "x"));
            Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty("  ", "x"));
        }

        [Test]
        public void ArgumentIsOfTypeTest()
        {
            Guard.ArgumentIsOfType("foo", typeof(string), "x");
            Assert.Throws<ArgumentException>(() => Guard.ArgumentIsOfType("foo", typeof(int), "x"));
            Assert.Throws<ArgumentException>(() => Guard.ArgumentIsOfType(null, typeof(string), "x"));
        }
    }
}
