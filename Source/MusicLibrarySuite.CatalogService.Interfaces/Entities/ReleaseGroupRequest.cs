using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release group page request.
/// </summary>
public class ReleaseGroupRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseGroup.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="ReleaseGroup.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
