using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="Work" /> entity.
/// </summary>
public class WorkDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkDatabaseProfile" /> type.
    /// </summary>
    public WorkDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Work, WorkDto>().ReverseMap();
        CreateMap<WorkRequest, WorkRequestDto>().ReverseMap();
        CreateMap<WorkPageResponse, PageResponseDto<WorkDto>>().ReverseMap();
        CreateMap<PageResponse<Work>, PageResponseDto<WorkDto>>().ReverseMap();
    }
}
