﻿using AutoMapper;
using Sofco.Core.Models.Admin;
using Sofco.Model.AzureAd;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.MapProfiles
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<User, UserModel>();

            CreateMap<User, UserDetailModel>();

            CreateMap<AzureAdLoginResponse, UserTokenModel>()
                .ForMember(d => d.AccessToken, s => s.MapFrom(x => x.access_token))
                .ForMember(d => d.RefreshToken, s => s.MapFrom(x => x.refresh_token))
                .ForMember(d => d.ExpiresIn, s => s.MapFrom(x => x.expires_in));
        }
    }
}
