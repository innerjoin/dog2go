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
        #region "Testmethods for ValidateMove-Method"​

        [Test]
        public void dog_testLeaveKennel()
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
                Card =
                    new Card("card11", 11,"testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.ElevenFields),
                            new CardAttribute(AttributeEnum.OneField),
                            new CardAttribute(AttributeEnum.LeaveKennel)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.LeaveKennel)
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void dog_testMovedInSameFieldAreaPositive()
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
                Card =
                    new Card("card4", 4, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.FourFieldsBack),
                            new CardAttribute(AttributeEnum.FourFields)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.FourFields)
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove)); 
        }

        [Test]
        public void dog_testMovedInSameFieldAreaNegative()
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
                Card =
                    new Card("card4", 4, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.FourFieldsBack),
                            new CardAttribute(AttributeEnum.FourFields)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.FourFieldsBack)
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void dog_testMovedInOtherFieldAreaPositive()
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
                Card = new Card("card13", 13, "testCardWithoutPic", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThirteenFields), new CardAttribute(AttributeEnum.LeaveKennel) }),
                SelectedAttribute = new CardAttribute(AttributeEnum.ThirteenFields)
            };
            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void dog_testLeaveKennelWrongField()
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
                Card =
                    new Card("card13", 13, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.ThirteenFields),
                            new CardAttribute(AttributeEnum.LeaveKennel)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.LeaveKennel)
            };
            Assert.AreEqual(false, Validation.ValidateMove(meepleMove, cardMove));
        }

        [Test]
        public void dog_testLeaveKennelNotAllowed()
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
                Card =
                    new Card("card13", 13, "testCardWithoutPic",
                        new List<CardAttribute>()
                        {
                            new CardAttribute(AttributeEnum.ThirteenFields),
                            new CardAttribute(AttributeEnum.LeaveKennel)
                        }),
                SelectedAttribute = new CardAttribute(AttributeEnum.LeaveKennel)
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