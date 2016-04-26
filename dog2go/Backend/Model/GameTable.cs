using System;
using System.Collections.Generic;
using dog2go.Backend.Services;

namespace dog2go.Backend.Model
{
    public class GameTable
    {
        public string Name { get; set; }

        public int Identifier { get; set; }

        public DateTime Start { get; set; }
        public string Cookie { get; set; }
        public DateTime Stop { get; set; }
        public List<PlayerFieldArea> PlayerFieldAreas;
        public List<Participation> Participations; 
        //public int CurremtRound { get; set; }
        public CardServices cardServiceData { get; private set; }
        public GameTable(List<PlayerFieldArea> areas, int id)
        {
            PlayerFieldAreas = areas;
            Identifier = id;
            Participations = new List<Participation>();
            //CurremtRound = 0;
        }

        public void RegisterCardService(CardServices service)
        {
            if(cardServiceData != null)
                throw new Exception("already registered");
            cardServiceData = service;
        }
    }
}
