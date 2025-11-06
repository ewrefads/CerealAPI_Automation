using CerealAPI.Models;
using MySql.Data.MySqlClient;
using NUnit;
namespace CerealAPI_Test
{
    /// <summary>
    /// Various tests of the CerealContext class
    /// </summary>
    public class CerealContextTests
    {
        
        string connectionString = "server=db;port=3306;database=cerealdb;user=root;password=test";
        CerealContext context;
        /// <summary>
        /// Sets up the context variable and sets its database
        /// </summary>
        [SetUp]
        public void Setup()
        {
            context = new CerealContext(connectionString);
            context.tablePrefix = "test";
        }

        /// <summary>
        /// Checks if the server connection works
        /// </summary>
        [Test]
        public void ConnectionWorks()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    conn.Close();
                    Assert.That(true);
                }
                catch (MySqlException ex)
                {
                    Assert.That(int.MinValue == ex.Number);
                }
            }
        }


        /// <summary>
        /// Checks if the tables gets created
        /// </summary>
        /// <param name="table">The table to check for</param>
        [TestCase("cereal")]
        [TestCase("users")]
        [TestCase("api_log")]
        public void TableGetsCreated(string table)
        {
            table = "test" + table;
            context.CreateDB(null);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand selectFromTable = new MySqlCommand($"SELECT * FROM {table}", conn);
                    selectFromTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.That(true);
                }
                catch (MySqlException ex)
                {
                    Assert.That(int.MinValue == ex.Number);
                }
            }
        }

        /*[Test]
        public void FailingTest()
        {
            Assert.That(true == false);
        }*/

        /// <summary>
        /// Checks if a collumn gets created
        /// </summary>
        /// <param name="table">The table containing the collumn</param>
        /// <param name="collumn">the collumn to check for</param>
        [TestCase("users", "username")]
        [TestCase("users", "psswrd")]
        [TestCase("api_log", "acces_time")]
        [TestCase("api_log", "command")]
        [TestCase("api_log", "arguments")]
        [TestCase("api_log", "result")]
        [TestCase("cereal", "id")]
        [TestCase("cereal", "cereal_name")]
        [TestCase("cereal", "mfr")]
        [TestCase("cereal", "cereal_type")]
        [TestCase("cereal", "calories")]
        [TestCase("cereal", "protein")]
        [TestCase("cereal", "fat")]
        [TestCase("cereal", "sodium")]
        [TestCase("cereal", "fiber")]
        [TestCase("cereal", "carbo")]
        [TestCase("cereal", "sugars")]
        [TestCase("cereal", "potass")]
        [TestCase("cereal", "vitamins")]
        [TestCase("cereal", "shelf")]
        [TestCase("cereal", "weight")]
        [TestCase("cereal", "cups")]
        [TestCase("cereal", "rating")]
        public void ColumnGetsCreated(string table, string collumn)
        {
            table = "test" + table;
            context.CreateDB(null);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand selectFromTable = new MySqlCommand($"SELECT {collumn} FROM {table}", conn);
                    selectFromTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.That(true);
                }
                catch (MySqlException ex)
                {
                    Assert.That(int.MinValue == ex.Number);
                }
            }
        }

        /// <summary>
        /// Tests whether the collumns which should not be nullable is actually not nullable
        /// </summary>
        /// <param name="table">The table of the collumn</param>
        /// <param name="collumn">The collumn to be tested</param>
        [TestCase("users", "username")]
        [TestCase("users", "psswrd")]
        [TestCase("cereal", "id")]
        [TestCase("cereal", "cereal_name")]
        [TestCase("cereal", "mfr")]
        public void CollumnIsNotNullable(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    switch (table.Split("test")[1])
                    {
                        case "users":
                            if(collumn == "username")
                            {
                                data = $"(psswrd) VALUES(\"test\")";
                            }
                            else
                            {
                                data = $"(username) VALUES(\"test\")";
                            }
                            break;
                        case "cereal":
                            if(collumn == "id")
                            {
                                data = $"(cereal_name, mfr) VALUES(\"test\", \"t\")";
                            }
                            else if(collumn == "cereal_name")
                            {
                                data = $"(id, mfr) VALUES(1, \"t\")";
                            }
                            else
                            {
                                data = $"(id, cereal_name) VALUES(1, \"test\")";
                            }
                            break;

                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);
                    InsertIntoTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.That(false);
                }
                catch (MySqlException ex)
                {
                    //1364 is the mysql error code for inserting into a table without data in a non nullable field 
                    Assert.That(1364 == ex.Number);
                }
            }
        }
        [TestCase("users", "username")]
        [TestCase("api_log", "acces_time")]
        [TestCase("api_log", "command")]
        [TestCase("api_log", "arguments")]
        [TestCase("api_log", "result")]
        [TestCase("cereal", "cereal_name")]
        [TestCase("cereal", "rating")]
        public void IsString(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    switch (table.Split("test")[1])
                    {
                        case "users":
                            data = $"VALUES(\"test\", \"test\")";
                            break;
                        case "cereal":
                            if (collumn == "cereal_name")
                            {
                                data = $"(id, cereal_name, mfr) VALUES(1, \"test\",\"t\")";
                            }
                            else
                            {
                                data = $"(id, cereal_name, mfr, {collumn}) VALUES(1, \"test\", \"t\", \"test\")";
                            }
                            break;
                        case "api_log":
                            data = $"({collumn}) VALUES({collumn})";
                            break;
                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);
                    InsertIntoTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.That(true);
                }
                catch (MySqlException ex)
                { 
                    Assert.That(int.MinValue == ex.Number);
                }
            }
        }

        [TestCase("cereal", "mfr")]
        [TestCase("cereal", "cereal_type")]
        public void IsChar(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    switch (table.Split("test")[1])
                    {
                        case "cereal":
                            if (collumn == "mfr")
                            {
                                data = $"(id, cereal_name, mfr) VALUES(1, \"test\",\"te\")";
                            }
                            else
                            {
                                data = $"(id, cereal_name, mfr, {collumn}) VALUES(1, \"test\", \"t\", \"te\")";
                            }
                            break;
                        default:
                            Assert.That(data, Is.SameAs("no collumns on table tested"));
                            break;
                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);
                    InsertIntoTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.That(false);
                }
                catch (MySqlException ex)
                {
                    //1406 is the mysql error code for inserting sometheing which is too long for a collumn
                    Assert.That(ex.Number, Is.EqualTo(1406));
                }
            }
        }

        [TestCase("cereal", "id")]
        [TestCase("cereal", "calories")]
        [TestCase("cereal", "protein")]
        [TestCase("cereal", "fat")]
        [TestCase("cereal", "sodium")]
        [TestCase("cereal", "potass")]
        [TestCase("cereal", "vitamins")]
        [TestCase("cereal", "shelf")]
        public void IsInt(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    switch (table.Split("test")[1])
                    {
                        case "cereal":
                            if (collumn == "id")
                            {
                                data = $"(id, cereal_name, mfr) VALUES(1, \"test\",\"t\")";
                            }
                            else
                            {
                                data = $"(id, cereal_name, mfr, {collumn}) VALUES(1, \"test\", \"t\", 1)";
                            }
                            break;
                        default:
                            Assert.That(data, Is.SameAs("no collumns on table tested"));
                            break;
                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);

                    Assert.That(InsertIntoTable.ExecuteNonQuery(), Is.EqualTo(1));
                    conn.Close();
                    
                }
                catch (MySqlException ex)
                {
                    Assert.That(ex.Number, Is.EqualTo(int.MinValue));
                }
            }
        }

        [TestCase("cereal", "carbo")]
        [TestCase("cereal", "sugars")]
        [TestCase("cereal", "weight")]
        [TestCase("cereal", "cups")]
        public void IsFloat(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    switch (table.Split("test")[1])
                    {
                        case "cereal":
                            data = $"(id, cereal_name, mfr, {collumn}) VALUES(1, \"test\", \"t\", 5.55555)";
                            break;
                        default:
                            Assert.That(data, Is.SameAs("no collumns on table tested"));
                            break;
                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);

                    Assert.That(InsertIntoTable.ExecuteNonQuery(), Is.EqualTo(1));
                    conn.Close();

                }
                catch (MySqlException ex)
                {
                    Assert.That(ex.Number, Is.EqualTo(int.MinValue));
                }
            }
        }

        [Test]
        public void TestSystemTest()
        {
            Assert.That(true);
        }

        [TestCase("cereal", "id")]
        public void IsUnique(string table, string collumn)
        {
            context.CreateDB(null);
            table = "test" + table;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string data = "";
                    string data1 = "";
                    switch (table.Split("test")[1])
                    {
                        case "cereal":
                            if(collumn == "id")
                            {
                                data = $"(id, cereal_name, mfr) VALUES(1, \"test\", \"t\")";
                            }
                            //data = $"(id, cereal_name, mfr, {collumn}) VALUES(1, \"test\", \"t\", \"t\")";
                            break;
                        default:
                            Assert.That(data, Is.SameAs("no collumns on table tested"));
                            break;
                    }
                    MySqlCommand InsertIntoTable = new MySqlCommand($"INSERT INTO {table} {data}", conn);
                    InsertIntoTable.ExecuteNonQuery();
                    MySqlCommand InsertIntoTable1 = new MySqlCommand($"INSERT INTO {table} {data}", conn);
                    InsertIntoTable1.ExecuteNonQuery();
                    Assert.That(InsertIntoTable1.ExecuteNonQuery(), Is.EqualTo(2));
                    InsertIntoTable1.ExecuteNonQuery();
                    conn.Close();

                }
                catch (MySqlException ex)
                {
                    //1062 is the mysql error code for duplicate values in a unqiue collumn
                    Assert.That(ex.Number, Is.EqualTo(1062));
                }
            }
        }

        /// <summary>
        /// Cleans up after the tests by removing the database
        /// </summary>
        [TearDown]
        public void TearDown() 
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cleanUp = new MySqlCommand("DROP TABLE IF EXISTS testcereal;", conn);
                cleanUp.ExecuteNonQuery();
                cleanUp = new MySqlCommand("DROP TABLE IF EXISTS testusers;", conn);
                cleanUp.ExecuteNonQuery();
                cleanUp = new MySqlCommand("DROP TABLE IF EXISTS testapi_logs;", conn);
                cleanUp.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}