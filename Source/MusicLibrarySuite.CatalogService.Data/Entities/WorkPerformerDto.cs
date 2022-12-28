using System;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a work-to-artist relationship where the artist has the "performer" role.
/// </summary>
public class WorkPerformerDto
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }
}
