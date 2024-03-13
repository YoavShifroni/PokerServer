using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public enum Command
    {
        REGISTRATION,
        LOGIN,
        OPEN_CARDS  ,
        RAISE   ,
        CHECK,
        FOLD,
        SUCCES,
        USERNAME_OF_CONNECTED_PLAYERS,
        START_GAME,
        SEND_STARTING_CARDS_TO_PLAYER,
        UPDATE_BET_MONEY,
        YOUR_TURN,
        TELL_EVERYONE_WHO_WON,
    };

    internal class ClientServerProtocol
    {
        public string username { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public string gender { get; set; }

        public int playerMoney { get; set; }

        public int betMoney { get; set; }
        public Command command { get; set; }

        public string[] cards = new string[5];

        public string AllUsernames { get; set; }

        public int minimumBet { get; set; }

        public int allTimeProfit {  get; set; }




        /// <summary>
        /// 
        /// </summary>
        public ClientServerProtocol()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ClientServerProtocol(string message)
        {
            string[] answer = message.Split('\n');
            Enum.TryParse(answer[0], out Command cmd);
            this.command = cmd;

            switch (cmd)
            {
                case Command.LOGIN:
                    this.username = answer[1];
                    this.password = answer[2];
                    break;
                case Command.REGISTRATION:
                    this.username = answer[1];
                    this.password = answer[2];
                    this.firstName = answer[3];
                    this.lastName = answer[4];
                    this.email = answer[5];
                    this.city = answer[6];
                    this.gender = answer[7];
                    break;
                case Command.SEND_STARTING_CARDS_TO_PLAYER:
                case Command.OPEN_CARDS:
                    for (int i = 1; i < answer.Length - 1; i++)
                    {
                        this.cards[i - 1] = answer[i];
                    }

                    break;
                case Command.USERNAME_OF_CONNECTED_PLAYERS:
                    this.AllUsernames = answer[1];
                    break;
                case Command.START_GAME:
                    try
                    {
                        this.playerMoney = Convert.ToInt32(answer[1]);
                        this.allTimeProfit = Convert.ToInt32(answer[2]);
                    }
                    catch
                    {

                    }
                    break;
                case Command.RAISE:
                    this.betMoney = Convert.ToInt32(answer[1]);
                    break;
                case Command.UPDATE_BET_MONEY:
                    this.betMoney = Convert.ToInt32(answer[1]);
                    this.username = answer[2];
                    break;
                case Command.SUCCES:
                    this.username = answer[1];
                    break;
                case Command.TELL_EVERYONE_WHO_WON:
                    this.username = answer[1];
                    break;
                case Command.YOUR_TURN:
                    this.minimumBet = Convert.ToInt32(answer[1]);
                    break;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string generate()
        {
            string answer = command.ToString() + "\n";
            switch (command)
            {
                case Command.LOGIN:
                    answer += username + "\n";
                    answer += password + "\n";
                    break;
                case Command.REGISTRATION:
                    answer += username + "\n";
                    answer += password + "\n";
                    answer += firstName + "\n";
                    answer += lastName + "\n";
                    answer += email + "\n";
                    answer += city + "\n";
                    answer += gender + "\n";
                    break;
                case Command.SEND_STARTING_CARDS_TO_PLAYER:
                case Command.OPEN_CARDS:
                    for (int i = 0; i < this.cards.Length; i++)
                    {
                        answer += cards[i] + "\n";
                    }
                    break;
                case Command.USERNAME_OF_CONNECTED_PLAYERS:
                    answer += this.AllUsernames + "\n";
                    break;
                case Command.START_GAME:
                    answer += this.playerMoney.ToString() + "\n";
                    answer += this.allTimeProfit.ToString() + "\n";
                    break;
                case Command.RAISE:
                    answer += this.betMoney.ToString() + "\n";
                    break;
                case Command.UPDATE_BET_MONEY:
                    answer += this.betMoney.ToString() + "\n";
                    answer += this.username + "\n";
                    break;
                case Command.SUCCES:
                    answer += this.username + "\n";
                    break;
                case Command.TELL_EVERYONE_WHO_WON:
                    answer += this.username + "\n";
                    break;
                case Command.YOUR_TURN:
                    answer += this.minimumBet.ToString() + "\n";
                    break;

            }
            answer += "\r\n";
            return answer;
        }

        public void SetOpenCard(string card, int index)
        {
            this.cards[index] = card;
        }










    }
}
