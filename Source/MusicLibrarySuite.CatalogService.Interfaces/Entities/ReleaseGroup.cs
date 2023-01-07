using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release group.
/// </summary>
public class ReleaseGroup
{
    /// <summary>
    /// Gets or sets the release group's unique identifier.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the release group's title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release group's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the disambiguation text in case multiple release groups have the same name.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current release group is enabled.
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value representing the moment of time when the entity was initially created.
    /// </summary>
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets a value representing the moment of time when the entity was updated the last time.
    /// </summary>
    public DateTimeOffset UpdatedOn { get; set; }
}
