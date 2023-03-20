using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a product page request.
/// </summary>
public class ProductPageRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="Product.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="Product.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
