using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Data;
using Microsoft.EntityFrameworkCore;
using TweekBook.Contracts.V1;
using TweekBook.Contracts.V1.Requests;
using System.Net.Http.Headers;

namespace TweekBook.IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(webhostbuilder => {
                    webhostbuilder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options => {
                           // options.UseInMemoryDatabase("TestDb")
                        });
                    });
                });

            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",await GetJwtAsync());
        }

        private Task<string> GetJwtAsync()
        {
            //var response = TestClient.PostAsync(ApiRoutes.Identity.Register, new UserRegisterationRequest {

            //});
            throw new NotImplementedException();
        }
    }
}
