using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerServer;
using System;
using System.Collections;
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
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[6]));


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
            cards.Add(new Card("2C"));
            cards.Add(new Card("3H"));
            cards.Add(new Card("4H"));
            cards.Add(new Card("5H"));
            cards.Add(new Card("8D"));
            cards.Add(new Card("2H"));
            (Card highCard, bool result) = PokerRules.IsStraightFlush(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(CardComparer.GetCardValue(highCard), CardComparer.GetCardValue(cards[4]));

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
        public void TestDetermineWinnerOnePair()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("KD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("3H"));
            cardsForPlayer1.Add(new Card("QS"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("4H"));
            cardsForPlayer2.Add(new Card("7S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player1.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);
        }

        [TestMethod]
        public void TestDetermineWinnerTwoPair()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("KD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("2S"));
            cardsForPlayer1.Add(new Card("QS"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("2C"));
            cardsForPlayer2.Add(new Card("7S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player1.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerThreeOfAKind()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("KD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("2C"));
            cardsForPlayer1.Add(new Card("2S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("7D"));
            cardsForPlayer2.Add(new Card("7S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerStraight()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("3D"));
            communityCards.Add(new Card("4C"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("8H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("5H"));
            cardsForPlayer1.Add(new Card("6S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("AH"));
            cardsForPlayer2.Add(new Card("5S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player1.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerFlush()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("KD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("KH"));
            cardsForPlayer1.Add(new Card("3H"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("QH"));
            cardsForPlayer2.Add(new Card("5H"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player1.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerFullHouse()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("2S"));
            communityCards.Add(new Card("2C"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("3H"));
            cardsForPlayer1.Add(new Card("3S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("AH"));
            cardsForPlayer2.Add(new Card("AS"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerSpecificFullHouse()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("2S"));
            communityCards.Add(new Card("2C"));
            communityCards.Add(new Card("3C"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("3H"));
            cardsForPlayer1.Add(new Card("7S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("AH"));
            cardsForPlayer2.Add(new Card("AS"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerSpecificFullHouse2()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("2S"));
            communityCards.Add(new Card("2C"));
            communityCards.Add(new Card("3C"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("3H"));
            cardsForPlayer1.Add(new Card("7S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("3D"));
            cardsForPlayer2.Add(new Card("7D"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner = null;
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }


        [TestMethod]
        public void TestDetermineWinnerFourOfAKind()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("2S"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("JH"));
            communityCards.Add(new Card("JC"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("2C"));
            cardsForPlayer1.Add(new Card("2D"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("JD"));
            cardsForPlayer2.Add(new Card("JS"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerStraightFlush()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("3H"));
            communityCards.Add(new Card("4H"));
            communityCards.Add(new Card("5H"));
            communityCards.Add(new Card("JC"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("AH"));
            cardsForPlayer1.Add(new Card("2D"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("6H"));
            cardsForPlayer2.Add(new Card("JS"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerStraightFlush2()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("3H"));
            communityCards.Add(new Card("4H"));
            communityCards.Add(new Card("5H"));
            communityCards.Add(new Card("KH"));
            communityCards.Add(new Card("JC"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("AH"));
            cardsForPlayer1.Add(new Card("2H"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("6H"));
            cardsForPlayer2.Add(new Card("7H"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerStraightFlush3()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("3H"));
            communityCards.Add(new Card("4H"));
            communityCards.Add(new Card("5H"));
            communityCards.Add(new Card("6H"));
            communityCards.Add(new Card("6D"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("6C"));
            cardsForPlayer1.Add(new Card("2H"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("4D"));
            cardsForPlayer2.Add(new Card("7H"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player2.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

        [TestMethod]
        public void TestDetermineWinnerRoyalFlush()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("AH"));
            communityCards.Add(new Card("QH"));
            communityCards.Add(new Card("10H"));
            communityCards.Add(new Card("KH"));
            communityCards.Add(new Card("JC"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("JH"));
            cardsForPlayer1.Add(new Card("7S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("6H"));
            cardsForPlayer2.Add(new Card("7H"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add(player1.username);
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }

    }
}
