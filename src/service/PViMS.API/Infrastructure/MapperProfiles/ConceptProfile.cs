using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using System;

namespace PVIMS.API.MapperProfiles
{
    public class ConceptProfile : Profile
    {
        public ConceptProfile()
        {
            CreateMap<Concept, ConceptIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.ConceptName} ({src.MedicationForm.Description})"));
            CreateMap<Concept, ConceptDetailDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.ConceptName} ({src.MedicationForm.Description})"))
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.MedicationForm.Description));

            CreateMap<Product, ProductIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description}) ({src.ProductName})" ));
            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description}) ({src.ProductName})"))
                .ForMember(dest => dest.ConceptName, opt => opt.MapFrom(src => src.Concept.ConceptName))
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Concept.MedicationForm.Description));

            CreateMap<MedicationForm, MedicationFormIdentifierDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Description));

            CreateMap<PatientMedication, PatientMedicationIdentifierDto>();
            CreateMap<PatientMedication, PatientMedicationDetailDto>()
                .ForMember(dest => dest.SourceDescription, opt => opt.MapFrom(src => src.MedicationSource))
                .ForMember(dest => dest.ConceptId, opt => opt.MapFrom(src => src.Concept.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Product != null ? $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description}) ({src.Product.ProductName})" : $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description})"))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("yyyy-MM-dd") : ""));
        }
    }
}
