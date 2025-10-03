using CerealAPI.Models;
using MySql.Data.MySqlClient;

namespace CerealAPI_Test
{
    /// <summary>
    /// Various tests of the CerealContext class
    /// </summary>
    public class CerealContextTests
    {
        string connectionString = "server=localhost;port=3306;user=root;password=test";
        CerealContext context;
        /// <summary>
        /// Sets up the context variable and sets its database
        /// </summary>
        [SetUp]
        public void Setup()
        {
            context = new CerealContext(connectionString);
            context.DatabaseName = "testCerealdb";
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
                    Assert.IsTrue(true);
                }
                catch (MySqlException ex)
                {
                    Assert.AreEqual(int.MinValue, ex.Number);
                }
            }
        }

        /// <summary>
        /// Checks if CreateDB creates the database
        /// </summary>
        [Test]
        public void DatabaseGetsCreated()
        {
            context.CreateDB(null);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand checkIfDatabaseGetsCreated = new MySqlCommand("USE testCerealdb", conn);
                    checkIfDatabaseGetsCreated.ExecuteNonQuery();
                    conn.Close();
                    Assert.IsTrue(true);
                }
                catch (MySqlException ex)
                {
                    Assert.AreEqual(int.MinValue, ex.Number);
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
            context.CreateDB(null);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand enterDatabase = new MySqlCommand("USE testCerealdb", conn);
                    enterDatabase.ExecuteNonQuery();
                    MySqlCommand selectFromTable = new MySqlCommand($"SELECT * FROM {table}", conn);
                    selectFromTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.IsTrue(true);
                }
                catch (MySqlException ex)
                {
                    Assert.AreEqual(int.MinValue, ex.Number);
                }
            }
        }

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
            context.CreateDB(null);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand enterDatabase = new MySqlCommand("USE testCerealdb", conn);
                    enterDatabase.ExecuteNonQuery();
                    MySqlCommand selectFromTable = new MySqlCommand($"SELECT {collumn} FROM {table}", conn);
                    selectFromTable.ExecuteNonQuery();
                    conn.Close();
                    Assert.IsTrue(true);
                }
                catch (MySqlException ex)
                {
                    Assert.AreEqual(int.MinValue, ex.Number);
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
                MySqlCommand cleanUp = new MySqlCommand("DROP DATABASE IF EXISTS testCerealdb", conn);
                cleanUp.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}