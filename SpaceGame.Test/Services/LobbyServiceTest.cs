using SpaceGame.Controllers.Api;
using SpaceGame.Data.Models;
using SpaceGame.Data.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpaceGame.Test.Services
{
    public class LobbyServiceTest : IClassFixture<TestBase>
    {
        private TestBase _fixture;
        private LobbyService _service;

        public LobbyServiceTest(TestBase fixture)
        {
            _fixture = fixture;
            _service = new LobbyService(_fixture.TestContext);
        }

        #region GetGroupById
        [Fact]
        public void TestGetGroup()
        {
            Group testGroup = _service.GetGroupById(TestData.group2.Id);
            Assert.Equal(testGroup, TestData.group2);
        }

        [Fact]
        public void TestNullGroup()
        {
            Group testGroup = _service.GetGroupById(-1);
            Assert.Null(testGroup);
        }
        #endregion GetGroupById

        #region GetActiveGroups
        [Fact]
        public void TestActiveGroups()
        {
            IEnumerable<Group> knownActiveGroups = _fixture.TestContext.Groups.Where(g => g.IsActive);
            IEnumerable<Group> testGroups = _service.GetActiveGroups();
            Assert.Equal(knownActiveGroups, testGroups);
        }
        #endregion GetActiveGroups

        #region StartGroup
        [Fact]
        public void TestStartGroup()
        {
            StartGroupRequest request = new StartGroupRequest() { HostPlayerName = "NewPlayer", GroupName = "NewGroup" };
            Group testGroup = _service.StartGroup(TestData.user1, request);
            Assert.Equal(testGroup.Name, request.GroupName);
            Assert.True(testGroup.IsActive);
            Assert.False(testGroup.IsInGame);
            Assert.Equal(testGroup.HostPlayer.Name, request.HostPlayerName);
            Assert.Equal(testGroup.HostPlayer.User, TestData.user1);
        }
        #endregion StartGroup

        #region DeactivateGroup
        [Fact]
        public void TestDeactivateGroup()
        {
            TestData.group2.IsActive = true;
            _fixture.TestContext.Update(TestData.group2);
            _fixture.TestContext.SaveChanges();

            _service.DeactivateGroup(TestData.group2);
            Assert.False(TestData.group2.IsActive);
        }
        #endregion DeactivateGroup

        #region AddPlayerToGroup
        [Fact]
        public void TestAddPlayerToGroupExisting()
        {
            AddPlayerRequest request = new AddPlayerRequest() { PlayerName = "NewAddedPlayer1" };

            List<Player> players = _service.AddPlayerToGroup(TestData.user2, TestData.group1, request);
            Assert.Equal(players, TestData.group1.Clients);
            Player player = TestData.group1.Clients.FirstOrDefault(p => p.Name == request.PlayerName);
            Assert.NotNull(player);
            Assert.Equal(player.User, TestData.user2);
        }

        [Fact]
        public void TestAddPlayerToGroupNullList()
        {
            AddPlayerRequest request = new AddPlayerRequest() { PlayerName = "NewAddedPlayer2" };

            List<Player> players = _service.AddPlayerToGroup(TestData.user2, TestData.group2, request);
            Assert.Equal(players, TestData.group2.Clients);
            Player player = TestData.group2.Clients.FirstOrDefault(p => p.Name == request.PlayerName);
            Assert.NotNull(player);
            Assert.Equal(player.User, TestData.user2);
        }
        #endregion AddPlayerToGroup

        #region RemovePlayerFromGroup
        [Fact]
        public void TestRemovePlayerToGroupExisting()
        {
            Player player = TestData.group1.Clients.First();

            List<Player> players = _service.RemovePlayerFromGroup(TestData.group1, player.Id);
            Assert.Equal(players, TestData.group1.Clients);
            Player testPlayer = TestData.group1.Clients.FirstOrDefault(p => p.Id == player.Id);
            Assert.Null(testPlayer);
        }
        #endregion RemovePlayerFromGroup
    }
}
