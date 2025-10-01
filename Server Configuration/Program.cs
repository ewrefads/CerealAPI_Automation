// See https://aka.ms/new-console-template for more information
using CerealAPI.Models;
using CerealAPI;
using Csv;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;

namespace ServerConfiguration
{
    public class ServerConfiguration
    {
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
            CreateDB();
        }

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
                    ");", conn);
                cmd.ExecuteNonQuery();

                MySqlCommand contentCheck = new MySqlCommand("SELECT * FROM cereal", conn);
                int rowNumber = contentCheck.ExecuteNonQuery();
                conn.Close();
                if(rowNumber == 0)
                {
                    Console.WriteLine("loading data");
                    string path = Path.Combine(@"Cereal.csv");
                    InsertFromCSV(path);
                }
                else
                {
                    Console.Write("type the name of the csv file placed in tis programs folder you wish to load: ");
                    string path = Console.ReadLine();
                    while(!File.Exists(path))
                    {
                        Console.Write("File could not be found. Please make sure it is spelled correctly and placed in the same folder as this executable. Try again: ");
                        path = Console.ReadLine();
                    }
                    InsertFromCSV(path);
                }
            }
        }

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

        private void CreateUser(string username, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(64);
            byte[] saltedPassword = BCrypt.Generate(BCrypt.PasswordToByteArray(password.ToCharArray()), salt, 64);
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=cerealdb;user=root;password=test"))
            {

            }
        }
    }



}
