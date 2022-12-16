using System;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for an artist-to-genre relationship.
/// </summary>
public class ArtistGenreDto
{
    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the genre's unique identifier.
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }
}
