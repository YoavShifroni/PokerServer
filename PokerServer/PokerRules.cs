﻿using System;
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
        public Card secondHighCard;
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
                (HandRanking playerHandRank, Card highCard , Card secondHighCard) = EvaluateHand(playerHand.cards, communityCards);
                playerHand.handRanking = playerHandRank;
                playerHand.highCard = highCard;
                playerHand.secondHighCard = secondHighCard;

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
            if (maxPower.handRanking.Equals(HandRanking.FullHouse) || 
                maxPower.handRanking.Equals(HandRanking.TwoPair))
            {
                PlayerHand maxHighCard1 = new PlayerHand(null, null);
                Card ezer1 = new Card("2S");
                maxHighCard1.highCard = ezer1;
                maxHighCard1.secondHighCard = ezer1;
                foreach (PlayerHand playerHand in allMaxPowerPlayers)
                {
                    if (CardComparer.GetCardValue(playerHand.highCard) > CardComparer.GetCardValue(maxHighCard1.highCard))
                    {
                        maxHighCard1 = playerHand;
                    }
                }
                List<PlayerHand> allSameHighCardPlayer= new List<PlayerHand>();
                foreach(PlayerHand playerHand in allMaxPowerPlayers)
                {
                    if(CardComparer.GetCardValue(maxHighCard1.highCard) == CardComparer.GetCardValue(playerHand.highCard))
                    {
                        allSameHighCardPlayer.Add(playerHand);
                    }
                }
                if(allSameHighCardPlayer.Count == 1)
                {
                    List<string> winner = new List<string>();
                    winner.Add(maxHighCard1.username);
                    return winner;
                }
                foreach(PlayerHand playerHand in allSameHighCardPlayer)
                {
                    if (CardComparer.GetCardValue(playerHand.secondHighCard) > CardComparer.GetCardValue(maxHighCard1.secondHighCard))
                    {
                        maxHighCard1 = playerHand;
                    }
                }
                int countSecondHighCard = 0;
                foreach (PlayerHand playerHand in allSameHighCardPlayer)
                {
                    if (CardComparer.GetCardValue(maxHighCard1.secondHighCard) == CardComparer.GetCardValue(playerHand.secondHighCard))
                    {
                        countSecondHighCard++;
                    }
                }
                if(countSecondHighCard == 1)
                {
                    List<string> winner = new List<string>();
                    winner.Add(maxHighCard1.username);
                    return winner;
                }
                return CompareHighCards(allSameHighCardPlayer, communityCards);

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

        private static (HandRanking, Card, Card) EvaluateHand(List<Card> playerCards, List<Card> communityCards)
        {
            List<Card> allCards = playerCards.Concat(communityCards).ToList();

            (Card highCard1, bool isStraightFlush) = IsStraightFlush(allCards);
            (Card highCard2, bool isFourOfAKind) = IsFourOfAKind(allCards);
            (Card highCard3, Card secondHighCard, bool isFullHouse) = IsFullHouse(allCards);
            (Card highCard4, bool isFlush) = IsFlush(allCards);
            (Card highCard5, bool isStraight) = IsStraight(allCards);
            (Card highCard6, bool isThreeOfAKind) = IsThreeOfAKind(allCards);
            (Card highCard7, Card secondHighCard2, bool isTwoPair) = IsTwoPair(allCards);
            (Card highCard8, bool isOnePair) = IsOnePair(allCards);



            if (IsRoyalFlush(allCards))
            {
                Card highCard = new Card("AD");
                return(HandRanking.RoyalFlush, highCard, null);
            }
            if (isStraightFlush)
            {
                return (HandRanking.StraightFlush, highCard1,null);
            }
            if (isFourOfAKind)
            {
                return (HandRanking.FourOfAKind, highCard2, null);
            }
            if (isFullHouse)
            {
                return (HandRanking.FullHouse, highCard3, secondHighCard);
            }
            if (isFlush)
            {
                return (HandRanking.Flush, highCard4, null);
            }
            if (isStraight)
            {
                return (HandRanking.Straight, highCard5, null);
            }
            if (isThreeOfAKind)
            {
                return (HandRanking.ThreeOfAKind, highCard6, null);
            }
            if (isTwoPair)
            {
                return (HandRanking.TwoPair, highCard7, secondHighCard2);
            }
            if (isOnePair)
            {
                return (HandRanking.OnePair, highCard8, null);
            }
            allCards = allCards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
            return (HandRanking.HighCard, allCards.Last(), null);
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
                List<string> allWinnersName = new List<string>();
                foreach(PlayerHand playerHand in playerHands)
                {
                    allWinnersName.Add(playerHand.username);
                }
                return allWinnersName; // If you reach here it's a tie
            }
            foreach(PlayerHand hand in playerHands)
            {
                List<int> playerCardValue = hand.cards.ConvertAll(c => CardComparer.GetCardValue(c)).ToList();
                if (playerCardValue.Contains(highCard))
                {
                    result.Add(hand.username);
                }
            }
            return result; //in this case or there is one wiiner or its a tie bettwen some playres
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

            if (!isRoyalStraight)
            {
                return false;
            }

            Dictionary<int, string> cardAppearance = new Dictionary<int, string>();
            foreach (Card card in cards)
            {
                int cardValue = CardComparer.GetCardValue(card);

                if (cardAppearance.ContainsKey(cardValue))
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
                if (Pair.Value != null)
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


        public static (Card, Card, bool) IsFullHouse(List<Card> cards)
        {
            // Check for a full house
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            bool isFullHouse = false;
            Card highCard = null;
            Card secondHighcard = null;
            Card highCard1 = null;
            Card highCard2 = null;
            Card highCard3 = new Card("2S");
            for (int i = 0; i < cards.Count; i++)
            {
                int count = 0;
                highCard1 = cards.ElementAt(i);
                for (int j = 0; j < cards.Count; j++)
                {
                    if (CardComparer.GetCardValue(highCard1) == CardComparer.GetCardValue(cards[j]))
                    {
                        count++;
                    }
                }
                if (count == 3)
                {
                    break;
                }
                highCard1 = null;
            }
            if (highCard1 != null)
            {
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
                    if (count >= 2)
                    {
                        if (CardComparer.GetCardValue(highCard1) != CardComparer.GetCardValue(highCard2))
                        {
                            isFullHouse = true;
                            break;
                        }
                    }
                    highCard2 = null;
                }
            }
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
                    if(count== 3)
                    {
                        break;
                    }
                    highCard1 = null;
                }
                for(int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    highCard2 = cards.ElementAt(i);
                    for(int j = 0;j<cards.Count; j++)
                    {
                        if(CardComparer.GetCardValue(highCard2) == CardComparer.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if(count==3 && CardComparer.GetCardValue(highCard1) != CardComparer.GetCardValue(highCard2))
                    {
                        break;
                    }
                    highCard2 = null;
                }
                if(highCard1 != null && highCard2 != null)
                {
                    if(CardComparer.GetCardValue(highCard1) > CardComparer.GetCardValue(highCard2))
                    {
                        highCard = highCard1;
                        secondHighcard = highCard2;
                    }
                    else
                    {
                        highCard = highCard2;
                        secondHighcard = highCard1;
                    }
                    return (highCard, secondHighcard, isFullHouse);
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
                    if (count < 3 && count >= 2)
                    {
                        if (CardComparer.GetCardValue(highCard2) != CardComparer.GetCardValue(highCard1))
                        {
                            break;
                        }
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
                    if (count < 3 && count >= 2)
                    {
                        if (CardComparer.GetCardValue(highCard3) != CardComparer.GetCardValue(highCard1)
                            && CardComparer.GetCardValue(highCard3) != CardComparer.GetCardValue(highCard2))
                        {
                            break;
                        }
                    }
                    highCard3 = new Card("2S");
                }
                highCard = highCard1;
                if(CardComparer.GetCardValue(highCard2) > CardComparer.GetCardValue(highCard3))
                {
                    secondHighcard = highCard2;
                }
                else
                {
                    secondHighcard = highCard3;
                }

            }
            return (highCard, secondHighcard, isFullHouse);
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


        public static (Card, Card, bool) IsTwoPair(List<Card> cards)
        {
            // Check for two pair
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            int pairsCount = groups.Count(g => g.Count() == 2);
            Card highCard1 = null;
            Card highCard2 = null;
            bool isTwoPair = (pairsCount >= 2);
            if (isTwoPair)
            {
                bool isFirstHighCard = true;
                bool isSecondHighCard = false;
                cards = cards.OrderBy(c => CardComparer.GetCardValue(c)).ToList();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (isFirstHighCard)
                    {
                        int count = 0;
                        highCard1 = cards.ElementAt(i);
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (CardComparer.GetCardValue(highCard1) == CardComparer.GetCardValue(cards[j]))
                            {
                                count++;
                            }
                        }
                        if (count == 2)
                        {
                            isFirstHighCard = false;
                            isSecondHighCard = true;
                        }
                    }
                    if (isSecondHighCard)
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
                        if (count == 2)
                        {
                            if(CardComparer.GetCardValue(highCard2) != CardComparer.GetCardValue(highCard1))
                            {
                                break;
                            }
                        }
                    }
                    

                }

            }
            return (highCard1, highCard2, isTwoPair);
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

