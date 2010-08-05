using System;
using System.ComponentModel;
using System.Diagnostics;
using Observal.Eventing;
using NUnit.Framework;

namespace Observal.Tests.Eventing
{
    [TestFixture]
    public class WeakEventsTests
    {
        #region Example

        public class Alarm
        {
            public event PropertyChangedEventHandler Beeped;

            public void Beep()
            {
                var handler = Beeped;
                if (handler != null) handler(this, new PropertyChangedEventArgs("Beep!"));
            }
        }

        public class Sleepy
        {
            private readonly Alarm _alarm;
            private int _snoozeCount;
            
            public Sleepy(Alarm alarm)
            {
                _alarm = alarm;
                _alarm.Beeped += new WeakEventHandler<PropertyChangedEventArgs>(Alarm_Beeped).WeakHandler;
            }

            private void Alarm_Beeped(object sender, PropertyChangedEventArgs e)
            {
                _snoozeCount++;
            }

            public int SnoozeCount
            {
                get { return _snoozeCount; }
            }
        }

        #endregion

        [Test]
        public void ShouldHandleEventWhenBothReferencesAreAlive()
        {
            var alarm = new Alarm();
            var sleepy = new Sleepy(alarm);
            alarm.Beep();
            alarm.Beep();

            Assert.AreEqual(2, sleepy.SnoozeCount);
        }

        [Test]
        public void ShouldAllowSubscriberReferenceToBeCollected()
        {
            var alarm = new Alarm();
            var sleepyReference = null as WeakReference;
            new Action(() =>
            {
                // Run this in a delegate to that the local variable gets garbage collected
                var sleepy = new Sleepy(alarm);
                alarm.Beep();
                alarm.Beep();
                Assert.AreEqual(2, sleepy.SnoozeCount);
                sleepyReference = new WeakReference(sleepy);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsNull(sleepyReference.Target);
        }

        [Test]
        public void DontCrashWhenSubscriberReferenceIsAlive()
        {
            var alarm = new Alarm();
            var sleepyReference = null as WeakReference;
            new Action(() =>
            {
                // Run this in a delegate to that the local variable gets garbage collected
                var sleepy = new Sleepy(alarm);
                alarm.Beep();
                alarm.Beep();
                Assert.AreEqual(2, sleepy.SnoozeCount);
                sleepyReference = new WeakReference(sleepy);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsNull(sleepyReference.Target);

            alarm.Beep();
        }

        [Test]
        public void SubscriberShouldNotBeUnsubscribedUntilCollection()
        {
            var alarm = new Alarm();
            var sleepy = new Sleepy(alarm);
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            alarm.Beep();
            alarm.Beep();
            Assert.AreEqual(2, sleepy.SnoozeCount);
        }

        [Test][NUnit.Framework.Category("Performance")]
        public void Performance()
        {
            var alarm = new Alarm();
            var sleepy = new Sleepy(alarm);
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 100000; i++)
            {
                alarm.Beep();
            }

            Assert.AreEqual(100000, sleepy.SnoozeCount);
            Assert.Less(watch.ElapsedMilliseconds, 100);
            Console.WriteLine("Called the delegate 100,000 times in {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
