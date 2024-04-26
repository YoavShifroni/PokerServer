using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public class CardComparer : IComparer<Card>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Card x, Card y)
        {
            return GetCardValue(y) - GetCardValue(x);
        }

        /// <summary>
        /// the function return the value of the card (int) - in this case the card 'A' equals to 14
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static int GetCardValue(Card card)
        {
            string value = card.nameOfCard.Substring(0, card.nameOfCard.Length - 1);
            switch (value)
            {
                case "A": return 14;
                case "K": return 13;
                case "Q": return 12;
                case "J": return 11;
                default: return int.Parse(value);
            }
        }

        /// <summary>
        /// the function return the value of the card (int) - in this case the card 'A' equals to 1
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static int GetCardValue2(Card card)
        {
            string value = card.nameOfCard.Substring(0, card.nameOfCard.Length - 1);
            switch (value)
            {
                case "A": return 1;
                case "K": return 13;
                case "Q": return 12;
                case "J": return 11;
                default: return int.Parse(value);
            }
        }

        /// <summary>
        /// the function return the type of the card string (Diamonds/ Hearts/ Clubs/ Spades)
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static string GetCardType(Card card)
        {
            string type = card.nameOfCard.Substring(card.nameOfCard.Length-1);
            return type;
        }
    }
}
