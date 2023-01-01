using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a work-to-genre relationship.
/// </summary>
public class WorkGenre
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    [Required]
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the genre's unique identifier.
    /// </summary>
    [Required]
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    [Required]
    public int Order { get; set; }
}
