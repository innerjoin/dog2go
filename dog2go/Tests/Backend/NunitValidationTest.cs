//using System.Collections.Generic;
//using dog2go.Backend.Hubs;
//using dog2go.Backend.Model;
//using dog2go.Backend.Repos;
//using dog2go.Backend.Services;
//using NUnit.Framework;

//namespace dog2go.Tests.Backend
//{
//    [TestFixture]
//    public class NunitValidationTest
//    {

//        private readonly GameHub _hub = new GameHub(GameRepository.Instance);
//        #region "Testmethods for ValidateMove-Method"
//        [Test]
//        public void dog_testPositiveMovedInSameFieldArea()
//        {
//            GameTable gameTable = _hub.GetGeneratedGameTable();
//            PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
//            StandardField startField = greenArea.Fields[10] as StandardField;
//            StandardField endField = greenArea.Fields[14] as StandardField;
//            Meeple meeple = greenArea.Meeples[0];
//            meeple.CurrentPosition = startField;
//            MeepleMove meepleMove = new MeepleMove() {Meeple = meeple, MoveDestination = endField};
//            CardMove cardMove = new CardMove() {Card = new Card("card4", 4, "testfile", new List<CardAttribute>() { new CardAttribute(AttributeEnum.FourFieldsBack), new CardAttribute(AttributeEnum.FourFields) }), SelectedAttribute = new CardAttribute(AttributeEnum.FourFields) };
//            Assert.AreEqual(true, Validation.ValidateMove(meepleMove, cardMove)); 
            

//            //GameTable table = _hub.GetGeneratedGameTable();
//            //PlayerFieldArea blueArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
//            //PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);
//            //StandardField standardField = (StandardField)blueArea.Fields.Find(field => field.Identifier == 12);
//            //standardField.CurrentMeeple = blueArea.Meeples.Find(meeple =>
//            //{
//            //    meeple.IsStartFieldBlocked = false;
//            //    return (meeple != null);
//            //});

//            //StartField startField = (StartField)redArea.Fields.Find(field => field.FieldType.Contains("StartField"));

//            //startField.CurrentMeeple = redArea.Meeples.Find(meeple =>
//            //{
//            //    meeple.IsStartFieldBlocked = true;
//            //    return (meeple != null);
//            //});

//            //Assert.That(_hub.HasBlockedField(standardField, 10), Is.EqualTo(true));
//        }

        
//        #endregion
//    }
//}