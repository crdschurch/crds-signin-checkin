using Crossroads.ApiVersioning;
using NUnit.Framework;
using System;

namespace SignInCheckIn.Tests.ApiVersioning
{
    public class SemanticVersionTest
    {
        private SemanticVersion _fixture;

        [Test]
        public void TestSemanticVersionMajorMinorPatch()
        {
            _fixture = new SemanticVersion("9.8.7");
            Assert.AreEqual(new Version("9.8.7"), _fixture.Version);
        }

        [Test]
        public void TestSemanticVersionMajorMinor()
        {
            _fixture = new SemanticVersion("9.8");
            Assert.AreEqual(new Version("9.8.0"), _fixture.Version);
        }

        [Test]
        public void TestSemanticVersionMajor()
        {
            _fixture = new SemanticVersion("9");
            Assert.AreEqual(new Version("9.0.0"), _fixture.Version);
        }

        [Test]
        public void TestIsBefore()
        {
            _fixture = new SemanticVersion("1.2.3");

            Assert.IsTrue(_fixture.IsBefore("2.3.4"));
            Assert.IsTrue(_fixture.IsBefore(new SemanticVersion("2.3.4")));

            Assert.IsFalse(_fixture.IsBefore("1.1.3"));
            Assert.IsFalse(_fixture.IsBefore(new SemanticVersion("1.1.3")));

            Assert.IsFalse(_fixture.IsBefore("1.2.3"));
            Assert.IsFalse(_fixture.IsBefore(new SemanticVersion("1.2.3")));
        }
    }
}
