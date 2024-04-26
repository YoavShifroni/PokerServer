using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    /// <summary>
     /// Enumeration of the different commands we're using in the client<->server protocol
    /// </summary>
    public enum Command
    {
        REGISTRATION,
        LOGIN,
        ERROR,
        FORGOT_PASSWORD,
        UPDATE_PASSWORD,
        OPEN_CARDS,
        RAISE,
        CHECK,
        FOLD,
        SUCCES,
        USERNAME_OF_CONNECTED_PLAYERS,
        START_GAME,
        SEND_STARTING_CARDS_TO_PLAYER,
        UPDATE_BET_MONEY,
        YOUR_TURN,
        NOTIFY_TURN,
        TELL_EVERYONE_WHO_WON,
        FINAL_WINNER,
    };

    /// <summary>
    /// This class represents the protocol between the server and the client
    /// it has the Command field and according to its value - we're sending and receiving
    /// fields of information
    /// </summary>
    public class ClientServerProtocol
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

        public int allTimeProfit { get; set; }

        public int playerIndex { get; set; }

        public string dealerName { get; set; }


        public string smallBlindUsername { get; set; }
        public int playersNumber { get; set; }

        public string bigBlindUsername { get; set; }

        public string allUserDetails { get; set; }

        public string raiseType { get; set; }

        public string allPlayersAndCards { get; set; }

        public string message { get; set; }

        public string oneWinnerName { get; set; }

        public string code { get; set; }

        public string newPassword { get; set; }

        /// <summary>
        /// empty constructor
        /// </summary>
        public ClientServerProtocol()
        {

        }


        /// <summary>
        /// This function generates the ClientServerProtocol instance from the string we're sending over TCP
        /// </summary>
        /// <param name="message"></param>
        public ClientServerProtocol(string message)
        {
            string[] answer = message.Split('\n');
            Enum.TryParse(answer[0], out Command cmd);
            this.command = cmd;

            switch (cmd)
            {
                case Command.ERROR:
                    this.message = answer[1];
                    break;
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
                case Command.FORGOT_PASSWORD:
                    this.username += answer[1];
                    this.code += answer[2];
                    break;
                case Command.UPDATE_PASSWORD:
                    this.username += answer[1];
                    this.newPassword = answer[2];
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
                    this.playerMoney = Convert.ToInt32(answer[1]);
                    this.allTimeProfit = Convert.ToInt32(answer[2]);
                    this.playerIndex = Convert.ToInt32(answer[3]);
                    this.dealerName = answer[4];
                    this.smallBlindUsername = answer[5];
                    this.bigBlindUsername = answer[6];
                    this.playersNumber = Convert.ToInt32(answer[7]);
                    this.allUserDetails = answer[8];
                    break;
                case Command.RAISE:
                    this.betMoney = Convert.ToInt32(answer[1]);
                    break;
                case Command.UPDATE_BET_MONEY:
                    this.betMoney = Convert.ToInt32(answer[1]);
                    this.username = answer[2];
                    this.raiseType = answer[3];
                    break;
                case Command.SUCCES:
                    this.username = answer[1];
                    break;
                case Command.NOTIFY_TURN:
                    this.username = answer[1];
                    break;
                case Command.TELL_EVERYONE_WHO_WON:
                    this.username = answer[1];
                    this.allPlayersAndCards = answer[2];
                    this.oneWinnerName = answer[3];
                    break;
                case Command.YOUR_TURN:
                    this.minimumBet = Convert.ToInt32(answer[1]);
                    break;

            }
        }


        /// <summary>
        /// This function generates the string of the protocol that we're sending over TCP
        /// from the fields of the class, based on the command
        /// </summary>
        /// <returns></returns>
        public string generate()
        {
            string answer = command.ToString() + "\n";
            switch (command)
            {
                case Command.ERROR:
                    answer += message + "\n";
                    break;
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
                case Command.FORGOT_PASSWORD:
                    answer += username + "\n";
                    answer += code + "\n";
                    break;
                case Command.UPDATE_PASSWORD:
                    answer += username + "\n";
                    answer += newPassword + "\n";
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
                    answer += this.playerIndex.ToString() + "\n";
                    answer += this.dealerName + "\n";
                    answer += this.smallBlindUsername + "\n";
                    answer += this.bigBlindUsername + "\n";
                    answer += this.playersNumber.ToString() + "\n";
                    answer += this.allUserDetails + "\n";


                    break;
                case Command.RAISE:
                    answer += this.betMoney.ToString() + "\n";
                    break;
                case Command.UPDATE_BET_MONEY:
                    answer += this.betMoney.ToString() + "\n";
                    answer += this.username + "\n";
                    answer += this.raiseType + "\n";
                    break;
                case Command.SUCCES:
                    answer += this.username + "\n";
                    break;
                case Command.NOTIFY_TURN:
                    answer += this.username + "\n";
                    break;
                case Command.TELL_EVERYONE_WHO_WON:
                    answer += this.username + "\n";
                    answer += this.allPlayersAndCards + "\n";
                    answer += this.oneWinnerName + "\n";
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
