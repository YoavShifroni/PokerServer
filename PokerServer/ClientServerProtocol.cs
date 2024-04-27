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
        /// <summary>
        /// username of player
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// password of player
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// first name of player
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// last name of player
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// email of player
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// city of player
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// gender of player
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// how much money does the player has
        /// </summary>
        public int playerMoney { get; set; }

        /// <summary>
        /// how much money does the player bet on
        /// </summary>
        public int betMoney { get; set; }

        /// <summary>
        /// the command 
        /// </summary>
        public Command command { get; set; }

        /// <summary>
        /// the cards that will sent to the table
        /// </summary>
        public string[] cards = new string[5];

        /// <summary>
        /// string that contain all the usernames of connected players
        /// </summary>
        public string AllUsernames { get; set; }

        /// <summary>
        /// the minimum bet that a player can bet on 
        /// </summary>
        public int minimumBet { get; set; }

        /// <summary>
        /// the all time profit of specific player
        /// </summary>
        public int allTimeProfit { get; set; }

        /// <summary>
        /// the player index
        /// </summary>
        public int playerIndex { get; set; }

        /// <summary>
        /// the dealer username
        /// </summary>
        public string dealerName { get; set; }

        /// <summary>
        /// the small blind username
        /// </summary>
        public string smallBlindUsername { get; set; }

        /// <summary>
        /// the amount of players that are connected to the game
        /// </summary>
        public int playersNumber { get; set; }

        /// <summary>
        /// the big blind username
        /// </summary>
        public string bigBlindUsername { get; set; }

        /// <summary>
        /// string that contain all user details
        /// </summary>
        public string allUserDetails { get; set; }

        /// <summary>
        /// the raise type (Raise/ Check/ Fold)
        /// </summary>
        public string raiseType { get; set; }

        /// <summary>
        /// all players usernames and their cards
        /// </summary>
        public string allPlayersAndCards { get; set; }

        /// <summary>
        /// the error message that will sent to the client
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// the username of one of the winners
        /// </summary>
        public string oneWinnerName { get; set; }

        /// <summary>
        /// the code that will sent to a player that forgot his password to the mail
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// the new password that the client choose for himself
        /// </summary>
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
        /// <summary>
        /// the function set a card in specific place
        /// </summary>
        /// <param name="card">the card</param>
        /// <param name="index">the index to place the card</param>
        public void SetOpenCard(string card, int index)
        {
            this.cards[index] = card;
        }


    }
}



