using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
    /// Gets or sets the disambiguation text in case multiple releases have the same name.
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
    /// Gets or sets a collection of release-to-release relationships where the current release is the principal entity.
    /// </summary>
    [Required]
    public ICollection<ReleaseRelationship> ReleaseRelationships { get; set; } = Enumerable.Empty<ReleaseRelationship>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseArtist> ReleaseArtists { get; set; } = Enumerable.Empty<ReleaseArtist>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "featured artist" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseFeaturedArtist> ReleaseFeaturedArtists { get; set; } = Enumerable.Empty<ReleaseFeaturedArtist>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "performer" role.
    /// </summary>
    [Required]
    public ICollection<ReleasePerformer> ReleasePerformers { get; set; } = Enumerable.Empty<ReleasePerformer>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-artist relationships associated to the current release where the artist has the "composer" role.
    /// </summary>
    [Required]
    public ICollection<ReleaseComposer> ReleaseComposers { get; set; } = Enumerable.Empty<ReleaseComposer>().ToList();

    /// <summary>
    /// Gets or sets a collection of release-to-genre relationships associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseGenre> ReleaseGenres { get; set; } = Enumerable.Empty<ReleaseGenre>().ToList();

    /// <summary>
    /// Gets or sets a collection of release media associated to the current release.
    /// </summary>
    [Required]
    public ICollection<ReleaseMedia> ReleaseMediaCollection { get; set; } = Enumerable.Empty<ReleaseMedia>().ToList();
}
