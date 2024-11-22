using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Dto;
using api.Services;
using api.Helpers;
using Dapper;
using api.Repositories.Exceptions;
using FluentValidation.Results;
using FluentValidation;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] 
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
            
        public UserController
        (
            ILogger<UserController> logger,
            UserRepository userRepository,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
        }   

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
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
        [HttpGet("{userId:int}")]
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
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest requestBody)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateUserRequest>>();

            ValidationResult result = await validator.ValidateAsync(requestBody);

            if (!result.IsValid)
            {
                var validationErrors = ValidationErrorHelper.ExtractValidationErrors(result);

                return BadRequest(new ProblemDetails
                { 
                    Title = "Validation Error",
                    Detail = "The request contains invalid data.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { { "errors", validationErrors } }
                });
            }

            try
            {
                User user = await _userRepository.CreateUserAsync(requestBody);

                return CreatedAtAction(nameof(GetUserById), new {userId = user.UserID }, user);
            }
            catch (RepositoryException repoEx)
            {
                int status = StatusCodes.Status500InternalServerError;
                return StatusCode(status, new ProblemDetails { Title = repoEx.Message, Status = status });
            }
        }
    }
}
