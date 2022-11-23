using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="Genre" /> entity.
/// </summary>
public class GenreDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenreDatabaseProfile" /> type.
    /// </summary>
    public GenreDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<GenreRelationship, GenreRelationshipDto>().ReverseMap();
        CreateMap<GenreRequest, GenreRequestDto>().ReverseMap();
        CreateMap<GenrePageResponse, PageResponseDto<GenreDto>>().ReverseMap();
        CreateMap<PageResponse<Genre>, PageResponseDto<GenreDto>>().ReverseMap();
    }
}
