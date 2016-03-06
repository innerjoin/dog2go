using NUnit.Framework;

namespace dog2goTest
{
    [TestFixture]
    public class MSTest_Sample
    {
        [Test]
        public void JustAHappyTest()
        {
            Assert.That(true, Is.EqualTo(true));    
        }
    }
}