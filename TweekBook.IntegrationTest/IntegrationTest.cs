using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Data;
using Microsoft.EntityFrameworkCore;
using TweekBook.Contracts.V1;
using TweekBook.Contracts.V1.Requests;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using TweekBook.Contracts.V1.Responses;

namespace TweekBook.IntegrationTest
{
    public class IntegrationTest : IDisposable  
    {
        protected readonly HttpClient TestClient;

        private readonly IServiceProvider _serviceProvider;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(webhostbuilder => {
                    webhostbuilder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options => {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });

            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",await GetJwtAsync());
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest createPostRequest)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, createPostRequest);
            return (await response.Content.ReadAsAsync<PostResponse>());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegisterationRequest {
                Email = "test@integrationtest.com",
                Password = "Test1234!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<DataContext>();

            context.Database.EnsureDeleted();
        }
    }
}
