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
    /// Gets or sets the work.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Work? Work { get; set; }

    /// <summary>
    /// Gets or sets the genre.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Genre? Genre { get; set; }
}
