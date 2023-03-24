using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DashboardAggregate;

namespace PVIMS.API.MapperProfiles
{
    public class DashboardProfile : Profile
    {
        public DashboardProfile()
        {
            CreateMap<Dashboard, DashboardDetailDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => Frequency.From(src.FrequencyId).Name))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
        }
    }
}