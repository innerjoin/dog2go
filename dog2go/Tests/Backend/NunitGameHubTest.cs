using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class NunitGameHubTest
    {

        private readonly GameHub _hub = new GameHub(GameRepository.Instance);
        #region "Testmethods for HasBlocked-Method"
        [Test]
        public void dog_testHasBlocked()
        {
            
            GameTable table = _hub.GetGeneratedGameTable();
            PlayerFieldArea blueArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);
            StandardField standardField = (StandardField)blueArea.Fields.Find(field => field.Identifier == 12);
            standardField.CurrentMeeple = blueArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = false;
                return (meeple != null);
            });

            StartField startField = (StartField)redArea.Fields.Find(field => field.FieldType.Contains("StartField"));

            startField.CurrentMeeple = redArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = true;
                return (meeple != null);
            });

            Assert.That(_hub.HasBlockedField(standardField, 10), Is.EqualTo(true));
        }

        [Test]
        public void dog_testHasBlockedWrong()
        {
            GameTable table = _hub.GetGeneratedGameTable();
            PlayerFieldArea blueArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);
            StandardField standardField = (StandardField)blueArea.Fields.Find(field => field.Identifier == 16);
            standardField.CurrentMeeple = blueArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = false;
                return (meeple != null);
            });

            StartField startField = (StartField)redArea.Fields.Find(field => field.FieldType.Contains("StartField"));

            startField.CurrentMeeple = redArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = false;
                return (meeple != null);
            });

            Assert.That(_hub.HasBlockedField(standardField, 10), Is.EqualTo(false));
        }

        [Test]
        public void dog_testHasBlockedBackwards()
        {
            GameTable table = _hub.GetGeneratedGameTable();
            PlayerFieldArea blueArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            PlayerFieldArea redArea = table.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);

            StartField startField = (StartField)blueArea.Fields.Find(field => field.FieldType.Contains("StartField"));
            startField.CurrentMeeple = blueArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = false;
                return (meeple != null);
            });

            StandardField standardField = (StandardField)redArea.Fields.Find(field => field.FieldType.Contains("StandardField"));

            startField.CurrentMeeple = redArea.Meeples.Find(meeple =>
            {
                meeple.IsStartFieldBlocked = false;
                return (meeple != null);
            });

            Assert.That(_hub.HasBlockedField(standardField, -4), Is.EqualTo(false));
        }
        #endregion
    }
}