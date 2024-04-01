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
        private int moneyOnTheTable = 0;
        private bool agreeOnBet = true;
        public Stage stage;
        private int minimumBet;
        private Random rnd = new Random();

        public const int MIN_BET_FACTOR = 2;

        public static GameManager GetInstance(GameHandlerForSinglePlayer gameHandlerForSinglePlayer)
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            _allPlayers.AddLast(gameHandlerForSinglePlayer);
            return instance;
        }

        private GameManager()
        {
            this._deck = new CardDeck();
            stage = Stage.BET_AGREE_ROUND_ONE;
        }

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

        public string getAllUsernameAndTheirMoney(int userId)
        {
            string answer = "";
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if(player.userId == userId)
                {
                    continue;
                }
                answer += player.username + ",";
                answer += player.playerMoney + ",";
            }
            answer = answer.Substring(0, answer.Length - 1);
            return answer;
        }



        public void StartGame()
        {
            int count = 0;
            int dealerIndex = rnd.Next(0, _allPlayers.Count);
            this.smallBlindIndex = (dealerIndex+1)%_allPlayers.Count;
            int bigBlindIndex = (this.smallBlindIndex+1)%_allPlayers.Count;
            int playersNumber = _allPlayers.Count;
            

            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                string allUsers = this.getAllUsernameAndTheirMoney(player.userId);
                player.isInGame = true;
                ClientServerProtocol clientServerProtocol2 = new ClientServerProtocol();
                clientServerProtocol2.command = Command.START_GAME;
                clientServerProtocol2.playerMoney = player.playerMoney;
                clientServerProtocol2.allTimeProfit = player.GetAllTimeProfit();
                clientServerProtocol2.playerIndex = count;
                clientServerProtocol2.dealerIndex = dealerIndex;
                clientServerProtocol2.smallBlindIndex = this.smallBlindIndex;
                clientServerProtocol2.bigBlindIndex = bigBlindIndex;
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




        public void nextTurn(bool firstTurn)
        {
            if (firstTurn)
            {
                this._activePlayerIndex = (this.smallBlindIndex)%_allPlayers.Count;
            }
            this.agreeOnBet = true;
            int betMoney = 0;
            GameHandlerForSinglePlayer winner = this.checkIfSingleWinner();
            if(winner != null)
            {
                this.NotifyWinner(winner.username);
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

            if(agreeOnBet == true)
            {
                // Check that all players had their turn in this round at least one time
                foreach(GameHandlerForSinglePlayer player in _allPlayers)
                {
                    if(player.lastStageTheUserPlayed != this.stage)
                    {
                        this.agreeOnBet = false;
                        break;
                    }
                }
            }

            if(this.agreeOnBet == true && betMoney != 0)
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
                    break;
                }
            }

        }

        private void DecideWinner()
        {
            List<PlayerHand> playersCards = new List<PlayerHand>();
            Dictionary<string, GameHandlerForSinglePlayer> playersMap = new Dictionary<string, GameHandlerForSinglePlayer> ();
            foreach(GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.isInGame)
                {
                    playersMap.Add(player.username, player);
                    playersCards.Add(player.GetPlayerHand());
                }
            }
            List<string> theWinners = PokerRules.DetermineWinner(playersCards, communityCards);
            if(theWinners == null)
            {
                this.HandleWinner(null);
                this.NotifyWinner("It's a tie!");
                return;
            }
            string allWinnerNames = "";
            foreach(string winnerName in theWinners)
            {
                GameHandlerForSinglePlayer winner = playersMap[winnerName];
                allWinnerNames += winnerName + ", ";

                this.HandleWinner(winner);
            }
            allWinnerNames = allWinnerNames.Substring(0, allWinnerNames.Length - 2);
            this.NotifyWinner(allWinnerNames);

        }

        public void HandleWinner(GameHandlerForSinglePlayer winner)
        {
            int count = 0;
            if (winner == null)
            {
                foreach (GameHandlerForSinglePlayer player in _allPlayers)
                {
                    if (player.isInGame)
                    {
                        count++;
                    }
                }
                foreach (GameHandlerForSinglePlayer player in _allPlayers)
                {
                    if (player.isInGame)
                    {
                        player.UpdateMoneyWhenGameEndsForWinner(this.moneyOnTheTable / count);
                    }
                }
                return;
            }
            foreach (GameHandlerForSinglePlayer player in _allPlayers)
            {
                if (player.Equals(winner))
                {
                    player.UpdateMoneyWhenGameEndsForWinner(this.moneyOnTheTable);
                }
                else
                {
                    player.UpdateMoneyWhenGameEndsForLosers();
                }
            }

        }

        private void NotifyWinner(string allWinnerNames)
        {

            ClientServerProtocol clientServerProtocol = new ClientServerProtocol();
            clientServerProtocol.username = allWinnerNames;
            clientServerProtocol.command = Command.TELL_EVERYONE_WHO_WON;
            this.Brodcast(clientServerProtocol.generate());
        }

        

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
    }
}
