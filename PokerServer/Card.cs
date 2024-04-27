using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    /// <summary>
    /// This class represents a single card
    /// </summary>
    public class Card
    {
        /// <summary>
        /// the name of the card
        /// </summary>
        public string nameOfCard { get; set; }

        

        /// <summary>
        /// the constructor store the string name of card in the varuble nameOfCard
        /// </summary>
        /// <param name="nameOfCard"></param>
        public Card(string nameOfCard)
        {
            this.nameOfCard = nameOfCard;
        }

        /// <summary>
        /// the function return the value of the card (int) - in this case the card 'A' equals to 14
        /// </summary>
        /// <param name="card">Card</param>
        /// <returns>The value of the card</returns>
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
        /// <param name="card">Card</param>
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
        /// <param name="card">Card</param>
        /// <returns></returns>
        public static string GetCardType(Card card)
        {
            string type = card.nameOfCard.Substring(card.nameOfCard.Length - 1);
            return type;
        }

        /// <summary>
        /// toString function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return nameOfCard;
        }

    }
}
