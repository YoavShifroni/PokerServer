using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokerServer
{
    public class PokerClientConnection
    {
        /// <summary>
        /// The Server class represents info about each client connecting to the server.
        /// </summary>

        // Store list of all clients connecting to the server
        // the list is static so all memebers of the chat will be able to obtain list
        // of current connected client
        public static Hashtable AllClients = new Hashtable();

        // information about the client
        private TcpClient _client;
        private string _clientIP;

        // used for sending and reciving data
        private byte[] data;

        public GameHandlerForSinglePlayer _handler;

        public DateTime _lastConnect;

        /// <summary>
        /// When the client gets connected to the server the server will create an instance and pass the TcpClient
        /// The constructor checks for DOS - whether that IP connected over 10 times in the past 10 seconds
        /// If all is ok - the new TCP connection is stored & a new instance of GameHandlerForSinglePlayer to handle
        /// this user/client.
        /// </summary>
        /// <param name="client"></param>
        public PokerClientConnection(TcpClient client)
        {
            int count = 0;
            _client = client;

            // get the ip address of the client to register him with our client list
            _clientIP = client.Client.RemoteEndPoint.ToString();

            foreach (DictionaryEntry c in AllClients)
            {
                string ipAndPort = (string)c.Key;
                int index = ipAndPort.LastIndexOf(':');
                string ip = ipAndPort.Substring(0, index);
                IPAddress newIp = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                if (ip.Equals(newIp.ToString()))
                {
                    DateTime time = ((PokerClientConnection)(c.Value))._lastConnect;
                    if((DateTime.Now- time).TotalSeconds < 10)
                    {
                        count++;
                    }
                }
            }
            if(count >= 10)
            {
                throw new Exception("someone trying to connect too fast (DOS)");
            }

            // Add the new client to our clients collection
            AllClients.Add(_clientIP, this);
            //UpdateClientList();
            // Read data from the client async
            data = new byte[_client.ReceiveBufferSize];

            _handler = new GameHandlerForSinglePlayer(this);

            _lastConnect = DateTime.Now;

            // BeginRead will begin async read from the NetworkStream
            // This allows the server to remain responsive and continue accepting new connections from other clients
            // When reading complete control will be transfered to the ReviveMessage() function.
            _client.GetStream().BeginRead(data,
                                          0,
                                          System.Convert.ToInt32(_client.ReceiveBufferSize),
                                          ReceiveMessage,
                                          null);
        }

        /// <summary>
        /// Allow the server to send message to the client.
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(string message)
        {
            try
            {
                System.Net.Sockets.NetworkStream ns;

                // we use lock to present multiple threads from using the networkstream object
                // this is likely to occur when the server is connected to multiple clients all of 
                // them trying to access to the networkstream at the same time.
                lock (_client.GetStream())
                {
                    ns = _client.GetStream();
                }

                // Send data to the client
                byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// Receives a message from the client and activates the command handler in GameHandlerForSinglePlayer
        /// In case the client's connection disconnects - handles it
        /// </summary>
        /// <param name="ar"></param>
        public void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                lock (_client.GetStream())
                {
                    // call EndRead to handle the end of an async read.
                    bytesRead = _client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1) // client was disconnected
                {
                    AllClients.Remove(_clientIP);
                    return;
                }
                string commandRecive = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
                Console.WriteLine(commandRecive);
                this._handler.HandleCommand(commandRecive);
                lock (_client.GetStream())
                {
                    // continue reading form the client
                    _client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                AllClients.Remove(_clientIP);
                this._handler.Close();
            }
        }

        /// <summary>
        /// send message to all the players that are connected
        /// </summary>
        /// <param name="message">Text to broadcast</param>
        public void Broadcast(string message)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                ((PokerClientConnection)(c.Value)).SendMessage(message);
            }
        }
        /// <summary>
        /// Close down the connection
        /// </summary>
        public void Close()
        {
            AllClients.Remove(_clientIP);
            _client.Close();
        }

    }
}





