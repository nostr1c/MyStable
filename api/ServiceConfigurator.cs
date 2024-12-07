﻿using api.Dto;
using api.Services;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using System.Data;

namespace api
{
    public class ServiceConfigurator
    {
        private readonly IServiceCollection _services;

        public ServiceConfigurator(IServiceCollection services)
        {
            _services = services;
        }

        public void AddRepositories()
        {
            _services.AddScoped<UserRepository>();
        }

        public void AddValidators()
        {
            _services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            _services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        }

        public void AddDbConnection(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string connectionString = DatabaseConfigurator.BuildConnectionString(configuration, environment);
            _services.AddTransient<IDbConnection>(sc => new SqlConnection(connectionString));
        }

        public void AddSwaggerGen()
        {
            _services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MyStable API",
                    Description = "An ASP.NET Core Web API for MyStable",
                    Contact = new OpenApiContact
                    {
                        Name = "My website",
                        Url = new Uri("https://filipsiri.se")
                    },
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }
    }

    public static class DatabaseConfigurator
    {
        public static string BuildConnectionString(IConfiguration config, IWebHostEnvironment environment)
        {
            string? server = config["DB_SERVER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(server);

            string? database = config["DB_DATABASE"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(database);

            string? user = config["DB_USER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(user);

            string? password = config["DB_PASSWORD"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(password);

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
