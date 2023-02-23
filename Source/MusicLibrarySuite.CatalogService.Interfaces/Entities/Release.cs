using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release.
/// </summary>
public class Release
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the release's title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release's disambiguation text.
    /// </summary>
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release's barcode.
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the release's catalog number.
    /// </summary>
    public string? CatalogNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's media format.
    /// </summary>
    public string? MediaFormat { get; set; }

    /// <summary>
    /// Gets or sets the release's publish format.
    /// </summary>
    public string? PublishFormat { get; set; }

    /// <summary>
    /// Gets or sets the release's release date.
    /// </summary>
    [Required]
    public DateTime ReleasedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ReleasedOn" /> property contains the year only.
    /// </summary>
    [Required]
    public bool ReleasedOnYearOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current release is enabled.
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
    /// Gets or sets a collection of release media associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseMedia> ReleaseMediaCollection { get; set; } = new List<ReleaseMedia>();

    /// <summary>
    /// Gets or sets a collection of release-to-release relationships where the current release is the principal entity.
    /// </summary>
    [Required]
    public ICollection<ReleaseRelationship> ReleaseRelationships { get; set; } = new List<ReleaseRelationship>();

    /// <summary>
    /// Gets or sets a collection of release-to-product relationships associated to the current release.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseToProductRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<ReleaseToProductRelationship> ReleaseToProductRelationships { get; set; } = new List<ReleaseToProductRelationship>();

    /// <summary>
    /// Gets or sets a collection of release-to-release-group relationships associated to the current release.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseToReleaseGroupRelationship" /> type has a display order on each end of the relationship.</remarks>
    [Required]
    public ICollection<ReleaseToReleaseGroupRelationship> ReleaseToReleaseGroupRelationships { get; set; } = new List<ReleaseToReleaseGroupRelationship>();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseArtist> ReleaseArtists { get; set; } = new List<ReleaseArtist>();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "featured artist" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseFeaturedArtist> ReleaseFeaturedArtists { get; set; } = new List<ReleaseFeaturedArtist>();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "performer" role.
    /// </summary>
    [Required]
    public ICollection<ReleasePerformer> ReleasePerformers { get; set; } = new List<ReleasePerformer>();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "composer" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseComposer> ReleaseComposers { get; set; } = new List<ReleaseComposer>();

    /// <summary>
    /// Gets or sets a collection of release-to-genre relationships associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseGenre> ReleaseGenres { get; set; } = new List<ReleaseGenre>();
}
