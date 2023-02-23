using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release.
/// </summary>
public class ReleaseDto
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the release's title.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release's disambiguation text.
    /// </summary>
    [StringLength(2048)]
    public string? DisambiguationText { get; set; }

    /// <summary>
    /// Gets or sets the release's barcode.
    /// </summary>
    [StringLength(32)]
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the release's catalog number.
    /// </summary>
    [StringLength(32)]
    public string? CatalogNumber { get; set; }

    /// <summary>
    /// Gets or sets the release's media format.
    /// </summary>
    [StringLength(256)]
    public string? MediaFormat { get; set; }

    /// <summary>
    /// Gets or sets the release's publish format.
    /// </summary>
    [StringLength(256)]
    public string? PublishFormat { get; set; }

    /// <summary>
    /// Gets or sets the release's release date.
    /// </summary>
    public DateTime ReleasedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ReleasedOn" /> property contains the year only.
    /// </summary>
    public bool ReleasedOnYearOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current release is enabled.
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
    /// Gets or sets a collection of release media associated to the current release.
    /// </summary>
    public ICollection<ReleaseMediaDto> ReleaseMediaCollection { get; set; } = Enumerable.Empty<ReleaseMediaDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-release relationships where the current release is the principal entity.
    /// </summary>
    public ICollection<ReleaseRelationshipDto> ReleaseRelationships { get; set; } = Enumerable.Empty<ReleaseRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-product relationships associated to the current release.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseToProductRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<ReleaseToProductRelationshipDto> ReleaseToProductRelationships { get; set; } = Enumerable.Empty<ReleaseToProductRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-release-group relationships associated to the current release.
    /// </summary>
    /// <remarks>An entity of the <see cref="ReleaseToReleaseGroupRelationshipDto" /> type has a display order on each end of the relationship.</remarks>
    public ICollection<ReleaseToReleaseGroupRelationshipDto> ReleaseToReleaseGroupRelationships { get; set; } = Enumerable.Empty<ReleaseToReleaseGroupRelationshipDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release.
    /// </summary>
    public ICollection<ReleaseArtistDto> ReleaseArtists { get; set; } = Enumerable.Empty<ReleaseArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "featured artist" role.
    /// </summary>
    public ICollection<ReleaseFeaturedArtistDto> ReleaseFeaturedArtists { get; set; } = Enumerable.Empty<ReleaseFeaturedArtistDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "performer" role.
    /// </summary>
    public ICollection<ReleasePerformerDto> ReleasePerformers { get; set; } = Enumerable.Empty<ReleasePerformerDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "composer" role.
    /// </summary>
    public ICollection<ReleaseComposerDto> ReleaseComposers { get; set; } = Enumerable.Empty<ReleaseComposerDto>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-genre relationships associated to the current release.
    /// </summary>
    public ICollection<ReleaseGenreDto> ReleaseGenres { get; set; } = Enumerable.Empty<ReleaseGenreDto>().ToList();
}
