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

        public int InsertNewUser(string username, string password, string firstName, string lastName, string email, string city, string gender, int money)
        {
            command.CommandText = "INSERT INTO Users VALUES('" + username + "','" + password + "','" + firstName + "','" + lastName + "','" + email + "','" + city + "','" + gender + "','" + money + "')";
            connection.Open();
            var x = command.ExecuteNonQuery();
            connection.Close();
            return x;
        }

        public bool IsExist(string username)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username ='" + username + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b > 0;
        }

        public bool ValidateLogin(string username, string password)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username ='" + username + "' AND Password ='" + password + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b > 0;
        }

        public string GetFirstName(string username)
        {
            command.CommandText = "SELECT FirstName FROM Users WHERE Username ='" + username + "'";
            connection.Open();
            command.Connection = connection;
            string b = (string)command.ExecuteScalar();
            connection.Close();
            return b;
        }

        public int GetUserId(string username, string password)
        {
            command.CommandText = "SELECT Id FROM Users WHERE Username ='" + username + "' AND Password ='" + password + "'";
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b ;
        }

        public int GetUserMoney(int id)
        {
            command.CommandText = "SELECT Money FROM Users WHERE Id =" + id;
            connection.Open();
            command.Connection = connection;
            int b = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return b;
        }

        public string GetUsernameFromId(int id)
        {
            command.CommandText = "SELECT Username FROM Users WHERE Id =" + id;
            connection.Open();
            command.Connection = connection;
            string b = (string)command.ExecuteScalar();
            connection.Close();
            return b;
        }

        public void UpdateMoney(int id, int money)
        {
            command.CommandText = "UPDATE Users SET Money = " + money + " WHERE Id =" + id;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }






    }
}
