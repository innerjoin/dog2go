using NUnit.Framework;

namespace dog2goTest
{
    [TestFixture]
    public class Nunit_Sample
    {
        [Test]
        public void Nunit_JustAHappyTest()
        {
            Assert.That(true, Is.EqualTo(true));    
        }

        [Test]
        public void Nunit_JustAnotherHappyTest()
        {
            Assert.That(false, Is.EqualTo(false));
        }

        [Test]
        public void Nunit_JustASadTest()
        {
            Assert.That(true, Is.EqualTo(false));
        }
    }
}