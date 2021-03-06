namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    public class AndConditionTests
    {
        private Mock<ICondition> _mock1;
        private Mock<ICondition> _mock2;
        private Mock<ICondition> _mock3;

        [SetUp]
        public void SetUp()
        {
            _mock1 = new Mock<ICondition>();
            _mock2 = new Mock<ICondition>();
            _mock3 = new Mock<ICondition>();
        }

        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, null)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, null, false)]
        [TestCase(false, null, null, false)]
        [TestCase(null, null, null, null)]
        public void IsSatisfied(bool? first, bool? second, bool? third, bool? expected)
        {
            _mock1.SetupGet(x => x.IsSatisfied).Returns(first);
            _mock2.SetupGet(x => x.IsSatisfied).Returns(second);
            _mock3.SetupGet(x => x.IsSatisfied).Returns(third);
            var collection = new AndCondition(_mock1.Object, _mock2.Object, _mock3.Object);
            Assert.AreEqual(expected, collection.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var argses = new List<string>();
            var fake1 = new FakeInpc { Prop1 = false };
            var fake2 = new FakeInpc { Prop1 = false };
            var fake3 = new FakeInpc { Prop1 = false };
            var condition1 = new Condition(fake1.ToObservable(x => x.Prop1), () => fake1.Prop1);
            var condition2 = new Condition(fake2.ToObservable(x => x.Prop1), () => fake2.Prop1);
            var condition3 = new Condition(fake3.ToObservable(x => x.Prop1), () => fake3.Prop1);
            var collection = new AndCondition(condition1, condition2, condition3);
            collection.PropertyChanged += (sender, args) => argses.Add(args.PropertyName);
            Assert.AreEqual(false, collection.IsSatisfied);
            fake1.Prop1 = true;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(0, argses.Count);

            fake2.Prop1 = true;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(0, argses.Count);

            fake3.Prop1 = true;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake1.Prop1 = false;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(2, argses.Count);

            fake2.Prop1 = false;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(2, argses.Count);

            fake3.Prop1 = false;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(2, argses.Count);
        }

        [Test]
        public void ThrowsIfEmpty()
        {
            Assert.Throws<ArgumentException>(() => new AndCondition());
        }
    }
}