using System;
using NUnit.Framework;

namespace SignInCheckIn.Tests
{
    [TestFixture]
    public class BuildTester
    {
        [Test]
        public void ShouldPass()
        {
            Assert.Pass();
        }

        [Test]
        [Category("IntegrationTests")]
        public void ShouldPassLocallyButNotRunOnBuildServer()
        {
            Assert.Pass();
        }
    }
}
