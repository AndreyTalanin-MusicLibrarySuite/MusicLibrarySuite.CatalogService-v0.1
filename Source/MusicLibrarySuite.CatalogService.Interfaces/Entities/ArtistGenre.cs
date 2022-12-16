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

    /// <summary>
    /// Gets or sets the artist.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Artist? Artist { get; set; }

    /// <summary>
    /// Gets or sets the genre.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Genre? Genre { get; set; }
}
