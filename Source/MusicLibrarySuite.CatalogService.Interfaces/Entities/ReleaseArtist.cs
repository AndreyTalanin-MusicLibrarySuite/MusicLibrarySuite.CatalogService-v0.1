using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-artist relationship.
/// </summary>
public class ReleaseArtist
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Release? Release { get; set; }

    /// <summary>
    /// Gets or sets the artist.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Artist? Artist { get; set; }
}
