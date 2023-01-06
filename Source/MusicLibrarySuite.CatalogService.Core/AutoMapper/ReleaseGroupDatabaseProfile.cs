using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="ReleaseGroup" /> entity.
/// </summary>
public class ReleaseGroupDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseGroupDatabaseProfile" /> type.
    /// </summary>
    public ReleaseGroupDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<ReleaseGroup, ReleaseGroupDto>().ReverseMap();
        CreateMap<ReleaseGroupRequest, ReleaseGroupRequestDto>().ReverseMap();
        CreateMap<ReleaseGroupPageResponse, PageResponseDto<ReleaseGroupDto>>().ReverseMap();
        CreateMap<PageResponse<ReleaseGroup>, PageResponseDto<ReleaseGroupDto>>().ReverseMap();
    }
}
