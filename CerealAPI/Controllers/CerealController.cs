using CerealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Buffers.Text;
using System.Net;
using System.Xml.Linq;

namespace CerealAPI.Controllers
{
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

        /*// GET: /<controller>/
        public IActionResult Index()
        {
            cerealContext = HttpContext.RequestServices.GetService(typeof(CerealContext)) as CerealContext;
            return View(cerealContext.GetAllCereals());
        }*/

        [HttpGet(Name = "GetCereals")]
        [ActionName("GetCereal")]
        public IActionResult Get(int? id, string? name, string? manufacturer, string? type, string? calories, string? protein, string? fat, string? sodium, string? fiber, string? carbo, string? sugars, string? potass, string? vitamins, string? shelf, string? weight, string? cups, string? rating, string? sort)
        {
            if (id != null)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return Ok(new List<Cereal> { cerealContext.GetCereal((int)id) });
            }
            if (name != null || manufacturer != null || type != null || calories != null || protein != null || fat != null || sodium != null || fiber != null || carbo != null || sugars != null || potass != null || vitamins != null || shelf != null || weight != null || cups != null || rating != null)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return Ok(cerealContext.GetFilteredCereals(name, manufacturer, type, calories, protein, fat, sodium, fiber, carbo, sugars, potass, vitamins, shelf, weight, cups, rating, sort));
            }
            cerealContext.LogAPICall(DateTime.Now.ToString(), "GetCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
            return Ok(cerealContext.GetAllCereals(sort));
        }

        
       
        private bool Login(string username, string password)
        {
            return cerealContext.VerifyUser(username, password);
        }

        [HttpDelete(Name = "DeleteCereal")]
        public IActionResult DeleteCereal(string username, string password, int id)
        {
            if(Login(username, password))
            {
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
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "DeleteCereal", $"{id}", "400:Bad Request");
                return BadRequest("invalid username or password");
            }
        }

        [HttpPost(Name = "AddCereal")]
        public IActionResult AddCereal(string username, string password, int? id, string name, string manufacturer, string? type, int? calories, int? protein, int? fat, int? sodium, float? fiber, float? carbo, int? sugars, int? potass, int? vitamins, int? shelf, float? weight, float? cups, string? rating)
        {
            if(Login(username, password))
            {
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
                    if (cerealContext.ContainsId((int)id))
                    {
                        cereal.Id = (int)id;
                        cereal = cerealContext.UpdateCereal(cereal);
                        cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                        return Ok(cereal);
                    }
                    else
                    {
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
                else
                {

                    cerealContext.AddCereal(cereal);
                    cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                    return Ok(cereal);
                }
            }
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "AddCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "400:Bad Request");
                return BadRequest("invalid username or password");
            }
            
        }

        [HttpPut(Name = "UpdateCereal")]
        public IActionResult UpdateCereal(string username, string password, string name, string manufacturer, string? type, int? calories, int? protein, int? fat, int? sodium, float? fiber, float? carbo, int? sugars, int? potass, int? vitamins, int? shelf, float? weight, float? cups, string? rating)
        {
            int id = cerealContext.GetId(name, manufacturer[0]);
            bool validUser = Login(username, password);
            if(id == -1 && validUser)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "404:Not Found");
                return NotFound("No cereal with the given id was found");
            }
            else if(validUser)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "200:Ok");
                return (AddCereal(username, password, id, name, manufacturer, type, calories, protein, fat, sodium, fiber, carbo, sugars, potass, vitamins, shelf, weight, cups, rating));
            }
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdateCereal", $"{id}, {name}, {manufacturer}, {type}, {calories}, {protein}, {fat}, {sodium}, {fiber}, {carbo}, {sugars}, {potass}, {vitamins}, {shelf}, {weight}, {cups}, {rating}", "400:Bad Request");
                return BadRequest("Invalid username or password");
            }
        }

    }
}
