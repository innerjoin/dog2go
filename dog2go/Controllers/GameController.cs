﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using dog2go.Backend.Hubs;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using dog2go.Models;

namespace dog2go.Controllers
{
    public class GameController : Controller
    {
        [Authorize]
        public ActionResult Play()
        {
            if (NotFullyAuthorized())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        private bool NotFullyAuthorized()
        {
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            if (users == null || users.Count == 0) return true;
            return users.FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value == null;
        }

        public ActionResult CreateTable()
        {
            TableViewModel model = new TableViewModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("CreateTable")]
        public ActionResult CreateTable(TableViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            int tableId = GameFactory.CreateGameTable(GameRepository.Instance, model.Name);
            AddParticipantToTable(tableId);
            return RedirectToAction("Play", "Game");
        }

        public ActionResult ChooseGameTable()
        {
            ChooseTableViewModel model = new ChooseTableViewModel();
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            gameTableList?.ForEach(table =>
            {
                model.GameTableList.Add(new TableViewModel(table.Identifier, table.Name));
            });
            return View(model);
        }

        [HttpPost]
        [ActionName("ChooseGameTable")]
        public ActionResult ChooseGameTable(TableViewModel model)
        {
            AddParticipantToTable(model.Identifier);
            return RedirectToAction("Play", "Game");
        }

        private void AddParticipantToTable(int tableId)
        {
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            GameTable gameTable = gameTableList?.Find(table => table.Identifier.Equals(tableId));
            ParticipationService.AddParticipation(gameTable, User.Identity.Name);
        }

        //[HttpGet]
        //[ActionName("GetTables")]
        //public ActionResult GetTables()
        //{
        //    List<GameTable> gamesList = GameRepository.Instance.Get();
        //    FormsAuthentication.SignOut();
        //    return RedirectToAction("Login", "Account");
        //}
    }
}