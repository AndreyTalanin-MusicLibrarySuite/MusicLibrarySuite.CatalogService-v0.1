using System;
using System.Collections.Generic;
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
    /// Gets or sets the release track's disambiguation text.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release track's International Standard Recording Code (ISRC).
    /// </summary>
    public string? InternationalStandardRecordingCode { get; set; }

    /// <summary>
    /// Gets or sets a collection of release-track-to-product relationships associated to the current release track.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseTrackToProductRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<ReleaseTrackToProductRelationship> ReleaseTrackToProductRelationships { get; set; } = new List<ReleaseTrackToProductRelationship>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-work relationships associated to the current release track.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseTrackToWorkRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<ReleaseTrackToWorkRelationship> ReleaseTrackToWorkRelationships { get; set; } = new List<ReleaseTrackToWorkRelationship>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrackArtist> ReleaseTrackArtists { get; set; } = new List<ReleaseTrackArtist>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "featured artist" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrackFeaturedArtist> ReleaseTrackFeaturedArtists { get; set; } = new List<ReleaseTrackFeaturedArtist>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "performer" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrackPerformer> ReleaseTrackPerformers { get; set; } = new List<ReleaseTrackPerformer>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "composer" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrackComposer> ReleaseTrackComposers { get; set; } = new List<ReleaseTrackComposer>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-genre relationships associated to the current release track.
    /// </summary>
    [Required]
    public ICollection<ReleaseTrackGenre> ReleaseTrackGenres { get; set; } = new List<ReleaseTrackGenre>();
}
