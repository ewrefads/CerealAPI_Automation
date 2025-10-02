using Csv;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
namespace CerealAPI.Models
{
    public class CerealContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public CerealContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            if(DatabaseName != null && DatabaseName != "")
            {
                ConnectionString = $"server=localhost;port=3306;database={DatabaseName};user=root;password=test";
                DatabaseName = "";
            }
            return new MySqlConnection(ConnectionString);
        }

        public List<Cereal> GetAllCereals(string? sort)
        {
            List<Cereal> list = new List<Cereal>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string cmdText = "SELECT * FROM cereal";
                if(sort != null)
                {
                    string[] sortWords = sort.Split('_');
                    cmdText += " ORDER BY " + CollumnTranslate(sortWords[0]);
                    if(sortWords.Length > 1)
                    {
                        cmdText += " " + sortWords[1];
                    }
                }
                MySqlCommand cmd = new MySqlCommand(cmdText, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Cereal()
                        { 
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("cereal_name"),
                            MFR =  reader.GetChar("mfr"),
                            Type = reader.GetChar("cereal_type"),
                            Calories = reader.GetInt32("calories"),
                            Protein = reader.GetInt32("protein"),
                            Fat = reader.GetInt32("fat"),
                            Sodium = reader.GetInt32("sodium"),
                            Fiber = reader.GetFloat("fiber"),
                            Carbo = reader.GetFloat("carbo"),
                            Sugars = reader.GetInt32("sugars"),
                            Potass = reader.GetInt32("potass"),
                            Vitamins = reader.GetInt32("vitamins"),
                            Shelf = reader.GetInt32("shelf"),
                            Weight = reader.GetFloat("weight"),
                            Cups = reader.GetFloat("cups"),
                            Rating = reader.GetString("rating")
                        });
                    }
                }
            }
            return list;
        }

        public bool DeleteCereal(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM cereal WHERE id = {id}", conn);
                int amount = deleteCommand.ExecuteNonQuery();
                conn.Close();
                if (amount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool ContainsId(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT id from cereal WHERE id = {id}", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                bool contains = reader.HasRows;
                reader.Close();
                return contains;
            }
        }

        private string CollumnTranslate(string collumn)
        {
            switch(collumn.ToLower())
            {
                case ("name"):
                    return "cereal_name";
                case ("manufacturer"):
                    return "mfr";
                case ("type"):
                    return "cereal_type";
                default:
                    return collumn;
            }
        }
        public List<Cereal> GetFilteredCereals(string? name, string? mfr, string? type, string? calories, string? protein, string? fat, string? sodium, string? fiber, string? carbo, string? sugars, string? potass, string? vitamins, string? shelf, string? weight, string? cups, string? rating, string? sort)
        {
            List<Cereal> filteredList = new List<Cereal>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string filter = "";
                if(name != null)
                {
                    filter = AddStringFilter(filter, "cereal_name", name);
                }
                if(mfr != null)
                {
                    if(mfr.Length > 1)
                    {
                        switch(mfr.ToLower())
                        {
                            case "american home food products":
                            case "ahfd":
                                mfr = "A";
                                break;
                            case "general mills":
                            case "gm":
                                mfr = "G";
                                break;
                            case "kellogs":
                                mfr = "K";
                                break;
                            case "nabisco":
                                mfr = "N";
                                break;
                            case "post":
                                mfr = "P";
                                break;
                            case "quaker oats":
                            case "qo":
                                mfr = "Q";
                                break;
                            case "raiston purina":
                            case "rp":
                                mfr = "R";
                                break;
                        }
                    }
                    filter = AddStringFilter(filter, "mfr", mfr);
                }
                if(type != null)
                {
                    if (type.Length > 1)
                    {
                        if (type.ToLower() == "cold")
                        {
                            type = "C";
                        }
                        else if (type.ToLower() == "hot")
                        {
                            type = "H";
                        }
                    }
                    filter = AddStringFilter(filter, "cereal_type", type);
                }
                if(calories != null)
                {
                    filter = AddNumericFilter(filter, "calories", calories);
                }
                if (protein != null)
                {
                    filter = AddNumericFilter(filter, "protein", protein);
                }
                if (fat != null)
                {
                    filter = AddNumericFilter(filter, "fat", fat);
                }
                if (sodium != null)
                {
                    filter = AddNumericFilter(filter, "sodium", sodium);
                }
                if (fiber != null)
                {
                    filter = AddNumericFilter(filter, "fiber", fiber);
                }
                if (carbo != null)
                {
                    filter = AddNumericFilter(filter, "carbo", carbo);
                }
                if (sugars != null)
                {
                    filter = AddNumericFilter(filter, "sugars", sugars);
                }
                if (potass != null)
                {
                    filter = AddNumericFilter(filter, "potass", potass);
                }
                if (vitamins != null)
                {
                    filter = AddNumericFilter(filter, "vitamins", vitamins);
                }
                if (shelf != null)
                {
                    filter = AddNumericFilter(filter, "shelf", shelf);
                }
                if (weight != null)
                {
                    filter = AddNumericFilter(filter, "weight", weight);
                }
                if (cups != null)
                {
                    filter = AddNumericFilter(filter, "cups", cups);
                }
                if(rating != null)
                {
                    filter = AddStringFilter(filter, "rating", rating);
                }
                if (sort != null)
                {
                    string[] sortWords = sort.Split('_');
                    filter += " ORDER BY " + CollumnTranslate(sortWords[0]);
                    if (sortWords.Length > 1)
                    {
                        filter += " " + sortWords[1];
                    }
                }
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM cereal WHERE {filter}", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        filteredList.Add(new Cereal()
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("cereal_name"),
                            MFR = reader.GetChar("mfr"),
                            Type = reader.GetChar("cereal_type"),
                            Calories = reader.GetInt32("calories"),
                            Protein = reader.GetInt32("protein"),
                            Fat = reader.GetInt32("fat"),
                            Sodium = reader.GetInt32("sodium"),
                            Fiber = reader.GetFloat("fiber"),
                            Carbo = reader.GetFloat("carbo"),
                            Sugars = reader.GetInt32("sugars"),
                            Potass = reader.GetInt32("potass"),
                            Vitamins = reader.GetInt32("vitamins"),
                            Shelf = reader.GetInt32("shelf"),
                            Weight = reader.GetFloat("weight"),
                            Cups = reader.GetFloat("cups"),
                            Rating = reader.GetString("rating")
                        });
                    }
                }
            }
            return filteredList;
        }

        private string AddNumericFilter(string filter, string collumn, string value)
        {
            if(filter.Length > 0)
            {
                filter += " AND";
            }
            if(value.Contains('=') || value.Contains('<') || value.Contains('>'))
            {
                return filter += $" {collumn} {value}";
            }
            return filter += $" {collumn} = {value}";
        }

        private string AddStringFilter(string filter, string collumn, string value)
        {
            if (filter.Length > 0)
            {
                filter += " AND";
            }
            return filter += $" {collumn} = \"{value}\"";
        }
        public void AddCereal(Cereal cereal)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand dbCmd = new MySqlCommand("USE cerealdb", conn);
                dbCmd.ExecuteNonQuery();
                MySqlCommand cmd = new MySqlCommand($"SELECT id FROM cereal ORDER BY id DESC LIMIT 1", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int id = 0;
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            id = reader.GetInt32("Id") + 1;
                        }
                    }
                    reader.Close();
                    MySqlCommand insertCmd = new MySqlCommand($"INSERT INTO cereal VALUES({id}, \"{cereal.Name}\", \"{cereal.MFR.ToString()}\", \"{cereal.Type.ToString()}\", {cereal.Calories}, {cereal.Protein}, {cereal.Fat}, {cereal.Sodium}, {cereal.Fiber}, {cereal.Carbo}, {cereal.Sugars}, {cereal.Potass}, {cereal.Vitamins}, {cereal.Shelf}, {cereal.Weight}, {cereal.Cups}, \"{cereal.Rating}\")", conn);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
        public Cereal UpdateCereal(Cereal cereal)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                Cereal c = GetCereal(cereal.Id);
                string updateValues = "";
                if(cereal.Name != c.Name && cereal.Name.Length != 0)
                {
                    updateValues += $" cereal_name = \"{cereal.Name}\"";
                }
                if(cereal.MFR != c.MFR && cereal.MFR != '0')
                {
                    if(updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" mfr = \"{cereal.MFR}\"";
                }
                if (cereal.Type != c.Type && cereal.Type != 'N')
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" cereal_type = \"{cereal.Type}\"";
                }
                if (cereal.Calories != c.Calories && cereal.Calories != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" calories = {cereal.Calories}";
                }
                if (cereal.Protein != c.Protein && cereal.Protein != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" protein = {cereal.Protein}";
                }
                if (cereal.Fat != c.Fat && cereal.Fat != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" fat = {cereal.Fat}";
                }
                if (cereal.Sodium != c.Sodium && cereal.Sodium != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" sodium = {cereal.Sodium}";
                }
                if (cereal.Fiber != c.Fiber && cereal.Fiber != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" fiber = {cereal.Fiber}";
                }
                if (cereal.Carbo != c.Carbo && cereal.Carbo != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" carbo = {cereal.Carbo}";
                }
                if (cereal.Sugars != c.Sugars && cereal.Sugars != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" sugars = {cereal.Sugars}";
                }
                if (cereal.Potass != c.Potass && cereal.Potass != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" potass = {cereal.Potass}";
                }
                if (cereal.Vitamins != c.Vitamins && cereal.Vitamins != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" vitamins = {cereal.Vitamins}";
                }
                if (cereal.Shelf != c.Shelf && cereal.Shelf != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $" shelf = {cereal.Shelf}";
                }
                if (cereal.Weight != c.Weight && cereal.Weight != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $"weight = {cereal.Weight}";
                }
                if (cereal.Cups != c.Cups && cereal.Cups != -1)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $"cups = {cereal.Cups}";
                }
                if (cereal.Rating != c.Rating && cereal.Rating.Length != 0)
                {
                    if (updateValues.Length > 0)
                    {
                        updateValues += ",";
                    }
                    updateValues += $"rating = \"{cereal.Rating}\"";
                }
                if (updateValues.Length > 0)
                {
                    MySqlCommand updateCommand = new MySqlCommand($"UPDATE cereal SET {updateValues} WHERE id = {cereal.Id}", conn);
                    updateCommand.ExecuteNonQuery();
                }
                return GetCereal(cereal.Id);
            }
        }
        public Cereal GetCereal(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand selectCommand = new MySqlCommand($"SELECT * FROM cereal WHERE id = {id}", conn);
                MySqlDataReader selectReader = selectCommand.ExecuteReader();
                if (selectReader.HasRows)
                {
                    Cereal cereal = new Cereal();
                    while (selectReader.Read())
                    {
                        cereal = new Cereal()
                        {
                            Id = selectReader.GetInt32("id"),
                            Name = selectReader.GetString("cereal_name"),
                            MFR = selectReader.GetChar("mfr"),
                            Type = selectReader.GetChar("cereal_type"),
                            Calories = selectReader.GetInt32("calories"),
                            Protein = selectReader.GetInt32("protein"),
                            Fat = selectReader.GetInt32("fat"),
                            Sodium = selectReader.GetInt32("sodium"),
                            Fiber = selectReader.GetFloat("fiber"),
                            Carbo = selectReader.GetFloat("carbo"),
                            Sugars = selectReader.GetInt32("sugars"),
                            Potass = selectReader.GetInt32("potass"),
                            Vitamins = selectReader.GetInt32("vitamins"),
                            Shelf = selectReader.GetInt32("shelf"),
                            Weight = selectReader.GetFloat("weight"),
                            Cups = selectReader.GetFloat("cups"),
                            Rating = selectReader.GetString("rating")
                        };
                    }
                    
                    selectReader.Close();
                    return cereal;
                }
                else
                {
                    return new Cereal();
                }
            }
        }

        public int GetId(string name, char mfr)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT id FROM cereal WHERE cereal_name = \"{name}\" AND mfr = \"{mfr}\"", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int id = -1;
                while(reader.HasRows && reader.Read())
                {
                    id = (int)reader.GetInt32("id");
                }
                return id;
            }
        }
        public void CreateDB(IWebHostEnvironment env)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;user=root;password=test"))
            {
                conn.Open();
                string db = "cerealdb";
                if(DatabaseName != null)
                {
                    db = DatabaseName;
                }
                MySqlCommand cmd = new MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS {db};\r\n" +
                    $"USE {db};\r\n" +
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
                if (!reader.HasRows)
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
                if (!hasRows && env != null)
                {
                    Console.WriteLine("loading data");
                    string path = Path.Combine(env.WebRootPath, "Data");
                    InsertFromCSV(path, "Cereal.csv");
                }
            }
        }

        public void InsertFromCSV(string filePath, string fileName)
        {
            try
            {
                var csv = File.ReadAllText(Path.Combine(filePath, fileName));
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

        private void CreateUser(string username, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            using (MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=cerealdb;user=root;password=test"))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE username = \"{username}\"", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                bool contains = reader.HasRows;
                reader.Close();

                cmd.ExecuteNonQuery();
                if (!contains)
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

        public bool VerifyUser(string username, string password)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                bool res = false;
                MySqlCommand cmd = new MySqlCommand($"SELECT psswrd FROM users WHERE username = \"{username}\"", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    string hash = "";
                    while(reader.Read())
                    {
                        hash = reader.GetString("psswrd");
                        
                    }
                    
                    if (BCrypt.Net.BCrypt.EnhancedVerify(password, hash))
                    {
                        res = true;
                        
                    }
                }
                reader.Close();
                return res;
            }
        }
        public void LogAPICall(string timestamp, string method, string arguments, string result)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO api_log VALUES(\"{timestamp}\", \"{method}\", \"{arguments}\", \"{result}\")", conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}