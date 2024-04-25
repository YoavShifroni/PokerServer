using System;
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

    public class GameHandlerForSinglePlayer
    {

        public bool isInGame;
        public Card highCard = null;
        public int userId;
        public int playerMoney;
        public string username = "";
        public int betMoney = 0;

        public Stage lastStageTheUserPlayed = Stage.NONE;
        private SqlConnect sqlConnect;
        private PokerClientConnection pokerClientConnection;
        private GameManager gameManager;
        private Card[] cards;

        /// <summary>
        /// the constructor create a new SqlConnection and stores the pokerClientConnection
        /// </summary>
        /// <param name="pokerClientConnection"></param>
        public GameHandlerForSinglePlayer(PokerClientConnection pokerClientConnection)
        {
            sqlConnect = new SqlConnect();
            this.pokerClientConnection = pokerClientConnection;

        }


        /// <summary>
        /// the function handle the commands that come from the client and deal with them
        /// </summary>
        /// <param name="command"></param>
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
                    this.ForgotPassword(c1.username);
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
            if(this.playerMoney - delta < 0)
            {
                delta = this.playerMoney;
            }
            this.betMoney = highestBet;
            this.playerMoney -= delta;
            this.lastStageTheUserPlayed = this.gameManager.stage;
            this.gameManager.handleRaise(delta, this.username, false, true, false);
        }

        /// <summary>
        /// the function deal with the amount of money that the user just bet
        /// </summary>
        /// <param name="betMoney"></param>
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
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        private void handleRegistration(string username, string password, string firstName, string lastName, string email, string city, string gender)
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
            string hashedPassword =  GameHandlerForSinglePlayer.CreateMD5(password);
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
        /// <param name="password"></param>
        /// <returns></returns>
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
        /// <param name="username"></param>
        public void ForgotPassword(string username)
        {
            if(!sqlConnect.IsExist(username)) {
                return;
            }
            string newPassword = this.randomPassword();
            string hashedPassword = GameHandlerForSinglePlayer.CreateMD5(newPassword);
            sqlConnect.UpdatePassword(username, hashedPassword);
            string email = sqlConnect.GetEmail(username);
            this.sendMail(email, newPassword);
        }

        /// <summary>
        /// the function return random password that contain at least one capital letter
        /// , one small letter, one digit and one specific sign
        /// </summary>
        /// <returns></returns>
        public string randomPassword()
        {
            var charsALL = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz#?!@$%^&*-";
            Random rnd = new Random();
            var randomIns = new Random();
            int N = rnd.Next(5, 10);
            var rndChars = Enumerable.Range(0, N)
                            .Select(_ => charsALL[randomIns.Next(charsALL.Length)])
                            .ToArray();
            rndChars[N-1] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[randomIns.Next(24)];
            rndChars[N-2] = "abcdefghijklmnopqrstuvwxyz"[randomIns.Next(24)];
            rndChars[N-3] = "0123456789"[randomIns.Next(10)];
            rndChars[N-4] = "#?!@$%^&*-"[randomIns.Next(10)];
            return new string(rndChars);
        }

        /// <summary>
        /// the function sending mail to user that forogt his password with new password using SMTP protocol
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
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
                , "New Password For Poker Game" , "Your new password is: " + password);
            smtpClient.Send(msg);
        }

        /// <summary>
        /// the function return the username of this player
        /// </summary>
        /// <returns></returns>
        public string getUsername()
        {
            return sqlConnect.GetUsernameFromId(this.userId);
        }

        /// <summary>
        /// the function calls the SendMessage mathod in the client connection class
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            this.pokerClientConnection.SendMessage(message);
        }

        /// <summary>
        /// the function set the player cards at the beginning of the game
        /// </summary>
        /// <param name="cards2"></param>
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
            if(this.gameManager != null)
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
        /// <param name="moneyOnTheTable"></param>
        public void UpdateMoneyWhenGameEndsForWinner(int moneyOnTheTable)
        {
            sqlConnect.UpdateAllTimeProfitForWinner(this.userId, moneyOnTheTable - this.betMoney);
            this.playerMoney += moneyOnTheTable;
        }

        /// <summary>
        /// the function return the object PlayerHand that contain the cards of some player
        /// </summary>
        /// <returns></returns>
        public PlayerHand GetPlayerHand()
        {
            List<Card> myCards = this.cards.OfType<Card>().ToList();
            PlayerHand hand = new PlayerHand(myCards, this.username);
            return hand;
        }

        /// <summary>
        /// the function return the value of the All Time Profit area from the data base
        /// </summary>
        /// <returns></returns>
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
