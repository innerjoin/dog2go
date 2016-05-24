using System.Collections.Generic;
using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class NunitValidationTest
    {
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

        private readonly Card _card11 = new Card("card11", 11,"testCardWithoutPic",
            new List<CardAttribute>()
            {
                CardAttributeElevenFields,
                CardAttributeOneField,
                CardAttributeLeaveKennel
            });

        private readonly Card _card4 = new Card("card4", 4, "testCardWithoutPic",
            new List<CardAttribute>()
            {
                CardAttributeFourFieldsBack,
                CardAttributeFourFields
            });

        private readonly Card _cardChange = new Card("cardChange", 14, "testCardWithoutPic",
            new List<CardAttribute>()
            {
                CardAttributeChangePlace,
            });

        private readonly Card _card13 = new Card("card13", 13, "testCardWithoutPic", 
            new List<CardAttribute>()
            {
                CardAttributeThirteenFields,
                CardAttributeLeaveKennel
            });

        private readonly Card _cardJoker = new Card("cardJoker", 0, "testCardWithoutPic",
            new List<CardAttribute>()
            {
               CardAttributeOneField,
               CardAttributeTwoFields,
               CardAttributeThreeFields,
               CardAttributeFourFields,
               CardAttributeFourFieldsBack,
               CardAttributeFiveFields,
               CardAttributeSixFields,
               CardAttributeSevenFields,
               CardAttributeEightFields,
               CardAttributeNineFields,
               CardAttributeTenFields,
               CardAttributeElevenFields,
               CardAttributeTwelveFields,
               CardAttributeThirteenFields,
               CardAttributeChangePlace,
               CardAttributeLeaveKennel
            });

        private static readonly CardAttribute CardAttributeOneField = new CardAttribute(CardFeature.OneField);
        private static readonly CardAttribute CardAttributeTwoFields = new CardAttribute(CardFeature.TwoFields);
        private static readonly CardAttribute CardAttributeThreeFields = new CardAttribute(CardFeature.ThreeFields);
        private static readonly CardAttribute CardAttributeFourFields = new CardAttribute(CardFeature.FourFields);
        private static readonly CardAttribute CardAttributeFourFieldsBack = new CardAttribute(CardFeature.FourFieldsBack);
        private static readonly CardAttribute CardAttributeFiveFields = new CardAttribute(CardFeature.FiveFields);
        private static readonly CardAttribute CardAttributeSixFields = new CardAttribute(CardFeature.SixFields);
        private static readonly CardAttribute CardAttributeSevenFields = new CardAttribute(CardFeature.SevenFields);
        private static readonly CardAttribute CardAttributeEightFields = new CardAttribute(CardFeature.EightFields);
        private static readonly CardAttribute CardAttributeNineFields = new CardAttribute(CardFeature.NineFields);
        private static readonly CardAttribute CardAttributeTenFields = new CardAttribute(CardFeature.TenFields);
        private static readonly CardAttribute CardAttributeElevenFields = new CardAttribute(CardFeature.ElevenFields);
        private static readonly CardAttribute CardAttributeTwelveFields = new CardAttribute(CardFeature.TwelveFields);
        private static readonly CardAttribute CardAttributeThirteenFields = new CardAttribute(CardFeature.ThirteenFields);
        private static readonly CardAttribute CardAttributeLeaveKennel = new CardAttribute(CardFeature.LeaveKennel);
        private static readonly CardAttribute CardAttributeChangePlace = new CardAttribute(CardFeature.ChangePlace);

        #region "Testmethods for ValidateMove-Method"​

        [Test]
        public void TestLeaveKennel()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            KennelField kennelField = greenArea.KennelFields[0] as KennelField;
            StartField endField = greenArea.Fields.Find(field => field.FieldType.Contains("StartField")) as StartField;
            Meeple meeple = greenArea.Meeples[0];
            meeple.CurrentPosition = kennelField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card11,
                SelectedAttribute = CardAttributeLeaveKennel
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestMovedInSameFieldAreaPositive()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple = greenArea.Meeples[0];
            meeple.CurrentPosition = startField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card4,
                SelectedAttribute = CardAttributeFourFields
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove)); 
        }

        [Test]
        public void TestMovedInSameFieldAreaNegative()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple = greenArea.Meeples[0];
            meeple.CurrentPosition = startField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card4,
                SelectedAttribute = CardAttributeFourFieldsBack
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestMovedInOtherFieldAreaPositive()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea startArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            PlayerFieldArea targeArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Yellow);
            StandardField startField = startArea.Fields[8] as StandardField;
            StartField endField = targeArea.Fields[4] as StartField;
            Meeple meeple = startArea.Meeples[0];
            meeple.CurrentPosition = startField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card13,
                SelectedAttribute = CardAttributeThirteenFields
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestLeaveKennelWrongField()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple = greenArea.Meeples[0];
            meeple.CurrentPosition = startField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card13,
                SelectedAttribute = CardAttributeLeaveKennel
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestLeaveKennelNotAllowed()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple = greenArea.Meeples[0];
            meeple.CurrentPosition = startField;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _card13,
                SelectedAttribute = CardAttributeLeaveKennel
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestChangeMeepleAllowed()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            PlayerFieldArea blueArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple1 = greenArea.Meeples[0];
            Meeple meeple2 = blueArea.Meeples[0];
            startField.CurrentMeeple = meeple1;
            endField.CurrentMeeple = meeple2;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple1,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _cardJoker,
                SelectedAttribute = CardAttributeChangePlace
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        #endregion

        #region "Testmethods for ProveChangePlace"

        [Test]
        public void TestProveChangePlaceStandardFieldSameColorMeeple()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new StandardField(2);
            StandardField field2 = new StandardField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceEndFieldSameColorMeeple()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new EndField(2);
            EndField field2 = new EndField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceEndField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Green);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new EndField(2);
            EndField field2 = new EndField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceKennelField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Green);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new KennelField(2);
            KennelField field2 = new KennelField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceStartfFieldBlocked()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Green) {IsStartFieldBlocked = true};
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            StartField field = new StartField(1, ColorCode.Green) {CurrentMeeple =  meeple};
            StartField field2 = new StartField(2,ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceStartfFieldBlockedMoveField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Green) { IsStartFieldBlocked = false };
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            meeple.CurrentPosition = new StartField(1, ColorCode.Green);
            StartField field2 = new StartField(1, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveChangePlace(meeple, field2));
        }
        [Test]
        public void TestProveChangePlaceStartfFieldNotBlocked()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Green) { IsStartFieldBlocked = false }; 
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            meeple.CurrentPosition = new StartField(1, ColorCode.Blue);
            StartField field2 = new StartField(2, ColorCode.Green) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceStandardField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green);
            meeple.CurrentPosition = new StandardField(2);
            StandardField field2 = new StandardField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceStandardFieldAndStartField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green) {IsStartFieldBlocked = false};
            meeple.CurrentPosition = new StandardField(2);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestProveChangePlaceStartFieldAndStandardField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue) { IsStartFieldBlocked = false };
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green) { IsStartFieldBlocked = false };
            meeple.CurrentPosition = new StandardField(2);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveChangePlace(meeple, field2));
        }

        [Test]
        public void TestChangeMeeplePositive()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            PlayerFieldArea blueArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple1 = greenArea.Meeples[0];
            Meeple meeple2 = blueArea.Meeples[0];
            startField.CurrentMeeple = meeple1;
            endField.CurrentMeeple = meeple2;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple1,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _cardJoker,
                SelectedAttribute = CardAttributeChangePlace
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestChangeMeepleNegativeSameColor()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StandardField endField = greenArea.Fields[14] as StandardField;
            Meeple meeple1 = greenArea.Meeples[0];
            Meeple meeple2 = greenArea.Meeples[1];
            startField.CurrentMeeple = meeple1;
            endField.CurrentMeeple = meeple2;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple1,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _cardChange,
                SelectedAttribute = CardAttributeChangePlace
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestChangeMeepleNegativeStartField()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable(TableId);
            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            PlayerFieldArea blueArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            StandardField startField = greenArea.Fields[10] as StandardField;
            StartField endField = greenArea.Fields.Find(field => field.FieldType.Contains("StartField")) as StartField;
            Meeple meeple1 = greenArea.Meeples[0];
            Meeple meeple2 = blueArea.Meeples[0];
            meeple2.IsStartFieldBlocked = true;
            startField.CurrentMeeple = meeple1;
            endField.CurrentMeeple = meeple2;
            MeepleMove meepleMove = new MeepleMove()
            {
                Meeple = meeple1,
                MoveDestination = endField
            };
            CardMove cardMove = new CardMove()
            {
                Card = _cardChange,
                SelectedAttribute = CardAttributeChangePlace
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }
        #endregion

        #region "Testmethods for ProveLeaveKennel"

        [Test]
        public void TestProveLeaveKennelNoKennelField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new StartField(2, ColorCode.Blue);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelNoStartField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue);
            meeple.CurrentPosition = new KennelField(2);
            KennelField field2 = new KennelField(3) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelSameColor()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green);
             meeple2.CurrentPosition = new KennelField(2);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelNotSameColor()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Red);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green);
            meeple.CurrentPosition = new KennelField(2);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(false, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelEmptyDestinationField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green);
            meeple.CurrentPosition = new KennelField(2);
            StartField field2 = new StartField(3, ColorCode.Blue);
            Assert.AreEqual(true, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelBlockedStartField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Red);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green) {IsStartFieldBlocked = true};
            meeple.CurrentPosition = new KennelField(2);
            StartField field2 = new StartField(3, ColorCode.Red) {CurrentMeeple = meeple2};
            Assert.AreEqual(false, Validation.ProveLeaveKennel(meeple, field2));
        }

        [Test]
        public void TestProveLeaveKennelFilledDestinationField()
        {
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Green);
            meeple.CurrentPosition = new KennelField(2);
            StartField field2 = new StartField(3, ColorCode.Blue) { CurrentMeeple = meeple2 };
            Assert.AreEqual(true, Validation.ProveLeaveKennel(meeple, field2));
        }

        #endregion

        #region "Testmethods for ProveValueCard"

        [Test]
        public void TestProveValueCardStartToStandardField()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField) table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.ProveValueCard(meeple2, standardField, 2));
        }

        [Test]
        public void TestProveValueCardStandardFieldOverStartBlocked()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) {IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField = (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StandardField"));
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.ProveValueCard(meeple, (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7], 3));
        }

        [Test]
        public void TestProveValueCardStandardFieldOverStartNotBlocked()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField = (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StandardField"));
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.ProveValueCard(meeple, (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[4], 4));
        }

        [Test]
        public void TestProveValueCardCanMoveToEndField()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField) table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[2];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.ProveValueCard(meeple, (EndField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("EndField")), 3));
        }

        [Test]
        public void TestProveValueCardCanNotMoveToEndField()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[2];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.ProveValueCard(meeple, (EndField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("EndField")), 4));
        }

        #endregion

        #region "CanMoveToEndFields"

        [Test]
        public void TestCanMoveToEndFields()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[3];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.CanMoveToEndFields(standardField, 3, meeple.ColorCode));
        }

        [Test]
        public void TestCanMoveToEndFieldsToManyFields()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = false };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[3];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.CanMoveToEndFields(standardField, 7, meeple.ColorCode));
        }

        [Test]
        public void TestCanMoveToEndFieldsBug340()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(77), ColorCode.Yellow) { IsStartFieldBlocked = false };
            StandardField standardField = (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Yellow).Fields[0];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.CanMoveToEndFields(standardField, 5, meeple.ColorCode));
        }

        [Test]
        public void TestCanNotMoveToEndFields()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[7];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.CanMoveToEndFields(standardField, 8, meeple.ColorCode));
        }
        #endregion

        #region "HasBlockedField"

        [Test]
        public void TestHasBlockedFieldForwards()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[3];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.HasBlockedField(standardField, 4));
        }

        [Test]
        public void TestHasBlockedFieldForwardsFromStart()
        {
            GameTable table = MakeInitialGameTable;
            //Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            Assert.AreEqual(false, Validation.HasBlockedField(startField, 4));
        }

        [Test]
        public void TestHasBlockedFieldBackwards()
        {
            GameTable table = MakeInitialGameTable;
            Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            StandardField standardField =
                (StandardField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields[5];
            standardField.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.HasBlockedField(standardField, -4));
        }

        [Test]
        public void TestHasBlockedFieldBackwardsFromStart()
        {
            GameTable table = MakeInitialGameTable;
            //Meeple meeple = new Meeple(new KennelField(1), ColorCode.Blue);
            Meeple meeple2 = new Meeple(new KennelField(2), ColorCode.Blue) { IsStartFieldBlocked = true };
            StartField startField = (StartField)table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue).Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = meeple2;
            Assert.AreEqual(false, Validation.HasBlockedField(startField, -4));
        }
        #endregion

        [Test]
        public void TestIsLastFreeEndField()
        {
            Meeple meeple = new Meeple(new KennelField(0));

            EndField ef1 = new EndField(1);
            EndField ef2 = new EndField(2);
            EndField ef3 = new EndField(3);
            EndField ef4 = new EndField(4);
            ef1.Next = ef2;
            ef2.Previous = ef1;
            ef2.Next = ef3;
            ef3.Previous = ef2;
            ef3.Next = ef4;
            ef4.Previous = ef3;
            
            // test for first meeple on end fields
            Assert.AreEqual(false, Validation.IsLastFreeEndField(ef1));
            Assert.AreEqual(false, Validation.IsLastFreeEndField(ef2));
            Assert.AreEqual(false, Validation.IsLastFreeEndField(ef3));
            Assert.AreEqual(true, Validation.IsLastFreeEndField(ef4));

            // ef2 is blocked, but 3 and 4 are still free, so we are not finished yet
            ef2.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.IsLastFreeEndField(ef1));

            // ef3 is blocked, but 4 is still free, so we are not finished yet
            ef3.CurrentMeeple = meeple;
            Assert.AreEqual(false, Validation.IsLastFreeEndField(ef1));

            // ef2 up to ef4 are all blocked, so we are finished
            ef4.CurrentMeeple = meeple;
            Assert.AreEqual(true, Validation.IsLastFreeEndField(ef1));

            // null is a problem
            Assert.AreEqual(false, Validation.IsLastFreeEndField(null));
        }
    }
}