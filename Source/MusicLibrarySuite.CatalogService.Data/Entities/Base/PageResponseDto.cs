using System.Collections.Generic;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities.Base;

/// <summary>
/// Represents a data-transfer object for a generic page response.
/// </summary>
/// <typeparam name="T">The entitiy type.</typeparam>
public class PageResponseDto<T>
    where T : class
{
    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the page index.
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the total count of available entities.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a collection of entities corresponding to the page configuration.
    /// </summary>
    public ICollection<T> Items { get; set; } = Enumerable.Empty<T>().ToList();
}
