using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    /// <summary>
    /// This class holds a data for a single user that easily represents their hand in the game
    /// </summary>
    public class PlayerHand
    {
        /// <summary>
        /// list of Cards that contain the player cards
        /// </summary>
        public List<Card> cards;
        /// <summary>
        /// username of the player
        /// </summary>
        public string username = "";
        /// <summary>
        /// hand ranking of the player
        /// </summary>
        public HandRanking handRanking;
        /// <summary>
        /// high card
        /// </summary>
        public Card highCard;
        /// <summary>
        /// second high card
        /// </summary>
        public Card secondHighCard;

        /// <summary>
        /// the constructor stores the List of Cards and the username and give the private hand
        /// </summary>
        /// <param name="cards">The user's cards</param>
        /// <param name="username">The username</param>
        public PlayerHand(List<Card> cards, string username)
        {
            this.cards = cards;
            this.username = username;
            handRanking = HandRanking.None;
        }


    }
}
