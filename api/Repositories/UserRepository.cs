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

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
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
