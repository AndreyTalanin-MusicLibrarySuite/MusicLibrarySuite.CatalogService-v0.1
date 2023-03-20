using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a data transfer object for a work page request.
/// </summary>
public class WorkPageRequestDto : PageRequestDto
{
    /// <summary>
    /// Gets or sets a filter value for the <see cref="WorkDto.Title" /> property.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a filter value for the <see cref="WorkDto.Enabled" /> property.
    /// </summary>
    public bool? Enabled { get; set; }
}
