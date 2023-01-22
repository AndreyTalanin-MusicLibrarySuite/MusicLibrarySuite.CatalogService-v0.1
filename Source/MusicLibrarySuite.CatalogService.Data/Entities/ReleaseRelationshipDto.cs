using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-to-release relationship.
/// </summary>
public class ReleaseRelationshipDto
{
    /// <summary>
    /// Gets or sets the principal release's unique identifier.
    /// </summary>
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the dependent release's unique identifier.
    /// </summary>
    public Guid DependentReleaseId { get; set; }

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
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the principal release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseDto? Release { get; set; }

    /// <summary>
    /// Gets or sets the dependent release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseDto? DependentRelease { get; set; }
}
