using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class MetaProfile : Profile
    {
        public MetaProfile()
        {
            CreateMap<MetaColumn, MetaColumnIdentifierDto>()
                .ForMember(dest => dest.MetaColumnGuid, opt => opt.MapFrom(src => src.metacolumn_guid));
            CreateMap<MetaColumn, MetaColumnDetailDto>()
                .ForMember(dest => dest.MetaColumnGuid, opt => opt.MapFrom(src => src.metacolumn_guid))
                .ForMember(dest => dest.ColumnType, opt => opt.MapFrom(src => src.ColumnType.Description))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table.TableName));

            CreateMap<MetaDependency, MetaDependencyIdentifierDto>()
                .ForMember(dest => dest.MetaDependencyGuid, opt => opt.MapFrom(src => src.metadependency_guid));
            CreateMap<MetaDependency, MetaDependencyDetailDto>()
                .ForMember(dest => dest.MetaDependencyGuid, opt => opt.MapFrom(src => src.metadependency_guid))
                .ForMember(dest => dest.ParentTableName, opt => opt.MapFrom(src => src.ParentTable.TableName))
                .ForMember(dest => dest.ReferenceTableName, opt => opt.MapFrom(src => src.ReferenceTable.TableName));

            CreateMap<MetaForm, MetaFormIdentifierDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.metaform_guid));
            CreateMap<MetaForm, MetaFormDetailDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.metaform_guid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"));

            CreateMap<MetaPage, MetaPageIdentifierDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.metapage_guid));
            CreateMap<MetaPage, MetaPageDetailDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.metapage_guid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.IsVisible ? "Yes" : "No"));
            CreateMap<MetaPage, MetaPageExpandedDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.metapage_guid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.IsVisible ? "Yes" : "No"));

            CreateMap<MetaTable, MetaTableIdentifierDto>()
                .ForMember(dest => dest.MetaTableGuid, opt => opt.MapFrom(src => src.metatable_guid));
            CreateMap<MetaTable, MetaTableDetailDto>()
                .ForMember(dest => dest.MetaTableGuid, opt => opt.MapFrom(src => src.metatable_guid))
                .ForMember(dest => dest.TableType, opt => opt.MapFrom(src => src.TableType.Description));

            CreateMap<MetaWidget, MetaWidgetIdentifierDto>()
                .ForMember(dest => dest.MetaWidgetGuid, opt => opt.MapFrom(src => src.metawidget_guid));
            CreateMap<MetaWidget, MetaWidgetDetailDto>()
                .ForMember(dest => dest.MetaWidgetGuid, opt => opt.MapFrom(src => src.metawidget_guid))
                .ForMember(dest => dest.WidgetType, opt => opt.MapFrom(src => src.WidgetType.Description));
        }
    }
}