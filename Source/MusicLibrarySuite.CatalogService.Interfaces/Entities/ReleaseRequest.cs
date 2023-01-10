using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release page request.
/// </summary>
public class ReleaseRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="Release.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="Release.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
