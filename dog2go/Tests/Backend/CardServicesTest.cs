using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class CardServicesTest
    {
        private CardServices cs;
        private readonly GameHub _hub = new GameHub(GameRepository.Instance);
        private const int TableId = 0;

        private GameTable MakeInitialGameTable
        {
            get
            {
                GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
                List<Participation> participationsList = new List<Participation>();
                User user1 = new User("user1", "1");
                User user2 = new User("user2", "2");
                User user3 = new User("user3", "3");
                User user4 = new User("user4", "4");
                Participation participation1 = new Participation(user1);
                Participation participation2 = new Participation(user2);
                Participation participation3 = new Participation(user3);
                Participation participation4 = new Participation(user4);

                participation1.Partner = user3;
                participation2.Partner = user4;
                participation3.Partner = user1;
                participation4.Partner = user2;

                participationsList.Add(participation1);
                participationsList.Add(participation2);
                participationsList.Add(participation3);
                participationsList.Add(participation4);

                gameTable.Participations = participationsList;

                for (int i = 0; i < gameTable.PlayerFieldAreas.Count; i++)
                {
                    PlayerFieldArea currentPlayerFieldArea = gameTable.PlayerFieldAreas[i];
                    currentPlayerFieldArea.Participation = participationsList[i];
                }
                gameTable.RegisterCardService(new CardServices());
                return gameTable;
            }
        }

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

        [Test]
        public void TestCardExchange()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<HandCard> handCardsUser = new List<HandCard>();
            List<HandCard> handCardsPartner = new List<HandCard>();
            for (int i = 0; i < cs.GetNumberOfCardsPerUser(); i++)
            {
                handCardsUser.Add(new HandCard(cs.GetCard()));
                handCardsPartner.Add(new HandCard(cs.GetCard()));
            }
            User user = actualGameTable.Participations.Find(participation =>
            {
                if (participation.Participant.Nickname == "user1")
                {
                    participation.ActualPlayRound = new PlayRound(1, 6) { Cards = handCardsUser };
                    return true;
                }
                return false;
            }).Participant;
            User partner = actualGameTable.Participations.Find(partnerParticipation => {
                if (partnerParticipation.Participant.Nickname == "user3")
                {
                    partnerParticipation.ActualPlayRound = new PlayRound(1, 6) { Cards = handCardsPartner };
                    return true;
                }
                return false;
            }).Participant;
            HandCard firstCard = handCardsUser.First();
            HandCard lastHandCard = handCardsPartner.Last();
            cs.CardExchange(user, ref actualGameTable, firstCard);
            cs.CardExchange(partner, ref actualGameTable, lastHandCard);
            Assert.AreEqual(true, actualGameTable.Participations.Find(participation => participation.Participant.Nickname == "user1").ActualPlayRound.Cards.FindAll(card => card.Id ==lastHandCard.Id).Count >=1);
            Assert.AreEqual(true, actualGameTable.Participations.Find(participation => participation.Participant.Nickname == "user3").ActualPlayRound.Cards.FindAll(card => card.Id == firstCard.Id).Count >=1);
        }

        [Test]
        public void TestCardExchangeNothingToChange()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<HandCard> handCardsUser = new List<HandCard>();
            List<HandCard> handCardsPartner = new List<HandCard>();
            for (int i = 0; i < cs.GetNumberOfCardsPerUser(); i++)
            {
                handCardsUser.Add(new HandCard(cs.GetCard()));
                handCardsPartner.Add(new HandCard(cs.GetCard()));
            }
            User user = actualGameTable.Participations.Find(participation =>
            {
                if (participation.Participant.Nickname == "user1")
                {
                    participation.ActualPlayRound = new PlayRound(1, 6) { Cards = handCardsUser };
                    return true;
                }
                return false;
            }).Participant;
            User partner = actualGameTable.Participations.Find(partnerParticipation => {
                if (partnerParticipation.Participant.Nickname == "user3")
                {
                    partnerParticipation.ActualPlayRound = new PlayRound(1, 6) { Cards = handCardsPartner };
                    return true;
                }
                return false;
            }).Participant;
            cs.CardExchange(user, ref actualGameTable, null);
            Assert.AreEqual(6, actualGameTable.Participations.Find(participation => participation.Participant.Nickname == "user1").ActualPlayRound.Cards.Count);
        }

        [Test]
        public void TestGetOtherMeeples()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red).Meeples;
            List<Meeple> allOtherMeeples =new List<Meeple>(); 
            allOtherMeeples.AddRange(actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples);
            allOtherMeeples.AddRange(actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples);
            allOtherMeeples.AddRange(actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Yellow).Meeples);
            List<Meeple> otherMeeples = GameTableService.GetOtherMeeples(actualGameTable, myMeeples);
            Assert.AreEqual(otherMeeples.Count, allOtherMeeples.Count);
        }

        [Test]
        public void TestGetOtherMeeplesNotInitialized()
        {
            List<Meeple> otherMeeples = GameTableService.GetOtherMeeples(null, null);
            Assert.AreEqual(otherMeeples, null);
        }

        [Test]
        public void TestGetOpenMeeplesNoAvailable()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> openMeeples = GameTableService.GetOpenMeeples(actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples);
            Assert.AreEqual(0, openMeeples.Count);
        }

        [Test]
        public void TestGetOpenMeeples()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            Meeple meeple = myMeeples.First();
            meeple.IsStartFieldBlocked = false;
            meeple.CurrentPosition =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7];
            List<Meeple> openMeeples = GameTableService.GetOpenMeeples(myMeeples);
            Assert.AreEqual(1, openMeeples.Count);
        }

        [Test]
        public void TestGetOpenMeeplesNotInitialized()
        {
            List<Meeple> otherMeeples = GameTableService.GetOpenMeeples(null);
            Assert.AreEqual(null, otherMeeples);
        }

        [Test]
        public void TestProveLeaveKennelCorrect()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;

            Assert.AreEqual(true, cs.ProveLeaveKennel(myMeeples));
        }

        [Test]
        public void TestProveLeaveKennelWrong()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).StartField.CurrentMeeple = myMeeples.First();
            Assert.AreEqual(false, cs.ProveLeaveKennel(myMeeples));
        }

        [Test]
        public void TestProveLeaveKennelNotInitialized()
        {
            Assert.AreEqual(false, cs.ProveLeaveKennel(null));
        }

        [Test]
        public void TestProveChangeFieldWrongColorCodeStandardField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[8].CurrentMeeple = myMeeples.Last();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, myMeeples));
        }

        [Test]
        public void TestProveChangeFieldWrongColorCodeEndField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).EndFields[0].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).EndFields[1].CurrentMeeple = myMeeples.Last();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, myMeeples));
        }

        [Test]
        public void TestProveChangeFieldWrongColorCodeKennelField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).KennelFields[0].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).KennelFields[1].CurrentMeeple = myMeeples.Last();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, myMeeples));
        }

        [Test]
        public void TestProveChangeFieldCorrectColorCodeStandardField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            List<Meeple> otherMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[8].CurrentMeeple = otherMeeples.First();
            Assert.AreEqual(true, cs.ProveChangePlace(myMeeples, otherMeeples));
        }

        [Test]
        public void TestProveChangeFieldCorrectColorCodeEndField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            List<Meeple> otherMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).EndFields[0].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).EndFields[1].CurrentMeeple = otherMeeples.First();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, otherMeeples));
        }

        [Test]
        public void TestProveChangeFieldCorrectColorCodeKennelField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            List<Meeple> otherMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).KennelFields[0].CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).KennelFields[1].CurrentMeeple = otherMeeples.First();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, otherMeeples));
        }

        [Test]
        public void TestProveChangeFieldCorrectColorCodeStartField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            List<Meeple> otherMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).StartField.CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).StartField.CurrentMeeple = otherMeeples.First();
            Assert.AreEqual(false, cs.ProveChangePlace(myMeeples, otherMeeples));
        }

        [Test]
        public void TestProveChangeFieldCorrectColorCodeStartFieldNotBlocked()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            List<Meeple> otherMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).Meeples;
            myMeeples.First().IsStartFieldBlocked = false;
            otherMeeples.First().IsStartFieldBlocked = false;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).StartField.CurrentMeeple = myMeeples.First();
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green).StartField.CurrentMeeple = otherMeeples.First();
            Assert.AreEqual(true, cs.ProveChangePlace(myMeeples, otherMeeples));
        }

        [Test]
        public void TestProveChangeFieldNotInitialized()
        {
            Assert.AreEqual(false, cs.ProveChangePlace(null, null));
        }

        [Test]
        public void TestProveValueCard()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            Assert.AreEqual(false, cs.ProveValueCard(myMeeples, 3));
        }

        [Test]
        public void TestProveValueCardEndField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).EndFields[0].CurrentMeeple = myMeeples.First();
            Assert.AreEqual(true, cs.ProveValueCard(myMeeples, 2));
        }

        [Test]
        public void TestProveValueCardKennelFieldWrong()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).KennelFields[0].CurrentMeeple = myMeeples.First();
            Assert.AreEqual(false, cs.ProveValueCard(myMeeples, 2));
        }

        [Test]
        public void TestProveValueCardStartField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).StartField.CurrentMeeple = myMeeples.First();
            Assert.AreEqual(true, cs.ProveValueCard(myMeeples, 2));
        }

        [Test]
        public void TestProveValueCardStandardField()
        {
            GameTable actualGameTable = MakeInitialGameTable;
            List<Meeple> myMeeples =
                actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Meeples;
            actualGameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7].CurrentMeeple = myMeeples.First();
            Assert.AreEqual(true, cs.ProveValueCard(myMeeples, 2));
        }
    }
}