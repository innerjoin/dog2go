using dog2go.Backend.Model;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class PlayerFieldAreaTest
    {
        private const int Identifier = 1;
        private const int FieldId = 0;
        private const ColorCode Color = ColorCode.Green;
        private PlayerFieldArea _pf;

        [SetUp]
        public void CreateAreaForTest()
        {
            _pf = new PlayerFieldArea(Identifier, Color, FieldId);
        }

        [Test]
        public void TestPlayerFieldArea_Generation()
        {
            // test area identification
            Assert.AreEqual(Identifier, _pf.Identifier);
            // test color set
            Assert.AreEqual(Color, _pf.ColorCode);
            // test the count of all fields (except kennel)
            Assert.AreEqual(20, _pf.Fields.Count);
        }

        [Test]
        public void TestPlayerFieldArea_Kennels()
        {
            // now test the kennel fields identifier
            Assert.AreEqual(4, _pf.KennelFields.Count);
            Assert.AreEqual(1, _pf.KennelFields[0].Identifier);
            Assert.AreEqual(2, _pf.KennelFields[1].Identifier);
            Assert.AreEqual(3, _pf.KennelFields[2].Identifier);
            Assert.AreEqual(4, _pf.KennelFields[3].Identifier);
        }

        [Test]
        public void TestPlayerFieldArea_StandardFieldsBefore()
        {
            Assert.AreEqual(5, _pf.Fields[0].Identifier);
            Assert.AreEqual(6, _pf.Fields[1].Identifier);
            Assert.AreEqual(7, _pf.Fields[2].Identifier);
            Assert.AreEqual(8, _pf.Fields[3].Identifier);
        }

        [Test]
        public void TestPlayerFieldArea_StartField()
        {
            Assert.AreEqual(9, _pf.Fields[4].Identifier);
            Assert.IsInstanceOf<StartField>(_pf.Fields[4]);
        }

        [Test]
        public void TestPlayerFieldArea_StandardFieldsAfter()
        {
            Assert.AreEqual(10, _pf.Fields[5].Identifier);
            Assert.AreEqual(11, _pf.Fields[6].Identifier);
            Assert.AreEqual(12, _pf.Fields[7].Identifier);
            Assert.AreEqual(13, _pf.Fields[8].Identifier);
            Assert.AreEqual(14, _pf.Fields[9].Identifier);
            Assert.AreEqual(15, _pf.Fields[10].Identifier);
            Assert.AreEqual(16, _pf.Fields[11].Identifier);
            Assert.AreEqual(17, _pf.Fields[12].Identifier);
            Assert.AreEqual(18, _pf.Fields[13].Identifier);
            Assert.AreEqual(19, _pf.Fields[14].Identifier);
            Assert.AreEqual(20, _pf.Fields[15].Identifier);
        }

        [Test]
        public void TestPlayerFieldArea_EndFields()
        {
            Assert.AreEqual(21, _pf.Fields[16].Identifier);
            Assert.AreEqual(22, _pf.Fields[17].Identifier);
            Assert.AreEqual(23, _pf.Fields[18].Identifier);
            Assert.AreEqual(24, _pf.Fields[19].Identifier);
            Assert.IsInstanceOf<EndField>(_pf.Fields[16]);
            Assert.IsInstanceOf<EndField>(_pf.Fields[17]);
            Assert.IsInstanceOf<EndField>(_pf.Fields[18]);
            Assert.IsInstanceOf<EndField>(_pf.Fields[19]);
        }

        [Test]
        public void TestPlayerFieldArea_Next()
        {
            // standard fields before start
            Assert.AreEqual(_pf.Fields[1], _pf.Fields[0].Next);
            Assert.AreEqual(_pf.Fields[2], _pf.Fields[1].Next);
            Assert.AreEqual(_pf.Fields[3], _pf.Fields[2].Next);
            // next is startfield, both test the same jump
            Assert.AreEqual(_pf.Fields[4], _pf.Fields[3].Next);
            Assert.AreEqual(_pf.StartField, _pf.Fields[3].Next);
            // next is after start field, both test teh same jump
            Assert.AreEqual(_pf.Fields[5], _pf.Fields[4].Next);
            Assert.AreEqual(_pf.Fields[5], _pf.StartField.Next);
            // other standard fields after start
            Assert.AreEqual(_pf.Fields[6], _pf.Fields[5].Next);
            Assert.AreEqual(_pf.Fields[7], _pf.Fields[6].Next);
            Assert.AreEqual(_pf.Fields[8], _pf.Fields[7].Next);
            Assert.AreEqual(_pf.Fields[9], _pf.Fields[8].Next);
            Assert.AreEqual(_pf.Fields[10], _pf.Fields[9].Next);
            Assert.AreEqual(_pf.Fields[11], _pf.Fields[10].Next);
            Assert.AreEqual(_pf.Fields[12], _pf.Fields[11].Next);
            Assert.AreEqual(_pf.Fields[13], _pf.Fields[12].Next);
            Assert.AreEqual(_pf.Fields[14], _pf.Fields[13].Next);
            Assert.AreEqual(_pf.Fields[15], _pf.Fields[14].Next);
            Assert.AreEqual(null, _pf.Fields[15].Next);
        }

        [Test]
        public void TestPlayerFieldArea_NextByIdentifier()
        {
            // standard fields before start
            Assert.AreEqual(6, _pf.Fields[0].Next.Identifier);
            Assert.AreEqual(7, _pf.Fields[1].Next.Identifier);
            Assert.AreEqual(8, _pf.Fields[2].Next.Identifier);
            // next is startfield, both test the same jump
            Assert.AreEqual(9, _pf.Fields[3].Next.Identifier);
            Assert.AreEqual(9, _pf.Fields[3].Next.Identifier);
            // next is after start field, both test teh same jump
            Assert.AreEqual(10, _pf.Fields[4].Next.Identifier);
            Assert.AreEqual(10, _pf.StartField.Next.Identifier);
            // other standard fields after start
            Assert.AreEqual(11, _pf.Fields[5].Next.Identifier);
            Assert.AreEqual(12, _pf.Fields[6].Next.Identifier);
            Assert.AreEqual(13, _pf.Fields[7].Next.Identifier);
            Assert.AreEqual(14, _pf.Fields[8].Next.Identifier);
            Assert.AreEqual(15, _pf.Fields[9].Next.Identifier);
            Assert.AreEqual(16, _pf.Fields[10].Next.Identifier);
            Assert.AreEqual(17, _pf.Fields[11].Next.Identifier);
            Assert.AreEqual(18, _pf.Fields[12].Next.Identifier);
            Assert.AreEqual(19, _pf.Fields[13].Next.Identifier);
            Assert.AreEqual(20, _pf.Fields[14].Next.Identifier);
            Assert.AreEqual(null, _pf.Fields[15].Next);
        }

        [Test]
        public void TestPlayerFieldArea_NextEndField()
        {
            Assert.AreEqual(_pf.EndFields[0], _pf.StartField.EndFieldEntry);
            Assert.AreEqual(_pf.EndFields[1], _pf.EndFields[0].Next);
            Assert.AreEqual(_pf.EndFields[2], _pf.EndFields[1].Next);
            Assert.AreEqual(_pf.EndFields[3], _pf.EndFields[2].Next);
            Assert.AreEqual(null, _pf.EndFields[3].Next);
            Assert.AreEqual(21, _pf.StartField.EndFieldEntry.Identifier);
            Assert.AreEqual(22, _pf.EndFields[0].Next.Identifier);
            Assert.AreEqual(23, _pf.EndFields[1].Next.Identifier);
            Assert.AreEqual(24, _pf.EndFields[2].Next.Identifier);
        }

        [Test]
        public void TestPlayerFieldArea_Previous()
        {
            // standard fields before start
            Assert.AreEqual(null, _pf.Fields[0].Previous);
            Assert.AreEqual(_pf.Fields[0], _pf.Fields[1].Previous);
            Assert.AreEqual(_pf.Fields[1], _pf.Fields[2].Previous);
            Assert.AreEqual(_pf.Fields[2], _pf.Fields[3].Previous);

            Assert.AreEqual(_pf.Fields[3], _pf.Fields[4].Previous);
            Assert.AreEqual(_pf.Fields[3], _pf.StartField.Previous);

            Assert.AreEqual(_pf.Fields[4], _pf.Fields[5].Previous);
            Assert.AreEqual(_pf.StartField, _pf.Fields[5].Previous);

            Assert.AreEqual(_pf.Fields[5], _pf.Fields[6].Previous);
            Assert.AreEqual(_pf.Fields[6], _pf.Fields[7].Previous);
            Assert.AreEqual(_pf.Fields[7], _pf.Fields[8].Previous);
            Assert.AreEqual(_pf.Fields[8], _pf.Fields[9].Previous);
            Assert.AreEqual(_pf.Fields[9], _pf.Fields[10].Previous);
            Assert.AreEqual(_pf.Fields[10], _pf.Fields[11].Previous);
            Assert.AreEqual(_pf.Fields[11], _pf.Fields[12].Previous);
            Assert.AreEqual(_pf.Fields[12], _pf.Fields[13].Previous);
            Assert.AreEqual(_pf.Fields[13], _pf.Fields[14].Previous);
            Assert.AreEqual(_pf.Fields[14], _pf.Fields[15].Previous);
        }

        [Test]
        public void TestPlayerFieldArea_PreviousEndField()
        {
            Assert.AreEqual(_pf.StartField, _pf.EndFields[0].Previous);
            Assert.AreEqual(_pf.EndFields[0], _pf.EndFields[1].Previous);
            Assert.AreEqual(_pf.EndFields[1], _pf.EndFields[2].Previous);
            Assert.AreEqual(_pf.EndFields[2], _pf.EndFields[3].Previous);
            Assert.AreEqual(9, _pf.EndFields[0].Previous.Identifier);
            Assert.AreEqual(21, _pf.EndFields[1].Previous.Identifier);
            Assert.AreEqual(22, _pf.EndFields[2].Previous.Identifier);
            Assert.AreEqual(23, _pf.EndFields[3].Previous.Identifier);
        }
    }
}