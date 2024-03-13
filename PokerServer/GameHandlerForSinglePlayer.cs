using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private Card[] cards ;

        public GameHandlerForSinglePlayer(PokerClientConnection pokerClientConnection) 
        {
            gameManager = GameManager.GetInstance(this);
            sqlConnect = new SqlConnect();
            this.pokerClientConnection = pokerClientConnection;
            
        }

        public void HandleCommand(string command) 
        {
            ClientServerProtocol c1 = new ClientServerProtocol(command);
            switch (c1.command)
            {
                case Command.LOGIN:
                    this.handleLogin(c1.username, c1.password);
                    break;
                case Command.REGISTRATION:
                    this.handleRegistration(c1.username, c1.password, c1.firstName, c1.lastName, c1.email, c1.city, c1  .gender);
                    return;
                case Command.START_GAME:
                    this.gameManager.StartGame();
                    this.gameManager.nextTurn();
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




        private void handleFold()
        {
            this.isInGame = false;
            this.gameManager.nextTurn();
        }

        private void handleCheck()
        {
            int highestBet = this.gameManager.highestBet();
            int delta = highestBet - this.betMoney;
            this.betMoney = highestBet;
            this.playerMoney -= delta;
            this.lastStageTheUserPlayed = this.gameManager.stage;
            this.gameManager.handleRaise(delta, this.username);
        }

        private void handleBetMoney(int betMoney)
        {
            this.betMoney += betMoney;
            this.playerMoney -= betMoney;
            this.lastStageTheUserPlayed = this.gameManager.stage;
            this.gameManager.handleRaise(betMoney, this.username);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void handleLogin(string username, string password)
        {
            this.userId = sqlConnect.GetUserId(username, password);
            if (this.userId <= 0)
            {
                pokerClientConnection.SendMessage("Error: wrong username or password");
                return;
            }
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

        private void handleRegistration(string username, string password, string firstName, string lastName, string email, string city, string gender)
        {
            if (sqlConnect.IsExist(username))
            {
                pokerClientConnection.SendMessage("Error: This client is already registered");
                return;
            }
            this.username = username;
            this.playerMoney = 1000;
            this.userId = sqlConnect.InsertNewUser(username,password, firstName, lastName, email, city, gender, 0);
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.command = Command.SUCCES;
            clientServerProtocol.username = username;
            pokerClientConnection.SendMessage(clientServerProtocol.generate());
            string userNames =GameManager.getAllUsername();
            clientServerProtocol.command = Command.USERNAME_OF_CONNECTED_PLAYERS;
            clientServerProtocol.AllUsernames = userNames;
            pokerClientConnection.Broadcast(clientServerProtocol.generate());

        }

        

        public string getUsername()
        {
            return sqlConnect.GetUsernameFromId(this.userId);
        }

        public void SendMessage(string message)
        {
            this.pokerClientConnection.SendMessage(message);
        }

        public void setCards(Card[] cards2)
        {
            this.cards = new Card[cards2.Length];
            for(int i = 0; i<cards2.Length;i++)
            {
                this.cards[i] = cards2[i];
            }
        }

        public void Close()
        {
            this.gameManager.Close(this);
        }


        public void UpdateMoneyWhenGameEndsForLosers()
        {

            sqlConnect.UpdateAllTimeProfitForLosers(this.userId, this.betMoney);
        }

        public void UpdateMoneyWhenGameEndsForWinner(int moneyOnTheTable)
        {
            sqlConnect.UpdateAllTimeProfitForWinner(this.userId, moneyOnTheTable - this.betMoney);
        }

        public PlayerHand GetPlayerHand()
        {
            List<Card> myCards = this.cards.OfType<Card>().ToList();
            PlayerHand hand = new PlayerHand(myCards, this.username);
            return hand;
        }

        public int GetAllTimeProfit()
        {
            return sqlConnect.GetAllTimeProfit(this.userId);

        }
    }
}
