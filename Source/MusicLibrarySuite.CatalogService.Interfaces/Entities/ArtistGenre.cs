using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents an artist-to-genre relationship.
/// </summary>
public class ArtistGenre
{
    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }

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
