using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public class CardComparer : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            return GetCardValue(y) - GetCardValue(x);
        }

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

        public static int GetCardValueString(string card)
        {
            string value = card.Substring(0, card.Length - 1);
            switch (value)
            {
                case "A": return 14;
                case "K": return 13;
                case "Q": return 12;
                case "J": return 11;
                default: return int.Parse(value);
            }
        }

        public static string GetCardType(Card card)
        {
            string type = card.nameOfCard.Substring(card.nameOfCard.Length-1);
            return type;
        }
    }
}
