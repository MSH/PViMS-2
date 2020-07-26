using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using System;

namespace PVIMS.API.MapperProfiles
{
    public class ReportInstanceProfile : Profile
    {
        public ReportInstanceProfile()
        {
            CreateMap<CausalityNotSetList, CausalityReportDto>()
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname));

            CreateMap<ReportInstance, ReportInstanceIdentifierDto>();
            CreateMap<ReportInstance, ReportInstanceDetailDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.CurrentActivity.CurrentStatus.Description))
                .ForMember(dest => dest.QualifiedName, opt => opt.MapFrom(src => src.CurrentActivity.QualifiedName));
            CreateMap<TerminologyMedDra, TerminologyMedDraDto>();

            CreateMap<ReportInstanceMedication, ReportInstanceMedicationIdentifierDto>()
                .ForMember(dest => dest.ReportInstanceId, opt => opt.MapFrom(src => src.ReportInstance.Id));
            CreateMap<ReportInstanceMedication, ReportInstanceMedicationDetailDto>()
                .ForMember(dest => dest.ReportInstanceId, opt => opt.MapFrom(src => src.ReportInstance.Id));

            CreateMap<ActivityExecutionStatusEvent, ActivityExecutionStatusEventDto>()
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.ExecutionStatus.Activity.QualifiedName))
                .ForMember(dest => dest.ExecutionEvent, opt => opt.MapFrom(src => src.ExecutionStatus.Description))
                .ForMember(dest => dest.ExecutedBy, opt => opt.MapFrom(src => src.EventCreatedBy != null ? src.EventCreatedBy.FullName : string.Empty))
                .ForMember(dest => dest.ExecutedDate, opt => opt.MapFrom(src => src.EventDateTime.ToString("yyyy-MM-dd HH:mm")))
                .ForMember(dest => dest.ReceiptDate, opt => opt.MapFrom(src => src.ContextDateTime != null ? Convert.ToDateTime(src.ContextDateTime).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ReceiptCode, opt => opt.MapFrom(src => src.ContextCode));
        }
    }
}