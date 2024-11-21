using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            // Add services to the container.
            string connectionString = BuildConnectionString(builder.Configuration, builder.Environment);

            builder.Services.AddTransient<IDbConnection>(sc => new SqlConnection(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        public static string BuildConnectionString(IConfiguration config, IWebHostEnvironment environment)
        {
            string? server = config["DB_SERVER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(server, "Environment variable 'DB_SERVER' must be set");
            string? database = config["DB_DATABASE"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(database, "Environment variable 'DB_DATABASE' must be set");
            string? user = config["DB_USER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(user, "Environment variable 'DB_USER' must be set");
            string? password = config["DB_PASSWORD"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(password, "Environment variable 'DB_PASSWORD' must be set");

            string encrypt;
            string trustCertificate;

            if (environment.IsDevelopment())
            {
                encrypt = "False";
                trustCertificate = "True";
            }
            else
            {
                encrypt = "True";
                trustCertificate = "False";
            }

            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};Encrypt={encrypt};TrustServerCertificate={trustCertificate};";
            return connectionString;
        }
    }
}
