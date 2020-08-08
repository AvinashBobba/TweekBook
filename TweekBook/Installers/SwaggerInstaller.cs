using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TweekBook.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tweek Book API", Version = "v1" });

                c.ExampleFilters();

                var security = new OpenApiSecurityRequirement()
                {
                };

                c.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer Scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>() }
                });

                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlfile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
        }
    }
}
