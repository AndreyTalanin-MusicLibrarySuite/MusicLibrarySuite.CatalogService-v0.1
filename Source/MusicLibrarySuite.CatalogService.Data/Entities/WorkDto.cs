using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a work.
/// </summary>
public class WorkDto
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the work's title.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the work's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the work's disambiguation text.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the work's International Standard Musical Work Code (ISWC).
    /// </summary>
    [StringLength(32)]
    public string? InternationalStandardMusicalWorkCode { get; set; }

    /// <summary>
    /// Gets or sets the work's release date.
    /// </summary>
    public DateTime ReleasedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ReleasedOn" /> property contains the year only.
    /// </summary>
    public bool ReleasedOnYearOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current work is a system entity like "Unknown Work".
    /// </summary>
    public bool SystemEntity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current work is enabled.
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
    /// Gets or sets a collection of work-to-work relationships where the current work is the principal entity.
    /// </summary>
    public ICollection<WorkRelationshipDto> WorkRelationships { get; set; } = Enumerable.Empty<WorkRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-product relationships associated to the current work.
    /// </summary>
    /// <remarks>An entity of the <see cref="WorkToProductRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<WorkToProductRelationshipDto> WorkToProductRelationships { get; set; } = Enumerable.Empty<WorkToProductRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work.
    /// </summary>
    public ICollection<WorkArtistDto> WorkArtists { get; set; } = Enumerable.Empty<WorkArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "featured artist" role.
    /// </summary>
    public ICollection<WorkFeaturedArtistDto> WorkFeaturedArtists { get; set; } = Enumerable.Empty<WorkFeaturedArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "performer" role.
    /// </summary>
    public ICollection<WorkPerformerDto> WorkPerformers { get; set; } = Enumerable.Empty<WorkPerformerDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "composer" role.
    /// </summary>
    public ICollection<WorkComposerDto> WorkComposers { get; set; } = Enumerable.Empty<WorkComposerDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-genre relationships associated to the current work.
    /// </summary>
    public ICollection<WorkGenreDto> WorkGenres { get; set; } = Enumerable.Empty<WorkGenreDto>().ToList();
}
