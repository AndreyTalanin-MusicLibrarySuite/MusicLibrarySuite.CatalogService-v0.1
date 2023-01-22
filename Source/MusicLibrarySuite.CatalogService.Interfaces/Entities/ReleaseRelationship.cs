using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-release relationship.
/// </summary>
public class ReleaseRelationship
{
    /// <summary>
    /// Gets or sets the principal release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the dependent release's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentReleaseId { get; set; }

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
    /// Gets or sets the principal release.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Release? Release { get; set; }

    /// <summary>
    /// Gets or sets the dependent release.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Release? DependentRelease { get; set; }
}
