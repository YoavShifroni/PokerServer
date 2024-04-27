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

    
    public class PokerRules
    {

        /// <summary>
        /// the function return List that contain the names of the winners in this around acording to the
        /// Poker rules
        /// </summary>
        /// <param name="playersCards">A list of PlayerHand per players in the game</param>
        /// <param name="communityCards">The list of community cards on the table</param>
        /// <returns>A list of usernames that are the winners (1 or more, in case of tie)</returns>
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

            // count how many players have the highest hand ranking
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
            // if there is only one player with the highest hand ranking he is the winner
            if(countHand == 1)
            {
                List<string> winner = new List<string>();
                winner.Add(maxPower.username);
                return winner;
            }
            // if the highest hand ranking is "TwoPair" or "FullHouse" deal with it in specific way
            if (maxPower.handRanking.Equals(HandRanking.FullHouse) || 
                maxPower.handRanking.Equals(HandRanking.TwoPair))
            {
                PlayerHand maxHighCard1 = new PlayerHand(null, null);
                Card ezer1 = new Card("2S");
                maxHighCard1.highCard = ezer1;
                maxHighCard1.secondHighCard = ezer1;
                // find the highest high card
                foreach (PlayerHand playerHand in allMaxPowerPlayers)
                {
                    if (Card.GetCardValue(playerHand.highCard) > Card.GetCardValue(maxHighCard1.highCard))
                    {
                        maxHighCard1 = playerHand;
                    }
                }
                // add to the list all the players that have this high card
                List<PlayerHand> allSameHighCardPlayer= new List<PlayerHand>();
                foreach(PlayerHand playerHand in allMaxPowerPlayers)
                {
                    if(Card.GetCardValue(maxHighCard1.highCard) == Card.GetCardValue(playerHand.highCard))
                    {
                        allSameHighCardPlayer.Add(playerHand);
                    }
                }
                // if there is only one player with the highest high card he is the winner
                if(allSameHighCardPlayer.Count == 1)
                {
                    List<string> winner = new List<string>();
                    winner.Add(maxHighCard1.username);
                    return winner;
                }
                // from the players that have the highest high card find the highest second high card
                foreach(PlayerHand playerHand in allSameHighCardPlayer)
                {
                    if (Card.GetCardValue(playerHand.secondHighCard) > Card.GetCardValue(maxHighCard1.secondHighCard))
                    {
                        maxHighCard1 = playerHand;
                    }
                }
                // from the players that have the highest high card count how many of theme have also
                // the highest second high card
                int countSecondHighCard = 0;
                foreach (PlayerHand playerHand in allSameHighCardPlayer)
                {
                    if (Card.GetCardValue(maxHighCard1.secondHighCard) == Card.GetCardValue(playerHand.secondHighCard))
                    {
                        countSecondHighCard++;
                    }
                }
                // if there is only one player with the highest second high card he is the winner
                if(countSecondHighCard == 1)
                {
                    List<string> winner = new List<string>();
                    winner.Add(maxHighCard1.username);
                    return winner;
                }
                // if you get to here that mean that some players have the highest hand ranking with the same cards in it
                return CompareHighCards(allSameHighCardPlayer, communityCards);

            }
            PlayerHand maxHighCard = new PlayerHand(null,null);
            Card ezer = new Card("2S");
            maxHighCard.highCard = ezer;
            // find the highest high card
            foreach (PlayerHand playerHand in allMaxPowerPlayers)
            {
                if(Card.GetCardValue(playerHand.highCard) > Card.GetCardValue(maxHighCard.highCard))
                {
                    maxHighCard = playerHand;
                }
            }
            // count how many players have the highest high card
            int countHighCard = 0;
            foreach (PlayerHand playerHand in allMaxPowerPlayers)
            {
                if (Card.GetCardValue(maxHighCard.highCard) == Card.GetCardValue(playerHand.highCard))
                {
                    countHighCard++;
                }
            }
            // if there is only one player with the highest high card he is the winner
            if (countHighCard == 1)
            {
                List<string> winner = new List<string>();
                winner.Add(maxHighCard.username);
                return winner;
            }

            // If multiple players have the same hand ranking, compare the high cards
            return CompareHighCards(allMaxPowerPlayers, communityCards);
        }

        /// <summary>
        /// the function return the hand ranking of the player with the cards that in it
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="communityCards"></param>
        /// <returns></returns>
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


            // if the player has hand ranking of "Royal Flush"
            if (IsRoyalFlush(allCards))
            {
                Card highCard = new Card("AD");
                return(HandRanking.RoyalFlush, highCard, null);
            }
            // if the player has hand ranking of "Straight Flush"
            if (isStraightFlush)
            {
                return (HandRanking.StraightFlush, highCard1,null);
            }
            // if the player has hand ranking of "Four Of A Kind"
            if (isFourOfAKind)
            {
                return (HandRanking.FourOfAKind, highCard2, null);
            }
            // if the player has hand ranking of "Full House"
            if (isFullHouse)
            {
                return (HandRanking.FullHouse, highCard3, secondHighCard);
            }
            // if the player has hand ranking of "Flush"
            if (isFlush)
            {
                return (HandRanking.Flush, highCard4, null);
            }
            // if the player has hand ranking of "Straight"
            if (isStraight)
            {
                return (HandRanking.Straight, highCard5, null);
            }
            // if the player has hand ranking of "Three Of A Kind"
            if (isThreeOfAKind)
            {
                return (HandRanking.ThreeOfAKind, highCard6, null);
            }
            // if the player has hand ranking of "Two Pair"
            if (isTwoPair)
            {
                return (HandRanking.TwoPair, highCard7, secondHighCard2);
            }
            // if the player has hand ranking of "Pair"
            if (isOnePair)
            {
                return (HandRanking.OnePair, highCard8, null);
            }
            // if you get here that mean that the player has the lowest hand ranking of "High Card" 
            allCards = allCards.OrderBy(c => Card.GetCardValue(c)).ToList();
            return (HandRanking.HighCard, allCards.Last(), null);
        }

        
        /// <summary>
        /// the function compare the high cards of the players that it recive and return List
        /// the contain the name of the winners
        /// </summary>
        /// <param name="playerHands"></param>
        /// <param name="communityCards"></param>
        /// <returns></returns>
        private static List<string> CompareHighCards(List<PlayerHand> playerHands, List<Card> communityCards)
        {
            List<string> result = new List<string>();
            List<int> communityCardsValue = communityCards.ConvertAll(c => Card.GetCardValue(c)).ToList();
            List<int> allCards = new List<int>();
            allCards.AddRange(communityCardsValue);
            foreach(PlayerHand playerHand in playerHands)
            {
                List<int> playerHandCards = playerHand.cards.ConvertAll(c => Card.GetCardValue(c)).ToList();
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
                List<int> playerCardValue = hand.cards.ConvertAll(c => Card.GetCardValue(c)).ToList();
                if (playerCardValue.Contains(highCard))
                {
                    result.Add(hand.username);
                }
            }
            return result; //in this case or there is one wiiner or its a tie bettwen some playres
        }



        // Custom card comparer

        // Hand evaluation methods


        /// <summary>
        /// the function check if the player has the hand rnking of "Royal Flush" if he has it will
        /// return true otherwise false
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static bool IsRoyalFlush(List<Card> cards)
        {
            // check for a royal flush
            bool isRoyalStraight = false;
            cards = cards.Where(c => Card.GetCardValue(c) >= 10).OrderBy(c => Card.GetCardValue(c)).ToList();
            List<Card> groupeCards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();


            int count = groupeCards.Count();
            if (count < 5)
            {
                return false;
            }

            if (Card.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 1)) == 14 &&
                Card.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 2)) == 13 &&
                Card.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 3)) == 12 &&
                Card.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 4)) == 11 &&
                Card.GetCardValue(groupeCards.ElementAt(groupeCards.Count - 5)) == 10)
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
                int cardValue = Card.GetCardValue(card);

                if (cardAppearance.ContainsKey(cardValue))
                {
                    cardAppearance[cardValue] = null;
                }
                else
                {
                    cardAppearance[cardValue] = Card.GetCardType(card);
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

            List<Card> cardsWithSameType = cards.Where(card => Card.GetCardType(card).Equals(cardType)).ToList();

            if (cardsWithSameType.Count == 5)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// the function check if the player has hand ranking of "Straight Flush" if he has it will return
        /// true and the highest card in the straight flush otherwise it will return false
        /// and the lowest card by value in the game (2)
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
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
                    if (Card.GetCardType(card).Equals(cardType))
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
                if (Card.GetCardType(card).Equals(cardType))
                {
                    cardsWithSameType.Add(card);
                }
            }
            cardsWithSameType = cardsWithSameType.OrderBy(c => Card.GetCardValue2(c)).ToList();
            (highCard, isStraightFlush) = PokerRules.IsStraight(cardsWithSameType);
            return (highCard, isStraightFlush);
        }

        /// <summary>
        /// the function check if the player has hand ranking of "Four Of A Kind" if he has it will return
        /// true and the card that apper four times otherwise it will return false and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static (Card, bool) IsFourOfAKind(List<Card> cards)
        {
            // Check for four of a kind
            var groups = cards.GroupBy(c => c.nameOfCard[0]);
            Card highCard = null;
            bool isFourOfAKind = groups.Any(g => g.Count() == 4);
            if(isFourOfAKind)
            {
                for (int i = 0; i < cards.Count; i++) { 
               
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (Card.GetCardValue(highCard) == Card.GetCardValue(cards[j]))
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

        /// <summary>
        /// the function check if the player has hand ranking of "Full House" if he has it will return
        /// true, the card that appering three times and the card that apper two times otherwise
        /// it will return false, null and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
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
                    if (Card.GetCardValue(highCard1) == Card.GetCardValue(cards[j]))
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
                        if (Card.GetCardValue(highCard2) == Card.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count >= 2)
                    {
                        if (Card.GetCardValue(highCard1) != Card.GetCardValue(highCard2))
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
                        if(Card.GetCardValue(highCard1) == Card.GetCardValue(cards[j]))
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
                        if(Card.GetCardValue(highCard2) == Card.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if(count==3 && Card.GetCardValue(highCard1) != Card.GetCardValue(highCard2))
                    {
                        break;
                    }
                    highCard2 = null;
                }
                if(highCard1 != null && highCard2 != null)
                {
                    if(Card.GetCardValue(highCard1) > Card.GetCardValue(highCard2))
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
                        if (Card.GetCardValue(highCard2) == Card.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count < 3 && count >= 2)
                    {
                        if (Card.GetCardValue(highCard2) != Card.GetCardValue(highCard1))
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
                        if (Card.GetCardValue(highCard3) == Card.GetCardValue(cards[j]))
                        {
                            count++;
                        }
                    }
                    if (count < 3 && count >= 2)
                    {
                        if (Card.GetCardValue(highCard3) != Card.GetCardValue(highCard1)
                            && Card.GetCardValue(highCard3) != Card.GetCardValue(highCard2))
                        {
                            break;
                        }
                    }
                    highCard3 = new Card("2S");
                }
                highCard = highCard1;
                if(Card.GetCardValue(highCard2) > Card.GetCardValue(highCard3))
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


        /// <summary>
        /// the function check if the player has hand ranking of "Flush" if he has it will return true and the highest card in the flush
        /// otherwise it will return false and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static (Card, bool) IsFlush(List<Card> cards)
        {
            // Check for a flush
            var groups = cards.GroupBy(c => c.nameOfCard.Last());
            bool isFlush = groups.Any(g => g.Count() >= 5);
            Card highCard = null;
            if (isFlush)
            {
                cards = cards.OrderBy(c => Card.GetCardValue(c)).ToList();
                for (int i = cards.Count-1; i >=0; i--)
                {
                    int count = 0;
                    highCard = cards.ElementAt(i);
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (Card.GetCardType(highCard).Equals(Card.GetCardType(cards[j])))
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

        /// <summary>
        /// the function check if the player has hand ranking of "Straight" if he has it will return true and the highest card 
        /// in the straight otherwise it will return false and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static (Card, bool) IsStraight(List<Card> cards)
        {
            // Check for a straight 

            bool isStraight = false;
            cards = cards.OrderBy(c => Card.GetCardValue(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();
            Card highCard = null;
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (Card.GetCardValue(cards[i]) == Card.GetCardValue(cards[i + 1]) - 1
                    && Card.GetCardValue(cards[i + 1]) == Card.GetCardValue(cards[i + 2]) - 1
                    && Card.GetCardValue(cards[i + 2]) == Card.GetCardValue(cards[i + 3]) - 1
                    && Card.GetCardValue(cards[i + 3]) == Card.GetCardValue(cards[i + 4]) - 1)
                {
                    highCard = cards[i+4];
                    isStraight = true;
                    return(highCard,isStraight);
                }
            }
            cards = cards.OrderBy(c => Card.GetCardValue2(c)).ToList();
            cards = cards.GroupBy(c => c.nameOfCard[0]).Select(g => g.First()).ToList();
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (Card.GetCardValue2(cards[i]) == Card.GetCardValue2(cards[i + 1]) - 1
                    && Card.GetCardValue2(cards[i + 1]) == Card.GetCardValue2(cards[i + 2]) - 1
                    && Card.GetCardValue2(cards[i + 2]) == Card.GetCardValue2(cards[i + 3]) - 1
                    && Card.GetCardValue2(cards[i + 3]) == Card.GetCardValue2(cards[i + 4]) - 1)
                {
                    highCard = cards[i+4];
                    isStraight = true;
                    return (highCard, isStraight);
                }
            }
            return (highCard, false);

        }

        /// <summary>a
        /// the function check if the player has hand ranking of "Three Of A Kind" if he has it will return true and the card that appering
        /// three times otherwise it will return false and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
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
                        if (Card.GetCardValue(highCard) == Card.GetCardValue(cards[j]))
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


        /// <summary>
        /// the function check if the player has hand ranking of "Two Pair" if he has it will return true and the two cards that appering
        /// twice otherwise it will return false, null and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
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
                cards = cards.OrderBy(c => Card.GetCardValue(c)).ToList();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (isFirstHighCard)
                    {
                        int count = 0;
                        highCard1 = cards.ElementAt(i);
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (Card.GetCardValue(highCard1) == Card.GetCardValue(cards[j]))
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
                            if (Card.GetCardValue(highCard2) == Card.GetCardValue(cards[j]))
                            {
                                count++;
                            }
                        }
                        if (count == 2)
                        {
                            if(Card.GetCardValue(highCard2) != Card.GetCardValue(highCard1))
                            {
                                break;
                            }
                        }
                    }
                    

                }

            }
            return (highCard1, highCard2, isTwoPair);
        }


        /// <summary>
        /// the function check if the player has hand ranking of "One Pair" if he has it will return true and the card that appering\
        /// twice otherwise it will return false and null
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
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
                        if (Card.GetCardValue(highCard) == Card.GetCardValue(cards[j]))
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

