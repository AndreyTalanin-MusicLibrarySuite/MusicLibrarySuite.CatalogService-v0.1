using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data-transfer object for a genre page request.
/// </summary>
public class GenreRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="GenreDto.Name" /> property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="GenreDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
