using System;
using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Model;

namespace dog2go.Backend.Services
{
    public class CardServices
    {

        private static readonly List<Card> Deck = new List<Card>();

        public int CurrentRound;

        private static readonly Random Rng = new Random();

        public CardServices()
        {
            CurrentRound = 0;
        }

        public int GetNumberOfCardsPerUser()
        {
            int nr = CurrentRound == 0 ? 6 : CurrentRound % 4 + 2;
            CurrentRound++;
            return nr;
        }

        public void CardExchange(User actualUser, ref GameTable actualGameTable, HandCard selectedCard)
        {
            User partner = GameServices.GetPartner(actualUser, actualGameTable.Participations);
            List<HandCard> actualHand = GetActualHandCards(actualUser, actualGameTable);
            actualHand.Remove(selectedCard);
            List<HandCard> partnerHandCards = GetActualHandCards(partner, actualGameTable);
            partnerHandCards.Add(selectedCard);
        }

        private List<Meeple> GetOtherMeeples(GameTable gameTable, List<Meeple> myMeeples)
        {
            List<Meeple> otherMeeples = new List<Meeple>();
            foreach (var playFieldArea in gameTable.PlayerFieldAreas)
            {
                otherMeeples.AddRange(playFieldArea.Meeples);
            }

            otherMeeples.RemoveAll(myMeeples.Contains);
            return otherMeeples;
        }

        private List<Meeple> GetOpenMeeples(List<Meeple> myMeeples)
        {
            return myMeeples.FindAll(
                    meeple =>
                        Validation.IsValidStartField(meeple.CurrentPosition) ||
                        meeple.CurrentPosition.FieldType.Contains("StandardField"));
        }

        private bool ProveLeaveKennel(List<Meeple> myMeeples)
        {
            List<Meeple> proveMeeples = myMeeples.FindAll(meeple => meeple.CurrentPosition.FieldType.Contains("KennelField"));
            Meeple proveStartMeeple = myMeeples.Find(startMeeple => !Validation.IsValidStartField(startMeeple.CurrentPosition));

            return proveMeeples != null && proveStartMeeple == null;
        }
        private bool ProveChangePlace(List<Meeple> myMeeples, List<Meeple> otherMeeples)
        {
            List<Meeple> myOpenMeeples = GetOpenMeeples(myMeeples);

            List<Meeple> otherOpenMeeples = GetOpenMeeples(otherMeeples);

            return myOpenMeeples.Count > 0 && otherOpenMeeples.Count > 0;
        }

        private bool ProveValueCard(List<Meeple> myMeeples, int value)
        {
            List<Meeple> myMovableMeeples = myMeeples.FindAll(meeple => Validation.IsMovableField(meeple.CurrentPosition));
            return myMovableMeeples.Any(meeple => !Validation.HasBlockedField(meeple.CurrentPosition, value) || Validation.CanMoveToEndFields(meeple.CurrentPosition, value));
        }

        private bool IsCardAlreadyUsed(HandCard card, List<HandCard> handCards)
        {
            HandCard duplicatedCard = handCards.Find(validCard => validCard != null && validCard.ImageIdentifier == card.ImageIdentifier);
            return duplicatedCard != null;
        }
        private List<HandCard> ProveCards(List<HandCard> actualHandCards, GameTable actualGameTable, User actualUser)
        {
            PlayerFieldArea actualArea = actualGameTable.PlayerFieldAreas.Find(
                area => area.Participation.Participant.Identifier == actualUser.Identifier);
            List<Meeple> myMeeples = actualArea.Meeples;

            return (from card in actualHandCards
                let validAttribute = card.Attributes.Find(attribute =>
                {
                    if (attribute.Attribute == AttributeEnum.LeaveKennel)
                    {
                        return ProveLeaveKennel(myMeeples);
                    }
                    return attribute.Attribute == AttributeEnum.ChangePlace ? ProveChangePlace(myMeeples, GetOtherMeeples(actualGameTable, myMeeples)) 
                                                                            : ProveValueCard(myMeeples, (int) attribute.Attribute);
                })
                select card).ToList();
        }

        public List<HandCard> GetActualHandCards(User actualUser, GameTable actualGameTable)
        {
            return actualGameTable.Participations.Find(
                participation =>
                    participation.Participant.Identifier == actualUser.Identifier).ActualPlayRound.Cards;
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
            if (Deck.Count < 1)
            {
                MakeInitDeck();
            }
            const int indexOfCardOnTop = 0;
            Card card = Deck[indexOfCardOnTop];
            Deck.RemoveAt(indexOfCardOnTop);
            return card;
        }

        private void MakeInitDeck()
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
                Deck.Add(new Card("card11", 11, "card_1-11-play_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ElevenFields), new CardAttribute(AttributeEnum.OneField), new CardAttribute(AttributeEnum.LeaveKennel) }));
                Deck.Add(new Card("card12", 12, "card_12_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.TwelveFields) }));
                Deck.Add(new Card("card13", 13, "card_13-play_190x300komp.png", new List<CardAttribute>() { new CardAttribute(AttributeEnum.ThirteenFields), new CardAttribute(AttributeEnum.LeaveKennel) }));
            }
            Shuffle();
        }
    }
}