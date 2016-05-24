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
    public class GameServicesTest
    {
        private readonly GameHub _hub = new GameHub(GameRepository.Instance);
        private readonly GameServices _gameServices = new GameServices();
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
        private static bool ArePartners(PlayerFieldArea currentPlayerFieldArea, PlayerFieldArea partnerPlayerFieldArea)
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

            Assert.AreEqual(true, GameServices.IsGameFinished(gameTable));
        }


        [Test]
        public void TestIsGameFinishedPositiveTeamFinished()
        {
            GameTable gameTable = MakeInitialGameTable;
            foreach (PlayerFieldArea currentPlayerFieldArea in gameTable.PlayerFieldAreas)
            {
                foreach (PlayerFieldArea partnerPlayerFieldArea in gameTable.PlayerFieldAreas)
                {
                    if (ArePartners(currentPlayerFieldArea, partnerPlayerFieldArea))
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
           Assert.AreEqual(true, GameServices.IsGameFinished(gameTable));
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
                    if (ArePartners(currentPlayerFieldArea, partnerPlayerFieldArea) && !partnerFounded)
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
            Assert.AreEqual(false, GameServices.IsGameFinished(gameTable));
        }

        [Test]
        public void TestIsGameFinishedNegativeNobodyFinished()
        {
            GameTable gameTable = MakeInitialGameTable;
            Assert.AreEqual(false, GameServices.IsGameFinished(gameTable));
        }



        #endregion

        #region "Testmethods for GetPartner"

        [Test]
        public void TestGetPartnerCorrectlyInitialized()
        {
            GameTable table = MakeInitialGameTable;
            User user1 = table.Participations.Find(participation => participation.Participant.Nickname == "user1").Participant;
            User user3 = table.Participations.Find(participation => participation.Participant.Nickname == "user3").Participant;

            User partner1 = ParticipationService.GetPartner(
                table.Participations.Find(participation => participation.Participant.Nickname == "user1").Participant,
                table.Participations);

            User partner3 = ParticipationService.GetPartner(
                table.Participations.Find(participation => participation.Participant.Nickname == "user3").Participant,
                table.Participations);

            if(user1.Identifier == partner3.Identifier && user3.Identifier == partner1.Identifier)
                Assert.AreEqual(true, true);
            else
                Assert.AreEqual(true, false);
        }

        [Test]
        public void TestGetPartnerNotInitializedParticipation()
        {
            User user = new User("test_user", "4");

            User partner = ParticipationService.GetPartner(user,
                null);
            Assert.AreEqual(null, partner);
        }

        [Test]
        public void TestGetPartnerNotInitializedUser()
        {

            User partner = ParticipationService.GetPartner(null,
                null);
            Assert.AreEqual(null, partner);
        }
        #endregion

        #region "Testmethods for GetNextPlayer"

        [Test]
        public void TestGetNextPlayerCorrectlyInitialized()
        {
            GameTable table = MakeInitialGameTable;
            string user2 = ParticipationService.GetNextPlayer(table, "user1");
            Assert.AreEqual(true, user2.Equals("user2"));
            string user3 = ParticipationService.GetNextPlayer(table, "user2");
            Assert.AreEqual(true, user3.Equals("user3"));
            string user4 = ParticipationService.GetNextPlayer(table, "user3");
            Assert.AreEqual(true, user4.Equals("user4"));
            string user1 = ParticipationService.GetNextPlayer(table, "user4");
            Assert.AreEqual(true, user1.Equals("user1"));
        }

        [Test]
        public void TestGetNextPlayerNonExistentUser()
        {
            GameTable table = MakeInitialGameTable;
            string user = ParticipationService.GetNextPlayer(table, "user5");
            Assert.AreEqual(true, user == null);
        }

        [Test]
        public void TestGetNextPlayerNotInitalizedGameTable()
        {
            GameTable table = MakeInitialGameTable;
            string user = ParticipationService.GetNextPlayer(null, "user1");
            Assert.AreEqual(true, user == null);
        }
        #endregion

        #region "Testmethods for UpdateMeeplePosition"

        [Test]
        public void TestUpdateMeeplePositionCorrectlyInitialized()
        {
            GameTable table = MakeInitialGameTable;
            PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);
            Meeple redMeeple = redArea.Meeples.First();
            MoveDestinationField currentField = redArea.Fields.Find(field => field.FieldType.Contains("StartField"));
            currentField.CurrentMeeple = redMeeple;
            MoveDestinationField moveField = redArea.Fields.Find(field => field.FieldType.Contains("StandardField"));
            GameTableService.UpdateMeeplePosition(new MeepleMove() {Meeple = redMeeple, MoveDestination = moveField}, table, false);
            Assert.AreEqual(moveField, table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red).Fields.Find(field => field.CurrentMeeple == redMeeple));
        }

        [Test]
        public void TestUpdateMeeplePositionNoMeepleMove()
        {
            GameTable table = MakeInitialGameTable;
            PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);
            Meeple redMeeple = redArea.Meeples.First();
            MoveDestinationField currentField = redArea.Fields.Find(field => field.FieldType.Contains("StartField"));
            currentField.CurrentMeeple = redMeeple;
            MoveDestinationField moveField = redArea.Fields.Find(field => field.FieldType.Contains("StandardField"));
            GameTable notModified = table;
            GameTableService.UpdateMeeplePosition(null, table,false);
            Assert.AreEqual(table, notModified);
        }

        [Test]
        public void TestUpdateMeeplePositionNoTable()
        {
            GameTableService.UpdateMeeplePosition(null,null,false);
            Assert.AreEqual(true, true);
        }
        #endregion

        #region "Testmethods for UpdateActualRoundCards"

        [Test]
        public void TestUpdateActualRoundCardsCorrectlyInitialized()
        {
            GameTable table = MakeInitialGameTable;
            GameTableService.UpdateActualRoundCards(table);
            Assert.AreEqual(4,
                table.Participations.FindAll(participation => participation.ActualPlayRound.Cards.Count() == 6).Count);

            GameTableService.UpdateActualRoundCards(table);
            Assert.AreEqual(4,
                table.Participations.FindAll(participation => participation.ActualPlayRound.Cards.Count() == 5).Count);
        }

        [Test]
        public void TestUpdateActualRoundCardsNonGameTable()
        {
            GameTable table = GameTableService.UpdateActualRoundCards(null);
            Assert.AreEqual(table, null);
        }

        [Test]
        public void TestUpdateActualRoundCardsNonInitializedGameTable()
        {
            GameTable table = GameTableService.UpdateActualRoundCards(new GameTable(new List<PlayerFieldArea>(), 4, ""));
            Assert.AreEqual(table, null);
        }
        #endregion

    }
}