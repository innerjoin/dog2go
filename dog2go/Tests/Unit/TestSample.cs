using NUnit.Framework;

namespace dog2go.Tests.Unit
{
    [TestFixture]
    public class TestSample
    {
        [Test]
        public void JustAHappyTest()
        {
            Assert.That(true, Is.EqualTo(true));    
        }
    }
}