using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class NunitGameServicesTest
    {
        private readonly GameHub _hub = new GameHub(GameRepository.Instance);
        private readonly GameServices _gameServices = new GameServices();
        private GameTable MakeInitialGameTable
        {
            get
            {
                GameTable gameTable = _hub.GetGeneratedGameTable();
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

                for (int i = 0; i < gameTable.PlayerFieldAreas.Count; i++)
                {
                    PlayerFieldArea currentPlayerFieldArea = gameTable.PlayerFieldAreas[i];
                    currentPlayerFieldArea.Participation = participationsList[i];
                }
                return gameTable;
            }
        }
        private static bool arePartners(PlayerFieldArea currentPlayerFieldArea, PlayerFieldArea partnerPlayerFieldArea)
        {
            return currentPlayerFieldArea.Participation.Participant.Identifier.Equals(
                partnerPlayerFieldArea.Participation.Participant.Identifier);
        }

        #region "Testmethods for IsGameFinished-Method" 

        [Test]
        public void TestIsGameFinishedPositiveAllFinished()
        {
            GameTable gameTable = MakeInitialGameTable;

            foreach (PlayerFieldArea currentPlayerFieldArea in gameTable.PlayerFieldAreas)
            {
                for (int j = 0; j < 4; j++)
                {
                    currentPlayerFieldArea.EndFields[j].CurrentMeeple = currentPlayerFieldArea.Meeples[j];
                }
            }

            Assert.AreEqual(true, _gameServices.IsGameFinished(gameTable));
        }


        [Test]
        public void TestIsGameFinishedPositiveTeamFinished()
        {
            GameTable gameTable = MakeInitialGameTable;
            foreach (PlayerFieldArea currentPlayerFieldArea in gameTable.PlayerFieldAreas)
            {
                foreach (PlayerFieldArea partnerPlayerFieldArea in gameTable.PlayerFieldAreas)
                {
                    if (arePartners(currentPlayerFieldArea, partnerPlayerFieldArea))
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            currentPlayerFieldArea.EndFields[j].CurrentMeeple = currentPlayerFieldArea.Meeples[j];
                            partnerPlayerFieldArea.EndFields[j].CurrentMeeple = partnerPlayerFieldArea.Meeples[j];
                        }
                        break;
                    }
                }
            }
           Assert.AreEqual(true, _gameServices.IsGameFinished(gameTable));
        }


        [Test]
        public void TestIsGameFinishedNegativeOnlyOneFinished()
        {
            bool partnerFounded = false;
            GameTable gameTable = MakeInitialGameTable;
            foreach (PlayerFieldArea currentPlayerFieldArea in gameTable.PlayerFieldAreas)
            {
                foreach (PlayerFieldArea partnerPlayerFieldArea in gameTable.PlayerFieldAreas)
                {
                    if (arePartners(currentPlayerFieldArea, partnerPlayerFieldArea) && !partnerFounded)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            partnerPlayerFieldArea.EndFields[j].CurrentMeeple = partnerPlayerFieldArea.Meeples[j];
                        }
                        partnerFounded = true;
                        break;
                    }
                }
            }
            Assert.AreEqual(false, _gameServices.IsGameFinished(gameTable));
        }

        [Test]
        public void TestIsGameFinishedNegativeNobodyFinished()
        {
            GameTable gameTable = MakeInitialGameTable;
            Assert.AreEqual(false, _gameServices.IsGameFinished(gameTable));
        }

        #endregion
    }
}