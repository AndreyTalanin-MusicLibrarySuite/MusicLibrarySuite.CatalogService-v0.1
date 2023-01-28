using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-release-group relationship.
/// </summary>
public class ReleaseToReleaseGroupRelationship
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the release group's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseGroupId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Release? Release { get; set; }

    /// <summary>
    /// Gets or sets the release group.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseGroup? ReleaseGroup { get; set; }
}
