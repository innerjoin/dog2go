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
        public int ProveCardsCount;

        private static readonly Random Rng = new Random();

        private static readonly CardAttribute CardAttributeOneField = new CardAttribute(CardFeature.OneField);
        private static readonly CardAttribute CardAttributeTwoFields = new CardAttribute(CardFeature.TwoFields);
        private static readonly CardAttribute CardAttributeThreeFields = new CardAttribute(CardFeature.ThreeFields);
        private static readonly CardAttribute CardAttributeFourFields = new CardAttribute(CardFeature.FourFields);
        private static readonly CardAttribute CardAttributeFourFieldsBack = new CardAttribute(CardFeature.FourFieldsBack);
        private static readonly CardAttribute CardAttributeFiveFields = new CardAttribute(CardFeature.FiveFields);
        private static readonly CardAttribute CardAttributeSixFields = new CardAttribute(CardFeature.SixFields);
        private static readonly CardAttribute CardAttributeSevenFields = new CardAttribute(CardFeature.SevenFields);
        private static readonly CardAttribute CardAttributeEightFields = new CardAttribute(CardFeature.EightFields);
        private static readonly CardAttribute CardAttributeNineFields = new CardAttribute(CardFeature.NineFields);
        private static readonly CardAttribute CardAttributeTenFields = new CardAttribute(CardFeature.TenFields);
        private static readonly CardAttribute CardAttributeElevenFields = new CardAttribute(CardFeature.ElevenFields);
        private static readonly CardAttribute CardAttributeTwelveFields = new CardAttribute(CardFeature.TwelveFields);
        private static readonly CardAttribute CardAttributeThirteenFields = new CardAttribute(CardFeature.ThirteenFields);
        private static readonly CardAttribute CardAttributeLeaveKennel = new CardAttribute(CardFeature.LeaveKennel);
        private static readonly CardAttribute CardAttributeChangePlace = new CardAttribute(CardFeature.ChangePlace);

        public CardServices()
        {
            CurrentRound = 1;
        }

        public int GetNumberOfCardsPerUser()
        {
            int nr = 6 - ((CurrentRound - 1) % 5);
            return nr;
        }

        public void CardExchange(User actualUser, ref GameTable actualGameTable, HandCard selectedCard)
        {
            if (actualUser == null || actualGameTable == null || selectedCard == null)
                return;
            User partner = ParticipationService.GetPartner(actualUser, actualGameTable.Participations);
            List<HandCard> actualHand = GetActualHandCards(actualUser, actualGameTable);
            actualHand.Remove(selectedCard);
            List<HandCard> partnerHandCards = GetActualHandCards(partner, actualGameTable);
            partnerHandCards.Add(selectedCard);
        }

        public bool ProveLeaveKennel(List<Meeple> myMeeples)
        {
            if (myMeeples == null)
                return false;
            List<Meeple> proveMeeples = myMeeples.FindAll(meeple => meeple.CurrentPosition.FieldType.Contains("KennelField"));
            Meeple proveStartMeeple = myMeeples.Find(startMeeple => startMeeple.CurrentPosition.FieldType.Contains("StartField") && !Validation.IsValidStartField(startMeeple.CurrentPosition));

            return proveMeeples.Count > 0 && proveStartMeeple == null;
        }
        public bool ProveChangePlace(List<Meeple> myMeeples, List<Meeple> otherMeeples)
        {
            if (myMeeples == null || otherMeeples == null)
                return false;
            List<Meeple> myOpenMeeples = GameTableService.GetOpenMeeples(myMeeples);

            List<Meeple> otherOpenMeeples = GameTableService.GetOpenMeeples(otherMeeples);

            if (myOpenMeeples.Count == 0 || otherOpenMeeples.Count == 0)
                return false;
            return myOpenMeeples.Where((t, i) => otherOpenMeeples.Exists(meeple => !Validation.IsSameColorCode(t.ColorCode, meeple.ColorCode))).Any();
        }

        public bool ProveValueCard(List<Meeple> myMeeples, int value)
        {
            if (myMeeples == null)
                return false;
            List<Meeple> myMovableMeeples = myMeeples.FindAll(meeple => Validation.IsMovableField(meeple.CurrentPosition));

            return myMovableMeeples.Any(meeple => (!Validation.HasBlockedField(meeple.CurrentPosition, value) &&
                                                    Validation.CanMoveToEndFields(meeple.CurrentPosition, value, meeple.ColorCode))||
                                                    (!Validation.HasBlockedField(meeple.CurrentPosition, value) &&
                                                    !Validation.CanMoveToEndFields(meeple.CurrentPosition, value, meeple.ColorCode) && !meeple.CurrentPosition.FieldType.Contains("EndField")));
        }

        private static void SetCardValid(List<HandCard> cards, bool valid)
        {
            foreach (HandCard card in cards)
            {
                card.IsValid = valid;
            }
        }
        public List<HandCard> ProveCards(List<HandCard> actualHandCards, GameTable actualGameTable, User actualUser)
        {
            if (actualHandCards == null || actualGameTable == null || actualUser == null)
                return null;
            PlayerFieldArea actualArea = actualGameTable.PlayerFieldAreas.Find(
                area => area.Participation.Participant.Identifier == actualUser.Identifier);
            List<Meeple> myMeeples = actualArea.Meeples;

            ProveCardsCount++;

            List<HandCard> validCards =  (from card in actualHandCards
                let validAttribute = card.Attributes.Find(attribute =>
                {
                    if (attribute.Attribute == CardFeature.LeaveKennel)
                    {
                        return ProveLeaveKennel(myMeeples);
                    }
                    return attribute.Attribute == CardFeature.ChangePlace
                        ? ProveChangePlace(myMeeples, GameTableService.GetOtherMeeples(actualGameTable, myMeeples))
                        : ProveValueCard(myMeeples, (int) attribute.Attribute);
                })
                where validAttribute != null
                select card).ToList();
            SetCardValid(actualHandCards, false);
            SetCardValid(validCards, true);
            return actualHandCards;
        }

        public bool RemoveCardFromUserHand(GameTable table, User actualUser,Card removeCard)
        {
            List<HandCard> handCards =
                table.Participations.Find(participation => participation.Participant.Nickname == actualUser.Nickname)
                    .ActualPlayRound.Cards;
            HandCard handCard = handCards.Find(card => card.Id == removeCard.Id);
            return handCards.Remove(handCard);
        }

        public void RemoveAllCardsFromUser(GameTable actualGameTable, User actualUser)
        {
            HandCard[] cardArray = new HandCard[6];
            actualGameTable.CardServiceData.GetActualHandCards(actualUser, actualGameTable).CopyTo(cardArray);
            foreach (var card in cardArray)
            {
                actualGameTable.CardServiceData.RemoveCardFromUserHand(actualGameTable, actualUser, card);
            }
        }

        public List<HandCard> GetActualHandCards(User actualUser, GameTable actualGameTable)
        {
            return actualGameTable.Participations.Find(
                participation =>
                    participation.Participant.Identifier == actualUser.Identifier).ActualPlayRound.Cards;
        }

        public bool AreCardsOnHand(GameTable actualGameTable)
        {
            return actualGameTable.Participations.Any(participation => GetActualHandCards(participation.Participant, actualGameTable).Count > 0);
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

        private static void MakeInitDeck()
        {
            //make all Jokercards
            for (int i = 0; i < 6; i++)
            {
                Deck.Add(new Card("cardJoker", 0, "card_joker_190x300komp.png", new List<CardAttribute>()
                {
                   CardAttributeOneField,
                   CardAttributeTwoFields,
                   CardAttributeThreeFields,
                   CardAttributeFourFields,
                   CardAttributeFourFieldsBack,
                   CardAttributeFiveFields,
                   CardAttributeSixFields,
                   CardAttributeSevenFields,
                   CardAttributeEightFields,
                   CardAttributeNineFields,
                   CardAttributeTenFields,
                   CardAttributeElevenFields,
                   CardAttributeTwelveFields,
                   CardAttributeThirteenFields,
                   CardAttributeChangePlace,
                   CardAttributeLeaveKennel
                }));
            }
            //make all other cards
            for (int i = 0; i < 8; i++)
            {
                Deck.Add(new Card("cardChangePlace", 1, "card_exchange_190x300komp.png", new List<CardAttribute>() { CardAttributeChangePlace }));
                Deck.Add(new Card("card2", 2, "card_2_190x300komp.png", new List<CardAttribute>() { CardAttributeTwoFields }));
                Deck.Add(new Card("card3", 3, "card_3_190x300komp.png", new List<CardAttribute>() { CardAttributeThreeFields }));
                Deck.Add(new Card("card4", 4, "card_4_190x300komp.png", new List<CardAttribute>() { CardAttributeFourFields, CardAttributeFourFieldsBack }));
                Deck.Add(new Card("card5", 5, "card_5_190x300komp.png", new List<CardAttribute>() { CardAttributeFiveFields }));
                Deck.Add(new Card("card6", 6, "card_6_190x300komp.png", new List<CardAttribute>() { CardAttributeSixFields }));
                Deck.Add(new Card("card7", 7, "card_7v1_190x300komp.png", new List<CardAttribute>() { CardAttributeSevenFields }));
                Deck.Add(new Card("card8", 8, "card_8_190x300komp.png", new List<CardAttribute>() { CardAttributeEightFields }));
                Deck.Add(new Card("card9", 9, "card_9_190x300komp.png", new List<CardAttribute>() { CardAttributeNineFields }));
                Deck.Add(new Card("card10", 10, "card_10_190x300komp.png", new List<CardAttribute>() { CardAttributeTenFields }));
                Deck.Add(new Card("card11", 11, "card_1-11-play_190x300komp.png", new List<CardAttribute>() { CardAttributeElevenFields, CardAttributeOneField, CardAttributeLeaveKennel }));
                Deck.Add(new Card("card12", 12, "card_12_190x300komp.png", new List<CardAttribute>() {CardAttributeTwelveFields }));
                Deck.Add(new Card("card13", 13, "card_13-play_190x300komp.png", new List<CardAttribute>() { CardAttributeThirteenFields, CardAttributeLeaveKennel }));
            }
            Shuffle();
        }

        public int GetDeckSize()
        {
            return Deck.Count;
        }
    }
}