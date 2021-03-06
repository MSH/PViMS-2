﻿using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using System;
using System.Linq;

namespace PVIMS.API.MapperProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientIdentifierDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName));
            CreateMap<Patient, PatientDetailDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? "" : Convert.ToDateTime(src.DateOfBirth).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
            CreateMap<Patient, PatientExpandedDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? "" : Convert.ToDateTime(src.DateOfBirth).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments.Where(a => a.Archived == false)))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments.Where(a => a.Archived == false)))
                .ForMember(dest => dest.Encounters, opt => opt.MapFrom(src => src.Encounters.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientConditions, opt => opt.MapFrom(src => src.PatientConditions.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientClinicalEvents, opt => opt.MapFrom(src => src.PatientClinicalEvents.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientMedications, opt => opt.MapFrom(src => src.PatientMedications.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientLabTests, opt => opt.MapFrom(src => src.PatientLabTests.Where(a => a.Archived == false)));

            CreateMap<PatientForCreationDto, PatientDetailForCreation>()
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName));
            CreateMap<PatientForUpdateDto, PatientDetailForCreation>()
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName));
            CreateMap<PatientForCreationDto, ConditionDetail>()
                .ForMember(dest => dest.DateStart, opt => opt.MapFrom(src => src.StartDate));

            CreateMap<PatientList, PatientIdentifierDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.FacilityName));
            CreateMap<PatientList, PatientDetailDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.FacilityName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
            CreateMap<PatientList, PatientListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.Surname}" ));

            CreateMap<PatientClinicalEvent, PatientClinicalEventIdentifierDto>();
            CreateMap<PatientClinicalEvent, PatientClinicalEventDetailDto>()
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.MedDraTerm))
                .ForMember(dest => dest.SourceTerminologyMedDraId, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.Id))
                .ForMember(dest => dest.OnsetDate, opt => opt.MapFrom(src => src.OnsetDate.HasValue ? Convert.ToDateTime(src.OnsetDate).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ResolutionDate, opt => opt.MapFrom(src => src.ResolutionDate.HasValue ? Convert.ToDateTime(src.ResolutionDate).ToString("yyyy-MM-dd") : ""));
            CreateMap<PatientClinicalEvent, PatientClinicalEventExpandedDto>()
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.MedDraTerm))
                .ForMember(dest => dest.SourceTerminologyMedDraId, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.Id))
                .ForMember(dest => dest.OnsetDate, opt => opt.MapFrom(src => src.OnsetDate.HasValue ? Convert.ToDateTime(src.OnsetDate).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ResolutionDate, opt => opt.MapFrom(src => src.ResolutionDate.HasValue ? Convert.ToDateTime(src.ResolutionDate).ToString("yyyy-MM-dd") : ""));

            CreateMap<AdverseEventList, AdverseEventReportDto>()
                .ForMember(dest => dest.AdverseEvent, opt => opt.MapFrom(src => src.Description));
            CreateMap<AdverseEventQuarterlyList, AdverseEventFrequencyReportDto>()
                .ForMember(dest => dest.PeriodDisplay, opt => opt.MapFrom(src => src.PeriodQuarter != null ? $"Quarter {src.PeriodQuarter} ({src.PeriodYear})" : ""))
                .ForMember(dest => dest.SystemOrganClass, opt => opt.MapFrom(src => src.MedDraTerm)); ;
            CreateMap<AdverseEventAnnualList, AdverseEventFrequencyReportDto>()
                .ForMember(dest => dest.PeriodDisplay, opt => opt.MapFrom(src => src.PeriodYear != null ? $"Year {src.PeriodYear}" : ""))
                .ForMember(dest => dest.SystemOrganClass, opt => opt.MapFrom(src => src.MedDraTerm)); ;
            CreateMap<PatientOnStudyList, PatientTreatmentReportDto>()
                .ForMember(dest => dest.EventPercentage, opt => opt.MapFrom(src => src.PatientCount > 0 ? Math.Round(((decimal)(src.PatientWithEventCount * 100.00) / (decimal)src.PatientCount), 2) : 0));
            CreateMap<DrugList, PatientMedicationReportDto>();

            CreateMap<PatientStatusHistory, PatientStatusDto>()
                .ForMember(dest => dest.PatientStatus, opt => opt.MapFrom(src => src.PatientStatus.Description))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
        }
    }
}