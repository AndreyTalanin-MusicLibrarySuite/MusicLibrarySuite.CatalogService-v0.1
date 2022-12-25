using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a work page request.
/// </summary>
public class WorkRequest : PageRequest
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="Work.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="Work.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
