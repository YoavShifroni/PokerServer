using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    internal class CardDeck
    {
        private Card[] cards;
        private int index = 0;
        private Random random = new Random();

        /// <summary>
        /// the constructor create a new card deck with all the cards that are in poker
        /// </summary>
        public CardDeck()
        {
            this.cards = new Card[52];
            Card card2S = new Card("2S");
            Card card2H = new Card("2H");
            Card card2D = new Card("2D");
            Card card2C = new Card("2C");
            Card card3S = new Card("3S");
            Card card3H = new Card("3H");
            Card card3D = new Card("3D");
            Card card3C = new Card("3C");
            Card card4S = new Card("4S");
            Card card4H = new Card("4H");
            Card card4D = new Card("4D");
            Card card4C = new Card("4C");
            Card card5S = new Card("5S");
            Card card5H = new Card("5H");
            Card card5D = new Card("5D");
            Card card5C = new Card("5C");
            Card card6S = new Card("6S");
            Card card6H = new Card("6H");
            Card card6D = new Card("6D");
            Card card6C = new Card("6C");
            Card card7S = new Card("7S");
            Card card7H = new Card("7H");
            Card card7D = new Card("7D");
            Card card7C = new Card("7C");
            Card card8S = new Card("8S");
            Card card8H = new Card("8H");
            Card card8D = new Card("8D");
            Card card8C = new Card("8C");
            Card card9S = new Card("9S");
            Card card9H = new Card("9H");
            Card card9D = new Card("9D");
            Card card9C = new Card("9C");
            Card card10S = new Card("10S");
            Card card10H = new Card("10H");
            Card card10D = new Card("10D");
            Card card10C = new Card("10C");
            Card cardAS = new Card("AS");
            Card cardAH = new Card("AH");
            Card cardAD = new Card("AD");
            Card cardAC = new Card("AC");
            Card cardJS = new Card("JS");
            Card cardJH = new Card("JH");
            Card cardJD = new Card("JD");
            Card cardJC = new Card("JC");
            Card cardQS = new Card("QS");
            Card cardQH = new Card("QH");
            Card cardQD = new Card("QD");
            Card cardQC = new Card("QC");
            Card cardKS = new Card("KS");
            Card cardKH = new Card("KH");
            Card cardKD = new Card("KD");
            Card cardKC = new Card("KC");
            this.cards[0] = card2S;
            this.cards[1] = card2H;
            this.cards[2] = card2D;
            this.cards[3] = card2C;
            this.cards[4] = card3S;
            this.cards[5] = card3H;
            this.cards[6] = card3D;
            this.cards[7] = card3C;
            this.cards[8] = card4S;
            this.cards[9] = card4H;
            this.cards[10] = card4D;
            this.cards[11] = card4C;
            this.cards[12] = card5S;
            this.cards[13] = card5H;
            this.cards[14] = card5D;
            this.cards[15] = card5C;
            this.cards[16] = card6S;
            this.cards[17] = card6H;
            this.cards[18] = card6D;
            this.cards[19] = card6C;
            this.cards[20] = card7S;
            this.cards[21] = card7H;
            this.cards[22] = card7D;
            this.cards[23] = card7C;
            this.cards[24] = card8S;
            this.cards[25] = card8H;
            this.cards[26] = card8D;
            this.cards[27] = card8C;
            this.cards[28] = card9S;
            this.cards[29] = card9H;
            this.cards[30] = card9D;
            this.cards[31] = card9C;
            this.cards[32] = card10S;
            this.cards[33] = card10H;
            this.cards[34] = card10D;
            this.cards[35] = card10C;
            this.cards[36] = cardJS;
            this.cards[37] = cardJH;
            this.cards[38] = cardJD;
            this.cards[39] = cardJC;
            this.cards[40] = cardQS;
            this.cards[41] = cardQH;
            this.cards[42] = cardQD;
            this.cards[43] = cardQC;
            this.cards[44] = cardKS;
            this.cards[45] = cardKH;
            this.cards[46] = cardKD;
            this.cards[47] = cardKC;
            this.cards[48] = cardAS;
            this.cards[49] = cardAH;
            this.cards[50] = cardAD;
            this.cards[51] = cardAC;


        }

        /// <summary>
        /// the function return random card from the card deck
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Card GetRandomCard()
        {
            if(this.index== 52)
            {
                throw new Exception("something went wrong, card deck is empty");
            }
            int place =  this.random.Next(this.index,this.cards.Length);
            Card answer = this.cards[place];
            Card ezer =  this.cards[this.index];
            this.cards[this.index] = this.cards[place];
            this.cards[place] = ezer;
            this.index++;
            return answer;
        }

        /// <summary>
        /// this funciton is called when the game ended and change the value of the varuble "index" to be 0
        /// </summary>
        public void RestartGame()
        {
            this.index = 0;
        }
    }
}
