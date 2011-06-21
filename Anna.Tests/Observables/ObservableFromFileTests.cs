using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Anna.Observables;
using NUnit.Framework;
using SharpTestsEx;

namespace Anna.Tests.Observables
{
    [TestFixture]
    public class ObservableFromFileTests
    {

        [Test]
        public void ShouldWork()
        {
            var ready = false;
            var bytes = new List<byte[]>();
            var obs = new ObservableFromFile(@"samples\example_1.txt");
            obs.Subscribe(bytes.Add, e => { }, () => { ready = true; });
            while (!ready)
            {
                Thread.Sleep(10);
            }

            var array = bytes.SelectMany(b => b)
                            .Skip(3) // ignore the first 3bytes
                            .ToArray();
            var result = Encoding.UTF8.GetString(array);
            var expected = String.Join(Environment.NewLine, Enumerable.Range(1,9));
            result.Should().Be.EqualTo(expected);
        }

        [Test]
        public void CanReadChunked()
        {
            var ready = false;
            var bytes = new List<byte[]>();
            var obs = new ObservableFromFile(@"samples\example_1.txt", 3);
            obs.Subscribe(bytes.Add, e => { }, () => { ready = true; });
            while (!ready)
            {
                Thread.Sleep(10);
            }

            var array = bytes.SelectMany(b => b)
                            .Skip(3) // ignore the first 3bytes
                            .ToArray();

            var result = Encoding.UTF8.GetString(array);
            var expected = String.Join(Environment.NewLine, Enumerable.Range(1, 9));
            result.Should().Be.EqualTo(expected);
        }

    }
}