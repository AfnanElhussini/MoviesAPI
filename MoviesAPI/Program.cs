using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(name: "V1", info: new OpenApiInfo
                {
                    Version = "V1",
                    Title = "Movies API TEST",
                    Description = "A Movies API is a web service that allows developers to access a database of movie information",
                    TermsOfService = new Uri(uriString: "https://www.google.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "Afnan",
                        Email = "afnan@domain.com"
                    },
                   
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "Movies API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}