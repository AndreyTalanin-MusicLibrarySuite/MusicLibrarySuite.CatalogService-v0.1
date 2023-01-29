using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release track.
/// </summary>
public class ReleaseTrackDto
{
    /// <summary>
    /// Gets or sets the release track's number.
    /// </summary>
    public byte TrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    public byte MediaNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the release track's title.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release track's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the disambiguation text in case multiple release tracks have the same name.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release track's International Standard Recording Code (ISRC).
    /// </summary>
    [StringLength(32)]
    public string? InternationalStandardRecordingCode { get; set; }

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track.
    /// </summary>
    public ICollection<ReleaseTrackArtistDto> ReleaseTrackArtists { get; set; } = Enumerable.Empty<ReleaseTrackArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "featured artist" role.
    /// </summary>
    public ICollection<ReleaseTrackFeaturedArtistDto> ReleaseTrackFeaturedArtists { get; set; } = Enumerable.Empty<ReleaseTrackFeaturedArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "performer" role.
    /// </summary>
    public ICollection<ReleaseTrackPerformerDto> ReleaseTrackPerformers { get; set; } = Enumerable.Empty<ReleaseTrackPerformerDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "composer" role.
    /// </summary>
    public ICollection<ReleaseTrackComposerDto> ReleaseTrackComposers { get; set; } = Enumerable.Empty<ReleaseTrackComposerDto>().ToList();
}
