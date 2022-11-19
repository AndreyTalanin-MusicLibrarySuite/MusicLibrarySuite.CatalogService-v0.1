using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

/// <summary>
/// Represents a generic page response.
/// </summary>
/// <typeparam name="T">The entitiy type.</typeparam>
public class PageResponse<T>
    where T : class
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

    /// <summary>
    /// Gets or sets the total count of available entities.
    /// </summary>
    [Required]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a collection of entities corresponding to the page configuration.
    /// </summary>
    [Required]
    public ICollection<T> Items { get; set; } = Enumerable.Empty<T>().ToList();

    /// <summary>
    /// Gets or sets a value representing the moment of time when the response was completed.
    /// </summary>
    [Required]
    public DateTimeOffset CompletedOn { get; set; }
}
