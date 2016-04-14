using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using NUnit.Framework;

namespace dog2go.Tests.Backend
{
    [TestFixture]
    public class NunitHubConnectionTest
    {

        private readonly GameHub _gameHub = new GameHub(UserRepository.Instance);
        private readonly ChatHub _chatHub = new ChatHub(ChatMessageRepository.Instance);

        #region "Testmethods for Communication between hubs"

        [Test]
        public void dog_testCanCommunicate()
        {
            _gameHub.GameUserRepository.Add(new User("test_user1"));
            _gameHub.GameUserRepository.Add(new User("test_user2"));
            _gameHub.GameUserRepository.Add(new User("test_user3"));

            Assert.AreEqual(UserRepository.Instance.Get().Count, 3);
        }

        #endregion
    }
}