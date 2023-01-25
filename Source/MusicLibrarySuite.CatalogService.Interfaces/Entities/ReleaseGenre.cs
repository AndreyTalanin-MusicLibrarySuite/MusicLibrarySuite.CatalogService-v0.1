using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-genre relationship.
/// </summary>
public class ReleaseGenre
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the genre's unique identifier.
    /// </summary>
    [Required]
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Release? Release { get; set; }

    /// <summary>
    /// Gets or sets the genre.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Genre? Genre { get; set; }
}
