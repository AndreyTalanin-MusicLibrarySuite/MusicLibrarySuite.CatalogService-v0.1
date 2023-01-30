using System;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-track-to-artist relationship.
/// </summary>
public class ReleaseTrackArtistDto
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
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the release track.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseTrackDto? ReleaseTrack { get; set; }

    /// <summary>
    /// Gets or sets the artist.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ArtistDto? Artist { get; set; }
}
