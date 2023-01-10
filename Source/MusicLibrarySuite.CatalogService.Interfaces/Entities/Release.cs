using System;
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
}
