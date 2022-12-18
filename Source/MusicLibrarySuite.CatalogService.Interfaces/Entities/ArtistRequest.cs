using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents an artist page request.
/// </summary>
public class ArtistRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="Artist.Name" /> property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="Artist.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
