using MySql.Data.MySqlClient;

namespace CerealAPI.Models
{
    /// <summary>
    /// A class representaion of a row in the cereal table
    /// </summary>
    public class Cereal
    {
        private CerealContext context;

        public Cereal(MySqlDataReader reader)
        {
            while(reader.Read())
            {
                Id = reader.GetInt32("id");
                Name = reader.GetString("cereal_name");
                MFR = reader.GetChar("mfr");
                Type = reader.GetChar("cereal_type");
                Calories = reader.GetInt32("calories");
                Protein = reader.GetInt32("protein");
                Fat = reader.GetInt32("fat");
                Sodium = reader.GetInt32("sodium");
                Fiber = reader.GetFloat("fiber");
                Carbo = reader.GetFloat("carbo");
                Sugars = reader.GetInt32("sugars");
                Potass = reader.GetInt32("potass");
                Vitamins = reader.GetInt32("vitamins");
                Shelf = reader.GetInt32("shelf");
                Weight = reader.GetFloat("weight");
                Cups = reader.GetFloat("cups");
                Rating = reader.GetString("rating");
            }
        }
        public Cereal()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public char MFR { get; set; }
        public char Type { get; set; }
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Sodium { get; set; }
        public float Fiber { get; set; }
        public float Carbo { get; set; }
        public int Sugars { get; set; }
        public int Potass { get; set; }
        public int Vitamins { get; set; }
        public int Shelf { get; set; }
        public float Weight { get; set; }
        public float Cups { get; set; }
        public string Rating { get; set; }
    }
}
