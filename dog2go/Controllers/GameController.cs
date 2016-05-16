using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using dog2go.Backend.Constants;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using dog2go.Controllers.Helpers;
using dog2go.Models;

namespace dog2go.Controllers
{
    public class GameController : Controller
    {
        [FullyAuthorized]
        public ActionResult Play(int id = -1)
        {
            if (id < 0)
                return RedirectToAction($"ChooseGameTable", $"Game");
            // TODO: verify that id exists and pass id to view and from there to signalR
            User user = UserRepository.Instance.Get().FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value;
            DisplayNameModel model = new DisplayNameModel
            {
                TableIdentifier = id,
                DisplayUser = user,
                ColorMeeplePath = GetUserMeeplePath()
            };
            return View(model);
        }

        [HttpGet]
        [FullyAuthorized]
        public ActionResult CreateTable()
        {
            TableViewModel model = new TableViewModel();
            return View(model);
        }

        [HttpPost]
        [FullyAuthorized]
        [ActionName("CreateTable")]
        public ActionResult CreateTable(TableViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            int tableId = GameFactory.CreateGameTable(GameRepository.Instance, model.Name);
            AddParticipantToTable(tableId);
            return RedirectToAction($"Play", $"Game", new {id = tableId});
        }

        [HttpGet]
        [FullyAuthorized]
        public ActionResult ChooseGameTable()
        {
            ChooseTableViewModel model = new ChooseTableViewModel();
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            gameTableList?.ForEach(table =>
            {
                List<string> participants = new List<string>();
                table.Participations?.ForEach(part => { participants.Add(part.Participant.Nickname); });
                model.GameTableList.Add(new TableViewModel(table.Identifier, table.Name, participants));
            });
            return View(model);
        }

        [HttpPost]
        [FullyAuthorized]
        [ActionName("ChooseGameTable")]
        public ActionResult ChooseGameTable(TableViewModel model)
        {
            AddParticipantToTable(model.Identifier);
            return RedirectToAction($"Play", $"Game", new {id = model.Identifier});
        }

        [FullyAuthorized]
        private void AddParticipantToTable(int tableId)
        {
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            GameTable gameTable = gameTableList?.Find(table => table.Identifier.Equals(tableId));
            if (!ParticipationService.IsAlreadyParticipating(gameTable, User.Identity.Name))
            {
                ParticipationService.AddParticipation(gameTable, User.Identity.Name);
            }
        }

        private static string GetUserMeeplePath()
        {
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            switch (users.Count%GlobalDefinitions.NofParticipantsPerTable)
            {
                case 0:
                    return "meeple_yellow.png";
                case 1:
                    return "meeple_blue.png";
                case 2:
                    return "meeple_red.png";
                case 3:
                    return "meeple_green.png";
                default:
                    return "not defined";
            }
        }
    }
}