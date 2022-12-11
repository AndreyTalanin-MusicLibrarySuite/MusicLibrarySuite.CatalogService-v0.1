using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="Artist" /> entity.
/// </summary>
public class ArtistDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistDatabaseProfile" /> type.
    /// </summary>
    public ArtistDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Artist, ArtistDto>().ReverseMap();
        CreateMap<ArtistRequest, ArtistRequestDto>().ReverseMap();
        CreateMap<ArtistPageResponse, PageResponseDto<ArtistDto>>().ReverseMap();
        CreateMap<PageResponse<Artist>, PageResponseDto<ArtistDto>>().ReverseMap();
    }
}
