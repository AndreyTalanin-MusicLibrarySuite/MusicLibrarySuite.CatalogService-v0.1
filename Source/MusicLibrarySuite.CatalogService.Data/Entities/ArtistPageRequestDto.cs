using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data transfer object for an artist page request.
/// </summary>
public class ArtistPageRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="ArtistDto.Name" /> property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="ArtistDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
