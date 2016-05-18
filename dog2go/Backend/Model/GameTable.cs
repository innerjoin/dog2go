using System;
using System.Collections.Generic;
using dog2go.Backend.Constants;
using dog2go.Backend.Services;

namespace dog2go.Backend.Model
{
    public class GameTable
    {
        public string Name { get; private set; }

        public int Identifier { get; private set; }

        public int TableSize { get; private set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<PlayerFieldArea> PlayerFieldAreas;

        public List<Participation> Participations;
        public Participation ActualParticipation;
        public CardServices CardServiceData { get; private set; }
        public GameTable(List<PlayerFieldArea> areas, int id, string name)
        {
            PlayerFieldAreas = areas;
            Identifier = id;
            Participations = new List<Participation>();
            Name = name;
            TableSize = GlobalDefinitions.NofParticipantsPerTable;
        }

        public void RegisterCardService(CardServices service)
        {
            /*if(cardServiceData != null)
                throw new Exception("already registered");
            cardServiceData = service;*/
            if (CardServiceData == null)
                CardServiceData = service;
        }

        public bool IsFull()
        {
            if(Participations.Count > TableSize)
                throw new Exception("should not happen: table has more participants than playerfieldareas!!");
            return Participations.Count == TableSize;
        }
    }
}
