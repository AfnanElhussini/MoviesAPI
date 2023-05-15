using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using MoviesAPI.Models;
using System.Reflection;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //Enable CORS
            builder.Services.AddCors();

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

                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name= "Authorization",
                    Type= SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat ="JWT",
                    In = ParameterLocation.Header,
                    Description  = "Please Enter JWT Key"
                });

                options.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id= "Bearer"
                                
                            },
                             Name = "Bearer",
                             In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                
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
            app.UseCors(
                c=> c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}