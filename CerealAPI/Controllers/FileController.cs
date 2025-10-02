using CerealAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CerealAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        IWebHostEnvironment env;
        CerealContext cerealContext;
        private string mimeType = "image/jpg";
        private string filepath;

        private readonly ILogger<FileController> _logger;


        public FileController(ILogger<FileController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            cerealContext = new CerealContext("server=localhost;port=3306;database=cerealdb;user=root;password=test");
            this.env = env;
            filepath = Path.Combine(env.WebRootPath, "Cereal Pictures");
        }

        [HttpGet(Name = "GetImage")]
        public IActionResult GetImage(int id)
        {
            Cereal cereal = cerealContext.GetCereal(id);
            if (cereal.Name != null)
            {
                var stream = new FileStream(Path.Combine(filepath, cereal.Name + ".jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetImage", id.ToString(), "200:Ok");
                return File(stream, mimeType, enableRangeProcessing: true);
            }
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetImage", id.ToString(), "404:Not Found");
                return NotFound("no cereal with the given id was found");
            }
        }

    }
}
