using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Contracts.V1;
using TweekBook.Contracts.V1.Responses;
using TweekBook.Domain;
using Xunit;

namespace TweekBook.IntegrationTest
{
    public class PostControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_Without_AnyPosts_ReturnsNull()
        {
            //Arrange
            await AuthenticateAsync();

            //Assign 
            var response = await  TestClient.GetAsync(ApiRoutes.Posts.GetAll);


            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
        }
    }
}
