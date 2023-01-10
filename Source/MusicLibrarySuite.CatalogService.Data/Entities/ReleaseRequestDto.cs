using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data-transfer object for a release page request.
/// </summary>
public class ReleaseRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseDto.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
