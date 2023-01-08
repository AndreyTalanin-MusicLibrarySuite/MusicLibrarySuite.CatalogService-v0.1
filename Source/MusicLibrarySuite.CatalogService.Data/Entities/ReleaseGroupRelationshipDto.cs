using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-group-to-release-group relationship.
/// </summary>
public class ReleaseGroupRelationshipDto
{
    /// <summary>
    /// Gets or sets the principal release group's unique identifier.
    /// </summary>
    public Guid ReleaseGroupId { get; set; }

    /// <summary>
    /// Gets or sets the dependent release group's unique identifier.
    /// </summary>
    public Guid DependentReleaseGroupId { get; set; }

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
    /// Gets or sets the principal release group.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseGroupDto? ReleaseGroup { get; set; }

    /// <summary>
    /// Gets or sets the dependent release group.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseGroupDto? DependentReleaseGroup { get; set; }
}
