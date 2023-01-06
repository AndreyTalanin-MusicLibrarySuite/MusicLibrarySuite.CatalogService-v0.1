using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data-transfer object for a release group page request.
/// </summary>
public class ReleaseGroupRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseGroupDto.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseGroupDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
