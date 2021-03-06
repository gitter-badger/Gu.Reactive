﻿namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Reactive;
    using Gu.Reactive.Tests;

    using NUnit.Framework;

    public class ObservingRelayCommandTests
    {
        [Test]
        public void NotifiesOnConditionChanged()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1);
            var command = new ObservingRelayCommand(_ => { }, _ => false, observable, false);
            int count = 0;
            command.CanExecuteChanged += (sender, args) => count++;
            fake.Prop1 = true;
            Assert.AreEqual(1, count);
        }
    }
}