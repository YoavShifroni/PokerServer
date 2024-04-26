using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public class PlayerHand
    {
        public List<Card> cards;
        public string username = "";
        public HandRanking handRanking;
        public Card highCard;
        public Card secondHighCard;

        /// <summary>
        /// the constructor stores the List of Cards and the username and give the private hand
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="username"></param>
        public PlayerHand(List<Card> cards, string username)
        {
            this.cards = cards;
            this.username = username;
            handRanking = HandRanking.None;
        }


    }
}
