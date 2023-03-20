using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a work.
/// </summary>
public class Work
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the work's title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the work's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the work's disambiguation text.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the work's International Standard Musical Work Code (ISWC).
    /// </summary>
    public string? InternationalStandardMusicalWorkCode { get; set; }

    /// <summary>
    /// Gets or sets the work's release date.
    /// </summary>
    [Required]
    public DateTime ReleasedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ReleasedOn" /> property contains the year only.
    /// </summary>
    [Required]
    public bool ReleasedOnYearOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current work is a system entity like "Unknown Work".
    /// </summary>
    [Required]
    public bool SystemEntity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current work is enabled.
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

    /// <summary>
    /// Gets or sets a collection of work-to-work relationships where the current work is the principal entity.
    /// </summary>
    [Required]
    public ICollection<WorkRelationship> WorkRelationships { get; set; } = Enumerable.Empty<WorkRelationship>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-product relationships associated to the current work.
    /// </summary>
    /// <remarks>An entity of the <see cref="WorkToProductRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<WorkToProductRelationship> WorkToProductRelationships { get; set; } = Enumerable.Empty<WorkToProductRelationship>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work.
    /// </summary>
    [Required]
    public ICollection<WorkArtist> WorkArtists { get; set; } = Enumerable.Empty<WorkArtist>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "featured artist" role.
    /// </summary>
    [Required]
    public ICollection<WorkFeaturedArtist> WorkFeaturedArtists { get; set; } = Enumerable.Empty<WorkFeaturedArtist>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "performer" role.
    /// </summary>
    [Required]
    public ICollection<WorkPerformer> WorkPerformers { get; set; } = Enumerable.Empty<WorkPerformer>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-artist relationships associated to the current work where the artist has the "composer" role.
    /// </summary>
    [Required]
    public ICollection<WorkComposer> WorkComposers { get; set; } = Enumerable.Empty<WorkComposer>().ToList();

    /// <summary>
    /// Gets or sets a collection of work-to-genre relationships associated to the current work.
    /// </summary>
    [Required]
    public ICollection<WorkGenre> WorkGenres { get; set; } = Enumerable.Empty<WorkGenre>().ToList();
}
