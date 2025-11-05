using CerealAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CerealAPI.Controllers
{
    /// <summary>
    /// Controller for filehandling API
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        IWebHostEnvironment env;
        CerealContext cerealContext;
        private string mimeType = "image/jpg";
        private string filepath;

        private readonly ILogger<FileController> _logger;


        public FileController(ILogger<FileController> logger, IWebHostEnvironment env, IConfiguration configuration)
        {
            _logger = logger;
            cerealContext = new CerealContext(configuration.GetConnectionString("DefaultConnection"));
            this.env = env;
            filepath = Path.Combine(env.WebRootPath, "Cereal Pictures");
        }

        /// <summary>
        /// Retrieves the image for a cereal with the given id
        /// </summary>
        /// <param name="id">The id of the desired cereal</param>
        /// <returns>Whether the operation was succesful and if it was the image</returns>
        [HttpGet(Name = "GetImage")]
        public IActionResult GetImage(int id)
        {
            Cereal cereal = cerealContext.GetCereal(id);
            //Returns the image if the id was valid
            if (cereal.Name != null)
            {
                var stream = new FileStream(Path.Combine(filepath, cereal.Name + ".jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetImage", id.ToString(), "200:Ok");
                return File(stream, mimeType, enableRangeProcessing: true);
            }
            //Returns an error if it does not
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetImage", id.ToString(), "404:Not Found");
                return NotFound("no cereal with the given id was found");
            }
        }

        /// <summary>
        /// Inserts data from a given CSV file located in the wwwroot folder of the server
        /// </summary>
        /// <param name="location">Which folder the file is located in</param>
        /// <param name="name">the name of the file. Must contain the .CSV extension</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Whether the input was correct</returns>
        [HttpPost(Name = "InsertFromCSV")]
        public IActionResult InsertFromCSV(string location, string name, string username, string password)
        {
            if(!Path.Exists(Path.Combine(env.WebRootPath, Path.Combine(location, name))))
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "InsertFromCSV", $"{location}, {name}", "404:Not Found");
                return NotFound("File not found");
            }
            if(!name.Contains(".CSV"))
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "InsertFromCSV", $"{location}, {name}", "400:Bad Request");
                return BadRequest("File must be a CSV file");
            }
            if(cerealContext.VerifyUser(username, password))
            {
                cerealContext.InsertFromCSV(Path.Combine(env.WebRootPath, location), name);
                cerealContext.LogAPICall(DateTime.Now.ToString(), "InsertFromCSV", $"{location}, {name}", "200:Ok");
                return Ok("File contents was inserted");
            }
            return BadRequest("Invalid username or password");
        }

    }
}
