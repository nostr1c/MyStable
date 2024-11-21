using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Dto;
using api.Services;
using Dapper;
using api.Repositories.Exceptions;
using FluentValidation.Results;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] 
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserRepository _userRepository;
            
        public UserController(ILogger<UserController> logger, UserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                IEnumerable<User> users = await _userRepository.GetUsersAsync();

                return Ok(users);
            }
            catch (RepositoryException repoEx)
            {
                int status = StatusCodes.Status500InternalServerError;
                return StatusCode(status, new ProblemDetails { Title = repoEx.Message, Status = status });
            }
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        [HttpGet("{userId:int}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ProblemDetails { Title = $"Could not find user with ID: {userId}", Status = StatusCodes.Status404NotFound });
                }

                return Ok(user);
            }
            catch (RepositoryException repoEx)
            {
                int status = StatusCodes.Status500InternalServerError;
                return StatusCode(status, new ProblemDetails { Title = repoEx.Message, Status = status });
            }
        }

        /// <summary>
        /// Create user.
        /// </summary>
        [HttpPost(Name = "CreateUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest requestBody)
        {
            CreateUserRequestValidator validator = new CreateUserRequestValidator();
            ValidationResult result = validator.Validate(requestBody);

            //if (!result.IsValid)
            //{
            //    foreach (var failure in result.Errors)
            //    {

            //    }
            //    return BadRequest(new ProblemDetails { Title = "Bad request", Detail = "The request body is invalid.", Status = StatusCodes.Status400BadRequest });
            //}

            try
            {   
                User user = await _userRepository.CreateUserAsync(requestBody);

                return CreatedAtAction(nameof(GetUserById), new { user.UserID }, user);
            }
            catch (RepositoryException repoEx)
            {
                int status = StatusCodes.Status500InternalServerError;
                return StatusCode(status, new ProblemDetails { Title = repoEx.Message, Status = status });
            }
        }
    }
}
