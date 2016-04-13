using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class NunitSample
    {
        [Test]
        public void dog_justAHappyTest()
        {
            Assert.That(true, Is.EqualTo(true));    
        }

        [Test]
        public void dog_JustAnotherHappyTest()
        {
            Assert.That(false, Is.EqualTo(false));
        }

    }
}