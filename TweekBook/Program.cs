using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TweekBook.Data;

namespace TweekBook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();
                
                //Adding Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                

                if(await roleManager.RoleExistsAsync(roleName:"Admin"))
                {
                    var adminRole = new IdentityRole(roleName: "Admin");
                    await roleManager.CreateAsync(adminRole);
                }
                if (await roleManager.RoleExistsAsync(roleName: "Poster"))
                {
                    var adminRole = new IdentityRole(roleName: "Poster");
                    await roleManager.CreateAsync(adminRole);
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
