using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Responses;
using TweekBook.Domain;

namespace TweekBook.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags, opt => 
                opt.MapFrom(src => src.Tags.Select(x => new TagResponse { Name = x.TagName})));

            CreateMap<Tags, TagResponse>();


        }
    }
}
