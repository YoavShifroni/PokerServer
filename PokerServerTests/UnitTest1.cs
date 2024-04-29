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
        /// <summary>
        /// this test check if the function IsOnePair working
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[0]));

        }

        /// <summary>
        /// this test check if the function IsTwoPair working
        /// </summary>
        [TestMethod]
        public void TestIsTwoPair()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("2S"));
            cards.Add(new Card("2C"));
            cards.Add(new Card("9D"));
            cards.Add(new Card("9C"));
            cards.Add(new Card("4H"));
            (Card highCard1, Card highCard2, bool result) = PokerRules.IsTwoPair(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(Card.GetCardValue(highCard1), Card.GetCardValue(cards[0]));
            Assert.AreEqual(Card.GetCardValue(highCard2), Card.GetCardValue(cards[4]));

        }


        /// <summary>
        /// this test check if the function IsThreeOfAKind working
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[0]));

        }

        /// <summary>
        /// this test check if the function IsStright working in case of A = 14
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[0]));

        }


        /// <summary>
        /// this test check if the function IsStraight working in case of A = 1
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[6]));


        }

        /// <summary>
        /// this test check if the function IsFlush working
        /// </summary>
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

        /// <summary>
        /// this test check if the function IsFullHouse working
        /// </summary>
        [TestMethod]
        public void TestIsFullHouse()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card("AH"));
            cards.Add(new Card("AD"));
            cards.Add(new Card("AS"));
            cards.Add(new Card("4C"));
            cards.Add(new Card("4D"));
            cards.Add(new Card("6S"));
            cards.Add(new Card("6C"));
            (Card highCard, Card secondHighCard, bool result) = PokerRules.IsFullHouse(cards);
            Assert.IsTrue(result);
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[0]));
            Assert.AreEqual(Card.GetCardValue(secondHighCard), Card.GetCardValue(cards[5]));

        }

        /// <summary>
        /// this test check if the function IsFourOfAKind working
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[1]));

        }

        /// <summary>
        /// this test check if the function IsStraightFlush working in case of duplicate card that apper
        /// in the stright and when A = 1
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[4]));

        }

        /// <summary>
        /// this test check if the function IsStraightFlush working in case of duplicate card that apper
        /// in the stright 
        /// </summary>
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
            Assert.AreEqual(Card.GetCardValue(highCard), Card.GetCardValue(cards[5]));

        }

        /// <summary>
        /// this test check if the function IsRoyalFlush working 
        /// </summary>
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

        /// <summary>
        /// this test check if the function IsRoyalFlush working in case of duplicate card that apper in the straight
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of OnePair
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of TwoPair
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of TwoPair but when there is already a TwoPair in the community cards
        /// </summary>
        [TestMethod]
        public void TestDetermineWinnerTwoPair2()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("QD"));
            communityCards.Add(new Card("QC"));
            communityCards.Add(new Card("2D"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("7S"));
            cardsForPlayer1.Add(new Card("3S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("4C"));
            cardsForPlayer2.Add(new Card("3H"));
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


        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of ThreeOfAKind 
        /// </summary>
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


        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of Straight
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of Flush
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of FullHouse 
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of FullHouse but when both player have the same card that apper three time but different card that
        /// apper two times
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of FullHouse but when there is already a TwoPair in the community cards
        /// </summary>
        [TestMethod]
        public void TestDetermineWinnerFullHouse2()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("AH"));
            communityCards.Add(new Card("AS"));
            communityCards.Add(new Card("2C"));
            communityCards.Add(new Card("2H"));
            communityCards.Add(new Card("7H"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("AC"));
            cardsForPlayer1.Add(new Card("3S"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("AD"));
            cardsForPlayer2.Add(new Card("2S"));
            PlayerHand player1 = new PlayerHand(cardsForPlayer1, "yoav");
            PlayerHand player2 = new PlayerHand(cardsForPlayer2, "roy");
            List<string> nameOfWinner = new List<string>();
            nameOfWinner.Add("yoav");
            nameOfWinner.Add("roy");
            List<PlayerHand> playersCards = new List<PlayerHand>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            List<string> winners = PokerRules.DetermineWinner(playersCards, communityCards);
            CollectionAssert.AreEqual(winners, nameOfWinner, StructuralComparisons.StructuralComparer);

        }



        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of FourOfAKind 
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of StraightFlush
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of StraightFlush but both players have 6 cards from the same type
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of StraightFlush but when there is a duplicate of one of the cards that apper in the straight
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of RoyalFlush 
        /// </summary>
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

        /// <summary>
        /// this test check if the function DetermineWinner is return the right winner name in case of two players
        /// with hand ranking of OnePair, the function check who has the highest high card
        /// </summary>
        [TestMethod]
        public void TestSpecificCase()
        {
            List<Card> communityCards = new List<Card>();
            communityCards.Add(new Card("5S"));
            communityCards.Add(new Card("QH"));
            communityCards.Add(new Card("6C"));
            communityCards.Add(new Card("JS"));
            communityCards.Add(new Card("6D"));
            List<Card> cardsForPlayer1 = new List<Card>();
            cardsForPlayer1.Add(new Card("AD"));
            cardsForPlayer1.Add(new Card("4C"));
            List<Card> cardsForPlayer2 = new List<Card>();
            cardsForPlayer2.Add(new Card("2D"));
            cardsForPlayer2.Add(new Card("9H"));
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
