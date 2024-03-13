using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    class SqlConnect
    {
        private string connectionString;
        private SqlConnection connection;
        private SqlCommand command;

        public SqlConnect()
        {

            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Visual Studio\PokerServer\PokerServer\Database1.mdf;Integrated Security = True";
            connection = new SqlConnection(connectionString);
            command = new SqlCommand();
        }

        /// <summary>
        /// the function insert the user into the data base
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="city"></param>
        /// <param name="gender"></param>
        /// <param name="allTimeProfit"></param>
        /// <returns></returns>
        public int InsertNewUser(string username, string password, string firstName, string lastName, string email, string city, string gender, int allTimeProfit)
        {
            command.CommandText = "INSERT INTO Users VALUES('" + username + "','" + password + "','" + firstName + "','" + lastName + "','" + email + "','" + city + "','" + gender + "','" + allTimeProfit + "')";
            connection.Open();
            var x = command.ExecuteNonQuery();
            connection.Close();
            return x;
        }

        /// <summary>
        /// the function check if this username exist in the data base
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsExist(string username)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username ='" + username + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b > 0;
        }

        /// <summary>
        /// the function check if the username and the password exist in the same row in the data base
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateLogin(string username, string password)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username ='" + username + "' AND Password ='" + password + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b > 0;
        }

        /// <summary>
        /// the function return the first name of this username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetFirstName(string username)
        {
            command.CommandText = "SELECT FirstName FROM Users WHERE Username ='" + username + "'";
            connection.Open();
            command.Connection = connection;
            string b = (string)command.ExecuteScalar();
            connection.Close();
            return b;
        }

        /// <summary>
        /// the function return the user Id from his username and his password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int GetUserId(string username, string password)
        {
            command.CommandText = "SELECT Id FROM Users WHERE Username ='" + username + "' AND Password ='" + password + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b ;
        }

        /// <summary>
        /// the function return the username from this Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUsernameFromId(int id)
        {
            command.CommandText = "SELECT Username FROM Users WHERE Id =" + id;
            connection.Open();
            command.Connection = connection;
            string b = (string)command.ExecuteScalar();
            connection.Close();
            return b;
        }

        /// <summary>
        /// the function return the All Time Profit from this Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetAllTimeProfit(int id)
        {
            command.CommandText = "SELECT AllTimeProfit FROM Users WHERE Id =" + id;
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b;
        }

        /// <summary>
        /// the function update the All Time Profit when the game end for the winners
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profitFromThisGame"></param>
        public void UpdateAllTimeProfitForWinner(int id, int profitFromThisGame)
        {
            command.CommandText = "UPDATE Users SET AllTimeProfit = AllTimeProfit +" + profitFromThisGame + " WHERE Id =" + id;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// the function update the All Time Profit when the game end for the losers
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profitFromThisGame"></param>
        public void UpdateAllTimeProfitForLosers(int id, int profitFromThisGame)
        {
            command.CommandText = "UPDATE Users SET AllTimeProfit = AllTimeProfit -" + profitFromThisGame + " WHERE Id =" + id;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }






    }
}
