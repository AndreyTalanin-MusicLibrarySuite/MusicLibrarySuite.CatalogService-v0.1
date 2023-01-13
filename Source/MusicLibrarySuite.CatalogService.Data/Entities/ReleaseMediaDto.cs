using System;
using System.ComponentModel.DataAnnotations;

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
    /// Gets or sets the disambiguation text in case multiple release media have the same name.
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
}
