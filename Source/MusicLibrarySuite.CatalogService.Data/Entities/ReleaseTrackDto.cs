using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    /// Gets or sets the release track's disambiguation text.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release track's International Standard Recording Code (ISRC).
    /// </summary>
    [StringLength(32)]
    public string? InternationalStandardRecordingCode { get; set; }

    /// <summary>
    /// Gets or sets a collection of release-track-to-product relationships associated to the current release track.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseTrackToProductRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<ReleaseTrackToProductRelationshipDto> ReleaseTrackToProductRelationships { get; set; } = new List<ReleaseTrackToProductRelationshipDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-work relationships associated to the current release track.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseTrackToWorkRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<ReleaseTrackToWorkRelationshipDto> ReleaseTrackToWorkRelationships { get; set; } = new List<ReleaseTrackToWorkRelationshipDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track.
    /// </summary>
    public ICollection<ReleaseTrackArtistDto> ReleaseTrackArtists { get; set; } = new List<ReleaseTrackArtistDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "featured artist" role.
    /// </summary>
    public ICollection<ReleaseTrackFeaturedArtistDto> ReleaseTrackFeaturedArtists { get; set; } = new List<ReleaseTrackFeaturedArtistDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "performer" role.
    /// </summary>
    public ICollection<ReleaseTrackPerformerDto> ReleaseTrackPerformers { get; set; } = new List<ReleaseTrackPerformerDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-artist relationships associated to the current release track where the artist has the "composer" role.
    /// </summary>
    public ICollection<ReleaseTrackComposerDto> ReleaseTrackComposers { get; set; } = new List<ReleaseTrackComposerDto>();

    /// <summary>
    /// Gets or sets a collection of release-track-to-genre relationships associated to the current release track.
    /// </summary>
    public ICollection<ReleaseTrackGenreDto> ReleaseTrackGenres { get; set; } = new List<ReleaseTrackGenreDto>();
}
