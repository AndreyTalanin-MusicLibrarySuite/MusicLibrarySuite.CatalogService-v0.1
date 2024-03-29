using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release media.
/// </summary>
public class ReleaseMedia
{
    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    [Required]
    public byte MediaNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the release media's title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release media's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release media's disambiguation text.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release media's media format.
    /// </summary>
    public string? MediaFormat { get; set; }

    /// <summary>
    /// Gets or sets the release media's catalog number.
    /// </summary>
    public string? CatalogNumber { get; set; }

    /// <summary>
    /// Gets or sets the release media's ToC checksum (an 8-digit hexadecimal number used by FreeDb).
    /// </summary>
    public string? FreeDbChecksum { get; set; }

    /// <summary>
    /// Gets or sets the release media's ToC checksum (a 28-character Base64 string used by MusicBrainz).
    /// </summary>
    public string? MusicBrainzChecksum { get; set; }

    /// <summary>
    /// Gets or sets a collection of release-media-to-product relationships associated to the current release media.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseMediaToProductRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<ReleaseMediaToProductRelationship> ReleaseMediaToProductRelationships { get; set; } = new List<ReleaseMediaToProductRelationship>();

    /// <summary>
    /// Gets or sets a collection of release tracks associated to the current release media.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrack> ReleaseTrackCollection { get; set; } = new List<ReleaseTrack>();
}
