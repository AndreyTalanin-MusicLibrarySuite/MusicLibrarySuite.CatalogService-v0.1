using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

/// <summary>
/// Represents a base page request.
/// </summary>
public class PageRequest
{
    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    [Required]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the page index.
    /// </summary>
    [Required]
    public int PageIndex { get; set; }
}
