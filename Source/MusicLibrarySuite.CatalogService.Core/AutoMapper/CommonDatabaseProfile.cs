using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for commonly-used entities.
/// </summary>
public class CommonDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonDatabaseProfile" /> type.
    /// </summary>
    public CommonDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<PageRequest, PageRequestDto>().ReverseMap();
    }
}
