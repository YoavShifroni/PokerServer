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
        /// toString function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return nameOfCard;
        }

    }
}
