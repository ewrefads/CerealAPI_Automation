using CerealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CerealAPI.Controllers
{
    /// <summary>
    /// Controller for the admin API which handles various tasks not covered by another API
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        CerealContext cerealContext;

        private readonly ILogger<AdminController> _logger;


        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
            cerealContext = new CerealContext("server=localhost;port=3306;database=cerealdb;user=root;password=test");

        }

        /// <summary>
        /// Returns all logs given the user provides valid userinfo
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The logs if succesful</returns>
        [HttpGet(Name = "GetLogs")]
        public IActionResult GetLogs(string username, string password) 
        { 
            if(cerealContext.VerifyUser(username, password))
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "GetLogs", "", "200:Ok");
                return Ok(cerealContext.Logs());
            }
            cerealContext.LogAPICall(DateTime.Now.ToString(), "GetLogs", "", "400:Bad Request");
            return BadRequest("Invalid username or password");
        }

        /// <summary>
        /// Adds a user to the database provided they dont exist yet
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost(Name = "AddUser")]
        public IActionResult AddUser(string username, string password)
        {
            if(!cerealContext.ContainsUser(username))
            {
                cerealContext.CreateUser(username, password);
                cerealContext.LogAPICall(DateTime.Now.ToString(), "AddUser", "", "200:Ok");
                return Ok("User was created");
            }
            else
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "AddUser", "", "400:Bad Request");
                return BadRequest("User allready Exists");
            }
        }

        /// <summary>
        /// Updates a users password provided the user can be verified by username and their old password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordRepeated"></param>
        /// <returns>Whether operation was succesful</returns>
        [HttpPut(Name = "UpdatePassword")]
        public IActionResult UpdatePassword(string username, string oldPassword, string newPassword, string newPasswordRepeated)
        {
            if(newPassword != newPasswordRepeated)
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdatePassword", "", "400:Bad Request");
                return BadRequest("passwords did not match");
            }
            if(!cerealContext.ContainsUser(username))
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdatePassword", "", "400:Bad Request");
                return BadRequest("Invalid username or old password");
            }
            if(!cerealContext.VerifyUser(username, oldPassword))
            {
                cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdatePassword", "", "400:Bad Request");
                return BadRequest("Invalid username or old password");
            }
            cerealContext.LogAPICall(DateTime.Now.ToString(), "UpdatePassword", "", "200:Ok");
            cerealContext.CreateUser(username, newPassword);
            return Ok("Password was updated");
        }
    }
}
