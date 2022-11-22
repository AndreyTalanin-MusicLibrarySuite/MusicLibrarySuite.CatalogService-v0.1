using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a genre-to-genre relationship.
/// </summary>
public class GenreRelationshipDto
{
    /// <summary>
    /// Gets or sets the principal genre's unique identifier.
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Gets or sets the dependent genre's unique identifier.
    /// </summary>
    public Guid DependentGenreId { get; set; }

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
    /// Gets or sets the principal genre.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public GenreDto? Genre { get; set; }

    /// <summary>
    /// Gets or sets the dependent genre.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public GenreDto? DependentGenre { get; set; }
}
