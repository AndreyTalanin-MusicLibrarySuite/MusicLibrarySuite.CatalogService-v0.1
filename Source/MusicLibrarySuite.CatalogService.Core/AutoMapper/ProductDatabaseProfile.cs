using AutoMapper;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;
using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Core.AutoMapper;

/// <summary>
/// Represents a database-layer AutoMapper profile for the <see cref="Product" /> entity.
/// </summary>
public class ProductDatabaseProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDatabaseProfile" /> type.
    /// </summary>
    public ProductDatabaseProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<ProductRelationship, ProductRelationshipDto>().ReverseMap();
        CreateMap<ProductRequest, ProductRequestDto>().ReverseMap();
        CreateMap<ProductPageResponse, PageResponseDto<ProductDto>>().ReverseMap();
        CreateMap<PageResponse<Product>, PageResponseDto<ProductDto>>().ReverseMap();
    }
}
