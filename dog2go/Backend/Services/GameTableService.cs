using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Constants;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;

namespace dog2go.Backend.Services
{
    public class GameTableService
    {
        public static ColorCode GetColorCodeForUser(string userName, object locker, IGameRepository games)
        {
            GameTable actualGameTable = GetActualGameTable(locker, games, userName);
            return
                actualGameTable.PlayerFieldAreas.Find(area => area.Participation.Participant.Nickname == userName)
                    .ColorCode;
        }

        public static GameTable GetActualGameTable(object locker, IGameRepository games, string userName)
        {
            lock (locker)
            {
                return games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == userName) != null);
            }
        }

        public static GameTable GetTable(IGameRepository games)
        {
            int gameId = GlobalDefinitions.GameId;
            if (games.Get().Count == 0)
                gameId = GameFactory.CreateGameTable(games, GlobalDefinitions.GroupName);
            return games.Get().Find(x => x.Identifier == gameId);
        }

        public static bool AlreadyConnected(GameTable table, string curUser)
        {
            return table?.Participations != null &&
                   (table.Participations).Any(part =>
                       curUser.Equals(part.Participant.Nickname));
        }

        public static User GetActualUser(string userName)
        {
            return UserRepository.Instance.Get()
                .FirstOrDefault(user => user.Value.Nickname == userName)
                .Value;
        }
    }

}