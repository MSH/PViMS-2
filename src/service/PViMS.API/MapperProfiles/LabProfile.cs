using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class LabProfile : Profile
    {
        public LabProfile()
        {
            CreateMap<LabResult, LabResultIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.LabResultName, opt => opt.MapFrom(src => src.Description));

            CreateMap<LabTest, LabTestIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.LabTestName, opt => opt.MapFrom(src => src.Description));

            CreateMap<LabTestUnit, LabTestUnitIdentifierDto>()
                .ForMember(dest => dest.LabTestUnitName, opt => opt.MapFrom(src => src.Description));

            CreateMap<PatientLabTest, PatientLabTestIdentifierDto>();
            CreateMap<PatientLabTest, PatientLabTestDetailDto>()
                .ForMember(dest => dest.TestDate, opt => opt.MapFrom(src => src.TestDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.LabTest, opt => opt.MapFrom(src => src.LabTest.Description))
                .ForMember(dest => dest.TestResultCoded, opt => opt.MapFrom(src => src.TestResult))
                .ForMember(dest => dest.TestResultValue, opt => opt.MapFrom(src => src.LabValue))
                .ForMember(dest => dest.TestUnit, opt => opt.MapFrom(src => src.TestUnit.Description));
        }
    }
}
