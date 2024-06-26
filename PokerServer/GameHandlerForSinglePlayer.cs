﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PokerServer
{
    /// <summary>
    /// This class holds the data for a single player in the poker game
    /// It stores handles the commands being received from the client and sends commands back to it
    /// </summary>
    public class GameHandlerForSinglePlayer 
    {
        /// <summary>
        /// the bool check if the player is still in the game
        /// </summary>
        public bool isInGame;

        /// <summary>
        /// the user id of the player
        /// </summary>
        public int userId;
        /// <summary>
        /// how much money does the player has
        /// </summary>
        public int playerMoney;
        /// <summary>
        /// the username of the player
        /// </summary>
        public string username = "";
        /// <summary>
        /// the money the player bet on
        /// </summary>
        public int betMoney = 0;
        /// <summary>
        /// the last stage that the player played in
        /// </summary>
        public Stage lastStageTheUserPlayed = Stage.NONE;
        /// <summary>
        /// the connection to the data base
        /// </summary>
        private SqlConnect sqlConnect;
        /// <summary>
        /// the connection to the client
        /// </summary>
        private PokerClientConnection pokerClientConnection;
        /// <summary>
        /// GameManager object
        /// </summary>
        private GameManager gameManager;
        /// <summary>
        /// array that contain the player starting cards
        /// </summary>
        private Card[] cards;

        /// <summary>
        /// the constructor create a new SqlConnection and stores the pokerClientConnection
        /// </summary>
        /// <param name="pokerClientConnection">The Connection that was created with the client</param>
        public GameHandlerForSinglePlayer(PokerClientConnection pokerClientConnection)
        {
            sqlConnect = new SqlConnect();
            this.pokerClientConnection = pokerClientConnection;
        }


        /// <summary>
        /// the function handle the commands that come from the client and deal with them
        /// </summary>
        /// <param name="command">The command that was received from the client</param>
        public void HandleCommand(string command)
        {
            ClientServerProtocol c1 = new ClientServerProtocol(command);
            switch (c1.command)
            {
                case Command.LOGIN:
                    this.handleLogin(c1.username, c1.password);
                    break;
                case Command.REGISTRATION:
                    this.handleRegistration(c1.username, c1.password, c1.firstName, c1.lastName, c1.email, c1.city, c1.gender);
                    return;
                case Command.FORGOT_PASSWORD:
                    this.ForgotPassword(c1.username, c1.code);
                    break;
                case Command.UPDATE_PASSWORD:
                    this.UpdatePassword(c1.username, c1.newPassword);
                    break;
                case Command.START_GAME:
                    this.gameManager.StartGame();
                    this.gameManager.nextTurn(true);
                    break;
                case Command.RAISE:
                    this.handleBetMoney(c1.betMoney);
                    break;
                case Command.CHECK:
                    this.handleCheck();
                    break;
                case Command.FOLD:
                    this.handleFold();
                    break;
            }
        }

        /// <summary>
        /// the function return the cards that this user is currently have
        /// </summary>
        /// <returns></returns>
        public string getUserCards()
        {
            return this.cards.ElementAt(0).ToString() + "," + this.cards.ElementAt(1).ToString() + ",";
        }

        /// <summary>
        /// the function handle the FOLD command when someone is folding
        /// </summary>
        private void handleFold()
        {
            this.isInGame = false;
            this.gameManager.handleRaise(0, this.username, false, false, true);
        }

        /// <summary>
        /// the function handle the CHECK command when someone press the check button in the client
        /// </summary>
        private void handleCheck()
        {
            int highestBet = this.gameManager.highestBet();
            int delta = highestBet - this.betMoney;
            if (this.playerMoney - delta < 0)
            {
                delta = this.playerMoney;
            }
            this.betMoney = highestBet;
            this.playerMoney -= delta;
            this.lastStageTheUserPlayed = this.gameManager.stage;
            this.gameManager.handleRaise(delta, this.username, false, true, false);
        }

        /// <summary>
        /// the function handle the RAISE command when someone press the bet button in the client
        /// </summary>
        /// <param name="betMoney">Amount of money</param>
        private void handleBetMoney(int betMoney)
        {
            this.betMoney += betMoney;
            this.playerMoney -= betMoney;
            this.lastStageTheUserPlayed = this.gameManager.stage;
            this.gameManager.handleRaise(betMoney, this.username, true, false, false);
        }

        /// <summary>
        /// the function handle the LOGIN command when someone is trying to login to the game
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        private void handleLogin(string username, string password)
        {
            if (GameManager.GetInstance(null).isActiveGame)
            {
                ClientServerProtocol protocol = new ClientServerProtocol();
                protocol.command = Command.ERROR;
                protocol.message = "Game in progress, please try again later :)";
                pokerClientConnection.SendMessage(protocol.generate());
                return;
            }
            string hashedPassword = GameHandlerForSinglePlayer.CreateMD5(password);
            this.userId = sqlConnect.GetUserId(username, hashedPassword);
            if (this.userId <= 0)
            {
                ClientServerProtocol protocol = new ClientServerProtocol();
                protocol.command = Command.ERROR;
                protocol.message = "Wrong username or password";
                pokerClientConnection.SendMessage(protocol.generate());
                return;
            }
            this.gameManager = GameManager.GetInstance(null);
            if (this.gameManager.IsUserAlreadyLoggedIn(this.userId))
            {
                ClientServerProtocol protocol = new ClientServerProtocol();
                protocol.command = Command.ERROR;
                protocol.message = "you already logged in";
                pokerClientConnection.SendMessage(protocol.generate());
                return;
            }
            this.gameManager = GameManager.GetInstance(this);
            this.username = username;
            this.playerMoney = 1000;
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.command = Command.SUCCES;
            clientServerProtocol.username = username;
            pokerClientConnection.SendMessage(clientServerProtocol.generate());
            string userNames = GameManager.getAllUsername();
            clientServerProtocol.command = Command.USERNAME_OF_CONNECTED_PLAYERS;
            clientServerProtocol.AllUsernames = userNames;
            pokerClientConnection.Broadcast(clientServerProtocol.generate());


        }


        /// <summary>
        /// the function handle the REGISTER command when someone is trying to register to the game
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="city"></param>
        /// <param name="gender"></param>
        private void handleRegistration(string username, string password, string firstName, string lastName
            , string email, string city, string gender)
        {
            if (GameManager.GetInstance(null).isActiveGame)
            {
                ClientServerProtocol protocol = new ClientServerProtocol();
                protocol.command = Command.ERROR;
                protocol.message = "Game in progress, please try again later :)";
                pokerClientConnection.SendMessage(protocol.generate());
                return;
            }
            if (sqlConnect.IsExist(username))
            {
                ClientServerProtocol protocol = new ClientServerProtocol();
                protocol.command = Command.ERROR;
                protocol.message = "username already exists";
                pokerClientConnection.SendMessage(protocol.generate());
                return;
            }
            this.gameManager = GameManager.GetInstance(this);
            this.username = username;
            this.playerMoney = 1000;
            string hashedPassword = GameHandlerForSinglePlayer.CreateMD5(password);
            this.userId = sqlConnect.InsertNewUser(username, hashedPassword, firstName, lastName, email
                , city, gender, 0);
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.command = Command.SUCCES;
            clientServerProtocol.username = username;
            pokerClientConnection.SendMessage(clientServerProtocol.generate());
            string userNames = GameManager.getAllUsername();
            clientServerProtocol.command = Command.USERNAME_OF_CONNECTED_PLAYERS;
            clientServerProtocol.AllUsernames = userNames;
            pokerClientConnection.Broadcast(clientServerProtocol.generate());

        }

        /// <summary>
        /// the function encode the password of the user using MD5 (hash) to protect the data base
        /// </summary>
        /// <param name="password">The password to encrypt</param>
        /// <returns>The encrypted value as a string</returns>
        public static string CreateMD5(string password)
        {
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            return BitConverter.ToString(hash);
        }

        /// <summary>
        /// the function handle the command FORGOT_PASSWORD when someone forget his password
        /// </summary>
        /// <param name="username">Username</param>
        public void ForgotPassword(string username, string code)
        {
            if (!sqlConnect.IsExist(username))
            {
                return;
            }

            string email = sqlConnect.GetEmail(username);
            this.sendMail(email, code);
        }

        /// <summary>
        /// the function handle the command UPDATE_PASSWORD when someone enter their new password after
        /// they change it
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="newPassword">The new password</param>
        public void UpdatePassword(string username, string newPassword)
        {
            string hashedPassword = GameHandlerForSinglePlayer.CreateMD5(newPassword);
            sqlConnect.UpdatePassword(username, hashedPassword);
            this.handleLogin(username, newPassword);
        }

        /// <summary>
        /// the function sending mail to user that forogt his password with new password using SMTP protocol
        /// </summary>
        /// <param name="email">Email to send to</param>
        /// <param name="password">New Password to send over email</param>
        private void sendMail(string email, string password)
        {
            // Command-line argument must be the SMTP host.
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("yudassin@gmail.com", "livv ckoy dtyo sqjp\r\n")
            };
            MailMessage msg = new System.Net.Mail.MailMessage("yudassin@gmail.com", email
                , "New Password For Poker Game", "Your code is: " + password);
            smtpClient.Send(msg);
        }


        /// <summary>
        /// the function calls the SendMessage mathod in the client connection class
        /// </summary>
        /// <param name="message">Message to send to the client</param>
        public void SendMessage(string message)
        {
            this.pokerClientConnection.SendMessage(message);
        }

        /// <summary>
        /// the function set the player cards at the beginning of the game
        /// </summary>
        /// <param name="cards2">Cards to set for the user</param>
        public void setCards(Card[] cards2)
        {
            this.cards = new Card[cards2.Length];
            for (int i = 0; i < cards2.Length; i++)
            {
                this.cards[i] = cards2[i];
            }
        }

        /// <summary>
        /// the function close and clean up this instance
        /// </summary>
        public void Close()
        {
            if (this.gameManager != null)
            {
                this.gameManager.Close(this);
            }
        }

        /// <summary>
        /// the function update the All Time Profit area in the data base for the players that didn't win
        /// </summary>
        public void UpdateMoneyWhenGameEndsForLosers()
        {

            sqlConnect.UpdateAllTimeProfitForLosers(this.userId, this.betMoney);
        }

        /// <summary>
        /// the function update the All Time Profit area in the data base for the players that won
        /// </summary>
        /// <param name="moneyOnTheTable">Total bet amount on the table</param>
        public void UpdateMoneyWhenGameEndsForWinner(int moneyOnTheTable)
        {
            sqlConnect.UpdateAllTimeProfitForWinner(this.userId, moneyOnTheTable - this.betMoney);
            this.playerMoney += moneyOnTheTable;
        }

        /// <summary>
        /// the function return the object PlayerHand that contain the cards of some player
        /// </summary>
        /// <returns>Instance of PlayerHand representing this user</returns>
        public PlayerHand GetPlayerHand()
        {
            List<Card> myCards = this.cards.OfType<Card>().ToList();
            PlayerHand hand = new PlayerHand(myCards, this.username);
            return hand;
        }

        /// <summary>
        /// the function return the value of the All Time Profit area from the data base
        /// </summary>
        /// <returns>All time profit of the user</returns>
        public int GetAllTimeProfit()
        {
            return sqlConnect.GetAllTimeProfit(this.userId);

        }

        /// <summary>
        /// the function disconect the TCP connection
        /// </summary>
        public void Disconnect()
        {
            pokerClientConnection.Close();
        }
    }


}
