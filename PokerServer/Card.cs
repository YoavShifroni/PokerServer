using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public class Card
    {
        public string nameOfCard { get; set; }

        


        public Card(string nameOfCard)
        {
            this.nameOfCard = nameOfCard;
        }
        
        public override string ToString()
        {
            return nameOfCard;
        }

    }
}
