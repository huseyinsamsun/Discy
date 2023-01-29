using AutoMapper;
using Discy.Api.Application.Features.Commands.User.Create;
using Discy.Api.Domain.Models;
using Discy.Common.Models.Queries;
using Discy.Common.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Api.Application.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginUserViewModel>().ReverseMap();
            CreateMap<User, CreateUserCommand>().ReverseMap();
            CreateMap<User, UpdateUserCommand>().ReverseMap();
        }
      

    }
}
