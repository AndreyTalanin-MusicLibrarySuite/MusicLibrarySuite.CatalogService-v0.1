using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a genre page request.
/// </summary>
public class GenrePageRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="Genre.Name" /> property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="Genre.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
