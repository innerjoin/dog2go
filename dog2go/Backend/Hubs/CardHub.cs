﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Backend.Hubs
{
    [HubName("cardHub")]
    public class CardHub : Hub
    {

        /*
         * Server Methoden
         * public void SendCards(List<HandCard> cards);
         * public void CheckHasOpportunity();// true notifyActualPlayer | false dropCards
         * public void ChooseCard(HandCard card);
         * public void ChooseCardExchange(HandCard card);
         * public void ChooseMove(MeepleMove move);
         */

        private static readonly List<Card> Deck = new List<Card>();

        private static int _currentRound = 0;

        private static readonly Random Rng = new Random();

        public void SendCards(List<HandCard> cards, User user)
        {
            // TODO: send hand cards to specific user
        }

        public void AllConnected()
        {
            int nr = GetNumberOfCardsPerUser();
            List<User> users = null; //TODO: how to access users???
            foreach (User user in users)
            {
                List<HandCard> cards = new List<HandCard>();
                for (int i = 0; i < nr; i++)
                {
                    cards.Add(new HandCard(GetCard()));
                }
                SendCards(cards, user);
            }
        }

        private static int GetNumberOfCardsPerUser()
        {
            int nr;
            if (_currentRound == 0)
                nr = 6;
            nr = _currentRound % 4 + 2;
            _currentRound++;
            return nr;
            // R    | Nr
            // 1    | 5
            // 2    | 4
            // 3    | 3
            // 4    | 2
        }

        private static void Shuffle()
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

        public Card GetCard()
        {
            const int indexOfCardOnTop = 0;
            Card card = Deck[indexOfCardOnTop];
            Deck.RemoveAt(indexOfCardOnTop);
            if (Deck.Count <= 1)
            {
                MakeInitDeck();
            }
            return card;
        }

        private List<Card> MakeInitDeck()
        {
            //make all Jokercards
            for (int i = 0; i < 6; i++)
            {
                Deck.Add(new Card("cardJocker", 0, "card_joker_190x300komp.png", new List<CardAttribute>()
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
                Deck.Add(new Card("cardChangePlace", 1, "card_exchange_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ChangePlace) }));
                Deck.Add(new Card("card2", 2, "card_2_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwoFields) }));
                Deck.Add(new Card("card3", 3, "card_3_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThreeFields) }));
                Deck.Add(new Card("card4", 4, "card_4_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.FourFieldsBack), new CardAttribute(AttributeEnum.FourFields) }));
                Deck.Add(new Card("card5", 5, "card_5_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.FiveFields) }));
                Deck.Add(new Card("card6", 6, "card_6_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.SixFields) }));
                Deck.Add(new Card("card7", 7, "card_7v1_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.SevenFields) }));
                Deck.Add(new Card("card8", 8, "card_8_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.EightFields) }));
                Deck.Add(new Card("card9", 9, "card_9_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.NineFields) }));
                Deck.Add(new Card("card10", 10, "card_10_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.TenFields) }));
                Deck.Add(new Card("card11", 11, "card_1-11_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ElevenFields), new CardAttribute(AttributeEnum.OneField), new CardAttribute(AttributeEnum.LeaveKennel) }));
                Deck.Add(new Card("card12", 12, "card_12_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwelveFields) }));
                Deck.Add(new Card("card13", 13, "card_13-play_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThirteenFields), new CardAttribute(AttributeEnum.LeaveKennel) }));
            }
            Shuffle();
            return Deck;
        }
    }
}