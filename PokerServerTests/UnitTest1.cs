using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerServer;
using System;
using System.Collections.Generic;

namespace PokerServerTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestIsOnePair()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("2S"));
            cards.Add(new Card("7C"));
            cards.Add(new Card("9D"));
            cards.Add(new Card("10D"));
            cards.Add(new Card("4H"));
            (Card highCard, bool result) = PokerRules.IsOnePair(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }


        [TestMethod]
        public void TestIsTwoPair()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("2S"));
            cards.Add(new Card("2C"));
            cards.Add(new Card("9D"));
            cards.Add(new Card("10D"));
            cards.Add(new Card("4H"));
            (Card highCard, bool result) = PokerRules.IsTwoPair(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }

        [TestMethod]
        public void TestIsThreeOfAKind()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("AS"));
            cards.Add(new Card("2C"));
            cards.Add(new Card("9D"));
            cards.Add(new Card("10D"));
            cards.Add(new Card("4H"));
            (Card highCard, bool result) = PokerRules.IsThreeOfAKind(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }

        [TestMethod]
        public void TestIsStraightUp()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("KD"));
            cards.Add(new Card("KS"));
            cards.Add(new Card("QC"));
            cards.Add(new Card("JD"));
            cards.Add(new Card("10D"));
            cards.Add(new Card("10H"));
            (Card highCard, bool result) = PokerRules.IsStraight(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }

        [TestMethod]
        public void TestIsStraightDown()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("2D"));
            cards.Add(new Card("3S"));
            cards.Add(new Card("3C"));
            cards.Add(new Card("3D"));
            cards.Add(new Card("4D"));
            cards.Add(new Card("5S"));
            (Card highCard, bool result) = PokerRules.IsStraight(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));


        }

        [TestMethod]
        public void TestIsFlush()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AS"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("3H"));
            cards.Add(new Card("4H"));
            cards.Add(new Card("9D"));
            cards.Add(new Card("6H"));
            cards.Add(new Card("JH"));
            (Card highCard, bool result) = PokerRules.IsFlush(cards);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsFullHouse()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("AS"));
            cards.Add(new Card("4C"));
            cards.Add(new Card("4D"));
            cards.Add(new Card("6D"));
            cards.Add(new Card("5C"));
            (Card highCard, bool result) = PokerRules.IsFullHouse(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }

        [TestMethod]
        public void TestIsFourOfAKind()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("5D"));
            cards.Add(new Card("5S"));
            cards.Add(new Card("5C"));
            cards.Add(new Card("5D"));
            cards.Add(new Card("6D"));
            cards.Add(new Card("4C"));
            (Card highCard, bool result) = PokerRules.IsFourOfAKind(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[1]));

        }

        [TestMethod]
        public void TestIsStraightFlushWithDuplicate()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("2H"));
            cards.Add(new Card("3H"));
            cards.Add(new Card("4H"));
            cards.Add(new Card("5H"));
            cards.Add(new Card("8D"));
            cards.Add(new Card("2C"));
            (Card highCard, bool result) = PokerRules.IsStraightFlush(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[0]));

        }

        [TestMethod]
        public void TestIsStraightFlushWithDuplicate2()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("4H"));
            cards.Add(new Card("4D"));
            cards.Add(new Card("7H"));
            cards.Add(new Card("5H"));
            cards.Add(new Card("6H"));
            cards.Add(new Card("8H"));
            cards.Add(new Card("2C"));
            (Card highCard, bool result) = PokerRules.IsStraightFlush(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[5]));

        }

        [TestMethod]
        public void TestIsRoyalFlush()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("KH"));
            cards.Add(new Card("QH"));
            cards.Add(new Card("JH"));
            cards.Add(new Card("10H"));
            cards.Add(new Card("8D"));
            cards.Add(new Card("2C"));
            bool result = PokerRules.IsRoyalFlush(cards);
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void TestIsRoyalFlushWithDuplicate()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("KH"));
            cards.Add(new Card("QH"));
            cards.Add(new Card("JH"));
            cards.Add(new Card("10H"));
            cards.Add(new Card("8D"));
            cards.Add(new Card("10D"));
            bool result = PokerRules.IsRoyalFlush(cards);
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void TestDetermineWinner()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("KD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("3H"));
            cardsForPlayer1.Add(new Card("5S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("4H"));
            cardsForPlayer2.Add(new Card("6S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            Assert.AreEqual(winners, null);


        }




    }
}
