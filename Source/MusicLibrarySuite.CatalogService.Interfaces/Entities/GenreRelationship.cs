using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a genre-to-genre relationship.
/// </summary>
public class GenreRelationship
{
    /// <summary>
    /// Gets or sets the principal genre's unique identifier.
    /// </summary>
    [Required]
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the dependent genre's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentGenreId { get; set; }

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
