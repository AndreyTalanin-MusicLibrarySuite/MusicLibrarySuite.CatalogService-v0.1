using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release track.
/// </summary>
public class ReleaseTrack
{
    /// <summary>
    /// Gets or sets the release track's number.
    /// </summary>
    [Required]
    public byte TrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the release media's total track count.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public short TotalTrackCount { get; set; }

    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    [Required]
    public byte MediaNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's total media count.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public short TotalMediaCount { get; set; }

    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the release track's title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release track's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the disambiguation text in case multiple release tracks have the same name.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release track's International Standard Recording Code (ISRC).
    /// </summary>
    public string? InternationalStandardRecordingCode { get; set; }
}