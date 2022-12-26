using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a work-to-work relationship.
/// </summary>
public class WorkRelationship
{
    /// <summary>
    /// Gets or sets the principal work's unique identifier.
    /// </summary>
    [Required]
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the dependent work's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentWorkId { get; set; }

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
    /// Gets or sets the principal work.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Work? Work { get; set; }

    /// <summary>
    /// Gets or sets the dependent work.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Work? DependentWork { get; set; }
}
