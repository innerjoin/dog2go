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

        private static readonly CardAttribute CardAttributeOneField = new CardAttribute(AttributeEnum.OneField);
        private static readonly CardAttribute CardAttributeTwoFields = new CardAttribute(AttributeEnum.TwoFields);
        private static readonly CardAttribute CardAttributeThreeFields = new CardAttribute(AttributeEnum.ThreeFields);
        private static readonly CardAttribute CardAttributeFourFields = new CardAttribute(AttributeEnum.FourFields);
        private static readonly CardAttribute CardAttributeFiveFields = new CardAttribute(AttributeEnum.FiveFields);
        private static readonly CardAttribute CardAttributeFourFieldsBack = new CardAttribute(AttributeEnum.FourFieldsBack);
        private static readonly CardAttribute CardAttributeSixFields = new CardAttribute(AttributeEnum.SixFields);
        private static readonly CardAttribute CardAttributeSevenFields = new CardAttribute(AttributeEnum.SevenFields);
        private static readonly CardAttribute CardAttributeEightFields = new CardAttribute(AttributeEnum.EightFields);
        private static readonly CardAttribute CardAttributeNineFields = new CardAttribute(AttributeEnum.NineFields);
        private static readonly CardAttribute CardAttributeTenFields = new CardAttribute(AttributeEnum.TenFields);
        private static readonly CardAttribute CardAttributeElevenFields = new CardAttribute(AttributeEnum.ElevenFields);
        private static readonly CardAttribute CardAttributeTwelveFields = new CardAttribute(AttributeEnum.TwelveFields);
        private static readonly CardAttribute CardAttributeThirteenFields = new CardAttribute(AttributeEnum.ThirteenFields);
        private static readonly CardAttribute CardAttributeLeaveKennel = new CardAttribute(AttributeEnum.LeaveKennel);
        private static readonly CardAttribute CardAttributeChangePlace = new CardAttribute(AttributeEnum.ChangePlace);

        #region "Testmethods for ValidateMove-Method"​

        [Test]
        public void TestLeaveKennel()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestMovedInOtherFieldAreaPositive()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable();
            PlayerFieldArea startArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            PlayerFieldArea targeArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Yellow);
            StandardField startField = startArea.Fields[8] as StandardField;
            StandardField endField = targeArea.Fields[5] as StandardField;
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
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
                Card = new Card("cardChangePlace", 14, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.ChangePlace)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.ChangePlace)
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void TestChangeMeepleNegativeStartField()
        {
            GameTable gameTable = _hub.GetGeneratedGameTable();
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
                Card = new Card("cardChangePlace", 14, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.ChangePlace)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.ChangePlace)
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        #endregion
    }
}