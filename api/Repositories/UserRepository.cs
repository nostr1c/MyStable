using System.Data;
using api.Models;
using api.Dto;
using Dapper;
using api.Repositories.Exceptions;

namespace api.Services
{
    public class UserRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnection connection, ILogger<UserRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }   

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                string query = "SELECT * FROM [User]";

                IEnumerable<User> users = await _connection.QueryAsync<User>(query);

                return users;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching users.", ex);
            }
        }

        public async Task<User?> GetUserByIdAsync(int userID)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE UserID = @UserID";
                return await _connection.QuerySingleOrDefaultAsync<User>(query, new { UserID = userID });
            } 
            catch (Exception ex)
            {
                throw new RepositoryException($"Error fetching user with ID: {userID}", ex);
            }
        }

        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                string query = @"INSERT INTO [User] (Firstname, Lastname, Username, Email, Biography, Avatar)
                            OUTPUT INSERTED.*
                            VALUES (@Firstname, @Lastname, @Username, @Email, @Biography, @Avatar)
                            ";
                return await _connection.QuerySingleAsync<User>(query, request);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error creating user.", ex);
            }
        }

        public async Task<User> UpdateUserAsync(UpdateUserRequest request)
        {
            try
            {
                string query = @"UPDATE [USER] SET Firstname = @Firstname,
                                            Lastname = @Lastname,
                                            Biography = @Biography,
                                            Avatar = @Avatar
                                OUTPUT INSERTED.*
                                WHERE UserID = @Id";

                var result = await _connection.QuerySingleAsync<User>(query, request);

                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating user.", ex);
            }
        }

        public async Task<User> DeleteUserAsync(int id)
        {
            try
            {
                string query = @"UPDATE [USER] SET IsDeleted = 1
                                OUTPUT INSERTED.*
                                WHERE UserID = @Id";

                var result = await _connection.QuerySingleAsync<User>(query, new { Id = id });

                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating user.", ex);
            }
        }

        public async Task<bool> UsernameAlreadyExists(string username)
        {
            try
            {
                string query = @"SELECT 1 UserID FROM [User] WHERE Username = @Username";
                var result = await _connection.ExecuteScalarAsync<int?>(query, new { UserName = username });
                return result.HasValue;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking username", ex);
            }
        }

        public async Task<bool> EmailAlreadyExists(string email)
        {
            try
            {
                string query = @"SELECT 1 UserID FROM [User] WHERE Email = @Email";
                var result = await _connection.ExecuteScalarAsync<int?>(query, new { Email = email });
                return result.HasValue;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking email", ex);
            }
        }
    }
}
