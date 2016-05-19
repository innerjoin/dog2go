using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
            if (id < 0 || !GameRepository.Instance.Exists(id) || !GameRepository.Instance.IsParticipant(id, User.Identity.Name)) 
                return RedirectToAction($"ChooseGameTable", $"Game");
            // TODO: pass id from view to signalR
            User user = UserRepository.Instance.Get().FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value;
            DisplayNameModel model = new DisplayNameModel
            {
                TableIdentifier = id,
                DisplayUser = user,
                ColorMeeplePath = GetUserMeeplePath(id)
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
        public ActionResult ChooseGameTable(string modelError)
        {
            if (!string.IsNullOrEmpty(modelError))
                ModelState.AddModelError(string.Empty, modelError);
            ChooseTableViewModel model = new ChooseTableViewModel();
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            gameTableList?.ForEach(table =>
            {
                if (table.IsFull()) return;
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
            if (!AddParticipantToTable(model.Identifier))
            {
                return RedirectToAction($"ChooseGameTable", $"Game", new { modelError = $"Could not join table \'" + model.Name + "' as it was already full. Please join another one." });
            }
            return RedirectToAction($"Play", $"Game", new {id = model.Identifier});
        }

        [FullyAuthorized]
        private bool AddParticipantToTable(int tableId)
        {
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            GameTable gameTable = gameTableList?.Find(table => table.Identifier.Equals(tableId));
            if (ParticipationService.IsAlreadyParticipating(gameTable, User.Identity.Name))
                return true;
            return ParticipationService.AddParticipation(gameTable, User.Identity.Name);
        }

        private string GetUserMeeplePath(int tableId)
        {
            GameTable gameTable = GameRepository.Instance.Get()?.Find(table => table.Identifier.Equals(tableId));
            PlayerFieldArea playerFieldArea = gameTable?.PlayerFieldAreas?.FirstOrDefault(x => x.Participation.Participant.Nickname == User.Identity.Name);
            if (playerFieldArea == null) return "not defined";
            switch (playerFieldArea.ColorCode)
            {
                case ColorCode.Yellow:
                    return "meeple_yellow.png";
                case ColorCode.Blue:
                    return "meeple_blue.png";
                case ColorCode.Red:
                    return "meeple_red.png";
                case ColorCode.Green:
                    return "meeple_green.png";
                default:
                    return "not defined";
            }
        }
    }
}