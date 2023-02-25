using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release media.
/// </summary>
public class ReleaseMediaDto
{
    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    public byte MediaNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the release media's title.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release media's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release media's disambiguation text.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release media's catalog number.
    /// </summary>
    [StringLength(32)]
    public string? CatalogNumber { get; set; }

    /// <summary>
    /// Gets or sets the release media's media format.
    /// </summary>
    [StringLength(256)]
    public string? MediaFormat { get; set; }

    /// <summary>
    /// Gets or sets the release media's ToC checksum (an 8-digit hexadecimal number used by CDDB).
    /// </summary>
    [StringLength(64)]
    public string? TableOfContentsChecksum { get; set; }

    /// <summary>
    /// Gets or sets the release media's ToC checksum (a 28-character Base64 string used by MusicBrainz).
    /// </summary>
    [StringLength(64)]
    public string? TableOfContentsChecksumLong { get; set; }

    /// <summary>
    /// Gets or sets a collection of release tracks associated to the current release media.
    /// </summary>
    public ICollection<ReleaseTrackDto> ReleaseTrackCollection { get; set; } = Enumerable.Empty<ReleaseTrackDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-media-to-product relationships associated to the current release media.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseMediaToProductRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<ReleaseMediaToProductRelationshipDto> ReleaseMediaToProductRelationships { get; set; } = Enumerable.Empty<ReleaseMediaToProductRelationshipDto>().ToList();
}
