using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.API.MapperProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<OutstandingVisitList, OutstandingVisitReportDto>()
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient_Id));

            CreateMap<Appointment, AppointmentIdentifierDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.AppointmentDate.ToString("yyyy-MM-dd")));
            CreateMap<Appointment, AppointmentDetailDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.AppointmentDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.DidNotArrive, opt => opt.MapFrom(src => src.DNA))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Cancelled ? "Yes" : "No"));

            CreateMap<AppointmentList, AppointmentIdentifierDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppointmentId));
            CreateMap<AppointmentList, AppointmentSearchDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.CurrentFacility, opt => opt.MapFrom(src => src.FacilityName));
        }
    }
}