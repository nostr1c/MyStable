namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            var serviceConfigurator = new ServiceConfigurator(builder.Services);

            serviceConfigurator.AddRepositories();
            serviceConfigurator.AddValidators();
            serviceConfigurator.AddDbConnection(builder.Configuration, builder.Environment);
            serviceConfigurator.AddSwaggerGen();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

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
    }
}
