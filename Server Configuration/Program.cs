// See https://aka.ms/new-console-template for more information
using CerealAPI.Models;
using CerealAPI;
using Csv;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using BCrypt.Net;

namespace ServerConfiguration
{
    /// <summary>
    /// Class to handle various admin tasks not handled directly by the server
    /// </summary>
    public class ServerConfiguration
    {
        
        /// <summary>
        /// Sets up the server if it has not been done yet. Afterwards it takes commands to update and add users
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Starting Server configuration");
            CerealContext cerealContext = new CerealContext("server=localhost;port=3306;database=cerealdb;user=root;password=test");
            cerealContext.CreateDB(null);
            bool active = true;
            Console.WriteLine("For a list of commands type help.");
            while(active)
            {
                //Handles userinput
                Console.Write(" Enter command: ");
                string command = Console.ReadLine();
                string[] words = command.Split(" ");
                switch(words[0].ToLower())
                {
                    //Ends the program
                    case ("q"):
                    case ("quit"):
                        active = false;
                        break;
                    //Adds or updates the password of a user
                    case ("user"):
                        bool valid = true;
                        //Checks if the amount of arguments was correct
                        if(words.Length != 3)
                        {
                            valid = false;
                            Console.WriteLine("wrong number of arguments");
                        }
                        //Checks if the allready user exists
                        if (valid)
                        {
                            MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=cerealdb;user=root;password=test");
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand($"SELECT psswrd FROM users WHERE username = \"{words[1]}\"", conn);
                            MySqlDataReader reader = cmd.ExecuteReader();
                            if(reader.HasRows)
                            {
                                string hash = "";
                                while (reader.Read())
                                {
                                    hash = reader.GetString("psswrd");
                                }
                                
                                //Checks if the user knows their current password before letting the update it
                                Console.Write("existing user found. Please enter the current username to confirm password change:");
                                string password = TypedPassword();
                                int triesRemaining = 3;
                                bool tooManyTries = false;
                                while(!BCrypt.Net.BCrypt.EnhancedVerify(password, hash) && triesRemaining > 0)
                                {
                                    triesRemaining--;
                                    if(triesRemaining == 0)
                                    {
                                        Console.WriteLine("You used too many tries");
                                        valid = false;
                                    }
                                    else
                                    {
                                        Console.Write($"Wrong password. You have {triesRemaining} attempts remaining. Please try again: ");
                                        password = TypedPassword();
                                    }
                                }
                            }
                            if(valid)
                            {
                                cerealContext.CreateUser(words[1], words[2]);
                            }
                        }
                        break;
                    //Prints a list of commands and a description
                    case ("help"):
                        Console.WriteLine("user [username] [password] | Creates a new user or updates an existing users password");
                        Console.WriteLine("help                       | Prints a list of commands and their arguments");
                        Console.WriteLine("quit                       | Closes the program.");
                        
                        break;
                    default:
                        Console.WriteLine("Unkown command");
                        break;
                }
            }
        }

        /// <summary>
        /// Obscures a typed password while it is being typed
        /// </summary>
        /// <returns>The typed password</returns>
        private static string TypedPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            //Handles keypress
            while(info.Key != ConsoleKey.Enter)
            {
                //Adds the typed letter to the password
                if(info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                //Deletes the last typed letter from password and removes the * from the console
                else if(info.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    int pos = Console.CursorLeft;
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }
    }
}
