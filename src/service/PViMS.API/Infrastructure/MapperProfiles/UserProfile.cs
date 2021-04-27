using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using System;
using System.Linq;

namespace PVIMS.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AuditLog, AuditLogIdentifierDto>();
            CreateMap<AuditLog, AuditLogDetailDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.HasLog, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Log)));

            CreateMap<IdentityRole<Guid>, RoleIdentifierDto>();

            CreateMap<User, UserIdentifierDto>();
            CreateMap<User, UserDetailDto>()
                .ForMember(dest => dest.AllowDatasetDownload, opt => opt.MapFrom(src => src.AllowDatasetDownload ? "Yes" : "No"))
                //.ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.Facilities.Select(f => f.Facility.FacilityName).ToArray()));

            //CreateMap<UserRole, UserRoleDto>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
