using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Requests;

namespace TweekBook.SwaggerExamples.Requests
{
    public class CreateTagRequestExample : IExamplesProvider<CreateTagRequest>
    {
        public CreateTagRequest GetExamples()
        {
            return new CreateTagRequest { 
                TagName = "new Tag"
            };
        }
    }
}
