using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-track-to-work relationship.
/// </summary>
public class ReleaseTrackToWorkRelationshipDto
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
    /// Gets or sets the work's unique identifier.
    /// </summary>
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order for the entity acting as a relationship owner (release track).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order used when requesting relationships for a work.
    /// </summary>
    public int ReferenceOrder { get; set; }

    /// <summary>
    /// Gets or sets the release track.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseTrackDto? ReleaseTrack { get; set; }

    /// <summary>
    /// Gets or sets the work.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public WorkDto? Work { get; set; }
}
