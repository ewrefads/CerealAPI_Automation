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
        /// Sets up the server if it has not been done yet. Afterwards it takes commands to update and add users and insert new data from CSV files
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Starting Server configuration");
            CreateDB();
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
                    //Parses a given CSV file
                    case ("parse"):
                        bool valid = true;
                        //Checks if the amount of arguments was correct
                        if(words.Length != 2)
                        {
                            valid = false;
                            Console.WriteLine("wrong number of arguments");
                        }
                        //Checks if the file exists
                        if(valid && !File.Exists(words[1]))
                        {
                            Console.WriteLine("Could not locate file");
                            valid = false;
                        }
                        //Parses the files
                        if(valid)
                        {
                            InsertFromCSV(words[1]);
                        }
                        break;
                    //Adds or updates the password of a user
                    case ("user"):
                        valid = true;
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
                                CreateUser(words[1], words[2]);
                            }
                        }
                        break;
                    //Prints a list of commands and a description
                    case ("help"):
                        Console.WriteLine("parse [filename.csv]       | Parses data from a given csv file into the cereal table");
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

        /// <summary>
        /// Creates the database. See the same method in cerealcontext for more info
        /// </summary>
        private static void CreateDB()
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;user=root;password=test"))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "CREATE DATABASE IF NOT EXISTS cerealdb;\r\n" +
                    "USE cerealdb;\r\n" +
                    "CREATE TABLE IF NOT EXISTS cereal (\r\n\t" +
                        "id INT UNIQUE,\r\n    " +
                        "cereal_name VARCHAR(255),\r\n    " +
                        "mfr CHAR(1),\r\n    " +
                        "cereal_type CHAR(1),\r\n    " +
                        "calories INT,\r\n    " +
                        "protein INT,\r\n    " +
                        "fat INT,\r\n    " +
                        "sodium INT,\r\n    " +
                        "fiber FLOAT,\r\n    " +
                        "carbo FLOAT,\r\n    " +
                        "sugars INT,\r\n    " +
                        "potass INT,\r\n    " +
                        "vitamins INT,\r\n    " +
                        "shelf INT,\r\n    " +
                        "weight FLOAT,\r\n    " +
                        "cups FLOAT,\r\n    " +
                        "rating VARCHAR(255),\r\n    " +
                        "UNIQUE KEY unique_name_mfr(cereal_name, mfr)\r\n" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS users (\r\n\t" +
                        "username VARCHAR(255) UNIQUE NOT NULL,\r\n    " +
                        "psswrd VARCHAR(255) NOT NULL\r\n);\r\n\r\n" +
                    "CREATE TABLE IF NOT EXISTS api_log (\r\n\t" +
                        "acces_time VARCHAR(255),\r\n    " +
                        "command VARCHAR(255),\r\n    " +
                        "arguments VARCHAR(255),\r\n    " +
                        "result VARCHAR(255)\r\n" +
                    ")"
                    , conn);
                cmd.ExecuteNonQuery();
                cmd = new MySqlCommand("SELECT * FROM users", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if(!reader.HasRows)
                {
                    reader.Close();
                    CreateUser("admin", "test");
                }
                else
                {
                    reader.Close();
                }
                
                MySqlCommand contentCheck = new MySqlCommand("SELECT * FROM cereal", conn);
                MySqlDataReader contentReader = contentCheck.ExecuteReader();

                bool hasRows = contentReader.HasRows;
                conn.Close();
                if (!hasRows)
                {
                    Console.WriteLine("loading data");
                    string path = Path.Combine(@"Cereal.csv");
                    InsertFromCSV(path);
                }
            }
        }

        /// <summary>
        /// Inserts data into the database from a given CSV file
        /// </summary>
        /// <param name="filepath">The location of the CSV file</param>
        /// <exception cref="Exception">An Exception is thrown if there was any issues with the file</exception>
        public static void InsertFromCSV(string filepath)
        {
            try
            {
                var csv = File.ReadAllText(filepath);
                foreach (var line in CsvReader.ReadFromText(csv))
                {
                    if (line[0].Length > 0)
                    {
                        Cereal cereal = new Cereal();
                        cereal.Name = line[0];
                        cereal.MFR = line[1][0];
                        cereal.Type = line[2][0];
                        cereal.Calories = int.Parse(line[3]);
                        cereal.Protein = int.Parse(line[4]);
                        cereal.Fat = int.Parse(line[5]);
                        cereal.Sodium = int.Parse(line[6]);
                        cereal.Fiber = float.Parse(line[7]);
                        cereal.Carbo = float.Parse(line[8]);
                        cereal.Sugars = int.Parse(line[9]);
                        cereal.Potass = int.Parse(line[10]);
                        cereal.Vitamins = int.Parse(line[11]);
                        cereal.Shelf = int.Parse(line[12]);
                        cereal.Weight = float.Parse(line[13]);
                        cereal.Cups = float.Parse(line[14]);
                        cereal.Rating = line[15];
                        AddCereal(cereal);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                throw new Exception("File could not be loaded");
            }

        }

        /// <summary>
        /// Adds a cereal to the server. See similar method in CerealContext for more info
        /// </summary>
        /// <param name="cereal"></param>
        public static void AddCereal(Cereal cereal)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=cerealdb;user=root;password=test"))
            {
                conn.Open();
                MySqlCommand dbCmd = new MySqlCommand("USE cerealdb", conn);
                dbCmd.ExecuteNonQuery();
                MySqlCommand cmd = new MySqlCommand($"SELECT id FROM cereal ORDER BY id DESC LIMIT 1", conn);
                MySqlCommand containsCommand = new MySqlCommand($"SELECT id FROM cereal WHERE cereal_name = \"{cereal.Name}\" AND mfr = \"{cereal.MFR}\"", conn);
                MySqlDataReader containsReader = containsCommand.ExecuteReader();
                bool contains = containsReader.HasRows;
                containsReader.Close();
                if (!contains)
                {
                    
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int id = 0;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                id = reader.GetInt32("Id") + 1;
                            }
                        }
                        reader.Close();
                        MySqlCommand insertCmd = new MySqlCommand($"INSERT INTO cereal VALUES({id}, \"{cereal.Name}\", \"{cereal.MFR.ToString()}\", \"{cereal.Type.ToString()}\", {cereal.Calories}, {cereal.Protein}, {cereal.Fat}, {cereal.Sodium}, {cereal.Fiber}, {cereal.Carbo}, {cereal.Sugars}, {cereal.Potass}, {cereal.Vitamins}, {cereal.Shelf}, {cereal.Weight}, {cereal.Cups}, \"{cereal.Rating}\")", conn);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    MySqlCommand updateCommand = new MySqlCommand($"UPDATE cereal SET cereal_type = \"{cereal.Type.ToString()}\" calories = {cereal.Calories} protein = {cereal.Protein} fat = {cereal.Fat} sodium = {cereal.Sodium} fiber = {cereal.Fiber} carbo = {cereal.Carbo} sugars = {cereal.Sugars} potass = {cereal.Potass} vitamins = {cereal.Vitamins} shelf = {cereal.Shelf} weight = {cereal.Weight} cups = {cereal.Cups} rating = {cereal.Rating} WHERE cereal_name = \"{cereal.Name}\" AND mfr = \"{cereal.MFR}\"", conn);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds a user updates their password. See method in CerealContext
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private static void CreateUser(string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=cerealdb;user=root;password=test"))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE username = \"{username}\"", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                bool contains = reader.HasRows;
                reader.Close();
                
                cmd.ExecuteNonQuery();
                if(!contains)
                {
                    cmd = new MySqlCommand($"INSERT INTO users VALUES(\"{username}\", \"{hashedPassword}\")", conn);
                }
                else
                {
                    cmd = new MySqlCommand($"UPDATE users SET psswrd = \"{hashedPassword}\" WHERE username = \"{username}\"", conn);
                }
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
