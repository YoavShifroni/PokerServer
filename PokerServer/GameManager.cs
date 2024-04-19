using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    public enum Stage
    {
        NONE,
        BET_AGREE_ROUND_ONE,
        THREE_CARDS_OPEN,
        BET_AGREE_ROUND_TWO,
        FOUR_CARDS_OPEN,
        BET_AGREE_ROUND_THREE,
        FIVE_CARDS_OPEN,
        BET_AGREE_ROUND_FOUR,
        OPEN_PLAYERS_CARDS,
    };
    internal class GameManager
    {

        private CardDeck _deck;
        private static GameManager instance = null;
        private static LinkedList<GameHandlerForSinglePlayer> _allPlayers = new LinkedList<GameHandlerForSinglePlayer>();
        private List<Card> communityCards = new List<Card>();
        private int _activePlayerIndex = 0;
        private int smallBlindIndex;
        private int dealerIndex = -1;
        private int bigBlindIndex;
        private int moneyOnTheTable = 0;
        public Stage stage ;
        private int minimumBet;
        public bool isActiveGame = false;
        private Random rnd = new Random();

        public const int MIN_BET_FACTOR = 2;

        public static GameManager GetInstance(GameHandlerForSinglePlayer gameHandlerForSinglePlayer)
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            if(gameHandlerForSinglePlayer != null)
            {
                _allPlayers.AddLast(gameHandlerForSinglePlayer);
            }
            return instance;
        }

        private GameManager()
        {
            this._deck = new CardDeck();
            stage = Stage.BET_AGREE_ROUND_ONE;
        }

        /// <summary>
        /// the function return the cards that will send to the table
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Card[] getCardsFromTable(int length)
        {
            Card[] answer = new Card[length];
            for (int i=  0; i<length; i++)
            {
                Card c = this._deck.GetRandomCard();
                answer[i] = c;
            }
            return answer;

        }

        /// <summary>
        /// the function return string that contain all the username of connected players devide by ','
        /// </summary>
        /// <returns></returns>
        public static string getAllUsername()
        {
            string answer = "";
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                answer += player.username + ",";
            }
            answer = answer.Substring(0, answer.Length - 1);
            return answer;
        }

        /// <summary>
        /// the function return string that contain all username and their money
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string getAllUsernameAndTheirMoney(int userId)
        {
            string answer = "";
            List< GameHandlerForSinglePlayer> sort = _allPlayers.OrderBy(x => x.userId).ToList();
       
            GameHandlerForSinglePlayer myUser = sort.Find(x => x.userId == userId);
            int indexOfUserInList = sort.IndexOf(myUser);
            for(int i=1; i< _allPlayers.Count; ++i)
            {
                int index = (indexOfUserInList + i) % _allPlayers.Count;
                GameHandlerForSinglePlayer player = sort.ElementAt(index);
                answer += player.username + ",";
                answer += player.playerMoney + ",";
            }
            answer = answer.Substring(0, answer.Length - 1);
            return answer;
        }


        /// <summary>
        /// this function is responsible for starting the game, it handle all the things that we need 
        /// for the game to start and send them to the client 
        /// </summary>
        public void StartGame()
        {
            this.isActiveGame = true;
            if(this.dealerIndex == -1)
            {
                this.dealerIndex = rnd.Next(0, _allPlayers.Count);
                this.smallBlindIndex = (dealerIndex + 1) % _allPlayers.Count;
                this.bigBlindIndex = (this.smallBlindIndex + 1) % _allPlayers.Count;
            }
            int count = 0;
            int playersNumber = _allPlayers.Count;
            Console.WriteLine("length:" + playersNumber);
            string dealerName = _allPlayers.ElementAt(this.dealerIndex).username;
            string smallBlindName = _allPlayers.ElementAt(this.smallBlindIndex).username;
            string bigBlindName = _allPlayers.ElementAt(bigBlindIndex).username;

            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                string allUsers = this.getAllUsernameAndTheirMoney(player.userId);
                player.isInGame = true;
                ClientServerProtocol clientServerProtocol2 = new ClientServerProtocol();
                clientServerProtocol2.command = Command.START_GAME;
                clientServerProtocol2.playerMoney = player.playerMoney;
                clientServerProtocol2.allTimeProfit = player.GetAllTimeProfit();
                clientServerProtocol2.playerIndex = count;
                clientServerProtocol2.dealerName = dealerName;
                clientServerProtocol2.smallBlindUsername = smallBlindName;
                clientServerProtocol2.bigBlindUsername = bigBlindName;
                clientServerProtocol2.playersNumber = playersNumber;
                clientServerProtocol2.allUserDetails = allUsers;
                player.SendMessage(clientServerProtocol2.generate());

                Card[] cards = this.getCardsFromTable(2);
                player.setCards(cards);
                string[] answer = new string[2];
                for(int i = 0; i<cards.Length; i++)
                {
                    answer[i] = cards[i].nameOfCard;
                }
                ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
                clientServerProtocol.command = Command.SEND_STARTING_CARDS_TO_PLAYER;
                clientServerProtocol.cards = answer;
                player.SendMessage(clientServerProtocol.generate());
                count++;
            }


        }
        
        /// <summary>
        /// the function send the cards for the table 
        /// </summary>
        /// <param name="numberOfCards"></param>
        public void sendCardsForTable(int numberOfCards)
        {
            int place=0;
            if (this.stage.Equals(Stage.THREE_CARDS_OPEN))
            {
                place = 0;
            }
            if (this.stage.Equals(Stage.FOUR_CARDS_OPEN))
            {
                place = 3;
            }
            if (this.stage.Equals(Stage.FIVE_CARDS_OPEN))
            {
                place = 4;
            }
            Card[] answer = this.getCardsFromTable(numberOfCards);
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.command = Command.OPEN_CARDS;
            for (int i = 0; i<answer.Length; i++)
            {
                communityCards.Add(answer[i]);
                clientServerProtocol.SetOpenCard(answer[i].nameOfCard, place);
                place++;
            }
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                player.SendMessage(clientServerProtocol.generate());
            }


        }



        /// <summary>
        /// the function deal with changing the turn of the players
        /// </summary>
        /// <param name="firstTurn"></param>
        public void nextTurn(bool firstTurn)
        {
            if (firstTurn)
            {
                this._activePlayerIndex = (this.smallBlindIndex)%_allPlayers.Count;
            }
            bool agreeOnBet = true;
            int betMoney = 0;
            GameHandlerForSinglePlayer winner = this.checkIfSingleWinner();
            if(winner != null)
            {
                this.NotifyWinner(winner.username, winner.username);
                this.HandleWinner(new List<GameHandlerForSinglePlayer> {winner});
                this.RestartGame();
                return;
            }
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame && player.betMoney > 0)
                {
                    betMoney = player.betMoney;
                    break;
                }
            }
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame)
                {
                    if (betMoney != player.betMoney)
                    {
                        agreeOnBet = false;
                    }
                }
            }

            if(agreeOnBet)
            {
                // Check that all players had their turn in this round at least one time
                foreach(GameHandlerForSinglePlayer player in _allPlayers)
                {
                    if(player.lastStageTheUserPlayed != this.stage && player.isInGame)
                    {
                        agreeOnBet = false;
                        break;
                    }
                }
            }

            if(agreeOnBet  && betMoney != 0)
            {
                switch (this.stage) 
                {
                    case Stage.BET_AGREE_ROUND_ONE:
                        this.minimumBet = 0;
                        this.stage = Stage.THREE_CARDS_OPEN;
                        this.sendCardsForTable(3);
                        this.stage = Stage.BET_AGREE_ROUND_TWO;
                        break;
                    case Stage.BET_AGREE_ROUND_TWO:
                        this.minimumBet = 0;
                        this.stage = Stage.FOUR_CARDS_OPEN;
                        this.sendCardsForTable(1);
                        this.stage = Stage.BET_AGREE_ROUND_THREE;
                        break;
                    case Stage.BET_AGREE_ROUND_THREE:
                        this.minimumBet = 0;
                        this.stage = Stage.FIVE_CARDS_OPEN;
                        this.sendCardsForTable(1);
                        this.stage = Stage.BET_AGREE_ROUND_FOUR;
                        break;
                    case Stage.BET_AGREE_ROUND_FOUR:
                        this.minimumBet = 0;
                        this.DecideWinner();
                        return;
                }
            }
            for(int i = 0; i<_allPlayers.Count; i++)
            {
                GameHandlerForSinglePlayer nextPlayer = _allPlayers.ElementAt(this._activePlayerIndex);
                this._activePlayerIndex = (this._activePlayerIndex + 1) % _allPlayers.Count;
                if (nextPlayer.isInGame)
                {
                    ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
                    clientServerProtocol.minimumBet = this.minimumBet;
                    clientServerProtocol.command = Command.YOUR_TURN;
                    nextPlayer.SendMessage(clientServerProtocol.generate());

                    ClientServerProtocol clientServerProtocol1 = new ClientServerProtocol();
                    clientServerProtocol1.username = nextPlayer.username;
                    clientServerProtocol1.command = Command.NOTIFY_TURN;
                    this.Brodcast(clientServerProtocol1.generate());
                    break;
                }
            }

        }

        /// <summary>
        /// the function decide who the winner is 
        /// </summary>
        private void DecideWinner()
        {
            List<PlayerHand> playersCards = new List<PlayerHand>();
            Dictionary<string, GameHandlerForSinglePlayer> playersMap = new Dictionary<string, GameHandlerForSinglePlayer>();
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame)
                {
                    playersMap.Add(player.username, player);
                    playersCards.Add(player.GetPlayerHand());
                }
            }
            List<string> theWinners = PokerRules.DetermineWinner(playersCards, communityCards);
            if (theWinners.Count > 1)
            {
                List<GameHandlerForSinglePlayer> winners = new List<GameHandlerForSinglePlayer>();
                string nameOfWinners = "It's a tie bettwen: ";
                foreach (string s in theWinners)
                {
                    winners.Add(playersMap[s]);
                    nameOfWinners += s + ", ";
                }
                nameOfWinners.Substring(0, nameOfWinners.Length - 2);
                this.HandleWinner(winners);
                this.NotifyWinner(nameOfWinners, theWinners[0]);
                this.RestartGame();
                return;
            }
            // geting here only if there is only one winner
            string allWinnerNames = "";
            List<GameHandlerForSinglePlayer> winner = new List<GameHandlerForSinglePlayer> { playersMap[theWinners.ElementAt(0)] };
            allWinnerNames = theWinners.ElementAt(0);
            this.HandleWinner(winner);
            this.NotifyWinner(allWinnerNames, allWinnerNames);
            this.RestartGame();
            
        }

        /// <summary>
        /// the function handle the winners and the losers at the end of the game (Update All Time Profit)
        /// </summary>
        /// <param name="winner"></param>
        public void HandleWinner(List<GameHandlerForSinglePlayer> winner)
        {
            int moneyToADD = this.moneyOnTheTable / winner.Count;
           
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame)
                {
                    if (winner.Contains(player))
                    {
                        player.UpdateMoneyWhenGameEndsForWinner(moneyToADD);
                    }
                    else
                    {
                        player.UpdateMoneyWhenGameEndsForLosers();
                    }
                }
                
            }

        }

        /// <summary>
        /// the function send to all clients the winners names
        /// </summary>
        /// <param name="allWinnerNames"></param>
        private void NotifyWinner(string allWinnerNames, string oneWinnerName)
        {
            string allPlayersAndCards = "";
            foreach(GameHandlerForSinglePlayer gm in _allPlayers)
            {
                allPlayersAndCards += gm.username + "," + gm.getUserCards();
            }
            allPlayersAndCards = allPlayersAndCards.Substring(0,allPlayersAndCards.Length - 1);
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.username = allWinnerNames;
            clientServerProtocol.allPlayersAndCards = allPlayersAndCards;
            clientServerProtocol.oneWinnerName = oneWinnerName;
            clientServerProtocol.command = Command.TELL_EVERYONE_WHO_WON;
            this.Brodcast(clientServerProtocol.generate());
        }

        
        /// <summary>
        /// the function check if there is only one player that hasn't fold at specific game and if there is she 
        /// return his username
        /// </summary>
        /// <returns></returns>
        private GameHandlerForSinglePlayer checkIfSingleWinner()
        {
            int count = 0;
            GameHandlerForSinglePlayer answer = null;
            foreach(GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame)
                {
                    count++;
                    answer = player;
                }
            }
            if(count == 1)
            {
                return answer;
            }
            return null;
            
        }

        /// <summary>
        /// the function handle restart the game when the game ending
        /// </summary>
        private void RestartGame()
        {
            
            List<GameHandlerForSinglePlayer> toRemove = new List<GameHandlerForSinglePlayer>();
            foreach(GameHandlerForSinglePlayer gm in _allPlayers)
            {
                if(gm.playerMoney > 0)
                {
                    gm.betMoney = 0;
                    gm.lastStageTheUserPlayed = Stage.NONE;
                    gm.isInGame = true;
                    gm.highCard = null;
                }
                else
                {
                    toRemove.Add(gm);
                    gm.Disconnect();
                }
               
            }
            foreach(GameHandlerForSinglePlayer player in toRemove)
            {
                _allPlayers.Remove(player);
            }
            if(_allPlayers.Count == 1)
            {
                ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
                clientServerProtocol.command = Command.FINAL_WINNER;
                _allPlayers.First().SendMessage(clientServerProtocol.generate());
                this.RestartGameManager();
                return;
            }
            this._deck.RestartGame();
            stage = Stage.BET_AGREE_ROUND_ONE;
            this.dealerIndex = (this.dealerIndex + 1) % _allPlayers.Count;
            this.smallBlindIndex = (this.dealerIndex + 1) % _allPlayers.Count;
            this.bigBlindIndex = (this.smallBlindIndex + 1) % _allPlayers.Count;
            this.moneyOnTheTable = 0;
            this.minimumBet = 0;
            this.communityCards.Clear();

        }

        private void RestartGameManager()
        {
            _allPlayers.Clear();
            this.isActiveGame = false;
            this.dealerIndex = -1;
            stage = Stage.BET_AGREE_ROUND_ONE;
            this.moneyOnTheTable = 0;
            this.minimumBet = 0;
            this.communityCards.Clear();

        }

        /// <summary>
        /// the function handle the three command - RAISE, CHECK and FOLD and deal with them
        /// </summary>
        /// <param name="betMoney"></param>
        /// <param name="username"></param>
        /// <param name="isRaise"></param>
        /// <param name="isCheck"></param>
        /// <param name="isFold"></param>
        /// <exception cref="Exception"></exception>
        public void handleRaise(int betMoney, string username, bool isRaise, bool isCheck, bool isFold)
        {
            this.minimumBet = betMoney*GameManager.MIN_BET_FACTOR;
            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            if (isRaise)
            {
                clientServerProtocol.raiseType = "Raise";
            }
            else if(isCheck)
            {
                clientServerProtocol.raiseType = "Check";
            }
            else if(isFold)
            {
                clientServerProtocol.raiseType = "Fold";
            }
            else
            {
                throw new Exception("non of the action was true please check whats the problem and fix it");
            }
            clientServerProtocol.command = Command.UPDATE_BET_MONEY;
            clientServerProtocol.betMoney = betMoney;
            this.moneyOnTheTable += betMoney;
            clientServerProtocol.username = username;
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                player.SendMessage(clientServerProtocol.generate());
            }
            this.nextTurn(false);
        }

        /// <summary>
        /// the function return the highest bet that some player bet on
        /// </summary>
        /// <returns></returns>
        public int highestBet()
        {
            int highestBet = _allPlayers.First().betMoney;
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if(highestBet< player.betMoney)
                {
                    highestBet = player.betMoney;
                }
            }
            return highestBet;
        }

        /// <summary>
        /// the functon remove from the list that contain all the players that are in the game,
        /// some player that forced/ dicide to leave the game
        /// </summary>
        /// <param name="gameHandlerForSinglePlayer"></param>
        public void Close(GameHandlerForSinglePlayer gameHandlerForSinglePlayer)
        {
            _allPlayers.Remove(gameHandlerForSinglePlayer);
        }

        private void Brodcast(string text)
        {
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                player.SendMessage(text);
            }
        }

        public bool IsUserAlreadyLoggedIn(int userId)
        {
            foreach(GameHandlerForSinglePlayer gm in _allPlayers)
            {
                if(gm.userId == userId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
