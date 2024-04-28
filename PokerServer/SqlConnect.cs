using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
    /// <summary>
    /// This class represents the data access layer - through SQL connection, storing and fetching data
    /// from the SQL database
    /// </summary>
    class SqlConnect
    {
        /// <summary>
        /// The SQL Connection object - actual connection with the SQL database
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// The constructor - creating the SQL connection with the database
        /// </summary>
        public SqlConnect()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Visual Studio\PokerServer\PokerServer\Database1.mdf;Integrated Security = True";
            connection = new SqlConnection(connectionString);
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
        public int InsertNewUser(string username, string password, string firstName, string lastName, string email,
            string city, string gender, int allTimeProfit)
        {

            string query = "INSERT INTO Users (Username, Password, FirstName, LastName, Email, City, Gender, AllTimeProfit) VALUES (@username, @password, @firstname, @lastname, @email, @city, @gender, @allTimeProfit)";
            SqlCommand command = new SqlCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@firstname", firstName);
            command.Parameters.AddWithValue("@lastname", lastName);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@gender", gender);
            command.Parameters.AddWithValue("@allTimeProfit", allTimeProfit);
            connection.Open();
            command.Connection = connection;
            int id = command.ExecuteNonQuery();
            Console.WriteLine("New Id is: " + id);
            connection.Close();
            return id;
        }

        /// <summary>
        /// the function check if this username exist in the data base
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsExist(string username)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
            command.Parameters.AddWithValue("@username", username);
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b > 0;
        }


        /// <summary>
        /// the function return the email of this username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetEmail(string username)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Email FROM Users WHERE Username  = @username";
            command.Parameters.AddWithValue("@username", username);
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
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Id FROM Users WHERE Username = @username AND Password = @password ";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
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
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Username FROM Users WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", id);
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
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT AllTimeProfit FROM Users WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", id);
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
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Users SET AllTimeProfit = AllTimeProfit + @profitFromThisGame WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@profitFromThisGame", profitFromThisGame);
            connection.Open();
            command.Connection = connection;
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
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Users SET AllTimeProfit = AllTimeProfit - @profitFromThisGame WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@profitFromThisGame", profitFromThisGame);
            connection.Open();
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// the function update the Password when someone forgot his password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="newPassword"></param>
        public void UpdatePassword(string username, string newPassword)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Users SET Password = @newPassword WHERE Username = @username ";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@newPassword", newPassword);
            connection.Open();
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
