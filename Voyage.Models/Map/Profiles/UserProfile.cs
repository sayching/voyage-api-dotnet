﻿using AutoMapper;
using Voyage.Models.Entities;

namespace Voyage.Models.Map.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerifyRequired, opt => opt.MapFrom(src => src.IsVerifyRequired))
                .ForMember(dest => dest.PasswordRecoveryToken, opt => opt.MapFrom(src => src.PasswordRecoveryToken))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
