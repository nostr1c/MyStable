using Microsoft.AspNetCore.Mvc;
using api.Models;
using System.Data;
using Dapper;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDbConnection _connection;

        public UserController(ILogger<UserController> logger, IDbConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            string query = "SELECT * FROM [USER]";
            IEnumerable<User> users = await _connection.QueryAsync<User>(query);
                    
            return Ok(users);
        }

        [HttpGet("{userId:int}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            string query = "SELECT * FROM [USER] WHERE UserID = @UserId";

            var user = await _connection.QueryFirstOrDefaultAsync<User>(query, new { UserId = userId });

            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
