using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data-transfer object for a product page request.
/// </summary>
public class ProductRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="ProductDto.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="ProductDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
