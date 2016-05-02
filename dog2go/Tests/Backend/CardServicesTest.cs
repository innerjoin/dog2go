using dog2go.Backend.Services;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class CardServicesTest
    {
        private CardServices cs;

        [SetUp]
        public void Init()
        {
            cs = new CardServices();
        }

        [Test]
        public void GetNumberOfCardsPerUser()
        {
            Assert.AreEqual(6, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(5, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(4, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(3, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(2, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(6, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(5, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(4, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(3, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(2, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(6, cs.GetNumberOfCardsPerUser());
            cs.CurrentRound++;
            Assert.AreEqual(5, cs.GetNumberOfCardsPerUser());
            // ... and so on...
        }

        [Test]
        public void GetCard_Initialization()
        {
            Assert.AreEqual(0, cs.GetDeckSize());
            cs.GetCard(); // will remove the top element, therefore -1 below
            Assert.AreEqual(110 - 1, cs.GetDeckSize());
        }
    }
}