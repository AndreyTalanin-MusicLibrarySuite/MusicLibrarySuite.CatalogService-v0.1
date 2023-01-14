using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="Release" /> entity.
/// </summary>
public class ReleaseDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseDatabaseProfile" /> type.
    /// </summary>
    public ReleaseDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Release, ReleaseDto>().ReverseMap();
        CreateMap<ReleaseMedia, ReleaseMediaDto>().ReverseMap();
        CreateMap<ReleaseTrack, ReleaseTrackDto>().ReverseMap();
        CreateMap<ReleaseRequest, ReleaseRequestDto>().ReverseMap();
        CreateMap<ReleasePageResponse, PageResponseDto<ReleaseDto>>().ReverseMap();
        CreateMap<PageResponse<Release>, PageResponseDto<ReleaseDto>>().ReverseMap();
    }
}
