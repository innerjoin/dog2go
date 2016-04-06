using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    public class CardHub : Hub
    {

        //Server Methoden
        /*public void SendCards(List<HandCard> cards);
        public void CheckHasOpportunity();// true notifyActualPlayer | false dropCards
        public void ChooseCard(HandCard card);
        public void ChooseCardExchange(HandCard card);
        public void ChooseMove(MeepleMove move);*/

        static List<Card> _deck = new List<Card>();

        private static Random _rng = new Random();
        public static void Shuffle()
        {
            int n = _deck.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                Card card = _deck[k];
                _deck[k] = _deck[n];
                _deck[n] = card;
            }
        }

        private List<Card> makeInitDeck()
        {
            //make Jokercards
            for (int i = 0; i < 6; i++)
            {
                _deck.Add(new Card("cardJocker", 0, null, new List<CardAttribute>()
                {
                    new CardAttribute(AttributeEnum.OneField),
                    new CardAttribute(AttributeEnum.TwoFields),
                    new CardAttribute(AttributeEnum.ThreeFields),
                    new CardAttribute(AttributeEnum.FourFields),
                    new CardAttribute(AttributeEnum.FourFieldsBack),
                    new CardAttribute(AttributeEnum.FiveFields),
                    new CardAttribute(AttributeEnum.SixFields),
                    new CardAttribute(AttributeEnum.SevenFields),
                    new CardAttribute(AttributeEnum.EightFields),
                    new CardAttribute(AttributeEnum.NineFields),
                    new CardAttribute(AttributeEnum.TenFields),
                    new CardAttribute(AttributeEnum.ElevenFields),
                    new CardAttribute(AttributeEnum.TwelveFields),
                    new CardAttribute(AttributeEnum.ThirteenFields),
                    new CardAttribute(AttributeEnum.ChangePlace),
                    new CardAttribute(AttributeEnum.LeaveKennel)
                }));
            }
            //make all other cards
            for (int i = 0; i < 8; i++)
            {
                _deck.Add(new Card("card2", 2, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwoFields) }));
                _deck.Add(new Card("card3", 3, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThreeFields) }));
                _deck.Add(new Card("card4", 4, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.FourFieldsBack), new CardAttribute(AttributeEnum.FourFields) }));
                _deck.Add(new Card("card5", 5, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.FiveFields) }));
                _deck.Add(new Card("card6", 6, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.SixFields) }));
                _deck.Add(new Card("card7", 7, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.SevenFields) }));
                _deck.Add(new Card("card8", 8, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.EightFields) }));
                _deck.Add(new Card("card9", 9, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.NineFields) }));
                _deck.Add(new Card("card10", 10, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TenFields) }));
                _deck.Add(new Card("card11", 11, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ElevenFields), new CardAttribute(AttributeEnum.OneField), new CardAttribute(AttributeEnum.LeaveKennel) }));
                _deck.Add(new Card("card12", 12, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwelveFields) }));
                _deck.Add(new Card("card13", 13, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThirteenFields), new CardAttribute(AttributeEnum.LeaveKennel) }));
                _deck.Add(new Card("cardChangePlace", 0, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ChangePlace) }));
            }
            Shuffle();
            return _deck;
        }
    }
}