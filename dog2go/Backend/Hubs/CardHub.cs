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

        private static readonly List<Card> Deck = new List<Card>();

        private static readonly Random Rng = new Random();
        public static void Shuffle()
        {
            int n = Deck.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                Card card = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = card;
            }
        }

        private List<Card> MakeInitDeck()
        {
            //make Jokercards
            for (int i = 0; i < 6; i++)
            {
                Deck.Add(new Card("cardJocker", 0, null, new List<CardAttribute>()
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
                Deck.Add(new Card("card2", 2, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwoFields) }));
                Deck.Add(new Card("card3", 3, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThreeFields) }));
                Deck.Add(new Card("card4", 4, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.FourFieldsBack), new CardAttribute(AttributeEnum.FourFields) }));
                Deck.Add(new Card("card5", 5, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.FiveFields) }));
                Deck.Add(new Card("card6", 6, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.SixFields) }));
                Deck.Add(new Card("card7", 7, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.SevenFields) }));
                Deck.Add(new Card("card8", 8, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.EightFields) }));
                Deck.Add(new Card("card9", 9, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.NineFields) }));
                Deck.Add(new Card("card10", 10, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TenFields) }));
                Deck.Add(new Card("card11", 11, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ElevenFields), new CardAttribute(AttributeEnum.OneField), new CardAttribute(AttributeEnum.LeaveKennel) }));
                Deck.Add(new Card("card12", 12, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwelveFields) }));
                Deck.Add(new Card("card13", 13, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThirteenFields), new CardAttribute(AttributeEnum.LeaveKennel) }));
                Deck.Add(new Card("cardChangePlace", 0, null, new List<CardAttribute>() { new CardAttribute(AttributeEnum.ChangePlace) }));
            }
            Shuffle();
            return Deck;
        }
    }
}