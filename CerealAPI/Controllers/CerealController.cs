using CerealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Buffers.Text;
using System.Net;
using System.Xml.Linq;

namespace CerealAPI.Controllers
{
    /// <summary>
    /// The controller for the primary API
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CerealController : Controller
    {
        CerealContext cerealContext;

        private readonly ILogger<CerealController> _logger;
        

        public CerealController(ILogger<CerealController> logger)
        {
            _logger = logger;
            cerealContext = new CerealContext("server=localhost;port=3306;database=cerealdb;user=root;password=test");
            
        }

        /// <summary>
        /// Returns all cereals which meets a given set of optional paramaters. All the numeric variables support range based filtering by adding the filter before for example >=1
        /// </summary>
        /// <param name="id">The id of the cereal</param>
        /// <param name="name">The name of the cereal</param>
        /// <param name="manufacturer">The manufacturerer of the cereal</param>
        /// <param name="type">The type of cereal</param>
        /// <param name="calories">The amount of calories in the cereal. Supports range based filtering</param>
        /// <param name="protein">The amount of protein in the cereal. Supports range based filtering</param>
        /// <param name="fat">The amount of fat in the cereal. Supports range based filtering</param>
        /// <param name="sodium">The amount of sodium in the cereal. Supports range based filtering</param>
        /// <param name="fiber">The amount of fiber in the cereal. Supports range based filtering</param>
        /// <param name="carbo">The amount of carbo in the cereal. Supports range based filtering</param>
        /// <param name="sugars">The amount of sugar in the cereal. Supports range based filtering</param>
        /// <param name="potass">The amount of potass in the cereal. Supports range based filtering</param>
        /// <param name="vitamins">The amount of vitamins in the cereal. Supports range based filtering</param>
        /// <param name="shelf">Which shelf to place it on. Supports range based filtering</param>
        /// <param name="weight">The weight of the cereal. Supports range based filtering></param>
        /// <param name="cups">The amount of cups pr. portion. Supports range based filtering</param>
        /// <param name="rating">The rating of the cereal</param>
        /// <param name="sort">Which value to sort by. _ASC or _DSC can optionally be added to tell whether it should be in ascending or descending order</param>
        /// <returns>A list containing the cereals which meets the criteria</returns>
        [HttpGet(Name = "GetCereals")]
        public IActionResult GetCereal(int? id, string? name, string? manufacturer, string? type, string? calories, string? protein, string? fat, string? sodium, string? fiber, string? carbo, string? sugars, string? potass, string? vitamins, string? shelf, string? weight, string? cups, string? rating, string? sort)
        {
            //Returns a specific cereal if an id was given
            if (id != null)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return Ok(cerealContext.GetCereal((int)id));
            }
            //Returns a filtered list if any of the variables except id or sort was given a value
            if (name != null || manufacturer != null || type != null || calories != null || protein != null || fat != null || sodium != null || fiber != null || carbo != null || sugars != null || potass != null || vitamins != null || shelf != null || weight != null || cups != null || rating != null)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return Ok(cerealContext.GetFilteredCereals(name, manufacturer, type, calories, protein, fat, sodium, fiber, carbo, sugars, potass, vitamins, shelf, weight, cups, rating, sort));
            }
            //returns the complete list optionally sorted
            cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
            return Ok(cerealContext.GetAllCereals(sort));
        }

        
        /// <summary>
        /// Checks if the given user info matches an existing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Whether the user is valid</returns>
        private bool Login(string username, string password)
        {
            return cerealContext.VerifyUser(username, password);
        }

        /// <summary>
        /// Deletes a cereal with a given id provided the username and password is valid
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="id">the id of the cereal which should be deleted</param>
        /// <returns>To which degree the operation was succesful</returns>
        [HttpDelete(Name = "DeleteCereal")]
        public IActionResult DeleteCereal(string username, string password, int id)
        {
            if(Login(username, password))
            {
                //tries to delete the cereal with the given id and tells whether it has been done
                bool deleted = cerealContext.DeleteCereal(id);
                
                if (deleted)
                {
                    cerealContext.LogAPICall(DateTime.Now.ToString(), "DeleteCereal", $"{id}", "200:Ok");
                    return Ok("Deleted the requested cereal");
                }
                else
                {
                    cerealContext.LogAPICall(DateTime.Now.ToString(), "DeleteCereal", $"{id}", "200:Ok");
                    return Ok($"No cereal with id {id} was found");
                }
            }
            //Prevents the user from deleting if they cannot log in
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "DeleteCereal", $"{id}", "400:Bad Request");
                return BadRequest("invalid username or password");
            }
        }

        /// <summary>
        /// Adds a cereal to the database if the userinfo is valid and it does not exists otherwise it updates the information of the given cereal. See GetCereal for descriptions of the variables
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="manufacturer"></param>
        /// <param name="type"></param>
        /// <param name="calories"></param>
        /// <param name="protein"></param>
        /// <param name="fat"></param>
        /// <param name="sodium"></param>
        /// <param name="fiber"></param>
        /// <param name="carbo"></param>
        /// <param name="sugars"></param>
        /// <param name="potass"></param>
        /// <param name="vitamins"></param>
        /// <param name="shelf"></param>
        /// <param name="weight"></param>
        /// <param name="cups"></param>
        /// <param name="rating"></param>
        /// <returns>Whether the operation was succesful</returns>
        [HttpPost(Name = "AddCereal")]
        public IActionResult AddCereal(string username, string password, int? id, string name, string manufacturer, string? type, int? calories, int? protein, int? fat, int? sodium, float? fiber, float? carbo, int? sugars, int? potass, int? vitamins, int? shelf, float? weight, float? cups, string? rating)
        {
            if(Login(username, password))
            {
                //Creates a cereal and sets default values before changing them if a specific value was given
                Cereal cereal = new Cereal()
                {
                    Name = name,
                    MFR = manufacturer[0],
                    Type = 'N',
                    Calories = -1,
                    Protein = -1,
                    Fat = -1,
                    Sodium = -1,
                    Fiber = -1,
                    Carbo = -1,
                    Sugars = -1,
                    Potass = -1,
                    Vitamins = -1,
                    Shelf = -1,
                    Weight = -1,
                    Cups = -1,
                    Rating = ""
                };

                if (type != null)
                {
                    cereal.Type = type[0];
                }
                if (calories != null)
                {
                    cereal.Calories = (int)calories;
                }
                if (protein != null)
                {
                    cereal.Protein = (int)protein;
                }
                if (fat != null)
                {
                    cereal.Fat = (int)fat;
                }
                if (sodium != null)
                {
                    cereal.Sodium = (int)sodium;
                }
                if (fiber != null)
                {
                    cereal.Fiber = (float)fiber;
                }
                if (carbo != null)
                {
                    cereal.Carbo = (float)carbo;
                }
                if (sugars != null)
                {
                    cereal.Sugars = (int)sugars;
                }
                if (potass != null)
                {
                    cereal.Potass = (int)potass;
                }
                if (vitamins != null)
                {
                    cereal.Vitamins = (int)vitamins;
                }
                if (shelf != null)
                {
                    cereal.Shelf = (int)shelf;
                }
                if (weight != null)
                {
                    cereal.Weight = (float)weight;
                }
                if (cups != null)
                {
                    cereal.Cups = (float)cups;
                }
                if (rating != null)
                {
                    cereal.Rating = rating;
                }
                if (id != null)
                {
                    //Checks if a given id allready exists
                    if (cerealContext.ContainsId((int)id))
                    {
                        cereal.Id = (int)id;
                        cereal = cerealContext.UpdateCereal(cereal);
                        cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                        return Ok(cereal);
                    }
                    else
                    {
                        //a teapot is not a cereal
                        if (name.ToLower() == "teapot")
                        {
                            cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "418:Im a teapot");
                            return BadRequest(StatusCodes.Status418ImATeapot + ": i'm a teapot which doesn't exists. id must be null or an existing value");
                        }
                        else
                        {
                            cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "404:Not Found");
                        }
                        return NotFound("id must be null or an existing value");
                    }
                }
                //Creates the cereal if no id was given
                else
                {

                    cerealContext.AddCereal(cereal);
                    cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                    return Ok(cereal);
                }
            }
            //Prevents the cereal from being created if no id was given
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "400:Bad Request");
                return BadRequest("invalid username or password");
            }
            
        }

        /// <summary>
        /// Updates a cereal which matches the given name and manufacturer provided the userinfo is correct. See GetCereal for descriptions of the variables
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="manufacturer"></param>
        /// <param name="type"></param>
        /// <param name="calories"></param>
        /// <param name="protein"></param>
        /// <param name="fat"></param>
        /// <param name="sodium"></param>
        /// <param name="fiber"></param>
        /// <param name="carbo"></param>
        /// <param name="sugars"></param>
        /// <param name="potass"></param>
        /// <param name="vitamins"></param>
        /// <param name="shelf"></param>
        /// <param name="weight"></param>
        /// <param name="cups"></param>
        /// <param name="rating"></param>
        /// <returns>Whether the operation was succesful</returns>
        [HttpPut(Name = "UpdateCereal")]
        public IActionResult UpdateCereal(string username, string password, string name, string manufacturer, string? type, int? calories, int? protein, int? fat, int? sodium, float? fiber, float? carbo, int? sugars, int? potass, int? vitamins, int? shelf, float? weight, float? cups, string? rating)
        {
            int id = cerealContext.GetId(name, manufacturer[0]);
            bool validUser = Login(username, password);
            //Does nothing if no cereal was found
            if(id == -1 && validUser)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "404:Not Found");
                return NotFound("No cereal with the given id was found");
            }
            //Updates the cereal if an id was found
            else if(validUser)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return (AddCereal(username, password, id, name, manufacturer, type, calories, protein, fat, sodium, fiber, carbo, sugars, potass, vitamins, shelf, weight, cups, rating));
            }
            //Prevents the updating of the cereal if the userinfo was invalid
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "400:Bad Request");
                return BadRequest("Invalid username or password");
            }
        }

    }
}
