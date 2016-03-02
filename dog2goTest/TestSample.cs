using NUnit.Framework;

namespace dog2goTest
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