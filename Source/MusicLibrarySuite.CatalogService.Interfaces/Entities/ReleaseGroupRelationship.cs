using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-group-to-release-group relationship.
/// </summary>
public class ReleaseGroupRelationship
{
    /// <summary>
    /// Gets or sets the principal release group's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseGroupId { get; set; }

    /// <summary>
    /// Gets or sets the dependent release group's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentReleaseGroupId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    public string? Description { get; set; }
}
