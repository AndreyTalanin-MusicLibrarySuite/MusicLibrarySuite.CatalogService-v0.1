namespace MusicLibrarySuite.CatalogService.Data.Entities.Base;

/// <summary>
/// Represents a data transfer object for a base page request.
/// </summary>
public class PageRequestDto
{
    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the page index.
    /// </summary>
    public int PageIndex { get; set; }
}
