using Refit;
using System;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Requests;

namespace TweekBook.SDK.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var chachedToken = string.Empty;

            var identityApi = RestService.For<IIdentityApi>("http://localhost:5000");
            var tweetBookApi = RestService.For<ITweetBookApi>("http://localhost:5000", new RefitSettings { 
                AuthorizationHeaderValueGetter = () => Task.FromResult(chachedToken)
            });

            var registerResponse = await identityApi.RegisterAsync(new UserRegisterationRequest { 
                Email = "venky@sdk.com",
                Password = "Venky1234$"
            });

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "venky@sdk.com",
                Password = "Venky1234$"
            });

            chachedToken = loginResponse.Content.Token;

            var allPosts = await tweetBookApi.GetaAllAsync();

            var createdPost = await tweetBookApi.CreateAsync(new CreatePostRequest { 
                Name = "This is Created by the SDK",
                Tags = new[] {"SDK"}
            });

        }
    }
}
