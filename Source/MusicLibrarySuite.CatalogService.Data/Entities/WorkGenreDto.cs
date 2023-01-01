using System;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a work-to-genre relationship.
/// </summary>
public class WorkGenreDto
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the genre's unique identifier.
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }
}
