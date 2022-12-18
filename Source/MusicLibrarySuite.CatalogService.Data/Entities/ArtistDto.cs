using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data-transfer object for an artist.
/// </summary>
public class ArtistDto
{
    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the artist's name.
    /// </summary>
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the artist's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the disambiguation text in case multiple artists have the same name.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current artist is a system entity like "Unknown Artist".
    /// </summary>
    public bool SystemEntity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current artist is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value representing the moment of time when the entity was initially created.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets a value representing the moment of time when the entity was updated the last time.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset UpdatedOn { get; set; }

    /// <summary>
    /// Gets or sets a collection of artist-to-artist relationships where the current artist is the principal entity.
    /// </summary>
    public ICollection<ArtistRelationshipDto> ArtistRelationships { get; set; } = Enumerable.Empty<ArtistRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of artist-to-genre relationships associated to the current artist.
    /// </summary>
    public ICollection<ArtistGenreDto> ArtistGenres { get; set; } = Enumerable.Empty<ArtistGenreDto>().ToList();
}
