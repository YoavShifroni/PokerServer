using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            int count = 0;
            foreach (PlayerHand playerHand in playersCards)
            {
                if(maxPower.handRanking == playerHand.handRanking)
                {
                    count++;
                    allMaxPowerPlayers.Add(playerHand);
                }
            }
            if(count == 1)
            {
                List<string> winner = new List<string>();
                winner.Add(maxPower.username);
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
                    result.Add(hand.username);
                }
            }
            return result; // return the names of all the winners
        }

        

        // Custom card comparer

        // Hand evaluation methods

        public static bool IsRoyalFlush(List<Card> cards)
        {
            // check for a royal flush
            bool isFlushOnRoyalCards = false;
            bool isRoyalStraight = false;
            cards = cards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();

            if(CardComparer.GetCardType(cards.ElementAt(cards.Count-1)).Equals
                (CardComparer.GetCardType(cards.ElementAt(cards.Count - 2)))
                && (CardComparer.GetCardType(cards.ElementAt(cards.Count-2))).Equals
                (CardComparer.GetCardType(cards.ElementAt(cards.Count - 3)))
                && (CardComparer.GetCardType(cards.ElementAt(cards.Count-3))).Equals
                (CardComparer.GetCardType(cards.ElementAt(cards.Count-4)))
                && (CardComparer.GetCardType(cards.ElementAt(cards.Count-4))).Equals
                (CardComparer.GetCardType(cards.ElementAt(cards.Count-5))))
            {
                isFlushOnRoyalCards=true;
            }
            if (CardComparer.GetCardValue(cards.ElementAt(cards.Count - 1)) == 14 &&
                CardComparer.GetCardValue(cards.ElementAt(cards.Count - 2)) == 13 &&
                CardComparer.GetCardValue(cards.ElementAt(cards.Count - 3)) == 12 &&
                CardComparer.GetCardValue(cards.ElementAt(cards.Count - 4)) == 11 &&
                CardComparer.GetCardValue(cards.ElementAt(cards.Count - 5)) == 10)
            {
                isRoyalStraight = true;

            }
            return isRoyalStraight && isFlushOnRoyalCards;

        }

        public static (Card, bool) IsStraightFlush(List<Card> cards)
        {
            // Check for a straight flush
            bool isFlush = false;
            bool isStraight = false;
            bool isStraightFlush = false;
            int index = 0;
            Card highCard = null;
            cards = cards.OrderBy(c => CardComparer.GetCardValue2(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (CardComparer.GetCardValue2(cards[i]) == CardComparer.GetCardValue2(cards[i + 1]) - 1
                    && CardComparer.GetCardValue2(cards[i + 1]) == CardComparer.GetCardValue2(cards[i + 2]) - 1
                    && CardComparer.GetCardValue2(cards[i + 2]) == CardComparer.GetCardValue2(cards[i + 3]) - 1
                    && CardComparer.GetCardValue2(cards[i + 3]) == CardComparer.GetCardValue2(cards[i + 4]) - 1)
                {
                    isStraight = true;
                    index = i;
                    if (cards[i].nameOfCard.Equals("AH") || cards[i].nameOfCard.Equals("AD") 
                        || cards[i].nameOfCard.Equals("AC") || cards[i].nameOfCard.Equals("AS"))
                    {
                        highCard = cards[i];
                    }
                    else
                    {
                        highCard = cards[i + 4];
                    }
                    break;
                }
            }
            if (CardComparer.GetCardType(cards[index]).Equals(CardComparer.GetCardType(cards[index + 1]))
                    && CardComparer.GetCardType(cards[index + 1]).Equals(CardComparer.GetCardType(cards[index + 2]))
                    && CardComparer.GetCardType(cards[index + 2]).Equals(CardComparer.GetCardType(cards[index + 3]))
                    && CardComparer.GetCardType(cards[index + 3]).Equals(CardComparer.GetCardType(cards[index + 4])))
            {
                isFlush = true;
            }
            if(isFlush && isStraight)
            {
                isStraightFlush = true;
            }
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

            if (isFullHouse)
            {
                for(int i = 0; i<cards.Count; i++)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for(int j = 0; j<cards.Count; j++)
                    {
                        if(CardComparer.GetCardValue(highCard) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if(count == 3)
                    {
                        break;
                    }
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
                    highCard = cards[i];
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

