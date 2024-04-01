using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokerServer
{

    // Hand ranking enumeration
    public enum HandRanking
    {
        None,
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush,
    };

    public class PlayerHand
    {
        public List<Card> cards;
        public string username = "";
        public HandRanking handRanking;
        public Card highCard;
        public PlayerHand(List<Card> cards, string username)
        {
            this.cards = cards;
            this.username = username;
            handRanking = HandRanking.None;
        }


    }
    public class PokerRules
    {

        public static List<string> DetermineWinner(List<PlayerHand> playersCards, List<Card> communityCards)
        {

            foreach (PlayerHand playerHand in playersCards)
            {
                (HandRanking playerHandRank, Card highCard) = EvaluateHand(playerHand.cards, communityCards);
                playerHand.handRanking = playerHandRank;
                playerHand.highCard = highCard;

            }

            // Compare hand rankings
            PlayerHand maxPower = new PlayerHand(null,null);
            maxPower.handRanking = HandRanking.None;
            foreach (PlayerHand playerHand in playersCards)
            {
                if(playerHand.handRanking > maxPower.handRanking)
                {
                    maxPower = playerHand;
                }
            }

            List<PlayerHand> allMaxPowerPlayers = new List<PlayerHand>();
            int countHand = 0;
            foreach (PlayerHand playerHand in playersCards)
            {
                if(maxPower.handRanking == playerHand.handRanking)
                {
                    countHand++;
                    allMaxPowerPlayers.Add(playerHand);
                }
            }
            if(countHand == 1)
            {
                List<string> winner = new List<string>();
                winner.Add(maxPower.username);
                return winner;
            }


            PlayerHand maxHighCard = new PlayerHand(null,null);
            Card ezer = new Card("2S");
            maxHighCard.highCard = ezer;
            foreach (PlayerHand playerHand in allMaxPowerPlayers)
            {
                if(CardComparer.GetCardValue(playerHand.highCard) > CardComparer.GetCardValue(maxHighCard.highCard))
                {
                    maxHighCard = playerHand;
                }
            }

            int countHighCard = 0;
            foreach (PlayerHand playerHand in allMaxPowerPlayers)
            {
                if (CardComparer.GetCardValue(maxHighCard.highCard) == CardComparer.GetCardValue(playerHand.highCard))
                {
                    countHighCard++;
                }
            }

            if (countHighCard == 1)
            {
                List<string> winner = new List<string>();
                winner.Add(maxHighCard.username);
                return winner;
            }

            // If multiple players have the same hand ranking, compare the high cards
            return CompareHighCards(allMaxPowerPlayers, communityCards);
        }

        private static (HandRanking, Card) EvaluateHand(List<Card> playerCards, List<Card> communityCards)
        {
            List<Card> allCards = playerCards.Concat(communityCards).ToList();

            (Card highCard1, bool isStraightFlush) = IsStraightFlush(allCards);
            (Card highCard2, bool isFourOfAKind) = IsFourOfAKind(allCards);
            (Card highCard3, bool isFullHouse) = IsFullHouse(allCards);
            (Card highCard4, bool isFlush) = IsFlush(allCards);
            (Card highCard5, bool isStraight) = IsStraight(allCards);
            (Card highCard6, bool isThreeOfAKind) = IsThreeOfAKind(allCards);
            (Card highCard7, bool isTwoPair) = IsTwoPair(allCards);
            (Card highCard8, bool isOnePair) = IsOnePair(allCards);



            if (IsRoyalFlush(allCards))
            {
                Card highCard = new Card("AD");
                return(HandRanking.RoyalFlush, highCard);
            }
            if (isStraightFlush)
            {
                return (HandRanking.StraightFlush, highCard1);
            }
            if (isFourOfAKind)
            {
                return (HandRanking.FourOfAKind, highCard2);
            }
            if (isFullHouse)
            {
                return (HandRanking.FullHouse, highCard3);
            }
            if (isFlush)
            {
                return (HandRanking.Flush, highCard4);
            }
            if (isStraight)
            {
                return (HandRanking.Straight, highCard5);
            }
            if (isThreeOfAKind)
            {
                return (HandRanking.ThreeOfAKind, highCard6);
            }
            if (isTwoPair)
            {
                return (HandRanking.TwoPair, highCard7);
            }
            if (isOnePair)
            {
                return (HandRanking.OnePair, highCard8);
            }
            allCards = allCards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
            return (HandRanking.HighCard, allCards.Last());
        }

        

        private static List<string> CompareHighCards(List<PlayerHand> playerHands, List<Card> communityCards)
        {
            int count = 0;
            List<string> result = new List<string>();
            List<int> communityCardsValue = communityCards.ConvertAll(c => CardComparer.GetCardValue(c)).ToList();
            List<int> allCards = new List<int>();
            allCards.AddRange(communityCardsValue);
            foreach(PlayerHand playerHand in playerHands)
            {
                List<int> playerHandCards = playerHand.cards.ConvertAll(c => CardComparer.GetCardValue(c)).ToList();
                allCards.AddRange(playerHandCards);
            }
            allCards.Sort();
            int highCard = allCards.Last();

            if (communityCardsValue.Contains(highCard))
            {
                return null; // If you reach here it's a tie
            }
            foreach(PlayerHand hand in playerHands)
            {
                if (CardComparer.GetCardType(hand.highCard).Equals(highCard))
                {
                    count++;
                    result.Add(hand.username);
                }
            }
            if(count == 1)
            {
                return result;
            }
            return null; // Its a tie
        }

        

        // Custom card comparer

        // Hand evaluation methods

        public static bool IsRoyalFlush(List<Card> cards)
        {
            // check for a royal flush
            bool isFlushOnRoyalCards = false;
            bool isRoyalStraight = false;
            cards = cards.Where(c => CardComparer.GetCardValue(c) >= 10).OrderBy(c => CardComparer.GetCardValue(c)).ToList();
            List<Card> groupeCards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();


            int count = groupeCards.Count();
            if (count < 5)
            {
                return false;
            }

            if (CardComparer.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 1)) == 14 &&
                CardComparer.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 2)) == 13 &&
                CardComparer.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 3)) == 12 &&
                CardComparer.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 4)) == 11 &&
                CardComparer.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 5)) == 10)
            {
                isRoyalStraight = true;
            }

            if(!isRoyalStraight)
            {
                return false;
            }

            Dictionary<int, string> cardAppearance = new Dictionary<int, string>();
            foreach (Card card in cards)
            {
                int cardValue = CardComparer.GetCardValue(card);

                if(cardAppearance.ContainsKey(cardValue))
                {
                    cardAppearance[cardValue] = null;
                } 
                else
                {
                    cardAppearance[cardValue] = CardComparer.GetCardType(card);
                }
            }

            string cardType = null;
            foreach (var Pair in cardAppearance)
            {
                if(Pair.Value != null)
                {
                    cardType = Pair.Value;
                    break;
                }
            }

            if (cardType == null)
            {
                return false;
            }

            List<Card> cardsWithSameType = cards.Where(card => CardComparer.GetCardType(card).Equals(cardType)).ToList();  

            if (cardsWithSameType.Count == 5)
            {
                return true;
            }
            
            return false;

        }


        //public static (Card, bool) IsStraightFlush2(List<Card> cards)
        //{
        //    // Extract card ranks and sort them
        //    var ranks = cards.Select(card => CardComparer.GetCardValue2(card)).Distinct().ToList();
        //    if (ranks.Count < 5)
        //    {
        //        return (null, false);
        //    }
        //    ranks.Sort();

        //    // Check for straight (consecutive ranks) and same suit
        //    bool isStraight = true;
        //    string firstSuit = CardComparer.GetCardType(cards[0]); // Get suit from first card
        //    for (int i = 1; i < ranks.Count; i++)
        //    {
        //        if (ranks[i] != ranks[i - 1] + 1 || 
        //            cards.Find(c => CardComparer.GetCardType(cards[i]).Equals(ranks[i])).nameOfCard.Substring(cards[i].nameOfCard.Length - 1) != firstSuit)
        //        {
        //            isStraight = false;
        //            break;
        //        }
        //    }

        //    // Check if it's a straight flush (straight and all same suit)
        //    bool isStraightFlush = isStraight && cards.All(card => CardComparer.GetCardType(card).Equals(firstSuit));

        //    // Return high card and straight flush status
        //    return (isStraightFlush ? cards.Last() : null, isStraightFlush);
        //}


        public static (Card, bool) IsStraightFlush(List<Card> cards)
        {
            // Check for a straight flush
            bool isStraightFlush = false;
            Card highCard = new Card("2S");
            string cardType = null;
            int count  = 0;
            for(int i = 0; i<4; i++)
            {
                count = 0;
                if (i == 0)
                {
                    cardType = "H";
                }
                if(i == 1)
                {
                    cardType = "D";
                }
                if(i == 2)
                {
                    cardType = "C";
                }
                if(i == 3)
                {
                    cardType = "S";
                }
                foreach(Card card in cards)
                {
                    if (CardComparer.GetCardType(card).Equals(cardType))
                    {
                        count++;
                    }
                    
                }
                if(count>= 5)
                {
                    break;
                }
            }
            if(count < 5)
            {
                return(null,false);
            }
            List<Card> cardsWithSameType = new List<Card>();
            foreach(Card card in cards)
            {
                if (CardComparer.GetCardType(card).Equals(cardType))
                {
                    cardsWithSameType.Add(card);
                }
            }
            cardsWithSameType = cardsWithSameType.OrderBy(c => CardComparer.GetCardValue2(c)).ToList();
            (highCard, isStraightFlush) = PokerRules.IsStraight(cardsWithSameType);
            return (highCard, isStraightFlush);
        }

        public static (Card, bool) IsFourOfAKind(List<Card> cards)
        {
            // Check for four of a kind
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            Card highCard = null;
            bool isFourOfAKind = groups.Any(g => g.Count() == 4);
            if(isFourOfAKind)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count == 4)
                    {
                        break;
                    }
                }

            }
            return (highCard, isFourOfAKind);

        }

        public static (Card, bool) IsFullHouse(List<Card> cards)
        {
            // Check for a full house
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            bool isFullHouse = groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2);
            Card highCard = null;
            Card highCard1 = null;
            Card highCard2 = null;
            Card highCard3 = new Card("2S");

            if (isFullHouse)
            {
                for(int i = 0; i<cards.Count; i++)
                {
                    int count = 0;
                    highCard1 = cards.ElementAt(i);
                    for(int j = 0; j<cards.Count; j++)
                    {
                        if(CardComparer.GetCardValue(highCard1) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if(count >= 3)
                    {
                        highCard1 = cards.ElementAt(i);
                        break;
                    }
                }

                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard2 = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard2) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count <= 3 && count>=2)
                    {
                        if(CardComparer.GetCardValue(highCard2) != CardComparer.GetCardValue(highCard1))
                        {
                            highCard2 = cards.ElementAt(i);
                        }
                        break;
                    }
                }
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard3 = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard3) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count <= 3 && count >= 2)
                    {
                        if (CardComparer.GetCardValue(highCard3) != CardComparer.GetCardValue(highCard1) 
                            && CardComparer.GetCardValue(highCard3) != CardComparer.GetCardValue(highCard2))
                        {
                            highCard3 = cards.ElementAt(i);
                        }
                        break;
                    }
                }
                if (CardComparer.GetCardValue(highCard1) >= CardComparer.GetCardValue(highCard2) 
                    && CardComparer.GetCardValue(highCard1) >= CardComparer.GetCardValue(highCard3))
                {
                    highCard = highCard1;
                }
                else if(CardComparer.GetCardValue(highCard2) >= CardComparer.GetCardValue(highCard1)
                    && CardComparer.GetCardValue(highCard2) >= CardComparer.GetCardValue(highCard3))
                { 
                    highCard = highCard2;
                }
                else
                {
                    highCard = highCard3;
                }
            }
            return (highCard,isFullHouse);
        }

        public static (Card, bool) IsFlush(List<Card> cards)
        {
            // Check for a flush
            var groups = cards.GroupBy(c => c.nameOfCard.Last());
            bool isFlush = groups.Any(g => g.Count() >= 5);
            Card highCard = null;
            if (isFlush)
            {
                cards = cards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
                for (int i = cards.Count-1; i >=0; i--)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardType(highCard).Equals(CardComparer.GetCardType(cards[j])))
                        {
                            count++;
                        }
                    }
                    if (count >= 5)
                    {
                        break;
                    }
                }

            }
            return (highCard, isFlush);
        }

        public static (Card, bool) IsStraight(List<Card> cards)
        {
            // Check for a straight 

            bool isStraight = false;
            cards = cards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();
            Card highCard = null;
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (CardComparer.GetCardValue(cards[i]) == CardComparer.GetCardValue(cards[i + 1]) - 1
                    && CardComparer.GetCardValue(cards[i + 1]) == CardComparer.GetCardValue(cards[i + 2]) - 1
                    && CardComparer.GetCardValue(cards[i + 2]) == CardComparer.GetCardValue(cards[i + 3]) - 1
                    && CardComparer.GetCardValue(cards[i + 3]) == CardComparer.GetCardValue(cards[i + 4]) - 1)
                {
                    highCard = cards[i+4];
                    isStraight = true;
                    return(highCard,isStraight);
                }
            }
            cards = cards.OrderBy(c => CardComparer.GetCardValue2(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (CardComparer.GetCardValue2(cards[i]) == CardComparer.GetCardValue2(cards[i + 1]) - 1
                    && CardComparer.GetCardValue2(cards[i + 1]) == CardComparer.GetCardValue2(cards[i + 2]) - 1
                    && CardComparer.GetCardValue2(cards[i + 2]) == CardComparer.GetCardValue2(cards[i + 3]) - 1
                    && CardComparer.GetCardValue2(cards[i + 3]) == CardComparer.GetCardValue2(cards[i + 4]) - 1)
                {
                    highCard = cards[i+4];
                    isStraight = true;
                    return (highCard, isStraight);
                }
            }
            return (highCard, false);

        }

        public static (Card, bool) IsThreeOfAKind(List<Card> cards)
        {
            // Check for three of a kind
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            bool isThreeOfAKind = groups.Any(g => g.Count() == 3);
            Card highCard = null;
            if (isThreeOfAKind)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count == 3)
                    {
                        break;
                    }
                }

            }
            return (highCard, isThreeOfAKind);
        }

        public static (Card, bool) IsTwoPair(List<Card> cards)
        {
            // Check for two pair
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            int pairsCount = groups.Count(g => g.Count() == 2);
            Card highCard = null;
            bool isTwoPair = (pairsCount == 2);
            if (isTwoPair)
            {
                cards = cards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count == 2)
                    {
                        break;
                    }

                }

            }
            return (highCard, isTwoPair);
        }

        public static (Card, bool) IsOnePair(List<Card> cards)
        {
            // Check for one pair
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            bool isOnePair = groups.Any(g => g.Count() == 2);
            Card highCard = null;
            if (isOnePair)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (CardComparer.GetCardValue(highCard) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count == 2)
                    {
                        break;
                    }
                }

            }
            return (highCard, isOnePair);
        }





    }

}

