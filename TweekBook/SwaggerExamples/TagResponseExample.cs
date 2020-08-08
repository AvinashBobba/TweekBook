using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Responses;

namespace TweekBook.SwaggerExamples
{
    public class TagResponseExample : IExamplesProvider<TagResponse>
    {
        public TagResponse GetExamples()
        {
            return new TagResponse
            {
                Name = "new Tag"
            };
        }
    }
}
